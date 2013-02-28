<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<L24CM.Models.ImageLink>" %>
<%@ Import Namespace="L24CM.Utility" %>
<% if (Model != null) { %>
<a href="<%= Model.Url == null ? "#" : Model.Url %>" <%= ViewData.GetHtmlAttributes("id, class, style") %>>
    <img src="<%= Model.Image.Url %>" alt="<%= Model.Image.Alt %>"  />
</a>
<% } %>


