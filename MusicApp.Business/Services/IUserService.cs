using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Domain;

namespace Business.Services
{
    public interface IUserService
    {
        IEnumerable<Song> GetUploadedSongs(int userId);
    }
}
