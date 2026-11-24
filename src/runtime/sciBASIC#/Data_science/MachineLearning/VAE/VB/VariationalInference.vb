' ============================================================================
' VariationalInference.vb - 均值场变分推断的坐标上升模板
'
' 基类固定了变分贝叶斯模型的迭代骨架：
'
'     InitializeModel  →  [ Expectation → Maximization → ComputeELBO ] × N  →  OnConverged
'
' 子类只需要实现四个钩子方法，即可获得完整的收敛监控、下界历史与迭代事件。
'
' 参考：Bishop, "Pattern Recognition and Machine Learning", 第 10 章
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace VariationalBayes

    ''' <summary>
    ''' 单次变分迭代完成之后的事件参数。
    ''' </summary>
    Public Class VariationalIterationEventArgs
        Inherits EventArgs

        ''' <summary>当前的迭代轮次（从 1 开始计数）</summary>
        Public ReadOnly Property Iteration As Integer
        ''' <summary>本次迭代计算得到的变分下界 ELBO</summary>
        Public ReadOnly Property ELBO As Double
        ''' <summary>与上一次迭代相比的下界增量</summary>
        Public ReadOnly Property Delta As Double

        Public Sub New(iteration As Integer, elbo As Double, delta As Double)
            Me.Iteration = iteration
            Me.ELBO = elbo
            Me.Delta = delta
        End Sub

        Public Overrides Function ToString() As String
            Return $"iter={Iteration}, ELBO={ELBO:F6}, Δ={Delta:E3}"
        End Function
    End Class

    ''' <summary>
    ''' 均值场变分贝叶斯模型的抽象基类。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 基类负责以下公共职责：参数校验、坐标上升循环、下界历史记录、收敛判定、
    ''' 迭代事件分发以及随机数生成器的生命周期管理。
    ''' </para>
    ''' <para>
    ''' 子类只需要实现 <see cref="InitializeModel"/>、<see cref="Expectation"/>、
    ''' <see cref="Maximization"/> 与 <see cref="ComputeELBO"/> 四个钩子方法。
    ''' </para>
    ''' </remarks>
    Public MustInherit Class VariationalInference

#Region "配置"

        ''' <summary>最大迭代轮次，默认 200</summary>
        Public Property MaxIter As Integer = 200

        ''' <summary>相对收敛阈值（下界变化的相对幅度），默认 1e-6</summary>
        Public Property Tolerance As Double = 0.000001R

        ''' <summary>随机种子，<c>Nothing</c> 表示使用时间相关的随机数</summary>
        Public Property Seed As Integer? = Nothing

        ''' <summary>是否在每次迭代的时候打印下界的进度</summary>
        Public Property Verbose As Boolean = False

        ''' <summary>拟合之前是否自动清空上一次的下界历史，默认 <c>True</c></summary>
        Public Property ResetBeforeFit As Boolean = True

#End Region

