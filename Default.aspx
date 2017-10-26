<%@ Page Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" title="Canteen Charging System Version 1.0" EnableEventValidation="false" %> 
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
         
        <div class="clearboth"></div>  
		
			<div class="fullwidth">
						<hr />
			<div class="one_half column">
			<h2>
			<asp:Label ID="Label1" runat="server" Text="ID No:"></asp:Label>
			<asp:Label ID="lblID" runat="server" Text=""></asp:Label>
			</h2>
			</div>
			<div class="one_half last column">
			<h2>
			<asp:Label ID="Label2" runat="server" Text="Name:"></asp:Label>
			<asp:Label  ID="lblName" runat="server" CssClass="capitalize"></asp:Label>
			</h2>
			</div>
			<hr />  
			<div class="clearboth"></div>
			
			<div class="one_half column">
				<div class="portlet">
				<div class="portlet-header">Summary:</div>
				<div class="portlet-content" style="display: block;">
            <div id="unpaid">
			 
                <table class="fullwidth normal">
				<tbody style="background-color:#DFF0D8;">
                <tr class="tooltip mpointer" title="Charges not included from the last payroll cutoff">
                <td>Pending Unpaid Charges:</td>
                <td><asp:Label ID="unpaidval" Text="0.00"  runat="server" Font-Bold="True" ForeColor="#C00000"  ></asp:Label> Php</td>
                </tr> 
                <tr>
                <td>Regular Subsidy:</td>
                <td><asp:Label ID="subsidy" Text="0.00" runat="server" Font-Bold="True" ForeColor="Blue" ></asp:Label> Php</td>
                </tr>
                <tr>
                <td>Overtime Subsidy:</td>
                <td><asp:Label ID="o_subsidy" Text="0.00" runat="server" Font-Bold="True" ForeColor="Blue" ></asp:Label> Php</td>
                </tr>   
				 
				<tr class="tooltip mpointer" title="Subsidies from Meetings and Others">
                <td>Other Subsidy:</td>
                <td><asp:Label ID="othersub" Text="0.00" runat="server" Font-Bold="True" ForeColor="Blue" ></asp:Label> Php</td>
                </tr>
 				 
                <tr class="tooltip mpointer" title="Previous Payroll CuttOff Balance">
                <td>Last CuttOff Balance:</td>
                <td><asp:Label ID="prev_rem_bal" Text="0.00" runat="server" Font-Bold="True" ForeColor="#C00000" ></asp:Label> Php</td>
                </tr>    
                 <tr style="background-color:#F2DEDE;" class="tooltip mpointer" title="Current Net Balance" ><td>(Charges + PrevBalance - Subsidies) Total:</td>
                 <td><asp:Label ID="subsidyVSunpaid" Text="0.00" runat="server" Font-Bold="True" ForeColor="#C00000"></asp:Label> Php 
                 </td></tr> 
				 </tbody>
                </table>
              <!--<span style="font-size: 12px;">Unpaid Balance as of </span><asp:Label ID="unpaiddate" runat="server" Text=""></asp:Label> -->
             </div>
             </div>
             </div>
			 
             </div>
			 <div class="one_half last column">
			 	<div class="portlet">
				<div class="portlet-header">Search:</div>
				<div class="portlet-content" style="display: block;">
		    <table class="fullwidth normal">
			<tbody style="background-color:#DFF0D8;">
            <tr>
            <td>From : </td><td><asp:TextBox ID="txt_s_transdate" runat="server" CssClass="datepicker" CausesValidation="True"></asp:TextBox></td>
            </tr>
            <tr>
            <td>To:</td><td><asp:TextBox ID="txt_e_transdate" runat="server" CssClass="datepicker"></asp:TextBox></td>    
		    </tr>
            <!--<tr>
            <td> <asp:Label ID="Label4" runat="server" Text="Status:"></asp:Label></td>
            <td> <asp:DropDownList ID="DropStatus" runat="server">
                     <asp:ListItem Value="1">Unpaid</asp:ListItem>
                     <asp:ListItem Value="2">Cutoff</asp:ListItem>
                 </asp:DropDownList>
            </td>
            </tr> -->
            <tr> 
		      <td colspan="2">
              <div class="two_third">
		        <p style="text-align:center;">Total Amount Search: <br /><span style="color:#C00000;"> <asp:Literal ID="stotal" runat="server" Text="0.00"></asp:Literal>&nbsp;</span>Php</p>
		      </div>
		      <div class="one_third last">
                 <p><asp:Button ID="btnSearch" runat="server" Text="Search" /></p>
              </div>
              </td>
            </tr>
			 </tbody>
            </table>
