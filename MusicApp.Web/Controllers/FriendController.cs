using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Services;
using PagedList;

namespace SocialApp.Controllers
{
    public class FriendController : Controller
    {
        const int PageSize = 20;
        private readonly IUserService userService;

        public FriendController(IUserService userService)
        {
            this.userService = userService;
        }

        public ViewResult List(int userId, int page)
        {
            var friends = userService.GetAllUsers(u => u.Id == userId);
            return View(friends.ToPagedList(page, PageSize));
        }

    }
}
