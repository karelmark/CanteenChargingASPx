Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization 
Partial Class _Default
    Inherits System.Web.UI.Page
    Dim st_conn As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(st_conn)
    Dim sqlCmd As SqlCommand
    Dim sqlCmd2 As SqlCommand
    Dim cur_idno As Integer
    Dim cur_user As String
    Dim ds As New DataSet()
    Dim rem_sub As String
    Dim rem_bal As String
    Dim ot_sub As String
    Dim reg_sub As String
    Dim unpd_bal As String





    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        Dim sb As New StringBuilder
        Dim str As String
        Dim searchsum As String
        'Build Statement

        str = "SELECT DISTINCT transno, CAST(transdate AS varchar(11)) AS TransDate, " + _
        " status, remarks, " + _
        " (SELECT SUM(subtotal) AS thesum FROM tbl_transdetails " + _
        " WHERE (transno = tbl_transaction.transno) AND status ='OK') AS SubSum " + _
        " FROM tbl_transaction " + _
        " WHERE idno = @idno  "

		'str = "SELECT DISTINCT transno, CAST(transdate AS varchar(11)) AS TransDate, " + _
        '" status, remarks, " + _
        '" (SELECT SUM(txtotal) AS thesum FROM tbl_transaction " + _
        '" WHERE (idno = @idno) AS SubSum " + _
        '" FROM tbl_transaction " + _
        '" WHERE idno = @idno  "
		
        'str += " AND tbl_transaction.status = @status "

        If Not check_date() Then
            str += " AND tbl_transaction.transdate >= @startdate "
        End If

        'str += " AND tbl_transaction.transdate  <= @endate  "
        str += " AND tbl_transaction.transdate  <= @endate  "

        searchsum = "Select isnull(sum(T1.subsum), 0.00) as searchtotal FROM ( " & str & " ) as  T1"

        str += "  ORDER BY tbl_transaction.transno "

		
	  
		
        'err_msg.Text = check_date()
        sqlCmd = New SqlCommand(str, con)
        sqlCmd.Parameters.Add("@idno", SqlDbType.VarChar, 50).Value = cur_idno
       ' sqlCmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = DropStatus.SelectedValue.ToString
        sqlCmd.Parameters.Add("@startdate", SqlDbType.VarChar, 50).Value = txt_s_transdate.Text.ToString
        sqlCmd.Parameters.Add("@endate", SqlDbType.VarChar, 50).Value = txt_e_transdate.Text.ToString
        sqlcmd.CommandTimeout = 400

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim adpt As New SqlDataAdapter(sqlCmd)

        ds.Clear()

        adpt.Fill(ds)

        'SearchTable.DataSource = ds
        SearchTable.DataBind()
        'lblrecords.Text = SearchTable.Rows.Count

        con.Open()
        'err_msg.Text = check_date()

        sqlCmd2 = New SqlCommand(searchsum, con)
        sqlCmd2.Parameters.Add("@idno", SqlDbType.VarChar, 50).Value = cur_idno
        sqlCmd2.Parameters.Add("@status", SqlDbType.NVarChar).Value = DropStatus.SelectedValue.ToString
        sqlCmd2.Parameters.Add("@startdate", SqlDbType.VarChar, 50).Value = txt_s_transdate.Text.ToString
        sqlCmd2.Parameters.Add("@endate", SqlDbType.VarChar, 50).Value = txt_e_transdate.Text.ToString

        Dim sqldreader As SqlDataReader = sqlCmd2.ExecuteReader()
        If sqldreader.HasRows Then
            If sqldreader.Read() Then
                stotal.Text = Format(sqldreader("searchtotal"), "##0.00").ToString
            End If
        End If
        con.Close()


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim thisDate As Date = Now
        ' If Session("IDNO") Is Nothing Then
        cur_idno = Session("IDNO")
        cur_user = Session("UserName")

        If cur_idno = 0 Then
            Response.Redirect("Login.aspx")
       Else

            lblID.Text = cur_idno

            Dim empdetail(2) As String
            Dim fullname As String

            empdetail = get_emp_details()
            fullname = UCFirst(empdetail(0).ToString) & ", " & UCFirst(empdetail(1).ToString) & " " & UCFirst(empdetail(2).ToString)

            lblName.Text = fullname
            Session("fullname") = fullname


            Dim ses_string_s As String = Session("startdate")
            Dim ses_string_e As String = Session("enddate")
            Dim ses_status As String = Session("status")

            If Not Page.IsPostBack Then


                If ses_string_s <> "" Then
                    txt_s_transdate.Text = ses_string_s.ToString
                Else
                    txt_s_transdate.Text = thisDate.ToString("d")
                End If
                If ses_string_e <> "" Then
                    txt_e_transdate.Text = ses_string_e.ToString
                Else
                    txt_e_transdate.Text = thisDate.ToString("d")
                End If



            End If

        End If

        filltable()

        unpaiddate.Text = thisDate.ToString("D")

        ' ''''''''''''''''''''''
        Dim curbalance As String = get_cur_bal()
        Dim numcursubsidy As String = get_regular_subsidy()
        Dim curotsubsidy As String = get_ot_subsidy()
        Dim prevbalance As String = get_prevbal()
        Dim othersubs As String = get_other_subsidy()

        'Dim curbalance As String = 0
        'Dim numcursubsidy As String = 0
        'Dim curotsubsidy As String = 0
        'Dim prevbalance As String = 0
        'Dim othersubs As String = 0

        's

        'show the current values

        unpaidval.Text = Format(Val(curbalance), "##,##0.00").ToString
        reg_sub = Val(numcursubsidy) * 40
        subsidy.Text = Format(Val(reg_sub), "##,##0.00").ToString
        othersub.Text = Format(Val(othersubs), "##,##0.00").ToString
        o_subsidy.Text = Format(Val(curotsubsidy), "##,##0.00").ToString
        prev_rem_bal.Text = Format(Val(prevbalance), "##,##0.00").ToString

        REM rem_bal = ot_sub + reg_sub + remnbal - rem_sub
        Dim tbal As String

        tbal = Val(curbalance) - (Val(reg_sub) + Val(curotsubsidy) + Val(othersubs)) + Val(prevbalance)

        rem_bal = Format(Val(tbal), "##,##0.00").ToString

        subsidyVSunpaid.Text = rem_bal


        'highlight current menu page
        Dim btnlink As LinkButton

        btnlink = CType(Me.Master.FindControl("btnhome"), LinkButton)
         
        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If

        

    End Sub
    Private Shared Function UCFirst(ByVal value As String) As String
        ' Check for empty string.
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        End If
        ' Return char and concat substring.
        Return Char.ToUpper(value(0)) & value.Substring(1).ToLower
    End Function
    Function time_diff() As Integer
        Dim input1 As String = txt_s_transdate.Text.ToString
        Dim input2 As String = txt_e_transdate.Text.ToString
        Dim result As Integer
        Dim d1 As Double
        Dim d2 As Double

        Dim thisdate As Date = Now
        If input1 = "" Or input1 = Nothing Then
            input1 = thisdate.ToString("d")
        End If

        If input2 = "" Or input2 = Nothing Then
            input2 = thisdate.ToString("d")
        End If


        d1 = GetUnixTimestamp(input1)
        d2 = GetUnixTimestamp(input2)

        If d1 <= d2 Then
            result = 1
        
        Else
            result = 0
        End If

        time_diff = result

    End Function

    Function check_date() As Boolean
        Dim check As Integer
        check = time_diff()
        If check = 0 Then
            check_date = True
        Else
            check_date = False
        End If

    End Function

    Protected Sub filltable()
        Dim sb As New StringBuilder
        Dim str As String

        'Build Statement


        str = "SELECT   transno, CAST(transdate AS varchar(11)) AS TransDate, " + _
         " status," + _
         " (SELECT SUM(subtotal) AS thesum FROM tbl_transdetails " + _
         " WHERE (transno = tbl_transaction.transno) AND status='OK') AS SubSum " + _
         " FROM tbl_transaction " + _
         " WHERE idno = @idno  "

        str = "SELECT   transno, CAST(transdate AS varchar(11)) AS TransDate, " + _
         " status," + _
         "  txtotal AS SubSum " + _
         " FROM tbl_transaction " + _
         " WHERE idno = @idno  "


        If DropStatus.SelectedItem.Text = "Paid" Then
            str += " AND tbl_transaction.status = 1 "

        ElseIf DropStatus.SelectedItem.Text = "Unpaid" Then
            str += " AND tbl_transaction.status != 1 "
        Else

        End If

        If check_date() Then
            str += " AND tbl_transaction.transdate >= @startdate "
        End If

        str += " AND tbl_transaction.transdate  <= @endate  " + _
        " ORDER BY tbl_transaction.transno"

        sqlCmd = New SqlCommand(str, con)
        sqlCmd.Parameters.Add("@idno", SqlDbType.VarChar, 50).Value = cur_idno
        sqlCmd.Parameters.Add("@startdate", SqlDbType.VarChar, 50).Value = txt_s_transdate.Text.ToString
        sqlCmd.Parameters.Add("@endate", SqlDbType.VarChar, 50).Value = txt_e_transdate.Text.ToString


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Dim adpt As New SqlDataAdapter(sqlCmd)
        ds.Clear()
        sqlcmd.CommandTimeout = 400
        
        adpt.Fill(ds)
         
        SearchTable.DataBind()
         

    End Sub

 

    Protected Sub txt_e_transdate_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_e_transdate.TextChanged
        Dim input1 As String = txt_s_transdate.Text.ToString
        Dim input2 As String = txt_e_transdate.Text.ToString
        Dim d1 As Double
        Dim d2 As Double

        Dim thisdate As Date = Now

        

        If input1 = "" Or input1 = Nothing Then
            input1 = thisdate.ToString("d")
        End If

        If input2 = "" Or input2 = Nothing Then
            input2 = thisdate.ToString("d")
        End If


        d1 = GetUnixTimestamp(input1)
        d2 = GetUnixTimestamp(input2)

        If d1 > d2 Then
            err_msg.ForeColor = Drawing.Color.DarkMagenta
            err_msg.BorderColor = Drawing.Color.Red
            err_msg.Text = "Start Date is Greater than End Date"
        Else
            err_msg.Text = ""
        End If



    End Sub

    Private Function GetUnixTimestamp(ByVal currDate As DateTime) As Double
        'create Timespan by subtracting the value provided from the Unix Epoch
        Dim span As TimeSpan = (currDate - New DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime())
        'return the total seconds (which is a UNIX timestamp)
        Return span.TotalSeconds
    End Function

    Protected Sub SearchTable_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles SearchTable.RowCommand

        If e.CommandName = "btnDetail" Then

            Dim IntIndex As Integer = Convert.ToInt16(e.CommandArgument.ToString)
 
            Session("transno") = SearchTable.DataKeys(IntIndex).Value.ToString
            Session("UserName") = lblName.Text.ToString

            Session("startdate") = txt_s_transdate.Text.ToString
            Session("enddate") = txt_e_transdate.Text.ToString
            Response.Redirect("ViewDetail.aspx")


        End If

    End Sub

    

    Protected Sub DropStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropStatus.SelectedIndexChanged
        filltable()
    End Sub
    Function get_prevbal() As Object

        Dim result As String = 0
        Dim balstr As String = "SELECT TOP (1) empbalance FROM  tbl_logs WHERE EmpId = @empno AND VOID = 0 order by recno DESC"

        sqlCmd = New SqlCommand(balstr, con)
        sqlCmd.Parameters.Add("@empno", SqlDbType.Int, 50).Value = Session("IDNO")
        sqlCmd.Connection.Open()

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()
        If sqlreader.HasRows Then
            If sqlreader.Read() Then

                If sqlreader("empbalance").Equals(Nothing) Then
                    result = "0.00"
                Else
                    result = Val(sqlreader("empbalance")).ToString
                End If

            End If

        Else
            result = 0.0
        End If
        sqlCmd.Connection.Close()
        ' get the prev bal
        get_prevbal = result

    End Function
 
    Function get_cur_bal() As String
        con.Open()
        Dim thisdate As Date = Now
        Dim result As String = ""

        Dim str As String = "SELECT   isnull(SUM(txtotal),0) AS total " + _
                " FROM " + _
                "  tbl_transaction  as t2" + _
                " WHERE (t2.transdate  <= @tdate) " + _
                " AND (t2.status != 2) AND (t2.idno = @idno) "
        sqlCmd = New SqlCommand(str, con)
        sqlCmd.Parameters.Add("@tdate", SqlDbType.VarChar, 50).Value = thisdate.ToString("d") 
        sqlCmd.Parameters.Add("@idno", SqlDbType.VarChar, 50).Value = Session("IDNO")
        Dim sqlreader As SqlDataReader

        sqlreader = sqlCmd.ExecuteReader()
        If sqlreader.HasRows Then
            If sqlreader.Read() Then

                If sqlreader("total").Equals(Nothing) Then
                    result = "0.00"
                Else
                    result = Val(sqlreader("total")).ToString
                End If

            End If

        Else
            result = 0.0
        End If

        get_cur_bal = result
        con.Close()
    End Function
    Function get_regular_subsidy() As String

        Dim result As String = ""
        Dim strSql As String

        con.Open()
        strSql = "SELECT  isnull(COUNT(T2.empno),0) as num_subsidy  FROM vwAttendance AS T1 INNER JOIN  vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo WHERE (T1.empNo = @Empid) AND  (T2.HMFStatus IS NULL OR T2.HMFStatus <> '1D') AND STFShift NOT IN ('0', '-') AND T2.STFCanteenSub != 1 "
        sqlCmd = New SqlCommand(strSql, con)
        sqlCmd.Parameters.Add("@Empid", SqlDbType.VarChar, 50).Value = cur_idno
        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.HasRows Then
            If sqlreader.Read() Then
                If sqlreader(0).Equals(Nothing) Then
                    result = 0.0
                Else
                    result = Val(sqlreader(0)).ToString

                End If
            End If
        Else
            result = 0.0
        End If
        con.Close()

        get_regular_subsidy = result

    End Function

	function get_other_subsidy() as string
	 Dim result As String = ""
        Dim strSql As String
        con.Open()
        strSql = "SELECT  isnull(Sum(subsidy),0) as totalsub FROM tbl_specialSub WHERE empno = @Empid AND status = 0"
        sqlCmd = New SqlCommand(strSql, con)
        sqlCmd.Parameters.Add("@Empid", SqlDbType.VarChar, 50).Value = cur_idno
        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.HasRows Then
            If sqlreader.Read() Then
                If sqlreader(0).Equals(Nothing) Then
                    result = 0.0
                Else
                    result = Val(sqlreader(0)).ToString

                End If
            End If
        Else
            result = 0.0
        End If
        con.Close()

        get_other_subsidy = result

	end function
	
    Function get_ot_subsidy() As String
        Dim result As String = "" 
        Dim strSql As String
        checkOT()
        con.Open()

        strSql = "SELECT isnull(SUM(OT.OTSUBSIDY),0) as ot_sub FROM (SELECT    subsidy as OTSUBSIDY FROM vwOTSub  WHERE  Empno = @Empid AND  OTCanteenSUb = 0) as OT"
        sqlCmd = New SqlCommand(strSql, con)
        sqlCmd.Parameters.Add("@Empid", SqlDbType.VarChar, 50).Value = cur_idno
        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.HasRows Then
            If sqlreader.Read() Then
                If sqlreader(0).Equals(Nothing) Then
                    result = 0.0
                Else
                    result = Val(sqlreader(0)).ToString
                End If
            End If
        Else
            result = 0.0
        End If
        con.Close()
        get_ot_subsidy = result
    End Function
    Private Sub checkOT()

        Dim strsql As String
        con.Open()

        strsql = "SELECT  OTFID  ,  OTFHRS  , CASE WHEN OTFHRS  < 2 THEN 0.0 WHEN OTFHRS < 10 THEN 40.0 WHEN OTFHRS < 18 THEN 80.0 ELSE 120 END as  OTSUBSIDY FROM vwOvertime as T1 WHERE t1.Empno = @Empid AND t1.OTFPay is NULL"
        sqlCmd2 = New SqlCommand(strsql, con)
        sqlCmd2.Parameters.Add("@Empid", SqlDbType.VarChar, 50).Value = cur_idno
        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd2.ExecuteReader()

        If sqlreader.HasRows Then

            While sqlreader.Read()
                Dim otid As String
                otid = sqlreader("OTFID").ToString
                Dim retvalue As Boolean
                retvalue = checkotid(otid)

                If retvalue Then

                    'Dim otid As String
                    Dim subsidy As Double 
                    otid = sqlreader("OTFID").ToString
                    subsidy = Val(sqlreader("OTSUBSIDY").ToString)
                    'hrs = Val(sqlreader("OTFHRS").ToString)
                    insertdata(otid, subsidy)
                End If
            End While

        End If


        con.Close()

    End Sub

    Private Function checkotid(ByVal otid As String) As Boolean

        Dim strsql As String
        Dim localcmd As SqlCommand
        Dim result As Boolean = False
        Dim con2 As New Data.SqlClient.SqlConnection(st_conn)
        con2.Open()
        strsql = "SELECT OTFID FROM tbl_OT WHERE OTFID = @otid"
        localcmd = New SqlCommand(strsql, con2)
        localcmd.Parameters.Add("@otid", SqlDbType.VarChar, 50).Value = otid.ToString
        Dim localsqlreader As SqlDataReader

        localsqlreader = localcmd.ExecuteReader()

        If localsqlreader.HasRows Then
            result = False
        Else
            result = True
        End If
       
        con2.Close()

        Return result
    End Function

    Private Sub insertdata(ByVal otid As String, ByVal subsidy As Double)

        Dim strsql As String = ""
        Dim localcmd As SqlCommand
        Dim con2 As New Data.SqlClient.SqlConnection(st_conn)
        con2.Open()

        strsql = "INSERT INTO tbl_OT ([OTFID], [Subsidy], [OTCanteenSub]) VALUES " & _
                                    "( @otid , @subsidy, @sub )"
        localcmd = New SqlCommand(strsql, con2)
        localcmd.Parameters.Add("@otid", SqlDbType.VarChar, 50).Value = otid.ToString 
        localcmd.Parameters.Add("@subsidy", SqlDbType.VarChar, 50).Value = subsidy.ToString
        localcmd.Parameters.Add("@sub", SqlDbType.SmallInt).Value = 0
        'execute qurery

        Try
            localcmd.ExecuteNonQuery()
        Catch ex As SqlException
            If ex.Number = 2627 Then
                lblName.Text = ex.Message.ToString

            Else
                lblName.Text = ex.Message.ToString
                lblName.Visible = True

            End If
        End Try
        con2.Close()
    End Sub

    Function get_emp_details() As Object
        Dim result(2) As String
        Dim strsql As String
        con.Open()
        strsql = "SELECT * FROM vwEmployeeMaster  WHERE EmpNo = @empid"
        sqlCmd = New SqlCommand(strsql, con)
        sqlCmd.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = cur_idno

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.HasRows Then
            If sqlreader.Read() Then
                result(0) = sqlreader("Emplname").ToString
                result(1) = sqlreader("EmpFname").ToString
                result(2) = sqlreader("EmpMname").ToString
            End If

        Else
            result = Nothing
        End If

        con.Close()
        get_emp_details = result
    End Function
    Protected Sub Page_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        Dim intMatches As Integer
        Dim dv As Data.DataView

        dv = CType(SqlDataSource1.Select(DataSourceSelectArguments.Empty), Data.DataView)
        intMatches = dv.Count.ToString

        If intMatches = 0 Then
            litRecordCountText.Text = "<span style='color:'cc0000'>Try to check the start and end date(s).</span>"
            btnexport.Visible = False
        ElseIf intMatches = 1 Then
            litRecordCountText.Text = "1 match found. The following details matches your search."
            btnexport.Visible = True
        Else
            litRecordCountText.Text = intMatches & " matches found. The following  details match your search."
            btnexport.Visible = True
        End If

    End Sub


    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        'do nothing
    End Sub

    Protected Sub btnexport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnexport.Click

        Dim sb As New StringBuilder
        Dim str As String
        Dim runTotal As Double = 0

        Dim date1 As String
        Dim date2 As String
        Dim status As String

        date1 = txt_s_transdate.Text.ToString
        date2 = txt_e_transdate.Text.ToString
        status = DropStatus.Text.ToString

        'table variable declartions
        Dim scellstr As String = "<Cell ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">"
        Dim scellint As String = "<Cell ss:StyleID=""Integer"">" & "<Data ss:Type=""Number"">"
        Dim scelldbl As String = "<Cell ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">"
        Dim ecell As String = "</Data></Cell>"
        'Build Statement

        str = "SELECT DISTINCT transno, CAST(transdate AS varchar(11)) AS TransDate, transtime  , " + _
        " Case status when 1 then 'unpaid' Else 'paid' End as p_status," + _
        "  txtotal AS Total " + _
        " FROM tbl_transaction " + _
        " WHERE idno = @idno  "
        'str += " AND tbl_transaction.status = @status  "
        If Not check_date() Then
            str += " AND tbl_transaction.transdate >= @startdate "
        End If
        str += " AND tbl_transaction.transdate  <= @endate  "
        str += "  ORDER BY tbl_transaction.transno "


        sqlCmd = New SqlCommand(str, con)
        sqlCmd.Parameters.Add("@idno", SqlDbType.VarChar, 50).Value = cur_idno
       ' sqlCmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = DropStatus.SelectedValue.ToString
        sqlCmd.Parameters.Add("@startdate", SqlDbType.VarChar, 50).Value = txt_s_transdate.Text.ToString
        sqlCmd.Parameters.Add("@endate", SqlDbType.VarChar, 50).Value = txt_e_transdate.Text.ToString

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim adpt As New SqlDataAdapter(sqlCmd)
        ds.Clear()
        adpt.Fill(ds)

        Dim filen As String
        filen = "" & DateAndTime.Now.Millisecond.ToString & ".xls"
        Dim s As New StringBuilder()
        Const startExcelXML As String = "<xml version>" & vbCr & vbLf & "<Workbook " & "xmlns=""urn:schemas-microsoft-com:office:spreadsheet""" & vbCr & vbLf & " xmlns:o=""urn:schemas-microsoft-com:office:office""" & vbCr & vbLf & " " & "xmlns:x=""urn:schemas-    microsoft-com:office:" & "excel""" & vbCr & vbLf & " xmlns:ss=""urn:schemas-microsoft-com:" & "office:spreadsheet"">" & vbCr & vbLf & " <Styles>" & vbCr & vbLf & " " & "<Style ss:ID=""Default"" ss:Name=""Normal"">" & vbCr & vbLf & " " & "<Alignment ss:Vertical=""Bottom""/>" & vbCr & vbLf & " <Borders/>" & vbCr & vbLf & " <Font/>" & vbCr & vbLf & " <Interior/>" & vbCr & vbLf & " <NumberFormat/>" & vbCr & vbLf & " <Protection/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "<Style ss:ID=""BoldColumn"">" & vbCr & vbLf & " <Font " & "x:Family=""Swiss"" ss:Bold=""1""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "<Style     ss:ID=""StringLiteral"">" & vbCr & vbLf & " <NumberFormat" & " ss:Format=""@""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " <Style " & "ss:ID=""Decimal"">" & vbCr & vbLf & " <NumberFormat " & "ss:Format=""0.0000""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "<Style ss:ID=""Integer"">" & vbCr & vbLf & " <NumberFormat " & "ss:Format=""0""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " <Style " & "ss:ID=""DateLiteral"">" & vbCr & vbLf & " <NumberFormat " & "ss:Format=""mm/dd/yyyy;@""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "</Styles>" & vbCr & vbLf & " "
        Const endExcelXML As String = "</Workbook>"
        s.Append(startExcelXML)
        s.Append("<Worksheet ss:Name='Sheet1'>")
        s.Append("<Table>")
        
		s.Append("<Row>")
        s.Append("<Cell ss:MergeAcross=""3"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        s.Append("Dates From:" & date1 & " To: " & date2  & ecell)
        s.Append("</Row>")
        
		
		s.Append("<Row>")
       	s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Tx #")
            s.Append("</Data></Cell>")
			
			s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Date")
            s.Append("</Data></Cell>")			
			
			s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Time")
            s.Append("</Data></Cell>")
			
			s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Status")
            s.Append("</Data></Cell>")

			s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Transaction Total")
            s.Append("</Data></Cell>")
			s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
			
            s.Append("#")
            s.Append("</Data></Cell>")
            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Name")
            s.Append("</Data></Cell>")

            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("QTY")
            s.Append("</Data></Cell>")

            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Price")
            s.Append("</Data></Cell>")

            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Subtotal")
            s.Append("</Data></Cell>")
 
        s.Append("</Row>")
		
        Dim innercon As New Data.SqlClient.SqlConnection(st_conn)
        Dim innersql As String
        Dim innercmd As SqlCommand
        Dim innerds As New DataSet()
        Dim inneradpt As SqlDataAdapter
        dim firstdetail as boolean = true

         
            For Each dr As DataRow In ds.Tables(0).Rows
                s.Append("<Row>")
                s.Append(scellint & dr("transno").ToString & ecell)
                s.Append(scellstr & dr("TransDate").ToString & ecell)
                s.Append(scellstr & dr("transtime").ToString & ecell)
                s.Append(scellstr & dr("p_status").ToString & ecell)
                s.Append(scelldbl & dr("Total").ToString & ecell)

            runTotal += Val(dr("Total"))
            If Val(dr("Total")) = 0 And firstdetail Then
                s.Append("</Row>")
            Else
                If Not firstdetail Then
                    s.Append("</Row>")
                End If

                innersql = "SELECT t1.itemno as itemno ,t1.itemcode as itemcode,t1.qty as qty ,t1.unitcode as unitcode,t1.price as price,t1.subtotal as subtotal, t2.itemname as itemname  FROM tbl_transdetails as t1 , tbl_inventory as t2 WHERE " + _
                                    " transno = @tr and t1.itemcode = t2.recno  AND t1.status = 'OK' ORDER BY itemno"
                innercmd = New SqlCommand(innersql, innercon)
                innercmd.Parameters.Add("@tr", SqlDbType.VarChar, 50).Value = dr("transno").ToString
                inneradpt = New SqlDataAdapter(innercmd)
                innerds.Clear()
				
				


                inneradpt.Fill(innerds)

                If (innerds.Tables(0).Rows.Count > 0) Then
                    For Each innerdr As DataRow In innerds.Tables(0).Rows

                        If Not firstdetail Then

                            s.Append("<Row>")
                            s.Append(scellstr & "  " & ecell)
                            s.Append(scellstr & "  " & ecell)
                            s.Append(scellstr & "  " & ecell)
                            s.Append(scellstr & "  " & ecell)
                            s.Append(scellstr & "  " & ecell)
                        End If
                        s.Append(scellstr & innerdr("itemno").ToString & ecell)
                        s.Append(scellstr & innerdr("itemname").ToString & ecell)
                        s.Append(scellint & innerdr("qty").ToString & ecell)
                        s.Append(scelldbl & innerdr("price").ToString & ecell)
                        s.Append(scelldbl & innerdr("subtotal").ToString & ecell)
                        s.Append("</Row>")


                        firstdetail = False
                    Next
                End If

                End If


                firstdetail = True

        Next


        s.Append("<Row><Cell  ss:MergeAcross=""9"" ><Data ss:Type=""String""></Data></Cell></Row>")
        s.Append("<Row>")
        s.Append("<Cell ss:MergeAcross=""3"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        s.Append("Total: " & ecell)
        s.Append("<Cell  ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">")
        s.Append(runTotal.ToString & ecell)

        s.Append(scellstr & "  " & ecell)
        s.Append(scellstr & "  " & ecell)
        s.Append(scellstr & "  " & ecell)
        s.Append(scellstr & "  " & ecell)
        s.Append("<Cell  ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">")
        s.Append(runTotal.ToString & ecell)
        
		s.Append("</Row>")


        s.Append("</Table>")
        s.Append(" </Worksheet>")
        s.Append(endExcelXML)

        Response.Clear()
        Response.AddHeader("content-disposition", "attachment;filename=ccs" & cur_idno & "report" & date1 & "to" & date2 & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.xls"
        Response.ContentEncoding = System.Text.Encoding.Default
        Response.Write(s.ToString())
        Response.End() 

    End Sub



 
    Protected Sub SearchTable_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchTable.SelectedIndexChanged

    End Sub

    Protected Sub OnRowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles SearchTable.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If TryCast(e.Row.DataItem, DataRowView)("remarks").ToString() <> "-" Then
                e.Row.ToolTip = TryCast(e.Row.DataItem, DataRowView)("remarks").ToString()
            End If
        End If
    End Sub
End Class
