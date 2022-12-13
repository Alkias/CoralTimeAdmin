using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Autofac;
using Autofac.Integration.Mvc;
using CoralTimeAdmin.Fakes;
using CoralTimeAdmin.Helpers;
using CoralTimeAdmin.Infrastructure.DependencyManagement;
using CoralTimeAdmin.Repositories;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoralTimeAdmin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region Implementation of IDependencyRegistrar

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterFilterProvider();

            // HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null
                    ? new HttpContextWrapper(HttpContext.Current) as HttpContextBase
                    : new FakeHttpContext("~/") as HttpContextBase)
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            // Register Context

            builder.Register((Func<IComponentContext, IDbContext>)(c => new CoralTimeContext(WebConfigurationManager.ConnectionStrings["CoralTimeContext"].ConnectionString))).InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();


            //builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();

            var loggerFactory = new LoggerFactory();

            loggerFactory.AddSerilog(Serilog.Log.Logger);

            // register logger factory and generic logger
            builder.RegisterInstance<ILoggerFactory>(loggerFactory);
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();


            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //Repositories
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<DapperRepository>().As<IDapperRepository>().InstancePerLifetimeScope();

        }

        public int Order {
            get { return 0; }
        }

        #endregion
    }
}