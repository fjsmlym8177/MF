using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

using MF.Core.MiddleWare.Logging;
using MF.Core.Infrastructure;
using MF.Core.Logging;

[assembly: OwinStartup(typeof(WebHost.Startup))]

namespace WebHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);

            app.Use<LoggingMiddleWare>(EngineContext.Current.Resolve<ILogger>());
        }
    }
}
