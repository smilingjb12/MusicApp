using System.Web;

namespace SocialApp.Models
{
    public class UserUpdateViewModel
    {
        public string FullName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string About { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        public string PictureFilePath { get; set; }
    }
}