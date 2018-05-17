using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Utilities
{
    public static class JsonExtensions
    {

        public static string ToJson(this object str)
        {
            return JsonConvert.SerializeObject(str);
        }


        public static T ToDeserialize<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
