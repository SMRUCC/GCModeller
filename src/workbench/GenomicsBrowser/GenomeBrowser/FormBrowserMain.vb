#Region "Microsoft.VisualBasic::fa42f6a0d387a5d4a4e99a9503f76563, ..\workbench\GenomicsBrowser\GenomeBrowser\FormBrowserMain.vb"

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

Public Class FormBrowserMain

    Dim Browser As GenomicsBrowser.GenomicsBrowserControl

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using OpenFile = New OpenFileDialog With {.Filter = "GFF(*.gff)|*.gff|Vector Script(*.vcs)|*.vcs"}

            If OpenFile.ShowDialog <> DialogResult.OK Then
                Return
            End If

            Dim Text = $"Genomics Browser [{OpenFile.FileName.ToFileURL}]"

            If Not Browser Is Nothing Then Call Me.Controls.Remove(Browser)
            Browser = New GenomicsBrowserControl(Sub(idx As String) Me.BeginInvoke(Sub() Me.Text = Text & "  " & idx & " %"))

            Call Me.Controls.Add(Browser)

            Browser.Dock = DockStyle.Fill
            Browser.Location = New Point(0, 20)
            Browser.Size = New Size(Width - 10, Height - 20)

            If IO.Path.GetExtension(OpenFile.FileName).Split(".").Last.Equals("gff", StringComparison.OrdinalIgnoreCase) Then
                Call Browser.LoadDocument(LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.GFF.LoadDocument(OpenFile.FileName))
            Else
                Call Browser.LoadDocument(Microsoft.VisualBasic.Drawing.Drawing2D.DrawingScript.LoadDocument(OpenFile.FileName).ToVectogram)
            End If

        End Using
    End Sub
End Class
