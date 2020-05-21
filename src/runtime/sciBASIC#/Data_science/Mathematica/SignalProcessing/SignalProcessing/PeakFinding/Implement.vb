Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PeakFinding

    Public Module Implement

        ' 算法原理，每当出现一个峰的时候，累加线就会明显升高一个高度
        ' 当升高的时候，曲线的斜率大于零
        ' 当处于基线水平的时候，曲线的斜率接近于零
        ' 则可以利用这个特性将色谱峰给识别出来
        ' 这个方法仅局限于色谱峰都是各自相互独立的情况之下

        <Extension>
        Public Function AccumulateLine(chromatogram As IEnumerable(Of ITimeSignal), baseline#) As PointF()
            Dim accumulate#
            Dim sumALL# = Aggregate t As ITimeSignal In chromatogram
                          Let x As Double = t.intensity - baseline
                          Where x > 0
                          Into Sum(x)
            Dim ay As Func(Of Double, Double) =
                Function(into As Double) As Double
                    into -= baseline
                    accumulate += If(into < 0, 0, into)
                    Return (accumulate / sumALL) * 100
                End Function
            Dim accumulates As PointF() = chromatogram _
                .Select(Function(tick)
                            Return New PointF(tick.time, ay(tick.intensity))
                        End Function) _
                .ToArray

            Return accumulates
        End Function
    End Module
End Namespace