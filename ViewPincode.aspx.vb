Imports System.Data.Sql
Imports System.Data.SqlClient
Imports System.Web.Services 
Imports System.Xml
Imports System.Web.Services.Protocols
Imports System.Web.Script.Services
Partial Class ViewPincode
    Inherits System.Web.UI.Page
    Dim st_conn As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(st_conn)
    Dim sqlCmd As SqlCommand
    Dim cur_user As String
    Dim cur_idno As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        cur_idno = Session("IDNO")
        cur_user = Session("UserName")
        
        If cur_idno = 0 Then
            Response.Redirect("Login.aspx")
        Else
            If Not Page.IsPostBack Then
            End If
        End If
        mypincode.Text = get_pincode(cur_idno)

        Dim msg As Integer

        msg = Request.QueryString("msg")
        If msg = 1 Then
            Panel1.Visible = True
        Else
            Panel1.Visible = False

        End If


        Dim btnlink As LinkButton

        btnlink = CType(Me.Master.FindControl("btnpincode"), LinkButton)

        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If


    End Sub
   
    Private Function get_pincode(ByVal empid As String) As String
        Dim result As String = "0"

        Dim strsql As String
        con.Open()

        strsql = "SELECT pincode FROM tbl_pincode WHERE empno = @id"
        sqlCmd = New SqlCommand(strsql, con)
        sqlCmd.Parameters.Add("@id", Data.SqlDbType.VarChar, 50).Value = empid
        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()
        If sqlreader.HasRows Then

            While sqlreader.Read()
                result = sqlreader("pincode").ToString
            End While

        End If


        con.Close()
        Return result

    End Function
    Private Function generateCode() As String
        Dim result As String = ""
        Dim s As String = "0123456789"
        Dim r As New Random
        Dim sb As New StringBuilder
        For i As Integer = 1 To 8
            Dim idx As Integer = r.Next(0, 9)
            sb.Append(s.Substring(idx, 1))
        Next
        result = sb.ToString

        Return result
    End Function

    
    Protected Sub btnpincode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnpincode.Click
        Dim pcode As String = ""
        pcode = generateCode()
        pcode = changepin.Text

        Dim pinstr As String
        pinstr = "UPDATE tbl_pincode SET pincode = @pcode  WHERE EmpNo=@empid"
        sqlCmd = New SqlCommand(pinstr, con)
        sqlCmd.Parameters.Add("@pcode", Data.SqlDbType.VarChar, 6).Value = pcode
        sqlCmd.Parameters.Add("@empid", Data.SqlDbType.VarChar, 10).Value = cur_idno
        sqlCmd.Connection.Open()

        Try
            sqlCmd.ExecuteNonQuery()
        Catch ex As SqlException
            MsgBox(ex.Message)
        End Try
        sqlCmd.Connection.Close()
        Response.Redirect("ViewPincode.aspx?msg=1")
    End Sub
End Class
