using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Domain
{
    public class WallMessage
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(441)]
        public string Text { get; set; }

        public User Receiver { get; set; }
        public int ReceiverId { get; set; }

        public User Sender { get; set; }
        public int SenderId { get; set; }
    }
}
