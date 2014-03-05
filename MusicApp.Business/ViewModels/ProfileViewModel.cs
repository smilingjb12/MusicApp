using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business;
using Data.Domain;

namespace SocialApp.Models
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
            RelationshipStatus = RelationshipStatus.Stranger;
        }

        public User ViewedUser { get; set; }
        public bool IsMyProfile { get; set; }
        public int CurrentUserId { get; set; }
        public IEnumerable<WallMessage> WallMessages { get; set; }
        public WallMessage NewWallMessage { get; set; }

        public bool IsNotMyProfile
        {
            get { return !IsMyProfile; }
        }

        public RelationshipStatus RelationshipStatus { get; set; }
    }
}