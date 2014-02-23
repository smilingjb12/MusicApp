using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;
using DataAccess;
using SocialApp.Models;
using System.Data.Entity;

namespace Business.Services
{
    public class MailService : IMailService
    {
        private readonly SocialAppContext db;
        private readonly IUserService userService;

        public MailService(SocialAppContext db, IUserService userService)
        {
            this.db = db;
            this.userService = userService;
        }

        public MailViewModel GetMailForUser(int userId)
        {
            var query = db.MailMessages
                          .Include(m => m.Sender)
                          .Include(m => m.Receiver)
                          .OrderByDescending(m => m.Date);
            var inbox = query.Where(m => m.Receiver.Id == userId).ToList();
            var outbox = query.Where(m => m.Sender.Id == userId).ToList();
            inbox.ForEach(msg => msg.IsInboxMessage = true); // kostyli-kostyliki
            var model = new MailViewModel
            {
                ReceivedMessages = inbox,
                SentMessages = outbox
            };
            return model;
        }

        public MailMessage AddMessage(int senderId, int receiverId, string text)
        {
            var msg = new MailMessage
            {
                Date = DateTime.Now,
                Sender = userService.FindUserById(senderId),
                Receiver = userService.FindUserById(receiverId),
                Text = text
            };
            db.MailMessages.Add(msg);
            db.SaveChanges();
            return msg;
        }
    }
}
