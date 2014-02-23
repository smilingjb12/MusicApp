using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using AutoMapper;
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

        public void UpdateUserInfo(int userId, UserUpdateModel updateModel)
        {
            User user = FindUserById(userId);
            Mapper.Map(updateModel, user);
            db.SaveChanges();
        }

        public void ActivateUserAccount(User user)
        {
            user.IsActivated = true;
            db.SaveChanges();
        }
    }
}
