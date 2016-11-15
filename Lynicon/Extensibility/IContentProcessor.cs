using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Extensibility
{
    /// <summary>
    /// A class providing methods for bulk processing content
    /// </summary>
    public interface IContentProcessor
    {
        /// <summary>
        /// Process a content item
        /// </summary>
        /// <typeparam name="T">We are processing all content items assignable to type T</typeparam>
        /// <param name="content">The content item</param>
        /// <returns></returns>
        T Process<T>(T content) where T : class;

        /// <summary>
        /// Method to process all the content
        /// </summary>
        void ProcessAll();

        /// <summary>
        /// Minimum period of time between consecutive runs of the process
        /// </summary>
        TimeSpan MinReprocessDelay { get; set; }

        /// <summary>
        /// Name of the content processor
        /// </summary>
        string Name { get; }
    }
}
