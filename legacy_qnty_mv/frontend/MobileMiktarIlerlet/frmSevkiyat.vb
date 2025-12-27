Imports System.Data

Public Class frmSevkiyat
    Inherits System.Windows.Forms.Form
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents txtPaletNo As System.Windows.Forms.TextBox
    Friend WithEvents lblPaletNo As System.Windows.Forms.Label
    Friend WithEvents txtListeNo As System.Windows.Forms.TextBox
    Friend WithEvents lblListeNo As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnCreatePalet As System.Windows.Forms.Button
    Friend WithEvents lblEtiketNo As System.Windows.Forms.Label
    Friend WithEvents txtEtiketNo As System.Windows.Forms.TextBox
    Friend WithEvents dtgrdMain As System.Windows.Forms.DataGrid
    Friend WithEvents btnSend As System.Windows.Forms.Button

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
    Friend WithEvents txtMiktar As System.Windows.Forms.TextBox
    Friend WithEvents lblMiktar As System.Windows.Forms.Label
    Friend WithEvents PaletNo As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents SiraNo As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Malzeme As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Adet As System.Windows.Forms.DataGridTextBoxColumn

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSevkiyat))
        Me.PaletNo = New System.Windows.Forms.DataGridTextBoxColumn
        Me.SiraNo = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Malzeme = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Adet = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.txtEtiketNo = New System.Windows.Forms.TextBox
        Me.lblEtiketNo = New System.Windows.Forms.Label
        Me.txtPaletNo = New System.Windows.Forms.TextBox
        Me.lblPaletNo = New System.Windows.Forms.Label
        Me.txtListeNo = New System.Windows.Forms.TextBox
        Me.lblListeNo = New System.Windows.Forms.Label
        Me.btnSend = New System.Windows.Forms.Button
        Me.btnCreatePalet = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.dtgrdMain = New System.Windows.Forms.DataGrid
        Me.btnDelete = New System.Windows.Forms.Button
        Me.txtMiktar = New System.Windows.Forms.TextBox
        Me.lblMiktar = New System.Windows.Forms.Label
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'PaletNo
        '
        Me.PaletNo.Format = ""
        Me.PaletNo.FormatInfo = Nothing
        Me.PaletNo.HeaderText = "PaletNo"
        Me.PaletNo.MappingName = "PaletNo"
        Me.PaletNo.NullText = ""
        Me.PaletNo.Width = 45
        '
        'SiraNo
        '
        Me.SiraNo.Format = ""
        Me.SiraNo.FormatInfo = Nothing
        Me.SiraNo.HeaderText = "SiraNo"
        Me.SiraNo.MappingName = "SiraNo"
        Me.SiraNo.NullText = ""
        Me.SiraNo.Width = 55
        '
        'Malzeme
        '
        Me.Malzeme.Format = ""
        Me.Malzeme.FormatInfo = Nothing
        Me.Malzeme.HeaderText = "Malzeme"
        Me.Malzeme.MappingName = "Malzeme"
        Me.Malzeme.NullText = ""
        Me.Malzeme.Width = 100
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
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.txtEtiketNo)
        Me.Panel1.Controls.Add(Me.lblEtiketNo)
        Me.Panel1.Controls.Add(Me.txtPaletNo)
        Me.Panel1.Controls.Add(Me.lblPaletNo)
        Me.Panel1.Controls.Add(Me.txtListeNo)
        Me.Panel1.Controls.Add(Me.lblListeNo)
        Me.Panel1.Controls.Add(Me.btnSend)
        Me.Panel1.Controls.Add(Me.btnCreatePalet)
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(240, 51)
        '
        'txtEtiketNo
        '
        Me.txtEtiketNo.Location = New System.Drawing.Point(64, 48)
        Me.txtEtiketNo.MaxLength = 60
        Me.txtEtiketNo.Name = "txtEtiketNo"
        Me.txtEtiketNo.Size = New System.Drawing.Size(89, 21)
        Me.txtEtiketNo.TabIndex = 0
        Me.txtEtiketNo.Visible = False
        '
        'lblEtiketNo
        '
        Me.lblEtiketNo.Location = New System.Drawing.Point(3, 48)
        Me.lblEtiketNo.Name = "lblEtiketNo"
        Me.lblEtiketNo.Size = New System.Drawing.Size(55, 20)
        Me.lblEtiketNo.Text = "Etiket No"
        Me.lblEtiketNo.Visible = False
        '
        'txtPaletNo
        '
        Me.txtPaletNo.Location = New System.Drawing.Point(64, 28)
        Me.txtPaletNo.Name = "txtPaletNo"
        Me.txtPaletNo.Size = New System.Drawing.Size(89, 21)
        Me.txtPaletNo.TabIndex = 2
        '
        'lblPaletNo
        '
        Me.lblPaletNo.Location = New System.Drawing.Point(3, 28)
        Me.lblPaletNo.Name = "lblPaletNo"
        Me.lblPaletNo.Size = New System.Drawing.Size(52, 20)
        Me.lblPaletNo.Text = "Palet No"
        '
        'txtListeNo
        '
        Me.txtListeNo.Location = New System.Drawing.Point(64, 8)
        Me.txtListeNo.Name = "txtListeNo"
        Me.txtListeNo.Size = New System.Drawing.Size(89, 21)
        Me.txtListeNo.TabIndex = 4
        '
        'lblListeNo
        '
        Me.lblListeNo.Location = New System.Drawing.Point(3, 8)
        Me.lblListeNo.Name = "lblListeNo"
        Me.lblListeNo.Size = New System.Drawing.Size(52, 20)
        Me.lblListeNo.Text = "Liste No"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(154, 8)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(84, 40)
        Me.btnSend.TabIndex = 7
        Me.btnSend.Text = "Gönder"
        '
        'btnCreatePalet
        '
        Me.btnCreatePalet.Location = New System.Drawing.Point(154, 8)
        Me.btnCreatePalet.Name = "btnCreatePalet"
        Me.btnCreatePalet.Size = New System.Drawing.Size(84, 30)
        Me.btnCreatePalet.TabIndex = 6
        Me.btnCreatePalet.Text = "Palet Aç"
        Me.btnCreatePalet.Visible = False
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(168, 176)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(72, 20)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Kapat"
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.dtgrdMain)
        Me.Panel3.Controls.Add(Me.btnDelete)
        Me.Panel3.Controls.Add(Me.txtMiktar)
        Me.Panel3.Controls.Add(Me.lblMiktar)
        Me.Panel3.Controls.Add(Me.btnCancel)
        Me.Panel3.Location = New System.Drawing.Point(0, 51)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(240, 217)
        '
        'dtgrdMain
        '
        Me.dtgrdMain.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.dtgrdMain.Location = New System.Drawing.Point(0, 0)
        Me.dtgrdMain.Name = "dtgrdMain"
        Me.dtgrdMain.Size = New System.Drawing.Size(240, 170)
        Me.dtgrdMain.TabIndex = 3
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(128, 176)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(43, 20)
        Me.btnDelete.TabIndex = 0
        Me.btnDelete.Text = "Sil"
        '
        'txtMiktar
        '
        Me.txtMiktar.Location = New System.Drawing.Point(80, 176)
        Me.txtMiktar.Name = "txtMiktar"
        Me.txtMiktar.ReadOnly = True
        Me.txtMiktar.Size = New System.Drawing.Size(48, 21)
        Me.txtMiktar.TabIndex = 1
        '
        'lblMiktar
        '
        Me.lblMiktar.Location = New System.Drawing.Point(6, 176)
        Me.lblMiktar.Name = "lblMiktar"
        Me.lblMiktar.Size = New System.Drawing.Size(74, 13)
        Me.lblMiktar.Text = "Kalem Sayisi"
        '
        'frmSevkiyat
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ClientSize = New System.Drawing.Size(240, 271)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmSevkiyat"
        Me.Text = "Sevkiyat"
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
    Dim dt2 As New DataTable

    Dim item As String
    Dim dtEtiketDty As DataTable
    Dim dtLotLoc As DataTable
    Dim sumBoxQty As Integer
    Dim loc As String
    Dim pickNo As String
    Dim dtUm As DataTable
    Dim ayniEtiketVar As Boolean = False
    Dim etiketler As String = String.Empty
    '
