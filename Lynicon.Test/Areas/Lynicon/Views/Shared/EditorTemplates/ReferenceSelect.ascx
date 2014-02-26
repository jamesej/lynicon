<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Relations.Reference>" %>
<div class="lyn-reference lyn-reference-select">
    <%= Html.DropDownListFor(m => m.Id, Model.GetSelectList(), null, new { @class = "lyn-reference-id chosen-select" })%>
    <%= Html.HiddenFor(m => m.DataType, new { @class = "lyn-reference-datatype" }) %>
</div>

