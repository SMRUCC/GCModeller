' Hamming 距离过滤 —— SSE2 向量化实现
'
' 实现 <see cref="IHammingFilter"/> 接口,在 48aa 窗口内用 SSE2 指令
' (pcmpeqb / pmovmskb / popcnt) 逐 16 字节比较查询与参考的缩减字母字节。
' 每个氨基酸经 <see cref="ReducedAlphabet"/> 映射为 0-10 的字节值,窗口内
' 一致字节越多、Hamming 距离越小。向量化版本比标量实现快约一个数量级。
'
' 运行时按 <see cref="Sse2.IsSupported"/> 选择向量化或标量回退:
' 不支持 SSE2(x86 32 位 / 非 x86 平台)时自动退化为标量逐字节比较,
' 保证跨平台可编译可运行且结果一致。

Imports System
Imports System.Numerics
Imports System.Runtime.CompilerServices
Imports System.Runtime.Intrinsics
Imports System.Runtime.Intrinsics.X86

Namespace DIAMOND

    ''' <summary>
    ''' SSE2 向量化 Hamming 距离过滤器(48aa 窗口),实现 IHammingFilter 接口。
    ''' </summary>
    Public Class HammingFilterSse : Implements IHammingFilter

        ''' <summary>DIAMOND 默认初筛窗口大小(氨基酸数)。</summary>
        Public Const WindowSize As Integer = 48

        ''' <summary>允许的最大 Hamming 距离(缩减字母表空间)。</summary>
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

            ' 将窗口内残基转为缩减字母字节(0-10),供 SSE2 逐字节比较
            Dim qb(len - 1) As Byte
            Dim sb(len - 1) As Byte

            For k As Integer = 0 To len - 1
                qb(k) = CByte(ReducedAlphabet.Map(query(qPos + k)))
                sb(k) = CByte(ReducedAlphabet.Map(subject(sPos + k)))
            Next

            Dim dist As Integer = 0
            Dim i As Integer = 0

            If Sse2.IsSupported Then
                ' 每次比较 16 个字节
                While i + 16 <= len
                    Dim vq = Sse2.LoadVector128(qb.AsSpan(i, 16))
                    Dim vs = Sse2.LoadVector128(sb.AsSpan(i, 16))
                    Dim eq = Sse2.CompareEqual(vq, vs)
                    Dim mask = Sse2.MoveMask(eq)          ' 16 位:相等位为 1
                    dist += 16 - BitOperations.PopCount(CUInt(mask))
                    i += 16
                End While
            End If

            ' 尾部(不足 16 字节)及非 SSE2 回退:标量补齐
            For k As Integer = i To len - 1
                If qb(k) <> sb(k) Then
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
