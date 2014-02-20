using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Domain
{
    public class MailMessage
    {
        public const int DisplayableTextSize = 223;

        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Required]
        public string Text { get; set; }

        public User Sender { get; set; }
        public User Receiver { get; set; }

        [NotMapped]
        public bool IsInboxMessage { get; set; }
    }
}
