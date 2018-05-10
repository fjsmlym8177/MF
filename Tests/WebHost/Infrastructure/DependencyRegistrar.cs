﻿using MF.Core.Infrastructure.DependencyManagement;
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

namespace WebHost.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 0;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, MikeConfig config)
        {
            builder.Register<IDbContext<Guid>>(c => new WebHostDbContext(new List<string> { "WebHost" }, "name=MyContext")).InstancePerRequest();

            builder.RegisterGeneric(typeof(EfRepository<,>)).As(typeof(IRepository<,>)).InstancePerRequest();

            //builder.Register<IRabbitContext>(c => new RabbitContext("amqp://mike:mike123@10.2.0.174:5672", "WebHost", new List<string> { "WebHost" })).InstancePerRequest();

            builder.RegisterType<NLogger>().As<ILogger>().SingleInstance();


            //var redisManager = new RedisManager(config.RedisConnection);
            //builder.Register(p => redisManager).SingleInstance();

            //builder.Register<ILockManager>(p => new RedisLockManager(redisManager, config.RedisLockDB)).SingleInstance();

            builder.RegisterApiControllers(typeFinder.GetAssemblies().ToArray()).InstancePerRequest();
        }
    }
}