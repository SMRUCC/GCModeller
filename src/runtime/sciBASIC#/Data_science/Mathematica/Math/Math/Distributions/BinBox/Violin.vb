Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports std = System.Math

Namespace Distributions.BinBox

    Public Class Violin

        ReadOnly data As Double()
        ReadOnly sd As Double
        ReadOnly nsize As Integer

        Sub New(x As IEnumerable(Of Double))
            data = x.NAremove.ToArray
            sd = data.SD
            nsize = data.Length
        End Sub

        ''' <summary>
        ''' Silverman带宽计算法则
        ''' </summary>
        Private Shared Function CalculateBandwidth(data As List(Of Double)) As Double
            Dim iqr = CalculatePercentile(data, 0.75) - CalculatePercentile(data, 0.25)
            Dim sigma = Min(stdDev, iqr / 1.34)
            Return 0.9 * sigma * Pow(data.Count, -0.2)
        End Function

        ''' <summary>
        ''' 计算核密度估计
        ''' </summary>
        Private Function KDE(yValues As Double(), bandwidth As Double) As Double()
            Dim factor = 1.0 / (nsize * bandwidth)
            Dim sqrt2Pi = std.Sqrt(2 * std.PI)

            Return yValues _
                .Select(Function(y)
                            Return (Aggregate x As Double
                                    In data
                                    Let u = (y - x) / bandwidth
                                    Let var = std.Exp(-0.5 * u * u) / sqrt2Pi
                                    Into Sum(var)) * factor
                        End Function) _
                .ToArray()
        End Function
    End Class
End Namespace