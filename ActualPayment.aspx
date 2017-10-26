<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ActualPayment.aspx.vb" Inherits="ActualPayment" title="Actual Payment Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <hr />
<asp:Panel ID="Panel1" runat="server" Height="20px" Width="100%" Visible="False">
    <div class="notification success clearboth"> 
    <span></span> 
    <div class="text"> 
    <p><strong>Success!</strong>Record Updated for <asp:Literal runat="server" ID="uptext" Text=""></asp:Literal> </p> 
    </div> 
    </div>
     </asp:Panel>
       <asp:Panel ID="Panel2" runat="server" Height="20px" Width="100%" Visible="false">
       <div class="notification error clearboth"> 
    <span></span> 
    <div class="text"> 
    <p><strong>Error!</strong>Please provide the amount.</p> 
    </div> 
    </div>
    </asp:Panel>
<fieldset>
<hr />
 <h3>Canteen Actual Payment:</h3>
<div class="clearboth"></div>
<hr />
<div class="fullwidth">
    <div class="one_half"><label>Employee ID: </label><asp:Label id="lblidname" Text="" runat="server"></asp:Label></div>
    <div class="one_half last"<label>Employee Name: </label><asp:Label ID="lblfullname" Text="" runat="server"></asp:Label></div>
</div>
<div class="clearboth"></div>
<hr />
<div class="one_third"> 
<table>
<tr>
<td colspan="2"><h5>For the Cutoff Period:</h5></td>
</tr>
<tr>
<td><label>From:</label></td>
<td><asp:TextBox runat="server" ID="fromdate" Text=""  CssClass="datepicker"></asp:TextBox></td>
</tr>
<tr><td colspan="2">&nbsp;</td></tr>
<tr>
<td><label>To:</label></td>
<td><asp:TextBox runat="server" ID="todate" Text="" CssClass="datepicker"></asp:TextBox></td>
</tr>
<tr><td colspan="2">&nbsp;</td></tr>
<tr>
<td>
<p><label>Amount Paid:</label></p>
</td>
<td><asp:TextBox ID="payment" runat="server" text="" ></asp:TextBox></td>
</tr>
<tr><td colspan="2">&nbsp;</td></tr>
<tr>
<td>&nbsp;</td>
<td>
<asp:Button ID="btnsubmit" runat="server" Text="Submit" />
</td>
 </tr>
</table>
</div>
<div class="clearboth"></div>
<hr />
    <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
        AllowPaging="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" 
        EnableModelValidation="True" CssClass="fullwidth normal" 
        EmptyDataText="No Records Found">
        <AlternatingRowStyle BackColor="Silver" />
        <Columns>
             <asp:BoundField DataField="recdate" HeaderText="Rec Date" 
                SortExpression="recdate" />
            <asp:BoundField DataField="incharge" HeaderText="Incharge" 
                SortExpression="incharge" />
            <asp:BoundField DataField="charges" HeaderText="Charges" 
                SortExpression="charges" />
            
            <asp:BoundField DataField="cutoffdate" HeaderText="CutOff Date" 
                SortExpression="cutoffdate" />
            <asp:BoundField DataField="subsidy" HeaderText="Subsidy" 
                SortExpression="subsidy" />
            <asp:BoundField DataField="ot_subsidy" HeaderText="OT Subsidy" 
                SortExpression="ot_subsidy" />
            <asp:BoundField DataField="subtotal" HeaderText="Subtotal" 
                SortExpression="subtotal" />
            <asp:BoundField DataField="payments" HeaderText="Payments" 
                SortExpression="payments" />
            <asp:BoundField DataField="balance" HeaderText="Balance" 
                SortExpression="balance" />
            <asp:BoundField DataField="remarks" HeaderText="Remarks" 
                SortExpression="remarks" />
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>" 
        SelectCommand="SELECT transcode, recdate, rectime, empId, incharge, charges, cutoffdate, subsidy, ot_subsidy, subtotal, payments, balance, remarks FROM tbl_logs WHERE (empId = @empno) ORDER BY recno Desc">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblidname" Name="empno" PropertyName="Text" />
        </SelectParameters>
    </asp:SqlDataSource>
</fieldset>
</asp:Content>
