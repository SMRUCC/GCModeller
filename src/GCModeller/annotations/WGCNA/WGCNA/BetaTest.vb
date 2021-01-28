Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
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

    Public Overrides Function ToString() As String
        Return $"[{Power.ToString.PadEnd(2, "0"c)}, score={score.ToString("F2")}] SFT.R.sq:{sftRsq.ToString("F3")}, slope:{slope.ToString("F2")}, truncated.R.sq:{truncatedRsq.ToString("F3")}, mean.K:{meanK.ToString("F2")}, median.K:{medianK.ToString("F2")}, max.k:{maxK.ToString("F2")}"
    End Function

    ''' <summary>
    ''' 利用一元线性回归取匹配最佳β值，即用不同的β值去试验，寻找最佳的β值
    ''' 在线性回归中，我们要求 R^2 大于0.8，slope位于 -1 左右，而平均连接度要尽可能大
    ''' </summary>
    ''' <param name="cor"></param>
    ''' <param name="betaRange"></param>
    ''' <returns>
    ''' 函数返回得分最高的beta值
    ''' </returns>
    Public Shared Function Best(cor As CorrelationMatrix, betaRange As IEnumerable(Of Double), adjacency As Double) As BetaTest
        Dim test As New List(Of BetaTest)
        Dim K As Vector
        Dim linear As FitResult

        For Each beta As Double In betaRange
            K = WeightedNetwork.Connectivity(cor, beta, adjacency)
            ' 基于无尺度分布的假设，我们认为p(ki)与ki呈负相关关系
            linear = SoftLinear.CreateLinear(K)

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

        Dim sftRsq As Vector = test.Select(Function(b) b.sftRsq).AsVector - 0.8
        Dim slope As Vector = test.Select(Function(b) b.slope).AsVector - 1
        Dim meanK As Vector = test.Select(Function(b) b.meanK).AsVector
        Dim score As Vector = sftRsq / sftRsq.Max + slope / slope.Max + meanK / meanK.Max

        Return test(Which.Max(score))
    End Function
End Class
