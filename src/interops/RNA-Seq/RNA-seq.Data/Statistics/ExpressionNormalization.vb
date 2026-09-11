' /********************************************************************************/
'
'  RNA-seq.Data —— 表达归一化工具（可复用）
'
'  复刻 Rockhopper 论文（gkt444.pdf / 13059_2014_Article_572.pdf）的定量策略：
'    * 上四分分位归一化：以样本全部基因表达水平的上四分位数作为归一化因子，
'      相比总读段数对高表达基因（如细菌 rRNA）不敏感，更稳健；
'    * 改良 RPKM：以样本上四分位数替代传统 RPKM 中的总读段数；
'    * Benjamini-Hochberg 过程：把差异表达 p 值校正为 q 值，控制 FDR。
'
'  本模块独立于 Rockhopper，可供其它 RNA-seq 流程直接复用。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq

Namespace RNA_Seq.Statistics

    ''' <summary>
    ''' 表达量归一化与多重检验校正。
    ''' </summary>
    Public Module ExpressionNormalization

        ''' <summary>
        ''' 上四分位数（75% 分位，线性插值）。
        ''' </summary>
        Public Function UpperQuartile(values As IEnumerable(Of Double)) As Double
            Return Percentile(values, 0.75)
        End Function

        ''' <summary>
        ''' 任意分位数（线性插值）。
        ''' </summary>
        Public Function Percentile(values As IEnumerable(Of Double), quantile As Double) As Double
            If values Is Nothing Then Return 0.0
            Dim sorted As Double() = values.Where(Function(v) Not Double.IsNaN(v)).OrderBy(Function(v) v).ToArray()
            If sorted.Length = 0 Then Return 0.0
            If sorted.Length = 1 Then Return sorted(0)

            Dim rank As Double = quantile * (sorted.Length - 1)
            Dim lo As Integer = CInt(System.Math.Floor(rank))
            Dim hi As Integer = CInt(System.Math.Ceiling(rank))
            If lo = hi Then Return sorted(lo)

            Dim frac As Double = rank - lo
            Return sorted(lo) + frac * (sorted(hi) - sorted(lo))
        End Function

        ''' <summary>
        ''' 改良 RPKM：counts × 1e9 / (geneLength × normFactor)。
        ''' </summary>
        ''' <param name="counts">映射到基因的读段数。</param>
        ''' <param name="geneLength">基因长度（bp）。</param>
        ''' <param name="normFactor">归一化因子（通常为样本的上四分位数表达水平）。</param>
        Public Function ModifiedRPKM(counts As Double, geneLength As Integer, normFactor As Double) As Double
            If geneLength <= 0 OrElse normFactor <= 0.0 Then Return 0.0
            Return counts * 1000000000.0 / (geneLength * normFactor)
        End Function

        ''' <summary>
        ''' 改良 RPKM 的批量版本。
        ''' </summary>
        Public Function ModifiedRPKM(counts As IEnumerable(Of Double), geneLength As Integer, normFactor As Double) As Double()
            Return counts.Select(Function(c) ModifiedRPKM(c, geneLength, normFactor)).ToArray()
        End Function

        ''' <summary>
        ''' Benjamini-Hochberg 校正：返回与输入同序的 q 值数组。
        ''' </summary>
        ''' <remarks>
        ''' 算法与原始 Rockhopper 的 Gene.correctPvalues 一致：
        ''' 先按 p 值升序排列，q_i = min(1, p_i × n / (i + 1))，并保持单调不减。
        ''' </remarks>
        Public Function BenjaminiHochberg(pValues As IEnumerable(Of Double)) As Double()
            Dim p As Double() = pValues.ToArray()
            Dim n As Integer = p.Length
            If n = 0 Then Return New Double() {}

            Dim order As Integer() = Enumerable.Range(0, n).OrderBy(Function(i) p(i)).ToArray()
            Dim q As Double() = New Double(n - 1) {}

            Dim previous As Double = 0.0
            For k As Integer = 0 To n - 1
                Dim idx As Integer = order(k)
                Dim value As Double = p(idx) * n / (k + 1)
                value = System.Math.Min(value, 1.0)
                value = System.Math.Max(value, previous)
                previous = value
                q(idx) = value
            Next

            ' 按输入的原始顺序回填（order 已升序，这里再按索引排序还原）
            Dim result As Double() = New Double(n - 1) {}
            For k As Integer = 0 To n - 1
                result(order(k)) = q(k)
            Next
            Return result
        End Function

    End Module

End Namespace
