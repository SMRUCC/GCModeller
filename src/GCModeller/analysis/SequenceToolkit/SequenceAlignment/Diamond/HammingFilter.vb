' Hamming 距离过滤 —— 标量实现 (48aa 窗口)
'
' 在种子命中点周围 <see cref="WindowSize"/> 个氨基酸窗口内,逐位置比较
' 查询与参考的残基(使用缩减字母表,使保守替换不计入距离)。距离超过阈值
' 的命中被丢弃。这一步比基于替换矩阵的无空位比对快约一个数量级。
'
' 说明:DIAMOND 原始实现使用 SSE2 (pcmpeqb/pmovmskb/popcnt) 高度优化本步骤。
' 此处为可正确运行的标量版本,接口与 <see cref="IHammingFilter"/> 一致,
' 后续可无缝替换为 X86 向量化实现。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 标量 Hamming 距离过滤器(48aa 窗口)。
    ''' </summary>
    Public Class HammingFilter : Implements IHammingFilter

        ''' <summary>DIAMOND 默认初筛窗口大小(氨基酸数)。</summary>
        Public Const WindowSize As Integer = 48

        ''' <summary>
        ''' 允许的最大 Hamming 距离(缩减字母表空间)。默认 0.4 * WindowSize,
        ''' 即窗口内至多 40% 位置不同即视为通过初筛。
        ''' </summary>
        Public ReadOnly MaxDistance As Integer

        Sub New(Optional maxDistance As Integer = -1)
            If maxDistance < 0 Then
                Me.MaxDistance = CInt(WindowSize * 0.4)
            Else
                Me.MaxDistance = maxDistance
            End If
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Distance(query As String, qPos As Integer, subject As String, sPos As Integer) As Integer Implements IHammingFilter.Distance
            Dim len = Math.Min(WindowSize, Math.Min(query.Length - qPos, subject.Length - sPos))

            If len <= 0 Then
                Return WindowSize
            End If

            Dim dist As Integer = 0

            For k As Integer = 0 To len - 1
                If ReducedAlphabet.Map(query(qPos + k)) <> ReducedAlphabet.Map(subject(sPos + k)) Then
                    dist += 1
                End If
            Next

            Return dist
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Pass(query As String, qPos As Integer, subject As String, sPos As Integer) As Boolean Implements IHammingFilter.Pass
            Return Distance(query, qPos, subject, sPos) <= MaxDistance
        End Function
    End Class
End Namespace
