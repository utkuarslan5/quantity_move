Imports System.Data

Public Class frmPaletIslemleri
    Inherits System.Windows.Forms.Form
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents lblEtiketNo As System.Windows.Forms.Label
    Friend WithEvents txtPaletNo As System.Windows.Forms.TextBox
    Friend WithEvents lblPaletNo As System.Windows.Forms.Label
    Friend WithEvents txtListeNo As System.Windows.Forms.TextBox
    Friend WithEvents lblListeNo As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnCreatePalet As System.Windows.Forms.Button
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents txtMiktar As System.Windows.Forms.TextBox
    Friend WithEvents lblMiktar As System.Windows.Forms.Label
    Friend WithEvents dtgrdMain As System.Windows.Forms.DataGrid
    Friend WithEvents tbSevk As System.Windows.Forms.DataGridTableStyle
    Friend WithEvents PaletNo As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents SiraNo As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Malzeme As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents Adet As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents txtEtiketNo As System.Windows.Forms.TextBox

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



    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPaletIslemleri))
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.lblEtiketNo = New System.Windows.Forms.Label
        Me.txtPaletNo = New System.Windows.Forms.TextBox
        Me.lblPaletNo = New System.Windows.Forms.Label
        Me.txtListeNo = New System.Windows.Forms.TextBox
        Me.lblListeNo = New System.Windows.Forms.Label
        Me.btnCreatePalet = New System.Windows.Forms.Button
        Me.btnSend = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.btnDelete = New System.Windows.Forms.Button
        Me.txtMiktar = New System.Windows.Forms.TextBox
        Me.lblMiktar = New System.Windows.Forms.Label
        Me.dtgrdMain = New System.Windows.Forms.DataGrid
        Me.tbSevk = New System.Windows.Forms.DataGridTableStyle
        Me.txtEtiketNo = New System.Windows.Forms.TextBox

        Me.paletNo = New System.Windows.Forms.DataGridTextBoxColumn
        Me.SiraNo = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Malzeme = New System.Windows.Forms.DataGridTextBoxColumn
        Me.Adet = New System.Windows.Forms.DataGridTextBoxColumn

        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.txtEtiketNo)
        Me.Panel1.Controls.Add(Me.lblEtiketNo)
        Me.Panel1.Controls.Add(Me.txtPaletNo)
        Me.Panel1.Controls.Add(Me.lblPaletNo)
        Me.Panel1.Controls.Add(Me.txtListeNo)
        Me.Panel1.Controls.Add(Me.lblListeNo)
        Me.Panel1.Controls.Add(Me.btnCreatePalet)
        Me.Panel1.Controls.Add(Me.btnSend)
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(240, 69)
        '
        'lblEtiketNo
        '
        Me.lblEtiketNo.Location = New System.Drawing.Point(3, 48)
        Me.lblEtiketNo.Name = "lblEtiketNo"
        Me.lblEtiketNo.Size = New System.Drawing.Size(55, 20)
        Me.lblEtiketNo.Text = "Etiket No"
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
        Me.txtListeNo.Visible = False
        '
        'lblListeNo
        '
        Me.lblListeNo.Location = New System.Drawing.Point(3, 8)
        Me.lblListeNo.Name = "lblListeNo"
        Me.lblListeNo.Size = New System.Drawing.Size(52, 20)
        Me.lblListeNo.Text = "Liste No"
        Me.lblListeNo.Visible = False
        '
        'btnCreatePalet
        '
        Me.btnCreatePalet.Location = New System.Drawing.Point(154, 8)
        Me.btnCreatePalet.Name = "btnCreatePalet"
        Me.btnCreatePalet.Size = New System.Drawing.Size(84, 30)
        Me.btnCreatePalet.TabIndex = 6
        Me.btnCreatePalet.Text = "Palet Aç"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(154, 37)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(84, 32)
        Me.btnSend.TabIndex = 7
        Me.btnSend.Text = "Gönder"
        Me.btnSend.Visible = False
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
        Me.Panel3.Controls.Add(Me.btnDelete)
        Me.Panel3.Controls.Add(Me.txtMiktar)
        Me.Panel3.Controls.Add(Me.lblMiktar)
        Me.Panel3.Controls.Add(Me.dtgrdMain)
        Me.Panel3.Controls.Add(Me.btnCancel)
        Me.Panel3.Location = New System.Drawing.Point(0, 70)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(240, 198)
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
        'dtgrdMain
        '
        Me.dtgrdMain.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.dtgrdMain.Location = New System.Drawing.Point(0, 0)
        Me.dtgrdMain.Name = "dtgrdMain"
        Me.dtgrdMain.Size = New System.Drawing.Size(240, 176)
        Me.dtgrdMain.TabIndex = 3
        Me.dtgrdMain.TableStyles.Add(Me.tbSevk)
        '
        'tbSevk
        '

        Me.tbSevk.GridColumnStyles.Add(Me.paletNo)
        Me.tbSevk.GridColumnStyles.Add(Me.SiraNo)
        Me.tbSevk.GridColumnStyles.Add(Me.Malzeme)
        Me.tbSevk.GridColumnStyles.Add(Me.Adet)
        Me.tbSevk.MappingName = "Sevk"
        '
        'txtEtiketNo
        '
        Me.txtEtiketNo.Location = New System.Drawing.Point(64, 48)
        Me.txtEtiketNo.MaxLength = 60
        Me.txtEtiketNo.Name = "txtEtiketNo"
        Me.txtEtiketNo.Size = New System.Drawing.Size(89, 21)
        Me.txtEtiketNo.TabIndex = 0
        '

        'paletNo
        '
        Me.paletNo.Format = ""
        Me.paletNo.FormatInfo = Nothing
        Me.paletNo.HeaderText = "Palet No"
        Me.paletNo.MappingName = "paletNo"
        Me.paletNo.NullText = ""
        Me.paletNo.Width = 65

        'Sýra No
        '
        Me.SiraNo.Format = ""
        Me.SiraNo.FormatInfo = Nothing
        Me.SiraNo.HeaderText = "Sýra No"
        Me.SiraNo.MappingName = "SiraNo"
        Me.SiraNo.NullText = ""
        Me.SiraNo.Width = 65

        'Malzeme
        '
        Me.Malzeme.Format = ""
        Me.Malzeme.FormatInfo = Nothing
        Me.Malzeme.HeaderText = "Malzeme"
        Me.Malzeme.MappingName = "Malzeme"
        Me.Malzeme.NullText = ""
        Me.Malzeme.Width = 75

        'Adet
        '
        Me.Adet.Format = ""
        Me.Adet.FormatInfo = Nothing
        Me.Adet.HeaderText = "Adet"
        Me.Adet.MappingName = "Adet"
        Me.Adet.NullText = ""
        Me.Adet.Width = 30
        '
     
        '
        
        '
      
        '
        'frmPaletIslemleri
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ClientSize = New System.Drawing.Size(240, 271)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmPaletIslemleri"
        Me.Text = "Palet Ýþlemleri"
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
        'dt.Columns.Add("etiketNo")
        'dt.Columns.Add("newCreated")
        'dt.Columns.Add("lot")
        'dt.Columns.Add("Item")
        'dt.Columns.Add("PLTTYPE")
        'dt.Columns.Add("Miktar")
        dt.Columns.Add("PaletNo")
        dt.Columns.Add("SiraNo")
        dt.Columns.Add("Malzeme")
        dt.Columns.Add("Adet")
        '
        ds = New DataSet
        ds.Tables.Add(dt)
        ''
    End Sub
    '
    Private Function GetBarkod() As Boolean

        Try

            If txtEtiketNo.Text.Trim <> "" Then

                etkBilg = New EtiketBilgisi

                If txtListeNo.Text = "" Then

                    MsgBox("Çekme No Tanýmsýz", MsgBoxStyle.Exclamation, "Uyarý")

                    txtEtiketNo.Text = ""

                    Return False

                End If

                RetVal = CheckEtiketNo(txtListeNo.Text, txtEtiketNo.Text, etkBilg)
                '
                If etkBilg.Pkmik = 0 Or etkBilg.Pkod = "" Then

                    MsgBox("Bu ürün paletlenmiyor..", MsgBoxStyle.Exclamation, "Uyarý")

                    txtEtiketNo.Text = ""

                    Return False

                End If


                If etkBilg.Pkmik = ds.Tables("Sevk").Select("paletNo = '1'").Length() Then

                    MsgBox("Bu Palet Dolmuþ Durumda Lütfen Palet i Kapatýp Yeni Palet Açýnýz..", MsgBoxStyle.Exclamation, "Uyarý")

                    txtEtiketNo.Text = ""

                    Return False

                End If
                '
                If RetVal.ReturnValue = False Then

                    MsgBox("Etiket Tanýmsýz", MsgBoxStyle.Exclamation, "Uyarý")

                    txtEtiketNo.Text = ""

                    Return False

                End If
                '
                '
                'L3P Karþýlaþtýrmasý 
                If sL3p = "" Then sL3p = etkBilg.L3P

                If sL3p.Trim <> etkBilg.L3P.Trim Then

                    MsgBox("Kutu bu palete eklenemez ! ", MsgBoxStyle.Exclamation, "Uyarý")

                    txtEtiketNo.Text = ""

                    Return False

                End If
                '
                Return True
            End If
            '
        Catch ex As Exception
            'Clear
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            '
            Return False
        End Try
        ''
    End Function
    '
    '
    Private Sub Add()

        'Cursor.Current = Cursors.WaitCursor
        ''
        'Try

        '    Dim dr As DataRow
        '    'Dim db As New Sevkiyat
        '    '
        '    dr = ds.Tables("Sevk").NewRow
        '    '
        '    dr("listeNo") = txtListeNo.Text
        '    dr("paletNo") = txtPaletNo.Text
        '    dr("etiketNo") = txtEtiketNo.Text
        '    dr("Item") = etkBilg.Itnbr
        '    dr("House") = etkBilg.House
        '    dr("newCreated") = 0
        '    dr("PLTTYPE") = "MONO"

        '    '
        '    ds.Tables("Sevk").Rows.Add(dr)

        '    bindData()

        '    etkBilg = Nothing
        '    '
        '    txtEtiketNo.Text = ""
        '    txtEtiketNo.Focus()

        '    ds.Tables("Sevk").DefaultView.Sort = "sirano desc"

        'Catch ex As Exception

        '    MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")

        'End Try
        ''
        'Cursor.Current = Cursors.Default
    End Sub
    '
    Private Sub bindData()
        '
        With dtgrdMain
            .DataSource = ds.Tables(0)
            '
        End With
        '
        txtMiktar.Text = (ds.Tables("Sevk").Rows.Count)
    End Sub
    '
    Private Sub SendData(Optional ByVal bKapat As Boolean = False)



        Try

            If Not palet Then

                If Not (ds Is Nothing) And _
                (ds.Tables(0).Rows.Count >= 1) Then
                    '

                    Dim ds1 As New DataSet

                    ds1 = ds.Clone

                    ds1.Tables("Sevk").Clear()
                    '
                    For Each dr As DataRow In ds.Tables("Sevk").Rows
                        '
                        Dim dr1 As DataRow
                        '
                        dr1 = ds1.Tables("Sevk").NewRow
                        '
                        dr1("PaletNo") = dr("PaletNo")
                        dr1("SiraNo") = dr("SiraNo")
                        dr1("Malzeme") = dr("Malzeme")
                        dr1("Adet") = dr("Adet")
                        '

                        ds1.Tables("Sevk").Rows.Add(dr1)


                        'UpdatePalet(dr("etiketNo"), dr("paletNo"))


                    Next


                    txtEtiketNo.Text = ""
                    txtListeNo.Text = ""
                    txtPaletNo.Text = ""
                    Qty = 0

                    palet = False

                    dtgrdMain.DataSource = Nothing
                    '
                    CreateTable()
                    '
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

            Else

                MsgBox("Paleti kapatmadan gönderme yapamazsýnýz!" + MsgBoxStyle.Exclamation, "Uyarý")

            End If


        Catch ex As Exception
            '
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")
            '
        End Try

    End Sub
    '

    Private Sub DeletePalet(ByVal ListNo As Integer)

        Try
            '
            '
            Sorgu = " Update ETIKETDTY" & _
                        " Set PLTNO=0" & _
                        " Where Pickno=" & ListNo

            srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)

            Sorgu = " Delete From ETIKETDTY" & _
                        " Where PickNo=" & ListNo & _
                        " And PLTETK=1"

            srv.RunSql(Sorgu, True, Mobile.ProviderTypes.SqlClient)

            '
        Catch ex As Exception
            '
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")
            '

        End Try

    End Sub

    Private Function UpdatePalet(ByVal etiketNo As String, _
            ByVal paletNo As String) As Boolean
        Try
            '
            '
            Return UpdatePaletNo(etiketNo, paletNo) '.ReturnValue
            '
        Catch ex As Exception
            '
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")
            '
            Return False
        End Try
        ''
    End Function
    '
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
                            txtPaletNo.Text = SeriNoAl("ETK")

                            sPaletNo = txtPaletNo.Text
                            '
                            For Each dr As DataRow In ds.Tables("Sevk").Select("paletNo = '1'")
                                '
                                dr("paletNo") = txtPaletNo.Text

                                dr("newCreated") = 1
                                '
                            Next
                            '
                            bindData()
                            '
                            palet = False

                            txtPaletNo.Text = ""

                            btnCreatePalet.Text = "Palet Aç"

                        Else

                            If Message("PaletMalzemeYok") = MsgBoxResult.Yes Then

                                palet = False

                                txtPaletNo.Text = ""

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
    Private Sub txtListeNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtListeNo.KeyDown
        '
        'If e.KeyCode = Keys.Enter Then
        '    '
        '    Cursor.Current = Cursors.WaitCursor
        '    '
        '    If txtListeNo.Text.Length > 0 Then

        '        txtListeNo.Text = IIf((txtListeNo.Text.ToUpper.Substring(0, 1).ToUpper = "L"), _
        '                            txtListeNo.Text.Remove(0, 1), txtListeNo.Text)

        '        txtListeNo.Text = Int(txtListeNo.Text)


        '        'Sorgu = "select ETKPAL" & _
        '        '            " from plantprm p " & _
        '        '            " inner Join " & _
        '        '                " (" & _
        '        '                        " Select Distinct C6CANB, C6B9CD " & _
        '        '                                " FRom trceklist " & _
        '        '                                " Where Ceklist = " & txtListeNo.Text & _
        '        '                  " Group By C6CANB, C6B9CD" & _
        '        '                " ) T" & _
        '        '                 " On T.C6CANB = p.CANB " & _
        '        '                 " and T.C6B9CD = p.B9CD "


        '        'dt = db.RunSql(Sorgu)

        '        'If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

        '        '    If dt.Rows(0).Item(0).ToString = "1" Then

        '        '        MessageBox.Show(txtListeNo.Text & " Nolu Çekmenin Paletleme Paremetreleri Otomatik Olarak Ayarlandý.., Buradan Paletleme Yapamazsýnýz...")

        '        '        txtListeNo.Text = ""



        '        '        Exit Sub

        '        '    End If

        '        'End If


        '        '
        '        Try

        '            RetVal = CheckPickNo(txtListeNo.Text)
        '            '


        '            'If PaletKontrol(txtListeNo.Text) = False Then
        '            '    txtListeNo.Text = ""
        '            '    txtListeNo.Focus()
        '            '    Exit Sub

        '            'End If

        '            If RetVal.ReturnValue = False Then

        '                MsgBox(RetVal.GetMessages, MsgBoxStyle.Exclamation, "Uyarý")

        '                txtListeNo.Text = ""

        '                QtyMax = 0

        '                Exit Sub

        '            End If

        '            QtyMax = RetVal.iInfo

        '            'Barkodlu Sevkiyat baþýnda çekmenin paletlerini sýfýrlayýp tüm kutularý harici kutuya çeviriyoruz.

        '            'Sorgu = " Delete From Etiketdty" & _
        '            '            " Where Pickno=" & txtListeNo.Text & _
        '            '            " And PltEtk=1"

        '            'db.RunSql(Sorgu)

        '            'Sorgu = " Update Etiketdty" & _
        '            '            " Set Pltno=-1" & _
        '            '            " Where Pickno=" & txtListeNo.Text

        '            'db.RunSql(Sorgu)
        '            '
        '        Catch ex As Exception

        '            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")

        '            txtListeNo.Text = ""

        '            Exit Sub

        '        End Try
        '        '
        '        txtEtiketNo.Text = ""

        '        txtEtiketNo.Focus()

        '    End If
        '    '
        '    Cursor.Current = Cursors.Default

        'End If

    End Sub
    '
