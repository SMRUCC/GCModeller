Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer

Namespace Model

    ''' <summary>
    ''' 一个可训练参数的登记项：值张量、梯度张量与 Adam 状态。
    ''' </summary>
    Public Class ParameterEntry

        ''' <summary>参数名称（全模型唯一，用于调试与梯度检查定位）</summary>
        Public Property Name As String

        ''' <summary>参数值张量（原地更新）</summary>
        Public Property Value As Tensor

        ''' <summary>
        ''' Adam 状态。外部梯度参数（如 <c>GEARSConvLayer</c> 自己持有的梯度张量）
        ''' 为 <c>Nothing</c>，此时由 <see cref="ParameterStore"/> 自行维护一阶/二阶矩。
        ''' </summary>
        Public ReadOnly Property State As Optimizer

        ''' <summary>梯度张量（累加器；反向传播阶段原地累加）</summary>
        Public ReadOnly Property Gradient As Tensor

        ''' <summary>创建参数登记项</summary>
        ''' <param name="name">参数名</param>
        ''' <param name="value">参数值</param>
        ''' <param name="state">Adam 状态；外部梯度参数传 Nothing</param>
        ''' <param name="gradient">梯度累加器</param>
        Public Sub New(name As String, value As Tensor, state As Optimizer, gradient As Tensor)
            Me.Name = name
            Me.Value = value
            Me.State = state
            Me.Gradient = gradient
        End Sub

    End Class

    ''' <summary>
    ''' 全模型统一的参数登记表：集中做梯度清零、全局范数裁剪与 Adam 更新。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 既有 Transformer 组件（<c>MultiHeadAttention</c>/<c>FeedForwardNetwork</c>）把
    ''' Adam 状态封装在层内部，层外无法做"全局梯度范数裁剪"这类需要跨参数聚合的操作；
    ''' 本类型把参数集中登记，从而支持 method.md §5.2 要求的 <c>clip_grad_norm_(1.0)</c>。
    ''' </para>
    ''' <para>
    ''' 支持两种登记方式：
    ''' <list type="bullet">
    ''' <item><description><see cref="Register"/>：本类型持有 Adam 状态与梯度张量；</description></item>
    ''' <item><description><see cref="RegisterExternal"/>：梯度张量由外部层（<c>GEARSConvLayer</c>）
    ''' 自己累加，本类型只借用它做裁剪与更新。这样可以复用既有的图卷积实现而不必重写。</description></item>
    ''' </list>
    ''' </para>
    ''' </remarks>
    Public Class ParameterStore

        Private Const Beta1 As Double = 0.9
        Private Const Beta2 As Double = 0.999
        Private Const Epsilon As Double = 0.00000001

        Private ReadOnly _entries As New List(Of ParameterEntry)()

        ' 外部梯度参数的一阶/二阶矩（键 = 参数名）
        Private ReadOnly _externalM As New Dictionary(Of String, Tensor)()
        Private ReadOnly _externalV As New Dictionary(Of String, Tensor)()

        ''' <summary>全部参数登记项</summary>
        Public ReadOnly Property Entries As List(Of ParameterEntry)
            Get
                Return _entries
            End Get
        End Property

        ''' <summary>参数数量</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return _entries.Count
            End Get
        End Property

        ''' <summary>
        ''' 登记一个由本类型维护 Adam 状态的参数。
        ''' </summary>
        ''' <param name="name">唯一参数名</param>
        ''' <param name="value">参数张量</param>
        ''' <returns>登记项</returns>
        Public Function Register(name As String, value As Tensor) As ParameterEntry
            If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))
            Call EnsureUnique(name)

            Dim state As New Optimizer(value)
            Dim entry As New ParameterEntry(name, value, state, state.Gradient)

            Call _entries.Add(entry)

            Return entry
        End Function

        ''' <summary>
        ''' 登记一个梯度由外部层累加的参数（如 <c>GEARSConvLayer</c> 的权重）。
        ''' </summary>
        ''' <param name="name">唯一参数名</param>
        ''' <param name="value">参数张量（与外部层持有的是同一个对象）</param>
        ''' <param name="gradient">外部层的梯度累加张量（与外部层持有的是同一个对象）</param>
        ''' <returns>登记项</returns>
        Public Function RegisterExternal(name As String, value As Tensor, gradient As Tensor) As ParameterEntry
            If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))
            If gradient Is Nothing Then Throw New ArgumentNullException(NameOf(gradient))
            Call EnsureUnique(name)

            _externalM(name) = New Tensor(value.Shape)
            _externalV(name) = New Tensor(value.Shape)

            Dim entry As New ParameterEntry(name, value, Nothing, gradient)

            Call _entries.Add(entry)

            Return entry
        End Function

        ''' <summary>按名称查找登记项；不存在时返回 <c>Nothing</c>。</summary>
        ''' <param name="name">参数名</param>
        ''' <returns>登记项</returns>
        Public Function Find(name As String) As ParameterEntry
            Return _entries.FirstOrDefault(Function(e) String.Equals(e.Name, name, StringComparison.Ordinal))
        End Function

        ''' <summary>把全部梯度累加器清零（每个训练步开始时调用）。</summary>
        Public Sub ZeroGradients()
            For Each entry As ParameterEntry In _entries
                Call TensorOps.ZeroInPlace(entry.Gradient)
            Next
        End Sub

        ''' <summary>
        ''' 计算全部参数梯度的全局 L2 范数。
        ''' </summary>
        ''' <returns>梯度范数</returns>
        Public Function GradientNorm() As Double
            Dim sum As Double = 0.0

            For Each entry As ParameterEntry In _entries
                For Each g As Double In entry.Gradient.Data
                    sum += g * g
                Next
            Next

            Return System.Math.Sqrt(sum)
        End Function

        ''' <summary>
        ''' 全局梯度范数裁剪：范数超过上限时把所有梯度等比缩小。
        ''' </summary>
        ''' <param name="maxNorm">范数上限；&lt;= 0 表示不裁剪</param>
        ''' <returns>裁剪前的梯度范数</returns>
        Public Function ClipGradients(maxNorm As Double) As Double
            Dim norm As Double = GradientNorm()

            If maxNorm <= 0.0 OrElse norm <= maxNorm OrElse norm <= 0.0 Then
                Return norm
            End If

            Dim scale As Double = maxNorm / norm

            For Each entry As ParameterEntry In _entries
                Dim data As Double() = entry.Gradient.Data

                For i As Integer = 0 To data.Length - 1
                    data(i) *= scale
                Next

                Call entry.Gradient.MarkHostModified()
            Next

            Return norm
        End Function

        ''' <summary>
        ''' 对全部参数执行一步 Adam 更新（更新完成后梯度自动清零）。
        ''' </summary>
        ''' <param name="learningRate">学习率</param>
        ''' <param name="stepIndex">训练步序号（从 1 开始，用于偏置校正）</param>
        Public Sub MakeTrainingStep(learningRate As Double, stepIndex As Integer)
            For Each entry As ParameterEntry In _entries
                If entry.State IsNot Nothing Then
                    Call entry.State.MakeTrainingStep(learningRate, stepIndex, entry.Value)
                Else
                    Call StepExternal(entry, learningRate, stepIndex)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 对外部梯度参数执行 Adam 更新（公式与 <see cref="Optimizer"/> 完全一致）。
        ''' </summary>
        ''' <param name="entry">登记项</param>
        ''' <param name="learningRate">学习率</param>
        ''' <param name="stepIndex">训练步序号</param>
        Private Sub StepExternal(entry As ParameterEntry, learningRate As Double, stepIndex As Integer)
            Dim m As Double() = _externalM(entry.Name).Data
            Dim v As Double() = _externalV(entry.Name).Data
            Dim p As Double() = entry.Value.Data
            Dim g As Double() = entry.Gradient.Data
            Dim bc1 As Double = 1.0 - System.Math.Pow(Beta1, stepIndex)
            Dim bc2 As Double = 1.0 - System.Math.Pow(Beta2, stepIndex)

            For i As Integer = 0 To p.Length - 1
                Dim gi As Double = g(i)
                Dim mi As Double = Beta1 * m(i) + (1.0 - Beta1) * gi
                Dim vi As Double = Beta2 * v(i) + (1.0 - Beta2) * gi * gi

                m(i) = mi
                v(i) = vi
                p(i) -= learningRate * (mi / bc1) / (System.Math.Sqrt(vi / bc2) + Epsilon)
            Next

            Call entry.Value.MarkHostModified()
            Call TensorOps.ZeroInPlace(entry.Gradient)
        End Sub

        ''' <summary>校验参数名唯一</summary>
        ''' <param name="name">参数名</param>
        Private Sub EnsureUnique(name As String)
            If String.IsNullOrEmpty(name) Then
                Throw New ArgumentException("参数名不能为空", NameOf(name))
            End If

            If Find(name) IsNot Nothing Then
                Throw New ArgumentException($"参数名重复：{name}", NameOf(name))
            End If
        End Sub

    End Class

End Namespace
