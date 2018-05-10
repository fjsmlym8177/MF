using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebHost.Controllers
{
    public class HttpTestV2Controller : ApiController
    {
        private int version = 2;

        public IEnumerable<string> Get()
        {
            return new List<string> { "Get版本" + version };
        }

        // GET api/values/5
        public string Get(int id)
        {

            //throw new MikeException("test");

            return "Get版本" + version;
        }

        // POST api/values
        public string Post([FromBody]string value)
        {
            return "POST版本" + version;
        }

        // PUT api/values/5
        public string Put(int id, [FromBody]string value)
        {
            return "PUT版本" + version;
        }

        // DELETE api/values/5
        public string Delete(int id)
        {
            return "DELETE版本" + version;
        }
    }
}
