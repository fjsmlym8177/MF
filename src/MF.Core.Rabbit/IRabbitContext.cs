using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public interface IRabbitContext
    {
        void AutoRegister(string queueName, int taskNumber = 1);

        void AddQueueTask(string queueName, Action<object, BasicDeliverEventArgs> action, int taskNumber = 1);

        void Publish(string exchengeName, string routeKey, string type, object dataPack, int expiration = 0);
    }
}
