using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialApp.Models
{
    public interface IEmailSender
    {
        void SendActivationCode(string email, string activationCode);
    }
}