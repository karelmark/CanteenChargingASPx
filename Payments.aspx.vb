Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization


Partial Class Payments
    Inherits System.Web.UI.Page
    Dim st_conn As String = "Data Source=SA9FI013;Initial Catalog=ccs;User ID=ccs_connect;Password=ccs"
    Dim con As New Data.SqlClient.SqlConnection(st_conn)
    Dim sqlCmd As SqlCommand
    Dim ds As New DataSet()
    Dim cur_selempid As String
    Dim cur_user As String
  
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            searchtxt.Text = " "
        End If
        If Not IsPostBack Then
            searhpayables.DataBind()
        End If


    End Sub
    Private Sub btnUpdatePayments_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdatePayments.Click
        Dim needstoupdate As Boolean
        Dim empstr As String = ""
        Dim comma As String = ""
        Dim tcode As String
        tcode = Hex(Val(Format(Now, "yyMMddhhmmssffff")))

        For Each row As GridViewRow In searhpayables.Rows

            Dim tb As TextBox = row.FindControl("txtpayment")
            Dim EmpNumber As Integer
            Dim payment As Double

            payment = Val(tb.Text)

            EmpNumber = Convert.ToInt32(searhpayables.DataKeys(row.RowIndex).Value)
            empstr = empstr.ToString & tb.Text.ToString

            If tb IsNot Nothing And payment <> 0 Then

                needstoupdate = True
                empstr = empstr & comma & "" & EmpNumber
                comma = ","
                update_payment(EmpNumber, payment, tcode)
            Else

                'do nothing

            End If

        Next
        lblmsg.Text = empstr
    End Sub

    Private Function checkvalusearenumbers() As Boolean

        Dim EmpNumber As Integer
        Dim payment As Double


        For Each row As GridViewRow In searhpayables.Rows

            Dim tb As TextBox = row.FindControl("txtpayment")

            payment = Val(tb.Text)
            EmpNumber = Convert.ToInt32(searhpayables.DataKeys(row.RowIndex).Value)

            If tb IsNot Nothing Then

                If Not IsNumeric(tb.Text.ToString) Then
 
                End If

            Else

                'do nothing

            End If

        Next


        Return True

    End Function

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

    Private Sub update_payment(ByVal empno As String, ByVal payment As Double, ByVal r As String)

        Dim incharge As String
        incharge = Session("IDNO")
        Dim prevbal As Double
        prevbal = get_prevbal(empno)

        'create transaction   code for logs 
        'tcode = Hex(Val(Format(Now, Val(incharge)))) & "-" & Hex(Val(Format(Now, "MMddyyyyhhmmsstt")))
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

        sqlCmd.Parameters.Add("@tcode", SqlDbType.VarChar, 32).Value = r
        sqlCmd.Parameters.Add("@recdate", SqlDbType.DateTime).Value = Now.ToString("MM/dd/yyyy hh:mm:ss tt")
        sqlCmd.Parameters.Add("@rectime", SqlDbType.VarChar, 50).Value = Now.ToString("T")
        sqlCmd.Parameters.Add("@empid", SqlDbType.VarChar, 50).Value = empno
        sqlCmd.Parameters.Add("@incharge", SqlDbType.VarChar, 50).Value = incharge
        sqlCmd.Parameters.Add("@charges", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@subsidy", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@ot_subsdiy", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@subtotal", SqlDbType.Decimal).Value = 0
        sqlCmd.Parameters.Add("@payments", SqlDbType.Decimal).Value = payment
        sqlCmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = prevbal - Convert.ToDouble(payment)
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
            lblmsg.Text = ex.Message
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

   

    Protected Sub btnprntreport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnprntreport.Click
        Dim sb As New StringBuilder
   


       

        Dim filen As String
        filen = "" & DateAndTime.Now.Millisecond.ToString & ".xls"
        Dim s As New StringBuilder()
        Const startExcelXML As String = "<xml version>" & vbCr & vbLf & "<Workbook " & "xmlns=""urn:schemas-microsoft-com:office:spreadsheet""" & vbCr & vbLf & " xmlns:o=""urn:schemas-microsoft-com:office:office""" & vbCr & vbLf & " " & "xmlns:x=""urn:schemas-    microsoft-com:office:" & "excel""" & vbCr & vbLf & " xmlns:ss=""urn:schemas-microsoft-com:" & "office:spreadsheet"">" &  _
            vbCr & vbLf & " <Styles>" & vbCr & vbLf & " " & "<Style ss:ID=""Default"" ss:Name=""Normal"">" & vbCr & vbLf & " " & "<Alignment ss:Vertical=""Bottom""/>" & vbCr & vbLf & " <Borders/>" & vbCr & vbLf & " <Font/>" & vbCr & vbLf & " <Interior/>" & vbCr & vbLf & " <NumberFormat/>" & vbCr & vbLf & " <Protection/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "<Style ss:ID=""BoldColumn"">" & vbCr & vbLf & " <Font " & "x:Family=""Swiss"" ss:Bold=""1""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "<Style     ss:ID=""StringLiteral"">" & vbCr & vbLf & " <NumberFormat" & " ss:Format=""@""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " <Style " & "ss:ID=""Decimal"">" & vbCr & vbLf & " <NumberFormat " & "ss:Format=""0.0000""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "<Style ss:ID=""Integer"">" & vbCr & vbLf & " <NumberFormat " & "ss:Format=""0""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " <Style " & "ss:ID=""DateLiteral"">" & vbCr & vbLf & " <NumberFormat " & "ss:Format=""mm/dd/yyyy;@""/>" & vbCr & vbLf & " </Style>" & vbCr & vbLf & " " & "</Styles>" & vbCr & vbLf & " "
        '       Const styleexcel As String = "<Styles>" & vbCr & vbLf & "<Style ss:ID=""Default"" ss:Name=""Normal"">" & vbCr & vbLf & "<Alignment ss:Vertical=""Bottom""/>" & vbCr & vbLf & "<Borders/>" & vbCr & vbLf & "<Font/>" & vbCr & vbLf & " <Interior/>" & vbCr & vbLf & "   <NumberFormat/>" & vbCr & vbLf & " <Protection/>" & vbCr & vbLf & "  </Style>" & vbCr & vbLf & " " & " <Style ss:ID=""s61"">" & vbCr & vbLf & "<Font ss:FontName=""Courier"" x:Family=""Modern""/>" & vbCr & vbLf & "</Style>" & vbCr & vbLf & "<Style ss:ID=""s62"">"
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Size="12" ss:Bold="1"/>
        ' </Style>
        ' <Style ss:ID="s63">
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Font ss:FontName="Courier" x:Family="Modern"/>
        ' </Style>
        ' <Style ss:ID="s64">
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern"/>
        ' </Style>
        ' <Style ss:ID="s65">
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '   <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern"/>
        ' </Style>
        ' <Style ss:ID="s66">
        '  <Alignment ss:Vertical="Bottom"/>
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '   <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Bold="1"/>
        ' </Style>
        ' <Style ss:ID="s67">
        '  <Alignment ss:Vertical="Bottom"/>
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Bold="1"/>
        ' </Style>
        ' <Style ss:ID="s68">
        '  <Alignment ss:Vertical="Bottom"/>
        '  <Borders/>
        '  <Font ss:FontName="Courier" x:Family="Modern"/>
        ' </Style>
        ' <Style ss:ID="s69">
        '  <Alignment ss:Vertical="Bottom"/>
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern"/>
        ' </Style>
        ' <Style ss:ID="s70">
        '  <Borders/>
        '  <Font ss:FontName="Courier" x:Family="Modern"/>
        ' </Style>
        ' <Style ss:ID="s71">
        '  <Alignment ss:Horizontal="Left" ss:Vertical="Bottom"/>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Size="12"/>
        ' </Style>
        ' <Style ss:ID="s72">
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Size="12" ss:Bold="1"/>
        ' </Style>
        ' <Style ss:ID="s73">
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Size="12"/>
        ' </Style>
        ' <Style ss:ID="s74">
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Size="12"/>
        ' </Style>
        ' <Style ss:ID="s75">
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Size="12"/>
        ' </Style>
        ' <Style ss:ID="s76">
        '  <Borders>
        '   <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
        '   <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
        '  </Borders>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Size="12"/>
        ' </Style>
        ' <Style ss:ID="s77">
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Bold="1"/>
        ' </Style>
        ' <Style ss:ID="s78">
        '  <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
        '  <Font ss:FontName="Courier" x:Family="Modern" ss:Bold="1"/>
        ' </Style>
        '</Styles>"
        'Const endExcelXML As String = "</Workbook>"
        's.Append(startExcelXML)
        's.Append("<Worksheet ss:Name='Sheet1'>")
        's.Append("<Table>")
        's.Append("<Row>")
        's.Append("<Cell ss:MergeAcross=""8"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        's.Append("<B>Dates From:" & date1 & " To: " & date2 & "  </B>" & ecell)
        's.Append("</Row>")
        's.Append("<Row>")
        's.Append("<Cell ss:MergeAcross=""9"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        's.Append("<U> </U>" & ecell)
        's.Append("</Row>")
        's.Append("<Row>")
        'Dim tcount As Integer = ds.Tables(0).Columns.Count
        'For x As Integer = 0 To ds.Tables(0).Columns.Count - 1
        '    s.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
        '    s.Append(UCFirst(ds.Tables(0).Columns(x).ColumnName))
        '    s.Append("</Data></Cell>")
        'Next
        's.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
        's.Append("Prev Balance")
        's.Append("</Data></Cell>")
        's.Append("<Cell ss:StyleID=""BoldColumn""><Data ss:Type=""String"">")
        's.Append("Subtotal")
        's.Append("</Data></Cell>")


        's.Append("</Row>")


        'For Each dr As DataRow In ds.Tables(0).Rows
        '    s.Append("<Row>")
        '    s.Append(scellint & dr("Empno").ToString & ecell)
        '    s.Append(scellstr & dr("Emplname").ToString & ecell)
        '    s.Append(scellstr & dr("Empfname").ToString & ecell)
        '    s.Append(scellstr & dr("Empmname").ToString & ecell)
        '    s.Append(scelldbl & dr("Otsubsidy").ToString & ecell)
        '    s.Append(scelldbl & dr("Subsidy").ToString & ecell)
        '    s.Append(scelldbl & dr("Payables").ToString & ecell)

        '    Dim prevbal As String
        '    prevbal = get_prevbal(dr("Empno").ToString).ToString

        '    s.Append(scelldbl & prevbal & ecell)

        '    Dim subtotal As Double

        '    subtotal = (Val(dr("Payables")) + Val(prevbal)) - (Val(dr("Otsubsidy")) + Val(dr("Subsidy")))
        '    s.Append(scelldbl & subtotal.ToString & ecell)
        '    runTotal += Val(subtotal)

        '    s.Append("</Row>")
        'Next

        ''display the running total
        's.Append("<Row>")
        's.Append("<Cell ss:MergeAcross=""4"" ss:StyleID=""StringLiteral"">" & "<Data ss:Type=""String"">")
        's.Append("Total: " & ecell)
        's.Append("<Cell  ss:StyleID=""Decimal"">" & "<Data ss:Type=""Number"">")
        's.Append(runTotal.ToString & ecell)
        's.Append("</Row>")


        's.Append("</Table>")
        's.Append(" </Worksheet>")
        's.Append(endExcelXML)

        'Response.Clear()
        'Dim dashdate1 As String = date1.ToString
        'Dim dashdate2 As String = date2.ToString

        'dashdate1 = dashdate1.Replace("/", "-")
        'dashdate2 = dashdate2.Replace("/", "-")


        'Response.AddHeader("content-disposition", "attachment;filename=ids_" & ids.ToString & "CutOffDate_from_" & dashdate1.ToString & "_to_" & dashdate2.ToString & "_" & Date.Now.TimeOfDay.ToString & ".xls")
        'Response.Charset = ""
        'Response.ContentType = "application/vnd.xls"
        'Response.ContentEncoding = System.Text.Encoding.Default
        'Response.Write(s.ToString())
        'Response.End()
    End Sub
End Class
