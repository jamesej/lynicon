using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Models
{
    /// <summary>
    /// Comparer for two summaries by id (Identity)
    /// </summary>
    /// <typeparam name="TSumm">The type of the summary</typeparam>
    public class SummaryEqualityComparer<TSumm> : IEqualityComparer<TSumm>
        where TSumm : Summary
    {
        #region IEqualityComparer<TSumm> Members

        public bool Equals(TSumm x, TSumm y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(TSumm obj)
        {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}
