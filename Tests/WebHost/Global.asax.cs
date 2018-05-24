using MF.Core;
using MF.Core.Configuration;
using MF.Core.Infrastructure;
using MF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebHost.Infrastructure;
using WebHost.Infrastructure.Mapping;
using MF.Core.Web;
using MF.Core.Pipeline;

namespace WebHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //EngineContext.Initialize(false);

            //EngineContext.Current.Resolve<IDbContext<Guid>>().Set<Customer>().ToList();

            ConfigLoader.SetConfig<TestConfig>("appsettings.json");

            var config = ConfigLoader.LoadConfig<TestConfig>();

            AppBoot.Instance
                .UseWebDeault()
                .UseRedis(config)
                .UseRedisCache(1)
                .UseRedisPipeline(new RedisPipelineConfig());

        }


    }
}
