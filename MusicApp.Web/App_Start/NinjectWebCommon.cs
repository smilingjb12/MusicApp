using System.Linq;
using System.Reflection;
using Business;
using Business.Services;
using DataAccess;
using SocialApp.Models;
using WebGrease.Css.Extensions;

[assembly: WebActivator.PreApplicationStartMethod(typeof(SocialApp.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(SocialApp.App_Start.NinjectWebCommon), "Stop")]

namespace SocialApp.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<SocialAppContext>()
                  .ToSelf()
                  .InRequestScope();

            kernel.Bind<IEmailSender>()
                  .To<EmailSender>();

            var serviceTypes = typeof(IUserService).Assembly
                .GetTypes()
                .Where(t => t.Name.EndsWith("Service"))
                .ToList();
            var interfaces = serviceTypes.Where(t => t.IsInterface).ToList();
            var implementations = serviceTypes.Where(t => t.IsClass).ToList();
            foreach (Type intf in interfaces)
            {
                Type implementation = implementations.First(impl => impl.Name == intf.Name.Replace("I", ""));
                kernel.Bind(intf).To(implementation);
            }
        }        
    }
}
