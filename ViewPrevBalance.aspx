<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="ViewPrevBalance.aspx.vb" Inherits="ViewPrevBalance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<hr />
<fieldset>
    <div class="one_half column">
    <h5>ID NO: <asp:Literal ID="Empid" Text="" runat="server"></asp:Literal> </h5>
    <h5>NAME : <asp:Literal id="fullnam" Text="" runat="server"></asp:Literal></h5>
    </div>
    <div class="one_half column last">
    <h5><asp:Literal ID="prevcutoffdates" runat="server" Text=""></asp:Literal></h5>  
    <h5>CutOff Balance: <span></span><asp:Literal ID="curprevbal" runat="server" Text=""></asp:Literal></span> Php</h5>
     
    </div>

    <div class="clearboth"></div>
    <hr />
   
     
    <h2>Transaction Logs</h2>
    <div class="clearboth"></div>
   
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        DataSourceID="SqlDataSource1" 
        AllowSorting="True" EmptyDataText="No Records Found" 
        CssClass="fullwidth normal r1000px" BorderColor="Black" BorderStyle="Solid" DataKeyNames="transcode" 
        BorderWidth="1px" PageSize="15" EnableModelValidation="True">
       
    <AlternatingRowStyle BackColor="#CCFFCC" />
        <Columns>
            <%--<asp:BoundField DataField="transcode" HeaderText="Transaction Code"  
                DataFormatString=""  HtmlEncode="false" /> 

<asp:ButtonField ButtonType="Image" CommandName="btnshowDetail"  HeaderText=" Detail View"    Text="Show Details" ImageUrl="assets/icons/actions_small/Contacts.png" ItemStyle-CssClass="tooltip mpointer" ItemStyle-BackColor="Transparent" ControlStyle-BackColor="transparent" ControlStyle-BorderStyle="None" ControlStyle-BorderWidth="0" ControlStyle-CssClass="tooltip mpointer" />
--%>
            <asp:BoundField DataField="payrolldate" HeaderText="PKI Payroll Date"   
                DataFormatString="{0:dd/MM/yyyy}"  HtmlEncode="False" >
            </asp:BoundField>
            <asp:BoundField DataField="rangedate" HeaderText="Canteen Covered Dates"   
                DataFormatString="{0:dd/MM/yyyy}"  HtmlEncode="False" >
            </asp:BoundField>
            <asp:BoundField DataField="charges" HeaderText="Canteen Charge" 
                DataFormatString="{0:F2}"  HtmlEncode="False" ItemStyle-ForeColor="#FF3300"  >
<ItemStyle CssClass="tooltip mpointer" ForeColor="#FF3300"></ItemStyle>
            </asp:BoundField>
            

            <asp:BoundField DataField="subsidy" HeaderText="Regular Subsidy"  
                DataFormatString="{0:F2}"  HtmlEncode="false" 
                ItemStyle-ForeColor="#000099" >
            <ItemStyle ForeColor="#000099"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="ot_subsidy" HeaderText="Overtime Subsidy"  
                DataFormatString="{0:F2}"  HtmlEncode="false"   
                ItemStyle-ForeColor="#000099" >
            <ItemStyle ForeColor="#000099"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="othersub" HeaderText="Other Subsidy"  
                DataFormatString="{0:F2}"  HtmlEncode="false"   
                ItemStyle-ForeColor="#000099" >
            <ItemStyle ForeColor="#000099"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="subtotal" HeaderText="Subtotal"   ItemStyle-ForeColor="#FF3300"     
                DataFormatString="{0:F2}"  HtmlEncode="false"
                ItemStyle-CssClass="rightborder" >
            <ItemStyle CssClass="rightborder"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField DataField="payrolldeduction" HeaderText="Payroll Deduction"   ItemStyle-ForeColor="#FF3300"     
                DataFormatString="{0:F2}"  HtmlEncode="false" >
            </asp:BoundField>
            <asp:BoundField DataField="actualdeduction"  HeaderText="Actual Deduction" DataFormatString="{0:F2}" 
                HtmlEncode="False" />
            <asp:BoundField DataField="empbalance" DataFormatString="{0:F2}" 
                HeaderText="Balance" HtmlEncode="False" />
           <%--  <asp:BoundField DataField="balance" HeaderText="Balance"   DataFormatString="{0:F2}"  HtmlEncode="false" />
            <asp:BoundField DataField="remarks" HeaderText="Remarks"    /> --%>
        </Columns>
       
        <EmptyDataRowStyle BackColor="Black" BorderColor="Black" BorderStyle="Solid" 
            ForeColor="White" />
       
        <HeaderStyle Font-Size="XX-Small" HorizontalAlign="Center" BackColor="#003300" 
            ForeColor="White" />
       
        <RowStyle BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
       
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>" 
        SelectCommand="SELECT  TOP 100 transcode, CAST(recdate AS varchar(11)) AS  recdate , empId, incharge,CAST(cutoffstartdate AS varchar(7)) + '- ' + CAST(cutoffdate AS varchar(11))  AS rangedate, CAST(payrolldate AS varchar(11)) AS   payrolldate,    charges, subsidy, ot_subsidy, other_subsidy as othersub,subtotal ,payrolldeduction , actualdeduction, empbalance, payments, balance, remarks ,payable FROM tbl_logs WHERE (empId = @empno) AND void = 0 ORDER BY recno DESC  ">
         <%--AND cutoffstartdate &gt;= @sdate AND cutoffdate &lt;= @edate  --%>
        <SelectParameters>
            <asp:SessionParameter Name="empno" SessionField="IdNo" />
            <%--<asp:ControlParameter ControlID="startdate" Name="sdate" PropertyName="Text" />
            <asp:ControlParameter ControlID="enddate" Name="edate" PropertyName="Text" />--%>
        </SelectParameters>
    </asp:SqlDataSource>
</fieldset>
</asp:Content>

