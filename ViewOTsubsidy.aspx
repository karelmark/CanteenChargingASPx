<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ViewOTsubsidy.aspx.vb" Inherits="ViewOTsubsidy" title="Overtime Subsidy Details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<hr />
<div class="clearboth"></div>
<fieldset>
<div class="one_half">
<p>Employee ID#: <asp:Label runat="server" ID="transno" Text=""></asp:Label></p>
<p>Name: <asp:Label CssClass="capitalize"  runat="server" ID="lblname" Text=""></asp:Label></p>
 <p>Total Available OT Subsidy: <asp:Label runat="server" ID="lbltotal" Text ="" ></asp:Label> Php</p>
</div>
<div class="one_half last">
 
<div class="portlet">
			<div class="portlet-header">Search:</div>
			<div class="portlet-content" style="display: block;">
		    <table class="fullwidth normal" style='padding-top:20px;background-color:#F4F4F4;'>
            <tr>
            <td>Payroll Date: </td><td><asp:DropDownList ID="payrolldate" runat="server"  ></asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>" 
                    SelectCommand="SELECT DISTINCT payrolldate FROM  tbl_logs WHERE void = 0 ORDER BY payrolldate DESC">
                </asp:SqlDataSource>
                <p style='float:right;margin:0;padding-right:20px;'><asp:Button ID="btnSearch" runat="server" Text="Search" /></p>
                </td>
            </tr>
            <tr>
                <td><p>&nbsp;</p></td>
                <td><p  style='float:right;margin:0;padding-right:20px;'><asp:Button ID="clearbutt" runat="server" Text="Clear" /></p></td>
            </tr>
            </table>
   <div class="one_third" >
   
   </div>
   <div class="one_third last">
   <p><asp:Button ID="btnclear" runat="server" Text="Clear" Visible="False" /></p>
  </div>
	    </div>
        </div>
</div>
<div class="clearboth"></div>
<hr />
 <div class="clearboth"></div>

    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" AllowPaging="True" CssClass="fullwidth fancy" DataSourceID="SqlDataSource1" EmptyDataText="No Records Found!" PageSize="30">
       <AlternatingRowStyle BackColor="#CCFFCC" />
        <Columns>
            <asp:BoundField DataField="OTFDate" HeaderText="Date" dataformatstring="{0:MMM dd yyyy, dddd}" HtmlEncode="false"  /> 
            <asp:BoundField DataField="OTFHrs" HeaderText="No. Hrs"  dataformatstring="{0:F2}" HtmlEncode="false" />
            <asp:BoundField DataField="otfsub" HeaderText="Subsidy Amount" /> 
          
           <%-- <asp:BoundField DataField="otfstatus" HeaderText="Status" />   --%>
          
            <asp:BoundField DataField="pdate" HeaderText="Payroll Date" dataformatstring="{0:MMM dd yyyy}" HtmlEncode="false" /> 
          </Columns>
 
    </asp:GridView>
     <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>" SelectCommand ="SELECT   TOP 100  OTFDate, SUM(OTFHrs) AS OTFhrs, CASE WHEN SUM(OTFHrs) < 2 THEN 0.0 WHEN SUM(OTFHRS) < 10 THEN 40.0 WHEN SUM(OTFHRS) < 18 THEN 80.0 ELSE 120 END AS OTFSUB,  prolldate as pdate FROM  vwOTSub
WHERE     (EmpNo = @empID) AND  OTFDATE >= '6/1/2014'  GROUP BY OTFDate, prolldate, OTCanteenSub ORDER BY OTFDATE DESC "
     >

         <SelectParameters>
             <asp:SessionParameter DefaultValue="0" Name="empID"   SessionField="IDNO" />
             <asp:ControlParameter ControlID="payrolldate" DefaultValue=" " Name="pdate" PropertyName="Text" />
         </SelectParameters>
          
        
     </asp:SqlDataSource>
     
    <div class="clearboth"></div>
  
  </fieldset>
</asp:Content>

