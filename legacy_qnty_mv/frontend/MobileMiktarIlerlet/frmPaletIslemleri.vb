Imports System.Data

Public Class frmPaletIslemleri
    Inherits System.Windows.Forms.Form
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents txtPaletNo As System.Windows.Forms.TextBox
    Friend WithEvents lblEtiketNo As System.Windows.Forms.Label
    Friend WithEvents txtKoliNo As System.Windows.Forms.TextBox
    Friend WithEvents lblPaletNo As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnCreatePalet As System.Windows.Forms.Button
    Friend WithEvents btnTemizle As System.Windows.Forms.Button
    Friend WithEvents btnKoliSil As System.Windows.Forms.Button

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents dtgrdMain As System.Windows.Forms.DataGrid
    Friend WithEvents listNo As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents tbSevk As System.Windows.Forms.DataGridTableStyle
    Friend WithEvents PaletNo As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents SiraNo As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Malzeme As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Adet As System.Windows.Forms.DataGridTextBoxColumn
    'Friend WithEvents EtiketNo As System.Windows.Forms.DataGridTextBoxColumn
    'Friend WithEvents MalzemeNo As System.Windows.Forms.DataGridTextBoxColumn
    'Friend WithEvents miktar As System.Windows.Forms.DataGridTextBoxColumn
    'Friend WithEvents paletNo As System.Windows.Forms.DataGridTextBoxColumn
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSevkiyat))
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.btnTemizle = New System.Windows.Forms.Button
        Me.txtPaletNo = New System.Windows.Forms.TextBox
        Me.lblEtiketNo = New System.Windows.Forms.Label
        Me.txtKoliNo = New System.Windows.Forms.TextBox
        Me.lblPaletNo = New System.Windows.Forms.Label
        Me.btnCreatePalet = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.btnKoliSil = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.dtgrdMain = New System.Windows.Forms.DataGrid
        Me.tbSevk = New System.Windows.Forms.DataGridTableStyle
        Me.PaletNo = New System.Windows.Forms.DataGridTextBoxColumn
        Me.SiraNo = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Malzeme = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Adet = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnTemizle)
        Me.Panel1.Controls.Add(Me.txtPaletNo)
        Me.Panel1.Controls.Add(Me.lblEtiketNo)
        Me.Panel1.Controls.Add(Me.txtKoliNo)
        Me.Panel1.Controls.Add(Me.lblPaletNo)
        Me.Panel1.Controls.Add(Me.btnCreatePalet)
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(240, 56)
        '
        'btnTemizle
        '
        Me.btnTemizle.Location = New System.Drawing.Point(155, 28)
        Me.btnTemizle.Name = "btnTemizle"
        Me.btnTemizle.Size = New System.Drawing.Size(83, 21)
        Me.btnTemizle.TabIndex = 7
        Me.btnTemizle.Text = "Temizle"
        '
        'txtPaletNo
        '
        Me.txtPaletNo.Location = New System.Drawing.Point(53, 8)
        Me.txtPaletNo.MaxLength = 30
        Me.txtPaletNo.Name = "txtPaletNo"
        Me.txtPaletNo.Size = New System.Drawing.Size(96, 21)
        Me.txtPaletNo.TabIndex = 0
        '
        'lblEtiketNo
        '
        Me.lblEtiketNo.Location = New System.Drawing.Point(3, 28)
        Me.lblEtiketNo.Name = "lblEtiketNo"
        Me.lblEtiketNo.Size = New System.Drawing.Size(44, 20)
        Me.lblEtiketNo.Text = "Koli No"
        '
        'txtKoliNo
        '
        Me.txtKoliNo.Location = New System.Drawing.Point(53, 28)
        Me.txtKoliNo.Name = "txtKoliNo"
        Me.txtKoliNo.Size = New System.Drawing.Size(96, 21)
        Me.txtKoliNo.TabIndex = 2
        '
        'lblPaletNo
        '
        Me.lblPaletNo.Location = New System.Drawing.Point(3, 8)
        Me.lblPaletNo.Name = "lblPaletNo"
        Me.lblPaletNo.Size = New System.Drawing.Size(52, 20)
        Me.lblPaletNo.Text = "Palet No"
        '
        'btnCreatePalet
        '
        Me.btnCreatePalet.Location = New System.Drawing.Point(155, 8)
        Me.btnCreatePalet.Name = "btnCreatePalet"
        Me.btnCreatePalet.Size = New System.Drawing.Size(83, 21)
        Me.btnCreatePalet.TabIndex = 6
        Me.btnCreatePalet.Text = "Palet Aç"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(168, 190)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(72, 20)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Kapat"
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.btnKoliSil)
        Me.Panel3.Controls.Add(Me.btnDelete)
        Me.Panel3.Controls.Add(Me.dtgrdMain)
        Me.Panel3.Controls.Add(Me.btnCancel)
        Me.Panel3.Location = New System.Drawing.Point(0, 55)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(240, 213)
        '
        'btnKoliSil
        '
        Me.btnKoliSil.Location = New System.Drawing.Point(0, 189)
        Me.btnKoliSil.Name = "btnKoliSil"
        Me.btnKoliSil.Size = New System.Drawing.Size(52, 20)
        Me.btnKoliSil.TabIndex = 5
        Me.btnKoliSil.Text = "Koli Sil"
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(113, 190)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(58, 20)
        Me.btnDelete.TabIndex = 0
        Me.btnDelete.Text = "Palet Sil"
        '
        'dtgrdMain
        '
        Me.dtgrdMain.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.dtgrdMain.Location = New System.Drawing.Point(0, 0)
        Me.dtgrdMain.Name = "dtgrdMain"
        Me.dtgrdMain.Size = New System.Drawing.Size(240, 184)
        Me.dtgrdMain.TabIndex = 3
        Me.dtgrdMain.TableStyles.Add(Me.tbSevk)
        '
        'tbSevk
        '
        Me.tbSevk.GridColumnStyles.Add(Me.PaletNo)
        Me.tbSevk.GridColumnStyles.Add(Me.SiraNo)
        Me.tbSevk.GridColumnStyles.Add(Me.Malzeme)
        Me.tbSevk.GridColumnStyles.Add(Me.Adet)
        Me.tbSevk.MappingName = "Sevk"
        '
        'PaletNo
        '
        Me.PaletNo.Format = ""
        Me.PaletNo.FormatInfo = Nothing
        Me.PaletNo.HeaderText = "Palet No"
        Me.PaletNo.MappingName = "paletNo"
        Me.PaletNo.NullText = ""
        Me.PaletNo.Width = 60
        '
        'SiraNo
        '
        Me.SiraNo.Format = ""
        Me.SiraNo.FormatInfo = Nothing
        Me.SiraNo.HeaderText = "Sira No"
        Me.SiraNo.MappingName = "SiraNo"
        Me.SiraNo.NullText = ""
        Me.SiraNo.Width = 65
        '
        'Malzeme
        '
        Me.Malzeme.Format = ""
        Me.Malzeme.FormatInfo = Nothing
        Me.Malzeme.HeaderText = "Malzeme"
        Me.Malzeme.MappingName = "Malzeme"
        Me.Malzeme.NullText = ""
        Me.Malzeme.Width = 85
        '
        'Adet
        '
        Me.Adet.Format = ""
        Me.Adet.FormatInfo = Nothing
        Me.Adet.HeaderText = "Adet"
        Me.Adet.MappingName = "Adet"
        Me.Adet.NullText = ""
        Me.Adet.Width = 30
        '
        'frmSevkiyat
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ClientSize = New System.Drawing.Size(240, 271)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmSevkiyat"
        Me.Text = "Palet Islemleri"
        Me.Panel1.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    'Dim Sevk As New WS_Sevk.srvSevkiyat
    Dim RetVal As New ReturnValue
    Dim srv As New Mobile.wsGeneral
    Private ds As DataSet
    Private palet As Boolean
    Private Surum As String = "1.0.0.1"
    Private etkBilg As EtiketBilgisi
    Private Qty As Decimal
    Private sL3p As String = ""
    Private QtyMax As Integer = 0
    Public Sorgu As String
    Dim dt As New DataTable

    Dim item As String
    Dim dtEtiketDty As DataTable
    Dim dtLotLoc As DataTable
    Dim sumBoxQty As Integer
    Dim loc As String
    Dim pickNo As String
    Dim dtUm As DataTable
    'Dim ayniEtiketVar As Boolean = False
    'Dim etiketler As String = String.Empty
    'Dim Malzeme As String
    Dim MalzemeMiktari As String
    '
