#Region "Microsoft.VisualBasic::48480181111610bab6cd81928acfff1a, ..\workbench\devenv\Forms\FormAbout.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Friend Class FormAbout : Inherits FormSplash
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents PictureBox4 As System.Windows.Forms.PictureBox

    Sub New()
        Call InitializeComponent()


    End Sub

    Private Sub FormAbout_Load(sender As Object, e As EventArgs) Handles Me.Load
        AddHandler MyBase.PictureBox1.Click, Sub() Me.Close()
    End Sub

    Private Sub FormAbout_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        Me.Close()
    End Sub

    Private Sub InitializeComponent()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.Label4 = New System.Windows.Forms.Label()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox4
        '
        Me.PictureBox4.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.PictureBox4.BackgroundImage = Global.LANS.SystemsBiology.GCModeller.Workbench.My.Resources.Resources.Twitter_logo_blue
        Me.PictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox4.Location = New System.Drawing.Point(12, 286)
        Me.PictureBox4.Name = "PictureBox4"
        Me.PictureBox4.Size = New System.Drawing.Size(24, 24)
        Me.PictureBox4.TabIndex = 8
        Me.PictureBox4.TabStop = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(51, 298)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(119, 12)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Follow: @GCModeller"
        '
        'FormAbout
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.ClientSize = New System.Drawing.Size(560, 322)
        Me.Controls.Add(Me.PictureBox4)
        Me.Controls.Add(Me.Label4)
        Me.Name = "FormAbout"
        Me.Controls.SetChildIndex(Me.Label4, 0)
        Me.Controls.SetChildIndex(Me.PictureBox4, 0)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        Call Process.Start("http://twitter.com/gcmodeller")
    End Sub
End Class

