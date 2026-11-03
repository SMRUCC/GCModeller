Namespace Intervention

    ''' <summary>
    ''' 可组合的「上下文 + 对齐模式 + 诊断」状态容器，供各虚拟扰动模型复用。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这个类型把「统一接口」需要的三件事收在一个地方：当前生效的对齐模式、
    ''' 最近一次对齐的结果与诊断、以及原始（未对齐）的上下文。这样
    ''' <see cref="InsilicoPerturbationExperiment.SetContextAlignmentMode"/> 之后可以
    ''' 直接用同一份原始上下文重新对齐，而不需要调用方重新提供数据。
    ''' </para>
    ''' <para>
    ''' 刻意采用组合而不是基类：11 个模型项目的类层次各不相同（XGBoost、ElasticNet、
    ''' 高斯 BN、ANFIS、GNN、S-system ODE、SNN、扩散模型），引入基类只会制造耦合。
    ''' </para>
    ''' </remarks>
    Public Class ContextAlignmentState

        Private _mode As ContextAlignmentMode
        Private _explicit As Boolean
        Private _raw As Dictionary(Of String, Double)
        Private _reference As ContextAlignmentReference
        Private _options As ContextAlignmentOptions
        Private _result As ContextAlignmentResult

        ''' <summary>
        ''' 创建状态容器。
        ''' </summary>
        ''' <param name="defaultMode">
        ''' 该模型的最优默认对齐模式。用户没有显式调用
        ''' <see cref="SetMode"/> 时一直生效。
        ''' </param>
        Public Sub New(Optional defaultMode As ContextAlignmentMode = ContextAlignmentMode.None)
            DefaultMode = defaultMode
            _mode = defaultMode
        End Sub

        ''' <summary>该模型的最优默认对齐模式（构造时固定）。</summary>
        Public ReadOnly Property DefaultMode As ContextAlignmentMode

        ''' <summary>当前生效的对齐模式。</summary>
        Public Property Mode As ContextAlignmentMode
            Get
                Return _mode
            End Get
            Set(value As ContextAlignmentMode)
                Call SetMode(value)
            End Set
        End Property

        ''' <summary>用户是否显式修改过对齐模式（用于诊断与存档）。</summary>
        Public ReadOnly Property IsModeExplicit As Boolean
            Get
                Return _explicit
            End Get
        End Property

        ''' <summary>最近一次对齐使用的参考统计量；未设置上下文时为 <c>Nothing</c>。</summary>
        Public ReadOnly Property Reference As ContextAlignmentReference
            Get
                Return _reference
            End Get
        End Property

        ''' <summary>最近一次对齐使用的可调参数。</summary>
        Public ReadOnly Property Options As ContextAlignmentOptions
            Get
                Return _options
            End Get
        End Property

        ''' <summary>最近一次对齐的结果；未设置上下文时为 <c>Nothing</c>。</summary>
        Public ReadOnly Property Result As ContextAlignmentResult
            Get
                Return _result
            End Get
        End Property

        ''' <summary>对齐之后的上下文（只包含命中模型基因的条目）。</summary>
        Public ReadOnly Property Values As IReadOnlyDictionary(Of String, Double)
            Get
                If _result Is Nothing Then
                    Return Nothing
                Else
                    Return _result.Values
                End If
            End Get
        End Property

        ''' <summary>原始（未对齐）上下文。</summary>
        Public ReadOnly Property Raw As IReadOnlyDictionary(Of String, Double)
            Get
                If _raw Is Nothing Then
                    Return Nothing
                Else
                    Return _raw
                End If
            End Get
        End Property

        ''' <summary>最近一次对齐的诊断信息。</summary>
        Public ReadOnly Property Diagnostics As ContextAlignmentDiagnostics
            Get
                If _result Is Nothing Then
                    Return Nothing
                Else
                    Return _result.Diagnostics
                End If
            End Get
        End Property

        ''' <summary>是否已经成功注入了上下文。</summary>
        Public ReadOnly Property HasContext As Boolean
            Get
                Return _result IsNot Nothing AndAlso _result.Values.Count > 0
            End Get
        End Property

        ''' <summary>
        ''' 切换对齐模式。若已经注入过上下文，会立即用同一份原始上下文重新对齐。
        ''' </summary>
        ''' <param name="mode">新的对齐模式</param>
        ''' <returns>当前对象（便于链式调用）</returns>
        ''' <exception cref="InvalidOperationException">
        ''' 已经注入上下文、但缺少参考统计量时无法重新对齐
        ''' </exception>
        Public Function SetMode(mode As ContextAlignmentMode) As ContextAlignmentState
            _mode = mode
            _explicit = True

            If _raw IsNot Nothing Then
                If _reference Is Nothing Then
                    Throw New InvalidOperationException(
                        "已经注入了上下文，但缺少参考统计量，无法按新的对齐模式重新对齐；请重新调用 SetContext")
                End If

                _result = ContextAligner.Align(_raw, _reference, _mode, _options)
            End If

            Return Me
        End Function

        ''' <summary>
        ''' 注入上下文并按当前模式对齐。
        ''' </summary>
        ''' <param name="context">基因名 → 表达值（可只覆盖部分基因）</param>
        ''' <param name="reference">模型侧的训练统计量</param>
        ''' <param name="options">可调参数；为空时沿用上一次的设置，否则使用默认值</param>
        ''' <returns>对齐结果（含诊断）</returns>
        ''' <exception cref="InvalidOperationException">所选模式缺少必需的参考统计量</exception>
        Public Function SetContext(context As IDictionary(Of String, Double),
                                   reference As ContextAlignmentReference,
                                   Optional options As ContextAlignmentOptions = Nothing) As ContextAlignmentResult
            If reference Is Nothing Then
                Throw New ArgumentNullException(NameOf(reference))
            End If

            _reference = reference
            _options = If(options, If(_options, ContextAlignmentOptions.Default))
            _raw = copy(context)
            _result = ContextAligner.Align(_raw, _reference, _mode, _options)

            Return _result
        End Function

        ''' <summary>把已注入的上下文展开成与模型基因顺序一致的稠密向量。</summary>
        ''' <param name="fill">未覆盖基因的填充策略</param>
        Public Function ToVector(Optional fill As ContextMissingGeneFill = ContextMissingGeneFill.Mean) As Double()
            If _result Is Nothing OrElse _reference Is Nothing Then
                Return Nothing
            End If

            Return _result.ToVector(_reference, fill)
        End Function

        ''' <summary>清空上下文，回到模型的训练基线。</summary>
        Public Sub Clear()
            _raw = Nothing
            _result = Nothing
        End Sub

        Private Shared Function copy(context As IDictionary(Of String, Double)) As Dictionary(Of String, Double)
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            If context Is Nothing Then
                Return result
            End If

            For Each item As KeyValuePair(Of String, Double) In context
                Dim name As String = If(item.Key, "").Trim()

                If name.Length > 0 Then
                    result(name) = item.Value
                End If
            Next

            Return result
        End Function

        Public Overrides Function ToString() As String
            If Not HasContext Then
                Return $"context alignment: {_mode} (no context)"
            End If

            Return $"context alignment: {_mode}, {_result.Values.Count} gene(s), " &
                   $"coverage={_result.Diagnostics.Coverage:P1}"
        End Function

    End Class

End Namespace
