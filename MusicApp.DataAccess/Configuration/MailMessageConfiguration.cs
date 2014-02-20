using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;

namespace DataAccess.Configuration
{
    public class MailMessageConfiguration : EntityTypeConfiguration<MailMessage>
    {
        public MailMessageConfiguration()
        {
            this.HasRequired(m => m.Sender)
                .WithMany()
                .WillCascadeOnDelete(false);

            this.HasRequired(m => m.Receiver)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}
