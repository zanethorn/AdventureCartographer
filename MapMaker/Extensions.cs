using System;
using System.Collections.Generic;
using System.Linq;

namespace MapMaker
{
    public static class Extensions
    {
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> target)
            where T : ICloneable
        {
            return target.Select(i => (T) i.Clone());
        }
    }
}