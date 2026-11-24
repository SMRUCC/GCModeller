Imports System
Imports System.Collections.Generic
Imports std = System.Math

Namespace Solver

    ''' <summary>
    ''' Anderson 加速（type-I mixing）：用最近若干步的残差差分做最小二乘外推。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 阻尼迭代 <c>z ← z + ω·r</c> 在 <c>ρ(J) ≈ 1</c> 的中立方向上是「几乎不动」的：
    ''' 收敛因子 <c>1 - ω(1-ρ)</c> 可以非常接近 1，于是几百步才走完的距离，
    ''' Anderson 用 5 步历史就能外推出同样的位移。这正是「静态回归面」这类
    ''' 近奇异映射最需要的东西。
    ''' </para>
    ''' <para>
    ''' 公式：<c>d = r - (X + R)·γ</c>，其中 <c>X</c> 是状态差分列、<c>R</c> 是残差差分列，
    ''' <c>γ</c> 解 <c>min ‖r - R·γ‖²</c>（带极小岭项保证 <c>m×m</c> 的 Gram 矩阵可逆）。
    ''' 外推步不保证下降，因此调用方必须再套一层回溯线搜索——本类只负责给方向。
    ''' </para>
    ''' <para>
    ''' 深度为 0 时退化成「方向 = 残差」，即 <c>ω = 1</c> 的全额迭代。
    ''' </para>
    ''' </remarks>
    Friend Class AndersonAcceleration

        Private ReadOnly _n As Integer
        Private ReadOnly _depth As Integer
        Private ReadOnly _m As Integer
        Private ReadOnly _dz As Double()()
        Private ReadOnly _dr As Double()()
        Private ReadOnly _prevZ As Double()
        Private ReadOnly _prevR As Double()
        Private ReadOnly _gamma As Double()
        Private ReadOnly _gram As Double(,)
        Private ReadOnly _rhs As Double()
        Private ReadOnly _clampIndex As List(Of Integer)

        Private _count As Integer
        Private _cursor As Integer
        Private _initialized As Boolean

        ''' <summary>按历史深度创建加速器（缓冲一次性分配）。</summary>
        ''' <param name="context">求解上下文</param>
        ''' <param name="depth">历史深度；0 表示不做外推</param>
        Public Sub New(context As SolveContext, depth As Integer)
            _n = context.Network.NGene
            _depth = std.Max(0, depth)
            _m = std.Max(1, _depth)
            _clampIndex = context.ClampIndex

            _dz = New Double(_m - 1)() {}
            _dr = New Double(_m - 1)() {}

            For i As Integer = 0 To _m - 1
                _dz(i) = New Double(_n - 1) {}
                _dr(i) = New Double(_n - 1) {}
            Next

            _prevZ = New Double(_n - 1) {}
            _prevR = New Double(_n - 1) {}
            _gamma = New Double(_m - 1) {}
            _gram = New Double(_m - 1, _m - 1) {}
            _rhs = New Double(_m - 1) {}
        End Sub

        ''' <summary>
        ''' 记录一步（在状态与残差都更新之后调用）。
        ''' </summary>
        ''' <param name="z">新的状态</param>
        ''' <param name="residual">新的残差</param>
        ''' <remarks>第一次调用只建立基准，不产生差分列。</remarks>
        Public Sub Update(z As Double(), residual As Double())
            If Not _initialized Then
                Call Array.Copy(z, _prevZ, _n)
                Call Array.Copy(residual, _prevR, _n)
                _initialized = True

                Return
            End If

            Dim slot As Integer = _cursor
            Dim dzColumn As Double() = _dz(slot)
            Dim drColumn As Double() = _dr(slot)

            For g As Integer = 0 To _n - 1
                dzColumn(g) = z(g) - _prevZ(g)
                drColumn(g) = residual(g) - _prevR(g)
            Next

            Call Array.Copy(z, _prevZ, _n)
            Call Array.Copy(residual, _prevR, _n)

            _cursor = (slot + 1) Mod _m

            If _count < _m Then
                _count += 1
            End If
        End Sub

        ''' <summary>计算外推方向（写入 <paramref name="direction"/>）。</summary>
        ''' <param name="residual">当前残差</param>
        ''' <param name="direction">输出方向（长度 = 基因数）</param>
        Public Sub ComputeStep(residual As Double(), direction As Double())
            Dim m As Integer = _count

            If _depth = 0 OrElse m = 0 Then
                Call Array.Copy(residual, direction, _n)
                Call ZeroClamped(direction)

                Return
            End If

            Dim trace As Double = 0.0

            For i As Integer = 0 To m - 1
                For j As Integer = 0 To m - 1
                    Dim value As Double = Dot(_dr(i), _dr(j))

                    _gram(i, j) = value

                    If i = j Then
                        trace += value
                    End If
                Next

                _rhs(i) = Dot(_dr(i), residual)
            Next

            ' 极小岭项：差分列近共线时 Gram 会接近奇异，γ 会炸开
            Dim ridge As Double = std.Max(trace, 1.0) * 1.0E-10 / m

            For i As Integer = 0 To m - 1
                _gram(i, i) += ridge
            Next

            If Not SolveSmall(m) Then
                Call Array.Copy(residual, direction, _n)
                Call ZeroClamped(direction)

                Return
            End If

            For g As Integer = 0 To _n - 1
                Dim correction As Double = 0.0

                For i As Integer = 0 To m - 1
                    Dim weight As Double = _gamma(i)

                    If weight <> 0.0 Then
                        correction += weight * (_dz(i)(g) + _dr(i)(g))
                    End If
                Next

                direction(g) = residual(g) - correction
            Next

            Call ZeroClamped(direction)
        End Sub

        ''' <summary>解 <c>m×m</c> 的 Gram 方程（部分主元高斯消元）。</summary>
        Private Function SolveSmall(m As Integer) As Boolean
            For col As Integer = 0 To m - 1
                Dim pivot As Integer = col
                Dim peak As Double = std.Abs(_gram(col, col))

                For row As Integer = col + 1 To m - 1
                    Dim value As Double = std.Abs(_gram(row, col))

                    If value > peak Then
                        peak = value
                        pivot = row
                    End If
                Next

                If peak < 1.0E-18 Then
                    Return False
                End If

                If pivot <> col Then
                    For j As Integer = col To m - 1
                        Dim temp As Double = _gram(col, j)

                        _gram(col, j) = _gram(pivot, j)
                        _gram(pivot, j) = temp
                    Next

                    Dim t As Double = _rhs(col)

                    _rhs(col) = _rhs(pivot)
                    _rhs(pivot) = t
                End If

                For row As Integer = col + 1 To m - 1
                    Dim factor As Double = _gram(row, col) / _gram(col, col)

                    If factor = 0.0 Then
                        Continue For
                    End If

                    For j As Integer = col To m - 1
                        _gram(row, j) -= factor * _gram(col, j)
                    Next

                    _rhs(row) -= factor * _rhs(col)
                Next
            Next

            For row As Integer = m - 1 To 0 Step -1
                Dim sum As Double = _rhs(row)

                For j As Integer = row + 1 To m - 1
                    sum -= _gram(row, j) * _gamma(j)
                Next

                If std.Abs(_gram(row, row)) < 1.0E-18 Then
                    Return False
                End If

                _gamma(row) = sum / _gram(row, row)
            Next

            Return True
        End Function

        Private Sub ZeroClamped(direction As Double())
            For k As Integer = 0 To _clampIndex.Count - 1
                direction(_clampIndex(k)) = 0.0
            Next
        End Sub

        Private Shared Function Dot(a As Double(), b As Double()) As Double
            Dim sum As Double = 0.0

            For i As Integer = 0 To a.Length - 1
                sum += a(i) * b(i)
            Next

            Return sum
        End Function

    End Class

End Namespace
