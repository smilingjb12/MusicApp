using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Domain;
using DataAccess;
using System.Data.Entity;
using SocialApp.Models;

namespace SocialApp.Controllers
{
    [Authorize]
    public class MailController : BaseController
    {
        private readonly SocialAppContext db;

        public MailController(SocialAppContext db)
        {
            this.db = db;
        }

        public ViewResult Index(string tab)
        {
            var query = db.MailMessages
                          .Include(m => m.Sender)
                          .Include(m => m.Receiver)
                          .OrderByDescending(m => m.Date);
            var inbox = query.Where(m => m.Receiver.Id == CurrentUserId).ToList();
            var outbox = query.Where(m => m.Sender.Id == CurrentUserId).ToList();
            inbox.ForEach(msg => msg.IsInboxMessage = true); // kostyli-kostyliki
            var model = new MailIndexViewModel
            {
                ReceivedMessages = inbox,
                SentMessages = outbox,
                ActiveTab = tab
            };
            return View(model);
        }

        public ViewResult Create()
        {
            var model = new MailMessageViewModel
            {
                Users = db.Users.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(MailMessageViewModel message)
        {
            if (!ModelState.IsValid)
            {
                message.Users = db.Users.ToList();
                return View(message);
            }
            var msg = new MailMessage
            {
                Date = DateTime.Now,
                Sender = db.Users.Find(CurrentUserId),
                Receiver = db.Users.Find(message.ReceiverId),
                Text = message.Text
            };
            db.MailMessages.Add(msg);
            db.SaveChanges();
            TempData["success"] = string.Format("Your message has been successfully sent to {0}", msg.Receiver.FullName);
            return RedirectToAction("Index", new { tab = "outbox" });
        }
    }
}
