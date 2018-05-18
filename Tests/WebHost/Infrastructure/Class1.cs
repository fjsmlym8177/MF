using MF.Core.Rabbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHost.Infrastructure
{
    public class EventDataHandler : IMessageEventHandler<EventData>
    {
        public string EventName => "1";

        public void Handler(EventData data)
        {
            Console.WriteLine("Test");
        }
    }

    public class EventData
    {
        public string Name { get; set; }

        public string Age { get; set; }
    }
}