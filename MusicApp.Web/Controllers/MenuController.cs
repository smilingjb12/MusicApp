using System.Web.Mvc;
using Business.Services;
using Data.Domain;
using DataAccess;

namespace SocialApp.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IUserService userService;

        public MenuController(IUserService userService)
        {
            this.userService = userService;
        }

        [ChildActionOnly]
        public PartialViewResult TopBar()
        {
            User user = userService.FindUserById(CurrentUserId);
            return PartialView(user);
        }

    }
}
