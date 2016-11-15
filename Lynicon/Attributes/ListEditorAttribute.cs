using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lynicon.Attributes
{
    /// <summary>
    /// Specifies that when a list editor is shown for a list of the type to which this is attached, use the specified view
    /// to show the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ListEditorAttribute : Attribute
    {
        /// <summary>
        /// The view name of the list editor view
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Create a list editor view
        /// </summary>
        /// <param name="viewName">The view name of the list editor view</param>
        public ListEditorAttribute(string viewName)
        {
            ViewName = viewName;
        }
    }
}