#Region " Methods "
    '
    Private Sub CreateTable()
        '
        ds = Nothing
        Dim dt As New DataTable("Sevk")
        '
        'dt.Columns.Add("listeNo")
        'dt.Columns.Add("paletNo")
        ''dt.Columns.Add("malzemeNo")
        'dt.Columns.Add("newCreated")
        ''dt.Columns.Add("lot")
        'dt.Columns.Add("Item")
        'dt.Columns.Add("Miktar")
        'dt.Columns.Add("PLTTYPE")


        dt.Columns.Add("PaletNo")
        dt.Columns.Add("SiraNo")
        dt.Columns.Add("Malzeme")
        dt.Columns.Add("Adet")
        dt.Columns.Add("newCreated")
        'dt.Columns.Add("PLTTYPE")
        '
        ds = New DataSet
        ds.Tables.Add(dt)
        ''
    End Sub
    '
    'Private Function GetBarkod() As Boolean

    '    Try

    '        If txtPaletNo.Text.Trim <> "" Then

    '            etkBilg = New EtiketBilgisi

    '            If txtListeNo.Text = "" Then

    '                MsgBox("Çekme No Tanýmsýz", MsgBoxStyle.Exclamation, "Uyarý")

    '                txtPaletNo.Text = ""

    '                Return False

    '            End If

    '            'RetVal = CheckEtiketNo(txtListeNo.Text, txtEtiketNo.Text, etkBilg)
    '            '
    '            If etkBilg.Pkmik = 0 Or etkBilg.Pkod = "" Then

    '                MsgBox("Bu ürün paletlenmiyor..", MsgBoxStyle.Exclamation, "Uyarý")

    '                txtPaletNo.Text = ""

    '                Return False

    '            End If


    '            If etkBilg.Pkmik = ds.Tables("Sevk").Select("paletNo = '1'").Length() Then

    '                MsgBox("Bu Palet Dolmuþ Durumda Lütfen Palet i Kapatýp Yeni Palet Açýnýz..", MsgBoxStyle.Exclamation, "Uyarý")

    '                txtPaletNo.Text = ""

    '                Return False

    '            End If
    '            '
    '            If RetVal.ReturnValue = False Then

    '                MsgBox("Etiket Tanýmsýz", MsgBoxStyle.Exclamation, "Uyarý")

    '                txtPaletNo.Text = ""

    '                Return False

    '            End If
    '            '
    '            '
    '            'L3P Karþýlaþtýrmasý 
    '            If sL3p = "" Then sL3p = etkBilg.L3P

    '            If sL3p.Trim <> etkBilg.L3P.Trim Then

    '                MsgBox("Kutu bu palete eklenemez ! ", MsgBoxStyle.Exclamation, "Uyarý")

    '                txtPaletNo.Text = ""

    '                Return False

    '            End If
    '            '
    '            Return True
    '        End If
    '        '
    '    Catch ex As Exception
    '        'Clear
    '        MsgBox(ex.Message, MsgBoxStyle.Exclamation)
    '        '
    '        Return False
    '    End Try
    '    ''
    'End Function
    '
    '
    'Private Sub Add()

    '    Cursor.Current = Cursors.WaitCursor
    '    '
    '    Try

    '        Dim dr As DataRow
    '        'Dim db As New Sevkiyat
    '        '
    '        dr = ds.Tables("Sevk").NewRow
    '        '
    '        dr("listeNo") = txtListeNo.Text
    '        dr("paletNo") = txtKoliNo.Text
    '        'dr("etiketNo") = txtEtiketNo.Text
    '        dr("Item") = etkBilg.Itnbr
    '        dr("House") = etkBilg.House
    '        dr("newCreated") = 0
    '        dr("PLTTYPE") = "MONO"

    '        '
    '        ds.Tables("Sevk").Rows.Add(dr)

    '        bindData()

    '        etkBilg = Nothing
    '        '
    '        txtPaletNo.Text = ""
    '        txtPaletNo.Focus()

    '        ds.Tables("Sevk").DefaultView.Sort = "Item desc"

    '    Catch ex As Exception

    '        MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")

    '    End Try
    '    '
    '    Cursor.Current = Cursors.Default
    'End Sub
    '
    Private Sub bindData()
        '
        With dtgrdMain
            .DataSource = ds.Tables(0)
            '
        End With
        '
        'txtMiktar.Text = (ds.Tables("Sevk").Rows.Count)
    End Sub
    '
    Private Sub SendData(Optional ByVal bKapat As Boolean = False)

        Try

            If Not (ds Is Nothing) And _
            (ds.Tables(0).Rows.Count >= 1) Then
                '

                Dim ds1 As New DataSet

                ds1 = ds.Clone

                ds1.Tables("Sevk").Clear()

                dt = SelectDistinct(ds.Tables("Sevk"), "paletNo")
                '
                For Each row As DataRow In ds.Tables("Sevk").Rows

                    'PaletleriInsert(row("paletNo").ToString)
                    Sorgu = "INSERT INTO TR_PAKET VALUES(" & row("listeNo").ToString & ",'" & row("paletNo").ToString & "','" & _
                        row("malzeme").ToString & "'," & row("Miktar").ToString & ",0,0,'" & Date.Now.ToString("yyyy-MM-dd") & "',newid(),'sa','sa','" & _
                        Date.Now.ToString("yyyy-MM-dd") & "')"
                    srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)
                Next

                dt = ds.Tables("Sevk")


                Message("SendCompleted")
                '
                If bKapat Then

                    Me.Close()

                End If

            Else

                If Not bKapat Then

                    Message("MalzemeYok")

                Else

                    Me.Close()

                End If

            End If



        Catch ex As Exception
            '
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")
            '
        End Try

    End Sub



    Private Function Formclosing() As Boolean

        If Not ds Is Nothing Then
            '
            If palet Or _
                ds.Tables("Sevk").Rows.Count >= 1 Then
                '
                Dim a As MsgBoxResult

                a = Message("SaveQuestion")

                If a = MsgBoxResult.Yes Then
                    '
                    If palet Then
                        '
                        Qty = ds.Tables("Sevk").Select("paletNo = '1'").Length()
                        '
                        If Not (Qty = 0) Then

                            Dim sPaletNo As String
                            '
                            txtKoliNo.Text = SeriNoAl("ETK")

                            sPaletNo = txtKoliNo.Text
                            '
                            For Each dr As DataRow In ds.Tables("Sevk").Select("paletNo = '1'")
                                '
                                dr("paletNo") = txtKoliNo.Text

                                dr("newCreated") = 1
                                '
                            Next
                            '
                            bindData()
                            '
                            palet = False

                            txtKoliNo.Text = ""

                            btnCreatePalet.Text = "Palet Aç"

                        Else

                            If Message("PaletMalzemeYok") = MsgBoxResult.Yes Then

                                palet = False

                                txtKoliNo.Text = ""

                                btnCreatePalet.Text = "Palet Aç"

                            End If

                        End If

                    End If
                    '
                    SendData()
                    '
                ElseIf a = MsgBoxResult.No Then

                    Return True

                Else

                    Return False

                End If

            Else

                Return True

            End If

        Else

            Return True

        End If

    End Function
    '
