using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Data.Domain;
using DataAccess;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            await Groups.Add(user.ConnectionId, room.GroupName);
            Clients.Group(room.GroupName).onUserJoined(new { username = user.FullName });
            SendUserListForRoom(room);
            SendPlaylistForRoom(room);
        }
        
        public void AddSongToPlaylist(int songId, int roomId)
        {
            Song song = db.Songs.Find(songId);
            Room room = db.Rooms.Include(r => r.PlaylistSongs.Select(ps => ps.Song))
                .FirstOrDefault(r => r.Id == roomId);
            room.PlaylistSongs.Add(new PlaylistSong { Room = room, Song = song });
            db.SaveChanges();
            SendPlaylistForRoom(room);
        }

        public override Task OnDisconnected()
        {
            User user = db.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            Room room = FindRoomWithUser(user);
            if (user.HostedRoom == room) // leaving user is the host
            {
                Clients.Group(room.GroupName).onRoomDestroyed();
                user.HostedRoom = null;
                room.Users.Clear();
                db.Rooms.Remove(room);
            }
            else
            {
                room.Users.Remove(user);
                Clients.Group(room.GroupName).onUserLeft(new { username = user.FullName });
                Groups.Remove(user.ConnectionId, room.GroupName);
                user.ConnectionId = null;
                SendUserListForRoom(room);
            }
            db.SaveChanges();
            return base.OnDisconnected();
        }

        #region Helper Methods
        private Room FindRoomWithUser(User user)
        {
            return db.Rooms.Include(r => r.Users)
                .FirstOrDefault(r => r.Users.Any(u => u.Id == user.Id));
        }

        private void SendPlaylistForRoom(Room room)
        {
            var playlist = ToObjectWithCamelCaseProperties(room.PlaylistSongs.Select(ps => ps.Song));
            Clients.Group(room.GroupName).onPlaylistReceived(playlist);
        }

        private void SendUserListForRoom(Room room)
        {
            var usersInRoom = room.Users
                .Select(u => new { username = u.FullName, picture = u.PictureFilePath });
            Clients.Group(room.GroupName).onUserListReceived(usersInRoom);
        }

        private object ToObjectWithCamelCaseProperties(object obj)
        {
            string json = JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return JsonConvert.DeserializeObject(json);
        }

        public void SendMessage(int userId, int roomId, string message)
        {
            User user = db.Users.Find(userId);
            Room room = db.Rooms.Find(roomId);
            Clients.Group(room.GroupName).onMessageReceived(new { text = message, username = user.FullName });
        }
        #endregion
    }
}