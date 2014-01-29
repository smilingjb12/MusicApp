using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Domain
{
    public class User
    {
        public User()
        {
            Role = Role.User;    
            UploadedSongs = new List<Song>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Email Is required")]
        [RegularExpression(@".+\@.+\..+", ErrorMessage = "Please input valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string ActivationCode { get; set; }
        public bool IsActivated { get; set; }
        public Role Role { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string About { get; set; }
        public string PictureFilePath { get; set; }

        public virtual IList<Song> UploadedSongs { get; set; } 

        [NotMapped]
        public string Login
        {
            get
            {
                return Email.Substring(0, Email.IndexOf('@'));
            }
        }
    }
}
