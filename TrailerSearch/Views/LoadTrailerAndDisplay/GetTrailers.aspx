<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<TrailerSearch.Models.IMDB>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>GetTrailers</title>
    <script type ="text/javascript">
        function ValidateForm() {
            var x = document.forms["displayTrailers"]["movieName"].value;
            if (x == null || x == "") {
                alert("Please enter a movie name to search!");
                return false;
            }
        }
    </script>
</head>
<body>
    <form  action="DisplayTrailers" onsubmit="return ValidateForm()" method="post" name="displayTrailers">
        <div >
            Enter movie name:<input type="text" name="movieName" />
            <input type="submit" name="search" value="Search For Trailers" />
        </div>
    </form>
</body>
</html>
