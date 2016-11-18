using System.Web;
using System.Web.Mvc;
using Lynicon.Attributes;

namespace Lynicon.Site
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Lynicon install inserted these 2 lines
            filters.Add(new ProcessIncludesAttribute());
            filters.Add(new ProcessHtmlAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
