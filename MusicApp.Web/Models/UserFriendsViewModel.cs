using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.ViewModels;

namespace SocialApp.Models
{
    public class UserFriendsViewModel
    {
        public FriendsViewModel FriendsViewModel { get; set; }
        public string ActiveTab { get; set; }
    }
}