Imports System.Data
Imports System.Data.SqlClient

Partial Class ActualPayment
    Inherits System.Web.UI.Page
    Dim st_conn As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(st_conn)
    Dim sqlCmd As SqlCommand 
    Dim ds As New DataSet()
    Dim cur_selempid As String
	dim cur_user as string
	
	 Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim thisdate As Date = Now

        cur_selempid = Session("paymentempid")

        cur_user = Session("UserName")

        If cur_selempid = 0 Then

            Response.Redirect("SearchCutOff.aspx")

        Else
            Dim empfullname As String
            empfullname = empdetails(cur_selempid)
            If Not Page.IsPostBack Then
                lblidname.Text = cur_selempid
                lblfullname.Text = empfullname.ToString
            End If
        End If

        'highlight current menu page
        Dim btnlink As LinkButton

        btnlink = CType(Me.Master.FindControl("btncutoff"), LinkButton)

        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If
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
    Private Sub 	update_payment(ByVal empno As String, ByVal r As String)
        Dim tcode As String
        Dim incharge As String
        incharge = Session("IDNO")
        Dim prevbal As Double
        prevbal = get_prevbal(empno)
		
        'create transaction   code for logs 
        'tcode = Hex(Val(Format(Now, Val(incharge)))) & "-" & Hex(Val(Format(Now, "MMddyyyyhhmmsstt")))
        tcode = Hex(Val(Format(Now, "MMddyyhhmmssffff")))
		'record logs
        Dim recstr As String
        recstr = "INSERT INTO tbl_logs " & _
                 " ([transcode],[recdate],[rectime],[empid],[incharge], " & _
                "  [charges], [subsidy], [ot_subsidy],[subtotal] ," & _
                 " [payments],[balance],[remarks]) " & _
                 " VALUES " & _
                 " ( @tcode , @recdate , @rectime, @empid, @incharge, " & _
                 " @charges, @subsidy, @ot_subsdiy,@subtotal," & _
                 " @payments , @balance,  @remarks)"

        sqlCmd = New SqlCommand(recstr, con)

        sqlCmd.Parameters.Add("@tcode", SqlDbType.VarChar, 32).Value = tcode
        sqlCmd.Parameters.Add("@recdate", SqlDbType.DateTime).Value = Now.ToString("MM/dd/yyyy hh:mm:ss tt")
        sqlCmd.Parameters.Add("@rectime", SqlDbType.VarChar, 50).Value = Now.ToString("T")
        sqlCmd.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = empno
        sqlCmd.Parameters.Add("@incharge", SqlDbType.VarChar, 50).Value = incharge
        sqlCmd.Parameters.Add("@charges", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@subsidy", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@ot_subsdiy", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@subtotal", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@payments", SqlDbType.Decimal).Value = payment.Text.ToString
        sqlCmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = prevbal - Convert.ToDouble(payment.Text.ToString)
        sqlCmd.Parameters.Add("@remarks", SqlDbType.Text).Value = "Canteen Input"

        sqlCmd.Connection.Open()


        Try
            sqlCmd.ExecuteNonQuery()
        Catch ex As SqlException
            If ex.Number = 2627 Then
                '
            Else
                '
            End If
			lblidname.text = ex.message
        End Try
		
        sqlCmd.Connection.Close()

    End Sub

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
	Protected Sub btnsubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsubmit.Click

        If Val(payment.Text) = 0 Then
            Panel2.Visible = True
            Panel1.Visible = False
            Page.SetFocus("payment")

            payment.BorderColor = Drawing.Color.Red
            Me.SetFocus("payment")

        Else


            update_payment(cur_selempid, randomcode())
            Panel2.Visible = False
            Panel1.Visible = True
            uptext.Text = " " & empdetails(cur_selempid) & " with the amount of " & payment.Text.ToString & " Php"
            payment.BorderColor = Drawing.Color.Empty
            payment.Text = ""
            Page.SetFocus("payment")
            Me.SetFocus("payment")
        End If

        GridView1.DataBind()


    End Sub

    Private Function empdetails(ByVal empno As String) As String
        Dim result As String = ""
        Dim strsql As String
        con.Open()
        strsql = "SELECT * FROM vwEmployeeMaster  WHERE EmpNo = @empid"
        sqlCmd = New SqlCommand(strsql, con)
        sqlCmd.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = empno

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.HasRows Then
            If sqlreader.Read() Then
                result = sqlreader("Emplname").ToString & " " & sqlreader("EmpFname").ToString & " " & sqlreader("EmpMname").ToString
            End If

        Else
            result = ""
        End If

        con.Close()

        Return result

    End Function
    
End Class