#End Region
    '
#Region " Events "
    '
    'Private Sub txtListeNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
    '    '
    '    If e.KeyCode = Keys.Enter Then
    '        '
    '        Cursor.Current = Cursors.WaitCursor
    '        '
    '        If txtListeNo.Text.Length > 0 Then

    '            txtListeNo.Text = IIf((txtListeNo.Text.ToUpper.Substring(0, 1).ToUpper = "L"), _
    '                                txtListeNo.Text.Remove(0, 1), txtListeNo.Text)

    '            txtListeNo.Text = Int(txtListeNo.Text)


    '            Try

    '                RetVal = CheckPickNo(txtListeNo.Text)


    '                If RetVal.ReturnValue = False Then

    '                    MsgBox(RetVal.GetMessages, MsgBoxStyle.Exclamation, "Uyarý")

    '                    txtListeNo.Text = ""

    '                    QtyMax = 0

    '                    Exit Sub

    '                End If

    '                QtyMax = RetVal.iInfo

    '                '
    '            Catch ex As Exception

    '                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")

    '                txtListeNo.Text = ""

    '                Exit Sub

    '            End Try
    '            '
    '            txtPaletNo.Text = ""

    '            txtKoliNo.Focus()

    '        End If
    '        '
    '        Cursor.Current = Cursors.Default

    '    End If

    'End Sub
    '
