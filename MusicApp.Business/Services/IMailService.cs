using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;
using SocialApp.Models;

namespace Business.Services
{
    public interface IMailService
    {
        MailViewModel GetMailForUser(int userId);
        MailMessage AddMessage(int senderId, int receiverId, string text);
        IEnumerable<MailMessage> GetUnreadMessagesForUser(int userId);
        void MarkAllMessagesAsReadForUser(int userId);
    }
}
