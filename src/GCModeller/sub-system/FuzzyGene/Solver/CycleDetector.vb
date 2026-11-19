Imports System
Imports System.Collections.Generic
Imports std = System.Math

Namespace Solver

    ''' <summary>
    ''' 极限环检测：在最近若干帧里寻找重复状态。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 阻尼迭代在 <c>ρ(J) ≈ 1</c> 或存在负实特征值（周期 2 振荡）时不会发散也不会收敛，
    ''' 而是沿一个闭轨打转。此时「跑满 50 步、报告未收敛」是没有信息量的：
    ''' 轨道的周期本身就是结论（生物上的持续振荡），周期均值可以作为稳态代理。
    ''' </para>
    ''' <para>
    ''' 判定用「量化到 1e-6 的哈希 + 逐元素精确比对」两级：哈希负责 O(窗口) 的快速筛选，
    ''' 精确比对负责排除哈希碰撞造成的假阳性——把不收敛误报成极限环比漏报更糟。
    ''' </para>
    ''' </remarks>
    Friend Class CycleDetector

        Private ReadOnly _window As Integer
        Private ReadOnly _keys As New List(Of Long)()
        Private ReadOnly _states As New List(Of Double())()
        Private ReadOnly _indices As New List(Of Integer)()

        Private _frame As Integer

        ''' <summary>按检测窗口创建（窗口 = 最大可识别周期）。</summary>
        Public Sub New(window As Integer)
            _window = std.Max(1, window)
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

                If period >= 1 AndAlso period <= _window AndAlso SameState(_states(i), z) Then
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

        ''' <summary>量化到 1e-6 后做整向量的顺序哈希（只用于快速筛选）。</summary>
        Private Shared Function Hash(z As Double()) As Long
            Dim h As Long = 17

            For i As Integer = 0 To z.Length - 1
                Dim scaled As Double = z(i) * 1000000.0

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

        ''' <summary>逐元素比较（量化后），排除哈希碰撞。</summary>
        Private Shared Function SameState(a As Double(), b As Double()) As Boolean
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then
                Return False
            End If

            For i As Integer = 0 To a.Length - 1
                If std.Round(a(i), 6) <> std.Round(b(i), 6) Then
                    Return False
                End If
            Next

            Return True
        End Function

    End Class

End Namespace
