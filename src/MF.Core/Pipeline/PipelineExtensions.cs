using Autofac;
using MF.Core.Infrastructure;
using MF.Core.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Pipeline
{
    public static class PipelineExtensions
    {
        public static AppBoot UseRedisPipeline(this AppBoot boot, RedisPipelineConfig config, Action<string> logAction = null)
        {
            var builder = new ContainerBuilder();

            var redisManager = EngineContext.Current.Resolve<RedisManager>();

            builder.RegisterInstance(new RedisPipelineContext(redisManager, config, logAction)).As<IPipelineContext>().SingleInstance();

            builder.Update(EngineContext.Current.ContainerManager.Container);

            return boot;
        }
    }
}
