using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using Lynicon.Extensions;
using Lynicon.Extensibility;
using HtmlAgilityPack;

namespace Lynicon.Attributes
{
    /// <summary>
    /// Marks that the output of this action method, controller or every controller should be parsed,
    /// processed and reoutput.  This processing can be cancelled by in the RouteData setting a
    /// DataToken called 'CancelProcessingHtml' to true. Processing is done by event processors registered
    /// to the global event 'PostProcess.Html'.
    /// </summary>
    public class ProcessHtmlAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.RouteData.DataTokens.ContainsKey("CancelProcessingHtml") &&
                (bool)filterContext.RouteData.DataTokens["CancelProcessingHtml"])
                return;

            if (filterContext.Result is ViewResult
                || (filterContext.Result is PartialViewResult && !filterContext.Controller.ControllerContext.IsChildAction))
            {
                if (filterContext.HttpContext.Response.Filter == null)
                    return;

                if (!(filterContext.HttpContext.Response.Filter is ProcessHtmlFilter))
                {
                    var filt = new ProcessHtmlFilter(filterContext.HttpContext.Response.Filter, filterContext.Controller as Controller);
                    filterContext.HttpContext.Response.Filter = filt;
                }
            }
        }
    }
}
