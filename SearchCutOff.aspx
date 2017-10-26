<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="SearchCutOff.aspx.vb" Inherits="SearchCutOff" title="Set Cut-Off Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <hr /> 
    
    <asp:Panel ID="Panel1" runat="server" Height="20px" Width="100%" Visible="False">
    <div class="notification success clearboth"> 
    <span></span> 
    <div class="text"> 
    <p><strong>Success!</strong>Cut-Off Posted</p> 
    </div> 
    </div>
     </asp:Panel>
       <asp:Panel ID="Panel2" runat="server" Height="20px" Width="100%" Visible="false">
       <div class="notification error clearboth"> 
    <span></span> 
    <div class="text"> 
    <p><strong>Error!</strong>Nothing Selected.</p> 
    </div> 
    </div>
    </asp:Panel>

     <div class="clearboth"></div>
    
         <fieldset>
     <div class="clearboth"></div>

        <h5>Set Cut Off</h5>
        <hr />
        <div class=" column">
         <label>ID / Name:</label> <asp:TextBox ID="Name" CssClass="sf" runat="server"></asp:TextBox>
         <asp:Button ID="btnSearch" runat="server" Text="Search" />
         </div>
         <hr />
         <div class="column">
         <label>Cut-Off Date:</label> <br />
         <label>From: </label>
         <asp:TextBox ID="startoffdate" CssClass="datepicker" runat="server" AutoPostBack="True"></asp:TextBox>
        
         <label>To: </label><asp:TextBox ID="cutoffdate" CssClass="datepicker" runat="server" AutoPostBack="True"></asp:TextBox>
          
          <asp:Button ID="btnCutoff" CssClass="tooltip mpointer" ToolTip="Please Review Before Posting "  runat="server" Text="Post Cut-Off" OnClientClick="return confirm('Are you sure you with this action?');"  />

        </div>
      <hr />
        <asp:Label ID="lblmsg" runat="server" Text=" " Visible="False"></asp:Label>
 <div class="clearboth"></div>       
 <div class="three_fourth column"> 
     <asp:ImageButton   CssClass="tooltip mpointer" ToolTip="Select All"  ID="AllCheck"  ImageAlign="Middle"  runat="server" ImageUrl="~/assets/icons/small_icons/Yes.png" />
     <asp:ImageButton   CssClass="tooltip mpointer" ToolTip="Unselect All"  ImageAlign="Middle"   runat="server" ID="AllUncheck" ImageUrl="~/assets/icons/small_icons/No.png" />

  </div>
  <div class="one_fourth culumn last">
      <asp:ImageButton   CssClass="tooltip mpointer" ToolTip="Export Selected Employees to Excel"  ID="exportselected" ImageAlign="Middle" runat="server" ImageUrl="~/assets/icons/small_icons/Flag.png" />
      <asp:ImageButton    CssClass="tooltip mpointer" ID="exportall" runat="server" ImageAlign="Middle" ImageUrl="~/assets/icons/small_icons/Download.png" ToolTip="Export All to Excel" />
  </div>
 <div class="clearboth"></div>
    <asp:GridView ID="SearchTable" runat="server" AllowPaging="False" 
            CssClass="fullwidth normal" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" 
            DataKeyNames="EmpNo,Subsidy,OtSubsidy,Payables" 
            EnableModelValidation="True" SelectedRowStyle-BackColor="#FFCCFF" 
            EmptyDataText="No Record Found" EnableTheming="False">
      
    <AlternatingRowStyle BackColor="#CCFFCC" />
        <Columns>
         
<asp:TemplateField>
     
    <ItemTemplate>
        <asp:CheckBox runat="server" ID="cbSelect" />
    </ItemTemplate>
