using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data.Domain;

namespace SocialApp.Models
{
    public class MenuTopBarViewModel
    {
        public User CurrentUser { get; set; }
        public IList<User> FriendRequests { get; set; }
    }
}