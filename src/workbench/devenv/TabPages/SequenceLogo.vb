#Region "Microsoft.VisualBasic::1e71fd03cfa2b38f5a262d9d62f62638, ..\workbench\devenv\TabPages\SequenceLogo.vb"

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
