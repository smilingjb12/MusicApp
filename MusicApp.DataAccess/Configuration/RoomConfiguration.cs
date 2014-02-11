using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;

namespace DataAccess.Configuration
{
    class RoomConfiguration : EntityTypeConfiguration<Room>
    {
        public RoomConfiguration()
        {
            HasRequired(r => r.Host).WithOptional(u => u.HostedRoom);
            HasMany(r => r.PlaylistSongs).WithOptional();
        }
    }
}
