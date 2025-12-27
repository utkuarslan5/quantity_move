<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmLotTakibi
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
    Private mainMenu1 As System.Windows.Forms.MainMenu

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.mainMenu1 = New System.Windows.Forms.MainMenu
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.btnClear = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnSend = New System.Windows.Forms.Button
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.txtJob = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.btnX = New System.Windows.Forms.Button
        Me.txtSira = New System.Windows.Forms.TextBox
        Me.txtTalimat = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtLot = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtTanim = New System.Windows.Forms.TextBox
        Me.txtMalzemeKodu = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.btnClear)
        Me.Panel2.Controls.Add(Me.btnCancel)
        Me.Panel2.Controls.Add(Me.btnSend)
        Me.Panel2.Location = New System.Drawing.Point(4, 179)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(233, 35)
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(11, 3)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(56, 30)
        Me.btnClear.TabIndex = 17
        Me.btnClear.Text = "Temizle"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(85, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(56, 30)
        Me.btnCancel.TabIndex = 18
        Me.btnCancel.Text = "Kapat"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(159, 3)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(56, 30)
        Me.btnSend.TabIndex = 19
        Me.btnSend.Text = "Kaydet"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Panel4)
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.txtLot)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.txtTanim)
        Me.Panel1.Controls.Add(Me.txtMalzemeKodu)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Location = New System.Drawing.Point(3, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(233, 170)
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.txtJob)
        Me.Panel4.Controls.Add(Me.Label6)
        Me.Panel4.Location = New System.Drawing.Point(4, 62)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(226, 31)
        '
        'txtJob
        '
        Me.txtJob.Location = New System.Drawing.Point(92, 5)
        Me.txtJob.Name = "txtJob"
        Me.txtJob.Size = New System.Drawing.Size(112, 21)
        Me.txtJob.TabIndex = 21
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(11, 6)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(86, 20)
        Me.Label6.Text = "2  İş Emri"
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.btnX)
        Me.Panel3.Controls.Add(Me.txtSira)
        Me.Panel3.Controls.Add(Me.txtTalimat)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Location = New System.Drawing.Point(3, 3)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(227, 57)
        '
        'btnX
        '
        Me.btnX.Location = New System.Drawing.Point(202, 6)
        Me.btnX.Name = "btnX"
        Me.btnX.Size = New System.Drawing.Size(21, 21)
        Me.btnX.TabIndex = 109
        Me.btnX.Text = "x"
        '
        'txtSira
        '
        Me.txtSira.Location = New System.Drawing.Point(90, 28)
        Me.txtSira.Name = "txtSira"
        Me.txtSira.Size = New System.Drawing.Size(112, 21)
        Me.txtSira.TabIndex = 8
        '
        'txtTalimat
        '
        Me.txtTalimat.Location = New System.Drawing.Point(90, 6)
        Me.txtTalimat.Name = "txtTalimat"
        Me.txtTalimat.Size = New System.Drawing.Size(112, 21)
        Me.txtTalimat.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(12, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(86, 20)
        Me.Label2.Text = "    Sıra"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(11, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(86, 20)
        Me.Label1.Text = "1  Talimat No"
        '
        'txtLot
        '
        Me.txtLot.Location = New System.Drawing.Point(52, 141)
        Me.txtLot.Name = "txtLot"
        Me.txtLot.Size = New System.Drawing.Size(166, 21)
        Me.txtLot.TabIndex = 12
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(14, 144)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(86, 20)
        Me.Label5.Text = "Lot"
        '
        'txtTanim
        '
        Me.txtTanim.Enabled = False
        Me.txtTanim.Location = New System.Drawing.Point(52, 119)
        Me.txtTanim.Name = "txtTanim"
        Me.txtTanim.ReadOnly = True
        Me.txtTanim.Size = New System.Drawing.Size(166, 21)
        Me.txtTanim.TabIndex = 10
        '
        'txtMalzemeKodu
        '
        Me.txtMalzemeKodu.Enabled = False
        Me.txtMalzemeKodu.Location = New System.Drawing.Point(52, 97)
        Me.txtMalzemeKodu.Name = "txtMalzemeKodu"
        Me.txtMalzemeKodu.ReadOnly = True
        Me.txtMalzemeKodu.Size = New System.Drawing.Size(166, 21)
        Me.txtMalzemeKodu.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(14, 121)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(44, 20)
        Me.Label4.Text = "Tanım"
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(14, 97)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(86, 20)
        Me.Label3.Text = "MLZ"
        '
        'frmLotTakibi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 268)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Menu = Me.mainMenu1
        Me.Name = "frmLotTakibi"
        Me.Text = "Lot Takibi"
        Me.Panel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtTanim As System.Windows.Forms.TextBox
    Friend WithEvents txtMalzemeKodu As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtLot As System.Windows.Forms.TextBox
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents txtJob As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents txtSira As System.Windows.Forms.TextBox
    Friend WithEvents txtTalimat As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnX As System.Windows.Forms.Button
End Class
