' /********************************************************************************/
'
'  Rockhopper —— 表达定量
'
'  复刻论文（gkt444.pdf / 13059_2014_Article_572.pdf）与 readme.md 第 2 节的归一化策略：
'    * 上四分分位归一化：对每个重复实验，取全部基因读段数的上四分位数(upper quartile)
'      作为该重复的归一化因子；跨全部条件、全部重复取平均得到全局缩放因子
'      （Core.Condition.AvgUpperQuartile）。
'    * 改良 RPKM：以样本上四分位数替代总读段数
'          RPKM = reads / (geneLength × upperQuartile)
'      具体计算在 Core.Gene.ComputeExpression 中完成（沿用原始实现）。
'
'  本模块负责把「原始计数 → 归一化计数 → 均值/RPKM → 差异检验」串起来，
'  对应的数学函数（lowess / 负二项分布 / BH 校正）由 RNA-seq.Data 的
'  Statistics 模块提供，本模块不重复实现。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Rockhopper.Core

Namespace Quantification

    ''' <summary>
    ''' 表达定量流程：上四分位归一化、均值/RPKM 计算与差异表达检验。
    ''' </summary>
    Public Module Quantification

        ''' <summary>
        ''' 为每个条件的每个重复计算上四分位表达水平，并设置全局平均上四分位缩放因子。
        ''' </summary>
        ''' <param name="genomes">全部基因组（其基因已通过 SetRawCount 填好原始计数）。</param>
        ''' <param name="conditions">全部实验条件。</param>
        Public Sub ComputeUpperQuartiles(genomes As List(Of Genome), conditions As List(Of Condition))
            Dim allQuartiles As New List(Of Long)()

            For c As Integer = 0 To conditions.Count - 1
                For r As Integer = 0 To conditions(c).NumReplicates() - 1
                    Dim values As New List(Of Long)()
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.NumGenes() - 1
                            values.Add(genome.GetGene(j).GetRawCount(c, r))
                        Next
                    Next

                    Dim quartile As Long = UpperQuartile(values)
                    conditions(c).GetReplicate(r).UpperQuartile = quartile
                    allQuartiles.Add(quartile)
                Next
            Next

            If allQuartiles.Count > 0 Then
                Condition.AvgUpperQuartile = CLng(System.Math.Round(allQuartiles.Average()))
            End If
        End Sub

        ''' <summary>
        ''' 使用全局缩放因子与各重复的上四分位数计算归一化计数。
        ''' </summary>
        Public Sub SetNormalizedCounts(genomes As List(Of Genome), conditions As List(Of Condition))
            For c As Integer = 0 To conditions.Count - 1
                For r As Integer = 0 To conditions(c).NumReplicates() - 1
                    Dim upperQuartile As Long = conditions(c).GetReplicate(r).UpperQuartile
                    If upperQuartile = 0 Then upperQuartile = 1 ' 避免除零
                    For z As Integer = 0 To genomes.Count - 1
                        Dim genome As Genome = genomes(z)
                        For j As Integer = 0 To genome.NumGenes() - 1
                            genome.GetGene(j).SetNormalizedCount(c, r, CDbl(Condition.AvgUpperQuartile), upperQuartile)
                        Next
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' 计算每个基因在各条件下的均值与 RPKM。
        ''' </summary>
        Public Sub ComputeExpressions(genomes As List(Of Genome), conditions As List(Of Condition))
            For z As Integer = 0 To genomes.Count - 1
                Dim genome As Genome = genomes(z)
                For j As Integer = 0 To genome.NumGenes() - 1
                    genome.GetGene(j).ComputeExpression(conditions)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 计算每个基因在每对条件间的差异表达 p 值。
        ''' </summary>
        Public Sub ComputeDifferentialExpressions(genomes As List(Of Genome), conditions As List(Of Condition))
            For z As Integer = 0 To genomes.Count - 1
                Dim genome As Genome = genomes(z)
                For j As Integer = 0 To genome.NumGenes() - 1
                    genome.GetGene(j).ComputeVariance(conditions)
                    genome.GetGene(j).ComputeDifferentialExpression()
                Next
            Next
        End Sub

        ''' <summary>
        ''' 上四分位数（75% 分位，线性插值）。
        ''' </summary>
        ''' <remarks>
        ''' 复刻原始 Rockhopper 对 "75% most expressed gene" 的取值；此处采用与
        ''' 常见分位定义一致的线性插值，便于与低层统计函数复用。
        ''' </remarks>
        Public Function UpperQuartile(values As IEnumerable(Of Long)) As Long
            Dim sorted As Long() = values.OrderBy(Function(v) v).ToArray()
            If sorted.Length = 0 Then Return 0
            If sorted.Length = 1 Then Return sorted(0)

            Dim rank As Double = 0.75 * (sorted.Length - 1)
            Dim lo As Integer = CInt(System.Math.Floor(rank))
            Dim hi As Integer = CInt(System.Math.Ceiling(rank))
            If lo = hi Then Return sorted(lo)

            Dim frac As Double = rank - lo
            Return CLng(System.Math.Round(sorted(lo) + frac * (sorted(hi) - sorted(lo))))
        End Function

    End Module

End Namespace
