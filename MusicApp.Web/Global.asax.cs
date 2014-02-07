using System.Collections.Generic;
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
using System.Data;
using System.Linq;

namespace SocialApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static void CreateTestUser()
        {
            using (var db = new SocialAppContext())
            {
                bool hasFakeUsers = db.Users.FirstOrDefault(u => u.Email == "test@test.test") != null;
                if (hasFakeUsers) return;
                var users = new List<User>
                {
                    new User() { Email = "test@test.test", Password = "test", FullName = "John Doe" },
                    new User() { Email = "test2@test.test", Password = "test", FullName = "Jill Jason" },
                    new User() { Email = "test3@test.test", Password = "test", FullName = "Bill Hill" },
                    new User() { Email = "test4@test.test", Password = "test", FullName = "Mary Jane" }
                };
                foreach (var user in users)
                {
                    WebSecurity.CreateUserAndAccount(user.Email, user.Password, propertyValues: new
                    {
                        Password = user.Password,
                        ActivationCode = "1",
                        IsActivated = true,
                        Role = Role.User,
                        FullName = user.FullName,
                        Country = "Country",
                        City = "City",
                        About = "About",
                        PictureFilePath = "/Content/images/default-profile-picture-b&w.png"
                    });
                    db.Users.Add(user);
                }
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