#End Region

    Private Sub btnCreatePalet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreatePalet.Click
        '
        Cursor.Current = Cursors.WaitCursor
        If Not palet Then
            '
            palet = 1
            txtPaletNo.Text = 1
            btnCreatePalet.Text = "Palet Kapat"
            sL3p = ""
            '
            SendData()
        ElseIf palet Then
            '
            Qty = ds.Tables("Sevk").Select("paletNo = '1'").Length()
            '
            If Not (Qty = 0) Then

                Dim sPaletNo As String
                '
                txtPaletNo.Text = SeriNoAl("ETK")
                sPaletNo = txtPaletNo.Text
                '
                'Palet tipi ayarlanýyor
                Dim sItem As String = ""
                Dim iItemQty As Int16 = 0
                For Each dr As DataRow In ds.Tables("Sevk").Select("paletNo = '1'")
                    '
                    If sItem <> dr("Item") Then
                        iItemQty += 1
                        sItem = dr("Item")
                    End If
                Next
                '
                Dim sPltType As String = ""
                sPltType = IIf(iItemQty <= 1, "MONO", "MIXED")
                'For Each dr As DataRow In ds.Tables("Sevk").Select("paletNo = '1'")
                '    '
                '    dr("paletNo") = txtPaletNo.Text
                '    dr("newCreated") = 1
                '    dr("PLTTYPE") = sPltType
                'Next
                '
                bindData()
                '
                palet = False
                txtPaletNo.Text = ""
                btnCreatePalet.Text = "Palet Aç"
            Else
                If Message("PaletMalzemeYok") = MsgBoxResult.Yes Then
                    palet = False
                    txtPaletNo.Text = ""
                    btnCreatePalet.Text = "Palet Aç"
                End If
            End If
        End If
        '
        txtEtiketNo.Text = ""
        txtEtiketNo.Focus()
        Cursor.Current = Cursors.Default
    End Sub
    '
    Private Sub txtEtiketNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtEtiketNo.KeyDown

        'Dim Malzeme As String = ""
        'Dim Lot As String = ""
        'Dim Miktar As String = "0"
        'Dim Etiket As String()

        'Try

        '    If e.KeyCode = Keys.Enter Then

        '        Cursor.Current = Cursors.WaitCursor

        '        If txtEtiketNo.Text.Length > 0 Then

        '            txtEtiketNo.Text = IIf(txtEtiketNo.Text.ToUpper.StartsWith("S"), txtEtiketNo.Text.Remove(0, 1), txtEtiketNo.Text)

        '            If Contains(txtEtiketNo.Text, "%") Then

        '                Etiket = txtEtiketNo.Text.Split("%")

        '                Malzeme = Etiket(0)

        '                Lot = Etiket(1)

        '                If Etiket.Length > 2 Then

        '                    Miktar = Etiket(2)

        '                End If

        '            End If


        '            'Dim cnt As Integer = 0
        '            'Dim toplamMiktar As Double = 0
        '            'Dim foundRows() As DataRow

        '            '
        '            'foundRows = ds.Tables("Sevk").Select("Item = '" & Malzeme & "'")
        '            '
        '            'For cnt = 0 To foundRows.GetUpperBound(0)

        '            'toplamMiktar += foundRows(cnt)("Miktar")

        '            'Next cnt

        '            'Sorgu = "Select Sum(KMIK) As KMIK " & _
        '            '" From ETIKETDTY" & _
        '            '" Where PICKNO=" & txtListeNo.Text & _
        '            '" and ITNBR=" & sTirnakEkle(Malzeme) & _
        '            '" and KUTUETK=1"

        '            'dt = srv.RunSqlDs(Sorgu, "ETIKETDTY", Mobile.ProviderTypes.SqlClient).Tables(0)

        '            'If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

        '            'toplamMiktar += dt.Rows(0).Item("KMIK")

        '            'Else

        '            'MessageBox.Show("Etiket Okuma Sýrasýnda Hata Oluþtu...", "Ekip Mapics")

        '            'Exit Sub

        '            'End If

        '            'Sorgu = "Select Sum(ADAQQT) As ADAQQT " & _
        '            '            " From TRCeklist" & _
        '            '            " Where Ceklist=" & txtListeNo.Text & _
        '            '            " and ADAITX=" & sTirnakEkle(Malzeme)

        '            'dt = srv.RunSqlDs(Sorgu, "TRCeklist", Mobile.ProviderTypes.SqlClient).Tables(0)

        '            'If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

        '            'If Miktar + toplamMiktar > dt.Rows(0).Item("ADAQQT") Then

        '            'MessageBox.Show("Çekme miktarýndan fazla etiket okutamazsýnýz...", "Ekip Mapics")

        '            'Exit Sub

        '            'End If

        '            'Else

        '            'MessageBox.Show("Etiket Okuma Sýrasýnda Hata Oluþtu...", "Ekip Mapics")

        '            'Exit Sub

        '            'End If

        '            'Dim ht As New Hashtable

        '            'Dim htout As New Hashtable

        '            'ht.Clear()
        '            'ht.Add("@CountField", "ETK")
        '            'ht.Add("@Inc", 1)

        '            'htout.Clear()
        '            'htout.Add("@Value", 0)

        '            'srv.RunSp("Tr_GetSeqNo", ht, htout)

        '            Sorgu = " declare @p1 numeric(10, 0) " & _
        '                    " exec [dbo].[Tr_GetSeqNo] @CountField = N'ETK'," & _
        '                    "    @Inc =1 , @Value= @p1 output " & _
        '                    " select  @p1"

        '            dt = srv.RunSqlDs(Sorgu, "Tr_GetSeqNo", Mobile.ProviderTypes.SqlClient).Tables(0)

        '            Dim EtiketNo As String = String.Empty

        '            If Not dt Is Nothing AndAlso _
        '                dt.Rows.Count > 0 Then

        '                EtiketNo = dt.Rows(0)(0).ToString

        '            End If

        '            Dim dr As DataRow
        '            'Dim db As New Sevkiyat
        '            '
        '            dr = ds.Tables("Sevk").NewRow
        '            '
        '            dr("ListeNo") = txtListeNo.Text
        '            dr("PaletNo") = txtPaletNo.Text
        '            dr("EtiketNo") = EtiketNo 'htout.Item("@Value")
        '            dr("Item") = Malzeme
        '            dr("Lot") = Lot
        '            dr("Miktar") = Miktar
        '            dr("NewCreated") = 0
        '            dr("PltType") = "MONO"
        '            '
        '            ds.Tables("Sevk").Rows.Add(dr)

        '            bindData()
        '            '
        '            txtEtiketNo.Text = ""

        '            txtEtiketNo.Focus()

        '            ds.Tables("Sevk").DefaultView.Sort = "etiketNo desc"

        '        End If

        '    End If
        '    '
        '    Cursor.Current = Cursors.Default

        'Catch ex As Exception

        '    Cursor.Current = Cursors.Default

        '    MessageBox.Show(ex.Message, "Ekip Mapics")

        'End Try

    End Sub

    Private Sub frmSevkiyat_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        srv.Url = ReadConfig("path")
        Cursor.Current = Cursors.WaitCursor

        '
        Me.Text = Me.Text & " "
        CreateTable()
        Cursor.Current = Cursors.Default
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
        Close()
        frmMenu.Visible = True
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        '
        If Message("DeleteQuestion") = MsgBoxResult.Yes Then
            Cursor.Current = Cursors.WaitCursor
            '
            If ds.Tables("Sevk").Rows.Count > 0 Then
                '
                'Dim lPaletNo As Long
                'lPaletNo = Long.Parse("0" & ds.Tables("Sevk").Rows(dtgrdMain.CurrentRowIndex)("PaletNo"))
                ''
                'If lPaletNo <> 0 And lPaletNo <> 1 Then
                '    '
                '    If Not UpdatePalet(False, lPaletNo) Then
                '        '
                '        Message("SaveNotCompleted")
                '        '
                '    End If
                'End If
                '
                'ds.Tables("Sevk").Rows.RemoveAt(dtgrdMain.CurrentRowIndex)

                For Each row As DataRow In ds.Tables("Sevk").Rows
                    If row("etiketNo").ToString = dtgrdMain.Item(dtgrdMain.CurrentRowIndex, 2).ToString Then
                        row.Delete()
                        Exit For
                    End If
                Next
                '
                bindData()
                '
            End If
            '            
            Cursor.Current = Cursors.Default
        End If
    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click

    End Sub

    Private Function PaletKontrol(ByVal PickNo As String) As Boolean

        Try

            Sorgu = "Select 1" & _
                        " From Veri_Toplama.dbo.[SYTELINEPALETLEME] " & _
                        " Where sirano =" & txtEtiketNo.Text & _
                        " And isnull(palet_no,0)<>0"

            dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

            If Not dt Is Nothing AndAlso _
                dt.Rows.Count > 0 Then

                MessageBox.Show(PickNo & " nolu etiket daha önce paletlenmiþ...", "Ekip Mapics")

                PaletKontrol = False

            Else

                PaletKontrol = True

            End If

        Catch ex As Exception

            MessageBox.Show("Ýþlem Gerçekleþtirilemedi" & vbNewLine & "Hata...:" & ex.Message, "Ekip Mapics")

        End Try

    End Function


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
    '
    '
    'Belirtilen EtiketNo belirtilen listeye ait Olup olmadýðý kontrol edilir
    Public Function CheckEtiketNo( _
                ByVal PickNo As Integer, _
                ByVal EtiketNo As String, _
                ByRef EtiketBilgisi As EtiketBilgisi) As ReturnValue
        '


        '
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

    'Belirtilen EtiketNo belirtilen listeye ait Olup olmadýðý kontrol edilir
    Public Function CheckEtiketNoxxx( _
                ByVal PickNo As Integer, _
                ByVal EtiketNo As String) As ReturnValue
        '


        '
        Try


            '
            dt = srv.RunSqlDs( _
                   "Select PICKNO, ETKSERINO, HOUSE, ITNBR,KMiK ,ORDNO, ORDSEQ, trim(l3p) || trim(hndcode) L3P  " & _
                       "From ETIKETDTY " & _
                       "Where PICKNO = " & PickNo.ToString & _
                            " And EtkSeriNo = '" & EtiketNo & "' ", "ETIKETDTY", Mobile.ProviderTypes.SqlClient).Tables(0)

            If Not (dt Is Nothing) Then
                With dt
                    If .Rows.Count > 0 Then
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


    '
    'ETÝKET NO ONAY -- Etikete ait ETÝKETNOONAY deðerini getirir.
    '

    'Belirtilen Malzeme Ambarda ne kadar olduðu kontrol edilir
    '
    'Sistemden yeni bir etiketno üretilir ve getirilir
    Public Function GetEtiketNo() As String
        '


        Dim sEtiketNo As String
        Dim iEtiketNo As Integer
        '
        Try


            '
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
            'Else
            'Me.ReturnValue.ReturnValue = False
            'Return ""
            'End If
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
    '
    '
    'Sevkiyat tamamdýðýnda EtiketDty Tablosundaki PltNo alaný güncellenecek
    Public Function UpdatePaletNo(ByVal EtiketNo As String, _
        ByVal PaletNo As String) As Boolean 'ReturnValue
        '


        '
        Try


            '
            srv.RunSql( _
                    "Update Veri_Toplama.dbo.[SYTELINEPALETLEME]  " & _
                        "Set palet_no = '" & PaletNo & "'" & _
                        "Where sirano = '" & EtiketNo & "' ", True, Mobile.ProviderTypes.SqlClient)

            Return True

            'If srv.Result.ReturnValue Then
            '  '
            '  Return srv.Result

            'End If
            '
        Catch ex As Exception
            '
            'Me.ReturnValue.Add(ex.Message)
            'Me.ReturnValue.ReturnValue = False
            '
            'Return srv.Result
            Return False
        Finally

        End Try
        ''
    End Function
    '

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

    Private Sub txtListeNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtListeNo.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then
            '
            Cursor.Current = Cursors.WaitCursor
            '
            If txtListeNo.Text.Length > 0 Then

                txtListeNo.Text = IIf((txtListeNo.Text.ToUpper.Substring(0, 1).ToUpper = "L"), _
                                    txtListeNo.Text.Remove(0, 1), txtListeNo.Text)

                txtListeNo.Text = Int(txtListeNo.Text)


                'Sorgu = "select ETKPAL" & _
                '            " from plantprm p " & _
                '            " inner Join " & _
                '                " (" & _
                '                        " Select Distinct C6CANB, C6B9CD " & _
                '                                " FRom trceklist " & _
                '                                " Where Ceklist = " & txtListeNo.Text & _
                '                  " Group By C6CANB, C6B9CD" & _
                '                " ) T" & _
                '                 " On T.C6CANB = p.CANB " & _
                '                 " and T.C6B9CD = p.B9CD "


                'dt = db.RunSql(Sorgu)

                'If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                '    If dt.Rows(0).Item(0).ToString = "1" Then

                '        MessageBox.Show(txtListeNo.Text & " Nolu Çekmenin Paletleme Paremetreleri Otomatik Olarak Ayarlandý.., Buradan Paletleme Yapamazsýnýz...")

                '        txtListeNo.Text = ""



                '        Exit Sub

                '    End If

                'End If


                '
                Try

                    RetVal = CheckPickNo(txtListeNo.Text)
                    '


                    'If PaletKontrol(txtListeNo.Text) = False Then
                    '    txtListeNo.Text = ""
                    '    txtListeNo.Focus()
                    '    Exit Sub

                    'End If

                    If RetVal.ReturnValue = False Then

                        MsgBox(RetVal.GetMessages, MsgBoxStyle.Exclamation, "Uyarý")

                        txtListeNo.Text = ""

                        QtyMax = 0

                        Exit Sub

                    End If

                    QtyMax = RetVal.iInfo

                    'Barkodlu Sevkiyat baþýnda çekmenin paletlerini sýfýrlayýp tüm kutularý harici kutuya çeviriyoruz.

                    'Sorgu = " Delete From Etiketdty" & _
                    '            " Where Pickno=" & txtListeNo.Text & _
                    '            " And PltEtk=1"

                    'db.RunSql(Sorgu)

                    'Sorgu = " Update Etiketdty" & _
                    '            " Set Pltno=-1" & _
                    '            " Where Pickno=" & txtListeNo.Text

                    'db.RunSql(Sorgu)
                    '
                Catch ex As Exception

                    MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Hata")

                    txtListeNo.Text = ""

                    Exit Sub

                End Try
                '
                txtEtiketNo.Text = ""

                txtEtiketNo.Focus()

            End If
            '
            Cursor.Current = Cursors.Default

        End If
    End Sub

    Private Sub txtEtiketNo_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtEtiketNo.KeyPress

        Try

            If e.KeyChar = Microsoft.VisualBasic.Chr(13) Then

                Cursor.Current = Cursors.WaitCursor


                If txtEtiketNo.Text.Length > 0 Then

                    'Etiket = txtEtiketNo.Text
                    'txtEtiketNo.Text = IIf(txtEtiketNo.Text.ToUpper.StartsWith("S"), txtEtiketNo.Text.Remove(0, 1), txtEtiketNo.Text)

                    'If Contains(txtEtiketNo.Text, "%") Then

                    'Etiket = txtEtiketNo.Text.Split("%")

                    '    Malzeme = Etiket(0)

                    '    Lot = Etiket(1)

                    '    If Etiket.Length > 2 Then

                    '        Miktar = Etiket(2)

                    '    End If

                    'End If


                    'Sorgu = " declare @p1 numeric(10, 0) " & _
                    '        " exec [dbo].[Tr_GetSeqNo] @CountField = N'ETK'," & _
                    '        "    @Inc =1 , @Value= @p1 output " & _
                    '        " select  @p1"

                    'dt = srv.RunSqlDs(Sorgu, "Tr_GetSeqNo", Mobile.ProviderTypes.SqlClient).Tables(0)

                    'Dim EtiketNo As String = String.Empty

                    'If Not dt Is Nothing AndAlso _
                    '    dt.Rows.Count > 0 Then

                    '    EtiketNo = dt.Rows(0)(0).ToString

                    'End If

                    Dim dt As DataTable

                    Sorgu = "SELECT * FROM Veri_Toplama.dbo.[SYTELINEPALETLEME] WHERE sirano ='" & txtEtiketNo.Text & "'"
                    dt = srv.RunSqlDs(Sorgu, "SYTELINEPALETLEME", Mobile.ProviderTypes.SqlClient).Tables(0)

                    If dt.Rows.Count > 0 Then
                        Dim dr As DataRow
                        dr = ds.Tables("Sevk").NewRow
                        dr("PaletNo") = "1"
                        dr("SiraNo") = dt.Rows(0)("sirano")
                        dr("Malzeme") = dt.Rows(0)("malzeme")
                        dr("Adet") = dt.Rows(0)("adet")

                        ds.Tables("Sevk").Rows.Add(dr)

                        bindData()
                        '
                        txtEtiketNo.Text = ""

                        txtEtiketNo.Focus()

                        ds.Tables("Sevk").DefaultView.Sort = "sirano desc"

                    Else
                        MessageBox.Show("Etiket numarasý bulunamadý. Tekrar deneyiniz.")
                        Exit Sub
                    End If

                End If

            End If
            '
            Cursor.Current = Cursors.Default

        Catch ex As Exception

            Cursor.Current = Cursors.Default

            MessageBox.Show(ex.Message, "Ekip Mapics")

        End Try
    End Sub
End Class
