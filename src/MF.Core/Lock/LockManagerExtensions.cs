using MF.Core.Exceptions;
using MF.Core.Infrastructure;
using MF.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Lock
{
    public static class LockManagerExtensions
    {
        public static void ToLock(this ILockManager lockManger, string key, string lockValue, Action action)
        {
            if (!lockManger.LockKey(key, lockValue))
                throw new MikeException("锁超时");

            try
            {
                action();
            }
            catch (Exception ex)
            {
                throw new MikeException(ex.Message);
            }
            finally
            {
                lockManger.UnlockKey(key, lockValue);
            }
        }
    }
}
