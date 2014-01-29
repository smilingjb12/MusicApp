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
    class TagConfiguration : EntityTypeConfiguration<Tag>
    {
        public TagConfiguration()
        {

        }
    }
}
