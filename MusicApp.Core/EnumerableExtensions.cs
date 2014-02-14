using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class EnumerableExtensions
    {
        private static readonly Random rand = new Random(DateTime.Now.Millisecond);

        public static T Sample<T>(this IEnumerable<T> seq)
        {
            int size = seq.Count();
            if (size == 0) return default(T);
            return seq.ElementAt(rand.Next(size));
        }
    }
}
