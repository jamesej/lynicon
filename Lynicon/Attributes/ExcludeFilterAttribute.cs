using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lynicon.Attributes
{
    /// <summary>
    /// Allows a filter to be removed from the action method or class to which this attribute is attached
    /// </summary>
    public class ExcludeFilterAttribute : FilterAttribute
    {
        private Type filterType;

        /// <summary>
        /// Create an exclude filter attribute
        /// </summary>
        /// <param name="filterType">The type of the filter to remove from acting on the action method on class to which this is attached</param>
        public ExcludeFilterAttribute(Type filterType)
        {
            this.filterType = filterType;
        }

        /// <summary>
        /// The type of filter to remove from acting on the action method on class to which this is attached
        /// </summary>
        public Type FilterType
        {
            get
            {
                return this.filterType;
            }
        }
    } 
}
