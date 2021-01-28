Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' 把连通性分隔，分隔内连通性的平均值取log10，跟频率的概率取log10，两者之间有线性关系。
''' </summary>
Module SoftLinear

    Public Function CreateLinear(k As Vector) As FitResult
        Dim cut1 = CutBins.FixedWidthBins(k, 10).ToArray
        Dim bin As Vector = cut1.Select(Function(b) b.Raw.Average).AsVector
        Dim freq1 As Vector = 0.00000001 + New Vector(cut1.Select(Function(b) b.Count)) / k.Dim

        Return LeastSquares.LinearFit(bin.Log(10), freq1.Log(10))
    End Function
End Module
