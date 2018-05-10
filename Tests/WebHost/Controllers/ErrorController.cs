using MF.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebHost.Controllers
{
    public class ErrorController : ApiController
    {
        public string Get(int id)
        {
            throw new MikeException("测试错误","测试错误code");

            return "V2版本";
        }


        public IList<string> Get()
        {
            throw new Exception("测试错误");

            return null;
        }
    }
}
