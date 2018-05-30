using MF.Core.Configuration;
using MF.Core.Logging;
using MF.Core.Utilities;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MF.Core.Web.MiddleWare.Logging
{
    /// <summary>
    /// 日志中间件
    /// </summary>
    public class LoggingMiddleWare : OwinMiddleware
    {
        private ILogger _logger;
        //private Func<LoggingMiddleWareConfig> _acquireConfig;


        public LoggingMiddleWare(OwinMiddleware next, ILogger logger) : base(next)
        {
            _logger = logger;
            //_acquireConfig = acquireConfig;
        }

        public override async Task Invoke(IOwinContext context)
        {


            HttpResponse httpResponse = HttpContext.Current.Response;
            OutputCaptureStream outputCapture = new OutputCaptureStream(httpResponse.Filter);
            httpResponse.Filter = outputCapture;

            IOwinResponse owinResponse = context.Response;
            //buffer the response stream in order to intercept downstream writes
            Stream owinResponseStream = owinResponse.Body;
            owinResponse.Body = new MemoryStream();

            //开始记录
            var requestLog = new ApiRequest();
            HttpContext.Current.Items["api_request"] = requestLog;
            requestLog.Headers = context.Request.Headers;
            requestLog.URI = context.Request.Path.ToString();
            requestLog.QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";


            using (var sr = new StreamReader(context.Request.Body))
            {
                requestLog.RequestParams = sr.ReadToEnd();
            }
            requestLog.Method = context.Request.Method;

            var wathch = new Stopwatch();
            wathch.Start();
            await Next.Invoke(context);

            if (outputCapture.CapturedData.Length == 0)
            {
                //response is formed by OWIN
                //make sure the response we buffered is flushed to the client
                owinResponse.Body.Position = 0;
                await owinResponse.Body.CopyToAsync(owinResponseStream);
            }
            else
            {
                //response by MVC
                //write captured data to response body as if it was written by OWIN         
                outputCapture.CapturedData.Position = 0;
                outputCapture.CapturedData.CopyTo(owinResponse.Body);
            }
            byte[] b = ((MemoryStream)owinResponse.Body).ToArray();

            requestLog.Result = System.Text.Encoding.UTF8.GetString(b, 0, b.Length);
            requestLog.Status = context.Response.StatusCode;
            wathch.Stop();
            requestLog.ElapsedTime = wathch.Elapsed.TotalMilliseconds;

            Log(requestLog);
        }

        public void Log(ApiRequest request)
        {
            if (request.Result.StartsWith("<!DOCTYPE html>") || request.URI.StartsWith("/swagger"))
            {
                return;
            }

            var config = ConfigLoader.LoadConfig<LoggingMiddleWareConfig>();

            if (config == null)
            {
                if (request.ElapsedTime > 1000)
                {
                    _logger.InsertRequestLog("requestLog", request.ToJson());
                }
            }
            else
            {
                if (config.IncludePath.Any(p => request.URI.Contains(p)))
                {
                    _logger.InsertRequestLog("requestLog", request.ToJson());
                    return;
                }

                if (!config.IgnorePath.Any(p => request.URI.Contains(p)) && config.Overtime <= request.ElapsedTime)
                {
                    _logger.InsertRequestLog("requestLog", request.ToJson());
                    return;
                }
            }
        }

        internal class OutputCaptureStream : Stream
        {
            private Stream InnerStream;
            public MemoryStream CapturedData { get; private set; }

            public OutputCaptureStream(Stream inner)
            {
                InnerStream = inner;
                CapturedData = new MemoryStream();
            }

            public override bool CanRead
            {
                get { return InnerStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return InnerStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return InnerStream.CanWrite; }
            }

            public override void Flush()
            {
                InnerStream.Flush();
            }

            public override long Length
            {
                get { return InnerStream.Length; }
            }

            public override long Position
            {
                get { return InnerStream.Position; }
                set { CapturedData.Position = InnerStream.Position = value; }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return InnerStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                CapturedData.Seek(offset, origin);
                return InnerStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                CapturedData.SetLength(value);
                InnerStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                CapturedData.Write(buffer, offset, count);
                InnerStream.Write(buffer, offset, count);
            }
        }
    }
}