#Region "训练状态"

        Private _elbo As Double = Double.NegativeInfinity
        Private _history As New List(Of Double)
        Private _converged As Boolean = False
        Private _niter As Integer = 0
        Private _rng As Random

        ''' <summary>当前（最后一次迭代）的变分下界 ELBO</summary>
        Public ReadOnly Property ELBO As Double
            Get
                Return _elbo
            End Get
        End Property

        ''' <summary>每一轮迭代结束之后记录的变分下界</summary>
        Public ReadOnly Property ELBOHistory As List(Of Double)
            Get
                Return _history
            End Get
        End Property

        ''' <summary>是否因为下界收敛而提前结束训练</summary>
        Public ReadOnly Property Converged As Boolean
            Get
                Return _converged
            End Get
        End Property

        ''' <summary>实际执行的迭代轮次</summary>
        Public ReadOnly Property NIter As Integer
            Get
                Return _niter
            End Get
        End Property

        ''' <summary>
        ''' 当前训练所使用的随机数生成器。子类在初始化参数的时候应当使用这个实例，
        ''' 以保证 <see cref="Seed"/> 可以复现整个训练过程。
        ''' </summary>
        Protected ReadOnly Property RNG As Random
            Get
                Return _rng
            End Get
        End Property

        ''' <summary>每完成一轮迭代触发一次</summary>
        Public Event IterationCompleted As EventHandler(Of VariationalIterationEventArgs)

#End Region

#Region "模板方法"

        ''' <summary>
        ''' 使用张量数据 [N, D] 训练模型。
        ''' </summary>
        ''' <exception cref="ArgumentNullException">输入为空的时候抛出</exception>
        ''' <exception cref="ArgumentException">输入不是二维张量、或者样本数为 0 的时候抛出</exception>
        ''' <exception cref="InvalidOperationException">迭代过程中下界出现 NaN/Inf 的时候抛出</exception>
        Public Sub Fit(X As Tensor)
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            If X.Rank <> 2 Then
                Throw New ArgumentException($"输入数据必须是二维张量 [N, D]，实际秩为 {X.Rank}", NameOf(X))
            End If

            Dim samples As Integer = X.Shape(0)
            Dim features As Integer = X.Shape(1)

            If samples < 1 OrElse features < 1 Then
                Throw New ArgumentException($"输入数据的形状非法：[{samples}, {features}]", NameOf(X))
            End If

            If MaxIter < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxIter), MaxIter, "最大迭代次数必须大于 0")
            End If

            If Tolerance < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(Tolerance), Tolerance, "收敛阈值不能为负数")
            End If

            If ResetBeforeFit Then
                Call ResetState()
            End If

            _rng = If(Seed.HasValue, New Random(Seed.Value), New Random())

            Call InitializeModel(X)

            Dim previous As Double = Double.NegativeInfinity
            Dim delta As Double = Double.PositiveInfinity

            For iter As Integer = 1 To MaxIter
                Call Expectation()
                Call Maximization()

                Dim lowerBound As Double = ComputeELBO()

                If Double.IsNaN(lowerBound) Then
                    Throw New InvalidOperationException(
                        $"第 {iter} 轮迭代的变分下界为 NaN，训练已中止（模型参数可能已经退化）")
                End If

                If Double.IsPositiveInfinity(lowerBound) Then
                    Throw New InvalidOperationException(
                        $"第 {iter} 轮迭代的变分下界为 +Inf，训练已中止（先验参数可能过大）")
                End If

                _elbo = lowerBound
                _niter = iter
                _history.Add(lowerBound)

                delta = If(Double.IsNegativeInfinity(previous),
                           Double.PositiveInfinity,
                           lowerBound - previous)

                If Verbose Then
                    Console.WriteLine($"[{Me.GetType().Name}] iter={iter}, ELBO={lowerBound:F6}, Δ={delta:E3}")
                End If

                RaiseEvent IterationCompleted(Me, New VariationalIterationEventArgs(iter, lowerBound, delta))

                ' 使用相对变化量做收敛判定，避免不同数据尺度下阈值不可移植
                If iter > 1 AndAlso
                   std.Abs(delta) <= std.Max(Tolerance, Tolerance * std.Abs(lowerBound)) Then
                    _converged = True
                    Exit For
                End If

                previous = lowerBound
            Next

            Call OnConverged()
        End Sub

        ''' <summary>
        ''' 使用矩形数组训练模型的便捷重载。
        ''' </summary>
        Public Sub Fit(X As Double(,))
            If X Is Nothing Then
                Throw New ArgumentNullException(NameOf(X))
            End If

            Call Fit(New Tensor(X))
        End Sub

        ''' <summary>
        ''' 清空训练状态（下界、历史、收敛标志、迭代计数）。
        ''' </summary>
        Public Sub ResetState()
            _elbo = Double.NegativeInfinity
            _history = New List(Of Double)
            _converged = False
            _niter = 0
        End Sub

#End Region

#Region "子类钩子"

        ''' <summary>
        ''' 初始化模型参数与变分分布。该方法是每一轮迭代之前唯一被调用的初始化入口。
        ''' </summary>
        ''' <param name="X">训练数据 [N, D]</param>
        Protected MustOverride Sub InitializeModel(X As Tensor)

        ''' <summary>
        ''' E 步：固定超参数的后验，更新隐变量的变分分布 q(Z)。
        ''' </summary>
        Protected MustOverride Sub Expectation()

        ''' <summary>
        ''' M 步：固定隐变量的后验，更新超参数的变分分布 q(θ)。
        ''' </summary>
        Protected MustOverride Sub Maximization()

        ''' <summary>
        ''' 计算当前变分分布的证据下界 ELBO。
        ''' </summary>
        Protected MustOverride Function ComputeELBO() As Double

        ''' <summary>
        ''' 训练结束（收敛或者达到最大迭代次数）之后的回调。
        ''' </summary>
        Protected Overridable Sub OnConverged()
        End Sub

#End Region

    End Class

End Namespace
