#Region "Microsoft.VisualBasic::144a0f6451b7580dbe98b65ed4ecb310, ..\interops\visualize\Cytoscape\CLI_tool\Viewer\ViewerCanvas.vb"

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

Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Data.visualize.Network.Canvas

Public Class ViewerCanvas

    Dim WithEvents canvas As New Canvas With {
        .Dock = DockStyle.Fill
    }

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Controls.Add(canvas)
        canvas.BringToFront()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using file As New OpenFileDialog With {
            .Filter = "cyjs(*.json)|*.json"
        }

        End Using
    End Sub
End Class
