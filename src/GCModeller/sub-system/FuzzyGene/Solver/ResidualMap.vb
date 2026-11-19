Imports System
Imports System.Collections.Generic
Imports std = System.Math

Namespace Solver

    ''' <summary>
    ''' 一次稳态求解的共享状态（求解器与残差映射共用同一组缓冲）。
    ''' </summary>
    ''' <remarks>
    ''' 把「求解什么」和「怎么解」分开：本类型只持有状态、约束与预算，
    ''' <see cref="NewtonSolver"/> / <see cref="AndersonAcceleration"/> 只读它、改 <see cref="Z"/>。
    ''' </remarks>
    Friend Class SolveContext

        ''' <summary>所属网络。</summary>
        Public Property Network As FuzzyGeneNetwork

        ''' <summary>当前状态（z 空间，就地更新）。</summary>
        Public Property Z As Double()

        ''' <summary>被钳制的基因下标。</summary>
        Public Property ClampIndex As List(Of Integer)

        ''' <summary>与 <see cref="ClampIndex"/> 一一对应的钳制值（z 空间）。</summary>
        Public Property ClampValue As List(Of Double)

        ''' <summary>迭代/时间步预算。</summary>
        Public Property MaxSteps As Integer

        ''' <summary>阻尼系数（Anderson / Legacy 路径使用；Newton 使用 LM 的 λ）。</summary>
        Public Property Relaxation As Double

        ''' <summary>收敛容差（自由基因残差，z 空间）。</summary>
        Public Property Tolerance As Double

        ''' <summary>并行度（<c>1</c> 表示串行）。</summary>
        Public Property Workers As Integer

        ''' <summary>轨迹收集器；<c>Nothing</c> 表示只求值、不记录。</summary>
        Public Property Trace As Data.SteadyStateTrace

        ''' <summary>诊断输出。</summary>
        Public Property Report As SteadyStateReport

    End Class

    ''' <summary>
    ''' 残差映射 <c>r(z) = F(z) - z</c>，并把「自由基因残差」与「约束违背量」严格分开。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这是本轮修复的核心。旧实现在阻尼混合的同一步里统计 <c>maxDelta</c>，
    ''' 而被钳制基因也会先被写成 <c>F_k(z)</c>（其 delta 为 <c>ω|F_k - clamp_k|</c>），
    ''' 随后才被回写覆盖——于是 <c>maxDelta</c> 恒被这个与收敛无关的量顶在高位，
    ''' <c>Converged</c> 几乎永远为 False。
    ''' </para>
    ''' <para>
    ''' 约束型不动点的正确提法只对自由基因要求 <c>F_g(z) = z_g</c>；
    ''' 被钳制基因的 <c>|F_k(z) - clamp_k|</c> 是「约束与模型冲突的程度」，属于诊断而非收敛判据。
    ''' </para>
    ''' </remarks>
    Friend Class ResidualMap

        Private ReadOnly _net As FuzzyGeneNetwork
        Private ReadOnly _context As SolveContext
        Private ReadOnly _nextZ As Double()
        Private ReadOnly _residual As Double()
        Private ReadOnly _dirty As Boolean()
        Private ReadOnly _trialZ As Double()
        Private ReadOnly _trialF As Double()
        Private ReadOnly _clamped As Boolean()
        Private ReadOnly _clampValue As Double()
        Private ReadOnly _supportLow As Double()
        Private ReadOnly _supportHigh As Double()
        Private ReadOnly _projectSupport As Boolean
        Private ReadOnly _n As Integer

        ''' <summary>残差映射（复用全部缓冲，不产生每步分配）。</summary>
        Public Sub New(network As FuzzyGeneNetwork, context As SolveContext)
            _net = network
            _context = context
            _n = network.NGene

            _nextZ = New Double(_n - 1) {}
            _residual = New Double(_n - 1) {}
            _dirty = New Boolean(_n - 1) {}
            _trialZ = New Double(_n - 1) {}
            _trialF = New Double(_n - 1) {}
            _clamped = New Boolean(_n - 1) {}
            _clampValue = New Double(_n - 1) {}
            _supportLow = New Double(_n - 1) {}
            _supportHigh = New Double(_n - 1) {}

            For g As Integer = 0 To _n - 1
                _dirty(g) = True
                _supportLow(g) = network.SupportMin(g) - 1.0
                _supportHigh(g) = network.SupportMax(g) + 1.0
            Next

            For k As Integer = 0 To context.ClampIndex.Count - 1
                Dim idx As Integer = context.ClampIndex(k)

                _clamped(idx) = True
                _clampValue(idx) = context.ClampValue(k)
            Next

            _projectSupport = network.Config.Solver.ClampOutputToSupport
        End Sub

        ''' <summary>当前状态（引用，不是副本）。</summary>
        Public ReadOnly Property Z As Double()
            Get
                Return _context.Z
            End Get
        End Property

        ''' <summary>上一轮前向的 <c>F(z)</c>。</summary>
        Public ReadOnly Property NextZ As Double()
            Get
                Return _nextZ
            End Get
        End Property

        ''' <summary>残差（被钳制位置恒为 0）。</summary>
        Public ReadOnly Property Residual As Double()
            Get
                Return _residual
            End Get
        End Property

        ''' <summary>被钳制基因的约束违背量（仅诊断）。</summary>
        Public Property ClampResidual As Double

        ''' <summary>本轮输入被夹断的基因数。</summary>
        Public Property OutOfSupport As Integer

        ''' <summary>本轮输出贴界的基因数。</summary>
        Public Property Pinned As Integer

        ''' <summary>某个位置是否被钳制。</summary>
        Public Function IsClamped(index As Integer) As Boolean
            Return _clamped(index)
        End Function

        ''' <summary>
        ''' 计算 <c>F(z)</c> 与残差。
        ''' </summary>
        ''' <param name="forceAll">
        ''' <c>True</c> 时全量重算；<c>False</c> 时沿用脏位优化（只有调控因子变化的基因才重算）。
        ''' 求根路径必须用 <c>True</c>：脏位优化会让「调控因子都不再变化」的基因被冻结在
        ''' 非不动点上（<c>nextZ</c> 被复制成 <c>z</c>），残差会被误报成 0。
        ''' </param>
        Public Sub Evaluate(forceAll As Boolean)
            Dim workers As Integer = _context.Workers

            Call _net.ResetForwardDiagnostics()
            Call _net.StepZInto(_context.Z, _dirty, forceAll, _nextZ, workers)

            ' 注意：局部变量不能叫 clampResidual —— VB 大小写不敏感，
            ' 那会让 `ClampResidual = clampResidual` 变成局部变量自赋值（属性永远是 0）
            Dim violationMax As Double = 0.0
            Dim diag = _net.ForwardDiagnostics()

            For g As Integer = 0 To _n - 1
                If _clamped(g) Then
                    _residual(g) = 0.0

                    Dim violation As Double = std.Abs(_nextZ(g) - _clampValue(g))

                    If violation > violationMax Then
                        violationMax = violation
                    End If
                Else
                    _residual(g) = _nextZ(g) - _context.Z(g)
                End If
            Next

            Me.ClampResidual = violationMax
            OutOfSupport = diag.outOfSupport
            Pinned = diag.pinned
        End Sub

        ''' <summary>
        ''' 把当前诊断写进报告（残差、贴界、主导基因的唯一口径）。
        ''' </summary>
        ''' <remarks>
        ''' 阻尼路径与牛顿路径都调用这里，避免两条路径对「什么算残差」出现分歧。
        ''' </remarks>
        Public Sub PublishTo(report As SteadyStateReport)
            report.FreeResidual = FreeMax()
            report.ClampResidual = ClampResidual
            report.OutOfSupportCount = OutOfSupport
            report.PinnedToBoundCount = Pinned
            report.DominantGene = DominantGene()
        End Sub

        ''' <summary>自由基因的最大残差（<c>L∞</c>，收敛判据）。</summary>
        Public Function FreeMax() As Double
            Dim result As Double = 0.0

            For g As Integer = 0 To _n - 1
                If _clamped(g) Then
                    Continue For
                End If

                Dim v As Double = std.Abs(_residual(g))

                If v > result Then
                    result = v
                End If
            Next

            Return result
        End Function

        ''' <summary>自由基因的残差 L2 范数（线搜索/Anderson 用）。</summary>
        Public Function FreeNorm() As Double
            Dim sum As Double = 0.0

            For g As Integer = 0 To _n - 1
                If Not _clamped(g) Then
                    sum += _residual(g) * _residual(g)
                End If
            Next

            Return std.Sqrt(sum)
        End Function

        ''' <summary>承担最大残差的基因名；没有自由基因时返回 <c>Nothing</c>。</summary>
        Public Function DominantGene() As String
            Dim best As Integer = -1
            Dim peak As Double = 0.0

            For g As Integer = 0 To _n - 1
                If _clamped(g) Then
                    Continue For
                End If

                Dim v As Double = std.Abs(_residual(g))

                If v > peak Then
                    peak = v
                    best = g
                End If
            Next

            If best < 0 Then
                Return Nothing
            End If

            Return _net.GeneNames(best)
        End Function

        ''' <summary>
        ''' 按 <c>z ← z + direction</c> 走一步（不做线搜索，旧阻尼路径用）。
        ''' </summary>
        Public Sub ApplyStep(direction As Double())
            For g As Integer = 0 To _n - 1
                If _clamped(g) Then
                    _context.Z(g) = _clampValue(g)

                    Continue For
                End If

                Dim nextValue As Double = _context.Z(g) + direction(g)
                Dim delta As Double = std.Abs(nextValue - _context.Z(g))

                _context.Z(g) = project(g, nextValue)
                _dirty(g) = delta > 1.0E-12
            Next

            ' 被钳制位置永远是「干净」的：它的值由实验施加，不随迭代改变
            For k As Integer = 0 To _context.ClampIndex.Count - 1
                _dirty(_context.ClampIndex(k)) = False
            Next
        End Sub

        ''' <summary>
        ''' 回溯线搜索：沿 <paramref name="direction"/> 折半试探，直到残差下降才接受。
        ''' </summary>
        ''' <param name="direction">搜索方向（长度 = 基因数，被钳制位置忽略）</param>
        ''' <param name="currentMax">当前自由残差 <c>L∞</c></param>
        ''' <param name="halvings">最大折半次数</param>
        ''' <returns>被接受的步长 α；<c>0</c> 表示没有任何步长能降低残差</returns>
        Public Function TryStep(direction As Double(), currentMax As Double, halvings As Integer) As Double
            Dim alpha As Double = 1.0

            For s As Integer = 0 To halvings - 1
                For g As Integer = 0 To _n - 1
                    If _clamped(g) Then
                        _trialZ(g) = _clampValue(g)
                    Else
                        _trialZ(g) = project(g, _context.Z(g) + alpha * direction(g))
                    End If
                Next

                Call _net.ResetForwardDiagnostics()
                Call _net.StepZInto(_trialZ, _dirty, True, _trialF, _context.Workers)

                Dim trialMax As Double = 0.0

                For g As Integer = 0 To _n - 1
                    If _clamped(g) Then
                        Continue For
                    End If

                    Dim v As Double = std.Abs(_trialF(g) - _trialZ(g))

                    If v > trialMax Then
                        trialMax = v
                    End If
                Next

                ' Armijo 型接受条件：残差必须有可测的下降，避免「原地踏步也算成功」
                If trialMax <= currentMax * (1.0 - 0.0001 * alpha) Then
                    Call AdoptTrial()

                    Return alpha
                End If

                alpha *= 0.5
            Next

            Return 0.0
        End Function

        ''' <summary>把试探步的结果写回当前状态，并刷新残差与诊断。</summary>
        Private Sub AdoptTrial()
            ' 同样不能叫 clampResidual：VB 大小写不敏感，会与属性自赋值而静默失效
            Dim violationMax As Double = 0.0
            Dim diag = _net.ForwardDiagnostics()

            For g As Integer = 0 To _n - 1
                Dim delta As Double = std.Abs(_trialZ(g) - _context.Z(g))

                _context.Z(g) = _trialZ(g)
                _dirty(g) = delta > 1.0E-12

                If _clamped(g) Then
                    _residual(g) = 0.0
                    _dirty(g) = False

                    Dim violation As Double = std.Abs(_trialF(g) - _clampValue(g))

                    If violation > violationMax Then
                        violationMax = violation
                    End If
                Else
                    _residual(g) = _trialF(g) - _trialZ(g)
                End If
            Next

            Call Array.Copy(_trialF, _nextZ, _n)

            Me.ClampResidual = violationMax
            OutOfSupport = diag.outOfSupport
            Pinned = diag.pinned
        End Sub

        ''' <summary>把单个取值投影回输出支持域（只在配置打开时生效）。</summary>
        Private Function project(gene As Integer, value As Double) As Double
            If Not _projectSupport Then
                Return value
            End If

            Dim lower As Double = _supportLow(gene)
            Dim upper As Double = _supportHigh(gene)

            If upper <= lower Then
                Return value
            End If
            If value < lower Then
                Return lower
            ElseIf value > upper Then
                Return upper
            Else
                Return value
            End If
        End Function

    End Class

End Namespace
