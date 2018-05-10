using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public class RabbitMQMessage<T>
    {
        public Guid Id { get; set; }

        public string PushName { get; set; }

        public string PushTime { get; set; }

        public int Expiration { get; set; }

        public T Data { get; set; }
    }
}
