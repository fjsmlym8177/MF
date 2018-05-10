using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Utilities
{
    public static class PollyExtensions
    {
        /// <summary>
        /// 3次重试 时间间隔 1 3 5
        /// </summary>
        /// <param name="action"></param>
        public static void ReTry(this Action action)
        {
            //try
            //{
            //    var politicaWaitAndRetry =
            //    Policy
            //    .Handle<>
            //    .WaitAndRetry(new[]{
            //               TimeSpan.FromSeconds(1),
            //               TimeSpan.FromSeconds(3),
            //               TimeSpan.FromSeconds(5),
            //    });

            //    politicaWaitAndRetry.Execute(() =>
            //    {
            //        action();
            //    });
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

        }
    }
}
