Namespace Intervention

    ''' <summary>上下文中未覆盖的模型基因应该如何取值。</summary>
    Public Enum ContextMissingGeneFill
        ''' <summary>使用该基因的训练均值（模型空间的“对照水平”，推荐）。</summary>
        Mean
        ''' <summary>使用该基因在训练数据中的最小值（幂律类模型推荐）。</summary>
        Minimum
        ''' <summary>填零。</summary>
        Zero
    End Enum

    ''' <summary>上下文取值落在训练支持域之外时的处理方式。</summary>
    Public Enum ContextOutOfSupportPolicy
        ''' <summary>仅统计数量并写入诊断（默认；不改变取值）。</summary>
        Report
        ''' <summary>钳制到支持域的上下界，并记录被钳制的基因数。</summary>
        Clamp
        ''' <summary>直接抛出异常，拒绝在不可信的支持域之外做推断。</summary>
        Reject
    End Enum

    ''' <summary>
    ''' 上下文对齐的可调参数。
    ''' </summary>
    ''' <remarks>
    ''' 默认值对应各模型项目历史上的行为：不填充缺失基因（未覆盖的基因沿用模型基线）、
    ''' 越界只报告、负值裁零、常量基因判据 <c>1e-12</c>。
    ''' </remarks>
    Public Class ContextAlignmentOptions

        ''' <summary>
        ''' 是否在 <see cref="ContextAlignmentResult.Values"/> 中补齐未覆盖的模型基因。
        ''' </summary>
        ''' <remarks>
        ''' 默认 <c>False</c>：未覆盖的基因不出现在结果里，模型侧继续沿用训练基线。
        ''' 需要稠密向量的实现（例如把上下文铺成滑窗张量）应改用
        ''' <see cref="ContextAlignmentResult.ToVector"/>。
        ''' </remarks>
        Public Property FillMissingGenes As Boolean = False

        ''' <summary>补齐未覆盖基因时使用的取值策略（仅在 <see cref="FillMissingGenes"/> 开启时生效）。</summary>
        Public Property MissingGeneFill As ContextMissingGeneFill = ContextMissingGeneFill.Mean

        ''' <summary>支持域越界的处理方式。</summary>
        Public Property OutOfSupport As ContextOutOfSupportPolicy = ContextOutOfSupportPolicy.Report

        ''' <summary>
        ''' 支持域判定的容差：落在 <c>[min - tol, max + tol]</c> 之外才算越界。
        ''' </summary>
        Public Property SupportTolerance As Double = 1.0R

        ''' <summary>是否把对齐结果裁剪为非负（生物学意义上表达量不能为负）。</summary>
        ''' <remarks>
        ''' 模型空间若允许负值（z-score / 对数空间），应显式设为 <c>False</c>。
        ''' </remarks>
        Public Property ClipNegative As Boolean = True

        ''' <summary>常量基因判据：种群极差小于该值时视为常量，直接映射为该常数。</summary>
        Public Property ConstantTolerance As Double = 1.0E-12R

        ''' <summary>默认参数（每次调用返回新实例，避免被调用方意外修改共享状态）。</summary>
        Public Shared ReadOnly Property [Default] As ContextAlignmentOptions
            Get
                Return New ContextAlignmentOptions()
            End Get
        End Property

    End Class

End Namespace
