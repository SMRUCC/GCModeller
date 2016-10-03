#Region "Microsoft.VisualBasic::dce5850657209cc95e288bb9f4ac4c89, ..\workbench\devenv\Forms\FormSplash.vb"

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

Friend Class FormSplash

    'Private Const CS_DROPSHADOW = &H20000
    'Private Const GCL_STYLE = (-26)

    'Private Declare Function GetClassLong Lib "user32" Alias "GetClassLongA" ( hwnd As Integer,  nIndex As Integer) As Integer
    'Private Declare Function SetClassLong Lib "user32" Alias "SetClassLongA" ( hwnd As Integer,  nIndex As Integer,  dwNewLong As Integer) As Integer

    Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'SetClassLong(Handle, GCL_STYLE, GetClassLong(Handle, GCL_STYLE) Or CS_DROPSHADOW)

        CheckForIllegalCrossThreadCalls = False
        'Label3.Text = String.Format("{0} {1}", Label3.Text, My.Application.Info.Version.ToString)
        PictureBox1.Size = PictureBox1.BackgroundImage.Size
        Me.Size = PictureBox1.Size
        Version.Text = String.Format("Version: {0}", My.Application.Info.Version.ToString)
    End Sub
End Class
