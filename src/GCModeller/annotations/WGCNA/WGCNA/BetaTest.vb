Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Linq

''' <summary>
''' test for best beta power value
''' </summary>
Public Class BetaTest

    Public Property Power As Double
    Public Property sftRsq As Double
    Public Property slope As Double
    Public Property truncatedRsq As Double
    Public Property meanK As Double
    Public Property medianK As Double
    Public Property maxK As Double

    Public ReadOnly Property score As Double
        Get
            Return (sftRsq - 0.8) - (slope - 1) + meanK
        End Get
    End Property

    ''' <summary>
    ''' 利用一元线性回归取匹配最佳β值，即用不同的β值去试验，寻找最佳的β值
    ''' 在线性回归中，我们要求 R^2 大于0.8，slope位于 -1 左右，而平均连接度要尽可能大
    ''' </summary>
    ''' <param name="cor"></param>
    ''' <param name="betaRange"></param>
    ''' <returns>
    ''' 函数返回得分最高的beta值
    ''' </returns>
    Public Shared Function Best(cor As CorrelationMatrix, betaRange As IEnumerable(Of Double)) As Double
        Dim test As New List(Of BetaTest)
        Dim K As Vector
        Dim pK As Vector
        Dim linear As FitResult

        For Each beta As Double In betaRange
            K = WeightedNetwork.Connectivity(cor, beta)
            pK = WeightedNetwork.Connectivity(cor, beta, pvalue:=True)

            ' 基于无尺度分布的假设，我们认为p(ki)与ki呈负相关关系
            linear = LeastSquares.LinearFit(x:=K, y:=pK)

            test += New BetaTest With {
                .meanK = K.Average,
                .maxK = K.Max,
                .medianK = K.Median,
                .Power = beta,
                .sftRsq = linear.R_square,
                .slope = linear.Slope,
                .truncatedRsq = linear.R_square
            }
        Next

        Return test _
            .OrderByDescending(Function(b) b.score) _
            .First _
            .Power
    End Function
End Class
