#Region "Microsoft.VisualBasic::880e3b3066656dc01e9624a512a240eb, annotations\Bifrost\Prodigal\PredictionPipeline.vb"

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

    '   Total Lines: 120
    '    Code Lines: 69 (57.50%)
    ' Comment Lines: 28 (23.33%)
    '    - Xml Docs: 39.29%
    ' 
    '   Blank Lines: 23 (19.17%)
    '     File Size: 4.59 KB


    ' Class PredictionPipeline
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ComputeConfidence, Predict
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' ProdigalCore.vb - Prodigal VB.NET 基因预测程序 核心算法
' 包含：ORF查找、编码区打分、RBS检测、起始密码子评分、
'       动态规划基因选择、无监督训练引擎、预测管线
' ============================================================================


Imports SMRUCC.genomics.SequenceModel.FASTA

' ========================================================================
' 基因预测管线
' ========================================================================

''' <summary>
''' 基因预测管线
''' 整合ORF查找、打分、动态规划选择，输出最终预测结果
''' </summary>
Public Class PredictionPipeline

    ''' <summary>最小ORF长度</summary>
    Private _minOrfLength As Integer

    Public Sub New(Optional minOrfLength As Integer = 90)
        _minOrfLength = minOrfLength
    End Sub

    ''' <summary>
    ''' 使用已有模型进行基因预测
    ''' </summary>
    Public Function Predict(sequences As IReadOnlyCollection(Of FastaSeq), model As TrainingModel) As List(Of PredictionResult)
        Dim results As New List(Of PredictionResult)()

        For Each seq In sequences
            Console.WriteLine($"  预测序列: {seq.locus_tag } ({seq.Length:N0} bp)")

            ' 第一步：查找所有候选ORF
            Dim finder As New OrfFinder(_minOrfLength)
            Dim orfs = finder.FindOrfs(seq)
            Console.WriteLine($"    候选ORF数: {orfs.Count}")

            If orfs.Count = 0 Then
                results.Add(New PredictionResult With {
                    .SeqId = seq.locus_tag,
                    .SeqLength = seq.Length,
                    .Model = model
                })
                Continue For
            End If

            ' 第二步：对所有ORF打分
            ScoringEngine.ScoreForSequence(orfs, model, seq.SequenceData)

            ' 第三步：动态规划选择最优基因组合
            Dim selectedOrfs = DynamicProgramming.SelectGenes(orfs)
            Console.WriteLine($"    预测基因数: {selectedOrfs.Count}")

            ' 第四步：构建预测结果
            Dim result As New PredictionResult With {
                .SeqId = seq.locus_tag,
                .SeqLength = seq.Length,
                .Model = model
            }

            Dim geneIndex = 1
            For Each orf As CandidateORF In selectedOrfs.OrderBy(Function(o) o.Start)
                Dim gene As New PredictedGene With {
                    .SeqId = orf.SeqId,
                    .Start = orf.Start,
                    .[End] = orf.[End],
                    .Strand = orf.Strand,
                    .Frame = orf.Frame,
                    .StartCodon = orf.StartCodon,
                    .StopCodon = orf.StopCodon,
                    .Length = orf.Length,
                    .CodingScore = orf.CodingScore,
                    .StartScore = orf.StartScore,
                    .RbsScore = orf.RbsScore,
                    .UpstreamScore = orf.UpstreamScore,
                    .TypeScore = orf.TypeScore,
                    .TotalScore = orf.TotalScore,
                    .AaSequence = orf.AaSequence,
                    .NtSequence = orf.NtSequence,
                    .RbsMotif = orf.RbsMotif,
                    .RbsSpacing = orf.RbsSpacing,
                    .GeneIndex = geneIndex,
                    .PartialType = orf.PartialType,
                    .RawStart = orf.RawStart,
                    .RawEnd = orf.RawEnd
                }

                ' 计算置信度
                gene.Confidence = ComputeConfidence(gene, model)
                result.Genes.Add(gene)
                geneIndex += 1
            Next

            results.Add(result)
        Next

        Return results
    End Function

    ''' <summary>
    ''' 计算基因置信度（0-1之间）
    ''' </summary>
    Private Function ComputeConfidence(gene As PredictedGene, model As TrainingModel) As Double
        ' 基于总得分的置信度
        ' 得分>20为高置信度，<5为低置信度
        Dim scoreConfidence = 1.0 / (1.0 + Math.Exp(-(gene.TotalScore - 10) / 5.0))

        ' 基于编码区得分的置信度
        Dim codingConfidence = 1.0 / (1.0 + Math.Exp(-(gene.CodingScore - 5) / 3.0))

        ' 综合置信度
        Return (scoreConfidence * 0.6 + codingConfidence * 0.4)
    End Function

End Class



