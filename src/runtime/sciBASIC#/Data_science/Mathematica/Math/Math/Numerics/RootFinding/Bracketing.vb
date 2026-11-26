Imports sysmath = System.Math

Namespace Numerics.RootFinding

    ''' <summary>
    ''' 一维求根器：有界二分（自动区间扩张）、割线法、不动点迭代。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 三个入口都支持求解 <c>f(x) = target</c>（target 缺省为 0），
    ''' 内部统一用 <c>g(x) = f(x) - target</c> 判定符号与残差。
    ''' </para>
    ''' <para>
    ''' <b>不抛异常</b>：函数求值出现 NaN/Inf、区间无法括住零点、或迭代次数耗尽时，
    ''' 返回 <see cref="RootResult"/> 并置 <see cref="RootResult.Converged"/> 为 <c>False</c>、
    ''' 必要时置 <see cref="RootResult.Capped"/> 为 <c>True</c>。只有在参数本身非法
    ''' （<c>f</c> 为 Nothing）时才抛 <see cref="ArgumentNullException"/>。
    ''' </para>
    ''' <para>
    ''' 复杂度：二分与不动点每次迭代一次函数求值；割线法每次迭代一次函数求值且超线性收敛
    ''' （对光滑单根，误差阶约 1.618）。
    ''' </para>
    ''' </remarks>
    Public Module RootSolver

        ''' <summary>
        ''' 有界二分求根。若初始区间未括住零点，则按 <see cref="RootOptions.ExpansionFactor"/>
        ''' 向上扩张上界（适用于 <c>x &gt;= 0</c> 的物理量，如方差、边际系数）。
        ''' </summary>
        ''' <param name="f">目标函数</param>
        ''' <param name="lo">区间下界</param>
        ''' <param name="hi">区间上界</param>
        ''' <param name="target">目标值，求 <c>f(x) = target</c></param>
        ''' <param name="opt">迭代参数，缺省使用 <see cref="RootOptions.Default"/></param>
        ''' <returns></returns>
        Public Function Bisect(f As Func(Of Double, Double),
                               lo As Double,
                               hi As Double,
                               Optional target As Double = 0.0,
                               Optional opt As RootOptions = Nothing) As RootResult

            If f Is Nothing Then Throw New ArgumentNullException(NameOf(f))
            If opt Is Nothing Then opt = RootOptions.Default()

            Dim history As List(Of Double) = If(opt.RecordHistory, New List(Of Double), Nothing)
            Dim flo As Double = f(lo) - target
            Dim fhi As Double = f(hi) - target

            If sysmath.Abs(flo) <= opt.AbsoluteTolerance Then
                Return Result(lo, flo, True, False, 0, history, "left endpoint is already a root")
            End If
            If sysmath.Abs(fhi) <= opt.AbsoluteTolerance Then
                Return Result(hi, fhi, True, False, 0, history, "right endpoint is already a root")
            End If
            If Not IsFinite(flo) OrElse Not IsFinite(fhi) Then
                Return Result(If(IsFinite(flo), lo, hi), If(IsFinite(flo), flo, fhi), False, True, 0, history,
                              "non-finite endpoint value")
            End If

            Dim capped As Boolean = False
            Dim expansions As Integer = 0

            ' 未括住零点时向上扩张上界：CellularAI 的 margin / amplitude 搜索都属于"目标越大越难达到"的
            ' 单调场景，向上扩张比对称扩张更贴合语义，也避免进入负半轴产生无意义求值
            Do While flo * fhi > 0.0
                If expansions >= opt.MaxExpansions Then
                    capped = True
                    Exit Do
                End If

                hi = If(hi = 0.0, 1.0, hi * opt.ExpansionFactor)
                fhi = f(hi) - target
                expansions += 1

                If Not IsFinite(fhi) Then
                    capped = True
                    Exit Do
                End If
            Loop

            Dim iterations As Integer = 0
            Dim bestX As Double = If(sysmath.Abs(flo) <= sysmath.Abs(fhi), lo, hi)
            Dim bestResidual As Double = sysmath.Min(sysmath.Abs(flo), sysmath.Abs(fhi))

            If Not capped Then
                For i As Integer = 1 To opt.MaxIterations
                    iterations = i

                    Dim mid As Double = 0.5 * (lo + hi)
                    Dim fm As Double = f(mid) - target

                    If history IsNot Nothing Then Call history.Add(mid)

                    Dim residual As Double = sysmath.Abs(fm)

                    If residual < bestResidual Then
                        bestResidual = residual
                        bestX = mid
                    End If

                    If residual <= opt.AbsoluteTolerance Then
                        Return Result(mid, fm, True, False, i, history, "residual tolerance reached")
                    End If
                    If fm = 0.0 Then
                        Return Result(mid, fm, True, False, i, history, "exact zero")
                    End If
                    If sysmath.Abs(hi - lo) <= opt.AbsoluteTolerance + opt.RelativeTolerance * sysmath.Abs(mid) Then
                        Return Result(mid, fm, True, False, i, history, "bracket width tolerance reached")
                    End If

                    If (flo < 0.0) = (fm < 0.0) Then
                        lo = mid
                        flo = fm
                    Else
                        hi = mid
                        fhi = fm
                    End If
                Next

                Return Result(bestX, f(bestX) - target, False, False, iterations, history, "iteration budget exhausted")
            End If

            Return Result(bestX, f(bestX) - target, False, True, 0, history,
                          $"bracket could not be expanded within {opt.MaxExpansions} expansion(s)")
        End Function

        ''' <summary>
        ''' 割线法求根（超线性收敛，但可能发散；发散时返回 <c>Converged = False</c>）
        ''' </summary>
        ''' <param name="f">目标函数</param>
        ''' <param name="x0">第一个初值</param>
        ''' <param name="x1">第二个初值（必须与 <paramref name="x0"/> 不同）</param>
        ''' <param name="target">目标值，求 <c>f(x) = target</c></param>
        ''' <param name="opt">迭代参数</param>
        ''' <returns></returns>
        Public Function Secant(f As Func(Of Double, Double),
                               x0 As Double,
                               x1 As Double,
                               Optional target As Double = 0.0,
                               Optional opt As RootOptions = Nothing) As RootResult

            If f Is Nothing Then Throw New ArgumentNullException(NameOf(f))
            If opt Is Nothing Then opt = RootOptions.Default()

            Dim history As List(Of Double) = If(opt.RecordHistory, New List(Of Double), Nothing)
            Dim r0 As Double = f(x0) - target
            Dim r1 As Double = f(x1) - target

            If sysmath.Abs(r0) <= opt.AbsoluteTolerance Then
                Return Result(x0, r0, True, False, 0, history, "initial point is a root")
            End If
            If sysmath.Abs(r1) <= opt.AbsoluteTolerance Then
                Return Result(x1, r1, True, False, 0, history, "second initial point is a root")
            End If

            If history IsNot Nothing Then
                Call history.Add(x0)
                Call history.Add(x1)
            End If

            Dim bestX As Double = If(sysmath.Abs(r0) <= sysmath.Abs(r1), x0, x1)
            Dim bestResidual As Double = sysmath.Min(sysmath.Abs(r0), sysmath.Abs(r1))
            Dim iterations As Integer = 0

            For i As Integer = 1 To opt.MaxIterations
                iterations = i

                Dim denominator As Double = r1 - r0

                If sysmath.Abs(denominator) <= Double.Epsilon Then
                    Return Result(bestX, f(bestX) - target, False, False, i, history, "secant denominator vanished")
                End If

                Dim x2 As Double = x1 - r1 * (x1 - x0) / denominator

                If Not IsFinite(x2) Then
                    Return Result(bestX, f(bestX) - target, False, False, i, history, "secant step became non-finite")
                End If

                Dim r2 As Double = f(x2) - target

                If history IsNot Nothing Then Call history.Add(x2)

                Dim residual As Double = sysmath.Abs(r2)

                If residual < bestResidual Then
                    bestResidual = residual
                    bestX = x2
                End If

                If residual <= opt.AbsoluteTolerance Then
                    Return Result(x2, r2, True, False, i, history, "residual tolerance reached")
                End If
                If sysmath.Abs(x2 - x1) <= opt.AbsoluteTolerance + opt.RelativeTolerance * sysmath.Abs(x2) Then
                    Return Result(x2, r2, True, False, i, history, "step tolerance reached")
                End If

                x0 = x1
                r0 = r1
                x1 = x2
                r1 = r2
            Next

            Return Result(bestX, f(bestX) - target, False, False, iterations, history, "iteration budget exhausted")
        End Function

        ''' <summary>
        ''' 不动点迭代 <c>x_(k+1) = g(x_k)</c>
        ''' </summary>
        ''' <param name="g">迭代映射</param>
        ''' <param name="x0">初值</param>
        ''' <param name="opt">迭代参数</param>
        ''' <returns>收敛时 <see cref="RootResult.Value"/> 为不动点，残差记为 <c>x_new - x_old</c></returns>
        ''' <remarks>
        ''' 发散（值变为 NaN/Inf）或振荡不收敛时返回 <c>Converged = False</c>，
        ''' 不抛异常也不静默返回最后一个迭代值——调用方可以据此决定是否改用阻尼或换算法。
        ''' </remarks>
        Public Function FixedPoint(g As Func(Of Double, Double),
                                   x0 As Double,
                                   Optional opt As RootOptions = Nothing) As RootResult

            If g Is Nothing Then Throw New ArgumentNullException(NameOf(g))
            If opt Is Nothing Then opt = RootOptions.Default()

            Dim history As List(Of Double) = If(opt.RecordHistory, New List(Of Double), Nothing)
            Dim current As Double = x0

            If history IsNot Nothing Then Call history.Add(x0)

            For i As Integer = 1 To opt.MaxIterations
                Dim nextValue As Double = g(current)

                If Not IsFinite(nextValue) Then
                    Return Result(current, Double.NaN, False, False, i, history, "iteration produced a non-finite value")
                End If

                If history IsNot Nothing Then Call history.Add(nextValue)

                Dim delta As Double = nextValue - current
                Dim converged As Boolean =
                    sysmath.Abs(delta) <= opt.AbsoluteTolerance + opt.RelativeTolerance * sysmath.Abs(nextValue)

                If converged Then
                    Return Result(nextValue, delta, True, False, i, history, "fixed point reached")
                End If

                current = nextValue
            Next

            Return Result(current, Double.NaN, False, False, opt.MaxIterations, history, "iteration budget exhausted")
        End Function

        ''' <summary>
        ''' 自动扩张区间，使 <c>f(x) = target</c> 的零点被 <c>[x0, hi]</c> 括住。
        ''' </summary>
        ''' <param name="f">目标函数</param>
        ''' <param name="x0">已知端点</param>
        ''' <param name="target">目标值</param>
        ''' <param name="opt">迭代参数</param>
        ''' <returns>
        ''' <c>lo</c> = <paramref name="x0"/>，<c>hi</c> = 扩张后的另一端点，<c>capped</c> = 是否达到扩张上限；
        ''' 当 <paramref name="x0"/> 本身是零点时返回 <c>(x0, x0, False)</c>。
        ''' </returns>
        Public Function ExpandBracket(f As Func(Of Double, Double),
                                     x0 As Double,
                                     Optional target As Double = 0.0,
                                     Optional opt As RootOptions = Nothing) As (lo As Double, hi As Double, capped As Boolean)

            If f Is Nothing Then Throw New ArgumentNullException(NameOf(f))
            If opt Is Nothing Then opt = RootOptions.Default()

            Dim r0 As Double = f(x0) - target

            If r0 = 0.0 OrElse sysmath.Abs(r0) <= opt.AbsoluteTolerance Then
                Return (x0, x0, False)
            End If

            Dim negative As Boolean = r0 < 0.0
            Dim ext As Double = x0

            For i As Integer = 1 To opt.MaxExpansions
                ext = If(ext = 0.0, If(negative, 1.0, -1.0), ext * opt.ExpansionFactor)

                Dim r As Double = f(ext) - target

                If Not IsFinite(r) Then Return (x0, ext, True)

                If (r < 0.0) <> negative Then Return (x0, ext, False)
            Next

            Return (x0, ext, True)
        End Function

        Private Function Result(value As Double,
                                residual As Double,
                                converged As Boolean,
                                capped As Boolean,
                                iterations As Integer,
                                history As List(Of Double),
                                message As String) As RootResult

            Return New RootResult With {
                .Value = value,
                .Residual = residual,
                .Converged = converged,
                .Capped = capped,
                .Iterations = iterations,
                .History = If(history Is Nothing, Nothing, history.ToArray),
                .Message = message
            }
        End Function

        Private Function IsFinite(x As Double) As Boolean
            Return Not (Double.IsNaN(x) OrElse Double.IsInfinity(x))
        End Function

    End Module
End Namespace
