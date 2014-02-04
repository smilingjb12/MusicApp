using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IMp3Parser
    {
        Id3Info ExtractId3Info(string filePath);
    }
}
