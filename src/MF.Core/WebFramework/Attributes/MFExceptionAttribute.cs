using MF.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace MF.Core.WebFramework.Attributes
{
    public class MFExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                ApiRequest model = (ApiRequest)HttpContext.Current.Items["api_request"];
                model.Exception =
                    actionExecutedContext.Exception.InnerException == null ?
                    $"Message:{actionExecutedContext.Exception.Message}{System.Environment.NewLine}StackTrace{actionExecutedContext.Exception.StackTrace}":
                    $"Message:{actionExecutedContext.Exception.InnerException.Message}{System.Environment.NewLine}StackTrace{actionExecutedContext.Exception.InnerException.StackTrace}";

                if (actionExecutedContext.Exception is MikeException)
                {
                    var exception = actionExecutedContext.Exception as MikeException;
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse<Object>(HttpStatusCode.OK
                    , new
                    {
                        Code = CodeEnum.Error,
                        SubCode = exception.SubCode,
                        Msg = exception.Message,
                    });
                }
                else if (actionExecutedContext.Exception.InnerException != null && (actionExecutedContext.Exception.InnerException is MikeException))
                {
                    var exception = actionExecutedContext.Exception.InnerException as MikeException;
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse<Object>(HttpStatusCode.OK
                    , new
                    {
                        Code = CodeEnum.Error,
                        SubCode = exception.SubCode,
                        Msg = exception.Message,
                    });
                }
                else
                {
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse<Object>(HttpStatusCode.OK
                   , new
                   {
                       Code = CodeEnum.Fault,
                       SubCode = "unknow-error",
                       Msg = "系统繁忙！"
                   });
                }
            }

            base.OnException(actionExecutedContext);
        }
    }
}
