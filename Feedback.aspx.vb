Imports System.Data
Imports System.Data.SqlClient
Imports System.Net.Mail
Partial Class Feedback
    Inherits System.Web.UI.Page
    Dim cur_idno As String
    Dim cur_user As String

    Dim con_str As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(con_str)
    Dim sqlCmd As SqlCommand

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        cur_idno = Session("IDNO")
        cur_user = Session("UserName")

        If cur_idno = 0 Then
            Response.Redirect("Login.aspx")

        Else
             
            Dim btnlink As LinkButton

            btnlink = CType(Master.FindControl("btnfeedback"), LinkButton)

            If Not btnlink Is Nothing Then
                btnlink.CssClass = "current"

            End If
        End If


    End Sub

    Protected Sub btnsubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsubmit.Click

        If Trim(feedentry.Text) <> "" Then
            Dim sql As String = "INSERT INTO [tbl_feedback] (Empno,Feedback,Status,Subtimedate) VALUES (@empno, @entry, @status,@subtimedate)"
            con.Open()
            sqlCmd = New SqlCommand(sql, con)
            sqlCmd.Parameters.Add("@empno", SqlDbType.VarChar, 50).Value = Session("IDNO")
            sqlCmd.Parameters.Add("@entry", SqlDbType.Text).Value = feedentry.Text.ToString
            sqlCmd.Parameters.Add("@status", SqlDbType.SmallInt).Value = 0
            sqlCmd.Parameters.Add("@subtimedate", SqlDbType.VarChar, 50).Value = Date.Now.ToString("yyyy-MM-dd hh:mm:ss")
            sqlCmd.ExecuteNonQuery()
            con.Close()
            AUTO_EMAIL(feedentry.Text.ToString)
            feedentry.Text = ""
            Panel1.Visible = True
            Panel2.Visible = False

        Else
            Panel1.Visible = False
            Panel2.Visible = True
        End If



    End Sub
    Public Sub AUTO_EMAIL(ByVal feedback As String)

        Dim strFrom As String = "no-reply@ccs.kao-phil.com"

        Dim strTo As String = "mgcosico@kao-phil.com"

        ' Dim strAttachment As String = "C:\parent screen.jpg,C:\Print Screen.jpg"
        Dim strSubject As String = "CCS feedback form submission."
        Dim strBody As String = feedback
        Dim IsBodyIsInHTML As Boolean = False


        Try
            'Dim smtp As New SmtpClient("sa9fi011.kao-phil.com")
			Dim smtp As New SmtpClient("mailhub1.sdd.kao.co.jp")
            Dim mssg As New MailMessage()
            mssg.From = New MailAddress(strFrom)
            mssg.Subject = strSubject
            mssg.Body = strBody
            mssg.To.Add(New MailAddress(strTo))

            mssg.Bcc.Add(New MailAddress("mcdagus@kao-phil.com"))
            mssg.Bcc.Add(New MailAddress("mgdomo@kao-phil.com"))
            mssg.Bcc.Add(New MailAddress("paranopa@kao-phil.com"))


            'Adding multiple To Addresses
            'For Each sTo As String In strTo.Split(",".ToCharArray())
            '    mssg.[To].Add(sTo)
            'Next

            mssg.Priority = MailPriority.Normal

            mssg.BodyEncoding = System.Text.Encoding.[Default]
            mssg.SubjectEncoding = System.Text.Encoding.[Default]
            mssg.IsBodyHtml = IsBodyIsInHTML
            smtp.Send(mssg)
            'trach the exception and write in the log. might be Main Addresses contains non mail id formats.
        Catch ex As Exception
        End Try


    End Sub
End Class
