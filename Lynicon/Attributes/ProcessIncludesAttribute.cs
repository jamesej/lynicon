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
    /// On the action method, controller or controllers for which this is registered, set up Includes filtering.
    /// Requires that ProcessHtmlAttribute has been applied to the action method, controller or controllers as it
    /// relies on it to operate.  Includes filtering is accessed via @Html.RegisterScript, @Html.RegisterCss or
    /// @Html.RegisterHtml and it ensures script, css or html items are included once and once only in the output
    /// and deals with any dependencies of these items by positioning correctly.
    /// </summary>
    public class ProcessIncludesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // request needs an includes manager
            IncludesManager.Instance = new IncludesManager();
        }
    }
}