</asp:TemplateField>
            <asp:BoundField DataField="EmpNo" HeaderText="ID" SortExpression="EmpNo" />
            <asp:BoundField DataField="empName" HeaderText="Name" SortExpression="EmpName" />
            <asp:BoundField DataField="Subsidy" HeaderText="Subsidy" SortExpression="Subsidy" dataformatstring="{0:F2}" HtmlEncode="false" />
            <asp:BoundField DataField="OtSubsidy" HeaderText="OT-Subsidy" SortExpression="OtSubsidy" dataformatstring="{0:F2}" HtmlEncode="false" />
            <asp:BoundField DataField="Payables" HeaderText="Payables" SortExpression="Payables" dataformatstring="{0:F2}" HtmlEncode="false" />
            <asp:BoundField DataField="Prevbal" HeaderText="Balance" SortExpression="Prevbal" dataformatstring="{0:F2}" HtmlEncode="false" />
            <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" SortExpression="Subtotal" dataformatstring="{0:F2}" HtmlEncode="false" />
            <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/icons/actions_small/Zip.png"
                Text="Button" CommandName="btndownload" />
           <%-- <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/icons/actions_small/Highlighter.png"
                Text="Button" CommandName="btnpayment" />--%>
        </Columns>
            
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>"
          
            
            SelectCommand="SELECT EmpNo AS empno, EmpLName + ', ' + EmpFName + ' '+ EmpMName AS empName, OtSubsidy, Subsidy, Payables, Prevbal, OtSubsidy + Subsidy + Payables AS Subtotal FROM (SELECT EmpNo, EmpLName, EmpFName, EmpMName, (SELECT ISNULL(SUM(OTSUBSIDY), 0) AS Expr1 FROM (SELECT T2.Subsidy AS OTSUBSIDY FROM tbl_OT AS T2 INNER JOIN vwOvertime AS T1 ON T1.OTFID = T2.OTFID WHERE (T2.OTCanteenSub = 0) AND (T1.EmpNo = cutoff.EmpNo) AND (T1.OTFDate &gt;= @bdate) AND (T1.OTFDate &lt;= @sdate)) AS OT) AS OtSubsidy, (SELECT ISNULL(COUNT(T2.EmpNo), 0) * 40 AS num_subsidy FROM vwAttendance AS T1 INNER JOIN vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo AND T1.AttDate &gt;= @bdate AND T1.AttDate &lt;= @sdate AND T2.STFActualDate &lt;= @sdate AND T2.STFActualDate &gt;= @bdate WHERE (T1.EmpNo = cutoff.EmpNo) AND (T2.HMFDate IS NULL) AND (T2.STFCanteenSub &lt;&gt; 1)) AS Subsidy, (SELECT ISNULL(SUM(subtotal), 0) AS total FROM (SELECT t1.subtotal FROM tbl_transdetails AS t1 INNER JOIN tbl_transaction AS t2 ON t1.transno = t2.transno WHERE (t2.status = 1) AND (t2.idno = cutoff.EmpNo) AND (t2.transdate &gt;= @bdate) AND (t2.transdate &lt;= @sdate)) AS derivedtbl_1) AS Payables, (SELECT ISNULL(SUM(prevbal), 0) AS prevbal FROM (SELECT TOP (1) balance AS prevbal FROM tbl_logs AS Logs WHERE (empId = cutoff.EmpNo) ORDER BY recno DESC) AS BAL) AS Prevbal FROM vwEmployeeMaster AS cutoff WHERE (EmpNo LIKE '%' + @txtbox + '%') OR (EmpLName LIKE '%' + @txtbox + '%') OR (EmpFName LIKE '%' + @txtbox + '%')  ) AS T10 ORDER BY emplname">
        <SelectParameters>
            <asp:ControlParameter ControlID="startoffdate" Name="bdate" 
                PropertyName="Text" />
            <asp:ControlParameter ControlID="cutoffdate" Name="sdate" PropertyName="Text" />
            <asp:ControlParameter ControlID="Name" DefaultValue=" " Name="txtbox" PropertyName="Text" />
        </SelectParameters>
    </asp:SqlDataSource>
    <hr />
    </fieldset>   

</asp:Content>