#End Region

    Private Sub btnCreatePalet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreatePalet.Click

        'If txtKoliNo.Text.Trim = "" Then
        '    MessageBox.Show("Palet etiketini okutunuz.")
        '    Exit Sub
        'End If
        '
        If Not palet Then

            'palet = txtKoliNo.Text
            'txtPaletNo.Text = palet
            btnCreatePalet.Text = "Palet Kapat"
            'txtPaletNo.Text = ""
            palet = True
        ElseIf palet Then

            'If ds.Tables("Sevk").Select("SiraNo = '" & txtKoliNo.Text & "'").Length() = 0 Then
            '    MessageBox.Show("Hiç Kutu Etiketi Okutmadan Paleti Kapatamazsýnýz.", "Ekip Mapics")
            '    Exit Sub
            'End If

            For Each dr As DataRow In ds.Tables("Sevk").Select("SiraNo = '" & txtKoliNo.Text & "'")
                '
                'dr("PaletNo") = "1"
                dr("SiraNo") = txtKoliNo.Text
                dr("newCreated") = 1
                'dr("PLTTYPE") = sPltType
            Next
            '
            bindData()
            '
            palet = False
            txtKoliNo.Text = ""
            btnCreatePalet.Text = "Palet Aç"

            Dim maxPaletNo As Integer
            Sorgu = "SELECT MAX(ISNULL(palet_no,1)) FROM Veri_Toplama.dbo.[SYTELINEPALETLEME]"
            dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

            maxPaletNo = dt.Rows(0)(0)

            For Each row As DataRow In ds.Tables("Sevk").Rows

                srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME]  " & _
                          "Set palet_no = " & maxPaletNo + 1 & "" & _
                          "Where sirano = '" & row.Item("sirano") & "'", True, Mobile.ProviderTypes.SqlClient)

            Next

            ds.Tables("Sevk").Rows.Clear()
            dtgrdMain.DataSource = Nothing

        Else
            If Message("PaletMalzemeYok") = MsgBoxResult.Yes Then
                palet = False
                txtKoliNo.Text = ""
                btnCreatePalet.Text = "Palet Aç"
            End If
        End If
      
    End Sub
    '
    Private Sub txtPaletNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtPaletNo.KeyDown
        Try
            If e.KeyCode = Keys.Enter Then

                'If ds.Tables("Sevk").Rows.Count > 0 Then
                '    For Each row In ds
                'End If

                Sorgu = "SELECT  * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE palet_no = '" & txtPaletNo.Text & "'"
                dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt.Rows.Count = 0 Then
                    MessageBox.Show("Girmiþ olduðunuz palet numarasý geçersiz.")
                    Exit Sub
                Else

                    Cursor.Current = Cursors.WaitCursor

                    If ds.Tables("Sevk").Rows.Count = 0 Then
                        For Each row As DataRow In dt.Rows
                            Dim dr As DataRow
                            'Dim db As New Sevkiyat
                            '
                            dr = ds.Tables("Sevk").NewRow
                            '
                            'dr("ListeNo") = txtListeNo.Text
                            dr("PaletNo") = txtPaletNo.Text.ToString
                            'dr("EtiketNo") = Etiket 'htout.Item("@Value")
                            dr("SiraNo") = row.Item("sirano")
                            dr("Malzeme") = row.Item("malzeme")
                            'dr("Malzeme") = row.Item("item")
                            dr("Adet") = row.Item("adet")
                            dr("newCreated") = 0
                            'dr("PLTTYPE") = "MONO"
                            '
                            ds.Tables("Sevk").Rows.InsertAt(dr, 0)

                            bindData()
                            '
                            ds.Tables("Sevk").DefaultView.Sort = "sirano desc"
                        Next
                    Else
                        MessageBox.Show("Ekraný temizledikten sonra tekrar deneyiniz.")
                        Exit Sub
                    End If



                End If

            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            Cursor.Current = Cursors.Default
        End Try

    End Sub

    Private Sub frmSevkiyat_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Cursor.Current = Cursors.WaitCursor
            srv.Url = ReadConfig("path")

            Me.Text = Me.Text & " "
            CreateTable()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            Cursor.Current = Cursors.Default
        End Try

    End Sub
    '
    Private Sub frmSevkiyat_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If Formclosing() Then
            e.Cancel = False
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
        frmMenu.Visible = True
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Dim dr As DialogResult
        dr = MessageBox.Show("Tüm palet silinecektir. Onaylýyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

        If dr = Windows.Forms.DialogResult.Yes Then
            Try
                Cursor.Current = Cursors.WaitCursor

                If ds.Tables("Sevk").Rows.Count > 0 Then

                    For Each row As DataRow In ds.Tables("Sevk").Rows
                        'If row("sirano").ToString = dtgrdMain.Item(dtgrdMain.CurrentRowIndex, 1).ToString Then

                        srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME] " & _
                              " Set palet_no = NULL " & _
                              " Where sirano = " & row("sirano").ToString, True, Mobile.ProviderTypes.SqlClient)
                        'row.Delete()

                        'Exit For
                        'End If
                    Next
                    '
                    ds.Tables("Sevk").Rows.Clear()
                    dtgrdMain.DataSource = Nothing
                    txtPaletNo.Text = ""
                    'bindData()
                    '
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            Finally
                Cursor.Current = Cursors.Default
            End Try
        End If

        'If ds.Tables("Sevk").Rows.Count > 0 Then

        '    bindData()

        'End If

    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Public ReturnValue As New ReturnValue

#Region " UserConfig "
    Public sConnStr As String '= "data source=VOLKAN;initial catalog=Demo_App;Uid=sa;pwd="
    Public sEkipEdi As String = ""
    Public sAmflib As String = ""
    '
#End Region
    '
    '
