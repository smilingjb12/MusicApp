using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Services;
using Data.Domain;
using DataAccess;
using System.Data.Entity;
using SocialApp.Models;

namespace SocialApp.Controllers
{
    [Authorize]
    public class MailController : BaseController
    {
        private readonly IMailService mailService;
        private readonly IUserService userService;

        public MailController(SocialAppContext db, IMailService mailService, IUserService userService)
        {
            this.mailService = mailService;
            this.userService = userService;
        }

        public ViewResult Index(string tab)
        {
            var model = new MailIndexViewModel
            {
                MailViewModel = mailService.GetMailForUser(CurrentUserId),
                ActiveTab = tab
            };
            mailService.MarkAllMessagesAsReadForUser(CurrentUserId);
            return View(model);
        }

        public ViewResult Create()
        {
            var model = new MailMessageViewModel
            {
                Users = userService.GetAllUsers()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(MailMessageViewModel message)
        {
            if (!ModelState.IsValid)
            {
                message.Users = userService.GetAllUsers();
                return View(message);
            }
            var msg = mailService.AddMessage(
                senderId: CurrentUserId, 
                receiverId: (int)message.ReceiverId, 
                text: message.Text
            );
            TempData["success"] = string.Format("Your message has been successfully sent to {0}", msg.Receiver.FullName);
            return RedirectToAction("Index", new { tab = "outbox" });
        }
    }
}
