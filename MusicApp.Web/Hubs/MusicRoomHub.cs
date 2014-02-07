using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Data.Domain;
using DataAccess;
using Microsoft.AspNet.SignalR;
using Ninject;
using System.Data.Entity;

namespace SocialApp.Hubs
{
    public class MusicRoomHub : Hub
    {
        private readonly SocialAppContext db = new SocialAppContext(); // DI doesn't work :(

        public async void JoinRoom(int userId, int roomId)
        {
            User user = db.Users.Find(userId);
            user.ConnectionId = Context.ConnectionId;
            Room room = db.Rooms
                .Include(r => r.Users)
                .FirstOrDefault(r => r.Id == roomId);
            room.Users.Add(user);
            db.SaveChanges();
            await Groups.Add(user.ConnectionId, roomId.ToString());
            Clients.Group(roomId.ToString()).onUserJoined(new { username = user.FullName });
            SendUserListForRoom(room);
        }

        public void SendMessage(int userId, int roomId, string message)
        {
            User user = db.Users.Find(userId);
            Clients.Group(roomId.ToString()).onMessageReceived(new { text = message, username = user.FullName });
        }

        public override Task OnDisconnected()
        {
            User user = db.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            Room room = db.Rooms
                .Include(r => r.Users)
                .FirstOrDefault(r => r.Users.Any(u => u.Id == user.Id));
            if (user.HostedRoom == room) // leaving user is the host
            {
                Clients.Group(room.Id.ToString()).onRoomDestroyed();
                user.HostedRoom = null;
                room.Users.Clear();
                db.Rooms.Remove(room);
            }
            else
            {
                room.Users.Remove(user);
                Clients.Group(room.Id.ToString()).onUserLeft(new { username = user.FullName });
                Groups.Remove(user.ConnectionId, room.Id.ToString());
                user.ConnectionId = null;
                SendUserListForRoom(room);
            }
            db.SaveChanges();
            return base.OnDisconnected();
        }

        private void SendUserListForRoom(Room room)
        {
            var usersInRoom = room.Users
                .Select(u => new { username = u.FullName, picture = u.PictureFilePath });
            Clients.Group(room.Id.ToString()).onUserListReceived(usersInRoom);
        }
    }
}