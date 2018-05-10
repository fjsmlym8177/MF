using MF.Core.Infrastructure;
using MF.Core.Rabbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHost.Infrastructure
{
    public class RabbitStartupTask : IStartupTask
    {
        //private IRabbitContext _rabitContext;

        public int Order => 0;

        public void Execute()
        {
            //var rabitContext = EngineContext.Current.Resolve<IRabbitContext>();


            //rabitContext.Publish("mike.test", "lym", "test");
            //rabitContext.Publish("mike.test", "lym", "test");
            //rabitContext.Publish("mike.test", "lym", "test");
            //rabitContext.Publish("mike.test", "lym", "test");

            //rabitContext.AutoRegister("mike.testqueue",10);


        }
    }
}