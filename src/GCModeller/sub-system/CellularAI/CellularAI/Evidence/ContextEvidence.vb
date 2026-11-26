Namespace Evidence

    ''' <summary>
    ''' 某个上下文的池化证据：log2 效应矩阵与显著性矩阵，外加诊断信息。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 命名上刻意避开 <c>PooledEvidence</c>：<c>Microsoft.VisualBasic.Math.Statistics.MetaAnalysis</c>
    ''' 里已经有一个同名类型（单基因的随机效应合并结果），两者语义不同，
    ''' 同时导入两个命名空间会造成歧义。
    ''' </para>
    ''' <para>
    ''' <see cref="L2"/> 与 <see cref="Z"/> 是后续所有阶段（计划、残差、设计、合成）的输入；
    ''' <see cref="L2Tot"/> / <see cref="ZTot"/> 在"单证据流"模式下与前者相同
    ''' （保留两套字段是为了与上游的 <c>Evidence</c> 结构对齐）。
    ''' </para>
    ''' </remarks>
    Public Class ContextEvidence

        ''' <summary>上下文相关的 log2 效应（基因×扰动）；<c>NaN</c> 表示没有任何证据。</summary>
        Public Property L2 As Double()()

        ''' <summary>合并后的显著性 <c>|z|</c>（基因×扰动）。</summary>
        Public Property Z As Double()()

        ''' <summary>总量设计使用的 log2 效应（基因×扰动）。</summary>
        Public Property L2Tot As Double()()

        ''' <summary>总量设计使用的显著性（基因×扰动）。</summary>
        Public Property ZTot As Double()()

        ''' <summary>护栏计数快照：<c>名称 → {命中数, 总数, 比例}</c>。</summary>
        Public Property Guards As Dictionary(Of String, Double())

        ''' <summary>完全未测的扰动名。</summary>
        Public Property Unmeasured As String()

        ''' <summary>基因数。</summary>
        Public ReadOnly Property GeneCount As Integer
            Get
                Return If(L2 Is Nothing, 0, L2.Length)
            End Get
        End Property

        ''' <summary>扰动数。</summary>
        Public ReadOnly Property PerturbationCount As Integer
            Get
                Return If(L2 Is Nothing OrElse L2.Length = 0, 0, L2(0).Length)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"context evidence: {GeneCount} gene x {PerturbationCount} pert, {If(Unmeasured Is Nothing, 0, Unmeasured.Length)} unmeasured"
        End Function

    End Class
End Namespace
