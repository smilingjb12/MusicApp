using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Data.Domain;
using DataAccess;
using Newtonsoft.Json;
using SocialApp.App_Start;
using SocialApp.Models;
using WebMatrix.WebData;
using System.Data.Entity;

namespace SocialApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static void CreateTestUser()
        {
            using (var db = new SocialAppContext())
            {
                var existingUser = db.Users.Find(1);
                if (existingUser != null) return;
                var user = new User
                {
                    Email = "test@test.test",
                    Password = "test"
                };
                WebSecurity.CreateUserAndAccount(user.Email, user.Password, propertyValues: new
                {
                    Password = user.Password,
                    ActivationCode = "1",
                    IsActivated = true,
                    Role = Role.User,
                    FullName = "FullName",
                    Country = "Country",
                    City = "City",
                    About = "About",
                    PictureFilePath = "/Content/images/default-profile-picture-b&w.png"
                });
                db.Users.Add(user);
                db.SaveChanges();
            }   
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SocialAppContext.Initialize();
            AuthConfig.RegisterAuth();
            AutoMapperConfig.RegisterMappings();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

            CreateTestUser();
        }
    }
}