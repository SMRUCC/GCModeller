Namespace ParameterLearning

    ''' <summary>
    ''' 参数学习结果
    ''' </summary>
    Public Class ParameterLearningResult

        ''' <summary>拟合后的网络（含CPD参数）</summary>
        Public Property Network As Core.BayesianNetwork

        ''' <summary>总对数似然</summary>
        Public Property TotalLogLikelihood As Double

        ''' <summary>总 BIC</summary>
        Public Property TotalBIC As Double

        ''' <summary>平均 R²</summary>
        Public Property AverageRSquared As Double

        ''' <summary>参数学习耗时（毫秒）</summary>
        Public Property ElapsedMs As Long

    End Class
End Namespace