using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Lynicon.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class ViewX
    {
        public static MvcHtmlString GetHtmlAttributes<T>(this ViewDataDictionary<T> dict, string attributes)
        {
            string html = attributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim() == "class" ? "cssClass" : a.Trim())
                .Where(a => dict.ContainsKey(a))
                .Select(a => (a == "cssClass" ? "class" : a) + "='" + HttpUtility.HtmlAttributeEncode(dict[a].ToString()) + "'")
                .Join(" ");
            return MvcHtmlString.Create(html);
        }
    }
}
