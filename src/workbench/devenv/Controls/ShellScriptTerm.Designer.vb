<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ShellScriptTerm
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
        ' Me.ShellControl1 = New UILibrary.ShellControl.ShellControl()
        Me.SuspendLayout()
        '
        'ShellControl1
        '
        'Me.ShellControl1.Dock = System.Windows.Forms.DockStyle.Fill
        'Me.ShellControl1.Location = New System.Drawing.Point(0, 0)
        'Me.ShellControl1.Name = "ShellControl1"
        'Me.ShellControl1.Prompt = "> "
        'Me.ShellControl1.ShellTextBackColor = System.Drawing.Color.White
        'Me.ShellControl1.ShellTextFont = New System.Drawing.Font("Microsoft YaHei", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        'Me.ShellControl1.ShellTextForeColor = System.Drawing.Color.Black
        'Me.ShellControl1.Size = New System.Drawing.Size(386, 353)
        'Me.ShellControl1.TabIndex = 0
        '
        'ShellScriptTerm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        '   Me.Controls.Add(Me.ShellControl1)
        Me.Name = "ShellScriptTerm"
        Me.Size = New System.Drawing.Size(386, 353)
        Me.ResumeLayout(False)

    End Sub
    '  Friend WithEvents ShellControl1 As UILibrary.ShellControl.ShellControl

End Class
