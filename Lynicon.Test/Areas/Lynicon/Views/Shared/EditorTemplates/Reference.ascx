<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Relations.Reference>" %>
<div class="lyn-reference">
    <span class="lyn-reference-title"><%= Model != null && Model.Summary != null ? Model.Summary.Title : "" %></span>
    <button class='l24-find-reference'>Find Item</button>
    <%= Html.HiddenFor(m => m.Id, new { @class = "lyn-reference-id" })%>
    <%= Html.HiddenFor(m => m.DataType, new { @class = "lyn-reference-datatype" }) %>
</div>
<%= Html.Partial("EnsureItemSelector") %>

