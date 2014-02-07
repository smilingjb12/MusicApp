using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data.Domain;

namespace SocialApp.Models
{
    public class RoomDetailsViewModel
    {
        public Room Room { get; set; }
        public User CurrentUser { get; set; }
        public bool CurrentUserIsHost { get; set; }
    }
}