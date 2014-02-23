using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data.Domain;

namespace SocialApp.Models
{
    public class MailViewModel
    {
        public IEnumerable<MailMessage> ReceivedMessages { get; set; }
        public IEnumerable<MailMessage> SentMessages { get; set; }
    }
}