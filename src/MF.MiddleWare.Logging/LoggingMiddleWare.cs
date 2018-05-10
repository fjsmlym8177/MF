using MF.Core;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.MiddleWare.Logging
{
    public class LoggingMiddleWare : OwinMiddleware
    {
        public LoggingMiddleWare(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            //开始记录
            //HttpContext.Current.Items["api_request"] = new ApiRequest();


            await Next.Invoke(context);

            //结束记录

        }
    }
}
