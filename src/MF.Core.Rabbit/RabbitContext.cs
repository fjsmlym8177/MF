using MF.Core.Infrastructure;
using MF.Core.Logging;
using MF.Core.Rabbit;
using MF.Core.Redis;
using MF.Core.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public class RabbitContext : IRabbitContext
    {
        private IConnection _connection;
        private string _name;
        private IList<string> _assemblyNames;
        private readonly Thread _sendMsgThread = null;
        private readonly AutoResetEvent EventData = new AutoResetEvent(false);
        private IDictionary<string, IList<Thread>> _subTasks = new Dictionary<string, IList<Thread>>();
        private readonly ConcurrentQueue<Tuple<string, string, RabbitMQMessage<Object>>> DataQueue = new ConcurrentQueue<Tuple<string, string, RabbitMQMessage<Object>>>();


        public RabbitContext(string address, string name, IList<string> assemblyNames)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("RabbitMQ服务地址没有配置！");
            _name = name;
            _assemblyNames = assemblyNames;

            var conFactory = new ConnectionFactory() { Uri = new Uri(address), AutomaticRecoveryEnabled = true };
            try
            {
                _connection = conFactory.CreateConnection(name);

                if (_sendMsgThread != null)
                    _sendMsgThread.Abort();
                _sendMsgThread = new Thread(SendData) { IsBackground = true };
                _sendMsgThread.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("RabbitMQ服务初始化失败！");
            }
        }

        public void Publish(string exchengeName, string routeKey, string type, object dataPack)
        {
            var message = new RabbitMQMessage<Object>()
            {
                Data = dataPack,
                Id = Guid.NewGuid(),
                PushTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                PushName = _name,
                Type = type
            };
            var tuple = new Tuple<string, string, RabbitMQMessage<Object>>(exchengeName, routeKey, message);
            DataQueue.Enqueue(tuple);
            EventData.Set();
        }

        private void SendData(Object obj)
        {
            var channel = _connection.CreateModel();
            Tuple<string, string, RabbitMQMessage<Object>> tuple = null;
            while (true)
            {
                try
                {
                    EventData.WaitOne();
                    if (!DataQueue.TryDequeue(out tuple))
                        continue;
                    var message = tuple.Item3.ToJson();
                    var binData = Encoding.UTF8.GetBytes(message);

                    if (tuple.Item3 is RabbitMQDelayMessage<Object>)
                    {
                        //IBasicProperties properties = channel.CreateBasicProperties();
                        //properties.Expiration 
                        var expiration = (tuple.Item3 as RabbitMQDelayMessage<Object>).Expiration;

                        //channel.BasicPublish("Delay", "in", properties, binData);

                        //"MQDelayHash";
                        var db = EngineContext.Current.Resolve<IRedisConnectionWrapper>().GetDatabase();

                        var key = $"DelayMessage:{ Guid.NewGuid()}";
                        db.HashSet("MQDelayHash", key, message);
                        db.StringSet(key, Guid.NewGuid().ToString(), TimeSpan.FromMilliseconds(expiration));

                    }
                    else
                    {
                        channel.BasicPublish(tuple.Item1, tuple.Item2, null, binData);
                    }


                    EngineContext.Current.Resolve<ILogger>().InsertQueueLog("Send", string.Format("消息发送成功：exchange_routingKey：{0}{1}message：{2}", tuple.Item1 + "" + tuple.Item2, Environment.NewLine, message));
                }
                catch (Exception ex)
                {
                    EngineContext.Current.Resolve<ILogger>().InsertQueueLog("Send", "消息发送失败" + ex.Message + obj.ToJson());
                    Thread.Sleep(100);
                }
                finally
                {
                    if (DataQueue.Count > 0)
                        EventData.Set();
                }
            }
        }

        public void AutoRegister(string queueName, int taskNumber = 1)
        {
            AddQueueTask(queueName, (sender, e) =>
             {
                 var checkName = string.Format("{0}_{1}", e.Exchange, e.RoutingKey);
                 var message = Encoding.UTF8.GetString(e.Body).ToDeserialize<RabbitMQMessage<Object>>();

                 MessageHandler.Execute(_assemblyNames, new ExecuteMessage()
                 {
                     Type = message.Type,
                     Data = message.Data.ToString()
                 });

             }, taskNumber);
        }

        public void AddQueueTask(string queueName, Action<object, BasicDeliverEventArgs> action, int taskNumber = 1)
        {
            if (_subTasks.Any(p => p.Key == queueName))
                throw new Exception("一个队列只能有一个线程解析！");

            if (taskNumber <= 0)
                taskNumber = 1;

            var threads = new List<Thread>();

            for (int i = 0; i < taskNumber; i++)
            {
                Thread thread = new Thread(() =>
                {
                    var channel = _connection.CreateModel();

                    channel.BasicQos(0, 1, false);//告诉broker同一时间只处理一个消息

                    var consurmer = new EventingBasicConsumer(channel);
                    consurmer.Received += (sender, e) =>
                    {

                        //Task.Run(() =>
                        //{
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();


                        var consumer = sender as EventingBasicConsumer;
                        try
                        {
                            action(sender, e);
                        }
                        catch (Exception ex)
                        {
                            EngineContext.Current.Resolve<ILogger>().InsertQueueLog("HandlerError", Encoding.UTF8.GetString(e.Body) + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            //处理完成，告诉Broker可以服务端可以删除消息，分配新的消息过来
                            channel.BasicAck(e.DeliveryTag, false);
                            stopwatch.Stop();
                            EngineContext.Current.Resolve<ILogger>().InsertQueueLog("Receive", $"处理耗时：{stopwatch.Elapsed.TotalMilliseconds}," + Encoding.UTF8.GetString(e.Body));
                        }
                        //});
                    };

                    //noAck设置false,告诉broker，发送消息之后，消息暂时不要删除，等消费者处理完成再说
                    channel.BasicConsume(queueName, false, consurmer);
                });

                threads.Add(thread);
            }

            _subTasks.Add(queueName, threads);

            foreach (var thread in threads)
                thread.Start();

        }

        public void PublishDelay(string exchengeName, string routeKey, string type, object dataPack, int expiration = 0)
        {
            if (expiration <= 0)
                return;

            var message = new RabbitMQDelayMessage<Object>()
            {
                Data = dataPack,
                Id = Guid.NewGuid(),
                PushTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                PushName = _name,
                Type = type,
                NextExchange = exchengeName,
                Expiration = expiration,
                NextRouteKey = routeKey
            };
            var tuple = new Tuple<string, string, RabbitMQMessage<Object>>("", "", message);
            DataQueue.Enqueue(tuple);
            EventData.Set();
        }
    }
}
