using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Web.MiddleWare
{
    public class LoggingMiddleWareConfig
    {
        public LoggingMiddleWareConfig()
        {
            IgnorePath = new List<string>();
            IncludePath = new List<string>();
        }

        public int Overtime { get; set; }

        public IList<string> IgnorePath { get; set; }

        public IList<string> IncludePath { get; set; }
    }
}
