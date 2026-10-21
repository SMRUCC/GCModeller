Imports System.Drawing
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Interpolation
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' The numerical derivative estimation of the gene expression time series data.
    ''' (基因表达时序数据的数值微分，即 method.md 之中的 s_diff 步骤)
    ''' </summary>
    ''' <remarks>
    ''' 通过对表达量做三次样条插值之后再求斜率，可以把ODE拟合问题转换为
    ''' 针对斜率的回归问题，从而避免对整个系统做反复的数值积分。
    ''' </remarks>
    Public Module SlopeEstimator

        ''' <summary>
        ''' 估计每一个基因在每一个时间点之上的斜率 dX/dt
        ''' </summary>
        ''' <param name="series"></param>
        ''' <param name="denoise">
        ''' 是否先做三次样条平滑之后再求斜率；当时间点的数量不足（&lt; 4）的时候会自动退化为直接的中心差分
        ''' </param>
        ''' <returns>第一维是基因、第二维是时间点的斜率矩阵</returns>
        Public Function Estimate(series As ExpressionTimeSeries, Optional denoise As Boolean = True) As Double()()
            If series Is Nothing Then
                Throw New ArgumentNullException(NameOf(series))
            End If

            Dim n As Integer = series.nGenes
            Dim m As Integer = series.nTimePoints
            Dim t As Double() = series.TimePoints
            Dim slopes As Double()() = New Double(n - 1)() {}

            If m < 2 Then
                Throw New ArgumentException("At least 2 time points are required by the slope estimation!")
            End If

            Dim dt As Double = minInterval(t)
            Dim h As Double = dt / 10

            For i As Integer = 0 To n - 1
                Dim y As Double() = series.X(i)

                If denoise AndAlso m >= 4 Then
                    slopes(i) = splineSlope(t, y, h)
                Else
                    slopes(i) = centralDifference(t, y)
                End If
            Next

            Return slopes
        End Function

        Private Function minInterval(t As Double()) As Double
            Dim dt As Double = Double.PositiveInfinity

            For i As Integer = 1 To t.Length - 1
                Dim d As Double = t(i) - t(i - 1)

                If d > 0 AndAlso d < dt Then
                    dt = d
                End If
            Next

            If Double.IsInfinity(dt) Then
                dt = 1
            End If

            Return dt
        End Function

        ''' <summary>
        ''' 直接的中心差分
        ''' </summary>
        Private Function centralDifference(t As Double(), y As Double()) As Double()
            Dim m As Integer = y.Length
            Dim s As Double() = New Double(m - 1) {}

            For k As Integer = 0 To m - 1
                If k = 0 Then
                    s(k) = (y(1) - y(0)) / (t(1) - t(0))
                ElseIf k = m - 1 Then
                    s(k) = (y(m - 1) - y(m - 2)) / (t(m - 1) - t(m - 2))
                Else
                    s(k) = (y(k + 1) - y(k - 1)) / (t(k + 1) - t(k - 1))
                End If
            Next

            Return s
        End Function

        ''' <summary>
        ''' 先做三次样条插值，再在插值函数之上做中心差分求斜率
        ''' </summary>
        Private Function splineSlope(t As Double(), y As Double(), h As Double) As Double()
            Dim m As Integer = y.Length
            Dim s As Double() = New Double(m - 1) {}

            Try
                Dim points As PointF() = New PointF(m - 1) {}

                For k As Integer = 0 To m - 1
                    points(k) = New PointF(CSng(t(k)), CSng(y(k)))
                Next

                Dim spline As New CubicSpline(points)

                Call spline.CalcSpline()

                Dim t0 As Double = t(0)
                Dim t1 As Double = t(m - 1)

                For k As Integer = 0 To m - 1
                    Dim a As Double = t(k) - h
                    Dim b As Double = t(k) + h
                    Dim clamped As Boolean = False

                    If a < t0 Then
                        a = t0
                        clamped = True
                    End If
                    If b > t1 Then
                        b = t1
                        clamped = True
                    End If

                    If clamped AndAlso b - a < 1.0E-12 Then
                        s(k) = centralDifference(t, y)(k)
                    Else
                        Dim ya As Double = spline.GetPoint(CSng(a)).Y
                        Dim yb As Double = spline.GetPoint(CSng(b)).Y

                        s(k) = (yb - ya) / (b - a)
                    End If

                    If Double.IsNaN(s(k)) OrElse Double.IsInfinity(s(k)) Then
                        s(k) = 0
                    End If
                Next
            Catch ex As Exception
                ' 样条插值失败的时候退化为直接的中心差分
                Return centralDifference(t, y)
            End Try

            Return s
        End Function
    End Module
End Namespace
