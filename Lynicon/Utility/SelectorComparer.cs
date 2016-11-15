using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Utility
{
    /// <summary>
    /// Comparer using a selector to extract values to be compared
    /// </summary>
    /// <typeparam name="T">The type to compare</typeparam>
    /// <typeparam name="TProp">The type of the extracted value used in comparison</typeparam>
    public class SelectorComparer<T, TProp> : IEqualityComparer<T>, IComparer<T>
    {
        private Func<T, TProp> selector;

        /// <summary>
        /// Create a comparer based on supplied Func
        /// </summary>
        /// <param name="compare">compare returns -ve if first arg less than second, zero if equal, positive if first more than second</param>
        public SelectorComparer(Func<T, TProp> selector)
        {
            this.selector = selector;
        }

        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            return selector(obj).GetHashCode();
        }

        #endregion

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return Comparer<TProp>.Default.Compare(selector(x), selector(y));
        }

        #endregion
    }
}
