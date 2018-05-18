using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Pipeline
{
    public class RedisPipelineConfig
    {
        public int DBSpace { get; set; } = 0;

        public int WaitSleep { get; set; } = 1000;

        public int LongWaitSleep { get; set; } = 10000;

        public int BatchTakeNumber { get; set; } = 1000;

    }
}
