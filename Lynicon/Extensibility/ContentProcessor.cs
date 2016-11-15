using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Collation;
using Lynicon.Models;
using Lynicon.Repositories;
using System.Diagnostics;

namespace Lynicon.Extensibility
{
    /// <summary>
    /// Extension methods to run a process over all content items assignable to a type
    /// </summary>
    public static class ContentProcessor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void ProcessAllIfTime(this IContentProcessor contentProcessor)
        {
            contentProcessor.ProcessAll();
        }

        /// <summary>
        /// Process all content items using a contentProcessor
        /// </summary>
        /// <typeparam name="T">Type to which all content items must be assigned</typeparam>
        /// <param name="contentProcessor">Set of methods for processing content</param>
        public static void ProcessLoop<T>(this IContentProcessor contentProcessor) where T : class
        {
            log.InfoFormat("**Process all for {0} begins", contentProcessor.Name);

            var contentTypes = ContentTypeHierarchy.GetAssignableContentTypes(typeof(T));
            foreach (Type t in contentTypes)
            {
                Debug.WriteLine("Processing: " + t.FullName);

                foreach (object content in Collator.Instance.Get<object, object>(new Type[] { t }, iq => iq))
                {
                    try
                    {
                        contentProcessor.Process(content);
                        Collator.Instance.Set(null, content, new Dictionary<string, object> { { "setAudit", false } });
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Summary s = Collator.Instance.GetSummary<Summary>(content);
                            log.Error("Error processing content item " + s.Title + " for " + contentProcessor.Name, ex);
                        }
                        catch
                        {
                            log.Error("Error processing unidentified content item for " + contentProcessor.Name, ex);
                        }
                    }
                }
            }

            log.InfoFormat("**Process all for {0} ends", contentProcessor.Name);
        }
    }
}
