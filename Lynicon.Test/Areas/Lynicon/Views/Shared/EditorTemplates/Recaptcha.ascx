<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Lynicon.Models.Recaptcha>" %>
<% if (Model != null) { 
       if (ViewData["theme"] != null)
       { %>
        <script type="text/javascript">
            var RecaptchaOptions = { theme: '<%= ViewData["theme"] %>' };
        </script>
    <% } %>
    <script type="text/javascript"
       src="http://www.google.com/recaptcha/api/challenge?k=<%= Model.PublicKey %>">
    </script>

    <noscript>
        
       <iframe src="http://www.google.com/recaptcha/api/noscript?k=<%= Model.PublicKey %>"
           height="300" width="500" frameborder="0"></iframe><br/>
       <textarea name="recaptcha_challenge_field" rows="3" cols="40">
       </textarea>
       <input type="hidden" name="recaptcha_response_field"
           value="manual_challenge"/>
    </noscript>
    <%= Html.Hidden(ViewData.TemplateInfo.HtmlFieldPrefix, "dummy") %>
<% } %>

