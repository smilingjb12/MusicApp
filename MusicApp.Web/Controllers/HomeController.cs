using System.Web.Mvc;
using DataAccess;

namespace SocialApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly SocialAppContext db;

        public HomeController(SocialAppContext db)
        {
            this.db = db;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Show", "User", new { id = CurrentUserId });
            }
            return View();
        }

        public ViewResult Chat()
        {
            return View();
        }
    }
}
