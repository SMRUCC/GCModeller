#Region "Microsoft.VisualBasic::f64f39803adca67d83fa83fb9f9da24a, meme_suite\MEME\Analysis\Similarity\TomQuery\TomVisual.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module TomVisual
    ' 
    '         Function: (+2 Overloads) VisualLevEdit
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Namespace Analysis.Similarity.TOMQuery

    ''' <summary>
    ''' 全局比对
    ''' </summary>
    <Package("TOM.Visual", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module TomVisual

        Dim margin As Integer = 100

        Const logoMargin As Integer = 50

        ''' <summary>
        ''' 可视化motif之间的Levenshtein相似度编辑矩阵
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="edits"></param>
        ''' <returns></returns>
        <ExportAPI("LevEdit.Visual")>
        Public Function VisualLevEdit(query As MotifScans.AnnotationModel,
                                      subject As MotifScans.AnnotationModel,
                                      edits As DistResult,
                                      Optional alignPrint As Boolean = True) As Image

            Dim logoQuery As Image = SequenceLogoAPI.DrawLogo(query, Margin:=logoMargin, reverse:=True).AsGDIImage
            Dim logoSubject As Image = SequenceLogoAPI.DrawLogo(subject, Margin:=logoMargin).AsGDIImage
            Dim gr = New Size(logoSubject.Width + 1500, logoQuery.Width + 2000).CreateGDIDevice

            Call gr.DrawImage(logoSubject, New Point(margin + logoQuery.Height + SequenceLogo.WordSize, margin))

            logoQuery = logoQuery.RotateImage(-90)
            Call gr.DrawImage(logoQuery, New Point(margin, margin + logoSubject.Height + SequenceLogo.WordSize))

            If edits Is Nothing Then
                Return gr.ImageResource
            End If

            ' 从这里开始进行编辑矩阵的绘制
            Dim x = margin + logoQuery.Width + logoMargin, y As Integer = margin + logoSubject.Height + logoMargin
            Dim matFONT As New Font("Ubuntu", 24, FontStyle.Bold)

            Dim idx As Integer = -1
            Dim offset As Integer = 0

            For j As Integer = 0 To subject.PWM.Length   ' 参照subject画列

                For i As Integer = -1 To query.PWM.Length - 1 ' 参照query画行
                    Dim value As String = CStr(edits.DistTable(i + 1)(j))
                    Dim size = gr.Graphics.MeasureString(value, matFONT)

                    Dim left = x + (SequenceLogo.WordSize - size.Width) / 2
                    Dim top = y + (SequenceLogo.WordSize - size.Height) / 2

                    If edits.IsPath(i, j) Then
                        Dim cl As Brush

                        'If i = 0 AndAlso j = 0 Then
                        '    cl = Brushes.Green
                        'Else
                        Dim ch As String = CStr(edits.DistEdits.Get(idx))

                        idx += 1

                        If ch = "m"c Then
                            cl = Brushes.Red
                        Else
                            cl = Brushes.Green
                        End If
                        ' End If

                        Call gr.Graphics.FillRectangle(cl, New Rectangle(New Point(x + offset, y), New Size(SequenceLogo.WordSize, SequenceLogo.WordSize)))
                        Call gr.Graphics.DrawString(value, matFONT, Brushes.White, New Point(left, top))
                    Else
                        Call gr.Graphics.DrawString(value, matFONT, Brushes.Black, New Point(left, top))
                    End If

                    y = y + SequenceLogo.WordSize
                Next

                y = margin + logoSubject.Height + logoMargin
                x += SequenceLogo.WordSize
            Next

            Dim xxd As Integer = x

            Call gr.Graphics.FillRectangle(Brushes.Blue, New Rectangle(New Point(xxd, y), New Size(SequenceLogo.WordSize, SequenceLogo.WordSize)))

            For Each ch As Char In edits.DistEdits
                Dim size = gr.Graphics.MeasureString(ch, matFONT)

                Select Case ch
                    Case "i"c
                        xxd = x
                        y = y + SequenceLogo.WordSize
                    Case "d"c
                        xxd += SequenceLogo.WordSize
                    Case "m"c
                        xxd = x
                        y = y + SequenceLogo.WordSize
                    Case "s"c
                        xxd = x
                        y = y + SequenceLogo.WordSize
                End Select

                Dim left = xxd + (SequenceLogo.WordSize - size.Width) / 2
                Dim top = y + (SequenceLogo.WordSize - size.Height) / 2

                If Not ch = "d"c Then
                    Call gr.Graphics.FillRectangle(Brushes.Blue, New Rectangle(New Point(xxd, y), New Size(SequenceLogo.WordSize, SequenceLogo.WordSize)))
                    Call gr.Graphics.DrawString(ch, matFONT, Brushes.White, New Point(left, top))
                Else
                    Dim df As New Font(matFONT.Name, matFONT.Size, FontStyle.Bold Or FontStyle.Italic)
                    Call gr.Graphics.DrawString(ch, df, Brushes.Gray, New Point(left, top))
                End If
            Next

            If Not alignPrint Then
                Return gr.ImageResource
            End If

            y += SequenceLogo.WordSize * 2
            x = margin + logoQuery.Width + logoMargin
            y += logoMargin * 2
            matFONT = New Font("Consolas", 28)
            Dim ht As Integer = gr.Graphics.MeasureString("0", matFONT).Height + 10

            Call gr.Graphics.DrawString($"Score      = {edits.Score}", matFONT, Brushes.Black, New Point(x, y)) : y += ht
            Call gr.Graphics.DrawString($"Similarity = {Math.Round(edits.MatchSimilarity * 100, 2)}%", matFONT, Brushes.Black, New Point(x, y)) : y += ht
            Call gr.Graphics.DrawString($"Distance   = {edits.Distance}", matFONT, Brushes.Black, New Point(x, y)) : y += ht
            y += ht
            Call gr.Graphics.DrawString($"Dist Edits = {edits.DistEdits}", matFONT, Brushes.Black, New Point(x, y)) : y += ht
            y += ht
            Call gr.Graphics.DrawString($"Query      = {edits.Reference}", matFONT, Brushes.Black, New Point(x, y)) : y += ht
            Call gr.Graphics.DrawString($"Matches    = {edits.Matches}", matFONT, Brushes.Black, New Point(x, y)) : y += ht
            Call gr.Graphics.DrawString($"Subject    = {edits.Hypotheses}", matFONT, Brushes.Black, New Point(x, y)) : y += ht

            Return gr.ImageResource
        End Function

        <ExportAPI("LevEdit.Visual")>
        Public Function VisualLevEdit(query As MotifScans.AnnotationModel,
                                      subject As MotifScans.AnnotationModel,
                                      Optional cost As Double = 0.7,
                                      Optional method As String = "pcc",
                                      Optional threshold As Double = 0.3) As Image
            Dim edits As DistResult = TomTOm.Compare(query, subject, method, cost, threshold)
            Return VisualLevEdit(query, subject, edits)
        End Function
    End Module
End Namespace
