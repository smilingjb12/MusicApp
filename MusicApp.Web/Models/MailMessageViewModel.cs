using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Data.Domain;

namespace SocialApp.Models
{
    public class MailMessageViewModel
    {
        public IEnumerable<User> Users { get; set; }

        [Required(ErrorMessage = "Choose a receiver please")]
        public int? ReceiverId { get; set; }

        [Required]
        public string Text { get; set; }
    }
}