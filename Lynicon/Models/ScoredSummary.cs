using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Models
{
    /// <summary>
    /// A container for a summary together with a score for search
    /// </summary>
    /// <typeparam name="T">The type of the summary</typeparam>
    public class ScoredSummary<T> where T : Summary
    {
        /// <summary>
        /// The summary
        /// </summary>
        public T Summary { get; set; }
        /// <summary>
        /// The score for the summary
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Create a new ScoredSummary
        /// </summary>
        public ScoredSummary()
        { }
        /// <summary>
        /// Create a new ScoredSummary from the summary and a score
        /// </summary>
        /// <param name="summary">The summary</param>
        /// <param name="score">the score</param>
        public ScoredSummary(T summary, decimal score)
        {
            this.Summary = summary;
            this.Score = score;
        }
    }
}
