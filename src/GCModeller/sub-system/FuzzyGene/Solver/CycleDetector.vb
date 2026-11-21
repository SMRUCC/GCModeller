Imports System
Imports System.Collections.Generic
Imports std = System.Math

Namespace Solver

    ''' <summary>
    ''' 极限环检测：在最近若干帧里寻找**近似**重复状态。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 阻尼迭代在 <c>ρ(J) ≈ 1</c> 或存在负实特征值（周期 2 振荡）时不会发散也不会收敛，
    ''' 而是沿一个闭轨打转。此时「跑满步数、报告未收敛」是没有信息量的：
    ''' 轨道的周期本身就是结论（生物上的持续振荡），周期均值可以作为稳态代理。
    ''' </para>
    ''' <para>
    ''' <b>为什么不能用精确比对</b>：真实模型上的振荡往往不是逐位重复的。以生产模型
    ''' （18533 基因）实测为例，残差在 1.4 与 10.6 之间来回跳，但两次「同一相位」的
    ''' 状态相差 1e-2 量级——精确比对（旧实现量化到 1e-6 后逐元素相等）永远检不出，
    ''' 于是 250 步全跑完还报 <c>MaxIterations</c>，把「系统性振荡」误报成「没跑够」。
    ''' 这里改成按容差比对：同一相位只要落在 <c>tolerance</c> 内就算重复。
    ''' </para>
    ''' <para>
    ''' 两级判定：先用粗量化（1e-4）的哈希做 O(窗口) 快速筛选，再用精确的
    ''' 逐元素 |Δ| ≤ 容差 复核——哈希只负责剪枝，不会因为碰撞产生假阳性。
    ''' 容差默认取求解器容差的 100 倍：既要能抓住「振幅远大于容差的稳定振荡」，
    ''' 又不能把「缓慢收敛到不动点」误判成周期 1（周期 1 直接排除）。
    ''' </para>
    ''' </remarks>
    Friend Class CycleDetector

        ''' <summary>哈希量化步长（只用于剪枝，越粗越容易命中，最终由精确比对把关）。</summary>
        Private Const HashScale As Double = 10000.0

        Private ReadOnly _window As Integer
        Private ReadOnly _tolerance As Double
        Private ReadOnly _keys As New List(Of Long)()
        Private ReadOnly _states As New List(Of Double())()
        Private ReadOnly _indices As New List(Of Integer)()

        Private _frame As Integer

        ''' <summary>按检测窗口创建（窗口 = 最大可识别周期）。</summary>
        ''' <param name="window">最大可识别周期</param>
        ''' <param name="tolerance">
        ''' 判定「同一相位」的状态容差（z 空间）。建议取求解器容差的 100 倍：
        ''' 太小检不出真实振荡，太大则会把慢收敛误报成极限环
        ''' </param>
        Public Sub New(window As Integer, Optional tolerance As Double = 0.0001)
            _window = std.Max(1, window)
            _tolerance = If(tolerance > 0.0, tolerance, 0.0001)
        End Sub

        ''' <summary>
        ''' 记录一帧并检测是否回到历史状态。
        ''' </summary>
        ''' <param name="z">当前状态（z 空间）</param>
        ''' <returns>检出的周期；<c>0</c> 表示没有检出</returns>
        Public Function Push(z As Double()) As Integer
            Dim key As Long = Hash(z)
            Dim lower As Integer = std.Max(0, _keys.Count - _window)

            For i As Integer = _keys.Count - 1 To lower Step -1
                If _keys(i) <> key Then
                    Continue For
                End If

                Dim period As Integer = _frame - _indices(i)

                ' 周期 1 意味着「状态没变」，那是不动点而不是极限环（收敛判据会先处理）
                If period >= 2 AndAlso period <= _window AndAlso SameState(_states(i), z) Then
                    Return period
                End If
            Next

            Call _keys.Add(key)
            Call _states.Add(CType(z.Clone(), Double()))
            Call _indices.Add(_frame)

            _frame += 1

            ' 只保留最近 2×窗口 帧：再早的帧不可能构成 ≤ 窗口 的周期
            While _keys.Count > _window * 2
                Call _keys.RemoveAt(0)
                Call _states.RemoveAt(0)
                Call _indices.RemoveAt(0)
            End While

            Return 0
        End Function

        ''' <summary>
        ''' 取最近 <paramref name="period"/> 帧的均值（周期均值）。
        ''' </summary>
        ''' <param name="period">检出的周期</param>
        ''' <returns>均值向量；帧数不足时返回 <c>Nothing</c></returns>
        ''' <remarks>
        ''' 给「没有轨迹快照」的调用路径（<c>Predict</c> 不建 trace）用：
        ''' 检测器自己保留了最近 2×窗口 帧，足够算一个周期均值。
        ''' </remarks>
        Public Function MeanOfLast(period As Integer) As Double()
            If period <= 0 OrElse _states.Count < period Then
                Return Nothing
            End If

            Dim genes As Integer = _states(0).Length
            Dim mean(genes - 1) As Double
            Dim start As Integer = _states.Count - period

            For i As Integer = start To _states.Count - 1
                Dim frame As Double() = _states(i)

                For g As Integer = 0 To genes - 1
                    mean(g) += frame(g)
                Next
            Next

            For g As Integer = 0 To genes - 1
                mean(g) /= period
            Next

            Return mean
        End Function

        ''' <summary>粗量化后做整向量的顺序哈希（只用于快速筛选）。</summary>
        Private Shared Function Hash(z As Double()) As Long
            Dim h As Long = 17

            For i As Integer = 0 To z.Length - 1
                Dim scaled As Double = z(i) * HashScale

                ' NaN / Inf 不参与哈希（CLng 会抛 OverflowException）；
                ' 夹到 ±1e15 是为了让极端外推值也不会溢出
                If Double.IsNaN(scaled) Then
                    Continue For
                End If
                If scaled > 1.0E15 Then
                    scaled = 1.0E15
                ElseIf scaled < -1.0E15 Then
                    scaled = -1.0E15
                End If

                ' 只用 XOR：VB 的整数运算默认是 checked，乘法溢出会抛异常
                h = h Xor CLng(std.Round(scaled))
            Next

            Return h
        End Function

        ''' <summary>逐元素按容差比较（排除哈希碰撞）。</summary>
        Private Function SameState(a As Double(), b As Double()) As Boolean
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then
                Return False
            End If

            For i As Integer = 0 To a.Length - 1
                Dim left As Double = a(i)
                Dim right As Double = b(i)

                If Double.IsNaN(left) OrElse Double.IsNaN(right) Then
                    ' NaN 只能与 NaN 视为相同，否则整条轨迹永远检不出周期
                    If Double.IsNaN(left) <> Double.IsNaN(right) Then
                        Return False
                    End If

                    Continue For
                End If

                If std.Abs(left - right) > _tolerance Then
                    Return False
                End If
            Next

            Return True
        End Function

    End Class

End Namespace
