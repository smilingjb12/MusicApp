using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Data
{
    public class Id3Info
    {
        public int Bitrate { get; set; }
        public TimeSpan Duration { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public IPicture Picture { get; set; }
    }
}
