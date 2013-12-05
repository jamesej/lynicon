<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Lynicon.Extensibility" %>

<% bool isAlt = false; foreach (string view in LyniconUi.Instance.RevealPanelViews)
   {%>
    <div class="reveal-panel-view <%= isAlt ? "alt" : "" %>">
       <%= Html.Partial(view) %>
       <div style="clear:both"></div>
    </div>

   <%
   isAlt = !isAlt;}
%>


