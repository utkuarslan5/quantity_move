<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmStokSorgulama
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
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtYer = New System.Windows.Forms.TextBox
        Me.txtItem = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.DataGrid1 = New System.Windows.Forms.DataGrid
        Me.DataGridTableStyle1 = New System.Windows.Forms.DataGridTableStyle
        Me.item = New System.Windows.Forms.DataGridTextBoxColumn
        Me.lot = New System.Windows.Forms.DataGridTextBoxColumn
        Me.loc = New System.Windows.Forms.DataGridTextBoxColumn
        Me.qty_on_hand = New System.Windows.Forms.DataGridTextBoxColumn
        Me.FIFO = New System.Windows.Forms.DataGridTextBoxColumn
        Me.btnClear = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.txtDescription = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(9, 55)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(78, 20)
        Me.Label7.Text = "Yer"
        '
        'txtYer
        '
        Me.txtYer.Location = New System.Drawing.Point(92, 52)
        Me.txtYer.Name = "txtYer"
        Me.txtYer.Size = New System.Drawing.Size(126, 21)
        Me.txtYer.TabIndex = 87
        '
        'txtItem
        '
        Me.txtItem.Location = New System.Drawing.Point(92, 8)
        Me.txtItem.Name = "txtItem"
        Me.txtItem.Size = New System.Drawing.Size(126, 21)
        Me.txtItem.TabIndex = 86
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(9, 10)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(100, 20)
        Me.Label3.Text = "Malzeme"
        '
        'DataGrid1
        '
        Me.DataGrid1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.DataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.DataGrid1.Location = New System.Drawing.Point(0, 78)
        Me.DataGrid1.Name = "DataGrid1"
        Me.DataGrid1.Size = New System.Drawing.Size(240, 150)
        Me.DataGrid1.TabIndex = 89
        Me.DataGrid1.TableStyles.Add(Me.DataGridTableStyle1)
        '
        'DataGridTableStyle1
        '
        Me.DataGridTableStyle1.GridColumnStyles.Add(Me.item)
        Me.DataGridTableStyle1.GridColumnStyles.Add(Me.lot)
        Me.DataGridTableStyle1.GridColumnStyles.Add(Me.loc)
        Me.DataGridTableStyle1.GridColumnStyles.Add(Me.qty_on_hand)
        Me.DataGridTableStyle1.GridColumnStyles.Add(Me.FIFO)
        Me.DataGridTableStyle1.MappingName = "lot_loc"
        '
        'item
        '
        Me.item.Format = ""
        Me.item.FormatInfo = Nothing
        Me.item.HeaderText = "Malzeme"
        Me.item.MappingName = "item"
        Me.item.Width = 75
        '
        'lot
        '
        Me.lot.Format = ""
        Me.lot.FormatInfo = Nothing
        Me.lot.HeaderText = "Lot"
        Me.lot.MappingName = "lot"
        Me.lot.Width = 90
        '
        'loc
        '
        Me.loc.Format = ""
        Me.loc.FormatInfo = Nothing
        Me.loc.HeaderText = "Yer"
        Me.loc.MappingName = "loc"
        Me.loc.Width = 40
        '
        'qty_on_hand
        '
        Me.qty_on_hand.Format = ""
        Me.qty_on_hand.FormatInfo = Nothing
        Me.qty_on_hand.HeaderText = "Eldeki Miktar"
        Me.qty_on_hand.MappingName = "qty_on_hand"
        Me.qty_on_hand.Width = 30
        '
        'FIFO
        '
        Me.FIFO.Format = ""
        Me.FIFO.FormatInfo = Nothing
        Me.FIFO.HeaderText = "FIFO"
        Me.FIFO.MappingName = "FIFO"
        Me.FIFO.Width = 40
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(54, 4)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(56, 30)
        Me.btnClear.TabIndex = 92
        Me.btnClear.Text = "Temizle"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(123, 4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(56, 30)
        Me.btnCancel.TabIndex = 93
        Me.btnCancel.Text = "Kapat"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnCancel)
        Me.Panel1.Controls.Add(Me.btnClear)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 234)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(240, 60)
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.txtDescription)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.txtItem)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.txtYer)
        Me.Panel2.Controls.Add(Me.Label7)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(240, 76)
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(92, 30)
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(126, 21)
        Me.txtDescription.TabIndex = 90
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(9, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 20)
        Me.Label1.Text = "Tanım"
        '
        'frmStokSorgulama
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.DataGrid1)
        Me.KeyPreview = True
        Me.Name = "frmStokSorgulama"
        Me.Text = "frmStokSorgulama"
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtYer As System.Windows.Forms.TextBox
    Friend WithEvents txtItem As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents DataGrid1 As System.Windows.Forms.DataGrid
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents DataGridTableStyle1 As System.Windows.Forms.DataGridTableStyle
    Friend WithEvents item As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents lot As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents loc As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents qty_on_hand As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents FIFO As System.Windows.Forms.DataGridTextBoxColumn
    Friend WithEvents txtDescription As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
