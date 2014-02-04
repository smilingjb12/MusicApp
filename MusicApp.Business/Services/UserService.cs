using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;
using DataAccess;

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
    }
}
