Namespace StructureLearning

    ''' <summary>
    ''' 结构学习结果
    ''' </summary>
    Public Class StructureLearningResult

        ''' <summary>学习到的网络</summary>
        Public Property Network As Core.BayesianNetwork

        ''' <summary>最终 BIC 评分</summary>
        Public Property FinalBIC As Double

        ''' <summary>迭代次数</summary>
        Public Property Iterations As Integer

        ''' <summary>学习耗时（毫秒）</summary>
        Public Property ElapsedMs As Long

        ''' <summary>每步 BIC 变化记录</summary>
        Public Property BICHistory As New List(Of Double)()

    End Class
End Namespace