using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        User FindUserWithHostedRoom(int userId);
        void UpdateUserInfo(int userId, UserUpdateViewModel updateViewModel);
        void SendFriendRequest(int senderId, int receiverId);
        ProfileViewModel GetProfileInfoFor(int userId, int  currentUserId);
    }
}