#Region " Methods "

    Private Sub CreateTable()

        ds = Nothing
        Dim dt As New DataTable("Sevk")

        If dt.Columns.Count = 0 Then
            dt.Columns.Add("Malzeme")
            dt.Columns.Add("Adet")
            dt.Columns.Add("PaletNo")
            dt.Columns.Add("SiraNo")
        End If

        ds = New DataSet
        ds.Tables.Add(dt)

    End Sub

    Private Sub bindData()

        With dtgrdMain
            .DataSource = ds.Tables(0)
        End With

        Dim tablestyle As New DataGridTableStyle

        Dim column As New DataGridTextBoxColumn
        column.MappingName = "Malzeme"
        column.HeaderText = "Malzeme"
        column.Width = 200
        tablestyle.GridColumnStyles.Add(column)

        column = New DataGridTextBoxColumn
        column.MappingName = "Adet"
        column.HeaderText = "Adet"
        column.Width = 30
        tablestyle.GridColumnStyles.Add(column)

        column = New DataGridTextBoxColumn
        column.MappingName = "PaletNo"
        column.HeaderText = "PaletNo"
        column.Width = 45
        tablestyle.GridColumnStyles.Add(column)

        column = New DataGridTextBoxColumn
        column.MappingName = "SiraNo"
        column.HeaderText = "SiraNo"
        column.Width = 55
        tablestyle.GridColumnStyles.Add(column)

        dtgrdMain.TableStyles.Clear()
        dtgrdMain.TableStyles.Add(tablestyle)

        txtMiktar.Text = (ds.Tables("Sevk").Rows.Count)

    End Sub

    Private Sub SendData(Optional ByVal bKapat As Boolean = False)

        Try

            If Not (ds Is Nothing) And _
            (ds.Tables(0).Rows.Count >= 1) Then

                Dim ds1 As New DataSet

                ds1 = ds.Clone

                ds1.Tables("Sevk").Clear()

                Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE palet_no = '" & txtPaletNo.Text & "'"
                dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt2.Rows.Count > 0 Then
                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE palet_no = '" & txtPaletNo.Text & "' AND isnull(cekmeno,0) <> 0"
                    dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt2.Rows.Count <> 0 Then
                        MessageBox.Show("Girmiþ olduðunuz palet zaten çekilmiþ.")
                        Exit Sub
                    End If
                Else
                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtPaletNo.Text & "' AND isnull(cekmeno,0) <> 0"
                    dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt2.Rows.Count <> 0 Then
                        MessageBox.Show("Girmiþ olduðunuz koli etiketi zaten çekilmiþ.")
                        Exit Sub
                    End If
                End If

                For Each dr As DataRow In ds.Tables("Sevk").Rows

                    Sorgu = "SELECT * FROM TRCEKLIST WHERE ceklist = '" & txtListeNo.Text & "' AND ADAITX = '" & dr("Malzeme") & "'"
                    dt2 = srv.RunSqlDs(Sorgu, "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt2.Rows.Count = 0 Then
                        MessageBox.Show(dr("Malzeme") & " nolu malzeme " & txtListeNo.Text & " nolu çekmeye ait deðildir." & vbNewLine & _
                                        "Geçerli bir palet okutunuz.")

                        ds.Tables("Sevk").Rows.Clear()
                        txtPaletNo.Text = ""
                        Exit Sub
                    End If

                Next

                ' ÇEKME MÝKTARI AÞILAMAZ KONTROLÜ '

                Dim sevkMik As Integer = 0

                For Each dr2 As DataRow In ds.Tables("Sevk").Rows

                    sevkMik = dr2("Adet")

                    Dim cekMik As Integer = 0

                    Sorgu = "select sum(ADAQQT-Okutulan) ADAQQT FROM TRCEKLIST WHERE CEKLIST = " & txtListeNo.Text & _
                    " AND ADAITX = '" & dr2("Malzeme") & "'"
                    dt2 = srv.RunSqlDs(Sorgu, "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

                    cekMik = dt2.Rows(0)("ADAQQT")

                    If cekMik <> sevkMik Then
                        MessageBox.Show(dr2("Malzeme") & " nolu malzemeden " & cekMik & " adet sevk etmelisiniz.")
                        Exit Sub
                    End If

                Next

                ' ÇEKME MÝKTARI AÞILAMAZ KONTROLÜ '

                dt = SelectDistinct(ds.Tables("Sevk"), "sirano")

                Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE palet_no = '" & txtPaletNo.Text & "'"
                dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt2.Rows.Count > 0 Then

                    srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME]  " & _
                                " Set cekmeno = " & txtListeNo.Text & "" & _
                                " Where palet_no = '" & txtPaletNo.Text & "'", True, Mobile.ProviderTypes.SqlClient)

                Else
                    srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME]  " & _
                                " Set cekmeno = " & txtListeNo.Text & "" & _
                                " Where sirano = '" & txtPaletNo.Text & "'", True, Mobile.ProviderTypes.SqlClient)
                End If

                Dim miktar As Integer = 0
                Dim toplam As Integer = 0
                Dim satirToplam As Integer = 0

                For Each row As DataRow In ds.Tables("Sevk").Rows
                    toplam = toplam + row("adet").ToString
                Next

                For Each row As DataRow In ds.Tables("Sevk").Rows

                    miktar = row("adet").ToString
                    satirToplam = row("adet").ToString

                    Sorgu = "SELECT (ADAQQT - Okutulan) Kalan , * FROM TRCEKLIST WHERE CEKLIST = '" & txtListeNo.Text & _
                    "' AND ADAITX ='" & row("Malzeme").ToString & _
                    "' AND (ADAQQT - Okutulan) > 0"
                    dt2 = srv.RunSqlDs(Sorgu, "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

                    For Each row2 As DataRow In dt2.Rows

                        Dim kalan As Integer = 0
                        kalan = row2("kalan")

                        If satirToplam <> 0 Then
                            If toplam <> 0 And toplam > 0 Then
                                If miktar > kalan Then
                                    Sorgu = "Update TRCEKLIST  " & _
                                      " Set Okutulan = Okutulan + " & kalan & "" & _
                                      " Where CEKLIST = '" & txtListeNo.Text & "' AND ADAITX ='" & row2("ADAITX").ToString & "'" & _
                                      " AND (ADAQQT - Okutulan) > 0 AND ADCVNB ='" & row2("ADCVNB") & "' AND ADFCNB =" & row2("ADFCNB")
                                    srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)

                                    miktar = miktar - kalan
                                    toplam = toplam - kalan
                                    satirToplam = satirToplam - kalan
                                Else
                                    Sorgu = "Update TRCEKLIST  " & _
                                     " Set Okutulan = Okutulan + " & miktar & "" & _
                                     " Where CEKLIST = '" & txtListeNo.Text & "' AND ADAITX ='" & row2("ADAITX").ToString & "'" & _
                                     " AND (ADAQQT - Okutulan) > 0 AND ADCVNB ='" & row2("ADCVNB") & "' AND ADFCNB =" & row2("ADFCNB")
                                    srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)

                                    toplam = toplam - miktar
                                    satirToplam = satirToplam - miktar
                                End If
                            End If
                        Else
                            Exit For
                        End If
                    Next
                Next

                dt = ds.Tables("Sevk")

                pickNo = txtListeNo.Text
                txtEtiketNo.Text = ""
                txtPaletNo.Text = ""
                Qty = 0

                palet = False

                dtgrdMain.DataSource = Nothing

                CreateTable()

                Message("SendCompleted")

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
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")
        End Try

    End Sub

    Function GetUm(ByVal item As String) As String
        Try

            Dim um As String
            Sorgu = "SELECT u_m FROM ITEM WHERE ITEM = '" & item & "'"
            dtUm = srv.RunSqlDs(Sorgu, "ITEM", Mobile.ProviderTypes.SqlClient).Tables(0)

            um = dtUm.Rows(0)(0)
            Return um

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return " "
        End Try
    End Function

    Private Function Formclosing() As Boolean

    End Function

