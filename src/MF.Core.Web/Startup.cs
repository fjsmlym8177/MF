using MF.Core.Configuration;
using MF.Core.Infrastructure;
using MF.Core.Logging;
using MF.Core.Web.MiddleWare;
using MF.Core.Web.MiddleWare.Logging;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(MF.Core.Web.Startup))]
namespace MF.Core.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);

            ConfigLoader.SetConfig<LoggingMiddleWareConfig>("LoggingMiddleWare.json");

            app.Use<LoggingMiddleWare>(EngineContext.Current.Resolve<ILogger>());
        }
    }
}
