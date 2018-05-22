using MF.Core.Configuration;
using MF.Core.Infrastructure;
using MF.Core.Rabbit;
using MF.Core.Utilities;
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

            var context = EngineContext.Current.Resolve<IRabbitContext>();

            context.AddQueueTask("DelayTransfer", (obj, eventArgs) =>
             {
                 var message = Encoding.UTF8.GetString(eventArgs.Body).ToDeserialize<RabbitMQDelayMessage<Object>>();
             
                 Console.WriteLine($"接收到延迟任务：");
                 Console.WriteLine($"NextExchange: { message.NextExchange}");
                 Console.WriteLine($"NextRouteKey:{ message.NextRouteKey}");
                 Console.WriteLine($"Type:{ message.Type}");
                 Console.WriteLine($"Body:{ message.Data.ToJson()}");

                 context.Publish(message.NextExchange, message.NextRouteKey, message.Type, message.Data);

             }, config.MQThread);



            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
