Namespace Numerics.RootFinding

    ''' <summary>
    ''' 一维求根的结果。
    ''' </summary>
    ''' <remarks>
    ''' <see cref="Converged"/> 为 <c>False</c> 时，<see cref="Value"/> 仍然是一个可用的近似解
    ''' （迭代过程中残差最小的那个点），调用方可以按业务语义决定是接受、放宽容差还是放弃。
    ''' </remarks>
    Public Class RootResult

        ''' <summary>
        ''' 求得的自变量取值
        ''' </summary>
        ''' <returns></returns>
        Public Property Value As Double

        ''' <summary>
        ''' 该点处的残差（<c>f(Value) - target</c>）
        ''' </summary>
        ''' <returns></returns>
        Public Property Residual As Double

        ''' <summary>
        ''' 是否满足容差判据
        ''' </summary>
        ''' <returns></returns>
        Public Property Converged As Boolean

        ''' <summary>
        ''' 是否触达区间边界/最大扩张次数（目标不可达）
        ''' </summary>
        ''' <returns></returns>
        Public Property Capped As Boolean

        ''' <summary>
        ''' 实际迭代次数
        ''' </summary>
        ''' <returns></returns>
        Public Property Iterations As Integer

        ''' <summary>
        ''' 迭代轨迹（仅当 <see cref="RootOptions.RecordHistory"/> 为 <c>True</c> 时非空）
        ''' </summary>
        ''' <returns></returns>
        Public Property History As Double()

        ''' <summary>
        ''' 结束原因（诊断文本，不参与逻辑判断）
        ''' </summary>
        ''' <returns></returns>
        Public Property Message As String

        Public Overrides Function ToString() As String
            Return $"x={Value:G10}, residual={Residual:G6}, converged={Converged}, capped={Capped}, iter={Iterations}"
        End Function

    End Class
End Namespace
