<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Lynicon.Utility" %>
<div id="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>" class="collection closed">
<%
    if (Model != null) {
        string oldPrefix = ViewData.TemplateInfo.HtmlFieldPrefix;
        int index = 0;

        ViewData.TemplateInfo.HtmlFieldPrefix = String.Empty;
        
        int count = (Model as IEnumerable).Cast<object>().Count();

        if (count == 0)
        {
            // even for zero count, ensure the markup is built once so that any necessary includes are
            // put on the page
            object dummyItem = ReflectionX.NewOrDefault(Model.GetType().GetGenericArguments()[0]);
            Html.EditorFor(m => dummyItem, null, "dummy");
        }
        else
        {
            foreach (object item in (IEnumerable)Model) {
                string fieldName = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}[{1}]", oldPrefix, index);
                %>
                <div id="del-<%= fieldName %>" class="action-button delete indent-<%= ViewData.TemplateInfo.TemplateDepth - 1 %>" style="float: left">x</div>
                <div class="reorder indent-<%= ViewData.TemplateInfo.TemplateDepth - 1 %><%= index == 0 ? " first" : (index == count - 1 ? " last" : "") %>" style="float: left">
                    <div class="reorder-up action-button">
                        <img alt="up" id="up-<%= fieldName %>" src="/Areas/Lynicon/Content/up-arrow-white-tiny.png" />
                    </div>
                    <div class="reorder-down action-button">
                        <img alt="down" id="down-<%= fieldName %>" src="/Areas/Lynicon/Content/down-arrow-white-tiny.png" />
                    </div>
                </div>
                <div class="editor-field indent-<%= ViewData.TemplateInfo.TemplateDepth %>">
                <%
                    MvcHtmlString editor = Html.EditorFor(m => item, null, fieldName);
                    Response.Write(editor);
                %>
                </div>
                <%
                index++;
            }
        }
        
        ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;
    }
%>
    <div id="add-<%= ViewData.TemplateInfo.HtmlFieldPrefix %>" class="add-button indent-<%= ViewData.TemplateInfo.TemplateDepth - 1 %> depth-<%= ViewData.TemplateInfo.TemplateDepth %>">+</div>
</div>
