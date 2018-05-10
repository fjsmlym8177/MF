using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Utilities
{
    public class ServiceLoader
    {
        public static T Post<T>(string url, string interfaceName, string method, params ServiceParameterInfo[] paramsData)
        {
            using (var httpclient = new HttpClient())
            {

                for (int i = 0; i < paramsData.Length; i++)
                {
                    paramsData[i].Sort = i;
                }

                var @params = new Dictionary<string, string>()
                   {
                       {"interfacename", interfaceName},
                       {"methodname", method},
                       {"param", paramsData.ToJson()},
                   };

                var content = new FormUrlEncodedContent(@params);

                var key = Guid.NewGuid().ToString();
                ApiRequest.AddChildrenRequest(key, url, "POST", @params.ToJson());
                var wathch = new Stopwatch();
                wathch.Start();

                var response = httpclient.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;

                wathch.Stop();
                ApiRequest.AddChildrenReponse(key, response, wathch.Elapsed.TotalMilliseconds);

                return response.ToDeserialize<T>();
            }
        }


        public class ServiceParameterInfo
        {
            public ServiceParameterInfo(string type, object value)
            {
                ParameterType = type;
                ParamValue = value.ToJson();
            }

            public ServiceParameterInfo(Type type, object value)
            {
                ParameterType = type.FullName;
                ParamValue = value.ToJson();

                if (type.FullName == "System.Guid")
                    ParamValue = value.ToString();
            }

            public string ParameterType { get; set; }
            public string ParamValue { get; set; }
            /// <summary>
            /// 排序
            /// </summary>
            public int Sort { get; set; }
        }
    }
}
