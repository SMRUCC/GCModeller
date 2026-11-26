Namespace Summary

    ''' <summary>
    ''' 一条训练记录的对照表达剖面（该细胞系所有对照细胞按基因求和）。
    ''' </summary>
    Public Class ControlProfile

        ''' <summary>基因轴。</summary>
        Public Property Genes As String()

        ''' <summary>逐基因的对照计数总和。</summary>
        Public Property Counts As Double()

        ''' <summary>基因数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(Genes Is Nothing, 0, Genes.Length)
            End Get
        End Property

        ''' <summary>计数总和（库大小）；为 0 时按 1 处理以避免除零。</summary>
        Public ReadOnly Property Total As Double
            Get
                Dim sum As Double = 0.0

                For i As Integer = 0 To Count - 1
                    sum += Counts(i)
                Next

                Return sum
            End Get
        End Property

        ''' <summary>换算成 CPM：<c>1e6·counts / total</c>。</summary>
        Public Function Cpm() As Double()
            Dim result(If(Count = 0, -1, Count - 1)) As Double
            Dim total As Double = Me.Total

            If total <= 0.0 Then
                total = 1.0
            End If

            For i As Integer = 0 To Count - 1
                result(i) = 1000000.0 * Counts(i) / total
            Next

            Return result
        End Function

        Public Sub Validate()
            If Genes Is Nothing OrElse Counts Is Nothing Then
                Throw New ArgumentException("the control profile is incomplete", NameOf(Genes))
            End If
            If Genes.Length <> Counts.Length Then
                Throw New ArgumentException(
                    $"the control profile has {Genes.Length} gene(s) but {Counts.Length} count(s)", NameOf(Counts))
            End If
        End Sub

    End Class

    ''' <summary>
    ''' 一条训练摘要：某个细胞系、某个视图下的效应矩阵、概率矩阵与对照剖面。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这里刻意保留<b>该行自己的基因轴与扰动轴</b>（而不是提前对齐到任务轴）：
    ''' 上游的 <c>log2fc_stack</c> 要在"训练基因的并集"上算相似度与方差，
    ''' 提前裁剪到任务轴会改变高变基因的选择结果。
    ''' </para>
    ''' <para>
    ''' 缺失用 <c>NaN</c> 表示（"未测"），与"测得 0"严格区分：
    ''' 池化时未测项按"值为 0、权重照算"计入分母，而测得的 0 是真实观测。
    ''' </para>
    ''' </remarks>
    Public Class SummaryRow

        ''' <summary>细胞系（训练行）名称。</summary>
        Public Property CellLine As String

        ''' <summary>视图名称。</summary>
        Public Property View As String

        ''' <summary>角色：<c>primary</c> 或 <c>auxiliary</c>。</summary>
        Public Property Role As String = "primary"

        ''' <summary>配对组名（同一实验的多个记录同名）；无配对时为 <c>Nothing</c>。</summary>
        Public Property PairGroup As String

        ''' <summary>是否为配对组里的交替记录。</summary>
        Public Property PairAlternate As Boolean

        ''' <summary>逐基因中心化表。</summary>
        Public Property Offset As OffsetTable

        ''' <summary>效应矩阵的基因轴。</summary>
        Public Property EffectGenes As String()

        ''' <summary>效应矩阵的扰动轴。</summary>
        Public Property EffectPerts As String()

        ''' <summary>log2FC 效应矩阵（该行自己的基因轴 × 扰动轴）。</summary>
        Public Property Effect As Double()()

        ''' <summary>双侧概率矩阵（与 <see cref="Effect"/> 同形状）。</summary>
        Public Property Probability As Double()()

        ''' <summary>可选的尺度矩阵（与 <see cref="Effect"/> 同形状，用于 sef 因子）。</summary>
        Public Property Scale As Double()()

        ''' <summary>对照表达剖面。</summary>
        Public Property Control As ControlProfile

        ''' <summary>是否为辅助行。</summary>
        Public ReadOnly Property IsAuxiliary As Boolean
            Get
                Return String.Equals(Role, "auxiliary", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        ''' <summary>行的唯一键 <c>view/cell_line</c>。</summary>
        Public ReadOnly Property Key As String
            Get
                Return $"{View}/{CellLine}"
            End Get
        End Property

        ''' <summary>取效应 / 概率 / 尺度矩阵；<paramref name="statistic"/> 取 <c>effect</c> / <c>probability</c> / <c>scale</c>。</summary>
        Public Function Panel(statistic As String) As Double()()
            Select Case (If(statistic, "")).ToLowerInvariant()
                Case "effect"
                    Return Effect
                Case "probability"
                    Return Probability
                Case "scale"
                    Return Scale
                Case Else
                    Throw New ArgumentException($"unknown summary statistic '{statistic}'", NameOf(statistic))
            End Select
        End Function

        ''' <summary>校验形状自洽（缺失的面板允许为 <c>Nothing</c>）。</summary>
        Public Sub Validate()
            If String.IsNullOrEmpty(CellLine) Then
                Throw New ArgumentException("the summary row has no cell line", NameOf(CellLine))
            End If

            If Effect Is Nothing Then
                Return
            End If
            If EffectGenes Is Nothing OrElse EffectPerts Is Nothing Then
                Throw New ArgumentException($"{Key}: the effect panel has no axis", NameOf(EffectGenes))
            End If
            If Effect.Length <> EffectGenes.Length Then
                Throw New ArgumentException($"{Key}: effect rows do not match the gene axis", NameOf(Effect))
            End If

            For i As Integer = 0 To Effect.Length - 1
                If Effect(i) Is Nothing OrElse Effect(i).Length <> EffectPerts.Length Then
                    Throw New ArgumentException(
                        $"{Key}: effect row #{i} does not match the perturbation axis ({EffectPerts.Length})", NameOf(Effect))
                End If
            Next

            If Probability IsNot Nothing Then
                If Probability.Length <> Effect.Length Then
                    Throw New ArgumentException($"{Key}: the probability panel does not match the effect panel", NameOf(Probability))
                End If
            End If
            If Offset IsNot Nothing Then
                Call Offset.Validate()
            End If
            If Control IsNot Nothing Then
                Call Control.Validate()
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Key} [{Role}]: {If(EffectGenes Is Nothing, 0, EffectGenes.Length)} gene x {If(EffectPerts Is Nothing, 0, EffectPerts.Length)} pert"
        End Function

    End Class
End Namespace
