' ---------------------------------------------------------------------------
' ParameterSet —— 参数注册表
'
' 把"参数张量 → 优化器状态"的配对集中管理，解决三件在 LLM 训练里必须的事：
'
'   1. 一键清零全部梯度：反传前调用 <see cref="ZeroGradients"/>；
'   2. 全局梯度范数裁剪：<see cref="ClipGradients"/> 一次性处理全部参数的梯度，
'      这样才能保持各参数之间的相对梯度尺度（逐参数裁剪会破坏这个关系）；
'   3. 统一的 AdamW 训练步 + 参数/显存占用统计。
'
' 同时它也是"权重衰减只作用于权重矩阵、不作用于 γ"这一惯例的落点：
' <see cref="Add"/> 的 weightDecay 参数由各层按自身语义给出。
' ---------------------------------------------------------------------------

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace LLM

    ''' <summary>
    ''' 与一组参数张量绑定的优化器状态集合。
    ''' </summary>
    Public Class ParameterSet

        ''' <summary>单个参数的登记项：名称 + 参数张量 + 它的 AdamW 状态。</summary>
        Public Class Entry

            Friend Sub New(name As String, value As Tensor, optimizer As AdamW)
                _name = name
                _value = value
                _optimizer = optimizer
            End Sub

            Private ReadOnly _name As String
            Private ReadOnly _value As Tensor
            Private ReadOnly _optimizer As AdamW

            ''' <summary>参数名称（用于统计输出与调试定位）。</summary>
            Public ReadOnly Property Name As String
                Get
                    Return _name
                End Get
            End Property

            ''' <summary>参数张量本体。</summary>
            Public ReadOnly Property Value As Tensor
                Get
                    Return _value
                End Get
            End Property

            ''' <summary>该参数的 AdamW 优化器状态。</summary>
            Public ReadOnly Property Optimizer As AdamW
                Get
                    Return _optimizer
                End Get
            End Property

            ''' <summary>该参数的梯度累加器。</summary>
            Public ReadOnly Property Gradient As Tensor
                Get
                    Return _optimizer.Gradient
                End Get
            End Property

        End Class

        Private ReadOnly _entries As New List(Of Entry)
        Private ReadOnly _index As New Dictionary(Of String, Entry)

        ''' <summary>登记的参数项（按登记顺序）。</summary>
        Public ReadOnly Property Entries As IList(Of Entry)
            Get
                Return _entries
            End Get
        End Property

        ''' <summary>全部参数的梯度累加器，供全局范数裁剪使用。</summary>
        Public ReadOnly Property Gradients As IEnumerable(Of Tensor)
            Get
                Return _entries.Select(Function(e) e.Gradient)
            End Get
        End Property

        ''' <summary>参数元素总数（即通常所说的"模型参数量"）。</summary>
        Public ReadOnly Property TotalParameters As Long
            Get
                Dim total As Long = 0
                For Each e In _entries
                    total += e.Value.Length
                Next
                Return total
            End Get
        End Property

        ''' <summary>
        ''' 参数本体占用的字节数。
        ''' </summary>
        ''' <remarks>
        ''' 注意这只是"权重"的部分：AdamW 还为每个参数额外持有 m、v、g 三份同形缓冲，
        ''' 因此训练时的实际内存需求约为本值的 4 倍。
        ''' </remarks>
        Public ReadOnly Property TotalBytes As Long
            Get
                Return TotalParameters * 8L
            End Get
        End Property

        ''' <summary>登记一个参数张量。</summary>
        ''' <param name="name">参数名称（同一名称重复登记会抛异常，避免静默覆盖）</param>
        ''' <param name="value">参数张量</param>
        ''' <param name="weightDecay">解耦权重衰减系数；γ / 偏置一类参数应传 0</param>
        Public Function Add(name As String, value As Tensor, Optional weightDecay As Double = 0.0) As Entry
            If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))

            If _index.ContainsKey(name) Then
                Throw New ArgumentException($"参数 '{name}' 已经登记过，不能重复登记")
            End If

            Dim entry As New Entry(name, value, New AdamW(value, weightDecay))

            _entries.Add(entry)
            _index.Add(name, entry)

            Return entry
        End Function

        ''' <summary>按名称查找登记项；不存在时返回 <see langword="Nothing"/>。</summary>
        Public Function Find(name As String) As Entry
            Dim entry As Entry = Nothing
            _index.TryGetValue(name, entry)
            Return entry
        End Function

        ''' <summary>清零全部参数的梯度累加器。</summary>
        Public Sub ZeroGradients()
            For Each e In _entries
                e.Optimizer.ZeroGrad()
            Next
        End Sub

        ''' <summary>
        ''' 对全部参数的梯度做全局 L2 范数裁剪。
        ''' </summary>
        ''' <param name="maxNorm">允许的最大全局范数；&lt;= 0 表示不裁剪</param>
        ''' <returns>裁剪前的全局 L2 范数（用于观测训练稳定性）</returns>
        Public Function ClipGradients(Optional maxNorm As Double = 0.0) As Double
            Return LLMTensorOps.ClipGlobalNorm(Gradients, maxNorm)
        End Function

        ''' <summary>对全部参数执行一次 AdamW 更新。</summary>
        Public Sub Step(learningRate As Double, step As Integer)
            For Each e In _entries
                e.Optimizer.MakeTrainingStep(learningRate, step, e.Value)
            Next
        End Sub

        ''' <summary>
        ''' 输出一份按参数名聚合的参数量统计，便于核对模型规模。
        ''' </summary>
        Public Function Describe() As String
            Const pattern As String = "{0,-48}{1,-22}{2,14:N0}"

            Dim sb As New Text.StringBuilder()
            Dim line As New String("-"c, 84)

            Call sb.AppendLine(String.Format(pattern, "parameter", "shape", "count"))
            Call sb.AppendLine(line)

            For Each e In _entries
                Call sb.AppendLine(String.Format(
                    pattern, e.Name, "[" & String.Join(",", e.Value.Shape) & "]", e.Value.Length))
            Next

            Call sb.AppendLine(line)
            Call sb.AppendLine(String.Format(pattern, "TOTAL", "", TotalParameters))

            Return sb.ToString()
        End Function

    End Class

End Namespace
