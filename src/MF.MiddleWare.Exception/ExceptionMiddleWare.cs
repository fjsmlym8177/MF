using MF.Core;
using MF.Core.Exceptions;
using MF.Core.Utilities;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.MiddleWare.Exception
{
    public class ExceptionMiddleWare : OwinMiddleware
    {
        public ExceptionMiddleWare(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            //HttpContext;

            try
            {
                await Next.Invoke(context);
            }
            catch (System.Exception ex)
            {
                if (ex is MikeException)
                {
                    var exception = (MikeException)ex;
                    context.Response.Write(new ApiResponse()
                    {
                        Code = exception.Code,
                        Msg = exception.Message,
                    }.ToJson());
                }
                else
                {
                    context.Response.Write(new ApiResponse()
                    {
                        Code = CodeEnum.Fault,
                        Msg = "业务繁忙！",
                    }.ToJson());
                }
            }

        }
    }
}
