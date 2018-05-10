
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebHost.Controllers
{
    //[VersionedRoute("api/values",2)]
    public class ValuesV2Controller : ApiController
    {

        public string Get(int id,string a,string b)
        {
            return "V2版本";
        }
    }
}
 