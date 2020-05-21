Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports stdNum = System.Math

Namespace PeakFinding

    ''' <summary>
    ''' 通过累加线的高度落差进行峰识别
    ''' 
    ''' 1. 首先计算出信号的累加线
    ''' 2. 然后删除所有的坡度小于给定角度值的平行段
    ''' 3. 剩下的片段就是信号峰对应的时间s区间
    ''' </summary>
    Public Class ElevationAlgorithm

        ReadOnly cos_angle As Double
        ReadOnly baseline_quantile As Double

        Sub New(angle As Double, baselineQuantile As Double)
            Me.cos_angle = stdNum.Cos(angle)
            Me.baseline_quantile = baselineQuantile
        End Sub

        Public Iterator Function FindAllSignalPeaks(signals As IEnumerable(Of ITimeSignal)) As IEnumerable(Of SeqValue(Of ITimeSignal()))
            Dim data As ITimeSignal() = signals.OrderBy(Function(t) t.time).ToArray
            Dim baseline As Double = data.SignalBaseline(baseline_quantile)
            Dim line As PointF() = data.AccumulateLine(baseline)
            ' 计算出角度
            Dim angles As PointF() = cos(line).ToArray
            ' 删掉所有角度低于阈值的片段
            ' 剩下所有递增的坡度片段
            Dim slopes As SeqValue(Of PointF())() = filterByCosAngles(angles).ToArray
            Dim rawSignals As IVector(Of ITimeSignal) = data.Shadows
            Dim rtmin, rtmax As Double
            Dim time As Vector = rawSignals.Select(Function(t) t.time).AsVector

            For Each region As SeqValue(Of PointF()) In slopes
                rtmin = region.value.First.X
                rtmax = region.value.Last.X

                Yield New SeqValue(Of ITimeSignal()) With {
                    .i = region.i,
                    .value = rawSignals((time >= rtmin) & (time <= rtmax))
                }
            Next
        End Function

        Private Iterator Function filterByCosAngles(angles As PointF()) As IEnumerable(Of SeqValue(Of PointF()))
            Dim buffer As New List(Of PointF)
            Dim i As i32 = 0

            For Each a As PointF In angles
                If a.Y <= cos_angle Then
                    If buffer > 0 Then
                        Yield New SeqValue(Of PointF()) With {
                            .i = ++i,
                            .value = buffer.PopAll
                        }
                    End If
                Else
                    buffer += a
                End If
            Next

            If buffer > 0 Then
                Yield New SeqValue(Of PointF()) With {
                    .i = ++i,
                    .value = buffer.PopAll
                }
            End If
        End Function

        Private Iterator Function cos(line As PointF()) As IEnumerable(Of PointF)
            Dim A As PointF
            Dim B As PointF
            Dim cosB, cosC As Double

            For Each con As SlideWindow(Of PointF) In line.SlideWindows(winSize:=2, offset:=1)
                If con.Length = 2 Then
                    A = con.First
                    B = con.Last
                    cosB = B.X - A.X
                    cosC = EuclideanDistance(New Double() {A.X, A.Y}, New Double() {B.X, B.Y})

                    '      B
                    '     /|
                    '    / |
                    '   /  |
                    ' A ---- X

                    Yield New PointF(A.X, cosB / cosC)
                End If
            Next
        End Function

    End Class
End Namespace