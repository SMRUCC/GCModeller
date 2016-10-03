#Region "Microsoft.VisualBasic::dcec355f596438b062f1a2344d68c9ef, ..\workbench\devenv\TabPages\WebBrowser.vb"

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

    Public Class WebBrowser : Inherits Microsoft.VisualBasic.MolkPlusTheme.Windows.Forms.Controls.WebBrowser

        Public Property MyHomePage As String

        Protected Overrides Function get_HomePage() As String
            Return MyHomePage
        End Function

        Private Sub WebBrowser_Load(sender As Object, e As EventArgs) Handles Me.Load
            Call Me.WebBrowser1.Navigate(MyHomePage)
        End Sub
    End Class
End Namespace
