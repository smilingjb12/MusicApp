using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string input, string term)
        {
            return input.IndexOf(term, StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
}
