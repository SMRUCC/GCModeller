Namespace TabPages

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class StartPage
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose( disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(StartPage))
            Dim Checkbox1 As Microsoft.VisualBasic.MolkPlusTheme.Visualise.Elements.Checkbox = New Microsoft.VisualBasic.MolkPlusTheme.Visualise.Elements.Checkbox()
            Dim Checkbox2 As Microsoft.VisualBasic.MolkPlusTheme.Visualise.Elements.Checkbox = New Microsoft.VisualBasic.MolkPlusTheme.Visualise.Elements.Checkbox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
            Me.LinkLabel2 = New System.Windows.Forms.LinkLabel()
            Me.LinkLabel3 = New System.Windows.Forms.LinkLabel()
            Me.PictureBox1 = New System.Windows.Forms.PictureBox()
            Me.KeepsCheckbox = New Microsoft.VisualBasic.MolkPlusTheme.Windows.Forms.Controls.Checkbox()
            Me.ShowCheckbox = New Microsoft.VisualBasic.MolkPlusTheme.Windows.Forms.Controls.Checkbox()
            Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
            Me.Label1.ForeColor = System.Drawing.Color.White
            Me.Label1.Location = New System.Drawing.Point(40, 300)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(62, 21)
            Me.Label1.TabIndex = 2
            Me.Label1.Text = "Recent"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
            Me.Label2.ForeColor = System.Drawing.Color.White
            Me.Label2.Location = New System.Drawing.Point(40, 147)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(46, 21)
            Me.Label2.TabIndex = 3
            Me.Label2.Text = "Start"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("微软雅黑", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
            Me.Label3.ForeColor = System.Drawing.Color.White
            Me.Label3.Location = New System.Drawing.Point(40, 66)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(167, 35)
            Me.Label3.TabIndex = 4
            Me.Label3.Text = "GCModeller"
            '
            'LinkLabel1
            '
            Me.LinkLabel1.ActiveLinkColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
            Me.LinkLabel1.AutoSize = True
            Me.LinkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline
            Me.LinkLabel1.LinkColor = System.Drawing.Color.SteelBlue
            Me.LinkLabel1.Location = New System.Drawing.Point(40, 180)
            Me.LinkLabel1.Name = "LinkLabel1"
            Me.LinkLabel1.Size = New System.Drawing.Size(93, 19)
            Me.LinkLabel1.TabIndex = 5
            Me.LinkLabel1.TabStop = True
            Me.LinkLabel1.Text = "New Project..."
            '
            'LinkLabel2
            '
            Me.LinkLabel2.ActiveLinkColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
            Me.LinkLabel2.AutoSize = True
            Me.LinkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline
            Me.LinkLabel2.LinkColor = System.Drawing.Color.SteelBlue
            Me.LinkLabel2.Location = New System.Drawing.Point(40, 230)
            Me.LinkLabel2.Name = "LinkLabel2"
            Me.LinkLabel2.Size = New System.Drawing.Size(261, 19)
            Me.LinkLabel2.TabIndex = 6
            Me.LinkLabel2.TabStop = True
            Me.LinkLabel2.Text = "Download Model from Database Server..."
            '
            'LinkLabel3
            '
            Me.LinkLabel3.ActiveLinkColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
            Me.LinkLabel3.AutoSize = True
            Me.LinkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline
            Me.LinkLabel3.LinkColor = System.Drawing.Color.SteelBlue
            Me.LinkLabel3.Location = New System.Drawing.Point(40, 205)
            Me.LinkLabel3.Name = "LinkLabel3"
            Me.LinkLabel3.Size = New System.Drawing.Size(99, 19)
            Me.LinkLabel3.TabIndex = 7
            Me.LinkLabel3.TabStop = True
            Me.LinkLabel3.Text = "Open Project..."
            '
            'PictureBox1
            '
            Me.PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), System.Drawing.Image)
            Me.PictureBox1.Location = New System.Drawing.Point(0, 64)
            Me.PictureBox1.MaximumSize = New System.Drawing.Size(22, 41)
            Me.PictureBox1.MinimumSize = New System.Drawing.Size(22, 41)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(22, 41)
            Me.PictureBox1.TabIndex = 10
            Me.PictureBox1.TabStop = False
            '
            'KeepsCheckbox
            '
            Me.KeepsCheckbox.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.KeepsCheckbox.Checked = False
            Me.KeepsCheckbox.LabelText = "Close page after the project load"
            Me.KeepsCheckbox.Location = New System.Drawing.Point(63, 824)
            Me.KeepsCheckbox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
            Me.KeepsCheckbox.Name = "KeepsCheckbox"
            Me.KeepsCheckbox.Size = New System.Drawing.Size(181, 18)
            Me.KeepsCheckbox.TabIndex = 1
            Me.KeepsCheckbox.UI = Checkbox1
            '
            'ShowCheckbox
            '
            Me.ShowCheckbox.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.ShowCheckbox.Checked = False
            Me.ShowCheckbox.LabelText = "  Show page on startup"
            Me.ShowCheckbox.Location = New System.Drawing.Point(63, 888)
            Me.ShowCheckbox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
            Me.ShowCheckbox.Name = "ShowCheckbox"
            Me.ShowCheckbox.Size = New System.Drawing.Size(138, 18)
            Me.ShowCheckbox.TabIndex = 0
            Me.ShowCheckbox.UI = Checkbox2
            '
            'WebBrowser1
            '
            Me.WebBrowser1.Location = New System.Drawing.Point(573, 344)
            Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
            Me.WebBrowser1.Name = "WebBrowser1"
            Me.WebBrowser1.Size = New System.Drawing.Size(250, 250)
            Me.WebBrowser1.TabIndex = 11
            '
            'StartPage
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 19.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(22, Byte), Integer), CType(CType(32, Byte), Integer), CType(CType(48, Byte), Integer))
            Me.Controls.Add(Me.WebBrowser1)
            Me.Controls.Add(Me.PictureBox1)
            Me.Controls.Add(Me.LinkLabel3)
            Me.Controls.Add(Me.LinkLabel2)
            Me.Controls.Add(Me.LinkLabel1)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.KeepsCheckbox)
            Me.Controls.Add(Me.ShowCheckbox)
            Me.Font = New System.Drawing.Font("微软雅黑", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
            Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
            Me.Name = "StartPage"
            Me.Size = New System.Drawing.Size(1250, 941)
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ShowCheckbox As Microsoft.VisualBasic.MolkPlusTheme.Windows.Forms.Controls.Checkbox
        Friend WithEvents KeepsCheckbox As Microsoft.VisualBasic.MolkPlusTheme.Windows.Forms.Controls.Checkbox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Friend WithEvents LinkLabel2 As System.Windows.Forms.LinkLabel
        Friend WithEvents LinkLabel3 As System.Windows.Forms.LinkLabel
        ' Friend WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
        '   Friend WithEvents LineShape1 As Microsoft.VisualBasic.PowerPacks.LineShape
        '  Friend WithEvents LineShape3 As Microsoft.VisualBasic.PowerPacks.LineShape
        '  Friend WithEvents LineShape2 As Microsoft.VisualBasic.PowerPacks.LineShape
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser

    End Class

End Namespace