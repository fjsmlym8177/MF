using MF.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.DelayMessageHandler
{
    public class Config : IRedisConfig
    {
        public string MQConn { get; set; }

        public int MQThread { get; set; }

        public string RedisConn { get; set; }
    }
}
