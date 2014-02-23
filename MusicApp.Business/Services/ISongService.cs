using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;

namespace Business.Services
{
    public interface ISongService
    {
        bool UpdateSongInfo(Song song);
        IEnumerable<Song> SearchByTerm(string term);
        void DeleteSong(int songId);
        Song FindSongById(int songId);
        void AddSong(Song song);
    }
}
