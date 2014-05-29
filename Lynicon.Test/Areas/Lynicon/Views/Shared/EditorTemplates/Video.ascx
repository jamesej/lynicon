<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Video>" %>
<%@ Import Namespace="Lynicon.Models" %>
<%@ Import Namespace="Lynicon.Utility" %>
<%= Html.TextAreaFor(m => m.Embed, new { @class = "lyn-vid-embed" })%>
<%= Html.TextBoxFor(m => m.Url, new { @class = "lyn-vid-url" })%>

