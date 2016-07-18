<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormInit
    Inherits FormBase

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.CommandLink1 = New Installer.CommandLink()
        Me.CommandLink2 = New Installer.CommandLink()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(203, 76)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(488, 34)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Select install now to install GCModeller with default settings, or choose Customi" &
    "ze " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "to enable or disable features."
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(697, 451)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Cancel"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'CommandLink1
        '
        Me.CommandLink1.Location = New System.Drawing.Point(233, 147)
        Me.CommandLink1.Name = "CommandLink1"
        Me.CommandLink1.Size = New System.Drawing.Size(297, 120)
        Me.CommandLink1.TabIndex = 4
        Me.CommandLink1.Text = "CommandLink1"
        '
        'CommandLink2
        '
        Me.CommandLink2.Location = New System.Drawing.Point(233, 273)
        Me.CommandLink2.Name = "CommandLink2"
        Me.CommandLink2.Size = New System.Drawing.Size(395, 132)
        Me.CommandLink2.TabIndex = 5
        Me.CommandLink2.Text = "CommandLink2"
        '
        'FormInit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 492)
        Me.Controls.Add(Me.CommandLink2)
        Me.Controls.Add(Me.CommandLink1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label2)
        Me.Margin = New System.Windows.Forms.Padding(3, 5, 3, 5)
        Me.Name = "FormInit"
        Me.Title = "Install GCModeller 3.3.54 (64-bit)"
        Me.Controls.SetChildIndex(Me.Label2, 0)
        Me.Controls.SetChildIndex(Me.Button1, 0)
        Me.Controls.SetChildIndex(Me.CommandLink1, 0)
        Me.Controls.SetChildIndex(Me.CommandLink2, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label2 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents CommandLink1 As CommandLink
    Friend WithEvents CommandLink2 As CommandLink
End Class
