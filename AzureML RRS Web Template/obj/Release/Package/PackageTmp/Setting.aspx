<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setting.aspx.cs" Inherits="AzureMLRRSWebTemplate.Setting" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title> Setting page</title>
    <link href="CSS/bootstrap.css" rel="stylesheet"/>
    <link href="CSS/jumbotron-narrow.css" rel="stylesheet"/>
    <link href="CSS/master.css" rel="stylesheet"/>
    <script src="Scripts/jquery-2.1.4.min.js"></script>    
    <script src="http://d3js.org/d3.v3.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>

    <link href="http://www.bootstrap-switch.org/dist/css/bootstrap3/bootstrap-switch.css" rel="stylesheet"/>
    <script src="http://www.bootstrap-switch.org/dist/js/bootstrap-switch.js"></script>

    <script type="text/javascript">
        
        function hideServiceInfo() {
            $("#serviceInfo").toggle("slow");
        };

        $(document).ready(function(){
            $("#icon_expand_info").click(function () {
                hideServiceInfo();
            });

            $(".amount").focus(function () {
                $("#serviceInfo").toggle(false);
            });

            // Use for IE
            $(".btnHome").click(function () {
                window.location.replace("Default.aspx");
            });

            $('#icon_help_url').tooltip({ title: "1. Go to Web Service Help Page</br>2. Copy Request URL</br><img src=\"Resources\\api_url.png\">", html: true, placement: "bottom" });
            $('#icon_help_key').tooltip({ title: "1. Go to Web Service Dashboard</br>2. Copy API Key<br><img src=\"Resources\\api_key.png\">", html: true, placement: "bottom" });
            $('.wheel').tooltip({ title: "Click me to Go Home", html: true, placement: "right" });

        });
    </script>

   
</head>
<body class="settingwidth">

<!-- Header -->
<div class="container azue-header" >  
    <div class="jumbotron" style="background-image:url('Resources/azure-ml.png');height:126px; position:relative;padding-top: 10px;">
        <a href="/"><div class="wheel"><wheel></wheel></div></a>
        <div style="padding-left: 110px;">
            <div style="max-height:110px;overflow-y:hidden">                
                <h3 class="text-muted"><asp:Label ID="lblDes" runat="server" Text="Web App Configuration"></asp:Label></h3>
                <h4 class="text-muted"><asp:Label ID="lblTitle" runat="server" Text=""></asp:Label></h4>
            </div>
        </div>
    </div>
</div>

<div class="container" >
    <form id="form1" runat="server">
        <fieldset>
            <legend>Web Service Info<span class="glyphicon glyphicon-triangle-bottom" id="icon_expand_info" style="float:right"></span></legend>
        <div style="text-align: left;" id="serviceInfo">    
            <div class="form-group has-success has-feedback">
                <div class="col-sm-9" style="width:100%; float:none">
                    <div class="fieldname">API Post URL: <span class="glyphicon glyphicon-question-sign" id="icon_help_url"></span></div>
                    <div >              
                        <asp:TextBox ID="txtURL" runat="server"  CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                    </div>   
                </div>
            </div>
            <div class="form-group has-success has-feedback" >
                <div class="col-sm-9" style="width:100%; float:none">
                    <div  class="fieldname">API Key: <span class="glyphicon glyphicon-question-sign" id="icon_help_key"></span></div>
                    <div >                   
                    <asp:TextBox ID="txtAPI" runat="server"  CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div style="float:right;margin-right: 15px;margin-bottom: 15px;">
                    <asp:Label ID="lblResult" runat="server" Text="Press Submit to load input parameters" CssClass="myIntruduce"></asp:Label>
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary btn-lg" OnClick="btnSubmit_Click"/>
            </div>
                
        </div>
            </fieldset>

        <div style="margin-top:15px" id="parameter">
            
            <asp:PlaceHolder ID="parameterRegion" runat="server"></asp:PlaceHolder>
            <div style="background-color: transparent; margin-bottom:15px" >
                <asp:Button ID="btnSave" runat="server" Text="Save change" OnClick="btnSave_Click" CssClass="btn btn btn-primary btnSubmit" />
            </div>
        </div>

        <footer >
            <div class="footerText">                
                <p >Powered by <strong>Azure Machine Learning</strong></p>   
            </div>             
        </footer>
        
<!-- Modal Enter Key -->
  <div class="modal fade" id="EnterKeyModal" role="dialog">
    <div class="modal-dialog">
    
      <div class="modal-content">        
        <div class="modal-body jumbotron" style="padding: 20px;">
          <p>Please enter the API Key to access the Setting page!</p>
            <asp:TextBox ID="txtKeyInput" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
        </div>
        <div class="modal-footer">
            <asp:Button ID="btnSubmitKey" runat="server" Text="OK" OnClick="btnSubmitKey_Click" CssClass="btn btn-primary btn-lg"/>
            <button type="button" class="btn btn-primary btn-lg btnHome">Cancel</button>
        </div>
      </div>
      
    </div>
  </div>

<!-- Modal Save change success -->
  <div class="modal fade" id="saveChangeModalSuccess" role="dialog">
    <div class="modal-dialog">    
      <div class="modal-content">    
        <div class="modal-body alert alert-success fade in" style="margin-bottom: 0px;">
            <a href="#" class="close" data-dismiss="modal" aria-label="Close">&times;</a>
          <p><strong>Success!</strong> Your changes have been saved successfully.</p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-primary btn-lg btnHome" >Go to Home Page</button>                
            <button type="button" class="btn btn-primary btn-lg" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">Continue</span></button>
        </div>
      </div>
      
    </div>
  </div>
  
<!-- Modal Save change Error-->
  <div class="modal fade" id="saveChangeModalError" role="dialog">
    <div class="modal-dialog">    
      <div class="modal-content">    
        <div class="modal-body alert alert-danger fade in" style="margin-bottom: 0px;">
            <a href="#" class="close" data-dismiss="modal" aria-label="Close">&times;</a>
          <p><strong>Error!</strong> A problem has been occurred while saving your data.</p>
        </div>        
      </div>
      
    </div>
  </div>

<!-- Modal Submit Confirm-->
  <div class="modal fade" id="submitModal" role="dialog">
    <div class="modal-dialog">    
      <div class="modal-content">    
        <div class="modal-body alert alert-warning fade in" style="margin-bottom: 0px;">            
          <h3><strong>Warning!</strong> This will reset all parameters. OK to continue?</h3>
        </div>            
        <div class="modal-footer">            
            <asp:Button ID="btnResetParameter" runat="server" Text="OK" CssClass="btn btn-primary btn-lg" OnClick="btnResetParameter_Click"/>
            <button type="button" class="btn btn-primary btn-lg" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">Cancel</span></button> 
        </div>        
      </div>      
    </div>
  </div>


    </form>

    </div>

</body>
</html>

<script type="text/javascript" src="Scripts/wheels.js"></script>
