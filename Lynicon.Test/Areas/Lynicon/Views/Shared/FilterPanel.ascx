<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>>" %>
<%@ Import Namespace="Lynicon.Utility" %>
<%
    Uri currentUri = ViewContext.HttpContext.Request.Url;
     %>
<% foreach (var filter in Model) { %>
<div class="lynicon-filter-textbox">
    <input type="text" />
    <a class="lynicon-ctl-button"><%= filter.Key %></a>
    <a class="<%= filter.Key %> filter-clear lynicon-ctl-button">Clear</a>
</div>
<% } %>
<%= Html.RegisterScript("lynicon-filter-textbox", @"javascript:
    var filterFields = {" + Model.Select(kvp => "'" + kvp.Key + "': ['" + kvp.Value.Join("','") + "']").Join(",") + @"};
    $(document).ready(function () {
        $('.lynicon-filter-textbox a').click(function (ev) {
            ev.preventDefault();
            if ($(this).hasClass('filter-clear')) {
                location.href = ensureKeyValue(location.href, '$filter', null);
                return false;
            }
            var val = $(this).closest('.lynicon-filter-textbox').find('input').val();
            if (val) {
                var fields = filterFields[$(this).text()];
                var filt = '';
                for (var i = 0; i < fields.length; i++) {
                    filt += (filt ? ' or ' : '') + ""'"" + val + ""' substringof "" + fields[i];
                }
                location.href = ensureKeyValue(location.href, '$filter', filt);
            }
            return false;
        });
    });", new List<string> { "jquery" }) %>

