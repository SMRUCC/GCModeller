Imports System
Imports std = System.Math

Namespace Solver

    ''' <summary>
    ''' 带解析 Jacobian 的 Levenberg–Marquardt / 牛顿求根器：解 <c>r(z) = F(z) - z = 0</c>。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么是求根而不是迭代</b>：静态路线学到的 <c>F</c> 是回归面，把它当时间步反复施加
    ''' 只在 <c>ρ(J) &lt; 1</c> 时收敛；而这一条恰恰逐靶基因独立拟合给不出保证。
    ''' 求根把问题写成「找 <c>F</c> 的不动点」，收敛判据变成 <c>‖r‖ &lt; tol</c>，
    ''' 与步数预算解耦，并且对 <c>ρ(J) ≈ 1</c> 的中立方向依然有效（牛顿法在简单零点附近是二次收敛的）。
    ''' </para>
    ''' <para>
    ''' 线性子问题取 <c>((1+λ)I − J)·δ = r</c>：<c>λ</c> 大时退化为阻尼迭代步，
    ''' <c>λ→0</c> 时是纯牛顿步。每轮先试 <c>λ</c>，线搜索不接受就放大 <c>λ</c> 重试，
    ''' 这与 Levenberg–Marquardt 的信任域语义一致。
    ''' </para>
    ''' <para>
    ''' 规模策略：<c>N ≤ DenseThreshold</c> 走稠密高斯消元（<c>N²</c> 内存，1024 基因约 8 MB）；
    ''' 更大规模走矩阵无关的 BiCGStab，每次迭代只需一次 <c>J·x</c>（<c>O(N·K)</c>），
    ''' 不物化 <c>N²</c> 矩阵。
    ''' </para>
    ''' </remarks>
    Friend Class NewtonSolver

        Private ReadOnly _net As FuzzyGeneNetwork
        Private ReadOnly _context As SolveContext
        Private ReadOnly _report As SteadyStateReport
        Private ReadOnly _options As Config.SolverOptions
        Private ReadOnly _residual As ResidualMap
        Private ReadOnly _jacobian As JacobianOperator
        Private ReadOnly _direction As Double()
        Private ReadOnly _n As Integer
        Private ReadOnly _useDense As Boolean

        ' ---- 稠密后端 ----
        Private ReadOnly _dense As Double(,)
        Private ReadOnly _pivot As Integer()

        ' ---- Krylov 后端 ----
        Private ReadOnly _jx As Double()
        Private ReadOnly _r As Double()
        Private ReadOnly _rHat As Double()
        Private ReadOnly _p As Double()
        Private ReadOnly _v As Double()
        Private ReadOnly _s As Double()
        Private ReadOnly _t As Double()

        ''' <summary>绑定网络与求解上下文。</summary>
        Public Sub New(network As FuzzyGeneNetwork, context As SolveContext)
            _net = network
            _context = context
            _report = context.Report
            _options = network.Config.Solver
            _n = network.NGene
            _residual = New ResidualMap(network, context)
            _jacobian = New JacobianOperator(network)
            _direction = New Double(_n - 1) {}
            _useDense = _n <= _options.DenseThreshold

            If _useDense Then
                _dense = New Double(_n - 1, _n - 1) {}
                _pivot = New Integer(_n - 1) {}
            Else
                _jx = New Double(_n - 1) {}
                _r = New Double(_n - 1) {}
                _rHat = New Double(_n - 1) {}
                _p = New Double(_n - 1) {}
                _v = New Double(_n - 1) {}
                _s = New Double(_n - 1) {}
                _t = New Double(_n - 1) {}
            End If
        End Sub

        ''' <summary>
        ''' 执行 LM 求根。
        ''' </summary>
        ''' <returns>
        ''' <c>True</c> 表示已经得到终局判定（收敛/发散/饱和/预算耗尽）；
        ''' <c>False</c> 表示线性子问题始终无法给出下降方向，调用方应回落到阻尼迭代。
        ''' </returns>
        Public Function Solve() As Boolean
            Dim tolerance As Double = _context.Tolerance
            Dim lambda As Double = _options.Lambda0
            Dim initialMax As Double = 0.0

            Call _residual.Evaluate(True)
            initialMax = _residual.FreeMax()

            For iteration As Integer = 1 To _options.MaxNewtonIterations
                Dim currentMax As Double = _residual.FreeMax()

                Call _residual.PublishTo(_report)

                If currentMax <= tolerance Then
                    _report.ExitReason = ExitReason.Converged

                    Return True
                End If

                If initialMax > 0.0 AndAlso currentMax > initialMax * _options.DivergenceFactor Then
                    _report.ExitReason = ExitReason.Diverged
                    _report.AddMessage($"the residual grew by more than {_options.DivergenceFactor:G3}x; the fitted network has no reachable fixed point from this initial state")

                    Return True
                End If

                If _n > 0 AndAlso _residual.Pinned >= _n \ 2 AndAlso currentMax > tolerance * 100.0 Then
                    _report.ExitReason = ExitReason.SupportSaturated
                    _report.AddMessage("more than half of the genes are pinned to the support bounds; the response is an artefact of clamping")

                    Return True
                End If

                Call _jacobian.Build(_context.Z, _context.Workers)

                _report.JacobianEvaluations += 1

                Dim accepted As Boolean = False

                For retry As Integer = 0 To _options.MaxLambdaRetries
                    If Not SolveLinear(lambda, _residual.Residual, _direction) Then
                        lambda *= _options.LambdaUp

                        Continue For
                    End If

                    Dim alpha As Double = _residual.TryStep(_direction, currentMax, _options.LineSearchSteps)

                    If alpha > 0.0 Then
                        lambda = std.Max(lambda / _options.LambdaDown, 1.0E-12)
                        accepted = True

                        Exit For
                    End If

                    ' 线搜索失败：加大信赖域阻尼后重解（LM 的标准做法）
                    lambda *= _options.LambdaUp
                Next

                If Not accepted Then
                    _report.AddMessage("the LM step could not reduce the residual; the linear sub-problem is ill-conditioned")

                    Return False
                End If

                _report.Iterations = iteration
                _report.PushResidual(currentMax)

                If _context.Trace IsNot Nothing Then
                    Call _context.Trace.Push(_net.ToRaw(_context.Z), currentMax)
                End If

                Call _residual.PublishTo(_report)
            Next

            _report.ExitReason = ExitReason.MaxIterations
            _report.AddMessage($"the Newton budget ({_options.MaxNewtonIterations}) was exhausted without reaching the tolerance {tolerance:G3}")
            Call _residual.PublishTo(_report)

            Return True
        End Function

        ''' <summary>
        ''' 解 <c>((1+λ)I − J)·δ = r</c>，结果写入 <see cref="_direction"/>。
        ''' </summary>
        ''' <remarks>
        ''' 被钳制基因在方程里是单位行、右端为 0，因此 <c>δ</c> 在这些位置恒为 0——
        ''' 这样「约束」与「自由变量」在同一个线性系统里自然分开，不需要额外消元。
        ''' </remarks>
        Private Function SolveLinear(lambda As Double, rhs As Double(), x As Double()) As Boolean
            If _useDense Then
                Return SolveDense(lambda, rhs, x)
            End If

            Return SolveKrylov(lambda, rhs, x)
        End Function

        Private Function SolveDense(lambda As Double, rhs As Double(), x As Double()) As Boolean
            Dim n As Integer = _n
            Dim scale As Double = 1.0 + lambda

            ' 逐行填充：先清成 0，再写对角与 -J
            For g As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    _dense(g, j) = 0.0
                Next
            Next

            For g As Integer = 0 To n - 1
                _dense(g, g) = scale

                Dim row As Double() = _jacobian.Row(g)

                If row IsNot Nothing Then
                    Dim inputs As Integer() = _net.RegulatorIndexTable(g)

                    For j As Integer = 0 To row.Length - 1
                        _dense(g, inputs(j)) -= row(j)
                    Next
                End If
            Next

            ' 被钳制位置：单位行 + 右端 0 ⇒ δ = 0
            For k As Integer = 0 To _context.ClampIndex.Count - 1
                Dim idx As Integer = _context.ClampIndex(k)

                For j As Integer = 0 To n - 1
                    _dense(idx, j) = 0.0
                Next

                _dense(idx, idx) = 1.0
            Next

            Call Array.Copy(rhs, x, n)

            For g As Integer = 0 To _context.ClampIndex.Count - 1
                x(_context.ClampIndex(g)) = 0.0
            Next

            ' 部分主元高斯消元（就地）
            For col As Integer = 0 To n - 1
                Dim pivot As Integer = col
                Dim peak As Double = std.Abs(_dense(col, col))

                For row As Integer = col + 1 To n - 1
                    Dim value As Double = std.Abs(_dense(row, col))

                    If value > peak Then
                        peak = value
                        pivot = row
                    End If
                Next

                If peak < 1.0E-14 Then
                    Return False
                End If

                If pivot <> col Then
                    For j As Integer = col To n - 1
                        Dim temp As Double = _dense(col, j)

                        _dense(col, j) = _dense(pivot, j)
                        _dense(pivot, j) = temp
                    Next

                    Dim t As Double = x(col)

                    x(col) = x(pivot)
                    x(pivot) = t
                End If

                Dim diagonal As Double = _dense(col, col)

                For row As Integer = col + 1 To n - 1
                    Dim factor As Double = _dense(row, col) / diagonal

                    If factor = 0.0 Then
                        Continue For
                    End If

                    For j As Integer = col To n - 1
                        _dense(row, j) -= factor * _dense(col, j)
                    Next

                    x(row) -= factor * x(col)
                Next
            Next

            For row As Integer = n - 1 To 0 Step -1
                Dim sum As Double = x(row)

                For j As Integer = row + 1 To n - 1
                    sum -= _dense(row, j) * x(j)
                Next

                Dim diagonal As Double = _dense(row, row)

                If std.Abs(diagonal) < 1.0E-14 Then
                    Return False
                End If

                x(row) = sum / diagonal
            Next

            Return True
        End Function

        ''' <summary>矩阵无关的 BiCGStab：<c>A = (1+λ)I − J</c>，不物化 <c>N²</c>。</summary>
        Private Function SolveKrylov(lambda As Double, rhs As Double(), x As Double()) As Boolean
            Dim n As Integer = _n
            Dim scale As Double = 1.0 + lambda
            Dim rhsNorm As Double = 0.0

            For g As Integer = 0 To n - 1
                x(g) = 0.0
                _r(g) = rhs(g)
                _rHat(g) = rhs(g)
                _p(g) = 0.0
                _v(g) = 0.0
                rhsNorm += rhs(g) * rhs(g)
            Next

            rhsNorm = std.Sqrt(rhsNorm)

            If rhsNorm <= 0.0 Then
                Return True
            End If

            Dim target As Double = _options.KrylovTolerance * rhsNorm
            Dim rhoOld As Double = 1.0
            Dim alpha As Double = 1.0
            Dim omega As Double = 1.0
            Dim residualNorm As Double = rhsNorm

            For iteration As Integer = 1 To _options.KrylovMaxIterations
                Dim rho As Double = Dot(_rHat, _r)

                If std.Abs(rho) < 1.0E-30 Then
                    Exit For
                End If

                If iteration = 1 Then
                    Call Array.Copy(_r, 0, _p, 0, n)
                Else
                    Dim beta As Double = (rho / rhoOld) * (alpha / omega)

                    For g As Integer = 0 To n - 1
                        _p(g) = _r(g) + beta * (_p(g) - omega * _v(g))
                    Next
                End If

                Call Apply(lambda, _p, _v)

                Dim denominator As Double = Dot(_rHat, _v)

                If std.Abs(denominator) < 1.0E-30 Then
                    Exit For
                End If

                alpha = rho / denominator

                For g As Integer = 0 To n - 1
                    _s(g) = _r(g) - alpha * _v(g)
                Next

                Dim sNorm As Double = Norm(_s)

                If sNorm <= target Then
                    For g As Integer = 0 To n - 1
                        x(g) += alpha * _p(g)
                    Next

                    Call ZeroClamped(x)

                    Return True
                End If

                Call Apply(lambda, _s, _t)

                Dim tt As Double = Dot(_t, _t)

                If tt <= 0.0 Then
                    Exit For
                End If

                omega = Dot(_t, _s) / tt

                For g As Integer = 0 To n - 1
                    x(g) += alpha * _p(g) + omega * _s(g)
                    _r(g) = _s(g) - omega * _t(g)
                Next

                residualNorm = Norm(_r)

                If residualNorm <= target Then
                    Call ZeroClamped(x)

                    Return True
                End If

                rhoOld = rho
            Next

            Return False
        End Function

        ''' <summary><c>y = ((1+λ)I − J)·x</c>，被钳制位置取单位行。</summary>
        Private Sub Apply(lambda As Double, x As Double(), y As Double())
            Dim scale As Double = 1.0 + lambda

            Call _jacobian.Matvec(x, _jx)

            For g As Integer = 0 To _n - 1
                y(g) = scale * x(g) - _jx(g)
            Next

            For k As Integer = 0 To _context.ClampIndex.Count - 1
                Dim idx As Integer = _context.ClampIndex(k)

                y(idx) = x(idx)
            Next
        End Sub

        Private Sub ZeroClamped(x As Double())
            For k As Integer = 0 To _context.ClampIndex.Count - 1
                x(_context.ClampIndex(k)) = 0.0
            Next
        End Sub

        Private Shared Function Dot(a As Double(), b As Double()) As Double
            Dim sum As Double = 0.0

            For i As Integer = 0 To a.Length - 1
                sum += a(i) * b(i)
            Next

            Return sum
        End Function

        Private Shared Function Norm(a As Double()) As Double
            Dim sum As Double = 0.0

            For i As Integer = 0 To a.Length - 1
                sum += a(i) * a(i)
            Next

            Return std.Sqrt(sum)
        End Function

    End Class

End Namespace
