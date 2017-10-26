<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Canteen Charging System P.K.I.</title>
     <link rel="shortcut icon" type="image/ico" href="assets/favicon.ico" />
    <link  type="text/css" rel="stylesheet" href="style.css" />
    <!-- <link type="text/css" href="css/jquery.cleditor.css" rel="stylesheet" /> -->
	<script type="text/javascript"> window.moveTo(0, 0); window.resizeTo(screen.width, screen.height) </script>  
    <script type='text/javascript' src='js/jquery-1.4.2.min.js'></script>	<!-- jquery library -->
	<script type='text/javascript' src='js/jquery-ui-1.8.5.custom.min.js'></script> <!-- jquery UI -->
	<script type='text/javascript' src='js/cufon-yui.js'></script> <!-- Cufon font replacement -->
	<script type='text/javascript' src='js/ColaborateLight_400.font.js'></script> <!-- the Colaborate Light font -->
	<script type='text/javascript' src='js/easyTooltip.js'></script> <!-- element tooltips -->
	<script type='text/javascript' src='js/jquery.tablesorter.min.js'></script> <!-- tablesorter -->
   <script type='text/javascript' src='js/visualize.jQuery.js'></script> <!-- visualize plugin for graphs / statistics -->
	<script type='text/javascript' src='js/iphone-style-checkboxes.js'></script> <!-- iphone like checkboxes -->
	<script type='text/javascript' src='js/jquery.cleditor.min.js'></script> <!-- wysiwyg editor -->
	<script type='text/javascript' src='js/custom.js'></script> <!-- the "make them work" script -->

</head>
<body > 

     <div id="loginbox" class="box" style="width:500px; margin:150px auto; text-align:center;">
     <form id="form1" runat="server">
		<h1 id="custom_font" style="margin:0px !important;color:#036F03;font-family:'MOLTOR3';text-indent: -99999px;">Canteen Charging System</h1> 
        <div id="loginform">
        <div class="tabs" id="overwrite" style="width:500px;background-color: transparent;border-top:1px solid #555; border-bottom:none;border-left:none; border-right: none;">
		<div class="ui-widget-header" >
			<span style="color:#8BEAB7;">Login</span>
			<ul>
				<li><a   href="#tabs-1">Employee</a></li>
				<li style="display:none;"><a href="#tabs-2">Canteen</a></li>
			 
			</ul>
		</div>

		<div id="tabs-1">
        <table class="fullwidth">
        <tr><td>
                <p style="text-align:center;"><asp:Label ID="errmsg" runat="server" ForeColor="Red"></asp:Label></p>
                <p style="text-align:center;">
                <asp:Label ID="Label1" runat="server" Text="Username:"></asp:Label>
                <asp:TextBox CssClass="sf"  ID="username" runat="server"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="usrnameValidator" ValidationGroup="Employee" runat="server" ErrorMessage="Required" ControlToValidate="username" Display="Dynamic" SetFocusOnError="False"></asp:RequiredFieldValidator>
                </p>
                <p style="text-align:center;">
                <asp:Label ID="label2" runat="server" Text= "Password:"></asp:Label>
                <asp:TextBox CssClass="sf" ID="password" runat="server" TextMode="Password">a</asp:TextBox><br />
                <asp:RequiredFieldValidator ID="passwordValidator" ValidationGroup="Employee" ControlToValidate="password" runat="server" ErrorMessage="Required" Display="Dynamic" SetFocusOnError="False"></asp:RequiredFieldValidator>
                </p>
                <div class="clearboth"></div>
           </td></tr>
           <tr>
           <td> <p style="width:200px; margin:0 auto;">
           <asp:Button ID="btnlogin" causesvalidation="true" validationgroup="Employee" runat="server" Text="Login" CssClass="adjust" />
           </p></td>
           </tr>
        </table>
		</div> <!-- end of first tab -->
		<div id="tabs-2">
		<table class="fullwidth">
        <tr>
        <td>
                <p style="text-align:center;"><asp:Label ID="Label3" runat="server" ForeColor="Red"></asp:Label></p>
                <p style="text-align:center;">
                <asp:Label ID="Label4" runat="server" Text="Username:"></asp:Label>
                <asp:TextBox CssClass="sf"  ID="canusername" runat="server"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="canusernamevalidator" ValidationGroup="Canteen" runat="server" ErrorMessage="Required" ControlToValidate="canusername" Display="Dynamic" SetFocusOnError="False"></asp:RequiredFieldValidator>
                </p>
                <p style="text-align:center;">
                <asp:Label ID="label5" runat="server" Text= "Password:"></asp:Label>
                <asp:TextBox CssClass="sf" ID="canpassword" runat="server" TextMode="Password">a</asp:TextBox><br />
                <asp:RequiredFieldValidator ID="canpasswordvalidator"  ValidationGroup="Canteen" ControlToValidate="canpassword" runat="server" ErrorMessage="Required" Display="Dynamic" SetFocusOnError="False"></asp:RequiredFieldValidator>
                </p>
                <div class="clearboth"></div>
           </td></tr>
           <tr><td>
            <p style="width:200px; margin:0 auto;"><asp:Button ID="btncantenlog" causesvalidation="true" validationgroup="Canteen"  runat="server" Text="Login" /></p>
            </td></tr>
        </table> 
        </div>
</div>
<div class="clearboth"></div>
        
        </div>
     <div class="clearboth"></div>
	<div id='logincopy'>
	<p style='color:#fff;font-size:10px;text-align:right;'>&copy; <%= Date.Today.Year.ToString %> PKI - MIS Department - <span style="color:#e11;;">seenkax error!</span></p>
	</div>
       </form>
    </div>
 
  
</body>
</html>
