#Region "Microsoft.VisualBasic::0538b66ce0be1bfd98e7952ac18cd363, analysis\SequenceToolkit\MotifFinder\test\Program.vb"

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

'   Total Lines: 14
'    Code Lines: 12 (85.71%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
'
'   Blank Lines: 2 (14.29%)
'     File Size: 583 B


' Module Program
'
'     Sub: Main
'
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program

    Const DEFAULT_FASTA As String = "G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\CP073066.fasta"

    ''' <summary>
    ''' 用法：
    '''   MotifFinder.test.exe em selftest
    '''   MotifFinder.test.exe em discover --input em_test\dna.fa --model zoops --minw 8 --maxw 12 --out motifs.json
    '''   MotifFinder.test.exe [fasta] [take] [motifWidth] [topN] [icpcCutoff] [evalueCutoff] [restarts] [maxIterations]
    '''
    ''' 第一个参数为 "em" 时转发给 EmMotif 命令行（test\em_test\Program.vb），
    ''' 其余情况保持原有的 Gibbs findTopN 冒烟测试。
    ''' </summary>
    Sub Main(args As String())
        If args.Length > 0 AndAlso (args(0) = "em" OrElse args(0) = "emtest") Then
            Dim exitCode As Integer = Global.test.EmMotif.Program.Main2(args.Skip(1).ToArray())

            Environment.Exit(exitCode)
            Return
        End If

        Dim path As String = If(args.Length > 0, args(0), DEFAULT_FASTA)
        Dim take As Integer = If(args.Length > 1, Integer.Parse(args(1)), 400)
        Dim width As Integer = If(args.Length > 2, Integer.Parse(args(2)), 12)
        Dim topN As Integer = If(args.Length > 3, Integer.Parse(args(3)), 5)
        Dim icpcCutoff As Double = If(args.Length > 4, Double.Parse(args(4)), -1)
        Dim evalueCutoff As Double = If(args.Length > 5, Double.Parse(args(5)), Double.PositiveInfinity)
        Dim restarts As Integer = If(args.Length > 6, Integer.Parse(args(6)), 0)
        Dim maxIterations As Integer = If(args.Length > 7, Integer.Parse(args(7)), 500)

        Dim data As FastaFile = FastaFile.LoadNucleotideData(path)
        ' CP073066.fasta 之中有 2444 条序列，全量跑一次耗时过长，
        ' 冒烟验证时默认只取其前一部分
        Dim input As FastaSeq() = data.Take(take).ToArray

        Call Console.WriteLine($"loaded {data.Count} sequences from {path}")
        Call Console.WriteLine($"run gibbs sampling on {input.Length} sequences, motif width = {width}")
        Call Console.WriteLine()

        Dim gibbs As New GibbsSampler(input, motifLength:=width)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim top As MSAMotif() = gibbs.findTopN(
            topN:=topN,
            maxIterations:=maxIterations,
            restarts:=restarts,
            maskPadding:=0.5,
            icpcCutoff:=icpcCutoff,
            evalueCutoff:=evalueCutoff
        )

        Call watch.Stop()
        Call Console.WriteLine()
        Call Console.WriteLine($"======== found {top.Length} motifs in {watch.ElapsedMilliseconds} ms ========")

        For Each motif As MSAMotif In top
            Call Console.WriteLine()
            Call Console.WriteLine($"[{motif.rank}] {motif.cost.ToString("F4")} bits/column, e-value = {motif.evalue.ToString("G4")}")
            Call Console.WriteLine($"    consensus : {Consensus(motif)}")
            Call Console.WriteLine($"    sites     : {motif.start.ints.JoinBy(",")}")
        Next

        ' 兼容旧接口：find 依旧只返回信息含量最高的那一个 motif
        Dim best As MSAMotif = gibbs.find(maxIterations:=500)

        Call Console.WriteLine()
        Call Console.WriteLine($"find() top1 : {If(best Is Nothing, "<nothing>", best.cost.ToString("F4") & " bits/column")}")

        Pause()
    End Sub

    ''' <summary>
    ''' 从计数矩阵之中取出每一列上占比最高的碱基，得到 consensus 序列
    ''' </summary>
    Private Function Consensus(motif As MSAMotif) As String
        Dim sb As New StringBuilder()

        For Each col As ints In motif.countMatrix
            Dim max As Integer = -1
            Dim best As Integer = 0

            For j As Integer = 0 To 3
                If col.ints(j) > max Then
                    max = col.ints(j)
                    best = j
                End If
            Next

            Call sb.Append(motif.alphabets(best))
        Next

        Return sb.ToString()
    End Function
End Module
