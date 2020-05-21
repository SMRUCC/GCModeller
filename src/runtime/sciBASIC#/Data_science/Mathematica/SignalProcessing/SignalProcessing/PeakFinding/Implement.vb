Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports stdNum = System.Math

Namespace PeakFinding

    Public Module Implement

        ' 算法原理，每当出现一个峰的时候，累加线就会明显升高一个高度
        ' 当升高的时候，曲线的斜率大于零
        ' 当处于基线水平的时候，曲线的斜率接近于零
        ' 则可以利用这个特性将色谱峰给识别出来
        ' 这个方法仅局限于色谱峰都是各自相互独立的情况之下

        ''' <summary>
        ''' 通过这个函数得到的累加线是一个单调递增曲线
        ''' </summary>
        ''' <param name="signals">应该是按照时间升序排序过了的</param>
        ''' <param name="baseline"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AccumulateLine(signals As ITimeSignal(), baseline As Double) As PointF()
            Dim accumulate#
            Dim sumALL# = Aggregate t As ITimeSignal In signals
                          Let x As Double = t.intensity - baseline
                          Where x > 0
                          Into Sum(x)
            Dim ay As Func(Of Double, Double) =
                Function(into As Double) As Double
                    ' 在这里已经消除了本底噪声的影响了
                    into -= baseline
                    accumulate += If(into < 0, 0, into)
                    Return (accumulate / sumALL) * 100
                End Function
            Dim accumulates As PointF() = signals _
                .Select(Function(tick)
                            Return New PointF(tick.time, ay(tick.intensity))
                        End Function) _
                .ToArray

            Return accumulates
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="signals"></param>
        ''' <param name="quantile">一般建议值为0.65</param>
        ''' <returns></returns>
        <Extension>
        Public Function SignalBaseline(signals As ITimeSignal(), quantile As Double) As Double
            Dim allIntensity As Double() = signals.Select(Function(t) t.intensity).ToArray
            Dim q As QuantileEstimationGK = allIntensity.GKQuantile
            Dim baseline As Double = q.Query(quantile)

            Return baseline
        End Function

        <Extension>
        Public Iterator Function Triming(regions As IEnumerable(Of SeqValue(Of ITimeSignal())), peakwidth As DoubleRange) As IEnumerable(Of SeqValue(Of ITimeSignal()))
            Dim rt As Double
            Dim rtmin, rtmax As Double
            Dim dt As Double
            Dim halfPeakWidth As Double = peakwidth.Length / 2

            For Each region In regions
                rt = region.value(Which.Max(region.value.Select(Function(a) a.intensity))).time
                rtmin = region.value.First.time
                rtmax = region.value.Last.time
                dt = rtmax - rtmin

                If dt < peakwidth.Min Then
                    Yield region
                ElseIf dt > peakwidth.Max Then
                    rtmin = stdNum.Max(rtmin, rt - halfPeakWidth)
                    rtmax = stdNum.Min(rtmax, rt + halfPeakWidth)

                    Yield New SeqValue(Of ITimeSignal()) With {
                        .i = region.i,
                        .value = region.value _
                            .Where(Function(a)
                                       Return a.time >= rtmin AndAlso a.time <= rtmax
                                   End Function) _
                            .ToArray
                    }
                Else
                    Yield region
                End If
            Next
        End Function
    End Module
End Namespace