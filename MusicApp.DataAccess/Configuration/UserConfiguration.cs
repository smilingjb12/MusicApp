using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Domain;

namespace DataAccess.Configuration
{
    class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            this.HasMany(u => u.UploadedSongs)
                .WithRequired(s => s.Uploader)
                .HasForeignKey(s => s.UploaderId);

            this.HasMany(u => u.Friends)
                .WithMany()
                .Map(m => m.ToTable("Friends"));

            this.HasMany(u => u.FriendRequests)
                .WithMany()
                .Map(m => m.ToTable("FriendRequests"));

            this.HasMany(u => u.UnreadMessages)
                .WithOptional();
        }
    }
}
