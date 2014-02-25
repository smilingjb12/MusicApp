using System.Web.Mvc;
using Business.Services;
using Data.Domain;
using DataAccess;
using SocialApp.Models;

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
            if (user == null) return PartialView(new MenuTopBarViewModel());
            var viewModel = new MenuTopBarViewModel
            {
                CurrentUser = user,
                FriendRequests = userService.GetFriendRequestsFor(CurrentUserId)
            };
            return PartialView(viewModel);
        }

    }
}
