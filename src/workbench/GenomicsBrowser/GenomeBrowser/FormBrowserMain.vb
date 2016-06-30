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
