using System.Linq;
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
        private readonly IMailService mailService;

        public MenuController(IUserService userService, IMailService mailService)
        {
            this.userService = userService;
            this.mailService = mailService;
        }

        [ChildActionOnly]
        public PartialViewResult TopBar()
        {
            User user = userService.FindUserById(CurrentUserId);
            if (user == null)
            {
                return PartialView(new MenuTopBarViewModel());
            }
            var viewModel = new MenuTopBarViewModel
            {
                CurrentUser = user,
                FriendRequests = userService.GetFriendRequestsFor(CurrentUserId),
                UnreadMessages = mailService.GetUnreadMessagesForUser(CurrentUserId)
            };
            return PartialView(viewModel);
        }

    }
}
