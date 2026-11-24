Imports System
Imports System.Collections.Generic
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config

Namespace Solver

    ''' <summary>
    ''' 一次稳态求解的退出原因。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 原来的实现只有一个 <c>Converged</c> 布尔值，于是「步数耗尽」「发散」「极限环」
    ''' 「贴在支持域边界上」这几种完全不同的结局被压成同一个 <c>False</c>，
    ''' 调用方无法区分「模型不稳」和「只是没等够」。
    ''' </para>
    ''' <para>
    ''' <see cref="Converged"/> 是唯一的成功口径：只有 <c>FreeResidual &lt; tolerance</c>
    ''' 才会落到这里。被钳制基因的约束违背量（<see cref="SteadyStateReport.ClampResidual"/>）
    ''' 不参与该判定——约束本来就不满足 <c>F(z)=z</c>，要求它收敛是范畴错误。
    ''' </para>
    ''' </remarks>
    Public Enum ExitReason
        ''' <summary>自由基因的残差已低于容差。</summary>
        Converged = 0
        ''' <summary>步数预算耗尽，仍未达到容差。</summary>
        MaxIterations = 1
        ''' <summary>残差持续放大，判定为发散。</summary>
        Diverged = 2
        ''' <summary>检出极限环（周期 &gt; 1）；周期均值可作为稳态代理。</summary>
        LimitCycle = 3
        ''' <summary>有相当比例的基因被压在支持域边界上，结论不可采信。</summary>
        SupportSaturated = 4
        ''' <summary>线性子问题无法求解（Jacobian 病态/奇异），约束不可行。</summary>
        ConstraintInfeasible = 5
    End Enum

    ''' <summary>
    ''' 一次稳态求解的完整诊断（被轨迹、扫描、导出共用，避免各处重复定义口径）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="FreeResidual"/> 与 <see cref="ClampResidual"/> 的拆分是本轮修复的核心：
    ''' 前者只统计**未被钳制**的基因，用于判收敛；后者只统计被敲除/敲低基因的
    ''' <c>|F_k(z) - clamp_k|</c>，纯诊断（它恒大于 0，因为约束本来就不在 <c>F</c> 的不动点上）。
    ''' </para>
    ''' <para>
    ''' 所有字段都是「只读快照」语义：求解结束后不再变化，可以安全地随轨迹一起导出。
    ''' </para>
    ''' </remarks>
    Public Class SteadyStateReport

        ''' <summary>本次求解实际使用的策略（Auto 会被解析成具体值）。</summary>
        Public Property Mode As SolverMode

        ''' <summary>求解实际采用的迭代轮数（牛顿轮 / 阻尼步 / 时间步）。</summary>
        Public Property Iterations As Integer

        ''' <summary>Jacobian 被构建的次数（牛顿轮数；阻尼模式为 0）。</summary>
        Public Property JacobianEvaluations As Integer

        ''' <summary>
        ''' 自由基因的最大残差 <c>max_{g∉clamp} |F_g(z) - z_g|</c>（z 空间）。
        ''' </summary>
        Public Property FreeResidual As Double

        ''' <summary>
        ''' 被钳制基因的约束违背量 <c>max_{k∈clamp} |F_k(z) - clamp_k|</c>（z 空间，仅诊断）。
        ''' </summary>
        Public Property ClampResidual As Double

        ''' <summary>本步中至少有一个调控因子取值被夹到支持域边界的基因数。</summary>
        Public Property OutOfSupportCount As Integer

        ''' <summary>本步中预测值贴在输出支持域上下界的基因数。</summary>
        Public Property PinnedToBoundCount As Integer

        ''' <summary>承担最大残差的基因名（诊断「到底是哪个基因在拖后腿」）。</summary>
        Public Property DominantGene As String

        ''' <summary>检出极限环时的周期长度；0 表示没有检出。</summary>
        Public Property AttractorPeriod As Integer

        ''' <summary>极限环的周期均值（原始表达量）；没有检出时为 Nothing。</summary>
        Public Property AttractorMean As Double()

        ''' <summary>
        ''' 终态是怎么"稳定下来"的：<c>fixed point</c> / <c>limit cycle</c> /
        ''' <c>fcm surrogate</c> / <c>tail mean</c>（空串 = 没有稳定下来）。
        ''' </summary>
        ''' <remarks>
        ''' 这个字段是给下游判断"能不能把这个数当稳态用"的唯一开关：
        ''' 只有 <c>fixed point</c> 是真正的不动点，其余三种都是代理值。
        ''' </remarks>
        Public Property StabilizedBy As String

        ''' <summary>逐轮的自由残差历史（长度 = <see cref="Iterations"/>）。</summary>
        Public Property ResidualHistory As List(Of Double) = New List(Of Double)()

        ''' <summary>终态（原始表达量）。</summary>
        Public Property FinalState As Double()

        ''' <summary>求解耗时（毫秒）。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>求解过程中的降级/告警说明（例如「Newton 回落到 Anderson」）。</summary>
        Public Property Messages As List(Of String) = New List(Of String)()

        ''' <summary>是否成功收敛到不动点（唯一的成功判据）。</summary>
        Public ReadOnly Property Converged As Boolean
            Get
                Return ExitReason = ExitReason.Converged
            End Get
        End Property

        ''' <summary>
        ''' 退出原因。
        ''' </summary>
        ''' <remarks>
        ''' 默认值刻意取 <see cref="ExitReason.MaxIterations"/> 而不是枚举的 0 值（Converged）：
        ''' 求解器如果在某条路径上忘了显式赋值，默认值必须是「成功」以外的哨兵，
        ''' 否则会静默地把「预算耗尽」报成收敛（阻尼回落路径踩过这个坑）。
        ''' </remarks>
        Public Property ExitReason As ExitReason = ExitReason.MaxIterations

        ''' <summary>追加一条说明（去重，避免同一句告警刷屏）。</summary>
        Public Sub AddMessage(message As String)
            If String.IsNullOrEmpty(message) Then
                Return
            End If
            If Not Messages.Contains(message) Then
                Call Messages.Add(message)
            End If
        End Sub

        ''' <summary>记录一轮残差（同时刷新 <see cref="FreeResidual"/>）。</summary>
        Public Sub PushResidual(residual As Double)
            FreeResidual = residual
            Call ResidualHistory.Add(residual)
        End Sub

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"solver={Mode}: {ExitReason}, iterations={Iterations}, " &
                   $"free={FreeResidual:G3}, clamp={ClampResidual:G3}, " &
                   $"pinned={PinnedToBoundCount}, out-of-support={OutOfSupportCount}"
        End Function

    End Class

End Namespace
