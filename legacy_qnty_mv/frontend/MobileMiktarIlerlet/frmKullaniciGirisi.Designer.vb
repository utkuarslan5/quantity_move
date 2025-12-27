<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmKullaniciGirisi
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.btnGiris = New System.Windows.Forms.Button
        Me.txtSifre = New System.Windows.Forms.TextBox
        Me.txtKullanici = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnCancel = New System.Windows.Forms.Button
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnCancel)
        Me.Panel1.Controls.Add(Me.btnGiris)
        Me.Panel1.Controls.Add(Me.txtSifre)
        Me.Panel1.Controls.Add(Me.txtKullanici)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Location = New System.Drawing.Point(3, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(234, 278)
        '
        'btnGiris
        '
        Me.btnGiris.Location = New System.Drawing.Point(142, 163)
        Me.btnGiris.Name = "btnGiris"
        Me.btnGiris.Size = New System.Drawing.Size(72, 20)
        Me.btnGiris.TabIndex = 9
        Me.btnGiris.Text = "Giriş"
        '
        'txtSifre
        '
        Me.txtSifre.Location = New System.Drawing.Point(114, 114)
        Me.txtSifre.Name = "txtSifre"
        Me.txtSifre.Size = New System.Drawing.Size(100, 21)
        Me.txtSifre.TabIndex = 8
        '
        'txtKullanici
        '
        Me.txtKullanici.Location = New System.Drawing.Point(114, 80)
        Me.txtKullanici.Name = "txtKullanici"
        Me.txtKullanici.Size = New System.Drawing.Size(100, 21)
        Me.txtKullanici.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(20, 114)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 20)
        Me.Label2.Text = "Şifre"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(20, 80)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 20)
        Me.Label1.Text = "Kullanıcı Adı"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(65, 163)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(71, 20)
        Me.btnCancel.TabIndex = 82
        Me.btnCancel.Text = "Kapat"
        '
        'frmKullaniciGirisi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmKullaniciGirisi"
        Me.Text = "Kullanıcı Girişi"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnGiris As System.Windows.Forms.Button
    Friend WithEvents txtSifre As System.Windows.Forms.TextBox
    Friend WithEvents txtKullanici As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
