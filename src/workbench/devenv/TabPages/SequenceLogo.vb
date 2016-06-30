Imports System.Drawing
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.LDM
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools

Public Class SequenceLogo

    Dim Motifs As Motif()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using File = New System.Windows.Forms.OpenFileDialog With {.Filter = "MEME Text(*.txt)|*.txt|MEME Xml Motif(*.xml)|*.xml|MEME Html(*.html)|*.html"}

            If Not File.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Return
            End If

            Try
                Dim Motifs = MEME.Text.Load(File.FileName)
                Me.Motifs = Motifs
            Catch ex As Exception
                Return
            End Try

            Call ListBox1.Items.Clear()

            For Each Motif In Motifs
                Call ListBox1.Items.Add($"{Motif.Id}  evt={Motif.Evalue }  {Motif.Signature }")
            Next

        End Using
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Dim idx = ListBox1.SelectedIndex

        If idx < 0 Then
            Return
        End If

        Dim Motif = Motifs(idx)
        Dim Model = SequenceLogoAPI.GenerateModel(Motif)
        Dim Logo As Image = SequencePatterns.SequenceLogo.InvokeDrawing(Model)

        PictureBox1.BackgroundImage = Logo

    End Sub
End Class
