<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ViewDetail.aspx.vb" Inherits="ViewDetail" title="View Details Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">   
<div class="clearboth"></div>
<fieldset>
<hr />
<div class="one_half">
<p>Transaction #: <asp:Label runat="server" ID="transno" Text=""></asp:Label></p>
<p>Name: <asp:Label CssClass="capitalize"  runat="server" ID="lblname" Text=""></asp:Label></p>
 
</div>
<div class="one_half last">
<p>Date: <asp:Label runat="server" ID="datecovered" Text="" ></asp:Label></p>
<p>Time: <asp:Label runat="server" ID="timecovered" Text="" ></asp:Label></p>
</div>
<div class="clearboth"></div>
<hr />
 <div class="clearboth"></div>
 <div style="min-height:200px;">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="fullwidth fancy">
       <AlternatingRowStyle BackColor="#CCFFCC" />
        <Columns>
            <asp:BoundField DataField="itemno" HeaderText="Item #" />
            <asp:BoundField DataField="itemname" HeaderText="Item Name" />
            <asp:BoundField DataField="qty" HeaderText="Qty" ApplyFormatInEditMode="True"  DataFormatString="{0:F0}"  HtmlEncode="False" />
            <asp:BoundField DataField="unitcode" HeaderText="Unit Code" />
            <asp:BoundField DataField="price" HeaderText="Price" SortExpression="price"  DataFormatString="{0:F2}"  HtmlEncode="False" />
            <asp:BoundField DataField="subtotal" HeaderText="Subtotal"  DataFormatString="{0:F2}"  HtmlEncode="False" />
        </Columns>
    </asp:GridView>
    </div>    
 	 <div class="clearboth"></div>
    
     <p style="text-align:right;">
         <asp:Label ID="lbltest" runat="server" CssClass="floatright"></asp:Label>
     
     </p>
     <div class="clearboth"></div>
     <hr />
 
    
<div class="one_third" >
    <asp:Button ID="btnback" runat="server" OnClientClick="javascript:history.back(1); return false;" Text="Back" />
</div>  
</fieldset>


</asp:Content>

