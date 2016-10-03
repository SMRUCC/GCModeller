#Region "Microsoft.VisualBasic::405b4bb56e242caea022513334203cd0, ..\workbench\IDE_PlugIns\FastaViewer\FormViwer.vb"

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

Imports System.Text
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Windows.Forms
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Imaging

Public Class FormViwer

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ToolStripManager.Renderer = New ChromeUIRender
    End Sub

    Private Sub QuitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QuitToolStripMenuItem.Click
        Close()
    End Sub

    Dim file As String

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using open As New OpenFileDialog With {
            .Title = "Select a fasta sequence file",
            .Filter = "Fasta sequence file(*.fasta,*.fa,*.fsa,*.fas)|*.fasta;*.fa;*.fsa;*.fas"
        }
            If open.ShowDialog = DialogResult.OK Then
                Dim fa As New FastaFile(open.FileName)
                Dim html As String = HTMLRenderer.VisualNts(fa)
                Dim tmp As String = App.GetAppSysTempFile(".html")
                Call html.SaveTo(tmp)
                Call WebBrowser1.Navigate(tmp)

                file = open.FileName
            End If
        End Using
    End Sub

    Private Sub FormViwer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call My.Resources.foundation.SaveTo($"{App.HOME}/assets/foundation.css")
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        Call Process.Start("https://github.com/SMRUCC/GCModeller.Workbench")
    End Sub

    Private Sub SequenceLogoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SequenceLogoToolStripMenuItem.Click
        Dim fa As New FastaFile(file)
        Dim l As Integer = fa.First.Length
        For Each x In fa
            If x.Length <> l Then
                MsgBox("The fasta sequence length is not identical!")
                Return
            End If
        Next

        Dim model As MotifPWM = Nothing
        Dim logo As Image =
            SequenceLogo.DrawingDevice.DrawFrequency(fa, getModel:=model)
        Dim tmp As String = App.GetAppSysTempFile(".png")
        Dim hmtl As String = PWMHtml(model, fa.NumberOfFasta, tmp)
        Call logo.SaveAs(tmp, ImageFormats.Png)
        tmp = App.GetAppSysTempFile(".html")
        Call hmtl.SaveTo(tmp)
        Call WebBrowser2.Navigate(tmp)

        MaterialTabControl1.SelectedIndex = 1
    End Sub

    Const autoScaleCSS As String = ".Image {
     max-width:800px;height:auto;cursor:pointer;
     border:1px dashed #4E6973;padding: 3px;
     zoom:expression( function(elm) {
         if (elm.width>560) {
             var oldVW = elm.width; elm.width=560;
             elm.height = elm.height*(560 /oldVW);
         }
         elm.style.zoom = '1';
     }(this));
}"

    Private Function PWMHtml(model As MotifPWM, n As Integer, logo As String) As String
        Dim sb As New StringBuilder(4096)

        Call sb.AppendLine($"<html><head>
<link rel=""stylesheet"" href=""{$"{App.HOME}/assets/foundation.css"}"">
<style type=""text/css"">
{autoScaleCSS}
</style>
</head><body><br>
")
        Call sb.AppendLine("<div style=""text-align:center;"">
<span style=""display:inline-block;height:100%;vertical-align: middle;""></span>")
        Call sb.AppendLine($"<img src=""{logo}"" class=""Image""/><br />")
        Call sb.AppendLine("</div>")
        Call sb.AppendLine($"<font face=""{FontFace.Consolas}"">")
        Call sb.AppendLine("<p>")
        Call sb.AppendLine($"Sequence logo model build from {n} sites.<br>")
        Call sb.AppendLine("Motif: " & New String(model.PWM.ToArray(Function(x) x.AsChar)))
        Call sb.AppendLine("<br /></p>")

        Call sb.AppendLine("<table class=""API"">")
        Call sb.AppendLine($"<tr><td>Chars</td>{String.Join("", model.PWM.ToArray(Function(x) $"<td>{x.AsChar}</td>"))}</tr>")
        Call sb.AppendLine($"<tr><strong><td>Bits</td>{String.Join("", model.PWM.ToArray(Function(x) $"<td>{Math.Round(x.Bits, 2)}</td>"))}</strong></tr>")

        For i As Integer = 0 To model.Alphabets.Length - 1
            n = i
            Call sb.AppendLine($"<tr><td>{model.Alphabets(i)}</td>{String.Join("", model.PWM.ToArray(Function(x) $"<td>{Math.Round(x.PWM(n), 2)}</td>"))}</tr>")
        Next

        Call sb.AppendLine("</table>")
        Call sb.AppendLine("</font>")
        Call sb.AppendLine("</body></html>")

        Return sb.ToString
    End Function
End Class

