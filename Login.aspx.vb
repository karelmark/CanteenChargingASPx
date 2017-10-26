Imports System.Data
Imports System.Data.SqlClient

Partial Class Login
    Inherits System.Web.UI.Page

    Dim str_con As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(str_con)
     Dim sqlCmd As SqlCommand
    Dim sqlReader As SqlDataReader
    Dim cur_id As String = 0

    Protected Sub btnlogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnlogin.Click
        Dim str_user As String = username.Text.ToString
        Dim str_pass As String = password.Text.ToString

        If str_user.Length > 0 And str_pass.ToString.Length > 0 And str_user.Substring(0) <> " " Then
            con.Open()
            Dim str As String = "SELECT * FROM vwEmployeeMaster WHERE " + _
            " Empno= @username " + _
            " AND Pwd= @pass "


            sqlCmd = New SqlCommand(str, con)
            sqlCmd.Parameters.Add("@username", System.Data.SqlDbType.NVarChar).Value = str_user
            sqlCmd.Parameters.Add("@pass", System.Data.SqlDbType.NVarChar).Value = str_pass
            sqlCmd.CommandTimeout = 240
            sqlReader = sqlCmd.ExecuteReader()
            
            If sqlReader.HasRows.Equals(True) Then

                If sqlReader.Read Then

                    Session("IDNO") = sqlReader("Empno").ToString
                    Session("UserName") = sqlReader("Empno").ToString

                End If
                Response.Redirect("Default.aspx")
            End If
            errmsg.Text = "Invalid User"

            username.Text = str_user.ToString
            password.Text = str_pass.ToString

            con.Dispose()
            con.Close()

        Else
            errmsg.Text = "Invalid Login!"

        End If



    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Not Page.IsPostBack Then
        '    If Session("IDNO") <> 0 Then
        '        Response.Redirect("Default.aspx")
        '    End If
        'End If


    End Sub

    Protected Sub btncantenlog_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btncantenlog.Click
        Dim str_user As String = canusername.Text.ToString
        Dim str_pass As String = canpassword.Text.ToString

        If str_user.Length > 0 And str_pass.ToString.Length > 0 And str_user.Substring(0) <> " " Then
            con.Open()
            Dim str As String = "SELECT * FROM tbl_login WHERE " + _
            " username= @username " + _
            " AND password= @pass "


            sqlCmd = New SqlCommand(str, con)
            sqlCmd.Parameters.Add("@username", System.Data.SqlDbType.VarChar, 50).Value = str_user
            sqlCmd.Parameters.Add("@pass", System.Data.SqlDbType.NVarChar).Value = str_pass
            sqlReader = sqlCmd.ExecuteReader()

            If sqlReader.HasRows.Equals(True) Then

                If sqlReader.Read Then

                    Session("IDNO") = sqlReader("idno").ToString
                    Session("UserName") = sqlReader("username").ToString

                End If
                Response.Redirect("Payments.aspx")
            End If
            errmsg.Text = "Invalid User"

            username.Text = str_user.ToString
            password.Text = str_pass.ToString

            con.Dispose()
            con.Close()

        Else
            errmsg.Text = "Invalid Login!"

        End If



    End Sub
End Class
