using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Rabbit
{
    public class ExecuteMessage
    {
        public string ID { get; set; }

        public string Type { get; set; }

        public string Data { get; set; }
    }
}
