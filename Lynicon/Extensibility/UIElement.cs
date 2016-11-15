using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Membership;
using Lynicon.Utility;

namespace Lynicon.Extensibility
{
    /// <summary>
    /// Base class for data describing a UI element in the Lynicon UI
    /// </summary>
    public class UIElement
    {
        /// <summary>
        /// The section of the UI in which the element appears
        /// 'Global' for all locations in the UI, 'Record' just for when a record is being edited
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// The view name to render this element
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// The content permission describing when to display the element
        /// </summary>
        public ContentPermission DisplayPermission { get; set; }

        /// <summary>
        /// Apply macro substitutions to a string based on things in the ViewContext
        /// </summary>
        /// <param name="s">The string with macros</param>
        /// <param name="viewContext">The ViewContext in which the string will be displayed</param>
        /// <returns>The string with macros substituted</returns>
        public string ApplySubstitutions(string s, ViewContext viewContext)
        {
            dynamic viewBag = viewContext.ViewBag;
            string subs = (s ?? "")
             .Replace("$$CurrentUrl$$", viewContext.HttpContext.Request.Url.OriginalString)
             .Replace("$$BaseUrl$$", viewBag._Lyn_BaseUrl)
             .Replace("$$OriginalQuery$$", viewBag.OriginalQuery);

            if (viewContext.ViewData.Model != null)
            {
                var address = new Address(viewContext.ViewData.Model.GetType().ContentType(), viewContext.RouteData);
                subs = subs
                    .Replace("$$Path$$", address.GetAsContentPath())
                    .Replace("$$Type$$", address.Type.FullName);
            }

            return subs;
        }
    }
}
