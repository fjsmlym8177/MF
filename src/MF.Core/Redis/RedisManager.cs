using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Redis
{
    public class RedisManager
    {
        public ConnectionMultiplexer muxer;

        public RedisManager(string connectionString)
        {
            muxer = ConnectionMultiplexer.Connect(connectionString);
        }
    }
}
