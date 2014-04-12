using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class StringExtensions
    {
        public static string TruncateTo(this string str, int symbols)
        {
            return string.Join("", str.ToCharArray().Take(symbols));
        }
    }
}
