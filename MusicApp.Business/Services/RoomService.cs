using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;
using DataAccess;
using System.Data.Entity;
using SocialApp.Models;

namespace Business.Services
{
    public class RoomService : IRoomService
    {
        private readonly SocialAppContext db;

        public RoomService(SocialAppContext db)
        {
            this.db = db;
        }

        public IEnumerable<Room> GetAllRoomsWithHosts()
        {
            return db.Rooms.Include(r => r.Host).ToList();
        }

        public void HostRoom(int userId, Room room)
        {
            User host = db.Users
                .Include(u => u.HostedRoom)
                .FirstOrDefault(u => u.Id == userId);
            host.HostedRoom = room;
            db.SaveChanges();
        }

        public Room FindById(int id)
        {
            return db.Rooms.Find(id);
        }
    }
}
