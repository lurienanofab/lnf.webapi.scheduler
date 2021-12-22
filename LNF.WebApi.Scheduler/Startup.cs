﻿using LNF.Impl.DependencyInjection;
using LNF.Web;
using Microsoft.Owin;
using Owin;
using SimpleInjector.Integration.WebApi;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

[assembly: OwinStartup(typeof(LNF.WebApi.Scheduler.Startup))]

namespace LNF.WebApi.Scheduler
{
    /// <summary>
    /// This class must be local to the application or there is an issue with routing when IIS resets.
    /// </summary>
    public class Startup : ApiOwinStartup
    {
        public static WebApp WebApp { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            WebApp = new WebApp();

            var wcc = new WebContainerConfiguration(WebApp.Context);
            wcc.RegisterAllTypes();

            // setup web dependency injection
            Assembly[] assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            WebApp.BootstrapMvc(assemblies);

            app.UseDataAccess(WebApp.Context);

            var config = CreateConfiguration();
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(WebApp.GetContainer());

            app.UseWebApi(config);
        }
    }
}