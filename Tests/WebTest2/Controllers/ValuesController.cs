﻿using MF.Core;
using MF.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebTest2.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            return Ok(new ApiResponse(new
            {
                Id = Guid.NewGuid()
            }));

            //return "value";
        }

        // POST api/values
        public IHttpActionResult Post([FromBody]string value)
        {


            var request = new List<string> { "11111", "2222", "3333" }.AsQueryable().OrderByDescending(p => p).ToPaged(1, 1);

            return Ok(new ApiResponsePager<string>(request));
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {

            throw new MikeException("xxx");
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            throw new Exception("xxx");
        }
    }
}
