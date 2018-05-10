using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core
{
    public class Pager<T>
    {
        public long TotalCount { get; set; }

        public IList<T> Data { get; set; }
    }
}
