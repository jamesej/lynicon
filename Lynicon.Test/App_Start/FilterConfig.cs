using System.Web;
using System.Web.Mvc;
using Lynicon.Attributes;

namespace Lynicon.Test
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ProcessIncludesAttribute());
            filters.Add(new ProcessHtmlAttribute());
        }
    }
}