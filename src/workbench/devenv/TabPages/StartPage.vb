#Region "Microsoft.VisualBasic::640435749f3922ab1b3ecfabd2852a93, ..\workbench\devenv\TabPages\StartPage.vb"

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

Namespace TabPages

    Friend Class StartPage

        Private Sub StartPage_Load(sender As Object, e As EventArgs) Handles Me.Load
            WebBrowser1.Location = New Drawing.Point With {.X = 340, .Y = 0}
            WebBrowser1.Navigate(My.Application.Info.DirectoryPath & "/html/dev_startpage/index.html")
        End Sub

        Private Sub StartPage_Resize(sender As Object, e As EventArgs) Handles Me.Resize
            '  LineShape3.StartPoint = New Drawing.Point With {.X = 40, .Y = Height - 100}
            '   LineShape3.EndPoint = New Drawing.Point With {.X = 300, .Y = Height - 100}

            KeepsCheckbox.Location = New Drawing.Point With {.X = 45, .Y = Height - 70}
            ShowCheckbox.Location = New Drawing.Point With {.X = 45, .Y = Height - 45}

            WebBrowser1.Size = New Drawing.Size With {.Width = Width - 340, .Height = Height}
        End Sub
    End Class
End Namespace