#Region " Çekme Listesi "
    '
    'Belirtilen Çekme listesinin Olup olmadýðý kontrol edilir
    Public Function CheckPickNo( _
                ByVal PickNo As Integer) As ReturnValue
        '

        Dim Qty As Decimal

        '
        Try


            '
            dt = srv.RunSqlDs( _
                   "Select CEKLIST " & _
                       "From TRCEKLIST " & _
                       "Where CEKLIST = " & PickNo.ToString & " ", "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

            If Not (dt Is Nothing) Then
                With dt
                    If .Rows.Count > 0 Then
                        '
                        Qty = .Rows.Count
                        ReturnValue.iInfo = Qty
                        Me.ReturnValue.ReturnValue = True
                    Else
                        Me.ReturnValue.Add(PickNo.ToString & " Numaralý Çekme Listesi sistemde tanýmlý deðil!")
                        'sMsg = sPickNo & " Numaralý Çekme Listesi sistemde tanýmlý deðil!"
                        Me.ReturnValue.ReturnValue = False
                    End If
                End With
            Else
                Me.ReturnValue.ReturnValue = False
            End If
            '
            Return Me.ReturnValue
            '
        Catch ex As Exception
            '
            Me.ReturnValue.Add(ex.Message)
            Me.ReturnValue.ReturnValue = False
            '
            Return Me.ReturnValue
        Finally

        End Try
        ''
    End Function
    'Etiket Numarasýna Ait Malmeze Getirilir

    Public Function GetMalzemeNo(ByVal strEtiketNo As String) As String

        Try
            If strEtiketNo.Length = 0 Then
                Throw New Exception("Lütfen etiket numarasý giriniz")
            End If

            Dim strAltMalmeze As String  'KULLANILAN DEÐER

            Dim Sorgu As String

            '
            Dim textA As String = sAmflib
            Sorgu = "Select (Select B.UVMCIM FROM ITEMASA As B  Where B.ITNBR=A.ITNBR) as UVMCIM" & _
                           " From ETIKETDTY As A Where ETKSERINO='" & strEtiketNo & "'"

            dt = srv.RunSqlDs(Sorgu, "ETIKETDTY", Mobile.ProviderTypes.SqlClient).Tables(0)
            '
            If Not (dt Is Nothing) Then
                With dt
                    If .Rows.Count > 0 Then
                        ' strMalzemeNo = IIf(IsDBNull(.Rows(0).Item("ITNBR")), 0, .Rows(0).Item("ITNBR"))
                        strAltMalmeze = IIf(IsDBNull(.Rows(0).Item("UVMCIM")), 0, .Rows(0).Item("UVMCIM"))
                        Me.ReturnValue.ReturnValue = True
                    Else
                        Me.ReturnValue.ReturnValue = False
                        strAltMalmeze = ""
                    End If
                    '
                End With
                '
                Return strAltMalmeze
            Else
                Return ""
            End If
        Catch ex As Exception
            '
            Me.ReturnValue.Add(ex.Message)
            Me.ReturnValue.ReturnValue = False
            '
            Return ""
        Finally

        End Try
        ''
    End Function

    'Sistemden yeni bir etiketno üretilir ve getirilir
    Public Function UpdateOnayKodu(ByVal strEtiketNo As String) As Boolean
        '
        '
        Try


            '
            srv.RunSql( _
                                " Update ETIKETDTY " & _
                                " Set ONAYKODU =1 " & _
                                " Where ETKSERINO='" & strEtiketNo & "'", True, Mobile.ProviderTypes.SqlClient)

            Me.ReturnValue.ReturnValue = True ' srv.Result.ReturnValue
            '
            Return Me.ReturnValue.ReturnValue
            '
        Catch ex As Exception
            '
            Me.ReturnValue.Add(ex.Message)
            Me.ReturnValue.ReturnValue = False
            '
            Return ""
        Finally

        End Try
        ''
    End Function
#End Region

    Function SeriNoAl(ByVal Type As String, Optional ByVal Artim As Integer = 1) As Double

        ''Tr_GetSeqNo()

        'Dim paramIn As New Hashtable
        'Dim paramOut As New Hashtable

        'paramIn.Add("@CountField", Type)
        'paramIn.Add("@Inc", Artim)

        'paramOut.Add("@Value", 0)

        'srv.RunSp("Tr_GetSeqNo", paramIn, paramOut)

        'Return paramOut.Keys

        Sorgu = " declare @p1 numeric(10, 0) " & _
               " exec [dbo].[Tr_GetSeqNo] @CountField = N'ETK'," & _
               "    @Inc =1 , @Value= @p1 output " & _
               " select  @p1"

        dt = srv.RunSqlDs(Sorgu, "Tr_GetSeqNo", Mobile.ProviderTypes.SqlClient).Tables(0)

        Dim EtiketNo As String = String.Empty

        If Not dt Is Nothing AndAlso _
            dt.Rows.Count > 0 Then

            EtiketNo = dt.Rows(0)(0).ToString

        End If
        Return CDbl(EtiketNo)

    End Function

#Region " Messages "
    Public Function Message(ByVal Type As String) As MsgBoxResult
        Dim sMessage As String = ""
        Dim sTitle As String = ""
        Dim msgStyl As MsgBoxStyle

        Select Case Type
            Case "SaveQuestion"
                sMessage = "Deðiþiklikleri Kaydetmek Ýstediðinizden Emin misiniz?"
                sTitle = "Kayýt Onayý"
                msgStyl = MsgBoxStyle.YesNoCancel + MsgBoxStyle.Question
                '
            Case "SendCompleted"
                sMessage = "Gönderme Ýþlemi Baþarý Ýle Gerçekleþtirildi"
                sTitle = "Bilgi"
                msgStyl = MsgBoxStyle.Information
                '
            Case "SendNotCompleted"
                sMessage = "Gönderme Ýþlemi Sýrasýnda Hata Oluþtu"
                sTitle = "Bilgi"
                msgStyl = MsgBoxStyle.Exclamation

            Case "PaletMalzemeYok"
                sMessage = "Palete ait malzeme bulunamadý! " & vbNewLine _
                            & "Paleti iptal etmek istiyor musunuz ?"
                sTitle = "Uyarý"
                msgStyl = MsgBoxStyle.YesNo + MsgBoxStyle.Exclamation
                '
            Case "AynýEtiket"
                sMessage = "Ayný etiket tekrar okutulamaz"
                sTitle = "Uyarý"
                msgStyl = MsgBoxStyle.Exclamation
                '
            Case "MalzemeYok"
                sMessage = "Listede sevk edilecek malzeme bulunamadý."
                sTitle = "Uyarý"
                msgStyl = MsgBoxStyle.Exclamation
                '
            Case "SaveCompleted"
                sMessage = "Kaydetme Ýþlemi Baþarý Ýle Gerçekleþtirildi"
                sTitle = "Bilgi"
                msgStyl = MsgBoxStyle.Information
                '
            Case "SaveNotCompleted"
                sMessage = "Kaydetme Ýþlemi Sýrasýnda Hata Oluþtu"
                sTitle = "Bilgi"
                msgStyl = MsgBoxStyle.Exclamation
                '
            Case "DeleteQuestion"
                sMessage = "Seçili Kaydý Silmek Ýstediðinizden Emin misiniz?"
                sTitle = "Silme Onayý"
                msgStyl = MsgBoxStyle.YesNoCancel + MsgBoxStyle.Question

            Case "NotFound"
                sMessage = "Kayýt bulunamadý"
                sTitle = "Bilgi"
                msgStyl = MsgBoxStyle.Exclamation
                '

            Case "IadeMiktarAsimi"
                sMessage = "Ýade edilmek istenen malzeme miktarý alýnandan daha fazla olamaz"
                sTitle = "Uyarý"
                msgStyl = MsgBoxStyle.Exclamation
                '
        End Select

        If sMessage.Trim = "" Then
            sMessage = "Mesaj Bulunamadý"
        End If
        '
        Return MsgBox(sMessage, msgStyl, sTitle)
        ''
    End Function

#End Region

    Public Structure EtiketBilgisi
        Public PickNo As Integer
        Public EtkSeriNo As String 'EtiketNo
        Public House As String
        Public Itnbr As String
        Public L3P As String '16.01.07 tarihinde eklendi
        Public Pkmik As Integer
        Public Pkod As String

    End Structure

    Public Function SelectDistinct(ByVal SourceTable As DataTable, ByVal ParamArray FieldNames() As String) As DataTable
        Dim lastValues() As Object
        Dim newTable As DataTable

        If FieldNames Is Nothing OrElse FieldNames.Length = 0 Then
            Throw New ArgumentNullException("FieldNames")
        End If

        lastValues = New Object(FieldNames.Length - 1) {}
        newTable = New DataTable

        For Each field As String In FieldNames
            newTable.Columns.Add(field, SourceTable.Columns(field).DataType)
        Next

        For Each Row As DataRow In SourceTable.Select("", String.Join(", ", FieldNames))
            If Not fieldValuesAreEqual(lastValues, Row, FieldNames) Then
                newTable.Rows.Add(createRowClone(Row, newTable.NewRow(), FieldNames))

                setLastValues(lastValues, Row, FieldNames)
            End If
        Next

        Return newTable
    End Function

    Public Function fieldValuesAreEqual(ByVal lastValues() As Object, ByVal currentRow As DataRow, ByVal fieldNames() As String) As Boolean
        Dim areEqual As Boolean = True

        For i As Integer = 0 To fieldNames.Length - 1
            If lastValues(i) Is Nothing OrElse Not lastValues(i).Equals(currentRow(fieldNames(i))) Then
                areEqual = False
                Exit For
            End If
        Next

        Return areEqual
    End Function

    Public Function createRowClone(ByVal sourceRow As DataRow, ByVal newRow As DataRow, ByVal fieldNames() As String) As DataRow
        For Each field As String In fieldNames
            newRow(field) = sourceRow(field)
        Next

        Return newRow
    End Function

    Public Sub setLastValues(ByVal lastValues() As Object, ByVal sourceRow As DataRow, ByVal fieldNames() As String)
        For i As Integer = 0 To fieldNames.Length - 1
            lastValues(i) = sourceRow(fieldNames(i))
        Next
    End Sub

    Public Function sTirnakEkle(ByVal sStr As String) As String

        sStr = RTrim(sStr)

        If sStr Is Nothing Then
            sStr = ""
        End If

        sStr = sStr.Replace("'", " ")

        sStr = sStr.Replace(",", " ")

        sStr = "'" & sStr & "'"


        sTirnakEkle = sStr

        Return sStr

    End Function

    Public Sub GetRowInfo(ByRef oObject As Object, ByVal dtRow As DataTable, ByVal oRow As Integer, ByVal oColumn As String)

        Try

            If Not dtRow Is Nothing AndAlso dtRow.Rows.Count >= oRow + 1 Then

                oObject = IIf(dtRow.Rows(oRow).Item(oColumn) = "" And (TypeOf (oObject) Is Double Or TypeOf (oObject) Is Integer), 0, dtRow.Rows(oRow).Item(oColumn).ToString)

            Else

                If TypeOf oObject Is Integer Or TypeOf oObject Is Double Then

                    oObject = 0

                ElseIf TypeOf oObject Is String Then

                    oObject = ""

                Else

                    oObject = Nothing

                End If

            End If

        Catch ex As Exception


            Throw ex

        End Try

    End Sub

    Public Function sLookup(ByVal sField As String, ByVal sTable As String, ByVal sWhere As String) As String
        Try
            Dim sQuery As String

            sLookup = ""

            sQuery = "SELECT " & sField

            sQuery = sQuery & " FROM " & sTable

            If sWhere <> "" Then

                sQuery = sQuery & " WHERE " & sWhere

            End If

            dt = srv.RunSqlDs(sQuery, "Table", Mobile.ProviderTypes.SqlClient).Tables(0)

            If Not dt Is Nothing Then

                If ds.Tables("Table").Rows.Count > 0 Then

                    sLookup = RTrim(ds.Tables("Table").Rows(0).Item(0).ToString)

                End If

            Else

                sLookup = ""

            End If

            Return sLookup

        Catch ex As Exception

            Throw ex

        End Try

    End Function

    Public Shared Function Contains(ByVal Hedef As String, ByVal Aranan As String) As Boolean

        Dim Durum As Boolean = False

        Try


            If Hedef.Length > 0 Then

                For i As Integer = 0 To Hedef.Length - 1

                    If Aranan = Hedef.Substring(i, Aranan.Length) Then

                        Durum = True

                    End If

                Next


            End If

            Return Durum

        Catch ex As Exception

            Return Durum

        End Try

    End Function

    Private Sub txtKoliNo_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtKoliNo.KeyDown
        'Malzeme = ""
        'Dim Lot As String = ""
        'MalzemeMiktari = "0"
        'Dim Etiket As String
        'ayniEtiketVar = False

        Try

            If e.KeyCode = Keys.Enter Then

                If txtKoliNo.Text.Length = 0 Then
                    Exit Sub
                End If


                If txtPaletNo.Text = "" Then
                    'If Not palet Then
                    If btnCreatePalet.Text = "Palet Aç" Then
                        MessageBox.Show("Palet açtýktan sonra koliyi okutunuz.")
                        txtKoliNo.Text = ""
                        Exit Sub
                    End If

                    'End If
                End If

                'If txtMalzemeNo.Text.IndexOf("%") <> -1 Then
                '    Malzeme = txtMalzemeNo.Text.Split("%")(0)
                '    MalzemeMiktari = txtMalzemeNo.Text.Split("%")(2)
                'Else
                '    Malzeme = txtMalzemeNo.Text
                '    MalzemeMiktari = 1
                'End If

                Cursor.Current = Cursors.WaitCursor

                Sorgu = "SELECT  * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtKoliNo.Text & "'"
                dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt.Rows.Count = 0 Then
                    MessageBox.Show("Girmiþ olduðunuz koli numarasý mevcut deðil. " & vbNewLine & _
                                    "Farklý bir koli numarasý giriniz.", _
                                   "Ekip Mapics")
                    txtKoliNo.Text = ""
                    Exit Sub

                Else

                    Sorgu = "SELECT  * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtKoliNo.Text & "' and ISNULL(palet_no,0) <> 0"
                    dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count <> 0 Then
                        MessageBox.Show("Girmiþ olduðunuz koli etiketi zaten paletlenmiþ.")
                        Exit Sub
                    End If

                    Sorgu = "SELECT  * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtKoliNo.Text & "' and adet = 0"
                    dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count <> 0 Then
                        MessageBox.Show("Miktar sýfýr olamaz.")
                        Exit Sub
                    End If

                    Dim musteri As String = ""
                    'Dim cust_seq As Integer = 0
                    Dim koli As String = ""

                    If ds.Tables("Sevk").Rows.Count > 0 Then

                        koli = ds.Tables("Sevk").Rows(0)("sirano")

                        Sorgu = "SELECT firmakoduzun + cast( cust_seq as nvarchar(5)) firma FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & koli & "'"
                        dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                        If dt.Rows.Count > 0 Then
                            musteri = dt.Rows(0)("firma")
                        End If

                        Sorgu = "SELECT firmakoduzun + cast( cust_seq as nvarchar(5)) firma FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtKoliNo.Text & "'"
                        dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                        If dt.Rows(0)("firma") <> musteri Then
                            MessageBox.Show("Ayný palete farklý müþterilerin kolisi eklenemez.")
                            txtKoliNo.Text = ""
                            Exit Sub
                        End If

                    End If

                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtKoliNo.Text & "'"
                    dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    Dim dr As DataRow
                    'Dim db As New Sevkiyat
                    '
                    dr = ds.Tables("Sevk").NewRow
                    '
                    'dr("ListeNo") = txtListeNo.Text

                    If txtPaletNo.Text <> "" Then
                        dr("PaletNo") = txtPaletNo.Text
                    Else
                        dr("PaletNo") = "1"
                    End If

                    'dr("EtiketNo") = Etiket 'htout.Item("@Value")
                    dr("SiraNo") = txtKoliNo.Text
                    dr("Malzeme") = dt.Rows(0)("malzeme")
                    'dr("Malzeme") = dt.Rows(0)("item")
                    dr("Adet") = dt.Rows(0)("adet")
                    dr("newCreated") = 0
                    'dr("PLTTYPE") = "MONO"
                    '
                    ds.Tables("Sevk").Rows.InsertAt(dr, 0)

                    bindData()
                    '


                    'If txtPaletNo.Text <> "" Then

                    srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME]  " & _
                      "Set palet_no = " & IIf(txtPaletNo.Text = "", "1", txtPaletNo.Text) & "" & _
                      "Where sirano = '" & txtKoliNo.Text & "'", True, Mobile.ProviderTypes.SqlClient)

                    'End If

                    txtKoliNo.Text = ""

                    txtKoliNo.Focus()

                    ds.Tables("Sevk").DefaultView.Sort = "sirano desc"

                End If




                'Dim eklenecekMiktar As Integer = 0
                'Sorgu = "SELECT SUM(MIKTAR) FROM TR_PAKET WHERE CEKLIST = " & sTirnakEkle(txtListeNo.Text) & _
                '       " AND MALZEME = " & sTirnakEkle(Malzeme)

                'dt = srv.RunSqlDs(Sorgu, "TR_PAKET", Mobile.ProviderTypes.SqlClient).Tables(0)
                'If dt.Rows.Count <> 0 Then
                '    eklenecekMiktar = dt.Rows(0)(0)
                'End If

                'Dim griddekiMiktar As Integer = 0
                'For Each row As DataRow In ds.Tables("Sevk").Rows
                '    If row("Item").ToString = Malzeme And row("listeNo").ToString = txtListeNo.Text Then
                '        griddekiMiktar = griddekiMiktar + row("Miktar")
                '    End If
                'Next

                'Sorgu = "SELECT SUM(ADAQQT) FROM TRCEKLIST WHERE CEKLIST = " & sTirnakEkle(txtListeNo.Text) & _
                '        " AND ADAITX = " & sTirnakEkle(Malzeme)
                'dt = srv.RunSqlDs(Sorgu, "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

                'If dt.Rows(0)(0) < eklenecekMiktar + griddekiMiktar + MalzemeMiktari Then
                '    MessageBox.Show("Toplam çekme miktarýný aþtýnýz.")
                '    txtMalzemeNo.Text = ""
                '    txtMalzemeNo.Focus()
                '    'Cursor.Current = Cursors.Default
                '    Exit Sub
                'End If

                'Dim dr As DataRow
                ''Dim db As New Sevkiyat
                ''
                'dr = ds.Tables("Sevk").NewRow
                ''
                'dr("ListeNo") = txtListeNo.Text
                'dr("PaletNo") = txtKoliNo.Text
                ''dr("EtiketNo") = Etiket 'htout.Item("@Value")
                'dr("Item") = Malzeme
                ''dr("Lot") = " "
                'dr("Miktar") = MalzemeMiktari
                'dr("newCreated") = 0
                'dr("PLTTYPE") = "MONO"
                ''
                'ds.Tables("Sevk").Rows.InsertAt(dr, 0)

                'bindData()
                ''
                'txtMalzemeNo.Text = ""

                'txtMalzemeNo.Focus()

                'ds.Tables("Sevk").DefaultView.Sort = "Item desc"

                'End If

            End If
            '

        Catch ex As Exception

            Cursor.Current = Cursors.Default

            MessageBox.Show(ex.Message, "Ekip Mapics")
        Finally
            Cursor.Current = Cursors.Default

        End Try
    End Sub

    'Private Sub btnKoliSil_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKoliSil.Click
    '    Dim dr As DialogResult

    '    If txtListeNo.Text = "" And txtKoliNo.Text = "" Then
    '        MessageBox.Show("Çekme numarasý ve koli bilgilerinden biri eksik.")
    '        Exit Sub
    '    End If

    '    Sorgu = "SELECT * FROM TR_PAKET WHERE CEKLIST = " & txtListeNo.Text & " AND KOLI = '" & txtKoliNo.Text & "'"
    '    dt = srv.RunSqlDs(Sorgu, "TR_PAKET", Mobile.ProviderTypes.SqlClient).Tables(0)

    '    If dt.Rows.Count = 0 Then
    '        MessageBox.Show("Sistemde kayýtlý böyle bir koli numarasý yoktur.")
    '        Exit Sub
    '    End If

    '    'If txtListeNo.Text <> "" And txtKoliNo.Text <> "" Then

    '    Sorgu = "SELECT * FROM SHPPACK WHERE PICKNO = " & txtListeNo.Text
    '    dt = srv.RunSqlDs(Sorgu, "SHPPACK", Mobile.ProviderTypes.SqlClient).Tables(0)

    '    If dt.Rows.Count > 0 Then
    '        MessageBox.Show("Ýlgili çekme sevk edilmiþ. Koli silinemez.")
    '        Exit Sub
    '    End If

    '    Sorgu = "SELECT COUNT(*) FROM TR_PAKET WHERE CEKLIST = " & txtListeNo.Text & " AND KOLI = '" & txtKoliNo.Text & "'"
    '    dt = srv.RunSqlDs(Sorgu, "TR_PAKET", Mobile.ProviderTypes.SqlClient).Tables(0)

    '    If dt.Rows.Count <> 0 Then
    '        dr = MessageBox.Show(txtListeNo.Text & " nolu çekmenin " & txtKoliNo.Text & " nolu kolisine ait " & dt.Rows(0)(0) & " kayýt silinecektir. Emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

    '        If dr = Windows.Forms.DialogResult.Yes Then
    '            Sorgu = "DELETE FROM TR_PAKET WHERE CEKLIST = " & txtListeNo.Text & " AND KOLI = '" & txtKoliNo.Text & "'"
    '            srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)
    '        End If

    '    End If
    '    'End If

    'End Sub

    Private Sub txtPaletNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPaletNo.KeyPress

    End Sub

    Private Sub btnTemizle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTemizle.Click
        txtPaletNo.Text = String.Empty
        txtKoliNo.Text = String.Empty
        ds.Tables("Sevk").Rows.Clear()
        dtgrdMain.DataSource = Nothing
    End Sub

    Private Sub btnKoliSil_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKoliSil.Click

        Dim dr As DialogResult
        dr = MessageBox.Show("Listeden seçmiþ olduðunuz koli etiketi silinecektir. Onaylýyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

        If dr = Windows.Forms.DialogResult.Yes Then

            Try
                If ds.Tables("Sevk").Rows.Count > 0 Then

                    For Each row As DataRow In ds.Tables("Sevk").Rows

                        If row("sirano").ToString = dtgrdMain.Item(dtgrdMain.CurrentRowIndex, 1).ToString Then

                            srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME] " & _
                                  " Set palet_no = NULL " & _
                                  " Where sirano = " & dtgrdMain.Item(dtgrdMain.CurrentRowIndex, 1).ToString, True, Mobile.ProviderTypes.SqlClient)
                            row.Delete()

                            Exit For
                        End If
                    Next
                    '
                    bindData()
                    '
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        End If

    End Sub
End Class
