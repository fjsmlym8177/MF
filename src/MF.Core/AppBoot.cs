using Autofac;
using MF.Core.Caching;
using MF.Core.Infrastructure;
using MF.Core.Logging;
using MF.Core.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MF.Core
{
    public class AppBoot
    {
        private static AppBoot _appBoot;

        public static AppBoot Instance
        {
            get
            {
                if (_appBoot==null)
                {
                    _appBoot = new AppBoot();
                }

                return _appBoot;
            }
        }

        public AppBoot UseRedis(IRedisConfig config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new RedisConnectionWrapper(config)).As<IRedisConnectionWrapper>().SingleInstance();


            builder.Update(EngineContext.Current.ContainerManager.Container);

            return this;
        }

        public AppBoot UseDeault()
        {
            EngineContext.Initialize(new MikeEngine());
            UseLog4();

            return this;
        }



        public AppBoot UseRedisCache(int db)
        {
            var builder = new ContainerBuilder();

            var redisManager = EngineContext.Current.Resolve<IRedisConnectionWrapper>();

            builder.RegisterInstance(new RedisCacheManager(redisManager, db)).As<ICacheManager>().SingleInstance();

            builder.Update(EngineContext.Current.ContainerManager.Container);

            return this;
        }

        public AppBoot UseLog4()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new NLogger()).As<ILogger>().SingleInstance();

            builder.Update(EngineContext.Current.ContainerManager.Container);

            return this;
        }


        public void RunStartupTasks()
        {
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }
    }
}