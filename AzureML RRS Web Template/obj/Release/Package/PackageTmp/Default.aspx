<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AzureMLRRSWebTemplate.Default" %>


<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!-- The above 3 meta tags *must* come first in the head; any other head content must come *after* these tags -->
    <meta name="description" content="Azure Machine Learning Web Service">
    <meta name="author" content="">

    <title></title>

    <!-- Bootstrap core CSS -->
    <link href="CSS/bootstrap.css" rel="stylesheet">
    <link href="CSS/jumbotron-narrow.css" rel="stylesheet">
    <link href="CSS/master.css" rel="stylesheet">

    <!-- Java script / jQuery -->
    <script src="Scripts/jquery-2.1.4.min.js"></script> 
    <script src="Scripts/bootstrap.min.js"></script>   
    <script src="http://d3js.org/d3.v3.min.js"></script>
    

    <!-- -->
    
    <link href="CSS/jquery-ui-1.10.0.custom.min.css" rel="stylesheet">
    <link href="CSS/sliderIE.css" rel="stylesheet">
    <link href="CSS/sliderHTML.css" rel="stylesheet">
    <script src="Scripts/sliderIE.js"></script>
    <script src="Scripts/sliderHTML.js"></script>
    <script src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/master.js"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            updateSliderIE();
            updatesliderHTML();
            updateResize();
        });        
    </script>


</head>
<body onresize="updateResize()">
    <div class="container azue-header">  
        <div class="jumbotron azue-header" style="background-image:url('Resources/azure-ml.png');height:126px; position:relative;padding-top: 10px;">
            <a href="/"><div class="wheel"><wheel></wheel></div></a>
            <div style="max-height:110px;overflow-y:hidden"><h3 class="text-muted" style="padding-left: 110px;"><asp:Label ID="lblTitle" runat="server" Text="Azure Machine Learning"></asp:Label></h3>
        </div>
    </div>
    <div class="container" id="fullpage">
        <form id="form1" runat="server" style="margin: 0 auto; position:relative" CssClass="form-inline">            
            <div id="divMain">            
                <div id="divControls">
                  <div class="row">
                    <div class="col-lg-6" id="col-1">
                        <p>
                            <asp:PlaceHolder ID="InputPlaceHolder1" runat="server"></asp:PlaceHolder>
                        </p>
                    </div>
                    <div class="col-lg-6" id="col-2">
                        <p>
                            <asp:PlaceHolder ID="InputPlaceHolder2" runat="server"></asp:PlaceHolder>
                        </p>
                    </div>
                  </div>
                </div>
            </div>

            <div id="submitDiv">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary btnSubmit" OnClick="btnSubmit_Click"/>
            </div>

            <div id="divResult" runat="server" >
                <asp:PlaceHolder ID="OutputPlaceHolder" runat="server"></asp:PlaceHolder>     
                <div id="topLoader"> </div>       
            </div>
                                    
            <asp:PlaceHolder ID="bottomsciprtPlaceHolde" runat="server"></asp:PlaceHolder>

            <div id="footer">
                <div class="footerText">
                    <p >Powered by <strong>Azure Machine Learning</strong></p>
                </div>
                 
            </div>

        </form>
    </div> <!-- /container -->
    
    
    <!-- IE10 viewport hack for Surface/desktop Windows 8 bug -->
    <script src="Scripts/js/ie10-viewport-bug-workaround.js"></script>
    <%--</form>--%>
</body>
</html>

<script type="text/javascript" src="Scripts/wheels.js"></script>
