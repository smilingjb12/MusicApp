using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using AutoMapper;
using Business.ViewModels;
using Data;
using Data.Domain;
using DataAccess;
using SocialApp.Models;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly SocialAppContext db;

        public UserService(SocialAppContext db)
        {
            this.db = db;
        }

        public IEnumerable<Song> GetUploadedSongs(int userId)
        {
            return db.Songs
                .Include(song => song.Tags)
                .Where(song => song.UploaderId == userId)
                .ToList();
        }

        public User FindUserByActivationCode(string activationCode)
        {
            return db.Users.FirstOrDefault(u => u.ActivationCode == activationCode);
        }

        public User FindUserByEmail(string email)
        {
            return db.Users.FirstOrDefault(u => u.Email == email);
        }

        public User FindUserById(int id)
        {
            return db.Users.Find(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return db.Users.ToList();
        }

        public User FindUserWithHostedRoom(int userId)
        {
            User user = db.Users
                          .Include(u => u.HostedRoom)
                          .FirstOrDefault(u => u.Id == userId);
            return user;
        }

        public void UpdateUserInfo(int userId, UserUpdateViewModel updateViewModel)
        {
            User user = FindUserById(userId);
            Mapper.Map(updateViewModel, user);
            db.SaveChanges();
        }

        public void SendFriendRequest(int senderId, int receiverId)
        {
            User sender = db.Users.Find(senderId);
            User receiver = db.Users.Find(receiverId);
            receiver.FriendRequests.Add(sender);
            db.SaveChanges();
        }

        public ProfileViewModel GetProfileInfoFor(int viewedUserId, int currentUserId)
        {
            User viewedUser = db.Users
                                .Include(u => u.Friends)
                                .Include(u => u.FriendRequests)
                                .FirstOrDefault(u => u.Id == viewedUserId);
            if (viewedUser == null) return null;
            var profile = new ProfileViewModel
            {
                ViewedUser = viewedUser,
                IsMyProfile = viewedUserId == currentUserId,
                CurrentUserId = currentUserId,
                WallMessages = GetWallMessagesForUser(viewedUserId),
                NewWallMessage = new WallMessage
                {
                    ReceiverId = viewedUserId,
                    SenderId = currentUserId
                }
            };
            if (profile.IsMyProfile) return profile; // no need for extra logic
            if (viewedUser.FriendRequests.Any(f => f.Id == currentUserId))
            {
                profile.RelationshipStatus = RelationshipStatus.RequestedFriend;
            }
            else if (viewedUser.Friends.Any(f => f.Id == currentUserId))
            {
                profile.RelationshipStatus = RelationshipStatus.Friend;
            }
            return profile;
        }

        public FriendsViewModel GetFriendsFor(int currentUserId)
        {
            User currentUser = db.Users
                                 .Include(u => u.Friends)
                                 .Include(u => u.FriendRequests)
                                 .FirstOrDefault(u => u.Id == currentUserId);
            if (currentUser == null) return null;
            var friendsViewModel = new FriendsViewModel
            {
                Friends = currentUser.Friends,
                FriendRequests = currentUser.FriendRequests
            };
            return friendsViewModel;
        }

        public IList<User> GetFriendRequestsFor(int userId)
        {
            User user = db.Users
                          .Include(u => u.FriendRequests)
                          .FirstOrDefault(u => u.Id == userId);
            return user.FriendRequests;
        }

        public void AcceptFriendRequest(int requesterId, int currentUserId)
        {
            User requester = db.Users
                               .Include(u => u.Friends)
                               .FirstOrDefault(u => u.Id == requesterId);
            User currentUser = db.Users
                                 .Include(u => u.FriendRequests)
                                 .Include(u => u.Friends)
                                 .FirstOrDefault(u => u.Id == currentUserId);
            currentUser.FriendRequests.Remove(requester);
            currentUser.Friends.Add(requester);
            requester.Friends.Add(currentUser);
            db.SaveChanges();
        }

        public void DeclineFriendRequest(int requesterId, int currentUserId)
        {
            User requester = db.Users.Find(requesterId);
            User currentUser = db.Users.Include(u => u.FriendRequests).FirstOrDefault();
            currentUser.FriendRequests.Remove(requester);
            db.SaveChanges();
        }

        public void RemoveFromFriends(int userId, int currentUserId)
        {
            var users = db.Users
                          .Include(u => u.Friends)
                          .Where(u => u.Id == userId || u.Id == currentUserId)
                          .ToList();
            users[0].Friends.Remove(users[1]);
            users[1].Friends.Remove(users[0]);
            db.SaveChanges();
        }

        public IEnumerable<WallMessage> GetWallMessagesForUser(int userId)
        {
            return db.WallMessages
                     .Include(m => m.Sender)
                     .Where(m => m.ReceiverId == userId)
                     .OrderByDescending(m => m.Date)
                     .ToList();
        }

        public void AddWallMessage(WallMessage wallMessage)
        {
            wallMessage.Date = DateTime.Now;
            db.WallMessages.Add(wallMessage);
            db.SaveChanges();
        }

        public void ActivateUserAccount(User user)
        {
            user.IsActivated = true;
            db.SaveChanges();
        }
    }
}
