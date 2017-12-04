#Region "Microsoft.VisualBasic::252520a3ebd53fb104d302ace278bdd4, ..\workbench\d3js\Browser\FormBrowser.vb"

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

Imports d3svg
Imports Gecko
Imports Gecko.Events

Public Class FormBrowser

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Xpcom.Initialize("./")
        geckoWebBrowser = New GeckoWebBrowser With {
            .Dock = DockStyle.Fill
        }
        Controls.Add(geckoWebBrowser)
    End Sub

    Dim WithEvents geckoWebBrowser As GeckoWebBrowser

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        ToolStripButton2.Enabled = False
        Call geckoWebBrowser.Navigate("http://127.0.0.1")
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Using file As New SaveFileDialog With {.Filter = "SVG image(*.svg)|*.svg"}
            If file.ShowDialog = DialogResult.OK Then
                Dim tmp As String = App.GetTempFile.TrimSuffix & ".html"
                Call geckoWebBrowser.SaveDocument(tmp)
                Dim parser As d3svg.D3Parser = New d3svg.ForceDirectedGraph
                Dim svg As d3svg.SVG = parser.HtmlFileParser(tmp)
                Call svg.BuildModel.SaveAsXml(file.FileName)
            End If
        End Using
    End Sub

    Private Sub geckoWebBrowser_DocumentCompleted(sender As Object, e As GeckoDocumentCompletedEventArgs) Handles geckoWebBrowser.DocumentCompleted
        ToolStripButton2.Enabled = True
    End Sub

    Private Sub FormBrowser_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ToolStripButton1_Click(Nothing, Nothing)
    End Sub
End Class
