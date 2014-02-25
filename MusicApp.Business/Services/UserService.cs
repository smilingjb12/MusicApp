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

        public ProfileViewModel GetProfileInfoFor(int userId, int currentUserId)
        {
            User viewedUser = db.Users
                                .Include(u => u.Friends)
                                .Include(u => u.FriendRequests)
                                .FirstOrDefault(u => u.Id == userId);
            if (viewedUser == null) return null;
            var profile = new ProfileViewModel
            {
                ViewedUser = viewedUser,
                IsMyProfile = userId == currentUserId,
                CurrentUserId = currentUserId
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

        public void DeclineUserRequest(int requesterId, int currentUserId)
        {
            User requester = db.Users.Find(requesterId);
            User currentUser = db.Users.Include(u => u.FriendRequests).FirstOrDefault();
            currentUser.FriendRequests.Remove(requester);
            db.SaveChanges();
        }

        public void RemoveFromFriends(int userId, int currentUserId)
        {
            User currentUser = db.Users
                                 .Include(u => u.Friends)
                                 .FirstOrDefault(u => u.Id == currentUserId);
            User removedUser = db.Users
                                 .Include(u => u.Friends)
                                 .FirstOrDefault(u => u.Id == userId);
            currentUser.Friends.Remove(removedUser);
            removedUser.Friends.Remove(currentUser);
            db.SaveChanges();
        }

        public void ActivateUserAccount(User user)
        {
            user.IsActivated = true;
            db.SaveChanges();
        }
    }
}
