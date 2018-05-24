using MF.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHost.Infrastructure
{
    public class TestConfig:IRedisConfig
    {
        public string DB { get; set; }

        public string config1 { get; set; }
        public string RedisConn { get; set; }
    }
}