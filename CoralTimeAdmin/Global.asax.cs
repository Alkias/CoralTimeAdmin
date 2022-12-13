using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using CoralTimeAdmin.DAL;
using CoralTimeAdmin.Fakes;
using CoralTimeAdmin.Helpers;
using CoralTimeAdmin.Repositories;
using CoralTimeAdmin;
using CoralTimeAdmin.Infrastructure;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace CoralTimeAdmin
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start() {

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new DoubleModelBinder());

            //ConfigureAutofac();

            // Configure SeriLog Logger
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: HttpContext.Current.Server.MapPath("~/App_Data/Serilog.txt"),
                    rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer(
                    connectionString: ConfigurationManager.ConnectionStrings["SeriLogContext"].ConnectionString,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "SeriLog" },
                    sinkOptionsSection: null,
                    appConfiguration: null,
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    formatProvider: null,
                    columnOptions: null,
                    columnOptionsSection: null,
                    logEventFormatter: null)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)//To capture Information and error only  
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            //initialize engine context
            EngineContext.Initialize(false);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            

        }

        protected void Application_Error(object sender, EventArgs e) {
            Exception exc = Server.GetLastError();

            Serilog.Log.Error(exc, "Application_Error");
        }

        private void ConfigureAutofac() {
            var builder = new ContainerBuilder();
            builder.RegisterFilterProvider();

            //HTTP context and other related stuff
            builder.Register(
                    c =>
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
            var conString = WebConfigurationManager.ConnectionStrings["CoralTimeContext"].ConnectionString;

            builder.Register<IDbContext>(c => new CoralTimeContext(conString)).InstancePerLifetimeScope();


            #region Serilog resolve

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger);

            // register logger factory and generic logger
            builder.RegisterInstance<ILoggerFactory>(loggerFactory);
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            #endregion

            #region Register all controllers for the assembly

            // Note that ASP.NET MVC requests controllers by their concrete types, 
            // so registering them As<IController>() is incorrect. 
            // Also, if you register controllers manually and choose to specify 
            // lifetimes, you must register them as InstancePerDependency() or 
            // InstancePerHttpRequest() - ASP.NET MVC will throw an exception if 
            // you try to reuse a controller instance for multiple requests. 
            //builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerHttpRequest();

            //controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            #endregion

            //Repositories
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<DapperRepository>().As<IDapperRepository>().InstancePerLifetimeScope();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}