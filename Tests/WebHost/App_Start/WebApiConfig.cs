using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Routing;
using System.Web.Http.Dispatcher;
using MF.Core.WebFramework;
using MF.Core.WebFramework.Attributes;

namespace WebHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            // 将 Web API 配置为仅使用不记名令牌身份验证。
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //var constraintResolver = new DefaultInlineConstraintResolver();
            //constraintResolver.ConstraintMap.Add("nonzero", typeof(VersionedRoute));

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
   
            //config.Services.Replace(typeof(IHttpControllerSelector), new VersionControllerSelector(config));
            config.Filters.Add(new MFExceptionAttribute());
        }
    }
}
