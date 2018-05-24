using MF.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MF.Core.Configuration;
using MF.Core.Infrastructure;
using Autofac;
using Autofac.Integration.WebApi;
using MF.Core.Logging;
using MF.Core.Rabbit;
using MF.Data;
using MF.Core.Redis;
using MF.Core.Lock;
using MF.Core.Pipeline;

namespace WebHost.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.Register<IDbContext<Guid>>(c => new WebHostDbContext(new List<string> { "WebHost" }, "name=MyContext")).InstancePerRequest();

            builder.RegisterGeneric(typeof(EfRepository<,>)).As(typeof(IRepository<,>)).SingleInstance();

            builder.Register<IRabbitContext>(c => new RabbitContext("amqp://admin:123456@192.168.0.145:5672", "WebHost", new List<string> { "WebHost" })).SingleInstance();

            builder.Register<IPipelineContext>(c => new RedisPipelineContext(new RedisManager("192.168.0.145"), new RedisPipelineConfig())).SingleInstance();


            //builder.RegisterType<NLogger>().As<ILogger>().SingleInstance();


            //var redisManager = new RedisManager(config.RedisConnection);
            //builder.Register(p => redisManager).SingleInstance();

            //builder.Register<ILockManager>(p => new RedisLockManager(redisManager, config.RedisLockDB)).SingleInstance();

            //builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray()).InstancePerRequest();
        }
    }
}