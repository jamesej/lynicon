<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.ImageLink>" %>
<%@ Import Namespace="Lynicon.Utility" %>
<% if (Model != null) { %>
<a href="<%= Model.Url == null ? "#" : Model.Url %>" <%= ViewData.GetHtmlAttributes("id, class, style") %>>
    <img src="<%= Model.Image.Url %>" alt="<%= Model.Image.Alt %>"  />
</a>
<% } %>


