Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class SearchCutOff
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


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim thisdate As Date = Now
        'Dim past15days As DateTime = DateTime.Today.Month.ToString
        cur_idno = Session("IDNO")
        cur_user = Session("UserName")

        If cur_idno = 0 Then

            Response.Redirect("Login.aspx")

        Else

            If Not Page.IsPostBack Then
                'past15days.ToString("d")
                startoffdate.Text = "01/01/2014".ToString
                cutoffdate.Text = thisdate.ToString("d")

            End If
        End If
        checkOT_all()
        'highlight current menu page
        Dim btnlink As LinkButton

        btnlink = CType(Me.Master.FindControl("btncutoff"), LinkButton)

        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If


    End Sub




    Private Sub checkOT_all()

        Dim strsql As String
        con.Open()

        strsql = "SELECT  OTFID  ,  OTFHRS  , CASE WHEN OTFHRS  < 2 THEN 0.0 WHEN OTFHRS < 10 THEN 40.0 WHEN OTFHRS < 18 THEN 80.0 ELSE 120 END as  OTSUBSIDY FROM vwOvertime as T1 WHERE   t1.OTFPay is NULL"
        sqlCmd2 = New SqlCommand(strsql, con)

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd2.ExecuteReader()
        If sqlreader.HasRows Then

            While sqlreader.Read()
                Dim otid As String
                otid = sqlreader("OTFID").ToString
                Dim retvalue As Boolean
                retvalue = checkotid(otid)

                If retvalue Then


                    Dim subsidy As Double 
                    otid = sqlreader("OTFID").ToString
                    subsidy = Val(sqlreader("OTSUBSIDY").ToString)

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
                'lblName.Text = ex.Message.ToString

            Else
                'lblName.Text = ex.Message.ToString
                'lblName.Visible = True

            End If
        End Try
        con2.Close()
    End Sub
    Protected Sub btnCutoff_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCutoff.Click

        Dim atLeastOneRow As Boolean = False
        Dim r As String
        r = randomcode()
        Dim tcode As String
        tcode = Hex(Val(Format(Now, "yyMMddhhmmssffff")))

        ' Iterate through the Products.Rows property
        For Each row As GridViewRow In SearchTable.Rows
            ' Access the CheckBox
            Dim cb As CheckBox = row.FindControl("cbSelect")
            If cb IsNot Nothing AndAlso cb.Checked Then

                atLeastOneRow = True

                Dim EmpNumber As Integer
                Dim Subsidy As Double
                Dim OtSubsidy As Double
                Dim CPayables As Double


                EmpNumber = Convert.ToInt32(SearchTable.DataKeys(row.RowIndex).Values(0))
                Subsidy = Convert.ToDouble(SearchTable.DataKeys(row.RowIndex).Values(1))
                OtSubsidy = Convert.ToDouble(SearchTable.DataKeys(row.RowIndex).Values(2))
                CPayables = Convert.ToDouble(SearchTable.DataKeys(row.RowIndex).Values(3))
                updatelog(EmpNumber, Subsidy, OtSubsidy, CPayables, r, tcode)


                 
            Else

                'do nothing

            End If

        Next

        If atLeastOneRow Then
            Panel1.Visible = True
            Panel2.Visible = False
            ToggleCheckState(False)
        Else
            Panel2.Visible = True
            Panel1.Visible = False
        End If
        ' Show the Label if at least one row was deleted...
        lblmsg.Visible = atLeastOneRow
        SearchTable.DataBind()

    End Sub
    Private Sub update_transaction(ByVal empno As Integer, ByVal cutdate As String, ByVal startdate As String)

        Dim updstr As String

        updstr = "Update tbl_transaction SET tbl_transaction.status = 2 WHERE idno = @empno AND transdate >= @sdate AND transdate <= @cutdate"
        sqlCmd = New SqlCommand(updstr, con)
        sqlCmd.Parameters.Add("@empno", SqlDbType.Int).Value = empno
        sqlCmd.Parameters.Add("@cutdate", SqlDbType.DateTime).Value = cutdate
        sqlCmd.Parameters.Add("@sdate", SqlDbType.DateTime).Value = startdate

        sqlCmd.Connection.Open()
        Try
            sqlCmd.ExecuteNonQuery()
        Catch ex As SqlException
            If ex.Number = 2627 Then
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True
            Else
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True

            End If
        End Try
        sqlCmd.Connection.Close()


    End Sub

    Private Sub update_shifting(ByVal empno As Integer, ByVal cutdate As String, ByVal startdate As String)
        Dim updstr As String
        updstr = "Update vwShifting SET vwShifting.STFCanteenSub = 1 WHERE vwShifting.EmpNo = @empno AND vwShifting.STFActualDate >= @sdate AND vwShifting.STFActualDate <= @cutdate "


        sqlCmd = New SqlCommand(updstr, con)
        sqlCmd.Parameters.Add("@empno", SqlDbType.Int).Value = empno
        sqlCmd.Parameters.Add("@cutdate", SqlDbType.DateTime).Value = cutdate
        sqlCmd.Parameters.Add("@sdate", SqlDbType.DateTime).Value = startdate


        sqlCmd.Connection.Open()
        Try
            sqlCmd.ExecuteNonQuery()
        Catch ex As SqlException
            If ex.Number = 2627 Then
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True
            Else
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True

            End If
        End Try
        sqlCmd.Connection.Close()

    End Sub
    Private Sub update_OTSub(ByVal empno As Integer, ByVal cutdate As String, ByVal startdate As String)
        Dim idsstr As String
        Dim localcmd As SqlCommand

        Dim result As Boolean = False
        Dim con3 As New Data.SqlClient.SqlConnection(st_conn)

        idsstr = "SELECT t1.OTFID as OTFID FROM vwOvertime as t1 inner join tbl_OT as t2 ON t1.OTFID = t2.OTFID  WHERE t1.EmpNo = @empno AND t1.OTFDate >= @sdate AND t1.OTFDate <= @cutdate"
        'idsstr = "SELECT OTFID FROM tbl_OT"

        localcmd = New SqlCommand(idsstr, con3)

        localcmd.Parameters.Add("@empno", SqlDbType.Int).Value = empno
        localcmd.Parameters.Add("@cutdate", SqlDbType.DateTime).Value = cutdate
        localcmd.Parameters.Add("@sdate", SqlDbType.DateTime).Value = startdate

        Dim localsqlreader As SqlDataReader

        con3.Open()
        localsqlreader = localcmd.ExecuteReader()

        If localsqlreader.HasRows Then
            While localsqlreader.Read()
                Dim eachid As String
                eachid = localsqlreader("OTFID").ToString
                UpdateOT(eachid)

            End While
        End If
        con3.Close()
    End Sub

    Private Sub UpdateOT(ByVal eachid As String)
        Dim updstr As String
        updstr = "Update tbl_OT SET OTCanteenSub = 1 WHERE OTFID = @id"

        Dim con3 As New Data.SqlClient.SqlConnection(st_conn)
        Dim localcmd1 As SqlCommand

        localcmd1 = New SqlCommand(updstr, con3)
        localcmd1.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = eachid


        localcmd1.Connection.Open()
        Try
            localcmd1.ExecuteNonQuery()
        Catch ex As SqlException
            If ex.Number = 2627 Then
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True
            Else
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True

            End If
        End Try
        localcmd1.Connection.Close()

    End Sub
    Private Sub updatelog(ByVal empno As Integer, ByVal s As Double, ByVal o As Double, ByVal p As Double, ByVal r As String, ByVal tcode As String)
      

        Dim incharge As String
        incharge = Session("IDNO")
        Dim prevbal As Double
        prevbal = get_prevbal(empno)
        Dim startdate As String = startoffdate.Text.ToString
        Dim cutdate As String = cutoffdate.Text.ToString()

        'create transaction 32char code for logs
        'tcode = Hex(Val(Format(Now, Val(incharge)))) & "-" & Hex(Val(Format(Now, "MMddyyyyhhmmsstt")))


        'update tbl_transaction.status
        update_transaction(empno, cutdate, startdate)


        'update shifting.stfcanteensub
        update_shifting(empno, cutdate, startdate)

        'update OT.OTCanteenSub
        update_OTSub(empno, cutdate, startdate)

        'record logs
        Dim recstr As String
        recstr = "INSERT INTO tbl_logs " & _
                 " ([transcode],[recdate],[rectime],[empid],[incharge], " & _
                 " [cutoffstartdate],[cutoffdate], [charges], [subsidy], [ot_subsidy], " & _
                 " [subtotal],  [payments],[balance],[remarks]) " & _
                 " VALUES " & _
                 " ( @tcode , @recdate , @rectime, @empid, @incharge, " & _
                 " @cutoffstartdate,@cutoffdate,@charges, @subsidy, @ot_subsdiy," & _
                 " @subtotal, @payments , @balance,  @remarks)"

        sqlCmd = New SqlCommand(recstr, con)
        sqlCmd.Parameters.Add("@tcode", SqlDbType.VarChar, 32).Value = tcode
        sqlCmd.Parameters.Add("@recdate", SqlDbType.DateTime).Value = Now.ToString("MM/dd/yyyy hh:mm:ss tt")
        sqlCmd.Parameters.Add("@rectime", SqlDbType.VarChar, 50).Value = Now.ToString("T")
        sqlCmd.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = empno
        sqlCmd.Parameters.Add("@incharge", SqlDbType.VarChar, 50).Value = incharge
        sqlCmd.Parameters.Add("@cutoffstartdate", SqlDbType.DateTime).Value = startdate
        sqlCmd.Parameters.Add("@cutoffdate", SqlDbType.SmallDateTime).Value = cutoffdate.Text.ToString()

        sqlCmd.Parameters.Add("@subsidy", SqlDbType.Decimal).Value = s   'subsidy for the cutoff
        sqlCmd.Parameters.Add("@ot_subsdiy", SqlDbType.Decimal).Value = o 'ot subsidy for the cutoff
        sqlCmd.Parameters.Add("@payments", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@charges", SqlDbType.Decimal).Value = p

        Dim bal As Double = 0

        bal = p - (s + o)

        sqlCmd.Parameters.Add("@subtotal", SqlDbType.Decimal).Value = bal
        sqlCmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = (prevbal + bal) 'previous balance + charges - (subsidy's)
        sqlCmd.Parameters.Add("@remarks", SqlDbType.Text).Value = "Cut-off Record"

        sqlCmd.Connection.Open()

        ' lblmsg.Text = prevbal
        lblmsg.Visible = True
        Try
            sqlCmd.ExecuteNonQuery()
        Catch ex As SqlException
            If ex.Number = 2627 Then
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True
            Else
                lblmsg.Text = ex.Message.ToString
                lblmsg.Visible = True

            End If
        End Try

        sqlCmd.Connection.Close()



    End Sub
    Private Function get_prevbal(ByVal idno As Integer)
        Dim result As String = 0
        Dim balstr As String = "SELECT TOP (1) balance FROM  tbl_logs WHERE EmpId = @empno order by recno DESC"

        sqlCmd = New SqlCommand(balstr, con)
        sqlCmd.Parameters.Add("@empno", SqlDbType.Int, 50).Value = idno
        sqlCmd.Connection.Open()

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()
        If sqlreader.HasRows Then
            If sqlreader.Read() Then

                If sqlreader("balance").Equals(Nothing) Then
                    result = "0.00"
                Else
                    result = Format(Val(sqlreader("balance").ToString), "###0.00")
                End If

            End If

        Else
            result = 0.0
        End If
        sqlCmd.Connection.Close()
        ' get the prev bal
        get_prevbal = result

    End Function

    Private Function randomcode() As String

        Dim result As String
        Dim elements As String = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim rand As New Random
        Dim l As Integer

        l = elements.Length

        Dim sb As New StringBuilder

        For i As Integer = 1 To 3
            Dim idx As Integer = rand.Next(0, l)
            sb.Append(elements.Substring(idx, 1))
        Next
        result = sb.ToString

        randomcode = result
    End Function


    Private Sub ToggleCheckState(ByVal checkState As Boolean)
        ' Iterate through the Products.Rows property
        For Each row As GridViewRow In SearchTable.Rows
            ' Access the CheckBox
            Dim cb As CheckBox = row.FindControl("cbSelect")
            If cb IsNot Nothing Then
                cb.Checked = checkState
            End If
        Next
    End Sub


    Protected Sub SearchTable_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchTable.SelectedIndexChanged

    End Sub

    Protected Sub SearchTable_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles SearchTable.RowCommand

        Dim IntIndex As Integer = Convert.ToInt16(e.CommandArgument.ToString)

        If e.CommandName = "btnpayment" Then

            Session("paymentempid") = SearchTable.DataKeys(IntIndex).Values(0).ToString
            Response.Redirect("ActualPayment.aspx")

        ElseIf e.CommandName = "btndownload" Then
            Dim selid As String = SearchTable.DataKeys(IntIndex).Values(0).ToString
            download_excelsheet(selid)

        End If

    End Sub
    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        'do nothing
    End Sub
    Private Shared Function UCFirst(ByVal value As String) As String
        ' Check for empty string.
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        End If
        ' Return char and concat substring.
        Return Char.ToUpper(value(0)) & value.Substring(1).ToLower
    End Function
    Private Sub download_excelsheet(ByVal selid As String)


        Dim sb As New StringBuilder
        Dim str As String
        Dim runTotal As Double = 0
        Dim sqlcmdlocal As SqlCommand
        Dim date1 As String
        Dim date2 As String

        date1 = startoffdate.Text.ToString
        date2 = cutoffdate.Text.ToString


        'table variable declartions
        Dim scellstr As String = "<Cell ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">"
        Dim scellint As String = "<Cell ss:StyleID=""Integer"">" & "<Data ss:Type=""Number"">"
        Dim scelldbl As String = "<Cell ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">"
        Dim ecell As String = "</Data></Cell>"
        'Build Statement
        'str = "SELECT     EmpNo, EmpLName, EmpFName, EmpMName, " & _
        '         "(SELECT     ISNULL(SUM(OTSUBSIDY), 0) AS Expr1 " & _
        '        " FROM (SELECT CASE WHEN OTFHRS < 2 THEN 0.0 WHEN OTFHRS < 10 THEN 40.0 WHEN OTFHRS < 18 THEN 80.0 ELSE 120 END AS OTSUBSIDY " & _
        '         " FROM   vwOvertime AS T1  WHERE  (OTFPay IS NULL) AND (EmpNo = cutoff.empno) AND (OTFDate >= @bdate) AND (OTFDate <= @sdate)) AS OT) " & _
        '         " AS OtSubsidy, (SELECT  ISNULL(COUNT(T2.EmpNo), 0) * 40 AS num_subsidy FROM vwAttendance AS T1 INNER JOIN" & _
        '         " vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo AND T1.AttDate >= @bdate AND " & _
        '         " T1.AttDate <= @sdate AND T2.STFActualDate <= @sdate AND T2.STFActualDate >= @bdate " & _
        '         " WHERE (T1.EmpNo = cutoff.EmpNo) AND (T2.HMFDate IS NULL) AND (T2.STFCanteenSub <> 1)) AS Subsidy," & _
        '         " (SELECT     ISNULL(SUM(subtotal), 0) AS total  FROM   (SELECT     t1.subtotal  FROM          tbl_transdetails AS t1 INNER JOIN " & _
        '         " tbl_transaction AS t2 ON t1.transno = t2.transno    WHERE      (t2.status = 1) AND (t2.idno = cutoff.EmpNo) AND (t2.transdate >= @bdate) AND (t2.transdate <= @sdate)) AS derivedtbl_1)  " & _
        '         " AS Payables FROM         vwEmployeeMaster AS cutoff  WHERE     (EmpNo = @txtbox )" & _
        '         " ORDER BY EmpLName "

        str = "Select * FROM tbl_logs WHERE empId = @txtbox ORDER BY recno DESC"

        sqlcmdlocal = New SqlCommand(str, con)
        sqlcmdlocal.Parameters.Add("@txtbox", SqlDbType.VarChar, 50).Value = selid
        sqlcmdlocal.Parameters.Add("@bdate", SqlDbType.DateTime, 50).Value = date1.ToString
        sqlcmdlocal.Parameters.Add("@sdate", SqlDbType.DateTime, 50).Value = date2.ToString




        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim adpt As New SqlDataAdapter(sqlcmdlocal)

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
        s.Append("<Cell ss:MergeAcross=""9"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        s.Append("<B>Dates From:" & date1 & " To: " & date2 & " </B> " & ecell)
        s.Append("</Row>")

        s.Append("<Row>")
        s.Append("<Cell ss:MergeAcross=""9"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        s.Append("<U> </U>" & ecell)
        s.Append("</Row>")

        s.Append("<Row>")
        Dim tcount As Integer = ds.Tables(0).Columns.Count
        For x As Integer = 0 To ds.Tables(0).Columns.Count - 1
            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append(UCFirst(ds.Tables(0).Columns(x).ColumnName))
            s.Append("</Data></Cell>")
        Next
        s.Append("</Row>")


        For Each dr As DataRow In ds.Tables(0).Rows
            s.Append("<Row>")
            s.Append(scellint & dr("recno").ToString & ecell)
            s.Append(scellstr & dr("transcode").ToString & ecell)
            s.Append(scellstr & dr("recdate").ToString & ecell)
            s.Append(scellstr & dr("rectime").ToString & ecell)
            s.Append(scellint & dr("empId").ToString & ecell)
            s.Append(scellstr & dr("incharge").ToString & ecell)
            s.Append(scellstr & dr("cutoffstartdate").ToString & ecell)
            s.Append(scellstr & dr("cutoffdate").ToString & ecell)
            s.Append(scelldbl & dr("charges").ToString & ecell)
            s.Append(scelldbl & dr("subsidy").ToString & ecell)
            s.Append(scelldbl & dr("ot_subsidy").ToString & ecell)
            s.Append(scelldbl & dr("subtotal").ToString & ecell)
            s.Append(scelldbl & dr("payments").ToString & ecell)
            s.Append(scelldbl & dr("balance").ToString & ecell)
            s.Append(scellstr & dr("remarks").ToString & ecell)
            s.Append("</Row>")
            runTotal = dr("balance").ToString
        Next

        'display the running total
        s.Append("<Row>")
        s.Append("<Cell ss:MergeAcross=""12"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        s.Append("Total: " & ecell)
        s.Append("<Cell  ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">")
        s.Append(runTotal.ToString & ecell)
        s.Append("</Row>")


        s.Append("</Table>")
        s.Append(" </Worksheet>")
        s.Append(endExcelXML)

        Response.Clear()

        Dim dashdate1 As String = date1.ToString
        Dim dashdate2 As String = date2.ToString

        dashdate1 = dashdate1.Replace("/", "-")
        dashdate2 = dashdate2.Replace("/", "-")

        Response.AddHeader("content-disposition", "attachment;filename=id_" & selid.ToString & "CutOffDate_from_" & dashdate1.ToString & "_to_" & dashdate2.ToString & "_" & Date.Now.TimeOfDay.ToString & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.xls"
        Response.ContentEncoding = System.Text.Encoding.Default
        Response.Write(s.ToString())
        Response.End()



    End Sub
    Function get_prevbal(ByVal eid As String, Optional ByVal sdate As String = "", Optional ByVal edate As String = "")

        Dim result As String = 0
        Dim balstr As String = "SELECT TOP (1) balance FROM  tbl_logs WHERE EmpId = @empno order by recno DESC"

        sqlCmd = New SqlCommand(balstr, con)
        sqlCmd.Parameters.Add("@empno", SqlDbType.Int, 50).Value = eid
        sqlCmd.Connection.Open()

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()
        If sqlreader.HasRows Then
            If sqlreader.Read() Then

                If sqlreader("balance").Equals(Nothing) Then
                    result = "0.00"
                Else
                    result = Val(sqlreader("balance")).ToString
                End If

            End If

        Else
            result = 0.0
        End If
        sqlCmd.Connection.Close()
        ' get the prev bal
        get_prevbal = result

    End Function

    Function get_cur_bal(ByVal eid As String, Optional ByVal sdate As String = "", Optional ByVal edate As String = "") As String
        con.Open()
        Dim thisdate As Date = Now
        Dim result As String = ""
        Dim sqlcmdlocal As SqlCommand
        Dim str As String = "SELECT  isnull(SUM(subtotal),0) AS total " + _
                " FROM " + _
                " (SELECT t1.subtotal FROM tbl_transdetails AS t1 INNER JOIN " + _
                "  tbl_transaction AS t2 ON t1.transno = t2.transno " + _
                " WHERE (t2.transdate  <= @tdate) " + _
                " AND (t2.status != @stat) AND (t2.idno = @idno)) AS T3"
        sqlcmdlocal = New SqlCommand(str, con)
        sqlcmdlocal.Parameters.Add("@tdate", SqlDbType.VarChar, 50).Value = thisdate.ToString("d")
        sqlcmdlocal.Parameters.Add("@stat", SqlDbType.VarChar, 50).Value = 2
        sqlcmdlocal.Parameters.Add("@idno", SqlDbType.VarChar, 50).Value = Session("IDNO")
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

    Function get_regular_subsidy(ByVal eid As String, Optional ByVal sdate As String = "", Optional ByVal edate As String = "") As String
        Dim result As String = ""
        Dim strSql As String
        Dim sqlcmdlocal As SqlCommand

        con.Open()
        strSql = "SELECT  isnull(COUNT(T2.empno),0) as num_subsidy  FROM vwAttendance AS T1 INNER JOIN  vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo WHERE (T1.empNo = @Empid) AND (T2.HMFDate IS NULL)  AND T2.STFCanteenSub != 1 "
        sqlcmdlocal = New SqlCommand(strSql, con)
        sqlcmdlocal.Parameters.Add("@Empid", SqlDbType.VarChar, 50).Value = cur_idno
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


    Function get_selids()
        Dim result As String = ""
        Dim comma As String = ""
        Dim atLeastOneRow As Boolean = False
        ' Iterate through the Products.Rows property
        For Each row As GridViewRow In SearchTable.Rows
            ' Access the CheckBox
            Dim cb As CheckBox = row.FindControl("cbSelect")
            If cb IsNot Nothing AndAlso cb.Checked Then
                atLeastOneRow = True
                Dim EmpNumber As Integer
                EmpNumber = Convert.ToInt32(SearchTable.DataKeys(row.RowIndex).Values(0))
                result += comma & "" & EmpNumber.ToString
                comma = ","
            Else
                'do nothing
            End If

        Next

        Return result
    End Function



    Private Sub exportalltoexcell()

        Dim ids As String = get_selids()

        Dim sb As New StringBuilder
        Dim str As String
        Dim runTotal As Double = 0
        Dim sqlcmdlocal As SqlCommand
        Dim date1 As String
        Dim date2 As String

        date1 = startoffdate.Text.ToString
        date2 = cutoffdate.Text.ToString


        'table variable declartions
        Dim scellstr As String = "<Cell ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">"
        Dim scellint As String = "<Cell ss:StyleID=""Integer"">" & "<Data ss:Type=""Number"">"
        Dim scelldbl As String = "<Cell ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">"
        Dim ecell As String = "</Data></Cell>"
        'Build Statement
        str = "SELECT     EmpNo, EmpLName, EmpFName, EmpMName, " & _
                "(SELECT     ISNULL(SUM(OTSUBSIDY), 0) AS Expr1 " & _
                " FROM (SELECT CASE WHEN OTFHRS < 2 THEN 0.0 WHEN OTFHRS < 10 THEN 40.0 WHEN OTFHRS < 18 THEN 80.0 ELSE 120 END AS OTSUBSIDY " & _
                " FROM   vwOvertime AS T1  WHERE  (OTFPay IS NULL) AND (EmpNo = cutoff.empno) AND (OTFDate >= @bdate) AND (OTFDate <= @sdate)) AS OT) " & _
                " AS OtSubsidy, (SELECT  ISNULL(COUNT(T2.EmpNo), 0) * 40 AS num_subsidy FROM vwAttendance AS T1 INNER JOIN" & _
                " vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo AND T1.AttDate >= @bdate AND " & _
                " T1.AttDate <= @sdate AND T2.STFActualDate <= @sdate AND T2.STFActualDate >= @bdate " & _
                " WHERE (T1.EmpNo = cutoff.EmpNo) AND (T2.HMFDate IS NULL) AND (T2.STFCanteenSub <> 1)) AS Subsidy," & _
                " (SELECT     ISNULL(SUM(subtotal), 0) AS total  FROM   (SELECT     t1.subtotal  FROM          tbl_transdetails AS t1 INNER JOIN " & _
                " tbl_transaction AS t2 ON t1.transno = t2.transno    WHERE      (t2.status = 1) AND (t2.idno = cutoff.EmpNo) AND (t2.transdate >= @bdate) AND (t2.transdate <= @sdate)) AS derivedtbl_1)  " & _
                " AS Payables FROM         vwEmployeeMaster AS cutoff " & _
                " ORDER BY EmpLName "
        ' WHERE     (EmpNo IN ( " & ids.ToString & "  ) )" 

        sqlcmdlocal = New SqlCommand(str, con)
        'sqlcmdlocal.Parameters.Add("@txtbox", SqlDbType.VarChar, 50).Value = ids.ToString
        sqlcmdlocal.Parameters.Add("@bdate", SqlDbType.DateTime).Value = date1.ToString
        sqlcmdlocal.Parameters.Add("@sdate", SqlDbType.DateTime).Value = date2.ToString




        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim adpt As New SqlDataAdapter(sqlcmdlocal)

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
        s.Append("<B>Dates From:" & date1 & " To: " & date2 & "  </B>" & ecell)
        s.Append("</Row>")
        s.Append("<Row>")
        s.Append("<Cell ss:MergeAcross=""9"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        s.Append("<U> </U>" & ecell)
        s.Append("</Row>")
        s.Append("<Row>")
        Dim tcount As Integer = ds.Tables(0).Columns.Count
        For x As Integer = 0 To ds.Tables(0).Columns.Count - 1
            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append(UCFirst(ds.Tables(0).Columns(x).ColumnName))
            s.Append("</Data></Cell>")
        Next
        s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
        s.Append("Prev Balance")
        s.Append("</Data></Cell>")
        s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
        s.Append("Subtotal")
        s.Append("</Data></Cell>")


        s.Append("</Row>")


        For Each dr As DataRow In ds.Tables(0).Rows
            s.Append("<Row>")
            s.Append(scellint & dr("Empno").ToString & ecell)
            s.Append(scellstr & dr("Emplname").ToString & ecell)
            s.Append(scellstr & dr("Empfname").ToString & ecell)
            s.Append(scellstr & dr("Empmname").ToString & ecell)
            s.Append(scelldbl & dr("Otsubsidy").ToString & ecell)
            s.Append(scelldbl & dr("Subsidy").ToString & ecell)
            s.Append(scelldbl & dr("Payables").ToString & ecell)

            Dim prevbal As String
            prevbal = get_prevbal(dr("Empno").ToString).ToString

            s.Append(scelldbl & prevbal & ecell)

            Dim subtotal As Double

            subtotal = (Val(dr("Payables")) + Val(prevbal)) - (Val(dr("Otsubsidy")) + Val(dr("Subsidy")))
            s.Append(scelldbl & subtotal.ToString & ecell)
            runTotal += Val(subtotal)

            s.Append("</Row>")
        Next

        'display the running total
        s.Append("<Row>")
        s.Append("<Cell ss:MergeAcross=""4"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        s.Append("Total: " & ecell)
        s.Append("<Cell  ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">")
        s.Append(runTotal.ToString & ecell)
        s.Append("</Row>")


        s.Append("</Table>")
        s.Append(" </Worksheet>")
        s.Append(endExcelXML)

        Response.Clear()
        Dim dashdate1 As String = date1.ToString
        Dim dashdate2 As String = date2.ToString

        dashdate1 = dashdate1.Replace("/", "-")
        dashdate2 = dashdate2.Replace("/", "-")


        Response.AddHeader("content-disposition", "attachment;filename=AllCutOffDate_from_" & dashdate1.ToString & "_to_" & dashdate2.ToString & "_" & Date.Now.TimeOfDay.ToString & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.xls"
        Response.ContentEncoding = System.Text.Encoding.Default
        Response.Write(s.ToString())
        Response.End()






    End Sub




    Protected Sub exportall_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles exportall.Click
        exportalltoexcell()
    End Sub

    Protected Sub exportselected_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles exportselected.Click
        Dim ids As String = get_selids()
        Panel2.Visible = False
        Panel1.Visible = True
        If ids <> "" Then
            Dim sb As New StringBuilder
            Dim str As String
            Dim runTotal As Double = 0
            Dim sqlcmdlocal As SqlCommand
            Dim date1 As String
            Dim date2 As String

            date1 = startoffdate.Text.ToString
            date2 = cutoffdate.Text.ToString


            'table variable declartions
            Dim scellstr As String = "<Cell ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">"
            Dim scellint As String = "<Cell ss:StyleID=""Integer"">" & "<Data ss:Type=""Number"">"
            Dim scelldbl As String = "<Cell ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">"
            Dim ecell As String = "</Data></Cell>"
            'Build Statement
            str = "SELECT     EmpNo, EmpLName, EmpFName, EmpMName, " & _
                    "(SELECT     ISNULL(SUM(OTSUBSIDY), 0) AS Expr1 " & _
                    " FROM (SELECT CASE WHEN OTFHRS < 2 THEN 0.0 WHEN OTFHRS < 10 THEN 40.0 WHEN OTFHRS < 18 THEN 80.0 ELSE 120 END AS OTSUBSIDY " & _
                    " FROM   vwOvertime AS T1  WHERE  (OTFPay IS NULL) AND (EmpNo = cutoff.empno) AND (OTFDate >= @bdate) AND (OTFDate <= @sdate)) AS OT) " & _
                    " AS OtSubsidy, (SELECT  ISNULL(COUNT(T2.EmpNo), 0) * 40 AS num_subsidy FROM vwAttendance AS T1 INNER JOIN" & _
                    " vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo AND T1.AttDate >= @bdate AND " & _
                    " T1.AttDate <= @sdate AND T2.STFActualDate <= @sdate AND T2.STFActualDate >= @bdate " & _
                    " WHERE (T1.EmpNo = cutoff.EmpNo) AND (T2.HMFDate IS NULL) AND (T2.STFCanteenSub <> 1)) AS Subsidy," & _
                    " (SELECT     ISNULL(SUM(subtotal), 0) AS total  FROM   (SELECT     t1.subtotal  FROM          tbl_transdetails AS t1 INNER JOIN " & _
                    " tbl_transaction AS t2 ON t1.transno = t2.transno    WHERE      (t2.status = 1) AND (t2.idno = cutoff.EmpNo) AND (t2.transdate >= @bdate) AND (t2.transdate <= @sdate)) AS derivedtbl_1)  " & _
                    " AS Payables FROM         vwEmployeeMaster AS cutoff WHERE     (EmpNo IN ( " & ids.ToString & "  ) )" & _
                    " ORDER BY EmpLName "

            sqlcmdlocal = New SqlCommand(str, con)
            'sqlcmdlocal.Parameters.Add("@txtbox", SqlDbType.VarChar, 50).Value = ids.ToString
            sqlcmdlocal.Parameters.Add("@bdate", SqlDbType.DateTime).Value = date1.ToString
            sqlcmdlocal.Parameters.Add("@sdate", SqlDbType.DateTime).Value = date2.ToString




            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim adpt As New SqlDataAdapter(sqlcmdlocal)

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
            s.Append("<Cell ss:MergeAcross=""8"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
            s.Append("<B>Dates From:" & date1 & " To: " & date2 & "  </B>" & ecell)
            s.Append("</Row>")
            s.Append("<Row>")
            s.Append("<Cell ss:MergeAcross=""9"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
            s.Append("<U> </U>" & ecell)
            s.Append("</Row>")
            s.Append("<Row>")
            Dim tcount As Integer = ds.Tables(0).Columns.Count
            For x As Integer = 0 To ds.Tables(0).Columns.Count - 1
                s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
                s.Append(UCFirst(ds.Tables(0).Columns(x).ColumnName))
                s.Append("</Data></Cell>")
            Next
            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Prev Balance")
            s.Append("</Data></Cell>")
            s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
            s.Append("Subtotal")
            s.Append("</Data></Cell>")


            s.Append("</Row>")


            For Each dr As DataRow In ds.Tables(0).Rows
                s.Append("<Row>")
                s.Append(scellint & dr("Empno").ToString & ecell)
                s.Append(scellstr & dr("Emplname").ToString & ecell)
                s.Append(scellstr & dr("Empfname").ToString & ecell)
                s.Append(scellstr & dr("Empmname").ToString & ecell)
                s.Append(scelldbl & dr("Otsubsidy").ToString & ecell)
                s.Append(scelldbl & dr("Subsidy").ToString & ecell)
                s.Append(scelldbl & dr("Payables").ToString & ecell)

                Dim prevbal As String
                prevbal = get_prevbal(dr("Empno").ToString).ToString

                s.Append(scelldbl & prevbal & ecell)

                Dim subtotal As Double

                subtotal = (Val(dr("Payables")) + Val(prevbal)) - (Val(dr("Otsubsidy")) + Val(dr("Subsidy")))
                s.Append(scelldbl & subtotal.ToString & ecell)
                runTotal += Val(subtotal)

                s.Append("</Row>")
            Next

            'display the running total
            s.Append("<Row>")
            s.Append("<Cell ss:MergeAcross=""4"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
            s.Append("Total: " & ecell)
            s.Append("<Cell  ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">")
            s.Append(runTotal.ToString & ecell)
            s.Append("</Row>")


            s.Append("</Table>")
            s.Append(" </Worksheet>")
            s.Append(endExcelXML)

            Response.Clear()
            Dim dashdate1 As String = date1.ToString
            Dim dashdate2 As String = date2.ToString

            dashdate1 = dashdate1.Replace("/", "-")
            dashdate2 = dashdate2.Replace("/", "-")


            Response.AddHeader("content-disposition", "attachment;filename=ids_" & ids.ToString & "CutOffDate_from_" & dashdate1.ToString & "_to_" & dashdate2.ToString & "_" & Date.Now.TimeOfDay.ToString & ".xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.xls"
            Response.ContentEncoding = System.Text.Encoding.Default
            Response.Write(s.ToString())
            Response.End()

        Else
            'no ids selected
            Panel1.Visible = False
            Panel2.Visible = True
        End If


    End Sub

    Protected Sub AllCheck_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles AllCheck.Click
        ToggleCheckState(True)
    End Sub

    Protected Sub AllUncheck_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles AllUncheck.Click
        ToggleCheckState(False)
    End Sub
End Class
