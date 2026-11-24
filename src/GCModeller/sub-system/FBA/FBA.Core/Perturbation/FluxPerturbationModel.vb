Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention

Namespace Perturbation

    ''' <summary>
    ''' FBA 侧对 <see cref="InsilicoPerturbationExperiment"/> 的显式降级适配器。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 通量平衡分析（FBA）描述的是<b>稳态通量</b>：给定化学计量矩阵与通量上下界，
    ''' 求解一个线性规划目标。它既没有“转录组上下文”这一输入，也没有从基因符号到反应的
    ''' 现成映射（本工程只提供 <see cref="LinearProgrammingEngine"/> 与矩阵构造工具）。
    ''' </para>
    ''' <para>
    ''' 因此这个适配器<b>不伪造结果</b>：接口中的上下文与基因扰动一律抛出
    ''' <see cref="NotSupportedException"/> 并给出可操作的建议。这样做的目的是让
    ''' “FBA 不支持上下文对齐”这件事在编译期可见、在运行期可解释，而不是返回一组
    ''' 看起来合理、实则为零的差异值。
    ''' </para>
    ''' <para>
    ''' 需要做通量层面的扰动时，正确路径是：自行构造修改后的化学计量矩阵/边界
    ''' （例如关闭某反应的通量），再调用 <see cref="LinearProgrammingEngine.Run"/>，
    ''' 与基准解相减即可得到反应通量的变化。
    ''' </para>
    ''' </remarks>
    Public Class FluxPerturbationModel : Implements InsilicoPerturbationExperiment

        ''' <summary>底层线性规划求解器（可为 Nothing）。</summary>
        Public Property Engine As LinearProgrammingEngine

        ''' <summary>
        ''' 对齐模式：FBA 没有上下文输入，这里只保留取值以便统一调用方式，
        ''' 不参与任何计算。
        ''' </summary>
        Public ReadOnly Property ContextAlignmentMode As ContextAlignmentMode

        ''' <summary>FBA 没有上下文，诊断恒为 <c>Nothing</c>。</summary>
        Public ReadOnly Property ContextDiagnostics As ContextAlignmentDiagnostics
            Get
                Return Nothing
            End Get
        End Property

        Public Sub New(Optional engine As LinearProgrammingEngine = Nothing)
            Me.Engine = engine
        End Sub

        ''' <summary>
        ''' 不支持：FBA 的输入是化学计量矩阵与通量界，不存在“表达状态上下文”。
        ''' </summary>
        ''' <exception cref="NotSupportedException">总是抛出</exception>
        Public Function SetContext(context As Dictionary(Of String, Double)) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContext
            Throw New NotSupportedException(contextMessage())
        End Function

        ''' <summary>记录对齐模式（FBA 无上下文，模式本身不产生任何效果）。</summary>
        Public Function SetContextAlignmentMode(mode As ContextAlignmentMode) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContextAlignmentMode
            Return Me
        End Function

        ''' <summary><see cref="SetContext"/> 的对应清理操作（空实现）。</summary>
        Public Sub ClearContext()
            ' FBA 没有上下文状态
        End Sub

        ''' <summary>不支持：FBA 不能从基因名推断反应并预测转录响应。</summary>
        ''' <exception cref="NotSupportedException">总是抛出</exception>
        Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
            Throw New NotSupportedException(perturbationMessage(geneName))
        End Function

        ''' <summary>不支持：同 <see cref="KnockoutGene"/>。</summary>
        ''' <exception cref="NotSupportedException">总是抛出</exception>
        Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
            Throw New NotSupportedException(perturbationMessage(geneName))
        End Function

        ''' <summary>不支持：同 <see cref="KnockoutGene"/>。</summary>
        ''' <exception cref="NotSupportedException">总是抛出</exception>
        Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
            Throw New NotSupportedException(perturbationMessage(geneName))
        End Function

        Private Shared Function contextMessage() As String
            Return "FBA 是通量平衡模型：输入是化学计量矩阵与通量上下界，不存在表达状态上下文，无法做上下文对齐。" &
                   "若需要环境条件，请修改边界通量约束后重新求解 LP"
        End Function

        Private Shared Function perturbationMessage(geneName As String) As String
            Return $"FBA 不能把基因 '{geneName}' 映射为反应并预测转录响应：" &
                   "本工程没有基因↔反应的关联表。请直接关闭目标反应的通量界，" &
                   "再调用 LinearProgrammingEngine.Run 与基准解比较"
        End Function

    End Class

End Namespace
