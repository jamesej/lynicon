<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<L24CM.FormClasses.LoginForm>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
</head>
<body>
    <div>
        <% using (Html.BeginForm())
           { %>
            <%= Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
            <%= Html.EditorForModel() %>
            <div><input type="submit" value="Save" /></div>
        <% } %>
    </div>
</body>
</html>
