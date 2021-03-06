﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Domain
{
    public class Song
    {
        public Song()
        {
            Likes = 0;
            Tags = new List<Tag>();
        }

        public int Id { get; set; }

        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string AlbumCoverPicturePath { get; set; }
        public int Bitrate { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public double FileSizeInMegaBytes { get; set; }

        public int Likes { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Title: {1}, Artist: {2}", Id, Title, Artist);
        }

        public int UploaderId { get; set; }
        public User Uploader { get; set; }

        public IList<Tag> Tags { get; set; }
    }
}
