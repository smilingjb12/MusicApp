using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Domain
{
    public class Room
    {
        public Room()
        {
            Users = new List<User>();
            PlaylistSongs = new List<PlaylistSong>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [MaxLength(40)]
        public string Description { get; set; }

        public User Host { get; set; }
        public IList<User> Users { get; set; }
        public IList<PlaylistSong> PlaylistSongs { get; set; } 
    
        [NotMapped]
        public String GroupName
        {
            get { return Id.ToString(); }
        }
    }
}
