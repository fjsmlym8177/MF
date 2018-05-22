using Autofac;
using MF.Core.Configuration;
using MF.Core.Infrastructure;
using MF.Core.Infrastructure.DependencyManagement;
using MF.Core.Logging;
using MF.Core.Rabbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.DelayMessageHandler
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, MikeConfig config)
        {
            var setting = ConfigLoader.LoadConfig<Config>();

            builder.Register<IRabbitContext>(c => new RabbitContext(setting.MQConn, "MF.DelayMessageHandler", new List<string> { "MF.DelayMessageHandler" })).SingleInstance();

            builder.RegisterType<NLogger>().As<ILogger>().SingleInstance();
        }
    }
}
