using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;

namespace Business.ViewModels
{
    public class FriendsViewModel
    {
        public IEnumerable<User> Friends { get; set; }
        public IEnumerable<User> FriendRequests { get; set; }
    }
}
