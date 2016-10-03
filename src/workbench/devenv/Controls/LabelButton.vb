#Region "Microsoft.VisualBasic::7076359cb2430de4f3d8ccff21b38945, ..\workbench\devenv\Controls\LabelButton.vb"

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

Public Class LabelButton

    Public Shadows Event Click()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        RaiseEvent Click()
    End Sub

    Public Overrides Property Text As String
        Get
            Return MyBase.Text
        End Get
        Set(value As String)
            MyBase.Text = value
            '只输入ASCII字符
            BackgroundImage = DrawText(value)
            Size = BackgroundImage.Size
            Button1.Location = New Point With {.X = 0, .Y = 20}
        End Set
    End Property

    Function DrawText(Text As String) As Image
        Dim size As Size = New Size With {.Width = Len(Text) * 7.5, .Height = 25}
        Dim bmp As New Bitmap(size.Width, size.Height)
        Dim gr As Graphics = Graphics.FromImage(bmp)

        gr.DrawString(Text, New Font("Microsoft YaHei", 8), Brushes.White, New Point With {.X = 0, .Y = 1})

        Return bmp
    End Function

    Private Sub LabelButton_Load(sender As Object, e As EventArgs) Handles Me.Load
        Button1.UI = New MolkPlusTheme.Visualise.Elements.ButtonResource With {
            .Normal = My.Resources.LabelButtonNormal,
            .PreLight = My.Resources.LabelButtonPrelight,
            .Active = My.Resources.LabelButtonPrelight}
    End Sub
End Class

