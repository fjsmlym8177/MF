using MF.Core.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MF.Core.Lock
{
    public class RedisLockManager : ILockManager
    {
        /// <summary>
        /// 获取锁的超时时间(ms)
        /// </summary>
        private const int GetLockTimeout = 3000;
        /// <summary>
        /// 占用锁的最大时间(ms)
        /// </summary>
        private const int LockMaxTime = 30000;
        /// <summary>
        /// 轮询频率
        /// </summary>
        private const int Frequency = 200;


        private readonly RedisManager _readisManager;
        private readonly IDatabase _db;

        public RedisLockManager(RedisManager redisManager, int dbSpace)
        {
            _readisManager = redisManager;
            _db = _readisManager.muxer.GetDatabase(dbSpace);
        }


        public bool LockKey(string key, string lockValue)
        {
            int spendTime = 0;
            var expiresIn = TimeSpan.FromMilliseconds(LockMaxTime);

            while (spendTime < GetLockTimeout)
            {
                if (_db.StringSet(key, lockValue, expiresIn, When.NotExists))
                {
                    return true;
                }
                spendTime += Frequency;
                Thread.Sleep(Frequency);
            }

            return false;
        }

        public bool UnlockKey(string key, string lockValue)
        {
            var keyValue = _db.StringGet(key).ToString();
            if (keyValue == lockValue)
            {
                _db.KeyDelete(key);
                return true;
            }

            return false;
        }
    }
}
