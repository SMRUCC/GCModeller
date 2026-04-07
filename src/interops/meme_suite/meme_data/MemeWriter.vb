#Region "Microsoft.VisualBasic::5f836a825f5d441224282102a279a7d1, meme_suite\meme_data\MemeWriter.vb"

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


    ' Code Statistics:

    '   Total Lines: 165
    '    Code Lines: 101 (61.21%)
    ' Comment Lines: 35 (21.21%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 29 (17.58%)
    '     File Size: 6.43 KB


    ' Class MemeWriter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: (+2 Overloads) WriteMemeFormat
    ' 
    '     Sub: WriteDocument, WriteMemeFormat, WriteMotif
    ' 
    ' Module ProbabilityExtensions
    ' 
    '     Function: SaveToMeme
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MemeWriter

    ReadOnly motifs As Probability()
    ReadOnly background As Dictionary(Of String, Double)
    ReadOnly nsites As Integer
    ReadOnly url_base As String

    Sub New(motifs As IEnumerable(Of Probability),
            Optional backgroundFreq As Dictionary(Of String, Double) = Nothing,
            Optional nsites As Integer = 100,
            Optional url_base As String = Nothing)

        Me.motifs = motifs.ToArray
        Me.background = If(backgroundFreq, New Dictionary(Of String, Double) From {
            {"A", 0.25},
            {"C", 0.25},
            {"G", 0.25},
            {"T", 0.25}
        })
        Me.nsites = nsites
        Me.url_base = url_base
    End Sub

    Public Sub WriteDocument(str As TextWriter)
        ' 1. 写入文件头
        str.WriteLine("MEME version 4.4")
        str.WriteLine("ALPHABET= ACGT")
        str.WriteLine("strands: + -")
        str.WriteLine("Background letter frequencies (from file `background.bg'):")

        ' 2. 写入背景频率
        Dim bgFreqStr As String = background.Select(Function(kv) $"{kv.Key} {kv.Value.ToString("0.00000")}").JoinBy(" ")

        str.WriteLine(bgFreqStr)
        str.WriteLine()

        For Each motif As Probability In motifs
            Call WriteMotif(str, motif)
            Call str.WriteLine()
        Next
    End Sub

    Private Sub WriteMotif(str As TextWriter, motif As Probability)
        ' 3. 写入MOTIF行
        str.WriteLine($"MOTIF {motif.name}")
        str.WriteLine()

        ' 4. 写入letter-probability matrix
        ' 5. 写入矩阵参数
        Dim eValue As String = If(motif.pvalue > 0, motif.pvalue.ToString("0.0e-000"), "1.0e-999")
        str.WriteLine($"letter-probability matrix: alength= 4 w= {motif.width} nsites= {nsites} E= {eValue}")

        ' 6. 写入PWM矩阵数据
        ' 定义碱基顺序：A, C, G, T
        Dim baseOrder As String() = If(motif.background.IsNullOrEmpty, {"A", "C", "G", "T"}, motif.background.Keys.ToArray)

        For Each residue As Residue In motif.region
            Dim values As New List(Of String)

            For Each baseChar As String In baseOrder
                Dim freq As Double = 0.0
                If residue.frequency IsNot Nothing AndAlso residue.frequency.ContainsKey(baseChar) Then
                    freq = residue.frequency(baseChar)

                    If freq.IsNaNImaginary Then
                        freq = 0
                    End If
                Else
                    freq = 0
                End If

                Call values.Add(freq.ToString("0.000000"))
            Next

            str.WriteLine(String.Join(" ", values))
        Next

        Call str.WriteLine()

        ' 7. 写入URL（如果有）
        If Not String.IsNullOrEmpty(url_base) Then
            str.WriteLine($"URL {url_base}?id={motif.name.UrlEncode}")
        Else
            str.WriteLine($"URL n/a")
        End If
    End Sub

    ''' <summary>
    ''' 将PWM模型保存为MEME格式的文本文件
    ''' </summary>
    ''' <param name="motifs">PWM模型对象集合</param>
    ''' <param name="outputPath">输出文件路径</param>
    ''' <param name="backgroundFreq">背景碱基频率（可选）</param>
    ''' <param name="nsites">位点数量（可选，默认为100）</param>
    ''' <param name="url">URL链接（可选）</param>
    Public Shared Function WriteMemeFormat(motifs As IEnumerable(Of Probability), outputPath As String,
                                           Optional backgroundFreq As Dictionary(Of String, Double) = Nothing,
                                           Optional nsites As Integer = 100,
                                           Optional url As String = Nothing) As Boolean
        Dim sb As New StringBuilder()
        Dim doc As New StringWriter(sb)
        Dim writer As New MemeWriter(motifs, backgroundFreq, nsites, url_base:=url)

        Call writer.WriteDocument(doc)
        Call doc.Flush()

        ' 8. 保存到文件
        Return sb.SaveTo(outputPath, Encoding.UTF8)
    End Function

    ''' <summary>
    ''' 将PWM模型保存为MEME格式的文本文件
    ''' </summary>
    ''' <param name="motif">PWM模型对象</param>
    ''' <param name="outputPath">输出文件路径</param>
    ''' <param name="backgroundFreq">背景碱基频率（可选）</param>
    ''' <param name="nsites">位点数量（可选，默认为100）</param>
    ''' <param name="url">URL链接（可选）</param>
    Public Shared Function WriteMemeFormat(motif As Probability, outputPath As String,
                                           Optional backgroundFreq As Dictionary(Of String, Double) = Nothing,
                                           Optional nsites As Integer = 100,
                                           Optional url As String = Nothing) As Boolean
        Dim sb As New StringBuilder()
        Dim doc As New StringWriter(sb)
        Dim writer As New MemeWriter({motif}, backgroundFreq, nsites, url_base:=url)

        Call writer.WriteDocument(doc)
        Call doc.Flush()

        ' 8. 保存到文件
        Return sb.SaveTo(outputPath, Encoding.UTF8)
    End Function

    ''' <summary>
    ''' 重载方法：使用默认参数保存
    ''' </summary>
    Public Shared Sub WriteMemeFormat(motif As Probability, outputPath As String)
        WriteMemeFormat(motif, outputPath, Nothing, 100, Nothing)
    End Sub
End Class

''' <summary>
''' 扩展方法模块
''' </summary>
Public Module ProbabilityExtensions

    ''' <summary>
    ''' 扩展方法：直接保存PWM模型为MEME格式文件
    ''' </summary>
    <Extension>
    Public Function SaveToMeme(motif As Probability, outputPath As String,
                               Optional backgroundFreq As Dictionary(Of String, Double) = Nothing,
                               Optional nsites As Integer = 100,
                               Optional url As String = Nothing) As Boolean

        Return MemeWriter.WriteMemeFormat(motif, outputPath, backgroundFreq, nsites, url)
    End Function

End Module

