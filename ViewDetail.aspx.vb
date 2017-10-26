Imports System.Data
Imports System.Data.SqlClient

Partial Class ViewDetail
    Inherits System.Web.UI.Page
    Dim cur_transid As String
    Dim sel_date As String
    Dim cur_name As String
    Dim cur_id As String

    Dim con_str As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(con_str)
    Dim sqlCmd As SqlCommand
    dim dt(2) as String	
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        If Session("IDNO") = 0 Then

            Response.Redirect("Login.aspx")

        End If

        cur_transid = Session("transno")
        sel_date = Session("Seldate")
        cur_name = Session("fullname")
        cur_id = Session("IDNO")

        If Not Page.IsPostBack Then
            transno.Text = cur_transid
            lblname.Text = cur_name
            dt  =  get_transdate()
			datecovered.Text = dt(0)
			timecovered.Text = dt(1)
			


        End If

        Dim sql_str As String = "SELECT t1.itemno as itemno ,t1.itemcode as itemcode,t1.qty as qty ,t1.unitcode as unitcode,t1.price as price,t1.subtotal as subtotal, t2.itemname as itemname  FROM tbl_transdetails as t1 , tbl_inventory as t2 WHERE " + _
                                " transno = @transno and t1.itemcode = t2.recno   AND status ='OK' ORDER BY itemno"
        con.Open()
        sqlCmd = New SqlCommand(sql_str, con)
        sqlCmd.Parameters.Add("@transno", SqlDbType.VarChar, 50).Value = cur_transid

        Dim adpt As New SqlDataAdapter(sqlCmd)
        Dim ds As New DataSet()

        adpt.Fill(ds)

        GridView1.DataSource = ds
        GridView1.DataBind()

        sql_str = "SELECT txtotal as total FROM tbl_transaction WHERE transno = @transno  "
        Dim sqlcmdtotal As SqlCommand
        sqlcmdtotal = New SqlCommand(sql_str, con)
        sqlcmdtotal.Parameters.Add("@transno", SqlDbType.VarChar, 50).Value = cur_transid

        Dim sqlReader As SqlDataReader
        sqlReader = sqlcmdtotal.ExecuteReader()

        If sqlReader.Read Then
            lbltest.Text = "Total: " & Format(val(sqlReader("total")), "###0.00") & " Php"
        Else
            lbltest.Text = "Total: 0.00 Php"
        End If
        con.Close()
		
		
		   Dim btnlink As LinkButton

        btnlink = CType(Me.Master.FindControl("btnhome"), LinkButton)
         
        If Not btnlink Is Nothing Then
            btnlink.CssClass = "current"
        End If
    End Sub


   

    Function get_transdate()  

        Dim result(2) As String 
        Dim sql_str As String = "SELECT transdate,transtime FROM tbl_transaction WHERE transno = @tid"
        con.Open()
        sqlCmd = New SqlCommand(sql_str, con)
        sqlCmd.Parameters.Add("@tid", SqlDbType.Int).Value = cur_transid

        Dim sqlreader As SqlDataReader
        sqlreader = sqlCmd.ExecuteReader()

        If sqlreader.Read Then

            result(0) = sqlreader("transdate")
            result(1) = sqlreader("transtime")

        End If
        con.Close()
        get_transdate = result
    End Function
   
 
End Class