#End Region

#Region " Events "

    Private Sub txtListeNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtListeNo.KeyDown

        If e.KeyCode = Keys.Enter Then

            Cursor.Current = Cursors.WaitCursor

            If txtListeNo.Text.Length > 0 Then

                txtListeNo.Text = IIf((txtListeNo.Text.ToUpper.Substring(0, 1).ToUpper = "L"), _
                                    txtListeNo.Text.Remove(0, 1), txtListeNo.Text)

                txtListeNo.Text = Int(txtListeNo.Text)

                Try

                    RetVal = CheckPickNo(txtListeNo.Text)

                    If RetVal.ReturnValue = False Then

                        MsgBox(RetVal.GetMessages, MsgBoxStyle.Exclamation, "Uyarý")

                        txtListeNo.Text = ""

                        QtyMax = 0

                        Exit Sub

                    End If

                    QtyMax = RetVal.iInfo

                Catch ex As Exception

                    MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")

                    txtListeNo.Text = ""

                    Exit Sub

                End Try

                txtPaletNo.Text = ""
                txtPaletNo.Focus()

            End If

            Cursor.Current = Cursors.Default

        End If

    End Sub

#End Region

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

    Private Sub frmSevkiyat_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If Formclosing() Then
            e.Cancel = False
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Hide()
        frmMenu.Visible = True
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim dr As DialogResult

        If ds.Tables("Sevk").Rows.Count = 0 Then
            MessageBox.Show("Önce Palet yada koli etiketi okutunuz!", "Ekip Mapics")
            Exit Sub
        End If

        dr = MessageBox.Show("Paleti silmek istediðinizden emin misiniz?", "Onay", _
                             MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

        If dr = Windows.Forms.DialogResult.Yes Then

            'Dim SilinecekPalet As String = ""

            'SilinecekPalet = dtgrdMain.Item(dtgrdMain.CurrentRowIndex, 2).ToString()

            If Not (ds Is Nothing) And (ds.Tables(0).Rows.Count >= 1) Then

                Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE palet_no = '" & txtPaletNo.Text & "'"
                dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt2.Rows.Count > 0 Then

                    srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME]  " & _
                                " Set cekmeno = NULL " & _
                                " Where palet_no = '" & txtPaletNo.Text & "'", True, Mobile.ProviderTypes.SqlClient)
                Else
                    srv.RunSql("Update Veri_Toplama.dbo.[SYTELINEPALETLEME]  " & _
                                " Set cekmeno = NULL " & _
                                " Where sirano = '" & txtPaletNo.Text & "'", True, Mobile.ProviderTypes.SqlClient)
                End If


                Dim miktar As Integer = 0
                Dim toplam As Integer = 0
                Dim satirToplam As Integer = 0

                For Each row As DataRow In ds.Tables("Sevk").Rows

                    'If row("PaletNo").ToString <> SilinecekPalet Then
                    '    Continue For
                    'End If

                    toplam = toplam + row("adet").ToString
                Next

                For Each row As DataRow In ds.Tables("Sevk").Rows

                    'If row("PaletNo").ToString <> SilinecekPalet Then
                    '    Continue For
                    'End If

                    miktar = row("adet").ToString
                    satirToplam = row("adet").ToString

                    Sorgu = "SELECT Okutulan , * FROM TRCEKLIST WHERE CEKLIST = '" & txtPaletNo.Text & "' AND ADAITX ='" & row("Malzeme").ToString & _
                    "' AND Okutulan > 0"
                    dt2 = srv.RunSqlDs(Sorgu, "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

                    For Each row2 As DataRow In dt2.Rows

                        Dim okutulan As Integer = 0
                        okutulan = row2("Okutulan")

                        If satirToplam <> 0 Then
                            If toplam <> 0 And toplam > 0 Then
                                If miktar > okutulan Then
                                    Sorgu = "Update TRCEKLIST  " & _
                                      " Set Okutulan = Okutulan - " & okutulan & "" & _
                                      " Where CEKLIST = '" & txtPaletNo.Text & "' AND ADAITX ='" & row2("ADAITX").ToString & "'" & _
                                      " AND Okutulan > 0 AND ADCVNB ='" & row2("ADCVNB") & "' AND ADFCNB =" & row2("ADFCNB")
                                    srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)

                                    miktar = miktar - okutulan
                                    toplam = toplam - okutulan
                                    satirToplam = satirToplam - okutulan
                                Else
                                    Sorgu = "Update TRCEKLIST  " & _
                                     " Set Okutulan = Okutulan - " & miktar & "" & _
                                     " Where CEKLIST = '" & txtPaletNo.Text & "' AND ADAITX ='" & row2("ADAITX").ToString & "'" & _
                                     " AND Okutulan > 0 AND ADCVNB ='" & row2("ADCVNB") & "' AND ADFCNB =" & row2("ADFCNB")
                                    srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)

                                    toplam = toplam - miktar
                                    satirToplam = satirToplam - miktar
                                End If
                            End If
                        Else
                            Exit For
                        End If
                    Next
                Next

            End If

        End If

        ds.Tables("Sevk").Rows.Clear()
        dtgrdMain.DataSource = Nothing
        txtPaletNo.Text = ""

    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click

        Dim dr As DialogResult

        If ds.Tables("Sevk").Rows.Count = 0 Then
            MessageBox.Show("Palet yada koli etiketi okutmadan Sevkiyatý Gönderemezsiniz!", "Ekip Mapics")
            Exit Sub
        End If

        dr = MessageBox.Show("Paleti sevk etmek istediðinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

        If dr = Windows.Forms.DialogResult.Yes Then
            Me.Enabled = False
            Cursor.Current = Cursors.WaitCursor
            SendData()
            Cursor.Current = Cursors.Default
            Me.Enabled = True
        End If

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

#End Region
    '
    '
    'Belirtilen EtiketNo belirtilen listeye ait Olup olmadýðý kontrol edilir
    Public Function CheckEtiketNo( _
                ByVal PickNo As Integer, _
                ByVal EtiketNo As String, _
                ByRef EtiketBilgisi As EtiketBilgisi) As ReturnValue

        Try


            '
            dt = srv.RunSqlDs( _
                   "Select ITNBR, KMIK, isnull(Pkod,'') As Pkod , PKMIK  " & _
                       "From ITMPACK " & _
                       "Where AMBKOD = " & PickNo.ToString & _
                            " And Itnbr = '" & EtiketNo & "' ", "ITMPACK", Mobile.ProviderTypes.SqlClient).Tables(0)

            If Not (dt Is Nothing) Then
                With dt
                    If .Rows.Count > 0 Then
                        '
                        EtiketBilgisi.Itnbr = IIf(IsDBNull(.Rows(0).Item("ITNBR")), 0, .Rows(0).Item("ITNBR"))
                        EtiketBilgisi.EtkSeriNo = IIf(IsDBNull(.Rows(0).Item("ETKSERINO")), 0, .Rows(0).Item("ETKSERINO"))
                        EtiketBilgisi.PickNo = IIf(IsDBNull(.Rows(0).Item("PICKNO")), 0, .Rows(0).Item("PICKNO"))
                        EtiketBilgisi.House = IIf(IsDBNull(.Rows(0).Item("HOUSE")), 0, .Rows(0).Item("HOUSE"))
                        EtiketBilgisi.L3P = IIf(IsDBNull(.Rows(0).Item("L3P")), 0, .Rows(0).Item("L3P"))
                        EtiketBilgisi.Pkmik = IIf(IsDBNull(.Rows(0).Item("L3P")), 0, .Rows(0).Item("PKMIK"))
                        EtiketBilgisi.Pkod = IIf(IsDBNull(.Rows(0).Item("L3P")), 0, .Rows(0).Item("Pkod"))

                        '
                        Me.ReturnValue.ReturnValue = True
                    Else
                        Me.ReturnValue.Add( _
                            PickNo.ToString & " Numaralý Çekme Listesi ve " & _
                            EtiketNo & " Numaralý Etiket eþleþmiyor !")
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

    Public Function GetEtiketNo() As String

        Dim sEtiketNo As String
        Dim iEtiketNo As Integer

        Try

            srv.RunSql( _
                    "Update TR_COUNTER " & _
                        "Set ETIKETNO = ETIKETNO + 1 ", True, Mobile.ProviderTypes.SqlClient)

            'If srv.Result.ReturnValue Then
            '
            dt = srv.RunSql( _
                   "Select ETIKETNO  " & _
                       "From TR_COUNTER ", "TR_COUNTER", Mobile.ProviderTypes.SqlClient)
            '
            If Not (dt Is Nothing) Then
                With dt
                    If .Rows.Count > 0 Then
                        iEtiketNo = IIf(IsDBNull(.Rows(0).Item("ETIKETNO")), 0, .Rows(0).Item("ETIKETNO"))
                        Me.ReturnValue.ReturnValue = True
                    Else
                        Me.ReturnValue.ReturnValue = False
                    End If
                    '
                End With
                '
                sEtiketNo = "1" & Now.ToString("yy") & "".PadLeft(6 - iEtiketNo.ToString.Length, "0") & iEtiketNo.ToString
                Return sEtiketNo
            Else
                Return ""
                '
            End If

        Catch ex As Exception

            Me.ReturnValue.Add(ex.Message)
            Me.ReturnValue.ReturnValue = False

            Return ""
        Finally

        End Try
        ''
    End Function

    Function SeriNoAl(ByVal Type As String, Optional ByVal Artim As Integer = 1) As Double

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

    Private Sub txtPaletNo_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtPaletNo.KeyDown

        Try

            If e.KeyCode = Keys.Enter Then

                If txtPaletNo.Text.Length = 0 Then
                    Exit Sub
                End If

                Cursor.Current = Cursors.WaitCursor

                Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE palet_no = '" & txtPaletNo.Text & "'"
                dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                If dt.Rows.Count = 0 Then

                    Sorgu = "SELECT  * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtPaletNo.Text & "'"
                    dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt2.Rows.Count = 0 Then

                        MessageBox.Show("Girmiþ olduðunuz koli yada palet numarasý mevcut deðil. " & vbNewLine & _
                                        "Farklý bir koli yada palet numarasý giriniz.", _
                                        "Ekip Mapics")
                        txtPaletNo.Text = ""
                        Exit Sub
                    End If

                End If

                'Palet Numarasý ile Sevkiyat Yapýlýyor Ýse 
                If dt.Rows.Count > 0 Then

                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE palet_no = '" & txtPaletNo.Text & "' and adet = 0"
                    dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count <> 0 Then
                        MessageBox.Show("Miktar sýfýr olamaz.")
                        Exit Sub
                    End If

                    Dim musteri As String = ""
                    Dim koli As String = ""
                    Dim palet As Integer = 0

                    If ds.Tables("Sevk").Rows.Count > 0 Then

                        palet = ds.Tables("Sevk").Rows(0)("PaletNo")

                        If palet <> txtPaletNo.Text Then
                            ds.Tables("Sevk").Rows.Clear()
                        End If

                    End If

                    If ds.Tables("Sevk").Rows.Count > 0 Then

                        palet = ds.Tables("PaletNo").Rows(0)("sirano")

                        If palet <> txtPaletNo.Text Then
                            ds.Tables("Sevk").Rows.Clear()
                        End If

                        koli = ds.Tables("Sevk").Rows(0)("sirano")

                        Sorgu = "SELECT TOP 1 C6CANB + cast( cust_seq as nvarchar(5)) Musteri FROM TRCEKLIST tc " & _
                        " LEFT JOIN customer cust ON cust.cust_num = tc.C6CANB AND tc.C6B9CD = cust.Uf_B9CD " & _
                        " WHERE CEKLIST = " & txtListeNo.Text
                        dt = srv.RunSqlDs(Sorgu, "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

                        musteri = dt.Rows(0)("Musteri")

                        Sorgu = "SELECT TOP 1 firmakoduzun + cast( cust_seq as nvarchar(5)) Musteri FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] " & _
                        " WHERE palet_no = " & koli
                        dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                        If dt.Rows(0)("Musteri") <> musteri Then
                            MessageBox.Show("Müþteri ve sevk adresi uyumsuz.")
                            Exit Sub
                        End If

                    End If

                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[TR_PALET] WHERE palet_no = '" & txtPaletNo.Text & "'"
                    dt = srv.RunSqlDs(Sorgu, "TR_PALET", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If ds.Tables("Sevk").Rows.Count = 0 Then

                        For Each row As DataRow In dt.Rows
                            Dim dr As DataRow
                            dr = ds.Tables("Sevk").NewRow
                            'dr = dt.NewRow

                            dr("Malzeme") = row.Item("item")
                            dr("Adet") = row.Item("adet")
                            dr("PaletNo") = txtPaletNo.Text.ToString
                            dr("SiraNo") = row.Item("sirano")

                            ds.Tables("Sevk").Rows.InsertAt(dr, 0)
                            bindData()
                            ds.Tables("Sevk").DefaultView.Sort = "sirano desc"
                        Next

                    Else
                        MessageBox.Show("Ekraný temizledikten sonra tekrar deneyiniz.")
                        Exit Sub
                    End If

                    'Koli Numarasý ile Sevkiyat Yapýlýyor Ýse 
                ElseIf dt2.Rows.Count > 0 Then

                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtPaletNo.Text & "' and adet = 0"
                    dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt2.Rows.Count <> 0 Then
                        MessageBox.Show("Miktar sýfýr olamaz.")
                        Exit Sub
                    End If

                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtPaletNo.Text & "' and ISNULL(palet_no,0) > 0"
                    dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt2.Rows.Count <> 0 Then
                        MessageBox.Show("Bu koli paletlenmiþ. Lütfen " & dt2.Rows(0)("palet_no") & " nolu paleti okutunuz.")
                        txtPaletNo.Text = ""
                        Exit Sub
                    End If

                    Dim musteri As String = ""
                    Dim koli As String = ""

                    If ds.Tables("Sevk").Rows.Count > 0 Then

                        koli = ds.Tables("Sevk").Rows(0)("sirano")

                        Sorgu = "SELECT TOP 1 C6CANB + cast( cust_seq as nvarchar(5)) Musteri FROM TRCEKLIST tc " & _
                        " LEFT JOIN customer cust ON cust.cust_num=tc.C6CANB AND tc.C6B9CD = cust.Uf_B9CD " & _
                        " WHERE CEKLIST = " & txtListeNo.Text
                        dt2 = srv.RunSqlDs(Sorgu, "TRCEKLIST", Mobile.ProviderTypes.SqlClient).Tables(0)

                        musteri = dt2.Rows(0)("Musteri")

                        Sorgu = "SELECT TOP 1 firmakoduzun + cast( cust_seq as nvarchar(5)) Musteri FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] " & _
                        " WHERE sirano = " & koli
                        dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                        If dt2.Rows(0)("Musteri") <> musteri Then
                            MessageBox.Show("Müþteri ve sevk adresi uyumsuz.")
                            Exit Sub
                        End If

                    End If

                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano = '" & txtPaletNo.Text & "'"
                    dt2 = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    Dim dr As DataRow

                    dr = ds.Tables("Sevk").NewRow
                    'dr = dt.NewRow

                    dr("Malzeme") = dt2.Rows(0)("item")
                    dr("Adet") = dt2.Rows(0)("adet")

                    If txtPaletNo.Text <> "" Then
                        dr("PaletNo") = txtPaletNo.Text
                    Else
                        dr("PaletNo") = "1"
                    End If

                    dr("SiraNo") = dt2.Rows(0)("sirano")

                    ds.Tables("Sevk").Rows.InsertAt(dr, 0)
                    bindData()
                    ds.Tables("Sevk").DefaultView.Sort = "sirano desc"

                End If

            End If


        Catch ex As Exception

            Cursor.Current = Cursors.Default

            MessageBox.Show(ex.Message, "Ekip Mapics")
        Finally
            Cursor.Current = Cursors.Default

        End Try
    End Sub

End Class
