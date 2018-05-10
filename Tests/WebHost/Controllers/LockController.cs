using MF.Core.Lock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebHost.Controllers
{
    public class LockController : ApiController
    {
        public static int Flag = 1;

        private readonly ILockManager _lock;
        public LockController(ILockManager @lock)
        {
            _lock = @lock;
        }

        [HttpPost]
        [Route("api/lock/lock")]
        public IHttpActionResult Lock(LockModel model)
        {
            return Ok(_lock.LockKey(model.LockKey, model.LockValue));
        }

        [HttpPost]
        [Route("api/lock/UnLock")]
        public IHttpActionResult UnLock(LockModel model)
        {
            return Ok(_lock.UnlockKey(model.LockKey, model.LockValue));
        }

        [HttpPost]
        [Route("api/lock/lockAction")]
        public IHttpActionResult LockAction(LockModel model)
        {
            _lock.ToLock(model.LockKey, model.LockValue, () =>
            {
                LockController.Flag = LockController.Flag + 1;
            });


            return Ok(LockController.Flag);
        }
    }

    public class LockModel
    {
        public string LockKey { get; set; }

        public string LockValue { get; set; }
    }
}
