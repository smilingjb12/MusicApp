using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;

namespace DataAccess.Configuration
{
    class PlaylistSongConfiguration : EntityTypeConfiguration<PlaylistSong>
    {
        public PlaylistSongConfiguration()
        {
            HasRequired(ps => ps.Song).WithMany();
        }
    }
}
