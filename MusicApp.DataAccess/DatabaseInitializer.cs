using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Domain;

namespace DataAccess
{
    internal class DatabaseInitializer : DropCreateDatabaseIfModelChanges<SocialAppContext>
    {
        protected override void Seed(SocialAppContext context)
        {

        }
    }
}
