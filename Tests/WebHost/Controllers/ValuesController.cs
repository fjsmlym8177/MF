using MF.Core.Exceptions;
using MF.Core.Infrastructure;
using MF.Core.Logging;
using MF.Core.Utilities;
using MF.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebHost.Infrastructure.Mapping;

namespace WebHost.Controllers
{
    //[Authorize]
    //[VersionedRoute("api/values", 1)]
    public class ValuesController : ApiController
    {
        private IRepository<Customer, Guid> _repository;
        private IDbContext<Guid> _dbContext;

        public ValuesController(IRepository<Customer, Guid> repository, IDbContext<Guid> dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            string BaseUrl = "http://localhost:53608/";
            var rep1 = EngineContext.Current.Resolve<IRepository<Customer, Guid>>();
            var rep2 = EngineContext.Current.Resolve<IRepository<Customer, Guid>>();

            Debug.WriteLine(rep1.GetHashCode());
            Debug.WriteLine(rep2.GetHashCode());

            var context1 = EngineContext.Current.Resolve<IDbContext<Guid>>();
            var context2 = EngineContext.Current.Resolve<IDbContext<Guid>>();

            Debug.WriteLine(context1.GetHashCode());
            Debug.WriteLine(context2.GetHashCode());

            var result = _repository.Table.ToList();

            _repository.Delete(result.FirstOrDefault());

            _dbContext.SaveChanges();

            EngineContext.Current.Resolve<ILogger>().Warning("Test");
            EngineContext.Current.Resolve<ILogger>().InsertQueueLog("Test", "收到的消息内容");

            HttpHelper.Delete<object>(BaseUrl + "api/HttpTest/1", null);
            HttpHelper.Post<object>(BaseUrl + "api/HttpTest", new { }, null);

            throw new MikeException("test", "code111111");

            Debug.WriteLine("Log:" + EngineContext.Current.Resolve<ILogger>().GetHashCode());

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {

            //throw new MikeException("test");

            return "v1版本";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
