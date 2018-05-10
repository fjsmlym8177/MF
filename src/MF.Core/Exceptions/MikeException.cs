using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Exceptions
{
    public class MikeException : Exception
    {
        public string SubCode { get; set; }


        public MikeException()
        {
        }

        public MikeException(string message, string subCode = "")
           : base(message)
        {
            SubCode = subCode;
        }
    }
}
