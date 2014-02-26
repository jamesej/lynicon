<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Relations.Reference>" %>
<div class="lyn-reference">
    <span class="lyn-reference-title"><%= Model != null && Model.Summary != null ? Model.Summary.Title : "" %></span>
    <button class='l24-find-reference'>Find Item</button>
    <%= Html.HiddenFor(m => m.Id, new { @class = "lyn-reference-id" })%>
    <%= Html.HiddenFor(m => m.DataType, new { @class = "lyn-reference-datatype" }) %>
    <%
        var type = Model.GetType();
        string refType = type.IsGenericType ? type.GetGenericArguments()[0].FullName : null;
         %>
    <%= Html.HiddenFor(m => refType, new { @class = "lyn-reference-typearg" }) %>
</div>
<%= Html.Partial("EnsureItemSelector") %>

