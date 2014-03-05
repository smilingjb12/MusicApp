using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Business.ViewModels;
using Data.Domain;
using SocialApp.Models;

namespace Business.Services
{
    public interface IUserService
    {
        IEnumerable<Song> GetUploadedSongs(int userId);
        User FindUserByActivationCode(string activationCode);
        void ActivateUserAccount(User user);
        User FindUserByEmail(string email);
        User FindUserById(int id);
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetAllUsers(Expression<Func<User, bool>> predicate);
        User FindUserWithHostedRoom(int userId);
        void UpdateUserInfo(int userId, UserUpdateViewModel updateViewModel);
        void SendFriendRequest(int senderId, int receiverId);
        ProfileViewModel GetProfileInfoFor(int viewedUserId, int  currentUserId);
        FriendsViewModel GetFriendsFor(int currentUserId);
        IList<User> GetFriendRequestsFor(int userId);
        void AcceptFriendRequest(int requesterId, int currentUserId);
        void DeclineFriendRequest(int requesterId, int currentUserId);
        void RemoveFromFriends(int userId, int currentUserId);
        IEnumerable<WallMessage> GetWallMessagesForUser(int userId);
        void AddWallMessage(WallMessage wallMessage);
    }
}
