Imports System.Data
Imports System.Data.SqlClient

Partial Class ViewOTsubsidy
    Inherits System.Web.UI.Page
    Dim cur_transid As String
    Dim sel_date As String
    Dim cur_name As String
    Dim cur_id As String
    Dim cur_idno As Integer
    Dim st_conn As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(st_conn)
    Dim sqlCmd As SqlCommand
    Dim ds As New DataSet()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("IDNO") = 0 Then

            Response.Redirect("Login.aspx")

        End If

        cur_transid = Session("transno")
        sel_date = Session("Seldate")
        cur_name = Session("fullname")
        cur_id = Session("IDNO")
        cur_idno = cur_id
        If Not Page.IsPostBack Then
            transno.Text = cur_id
            lblname.Text = cur_name
            lbltotal.Text = get_total()
            set_payrolldates()
        Else

            set_sqldata()
        End If

        Dim btnlink As LinkButton
        btnlink = CType(Me.Master.FindControl("btnotsub"), LinkButton)
        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If
    End Sub

    Private Sub set_sqldata()

        SqlDataSource1.SelectCommand = "SELECT  TOP 100 OTFDate, SUM(OTFHrs) AS OTFhrs, CASE WHEN SUM(OTFHrs) < 2 THEN 0.0 WHEN SUM(OTFHRS) < 10 THEN 40.0 WHEN SUM(OTFHRS) < 18 THEN 80.0 ELSE 120 END AS OTFSUB, CASE WHEN  OTCanteenSub = 0 THEN 'Available' ELSE 'Comsumed' END AS otfstatus , prolldate as pdate FROM  vwOTSub WHERE (EmpNo = @empID) AND prolldate = @pdate   GROUP BY OTFDate, prolldate, OTCanteenSub ORDER BY OTFDATE DESC"
        GridView1.DataBind()

    End Sub

    Private Sub set_payrolldates()

        Dim thisyear As String = Date.Now.Year
        Try
            Dim strSql As String
            con.Open()
            strSql = "SELECT DISTINCT payrolldate FROM tbl_logs WHERE void = 0  ORDER BY payrolldate DESC"
            sqlCmd = New SqlCommand(strSql, con)
            sqlCmd.Parameters.Add("@y", SqlDbType.VarChar, 50).Value = thisyear
            Dim sqlreader As SqlDataReader
            sqlreader = sqlCmd.ExecuteReader()
            payrolldate.Items.Clear()
            'payrolldate.Items.Add("")
            If sqlreader.HasRows Then
                While sqlreader.Read()
                    payrolldate.Items.Add(sqlreader.Item("payrolldate"))
                End While

            End If
            con.Close()
        Catch e As DataException

        End Try


    End Sub

    Private Function get_total()
        Dim result As String = ""
        Dim strSql As String
        con.Open()
        'strSql = "SELECT isnull(SUM(OT.OTSUBSIDY),0.0) as ot_sub FROM (SELECT OTFHRS, CASE WHEN OTFHRS  < 2 THEN 0.0 WHEN OTFHRS < 10 THEN 40.0 WHEN OTFHRS < 18 THEN 80.0 ELSE 120 END as OTSUBSIDY FROM vwOvertime as T1 WHERE t1.Empno = @Empid AND t1.OTFPay is NULL) as OT"
        strSql = "SELECT isnull(SUM(OT.OTSUBSIDY),0) as ot_sub FROM (SELECT   T2.subsidy as OTSUBSIDY FROM vwOvertime as T1 INNER JOIN  tbl_OT as T2 ON T1.OTFID = T2.OTFID WHERE t1.Empno = @Empid AND t2.OTCanteenSUb = 0) as OT"
        strSql = "SELECT isnull(SUM(OTFSUB),0) AS ot_sub FROM (SELECT SUM(isnull(OTFHrs,0)) AS OTFhrs, CASE WHEN SUM(OTFHrs) < 2 THEN 0.0 WHEN SUM(OTFHRS) < 10 THEN 40.0 WHEN SUM(OTFHRS) < 18 THEN 80.0 ELSE 120 END AS OTFSUB FROM vwOTSub WHERE (  EmpNo = @empID)  AND  prolldate is null AND OTFDATE > '4/15/2014' GROUP BY OTFDate, prolldate, OTCanteenSub) AS OTSUB"
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
        get_total = result
    End Function

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        'Dim sb As New StringBuilder
        'Dim str As String
        'Dim searchsum As String
        ''Build Statement
        'con.Open()
        'str = "SELECT TOP 100 T1.OTFPay, T1.OTFDate, T1.OTFSTime, T1.OTFETime, T1.OTFHrs, T2.subsidy as otfsub ,  CASE WHEN T2.OTCanteenSub = 0 THEN 'Available' ELSE 'Comsumed' END AS otfstatus, t2.prolldate as pdate     FROM vwOvertime as T1 INNER JOIN tbl_OT as T2 ON T1.OTFID = T2.OTFID WHERE (t1.EmpNo = @empID)      AND t2.prolldate = @pdate ORDER BY T1.OTFDate DESC"

        ''err_msg.Text = check_date()
        'sqlCmd = New SqlCommand(str, con)
        'sqlCmd.Parameters.Add("@empID", SqlDbType.VarChar, 50).Value = cur_idno
        'sqlCmd.Parameters.Add("@pdate", SqlDbType.NVarChar).Value = payrolldate.SelectedValue.ToString


        ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Dim adpt As New SqlDataAdapter(sqlCmd)
        'ds.Clear()
        'adpt.Fill(ds)
        
            GridView1.DataBind()
      


        'con.Close()


    End Sub

    Protected Sub clearbutt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles clearbutt.Click
        Response.Redirect("viewOTsubsidy.aspx")
    End Sub
End Class
