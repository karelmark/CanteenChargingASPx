Imports System.Data
Imports System.Data.SqlClient

Partial Class Site
    Inherits System.Web.UI.MasterPage
    Dim str As String = ""
    Dim con_str As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(con_str)
   
     
    Protected Sub btnlogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnlogout.Click
        Session.Clear()
        'Session.Abandon()
        'Session.RemoveAll()
        Response.Redirect("Login.aspx")
    End Sub

    Protected Sub btnhome_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnhome.Click
        Response.Redirect("Default.aspx")
    End Sub

    Protected Sub btnviewsub_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnviewsub.Click
        Response.Redirect("ViewSubsidy.aspx")
    End Sub

    Protected Sub btnotsub_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnotsub.Click
        Response.Redirect("ViewOTsubsidy.aspx")
    End Sub

    Protected Sub btnfeedback_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnfeedback.Click
        Response.Redirect("Feedback.aspx")
    End Sub

   

    Protected Sub btnviewprevbal_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnviewprevbal.Click
        Response.Redirect("ViewPrevBalance.aspx")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim int As Integer = Val(Session("IDNO"))

        If Session("IDNO") = 0 Then
            Response.Redirect("Login.aspx")
    
        End If
    End Sub

    Protected Sub btnpincode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnpincode.Click
        Response.Redirect("ViewPincode.aspx")
    End Sub

  
    Protected Sub btnothersub_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnothersub.Click
        Response.Redirect("ViewOtherSubsidy.aspx")
    End Sub
End Class

