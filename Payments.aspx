<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Payments.aspx.vb" Inherits="Payments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 <div class="clearboth"></div>  
  
 <div class="fullwidth">
<hr />
<div class="clearboth"></div>
<fieldset>
<div class="clearboth"></div>
   <asp:Panel ID="ErrorPanel" runat="server" Visible="False">
     <div class="notification error clearboth"> 
						<span></span> 
						<div class="text"> 
							<p><strong>Error!</strong><asp:Literal ID="errormsg" runat="server"></asp:Literal> </p> 
						</div> 
					</div> 

    </asp:Panel>

    <asp:Panel ID="SuccessPanel" runat ="server" Visible="False">
    <div class="notification success clearboth"> 
						<span></span> 
						<div class="text"> 
							<p><strong>Success!</strong> <asp:Literal ID="successmsg" runat="server"></asp:Literal></p> 
						</div> 
					</div> 
	</asp:Panel>
    <hr />
<h5>Canteen Payment/Adjustment</h5>
<hr />
<div class="one_half   column">
			 	<div class="portlet">
				<div class="portlet-header">Search:</div>
				<div class="portlet-content" style="display: block;">
     <asp:TextBox runat="server"  ID="searchtxt" Text="" AutoPostBack="true"/>
      <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
</div>
</div>
</div>
<div class="one_half column last ">
			 	<div class="portlet">
				<div class="portlet-header">Submit Payments:</div>
				<div class="portlet-content" style="display: block;">
  <div id="unpaid">
<table class="fullwidth normal">
<%--<tr>
<td><label>From:</label></td>
<td><asp:TextBox runat="server" ID="fromdate" Text=""  CssClass="datepicker"></asp:TextBox></td>
</tr>
<tr><td colspan="2">&nbsp;</td></tr>
<tr>
<td><label>To:</label></td>
<td><asp:TextBox runat="server" ID="todate" Text="" CssClass="datepicker"></asp:TextBox></td>
</tr>--%>
<tr><td colspan="2">Warning! Please Double Check before Updating</td></tr>
<tr><td></td> <td  ><asp:Button ID="btnUpdatePayments" runat="server" Text="Update" /></td></tr>
</table>
</div>
   <asp:Label ID="lblmsg" runat="server" Text="Label"></asp:Label>
</div>
</div>
</div>

<div class="clearboth"></div>
<hr />
<div class="clearboth"></div>

<asp:GridView ID="searhpayables" runat="server" AllowSorting="True"  ShowFooter="True"
        AutoGenerateColumns="False" DataSourceID="SqlDataSource1" 
        EnableModelValidation="True" DataKeyNames="EmpNo" 
        CssClass="fullwidth fancy" EmptyDataText="No Records Found">

    <AlternatingRowStyle BackColor="#CCFFCC" />

<Columns>
        
    <asp:BoundField DataField="EmpNo" HeaderText="ID" SortExpression="EmpNo" />
    <asp:BoundField DataField="EmpFName" HeaderText="First Name" 
        SortExpression="EmpFName" />
    <asp:BoundField DataField="EmpMName" HeaderText="Middle Name" 
        SortExpression="EmpMName" />
     <asp:BoundField DataField="EmpLName" HeaderText="Last Name" 
        SortExpression="EmpLName" />

    <asp:TemplateField>
            <ItemTemplate>
                <asp:TextBox ID="txtpayment" runat="server" Text='0' AutoPostBack="false" onkeydown = "return (event.keyCode!=13);" />
            </ItemTemplate>
    </asp:TemplateField> 
     

</Columns>

     <HeaderStyle BackColor="#66FF66" />

</asp:GridView>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>" 
        SelectCommand="SELECT EmpNo, EmpLName, EmpFName, EmpMName FROM vwEmployeeMaster WHERE 
 (EmpNo LIKE '%' + lTrim(rTrim(@txtbox)) + '%') OR (EmpLName LIKE '%' + lTrim(rTrim(@txtbox)) + '%') OR (EmpFName LIKE '%' + lTrim(rTrim(@txtbox)) + '%')  ORDER BY EmpLName">
        <SelectParameters>
            <asp:ControlParameter ControlID="searchtxt" Name="txtbox" PropertyName="Text" />
        </SelectParameters>
    </asp:SqlDataSource>

       <a href="#" style="float:right; margin-bottom: 25px;" >Top</a>


    <asp:Button ID="btnprntreport" runat="server" Text="Button" />
    </fieldset>
</div>

</asp:Content>

