using MF.Core.Infrastructure;
using MF.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MF.Core
{
    public class ApiRequest
    {
        public ApiRequest()
        {
            ChildrenRequests = new Dictionary<string, ChildRequest>();
            DBLogs = new List<string>();
            Headers = new Dictionary<string, string[]>();
        }

        public string URI { get; set; }
        public double ElapsedTime { get; set; }
        //public string ActionName { get; set; }    
        public string Method { get; internal set; }
        public string RequestParams { get; set; }
        public string QueryString { get; internal set; }
        public string Result { get; set; }
        public int Status { get; set; }

        public IDictionary<string, string[]> Headers { get; set; }

        public Dictionary<string, ChildRequest> ChildrenRequests { get; set; }
        public IList<string> DBLogs { get; set; }
        public string Exception { get; set; }
       
  

        public static void AddChildrenRequest(string key, string url, string method, string @params)
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["api_request"] != null)
            {
                ApiRequest model = (ApiRequest)HttpContext.Current.Items["api_request"];

                model.ChildrenRequests[key] = new ChildRequest()
                {
                    Method = method,
                    Url = url,
                    Params = @params
                };
            }
        }

        public static void AddChildrenReponse(string key, string result, double elapsedTime)
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["api_request"] != null)
            {
                ApiRequest model = (ApiRequest)HttpContext.Current.Items["api_request"];
                model.ChildrenRequests[key].Value = result;
                model.ChildrenRequests[key].ElapsedTime = elapsedTime;
            }
        }

        public static void AddHttpLog(string log)
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["api_request"] != null)
            {

            }
            else
            {
                EngineContext.Current.Resolve<ILogger>().InsertDBLog("EF", log);
            }
        }

        public static void AddDBLog(string log)
        {
            if (HttpContext.Current != null && HttpContext.Current.Items["api_request"] != null)
            {
                ApiRequest model = (ApiRequest)HttpContext.Current.Items["api_request"];
                model.DBLogs.Add(log);
            }
            else
            {
                EngineContext.Current.Resolve<ILogger>().InsertDBLog("EF", log);
            }
        }
    }


    public class ChildRequest
    {
        public string Method { get; set; }

        public string Params { get; set; }

        public string Value { get; set; }

        public double ElapsedTime { get; set; }
        public string Url { get; internal set; }
    }
}
