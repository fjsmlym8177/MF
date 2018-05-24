using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace MF.Core.Web.WebFramework
{
    public class VersionControllerSelector : DefaultHttpControllerSelector
    {
        public const string VERSION_HEADER_NAME = "mike-api-version";

        public VersionControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
        }

        private string GetVersionFromHTTPHeader(HttpRequestMessage request)
        {
            if (request.Headers.Contains(VERSION_HEADER_NAME))
            {
                var versionHeader = request.Headers.GetValues(VERSION_HEADER_NAME).FirstOrDefault();
                if (versionHeader != null)
                {
                    return versionHeader;
                }
            }

            return string.Empty;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping();

            var routeData = request.GetRouteData();

            var controllerName = (string)routeData.Values["controller"];

            HttpControllerDescriptor controllerDescriptor;

            if (controllers.TryGetValue(controllerName, out controllerDescriptor))
            {
                var version = GetVersionFromHTTPHeader(request);

                if (!string.IsNullOrEmpty(version))
                {
                    var versionedControllerName = string.Concat(controllerName, "V", version);

                    HttpControllerDescriptor versionedControllerDescriptor;
                    if (controllers.TryGetValue(versionedControllerName, out versionedControllerDescriptor))
                    {
                        return versionedControllerDescriptor;
                    }
                }

                return controllerDescriptor;
            }

            return null;
        }
    }
}
