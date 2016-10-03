#Region "Microsoft.VisualBasic::a2bee4522728a7dcbd8fcc4a2de74c92, ..\workbench\d3js\Hierarchical-Edge-Bundling\Hierarchical-Edge-Bundling\FormHEBViewer.vb"

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

Imports Microsoft.VisualBasic.Windows.Forms
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.HTTPInternal.Core
Imports Microsoft.VisualBasic.Serialization
Imports System.Text

Public Class FormHEBViewer

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ToolStripManager.Renderer = New ChromeUIRender
    End Sub

    Dim __services As New HttpFileSystem(8523, App.AppSystemTemp, False, AddressOf __getValue)
    Dim data As Byte() = {}

    Private Function __getValue(res As String) As Byte()
        If InStr(res, "readme-flare-imports.json", CompareMethod.Text) > 0 Then
            Return data
        End If
        Return __services.GetResource(res)
    End Function

    Private Sub FormHEBViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call My.Resources.index.SaveTo(App.AppSystemTemp & "/index.html")
        Call My.Resources.d3_v3_min.SaveTo(App.AppSystemTemp & "/d3.v3.min.js")
        ' Call RunTask(AddressOf __services.Run)
        Call WebBrowser1.Navigate("http://127.0.0.1")
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using open As New OpenFileDialog With {.Filter = "Network data(*.csv)|*.csv"}
            If open.ShowDialog = DialogResult.OK Then
                Dim json As String = LoadAs(open.FileName).GetJson
                data = Encoding.UTF8.GetBytes(json)
                Call WebBrowser1.Navigate("http://127.0.0.1:8523/")

#If DEBUG Then
                Call json.SaveTo(App.GetAppSysTempFile(".json"))
#End If
            End If
        End Using
    End Sub
End Class

