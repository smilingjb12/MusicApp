using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Domain;
using DataAccess.Configuration;

namespace DataAccess
{
    public class SocialAppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<MailMessage> MailMessages { get; set; }

        public SocialAppContext()
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false; 
        }

        public static void Initialize()
        {
            Database.SetInitializer(new DatabaseInitializer());
            using (var c = new SocialAppContext())
            {
                c.Database.Initialize(force: true);
                c.Database.CreateIfNotExists();
            } 
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new SongConfiguration());
            modelBuilder.Configurations.Add(new TagConfiguration());
            modelBuilder.Configurations.Add(new RoomConfiguration());
            modelBuilder.Configurations.Add(new MailMessageConfiguration());
        }
    }
}
