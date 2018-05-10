using MF.Core.Rabbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHost.Infrastructure
{
    public class Class1 : IMessageEventHandler
    {
        public string EventName => "mike.test_lym";

        public void Handler(string data)
        {
            throw new NotImplementedException();
        }
    }
}