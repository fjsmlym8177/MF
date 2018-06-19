using MF.Core;
using MF.Core.Configuration;
using MF.Core.Infrastructure;
using MF.Core.Logging;
using MF.Core.Rabbit;
using MF.Core.Redis;
using MF.Core.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.DelayMessageHandler
{
    class Program
    {
        static void Main(string[] args)
        {
      
       


        ConfigLoader.SetConfig<Config>("appsettings.json");
            var config = ConfigLoader.LoadConfig<Config>();

            AppBoot.Instance
                   .UseDeault()
                   .UseRedis(config);

            //Task.Run(() =>
            //{


            //});



            var context = EngineContext.Current.Resolve<IRabbitContext>();

            //context.AddQueueTask("DelayTransfer", (obj, eventArgs) =>
            //{
            //    var message = Encoding.UTF8.GetString(eventArgs.Body).ToDeserialize<RabbitMQDelayMessage<Object>>();

            //    Console.WriteLine($"接收到延迟任务：");
            //    Console.WriteLine($"NextExchange: { message.NextExchange}");
            //    Console.WriteLine($"NextRouteKey:{ message.NextRouteKey}");
            //    Console.WriteLine($"Type:{ message.Type}");
            //    Console.WriteLine($"Body:{ message.Data.ToJson()}");

            //    try
            //    {
            //        context.Publish(message.NextExchange, message.NextRouteKey, message.Type, message.Data);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);id
            //    }


            //}, config.MQThread);
            var redisManger = EngineContext.Current.Resolve<IRedisConnectionWrapper>();
            var subscriber = redisManger.GetConnection().GetSubscriber();
            var db = redisManger.GetConnection().GetDatabase();
            var logger= EngineContext.Current.Resolve<ILogger>();

            Task.Run(() => {
                subscriber.Subscribe("__keyspace@0__:*", (channel, value) =>
                {

                    //__keyspace@0__:key1
                    if (value.Equals("expired"))
                    {
                        var keyName = channel.ToString().Replace("__keyspace@0__:", "");
                        Console.WriteLine($"监听到到期时间：{keyName}");

                        try
                        {
                            RedisValue value1;

                            if (db.HashExists("MQDelayHash", keyName))
                            {
                                value1 = db.HashGet("MQDelayHash", keyName);
                            }
                            else
                            {
                                value1 = db.HashGet("MQDelayHash2", keyName);
                            }

                            var message = value1.ToString().ToDeserialize<RabbitMQDelayMessage<Object>>();

                            Console.WriteLine($"接收到延迟任务：");
                            Console.WriteLine($"NextExchange: { message.NextExchange}");
                            Console.WriteLine($"NextRouteKey:{ message.NextRouteKey}");
                            Console.WriteLine($"Type:{ message.Type}");
                            Console.WriteLine($"Body:{ message.Data.ToJson()}");

                            context.Publish(message.NextExchange, message.NextRouteKey, message.Type, message.Data);                           

                            //logger.Information($"发送成功：{message.ToJson()}");
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }
                        finally
                        {
                            try
                            {
                                if (db.HashDelete("MQDelayHash", keyName) == false && db.HashDelete("MQDelayHash2", keyName) == false)
                                {
                                    logger.Warning($"{keyName},未删除");
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                            }
                        
                        }

                    }
                });

            });
                
         


            while (true)
            {
                Console.Read();
            }
        }
    }
}

