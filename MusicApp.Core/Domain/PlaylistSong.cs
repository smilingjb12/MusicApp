using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Domain
{
    public class PlaylistSong
    {
        public int Id { get; set; }
        public Song Song { get; set; }
        public Room Room { get; set; }
    }
}
