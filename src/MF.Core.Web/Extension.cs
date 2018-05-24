using Autofac;
using Autofac.Integration.WebApi;
using MF.Core.Infrastructure;
using MF.Core.Web.Infrastructure;
using MF.Core.Web.WebFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MF.Core.Web
{
    public static class Extension
    {
        public static AppBoot UseWebDeault(this AppBoot boot)
        {
            EngineContext.Initialize(new MikeWebEngine());
            boot.UseLog4();

            var builder = new ContainerBuilder();

            //builder.RegisterInstance(new RedisManager(config.RedisConn)).As<RedisCacheManager>().SingleInstance();

            builder.RegisterApiControllers(EngineContext.Current.ContainerManager.Container.Resolve<ITypeFinder>().GetAssemblies().ToArray())
                .InstancePerRequest();

            builder.Update(EngineContext.Current.ContainerManager.Container);

            GlobalConfiguration.Configure(config=>
            {
                config.Filters.Add(new MFExceptionAttribute());
            });

            return new AppBoot();
        }
    }
}
