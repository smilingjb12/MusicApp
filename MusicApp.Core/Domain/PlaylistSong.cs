using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Domain
{
    /// <summary>
    /// Used to enable adding same songs
    /// to playlist.
    /// </summary>
    public class PlaylistSong
    {
        public int Id { get; set; }

        public Room Room { get; set; }
        public Song Song { get; set; }
    }
}
