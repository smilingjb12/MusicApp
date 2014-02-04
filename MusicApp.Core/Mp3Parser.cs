using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Data
{
    public class Mp3Parser : IMp3Parser
    {
        public Id3Info ExtractId3Info(string filePath)
        {
            var info = new Id3Info();
            using (File tagFile = File.Create(filePath))
            {
                info.Bitrate = tagFile.Properties.AudioBitrate;
                info.Duration = tagFile.Properties.Duration;
                info.Title = tagFile.Tag.Title ?? "Song Title";
                info.Artist = tagFile.Tag.FirstAlbumArtist ?? tagFile.Tag.FirstArtist ?? "Song Artist";
                info.Album = tagFile.Tag.Album ?? "Song Album Title";
                info.Picture = tagFile.Tag.Pictures.FirstOrDefault();
            }
            return info;
        }
    }
}
