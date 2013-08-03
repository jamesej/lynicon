<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Link>" %>
<% if (Model.IsInternal)
   {  %>
   <%= Html.ActionLink(Model.Content, Model.Action, Model.Controller) %>
<% }
   else
   { %>
<a href="<%= Model.Url == null ? "#" : Model.Url %>">
    <%= Model.Content %>
</a>
<% } %>


