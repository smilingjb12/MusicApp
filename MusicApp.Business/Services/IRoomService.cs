using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;
using SocialApp.Models;

namespace Business.Services
{
    public interface IRoomService
    {
        IEnumerable<Room> GetAllRoomsWithHosts();
        void HostRoom(int userId, Room room);
        Room FindById(int id);
    }
}
