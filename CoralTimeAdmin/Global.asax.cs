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

namespace CoralTimeAdmin
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start() {

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new DoubleModelBinder());

            ConfigureAutofac();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void ConfigureAutofac() {
            var builder = new ContainerBuilder();

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
            //builder.RegisterType<EmulatorContext>().InstancePerRequest();

            //builder.RegisterType<CoralTimeContext>().InstancePerRequest();
            //builder.Register<IDbContext>(c => new CoralTimeContext()).InstancePerLifetimeScope();
            var conString = WebConfigurationManager.ConnectionStrings["CoralTimeContext"].ConnectionString;

            builder.Register<IDbContext>(c => new CoralTimeContext(conString)).InstancePerLifetimeScope();

            //controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            #region Register all controllers for the assembly

            // Note that ASP.NET MVC requests controllers by their concrete types, 
            // so registering them As<IController>() is incorrect. 
            // Also, if you register controllers manually and choose to specify 
            // lifetimes, you must register them as InstancePerDependency() or 
            // InstancePerHttpRequest() - ASP.NET MVC will throw an exception if 
            // you try to reuse a controller instance for multiple requests. 
            //builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerHttpRequest();

            #endregion

            //Repositories
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<DapperRepository>().As<IDapperRepository>().InstancePerLifetimeScope();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}