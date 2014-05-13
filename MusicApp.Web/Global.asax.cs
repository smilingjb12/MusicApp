using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Data;
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
        private static void CreateTestUsers()
        {
            using (var db = new SocialAppContext())
            {
                bool hasFakeUsers = db.Users.FirstOrDefault(u => u.Email == "test@test.test") != null;
                if (hasFakeUsers) return;
                var users = new List<User>();
                var names = new[] {"Jill John Jack Jane Jim Kate Bill"};
                for (int i = 0; i < 20; i++)
                {
                    users.Add(new User() { Email = "test" + i + "@test.test", Password = "test", FullName = names.Sample()});
                }
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
                }
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

            CreateTestUsers();
        }
    }
}