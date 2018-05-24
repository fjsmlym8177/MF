using MF.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Redis
{
    public interface IRedisConfig : IConfig
    {
        string RedisConn { get; set; }
    }
}
