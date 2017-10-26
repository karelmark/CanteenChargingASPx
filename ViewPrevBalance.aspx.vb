Imports System.Data
Imports System.Data.SqlClient

Partial Class ViewPrevBalance
    Inherits System.Web.UI.Page
    Dim st_conn As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(st_conn)
    Dim sqlCmd As SqlCommand
    Dim sqlCmd2 As SqlCommand
    Dim cur_idno As Integer
    Dim cur_user As String
    Dim ds As New DataSet()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim thisDate As Date = Now
        Dim past15days As DateTime = DateTime.Today.AddDays(-15)
       
        ' If Session("IDNO") Is Nothing Then
        cur_idno = Session("IDNO")
        cur_user = Session("UserName")

        If cur_idno = 0 Then

            Response.Redirect("Login.aspx")

        Else

            Empid.Text = cur_idno
            fullnam.Text = get_empdetails(cur_idno)
            If Not Page.IsPostBack Then
                'startdate.Text = "01/01/2014" 'past15days.ToString("d")
                'enddate.Text = thisDate.ToString("d")
                prevcutoffdates.text = lastcutoff(cur_idno)
                curprevbal.Text = get_prevbal(cur_idno)

                GridView1.DataBind()
            End If

        End If

        Dim btnlink As LinkButton
        btnlink = CType(Me.Master.FindControl("btnviewprevbal"), LinkButton)
        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If


        'Dim ipaddress As String
        'ipaddress = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")

        'If ipaddress <> "10.30.10.116" Then
        '    Response.Redirect("Default.aspx")
        'End If
    End Sub

    Private Function get_empdetails(ByVal eid As String) As String
        Dim result As String = ""
        Dim strsql As String
        con.Open()
        strsql = "SELECT * FROM vwEmployeeMaster  WHERE EmpNo = @empid"
        sqlCmd = New SqlCommand(strsql, con)
        sqlCmd.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = eid

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.HasRows Then
            If sqlreader.Read() Then
                result = sqlreader("Emplname").ToString & ", " & sqlreader("EmpFname").ToString & " " & sqlreader("EmpMname").ToString.Substring(0)
            End If

        Else
            result = ""
        End If

        con.Close()

        Return result

    End Function


    Private Function get_prevbal(ByVal idno As Integer)
        Dim result As String = 0
        Dim balstr As String = "SELECT TOP (1) empbalance FROM  tbl_logs WHERE EmpId = @empno AND void = 0 order by recno DESC"

        sqlCmd = New SqlCommand(balstr, con)
        sqlCmd.Parameters.Add("@empno", SqlDbType.Int, 50).Value = idno
        sqlCmd.Connection.Open()

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()
        If sqlreader.HasRows Then
            If sqlreader.Read() Then

                If sqlreader("empbalance").Equals(Nothing) Then
                    result = "0.00"
                Else
                    result = Format(Val(sqlreader("empbalance").ToString), "###0.00")
                End If

            End If

        Else
            result = 0.0
        End If
        sqlCmd.Connection.Close()
        ' get the prev bal
        get_prevbal = result

    End Function
    Private Function lastcutoff(ByVal idno As Integer)

        Dim result As String = "No Records from the Last Cutoff Date"
        Dim balstr As String = "SELECT TOP (1) balance, CAST(cutoffstartdate AS varchar(11)) AS cutoffstartdate, CAST(cutoffdate AS varchar(11)) AS cutoffdate FROM  tbl_logs WHERE EmpId = @empno AND void = 0 order by recno DESC"
        'AND cutoffstartdate &gt;= @sdate AND cutoffdate &lt;= @edate
        sqlCmd = New SqlCommand(balstr, con)
        sqlCmd.Parameters.Add("@empno", SqlDbType.Int, 50).Value = idno
        sqlCmd.Connection.Open()

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()
        If sqlreader.HasRows Then
            If sqlreader.Read() Then

                If sqlreader("balance").Equals(Nothing) Then
                    result = "No Records from the Last Cutoff Date"
                Else
                    result = "Last Cutoff Date: " & sqlreader("cutoffdate").ToString


                End If

            End If

        Else
            result = "No Records from the Last Cutoff Date"
        End If
        sqlCmd.Connection.Close()
        ' get the prev bal
        Return result

    End Function






    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView1.RowCommand
       
        If e.CommandName = "btnshowDetail" Then
            Dim IntIndex As Integer = Convert.ToInt16(e.CommandArgument.ToString)
            Dim txnumber As String
            txnumber = GridView1.DataKeys(IntIndex).Value.ToString()
            Response.Redirect("ViewPrevBalance.aspx?transno=" & txnumber)
        End If

    End Sub
End Class
