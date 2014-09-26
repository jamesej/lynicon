<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.DateTime?>" %>
<%@ Import Namespace="Lynicon.Models" %>
<%@ Import Namespace="Lynicon.Utility" %>
<%: Html.TextBox("", string.Format("{0:yyyy-MM-dd}", Model.HasValue ? Model : DateTime.Today),
    ViewData.ModelMetadata.IsReadOnly
    ? (object) new { @class = "text-box single-line", @readonly = true }
    : (object) new { @class = "l24-datetime" }) %>

