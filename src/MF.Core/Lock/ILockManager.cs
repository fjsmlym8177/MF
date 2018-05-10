using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Lock
{
    public interface ILockManager
    {
        bool LockKey(string key, string lockValue);

        bool UnlockKey(string key,string lockValue);
    }
}