<div class="one_third" >
    <asp:Button ID="btnexport" runat="server" Text="Export to Excel" />
</div>
	    </div>
        </div>
		</div>
         <div class="clearboth"></div>
             <hr />
<div class="column">
 	 		<h5>
              <asp:Label ID="err_msg" runat="server" Text=""></asp:Label>
              <asp:Literal ID="litRecordCountText" runat="server" /> 
            </h5>
            <div class="clearboth"></div>
             <hr />
               
            <div style="min-height:450px;clear:both;">
             <asp:GridView  EmptyDataText="No Records Found!" CssClass="fullwidth fancy" 
                    ShowFooter="True"  runat="server" AllowPaging="True" AllowSorting="True" 
                    PageSize="30" AutoGenerateColumns="False" ID="SearchTable" 
                    DataSourceID="SqlDataSource1" DataKeyNames="transno" 
                    EnableModelValidation="True"  >
              <AlternatingRowStyle BackColor="#CCFFCC" CssClass="tooltip mpointer" />
              <RowStyle CssClass="tooltip mpointer" />
              <Columns>
                    <asp:BoundField DataField="transno" HeaderText="Trans No."  />
                    <asp:BoundField SortExpression="Transdate" DataField="Transdate" HeaderText="Date" HtmlEncode="False" />
                    <asp:BoundField SortExpression="Transtime" DataFormatString="{0:t}" DataField="recorddate" HeaderText="Time" HtmlEncode="False" />
                    <asp:BoundField SortExpression="SubSum" DataFormatString="{0:F3}"   DataField="SubSum" HeaderText="Subtotal"    HtmlEncode="False" />
                    <asp:BoundField DataField="statusname" HeaderText="Status" SortExpression="statusname"    />
                    <asp:BoundField DataField="txtype" HeaderText="Tx-Type" SortExpression="txtype"    />

                    <asp:ButtonField ButtonType="Image" CommandName="btnDetail" HeaderText="Details"
                       Text="View Details" DataTextField="transno" ImageUrl="~/assets/icons/small_icons/View.png">
                        <ItemStyle CssClass="btn tooltip mpointer" BorderStyle="Solid"  BackColor="Transparent" />
                        <ControlStyle BackColor="ControlLightLight" BorderColor="Control" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>

                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:CCSConnectionString %>" 
                    SelectCommand="SELECT   transno,transtime as transtime,cardno,remarks, txtype = CASE cardno  WHEN '-' THEN 'Manual' ELSE 'Card' END, CAST(transdate AS varchar(11)) AS TransDate, recorddate,
statusname = CASE status WHEN 1 THEN 'Unpaid' WHEN 2 THEN 'Cuttoff' ELSE 'Unknown' END , txtotal AS SubSum FROM tbl_transaction WHERE (idno = @idno)   AND transdate BETWEEN  @startdate  AND  @endate  ORDER BY transno DESC">
                <SelectParameters>
                         <asp:ControlParameter ControlID="lblID" Name="idno" PropertyName="Text" DefaultValue="" Type="Int32" />
                         <asp:ControlParameter ControlID="DropStatus" DefaultValue="0" Name="stat" PropertyName="SelectedValue"
                             Type="Int16" />
                         <asp:ControlParameter ControlID="txt_s_transdate" DefaultValue="" Name="startdate"
                             PropertyName="Text" ConvertEmptyStringToNull="False" />
                         <asp:ControlParameter ControlID="txt_e_transdate" Name="endate" PropertyName="Text" ConvertEmptyStringToNull="False" />
                     </SelectParameters>
                     <FilterParameters>
                         <asp:ControlParameter ControlID="txt_s_transdate" DefaultValue="" Name="start_date"
                             PropertyName="Text" />
                         <asp:ControlParameter ControlID="txt_e_transdate" DefaultValue="" Name="end_date"
                             PropertyName="Text" />
                     </FilterParameters>
                </asp:SqlDataSource>
                
               </div>
</div>
<div class="clearfix"></div>
<hr />

               
               
            </div>
             
    
</asp:Content>

