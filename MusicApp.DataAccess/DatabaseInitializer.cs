using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Domain;
using WebMatrix.WebData;

namespace DataAccess
{
    internal class DatabaseInitializer : DropCreateDatabaseIfModelChanges<SocialAppContext>
    {
        protected override void Seed(SocialAppContext context)
        {

        }
    }
}
