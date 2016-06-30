Namespace TabPages

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Options
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
            Me.ComboLanguage = New System.Windows.Forms.ComboBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.PictureBox1 = New System.Windows.Forms.PictureBox()
            Me.Panel2 = New System.Windows.Forms.Panel()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Panel3 = New System.Windows.Forms.Panel()
            Me.Button2 = New System.Windows.Forms.Button()
            Me.Button1 = New System.Windows.Forms.Button()
            Me.TabControl1 = New System.Windows.Forms.TabControl()
            Me.TabPage1 = New System.Windows.Forms.TabPage()
            Me.TabPage2 = New System.Windows.Forms.TabPage()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.TextBox1 = New System.Windows.Forms.TextBox()
            Me.Button3 = New System.Windows.Forms.Button()
            Me.Button4 = New System.Windows.Forms.Button()
            Me.TextBox2 = New System.Windows.Forms.TextBox()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.Button5 = New System.Windows.Forms.Button()
            Me.TextBox3 = New System.Windows.Forms.TextBox()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.Panel1.SuspendLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.Panel2.SuspendLayout()
            Me.Panel3.SuspendLayout()
            Me.TabControl1.SuspendLayout()
            Me.TabPage1.SuspendLayout()
            Me.TabPage2.SuspendLayout()
            Me.SuspendLayout()
            '
            'ComboLanguage
            '
            Me.ComboLanguage.FormattingEnabled = True
            Me.ComboLanguage.Items.AddRange(New Object() {"System", "zh-CN(简体中文)", "en-US(English)", "fr-FR(Francais)"})
            Me.ComboLanguage.Location = New System.Drawing.Point(245, 113)
            Me.ComboLanguage.Name = "ComboLanguage"
            Me.ComboLanguage.Size = New System.Drawing.Size(121, 21)
            Me.ComboLanguage.TabIndex = 0
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(121, 121)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(55, 13)
            Me.Label1.TabIndex = 1
            Me.Label1.Text = "Language"
            '
            'Panel1
            '
            Me.Panel1.Controls.Add(Me.Label3)
            Me.Panel1.Controls.Add(Me.Label2)
            Me.Panel1.Controls.Add(Me.PictureBox1)
            Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(1253, 80)
            Me.Panel1.TabIndex = 2
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label3.Location = New System.Drawing.Point(93, 40)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(75, 22)
            Me.Label3.TabIndex = 2
            Me.Label3.Text = "Options"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(94, 12)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(62, 13)
            Me.Label2.TabIndex = 1
            Me.Label2.Text = "GCModeller"
            '
            'PictureBox1
            '
            Me.PictureBox1.BackgroundImage = Global.LANS.SystemsBiology.GCModeller.Workbench.My.Resources.Resources.admin_option_file
            Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
            Me.PictureBox1.Location = New System.Drawing.Point(24, 12)
            Me.PictureBox1.Name = "PictureBox1"
            Me.PictureBox1.Size = New System.Drawing.Size(50, 50)
            Me.PictureBox1.TabIndex = 0
            Me.PictureBox1.TabStop = False
            '
            'Panel2
            '
            Me.Panel2.Controls.Add(Me.Label4)
            Me.Panel2.Controls.Add(Me.Panel3)
            Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.Panel2.Location = New System.Drawing.Point(0, 790)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(1253, 80)
            Me.Panel2.TabIndex = 3
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Location = New System.Drawing.Point(43, 35)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(39, 13)
            Me.Label4.TabIndex = 2
            Me.Label4.Text = "Label4"
            '
            'Panel3
            '
            Me.Panel3.Controls.Add(Me.Button2)
            Me.Panel3.Controls.Add(Me.Button1)
            Me.Panel3.Dock = System.Windows.Forms.DockStyle.Right
            Me.Panel3.Location = New System.Drawing.Point(995, 0)
            Me.Panel3.Name = "Panel3"
            Me.Panel3.Size = New System.Drawing.Size(258, 80)
            Me.Panel3.TabIndex = 3
            '
            'Button2
            '
            Me.Button2.Location = New System.Drawing.Point(14, 25)
            Me.Button2.Name = "Button2"
            Me.Button2.Size = New System.Drawing.Size(75, 23)
            Me.Button2.TabIndex = 1
            Me.Button2.Text = "Apply"
            Me.Button2.UseVisualStyleBackColor = True
            '
            'Button1
            '
            Me.Button1.Location = New System.Drawing.Point(116, 25)
            Me.Button1.Name = "Button1"
            Me.Button1.Size = New System.Drawing.Size(75, 23)
            Me.Button1.TabIndex = 0
            Me.Button1.Text = "Discard"
            Me.Button1.UseVisualStyleBackColor = True
            '
            'TabControl1
            '
            Me.TabControl1.Controls.Add(Me.TabPage1)
            Me.TabControl1.Controls.Add(Me.TabPage2)
            Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TabControl1.Location = New System.Drawing.Point(0, 80)
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.SelectedIndex = 0
            Me.TabControl1.Size = New System.Drawing.Size(1253, 710)
            Me.TabControl1.TabIndex = 4
            '
            'TabPage1
            '
            Me.TabPage1.Controls.Add(Me.ComboLanguage)
            Me.TabPage1.Controls.Add(Me.Label1)
            Me.TabPage1.Location = New System.Drawing.Point(4, 22)
            Me.TabPage1.Name = "TabPage1"
            Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage1.Size = New System.Drawing.Size(1245, 684)
            Me.TabPage1.TabIndex = 0
            Me.TabPage1.Text = "GCModeller IDE"
            Me.TabPage1.UseVisualStyleBackColor = True
            '
            'TabPage2
            '
            Me.TabPage2.Controls.Add(Me.Button5)
            Me.TabPage2.Controls.Add(Me.TextBox3)
            Me.TabPage2.Controls.Add(Me.Label7)
            Me.TabPage2.Controls.Add(Me.Button4)
            Me.TabPage2.Controls.Add(Me.TextBox2)
            Me.TabPage2.Controls.Add(Me.Label6)
            Me.TabPage2.Controls.Add(Me.Button3)
            Me.TabPage2.Controls.Add(Me.TextBox1)
            Me.TabPage2.Controls.Add(Me.Label5)
            Me.TabPage2.Location = New System.Drawing.Point(4, 22)
            Me.TabPage2.Name = "TabPage2"
            Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage2.Size = New System.Drawing.Size(1245, 684)
            Me.TabPage2.TabIndex = 1
            Me.TabPage2.Text = "External Commands"
            Me.TabPage2.UseVisualStyleBackColor = True
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(47, 52)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(33, 13)
            Me.Label5.TabIndex = 0
            Me.Label5.Text = "R Bin"
            '
            'TextBox1
            '
            Me.TextBox1.Location = New System.Drawing.Point(130, 52)
            Me.TextBox1.Name = "TextBox1"
            Me.TextBox1.Size = New System.Drawing.Size(100, 20)
            Me.TextBox1.TabIndex = 1
            '
            'Button3
            '
            Me.Button3.Location = New System.Drawing.Point(326, 52)
            Me.Button3.Name = "Button3"
            Me.Button3.Size = New System.Drawing.Size(75, 23)
            Me.Button3.TabIndex = 2
            Me.Button3.Text = "Browse..."
            Me.Button3.UseVisualStyleBackColor = True
            '
            'Button4
            '
            Me.Button4.Location = New System.Drawing.Point(324, 123)
            Me.Button4.Name = "Button4"
            Me.Button4.Size = New System.Drawing.Size(75, 23)
            Me.Button4.TabIndex = 5
            Me.Button4.Text = "Browse..."
            Me.Button4.UseVisualStyleBackColor = True
            '
            'TextBox2
            '
            Me.TextBox2.Location = New System.Drawing.Point(128, 123)
            Me.TextBox2.Name = "TextBox2"
            Me.TextBox2.Size = New System.Drawing.Size(100, 20)
            Me.TextBox2.TabIndex = 4
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(45, 123)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(59, 13)
            Me.Label6.TabIndex = 3
            Me.Label6.Text = "BLAST Bin"
            '
            'Button5
            '
            Me.Button5.Location = New System.Drawing.Point(326, 188)
            Me.Button5.Name = "Button5"
            Me.Button5.Size = New System.Drawing.Size(75, 23)
            Me.Button5.TabIndex = 8
            Me.Button5.Text = "Browse..."
            Me.Button5.UseVisualStyleBackColor = True
            '
            'TextBox3
            '
            Me.TextBox3.Location = New System.Drawing.Point(130, 188)
            Me.TextBox3.Name = "TextBox3"
            Me.TextBox3.Size = New System.Drawing.Size(100, 20)
            Me.TextBox3.TabIndex = 7
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Location = New System.Drawing.Point(47, 188)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(59, 13)
            Me.Label7.TabIndex = 6
            Me.Label7.Text = "BLAST DB"
            '
            'Options
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TabControl1)
            Me.Controls.Add(Me.Panel2)
            Me.Controls.Add(Me.Panel1)
            Me.Name = "Options"
            Me.Size = New System.Drawing.Size(1253, 870)
            Me.Panel1.ResumeLayout(False)
            Me.Panel1.PerformLayout()
            CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.Panel2.ResumeLayout(False)
            Me.Panel2.PerformLayout()
            Me.Panel3.ResumeLayout(False)
            Me.TabControl1.ResumeLayout(False)
            Me.TabPage1.ResumeLayout(False)
            Me.TabPage1.PerformLayout()
            Me.TabPage2.ResumeLayout(False)
            Me.TabPage2.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents ComboLanguage As System.Windows.Forms.ComboBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
        Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
        Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
        Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Button2 As System.Windows.Forms.Button
        Friend WithEvents Button1 As System.Windows.Forms.Button
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Panel3 As System.Windows.Forms.Panel
        Friend WithEvents Button3 As System.Windows.Forms.Button
        Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Button5 As System.Windows.Forms.Button
        Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents Button4 As System.Windows.Forms.Button
        Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
        Friend WithEvents Label6 As System.Windows.Forms.Label

    End Class

End Namespace