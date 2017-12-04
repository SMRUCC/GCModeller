#Region "Microsoft.VisualBasic::381aa9be781857f3bbecdd264b5729a6, ..\workbench\devenv\Controls\OutputConsole.vb"

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

Imports System.Drawing

Public Class OutputConsole
    Dim _popout As Boolean = False

    Public Sub PopOut()
        If _popout Then
            Visible = False
            _popout = False
        Else
            Me.Visible = True
            Me.BringToFront()
            _popout = True
        End If
    End Sub

    Public Sub Close()
        Visible = False
        _popout = False
    End Sub

    Private Sub PopoutTAB_Load(sender As Object, e As EventArgs) Handles Me.Load
        TextBox1.Location = New Drawing.Point With {.X = 2, .Y = 20}
        Label1.Location = New Drawing.Point With {.X = 2, .Y = 2}

        Call Me.Close()
    End Sub

    Private Sub PopoutTAB_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        TextBox1.Size = New Drawing.Point With {.X = Width - 4, .Y = Height - 21}
        Label1.Width = Width - 4
    End Sub

    Public Sub Write(data As String)
        TextBox1.AppendText("  " & data & vbCrLf)
    End Sub

    Dim Sizing As Boolean = False
    Dim UserCursor As Point

    Private Sub Label1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseDown
        Sizing = True
        UserCursor = e.Location
    End Sub

    Private Sub Label1_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseMove
        If Sizing Then
            Dim pLocation = New Point With {
                  .X = Me.Location.X,
                  .Y = Me.Location.Y - UserCursor.Y + e.Y}

            Me.Height = Me.Height + (UserCursor.Y - e.Y)
            Me.Location = pLocation
        End If
    End Sub

    Private Sub Label1_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseUp
        Sizing = False
    End Sub
End Class
