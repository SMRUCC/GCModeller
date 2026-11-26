Namespace Numerics.RootFinding

    ''' <summary>
    ''' 一维求根器的迭代控制参数。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 收敛判据为 <c>|x_new - x_old| &lt;= AbsoluteTolerance + RelativeTolerance * |x_new|</c>，
    ''' 同时接受残差判据 <c>|f(x)| &lt;= AbsoluteTolerance</c>（当函数量纲远小于变量量纲时更有意义）。
    ''' </para>
    ''' <para>
    ''' 所有求根入口在<b>不收敛时都不抛异常</b>：它们返回带 <see cref="RootResult.Converged"/> /
    ''' <see cref="RootResult.Capped"/> 标志的结果对象，把"是否接受这个解"的决定权留给调用方。
    ''' 这是数值库与业务代码之间的契约边界。
    ''' </para>
    ''' </remarks>
    Public Class RootOptions

        ''' <summary>
        ''' 最大迭代次数，默认 100
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxIterations As Integer = 100

        ''' <summary>
        ''' 绝对容差，默认 1e-12
        ''' </summary>
        ''' <returns></returns>
        Public Property AbsoluteTolerance As Double = 0.000000000001

        ''' <summary>
        ''' 相对容差，默认 1e-10
        ''' </summary>
        ''' <returns></returns>
        Public Property RelativeTolerance As Double = 0.0000000001

        ''' <summary>
        ''' 区间扩张的倍数，默认 2
        ''' </summary>
        ''' <returns></returns>
        Public Property ExpansionFactor As Double = 2.0

        ''' <summary>
        ''' 区间扩张的最大次数，默认 60（<c>2^60</c> 已远超任何实际量纲）
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxExpansions As Integer = 60

        ''' <summary>
        ''' 是否记录迭代轨迹（只在调试/审计时打开，打开后会分配与迭代次数等长的数组）
        ''' </summary>
        ''' <returns></returns>
        Public Property RecordHistory As Boolean = False

        ''' <summary>
        ''' 默认参数（每次返回新实例，避免调用方共享可变状态）
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function [Default]() As RootOptions
            Return New RootOptions
        End Function

    End Class
End Namespace
