' ============================================================================
' Uniop.vb — UniOP 无监督距离模型 [operon.md §1.4]
' ----------------------------------------------------------------------------
' 1. 先验：q = (M − 2O)/(M − O)，M=同链相邻对数，O=反链对数（趋同+发散）。
'    退化截断：M ≤ O → 0.5；q 超出 [0.05, 0.95] 截断。
' 2. 非操纵子距离分布近似：趋对（2 终止子）与发散对（2 启动子）距离成对
'    取算术平均（同链边界 = 1 终止子 + 1 启动子）；稀疏（<10 对）时退回
'    直接使用全部反义对距离（分布偏宽 → 后验保守）。
' 3. 同链距离混合分布：高斯核 KDE，Silverman 带宽 h = 0.9·min(σ, IQR/1.34)·n^(-1/5)。
' 4. 后验（闭式精确贝叶斯）：由混合恒等式 f_mix = q·f_op + (1−q)·f_non：
'       P(op|d) = 1 − (1−q)·f_non(d)/f_mix(d)
'    （文档 §1.4 的"用贝叶斯公式"若直接以混合 KDE 当 f_op，长距离后验下界
'      会是 q——闭式无此缺陷且只用文档已有的三个量；README 已记录。）
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace OperonPredictor.Core

    Public Class UniopModel

        Public ReadOnly QPrior As Double
        Public ReadOnly SameDist As List(Of Double)      ' 同链相邻对 IGD
        Public ReadOnly NonopSample As List(Of Double)   ' 非操纵子距离参照样本
        Public ReadOnly BandwidthMix As Double
        Public ReadOnly BandwidthNon As Double

        Public Sub New(sameDist As List(Of Double), convDist As List(Of Double),
                       divDist As List(Of Double), mCount As Int32, oCount As Int32)
            SameDist = sameDist
            QPrior = ComputePrior(mCount, oCount)

            Dim c = convDist.ToList()
            Dim d = divDist.ToList()
            Shuffle(c)
            Shuffle(d)
            Dim n = Math.Min(c.Count, d.Count)
            If n >= 10 Then
                NonopSample = New List(Of Double)()
                For i = 0 To n - 1
                    NonopSample.Add((c(i) + d(i)) / 2.0)
                Next
            Else
                NonopSample = New List(Of Double)()
                NonopSample.AddRange(convDist)
                NonopSample.AddRange(divDist)
            End If
            If NonopSample.Count = 0 Then NonopSample.Add(200.0)

            BandwidthMix = SilvermanBandwidth(sameDist)
            BandwidthNon = SilvermanBandwidth(NonopSample)
        End Sub

        ''' <summary>[operon.md §1.4] q = (M−2O)/(M−O)，退化截断</summary>
        Public Shared Function ComputePrior(mCount As Int32, oCount As Int32) As Double
            Dim denom = CDbl(mCount) - CDbl(oCount)
            If denom <= 0 Then Return 0.5
            Dim q = (CDbl(mCount) - 2.0 * CDbl(oCount)) / denom
            Return Math.Max(0.05, Math.Min(0.95, q))
        End Function

        Private Sub Shuffle(lst As List(Of Double))
            Dim rng As New Random(7)
            For i = lst.Count - 1 To 1 Step -1
                Dim j = rng.Next(i + 1)
                Dim tmp = lst(i) : lst(i) = lst(j) : lst(j) = tmp
            Next
        End Sub

        ''' <summary>Silverman 经验法则带宽</summary>
        Public Shared Function SilvermanBandwidth(sample As List(Of Double)) As Double
            Dim n = sample.Count
            If n < 2 Then Return 1.0
            Dim mean = sample.Average()
            Dim varr = sample.Sum(Function(x) (x - mean) * (x - mean)) / (n - 1)
            Dim sd = Math.Sqrt(varr)
            Dim sorted = sample.OrderBy(Function(x) x).ToList()
            Dim q1 = sorted(CInt(Math.Floor(0.25 * (n - 1))))
            Dim q3 = sorted(CInt(Math.Floor(0.75 * (n - 1))))
            Dim iqr = q3 - q1
            Dim sig = If(iqr > 0, Math.Min(sd, iqr / 1.34), sd)
            If sig <= 0 Then sig = If(sd > 0, sd, 1.0)
            Return 0.9 * sig * Math.Pow(n, -0.2)
        End Function

        ''' <summary>高斯核 KDE</summary>
        Public Shared Function KdeDensity(sample As List(Of Double), x As Double, bw As Double) As Double
            If sample.Count = 0 OrElse bw <= 0 Then Return 0.0
            Dim s As Double = 0
            Dim norm = 1.0 / (sample.Count * bw * 2.5066282746310002)   ' √(2π)
            For Each v In sample
                Dim z = (x - v) / bw
                s += Math.Exp(-0.5 * z * z)
            Next
            Return s * norm
        End Function

        ''' <summary>
        ''' 闭式后验 P(same-operon | d) = 1 − (1−q)·f_non(d)/f_mix(d)，截断 [0,1]。
        ''' </summary>
        Public Function Posterior(d As Double) As Double
            Dim fMix = KdeDensity(SameDist, d, BandwidthMix)
            If fMix <= 0 Then Return 0.0
            Dim fNon = KdeDensity(NonopSample, d, BandwidthNon)
            Dim p = 1.0 - (1.0 - QPrior) * fNon / fMix
            Return Math.Max(0.0, Math.Min(1.0, p))
        End Function

    End Class

End Namespace
