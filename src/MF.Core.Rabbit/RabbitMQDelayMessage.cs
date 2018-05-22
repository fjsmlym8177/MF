using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public class RabbitMQDelayMessage<T> : RabbitMQMessage<T>
    {
        public int Expiration { get; set; }

        public string NextExchange { get; set; }

        public string NextRouteKey { get; set; }
    }
}
