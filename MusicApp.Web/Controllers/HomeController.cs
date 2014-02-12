using System.Web.Mvc;
using DataAccess;

namespace SocialApp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Show", "User", new { id = CurrentUserId });
            }
            return View();
        }
    }
}
