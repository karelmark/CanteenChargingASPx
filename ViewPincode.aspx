    <%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ViewPincode.aspx.vb" Inherits="ViewPincode"  %>
     <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
         <div class="clearboth"></div>  
    <div class="fullwidth" style="min-height: 600px; ">
    <hr />
    <div class="clearboth"></div>
    <div class="tabs" style="width:670px;">
        <asp:Panel ID="Panel1" runat="server" Visible="False">
        <div class="notification success"> 
						<span></span> 
						<div class="text"> 
							<p><strong>Success!</strong> Your Pincode has changed.</p> 
						</div> 
					</div> 
        </asp:Panel>
    	

  <div class="clearboth"></div>
    <div class="ui-widget-header">
          	 <span>Pincode</span> 
            <ul>
            <li><a href="#tabs-1">View</a></li>
            <li><a href="#tabs-2">Change</a></li>
             </ul>
    </div>
	<div id="tabs-1"> 
    <div id="viewpin">
    <div class="fullwidth">
    <span>Incase you forgot ,lost or misplaced your Employee ID Card, You need to provide this pincode to allow the Canteen Staff to search your name manually in the Canteen Charging System.</span>
    <hr />
     <p align="center">
     <label>Your Pincode</label><asp:TextBox ID="mypincode" runat="server" 
             Font-Bold="true" Font-Size="Large" ForeColor="Black" 
             AutoPostBack="True" ReadOnly="True"></asp:TextBox>
    </p>
     <hr />
      
    </div>
    </div>
    <div class="clearboth"></div>

    </div> <!-- end of first tab -->
     <div id="tabs-2">
    <div class="fullwidth"> 
    <p align="center"> 
    <asp:RequiredFieldValidator  ControlToValidate="changepin"  ID="RequiredFieldValidator1" runat="server" ErrorMessage="Enter your New Pin" Display="Dynamic"></asp:RequiredFieldValidator>
    <br />
   <asp:RegularExpressionValidator ValidationExpression="^(\d{6})" ControlToValidate="changepin" ID="RegularExpressionValidator1" runat="server" ErrorMessage="Your Pincode Must be 6 numeric digits!"></asp:RegularExpressionValidator> 
    </p>
    <p align="center"> 
   <asp:TextBox  ID="changepin" runat="server" ValidationGroup="changepin" Font-Bold="true" Font-Size="Large" ForeColor="Black" ></asp:TextBox>
    </p>
   <p align="center">
    <asp:Button ID="btnpincode" CssClass="btnpincode" runat="server" Text="Reset Pincode" />
    </p>
     <hr />
    
    </div>
     </div> <!-- end of second tab -->


    </div>
        <%------------------%>
 
 </div>
    
     
    </asp:Content>

