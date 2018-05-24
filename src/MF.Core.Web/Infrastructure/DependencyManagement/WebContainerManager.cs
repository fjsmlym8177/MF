using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.WebApi;
using MF.Core.Exceptions;
using MF.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MF.Core.Web.Infrastructure.DependencyManagement
{
    public class WebContainerManager: ContainerManager
    {
        public WebContainerManager(IContainer container) 
            : base(container)
        {
        }

        public override ILifetimeScope Scope()
        {
            try
            {
                //HttpRequestMessage
                //return AutofacDependencyResolver
                if (HttpContext.Current != null)
                    //    return AutofacDependencyResolver.Current.RequestLifetimeScope;
                    return (HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage).GetDependencyScope().GetRequestLifetimeScope();
                //ILifetimeScopeProvider.RequestLifetime

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
            catch (Exception)
            {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }
    }
}
