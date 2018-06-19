using MF.Core.Infrastructure;
using MF.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MF.Core.Utilities
{
    public class HttpHelper
    {
        //private static HttpClient _client = new HttpClient();

        private static T LogAction<T>(string url, object data, string method, Dictionary<string, string> customerHeaders, string contentType = "application/json")
        {
            var request = WebRequest.CreateHttp(url);
            request.Timeout = 10000;

            if (customerHeaders != null)
            {
                foreach (var header in customerHeaders)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            request.Method = method.ToUpper();
            request.ContentType = $"{contentType};charset=UTF-8";

            var @params = data.ToJson();
            var key = Guid.NewGuid().ToString();

            ApiRequest.AddChildrenRequest(key, url, method, @params);
            var wathch = new Stopwatch();
            wathch.Start();

            if (request.Method != "GET" && data != null)
            {


                if (contentType == "application/json")
                {
                    var reqStreamWriter = new StreamWriter(request.GetRequestStream());
                    reqStreamWriter.Write(@params);
                    reqStreamWriter.Close();
                }
                else
                {
                    var postData = Encoding.UTF8.GetBytes(data.ObjectToQuery());

                    var reqStream = request.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    reqStream.Close();
                    //reqStreamWriter.Write(postData, 0, postData.Length);
                }




            }

            var response = request.GetResponse() as HttpWebResponse;
            var responseStream = new StreamReader(response.GetResponseStream());
            var result = responseStream.ReadToEnd();
            responseStream.Close();

            wathch.Stop();
            ApiRequest.AddChildrenReponse(key, result, wathch.Elapsed.TotalMilliseconds);

            if (HttpContext.Current == null)
            {
                EngineContext.Current.Resolve<ILogger>().InsertHttpLog("HttpLog", new ChildRequest
                {
                    ElapsedTime = wathch.Elapsed.TotalMilliseconds,
                    Method = method,
                    Params = @params,
                    Url = url,
                    Value = result
                }.ToJson());
            }

            return result.ToDeserialize<T>();
        }

        public static T Get<T>(string url, Dictionary<string, string> customerHeaders)
        {
            var returnResult = LogAction<T>(url, null, "GET", customerHeaders);

            return returnResult;
        }

        public static T Put<T>(string url, object postData, Dictionary<string, string> customerHeaders)
        {
            var returnResult = LogAction<T>(url, "", "PUT", customerHeaders);

            return returnResult;
        }


        public static T Delete<T>(string url, Dictionary<string, string> customerHeaders)
        {
            var returnResult = LogAction<T>(url, "", "DELETE", customerHeaders);

            return returnResult;
        }

        public static T Post<T>(string url, object postData, Dictionary<string, string> customerHeaders, string contentType = "application/json")
        {

            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var key = Guid.NewGuid().ToString();
            //var @params = postData.ToJson();

            //ApiRequest.AddChildrenRequest(key, url, @params);
            //var wathch = new Stopwatch();
            //wathch.Start();

            ////var content = new StringContent(@params, Encoding.UTF8, "application/json");
            //var result = client.PostAsJsonAsync(url, postData).Result;
            //var returnResult = result.Content.ReadAsAsync<T>().Result;

            //wathch.Stop();
            //ApiRequest.AddChildrenReponse(key, returnResult.ToJson(), wathch.Elapsed.TotalMilliseconds);


            var returnResult = LogAction<T>(url, postData, "POST", customerHeaders, contentType);

            return returnResult;
        }


        public static void PostSyncNoResult(string url, object postData, Dictionary<string, string> customerHeaders)
        {
            Task.Run(() =>
            {
                Post<object>(url, postData, customerHeaders);
            });
        }
    }


    public static class UrlHelper
    {
        public static string ObjectToQuery<T>(this T obj, bool isAddEmptyValue = false, string orderBy = null, string removeFiled = null /*移除那个字段*/, string fieldReplace = null /*格式old,name*/) where T : class
        {
            if (obj == null)
            {
                return "";
            }
            var properties = obj.GetType().GetProperties();
            var list = new List<string>();
            foreach (var item in properties)
            {
                if (removeFiled != null && item.Name == removeFiled)
                {
                    //移除不必要字段
                    continue;
                }
                var proValue = item.GetValue(obj, null);
                if ((proValue != null && !string.IsNullOrEmpty(proValue.ToString())) || isAddEmptyValue)
                {
                    var value = proValue != null ? proValue.ToString() : "";
                    value = value.Replace("+", "%20");
                    var filedName = item.Name;
                    //替换key名字，如xxx_Name替换为xxx[0].Name
                    if (!string.IsNullOrEmpty(fieldReplace))
                    {
                        var arry = fieldReplace.Split('$');
                        if (arry.Length > 1)
                        {
                            filedName = filedName.Replace(arry[0], arry[1]);
                        }
                    }
                    list.Add(filedName + "=" + value);
                }
            }
            if (orderBy != null)
            {
                switch (orderBy)
                {
                    case "asc":
                        {
                            list = list.OrderBy(m => m).ToList();
                            break;
                        }
                    default:
                        {
                            list = list.OrderByDescending(m => m).ToList(); break;
                        }
                }
            }
            return string.Join("&", list);
        }
    }
}
