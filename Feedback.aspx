<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Feedback.aspx.vb" Inherits="Feedback" title="Feedback" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="clearboth"></div>
 <div class="clearboth"></div>
  
    <asp:Panel ID="Panel1" runat="server" Height="20px" Width="90%" Visible="False">
    <div class="notification success"> 
    <span></span> 
    <div class="text"> 
    <p><strong>Success!</strong>Feedback Submitted</p> 
    </div> 
    </div>
     </asp:Panel>
     <asp:Panel ID="Panel2" runat="server" Height="20px" Width="90%" Visible="false">
       <div class="notification error"> 
    <span></span> 
    <div class="text"> 
    <p><strong>Error!</strong>Empty Feedback.</p> 
    </div> 
    </div>
     
     </asp:Panel>
     
  <div class="clearboth"></div>   
  <hr />
<h2 style="clear:both;">Feedback Form</h2> 
<p>We would like to hear your suggestions, feedback and ideas in order to help us serve you better. Let us know about your experience with the PKI Canteen.</p>
<asp:TextBox ID="feedentry" runat="server" CssClass="fullwidth" 
        TextMode="MultiLine" BackColor="#009933" BorderColor="#003300" 
        BorderStyle="Double" BorderWidth="2px" Columns="110" ForeColor="White" 
        Rows="10"></asp:TextBox>
<br />

<div class="clearboth"></div>
<div class="one_third"  > 
    <asp:Button ID="btnsubmit" runat="server" Text="Submit" />
</div>
 

<hr />
<div class="clearboth"></div>
For MIS Support: Email the <a href="mailto:mcdagus@kao-phil.com">CCS Administrator</a>  or Call local &num; 253
<hr />
<div class="clearboth"></div>

</asp:Content>

