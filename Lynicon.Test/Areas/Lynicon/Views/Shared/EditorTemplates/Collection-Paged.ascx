<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="L24CM.Models" %>
<%
    if (Model != null) {
        
        string oldPrefix = ViewData.TemplateInfo.HtmlFieldPrefix;
        PagingInfo paging = new PagingInfo(oldPrefix, 6);
        int index = paging.FirstItemIndex;

        ViewData.TemplateInfo.HtmlFieldPrefix = String.Empty;
        
        foreach (object item in (IEnumerable)Model) {
            string fieldName = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}[{1}]", oldPrefix, index++); %>
            <%=Html.EditorFor(m => item, null, fieldName)%>
        <%
        }
        
        ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;

         %>
        <div>
            <a href="<%= paging.GetPageLink(0) %>">|<</a>
            <a href="<%= paging.GetOffsetPageLink(-1) %>"><</a>
            <% foreach (KeyValuePair<int, string> pageLink in paging.GetPageNumberLinks())
               { %>
            <a href="<%= pageLink.Value %>"><%= pageLink.Key + 1%></a>
            <% } %>
            <a href="<%= paging.GetOffsetPageLink(1) %>">></a>
            <a href="<%= paging.GetPageLink(int.MaxValue) %>">>|</a>
        </div>
        <%
    }
%>
