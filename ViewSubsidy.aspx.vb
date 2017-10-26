Imports System.Data
Imports System.Data.SqlClient

Partial Class ViewSubsidy
    Inherits System.Web.UI.Page
    Dim cur_transid As String
    Dim sel_date As String
    Dim cur_name As String
    Dim cur_id As String
    Dim cur_idno As Integer
    Dim st_conn As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(st_conn)
    Dim sqlCmd As SqlCommand
    Dim sqlCmd2 As SqlCommand

    Dim ds As New DataSet()
 

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("IDNO") = 0 Then

            Response.Redirect("Login.aspx")

        End If

        cur_transid = Session("transno")
        cur_name = Session("fullname")
        cur_id = Session("IDNO")
        cur_idno = cur_id
        If Not Page.IsPostBack Then

            transno.Text = cur_id
            lblname.Text = cur_name
            lbltotal.Text = Format(Val(get_regular_subsidy() * 40), "###0.00").ToString
            Session("sdate") = "1/1/2014"
            Session("edate") = "12/31/9999"

               
            Dim day As String
            Dim tm As String = Date.Now.Month
            Dim ty As String = Date.Now.Year
            Dim eom As String = GetLastDayOfMonth(Date.Now.Month.ToString, Date.Now.Year.ToString)

                txt_s_transdate.text = tm & "/1/" & ty 
                txt_e_transdate.text = eom
        
            day = Date.Now.Day
            If Val(day) <= 15 Then
                payrolldate.SelectedValue = tm & "/15/" & ty & " 12:00:00 AM"
            Else
                payrolldate.SelectedValue = eom & " 12:00:00 AM"
            End If
        Else

            'payrolldate.SelectedValue = Session("pdate")



        End If

        Dim btnlink As LinkButton
        btnlink = CType(Me.Master.FindControl("btnviewsub"), LinkButton)
        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If
        '
    End Sub
    Public Function GetLastDayOfMonth(ByVal intMonth As String, ByVal intYear As String) As Date

        GetLastDayOfMonth = DateSerial(intYear, intMonth + 1, 0)

    End Function
    Private Sub set_payrolldates()

        Dim thisyear As String = Date.Now.Year
        Try
            Dim strSql As String
            con.Open()
            strSql = "SELECT * FROM vwPayrollCalendar WHERE Year = @y"
            sqlCmd = New SqlCommand(strSql, con)
            sqlCmd.Parameters.Add("@y", SqlDbType.VarChar, 50).Value = thisyear
            Dim sqlreader As SqlDataReader
            sqlreader = sqlCmd.ExecuteReader()
            payrolldate.Items.Clear()
            payrolldate.Items.Add("")
            If sqlreader.HasRows Then
                While sqlreader.Read()
                    payrolldate.Items.Add(sqlreader.Item("payrolldate"))

                End While

            End If
            con.Close()
        Catch e As DataException

        End Try


    End Sub
    Private Function get_payrolldates()

        Dim year As String = Date.Now.Year.ToString
        Dim dpayrolldate As String
        Dim dt(2) As String

        dpayrolldate = payrolldate.SelectedValue.ToString


        Try

            Dim sSQLText As String = "SELECT startdate , enddate" & _
                " FROM vwPayrollCalendar  WHERE payrolldate = @payrolldate AND Year = @year"

            Using dbConnection As New SqlConnection(st_conn)
                dbConnection.Open()
                Dim dbReader As SqlDataReader
                Dim dbCommand As New SqlCommand(sSQLText, dbConnection)
                dbCommand.Parameters.Add("@payrolldate", SqlDbType.VarChar, 50).Value = dpayrolldate
                dbCommand.Parameters.Add("@year", SqlDbType.VarChar, 5).Value = year

                dbReader = dbCommand.ExecuteReader
                If dbReader.HasRows Then
                    Dim bal As String = 0
                    While dbReader.Read
                        dt(0) = dbReader.Item("startdate")
                        dt(1) = dbReader.Item("enddate")
                        'MsgBox(dt(0).ToString & " " & dt(1).ToString)
                    End While

                End If

            End Using
        Catch ex As Exception
            'MessageBox.Show(ex.Message.ToString, "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblname.Text = ex.Message
        End Try
        Session("sdate") = dt(0)
        Session("edate") = dt(1)

        Return dt

    End Function

    Function get_regular_subsidy() As String
        Dim result As String = ""
        Dim strSql As String
        con.Open()
        strSql = "SELECT  isnull(COUNT(T2.empno),0.0) as num_subsidy  FROM (SELECT DISTINCT AttDate, EmpNo FROM vwAttendance) AS T1 INNER JOIN  vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo WHERE (T1.empNo = @Empid) AND (T2.HMFDate IS NULL) AND STFShift NOT IN ('0', '-') AND t2.STFCanteenSub != 1 "
        sqlCmd = New SqlCommand(strSql, con)
        sqlCmd.Parameters.Add("@Empid", SqlDbType.VarChar, 50).Value = cur_idno
        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.HasRows Then
            If sqlreader.Read() Then
                If sqlreader(0).Equals(Nothing) Then
                    result = 0.0
                Else
                    result = sqlreader(0).ToString 'Format(Val(sqlreader(0).ToString), "###0.0")

                End If
            End If
        Else
            result = 0.0
        End If
        con.Close()

        get_regular_subsidy = result

    End Function

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        Session("pdate") = payrolldate.SelectedItem.Text.ToString
        SearchSubsidy()
    End Sub
    Private Sub SearchSubsidy()
        Dim sb As New StringBuilder
        'Dim str As String
        Dim searchsum As String
        Dim dtpayrolls(2) As String
        'Build Statement
        dtpayrolls = get_payrolldates()

        'str = "SELECT T1.EmpNo, T1.AttDate as attdate, T1.StartTime as starttime, T1.EndTime as endtime, T2.STFShift, T2.HMFDate, 40.0 as Subsidy , CASE WHEN  T2.STFCanteenSub != 1 THEN  'Available' ELSE 'Consumed' END  AS Status FROM vwAttendance AS T1 INNER JOIN vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo WHERE (T1.EmpNo = @empid) AND (T2.HMFDate IS NULL) AND T2.STFActualDate >= @startdate AND T2.STFActualDate <= @enddate ORDER BY T1.AttDate DESC"

        ''err_msg.Text = check_date()
        'sqlCmd = New SqlCommand(str, con)
        'sqlCmd.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = cur_idno
        'sqlCmd.Parameters.Add("@startdate", SqlDbType.VarChar, 50).Value =
         'txt_e_transdate.text <dtpayrolls(0)
        'sqlCmd.Parameters.Add("@enddate", SqlDbType.VarChar, 50).Value = dtpayrolls(1)

        ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Dim adpt As New SqlDataAdapter(sqlCmd)

        ' ds.Clear()

        'adpt.Fill(ds)

        'SearchTable.DataSource = ds
        GridView1.DataBind()
        'lblrecords.Text = SearchTable.Rows.Count

        con.Open()
        'err_msg.Text = check_date()
        searchsum = "SELECT  isnull(COUNT(T2.empno),0.0) as num_subsidy  FROM (SELECT DISTINCT AttDate, EmpNo FROM vwAttendance) AS T1 INNER JOIN  vwShifting AS T2 ON T1.AttDate = T2.STFActualDate AND T1.EmpNo = T2.EmpNo WHERE (T1.empNo = @Empid) AND (T2.HMFDate IS NULL) AND STFShift NOT IN ('0', '-') AND T1.AttDate >= @startdate AND T1.AttDate <= @enddate  "
        sqlCmd2 = New SqlCommand(searchsum, con)
        sqlCmd2.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = cur_idno
'sqlCmd2.Parameters.Add("@startdate", SqlDbType.VarChar, 50).Value = dtpayrolls(0)
'sqlCmd2.Parameters.Add("@enddate", SqlDbType.VarChar, 50).Value = dtpayrolls(1)
sqlCmd2.Parameters.Add("@startdate", SqlDbType.VarChar, 50).Value =  txt_s_transdate.text
sqlCmd2.Parameters.Add("@enddate", SqlDbType.VarChar, 50).Value = txt_e_transdate.text


        Dim sqldreader As SqlDataReader = sqlCmd2.ExecuteReader()
        If sqldreader.HasRows Then
            If sqldreader.Read() Then
                stotal.Text = Format(Val(sqldreader("num_subsidy")) * 40, "##0.00").ToString
            
            End If


        End If
        con.Close()
        lblcovered.Text = "Covered Dates From:<br /> " & txt_s_transdate.text & " To: " & txt_e_transdate.text
        btnclear.Visible = True
    End Sub

    Protected Sub btnclear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnclear.Click
        Response.Redirect("ViewSubsidy.aspx")
    End Sub
End Class
