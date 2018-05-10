using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebHost.Controllers
{
    public class HttpTestController : ApiController
    {
        private int version = 1;

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
        public string Post(TestClass dto)
        {
            return "POST版本" + version + dto.A + " " + dto.B;
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

    public class TestClass
    {
        public string A { get; set; }

        public string B { get; set; }


    }
}
