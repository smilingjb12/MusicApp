using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;
using DataAccess;
using System.Data.Entity;

namespace Business.Services
{
    public class SongService : ISongService
    {
        private readonly SocialAppContext db;
        private readonly ITagService tagService;

        public SongService(SocialAppContext db, ITagService tagService)
        {
            this.db = db;
            this.tagService = tagService;
        }

        public bool UpdateSongInfo(Song song)
        {
            Song existingSong = db.Songs
                .Include(s => s.Tags)
                .FirstOrDefault(s => s.Id == song.Id);
            if (existingSong == null) return false;

            existingSong.Title = song.Title;
            existingSong.Artist = song.Artist;
            existingSong.Album = song.Album;
            existingSong.Tags = tagService
                .GetOrCreateTags(song.Tags.Select(t => t.Name))
                .ToList();

            db.SaveChanges();
            return true;
        }

        public IEnumerable<Song> SearchByTerm(string term)
        {
            term = term.ToLower();
            var songs = db.Songs.Where(s => s.Artist.ToLower().Contains(term) || 
                                            s.Title.ToLower().Contains(term));
            return songs.ToList();
        }

        public void DeleteSong(int songId)
        {
            Song song = db.Songs.Find(songId);
            db.Songs.Remove(song);
            db.SaveChanges();
        }

        public Song FindSongById(int songId)
        {
            Song song = db.Songs.Find(songId);
            return song;
        }

        public void AddSong(Song song)
        {
            db.Songs.Add(song);
            db.SaveChanges();
        }
    }
}
