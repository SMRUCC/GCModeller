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

        ReadOnly sin_angle As Double
        ReadOnly baseline_quantile As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="angle">这个是一个角度值，取值区间为[0,90]</param>
        ''' <param name="baselineQuantile"></param>
        Sub New(angle As Double, baselineQuantile As Double)
            Me.sin_angle = stdNum.Sin((angle / 90) * (1 / 2 * stdNum.PI))
            Me.baseline_quantile = baselineQuantile
        End Sub

        Public Iterator Function FindAllSignalPeaks(signals As IEnumerable(Of ITimeSignal)) As IEnumerable(Of SignalPeak)
            Dim data As ITimeSignal() = signals.OrderBy(Function(t) t.time).ToArray
            Dim baseline As Double = data.SignalBaseline(baseline_quantile)
            Dim line As IVector(Of PointF) = data.AccumulateLine(baseline).Shadows
            ' 计算出角度
            Dim angles As PointF() = sin(line.Array).ToArray
            ' 删掉所有角度低于阈值的片段
            ' 剩下所有递增的坡度片段
            Dim slopes As SeqValue(Of PointF())() = filterByCosAngles(angles).ToArray
            Dim rawSignals As IVector(Of ITimeSignal) = data.Shadows
            Dim rtmin, rtmax As Double
            Dim time As Vector = rawSignals.Select(Function(t) t.time).AsVector
            Dim area As PointF()

            For Each region As SeqValue(Of PointF()) In slopes
                If region.value.Length = 1 Then
                    Dim t As Single = region.value(Scan0).X
                    Dim i As Integer = Which(angles.Select(Function(a) a.X = t)).First

                    region = New SeqValue(Of PointF()) With {
                        .value = {angles(i - 1), region.value(Scan0), angles(i + 1)}
                    }
                End If

                rtmin = region.value.First.X
                rtmax = region.value.Last.X
                area = line((time >= rtmin) & (time <= rtmax))

                ' 因为Y是累加曲线的值，所以可以近似的看作为峰面积积分值
                ' 在这里将区间的上限的积分值减去区间的下限的积分值即可得到当前的这个区间的积分值（近似于定积分）
                Yield New SignalPeak With {
                    .integration = area.Last.Y - area.First.Y,
                    .region = rawSignals((time >= rtmin) & (time <= rtmax))
                }
            Next
        End Function

        Private Iterator Function filterByCosAngles(angles As PointF()) As IEnumerable(Of SeqValue(Of PointF()))
            Dim buffer As New List(Of PointF)
            Dim i As i32 = 0

            For Each a As PointF In angles
                If a.Y < sin_angle Then
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

        Private Iterator Function sin(line As PointF()) As IEnumerable(Of PointF)
            Dim A As PointF
            Dim B As PointF
            Dim sinA, sinC As Double

            For Each con As SlideWindow(Of PointF) In line.SlideWindows(winSize:=2, offset:=1)
                If con.Length = 2 Then
                    A = con.First
                    B = con.Last
                    sinA = B.Y - A.Y
                    sinC = EuclideanDistance(New Double() {A.X, A.Y}, New Double() {B.X, B.Y})

                    '      B
                    '     /|
                    '    / |
                    '   /  |
                    ' A ---- X

                    Yield New PointF(A.X, sinA / sinC)
                End If
            Next
        End Function

    End Class
End Namespace