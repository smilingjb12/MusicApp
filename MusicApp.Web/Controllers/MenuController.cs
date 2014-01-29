using System.Web.Mvc;
using Data.Domain;
using DataAccess;

namespace SocialApp.Controllers
{
    public class MenuController : BaseController
    {
        private readonly SocialAppContext db;

        public MenuController(SocialAppContext db)
        {
            this.db = db;
        }

        public PartialViewResult TopBar()
        {
            User user = db.Users.Find(CurrentUserId);
            return PartialView(user);
        }

    }
}
