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
            Room room = db.Rooms.Include(r => r.Users)
                                .Include(r => r.PlaylistSongs.Select(ps => ps.Song))
                                .FirstOrDefault(r => r.Id == roomId);
            room.Users.Add(user);
            db.SaveChanges();
            Debug.WriteLine("{0} joined the room {1}", user, room);
            await Groups.Add(user.ConnectionId, room.GroupName);
            Clients.Group(room.GroupName).onUserJoined(new { username = user.FullName });
            SendUserListForRoom(room);
            Clients.Group(room.GroupName).onSkipVotesCountReceived(new {count = room.SkipVotes});
            await SendPlaylistForRoom(room);
            Clients.Caller.onCurrentSongTimeReceived(new { time = room.CurrentSongTime });
        }

        public void VoteSkipSong(int roomId, int voterId)
        {
            var room = db.Rooms.First(r => r.Id == roomId);
            var voter = db.Users.First(u => u.Id == voterId);
            room.SkipVotes++;
            db.SaveChanges();
            Clients.Group(room.GroupName)
                .onSkipVoteReceived(new {voter = voter.FullName});
            Clients.Group(room.GroupName)
                .onSkipVotesCountReceived(new { count = room.SkipVotes });
        }

        public void UpdateSongTime(int roomId, double newTime)
        {
            var room = db.Rooms.FirstOrDefault(r => r.Id == roomId);
            room.CurrentSongTime = newTime;
            db.SaveChanges();
        }

        public void SendMessage(int userId, int roomId, string message)
        {
            User user = db.Users.Find(userId);
            Room room = db.Rooms.Find(roomId);
            Debug.WriteLine("Sending text message from user {0} to room {1}", user, room);
            Clients.Group(room.GroupName).onMessageReceived(new { text = message, username = user.FullName });
        }

        public void SetCurrentSongIndex(int userId, int roomId, int index)
        {
            Debug.WriteLine("Setting current song index to {0}", index);
            Room room = db.Rooms.FirstOrDefault(r => r.Id == roomId);
            User changer = db.Users.FirstOrDefault(u => u.Id == userId);
            room.CurrentSongIndex = index;
            room.CurrentSongTime = 0;
            room.SkipVotes = 0;
            db.SaveChanges();
            Clients.Group(room.GroupName).onSkipVotesCountReceived(new { count = room.SkipVotes });
            Clients.Group(room.GroupName).onCurrentSongChanged(new
            {
                whoChanged = changer.FullName,
                index
            });
        }

        public void AddSongToPlaylist(int songId, int roomId)
        {
            Song song = db.Songs.Find(songId);
            Debug.WriteLine("Adding {0} to playlist", song);
            Room room = db.Rooms.Include(r => r.PlaylistSongs.Select(ps => ps.Song))
                                .FirstOrDefault(r => r.Id == roomId);
            room.PlaylistSongs.Add(new PlaylistSong { Room = room, Song = song });
            db.SaveChanges();
            User currentUser = GetCurrentUser();
            Clients.Group(room.GroupName).onSongAddedToPlaylist(currentUser.FullName, song);
        }

        public override Task OnDisconnected()
        {
            User user = GetCurrentUser();
            Debug.WriteLine("{0} has disconnected", user);
            Room room = FindRoomWithUser(user);
            if (user.HostedRoom == room) // leaving user is the host
            {
                Debug.WriteLine("Leaving user was the host");
                Clients.Group(room.GroupName).onRoomDestroyed();
                user.HostedRoom = null;
                room.Users.Clear();
                db.Rooms.Remove(room);
            }
            else
            {
                Debug.WriteLine("Leaving user was not the host");
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
        private User GetCurrentUser()
        {
            return db.Users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
        }

        private Room FindRoomWithUser(User user)
        {
            return db.Rooms.Include(r => r.Users)
                           .FirstOrDefault(r => r.Users.Any(u => u.Id == user.Id));
        }

        private async Task SendPlaylistForRoom(Room room)
        {
            using (var db = new SocialAppContext())
            {
                User user = GetCurrentUser();
                room = db.Rooms.Include("PlaylistSongs.Song")
                               .First(r => r.Id == room.Id);
                var playlistData = new 
                {
                    playlist = room.PlaylistSongs.Select(ps => ps.Song),
                    username = user.FullName,
                    currentSongIndex = room.CurrentSongIndex
                };
                Debug.WriteLine("Sending playlist for room {0}", room);
                await Clients.Caller.onPlaylistReceived(playlistData);
            }
        }

        private void SendUserListForRoom(Room room)
        {
            var usersInRoom = room.Users
                .Select(u => new { username = u.FullName, picture = u.PictureFilePath, id = u.Id });
            Debug.WriteLine("Sending userlist for room {0}", room);
            Clients.Group(room.GroupName).onUserListReceived(usersInRoom);
        }
        #endregion
    }
}