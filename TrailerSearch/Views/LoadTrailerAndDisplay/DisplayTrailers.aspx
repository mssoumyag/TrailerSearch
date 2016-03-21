<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<TrailerSearch.Models.IMDB>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>DisplayTrailers</title>
</head>
<body>    
    <input type="button" name ="home" value="Search For Another Movie" onclick=" window.location.href = 'GetTrailers' " />  
    <br /><br>
    <div>

       <%=Model.urlEmbededHTML %>
    </div>
</body>
</html>
