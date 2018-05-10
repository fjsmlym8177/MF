using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public interface IMessageEventHandler
    {

        /// <summary>
        /// {0}_{1} exchange_routeKey
        /// </summary>
        string EventName { get; }

        void Handler(string data);
    }
}
