<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ViewOtherSubsidy.aspx.vb" Inherits="ViewOtherSubsidy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">  
    <hr />
 <div class="clearboth"></div>
 <fieldset><%--
<div class="one_half">
<div class="portlet">
				<div class="portlet-header">Employee ID #:  <asp:Label runat="server" ID="transno" Text=""></asp:Label></div>
				<div class="portlet-content" style="display: block;">
		    <table class="fullwidth normal">
<tr>
<td>Name: <asp:Label CssClass="capitalize"  runat="server" ID="lblname" Text=""></asp:Label></td>
</tr>
<tr> 
<td>Available Subsidy: <asp:Label runat="server" ID="lbltotal" Text="" ></asp:Label> Php</td>
</tr>      
      </table>
 
	    </div>
        </div>
</div>
<div class="one_half last">
<div class="portlet">
				<div class="portlet-header">Search:</div>
				<div class="portlet-content" style="display: block;">
		    <table class="fullwidth normal">
           
            <tr style="display:none;">
            <td>Payroll Date: </td><td><asp:DropDownList ID="payrolldate" runat="server" 
                    DataSourceID="SqlDataSource2" DataTextField="PayrollDate" 
                    DataTextFormatString="{0:d}" DataValueField="PayrollDate"></asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>" 
                    SelectCommand="SELECT [PayrollDate] FROM [vwPayrollCalendar]">
                </asp:SqlDataSource>
                </td>
                
            </tr>
            <tr>
            <td>From : </td><td><asp:TextBox ID="txt_s_transdate" runat="server" CssClass="datepicker" CausesValidation="True"></asp:TextBox></td>
            </tr>
            <tr>
            <td>To:</td><td><asp:TextBox ID="txt_e_transdate" runat="server" CssClass="datepicker"></asp:TextBox></td>    
        </tr>
            <tr>
            <td colspan="2">
              Total Subsidy Search: <asp:Literal ID="stotal" runat="server" Text="0.00"></asp:Literal>&nbsp; Php 
		    </td>
            </tr>
            <tr>
            <td colspan="2"><asp:Label ID="lblcovered" runat="server"></asp:Label></td>
            </tr>
            </table>
   <div class="one_third" >
    <p><asp:Button ID="btnSearch" runat="server" Text="Search" /></p>
   </div>
   <div class="one_third last">
   <p><asp:Button ID="btnclear" runat="server" Text="Clear" Visible="False" /></p>
  </div>
	    </div>
        </div>
</div>--%>
<div class="clearboth"></div>
<hr />
 <div class="clearboth"></div> 
     <asp:GridView ID="GridView1" runat="server" PageSize="30" AutoGenerateColumns="False" CssClass="fullwidth fancy" DataSourceID="SqlDataSource1" AllowPaging="True" EmptyDataText="No Records Found"  >
         <AlternatingRowStyle BackColor="#CCFFCC" />
        <Columns>
            <asp:BoundField DataField="evdate" HeaderText="Date" dataformatstring="{0:MMM dd yyyy}" HtmlEncode="False" />
            <asp:BoundField DataField="title" HeaderText="Title" HtmlEncode ="false" />
            <asp:BoundField DataField="Subsidy" HeaderText="Subsidy Amount" />
            <asp:BoundField DataField="payrolldate" HeaderText="Payroll Date"  dataformatstring="{0:MMM dd yyyy}" HtmlEncode="false" />
        </Columns>
         <PagerStyle HorizontalAlign="Right" />
    
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>"
        SelectCommand=" SELECT CAST(eventdate AS varchar(11)) AS evdate , title, subsidy , payrolldate FROM tbl_specialSub WHERE empNo = @empID ORDER BY evdate DESC">
        <SelectParameters>
            <asp:SessionParameter Name="empid" SessionField="IDNO" />
             <%--
             <asp:SessionParameter Name="sdate" SessionField="sdate" />
              <asp:SessionParameter Name="edate" SessionField="edate" />
              --%>
            <%--<asp:ControlParameter ControlID="txt_s_transdate" DefaultValue="" Name="sdate"
            PropertyName="Text" ConvertEmptyStringToNull="False" />
            <asp:ControlParameter ControlID="txt_e_transdate" Name="edate" PropertyName="Text" ConvertEmptyStringToNull="False" />--%>
        </SelectParameters>
    </asp:SqlDataSource>
    <div class="clearboth"></div>

</fieldset>
</asp:Content>

