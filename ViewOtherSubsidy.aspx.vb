Imports System.Data
Imports System.Data.SqlClient

Partial Class ViewOtherSubsidy
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("IDNO") = 0 Then

            Response.Redirect("Login.aspx")

        End If

        cur_transid = Session("transno")
        cur_name = Session("fullname")
        cur_id = Session("IDNO")
        cur_idno = cur_id

        Dim btnlink As LinkButton
        btnlink = CType(Me.Master.FindControl("btnothersub"), LinkButton)
        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If
    End Sub
End Class
