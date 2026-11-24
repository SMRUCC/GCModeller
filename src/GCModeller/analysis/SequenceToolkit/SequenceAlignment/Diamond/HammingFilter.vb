' Hamming 距离过滤 —— 标量实现 (48aa 窗口)
'
' 在种子命中点周围 <see cref="WindowSize"/> 个氨基酸窗口内,逐位置比较
' 查询与参考的残基(使用缩减字母表,使保守替换不计入距离)。距离超过阈值
' 的命中被丢弃。这一步比基于替换矩阵的无空位比对快约一个数量级。
'
' 性能要点(相对最初实现):
'   1) 不再逐残基调用 Dictionary 映射,而是直接在氨基酸编码缓冲区上做
'      "同类判定" 查表(一次内存访问,见 <see cref="AminoAcidScorer.SameReduced"/>);
'   2) 判定为 Pass 的路径带早退:累计距离一旦超过阈值立即返回,
'      因此绝大多数假阳性命中只需扫过窗口的前若干位置;
'   3) 提供预编码缓冲区重载(<see cref="PassPacked"/> / <see cref="DistancePacked"/>),
'      热路径上完全不触碰字符串与字符映射。

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

        ''' <summary>氨基酸编码 / 同类判定查表。</summary>
        Protected ReadOnly scorer As AminoAcidScorer

        Sub New(Optional maxDistance As Integer = -1, Optional scorer As AminoAcidScorer = Nothing)
            If maxDistance < 0 Then
                Me.MaxDistance = CInt(WindowSize * 0.4)
            Else
                Me.MaxDistance = maxDistance
            End If

            Me.scorer = If(scorer, AminoAcidScorer.DefaultScorer)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Distance(query As String, qPos As Integer, subject As String, sPos As Integer) As Integer Implements IHammingFilter.Distance
            Dim len = Math.Min(WindowSize, Math.Min(query.Length - qPos, subject.Length - sPos))

            If len <= 0 Then
                Return WindowSize
            End If

            Dim same As Byte() = scorer.SameReduced
            Dim dist As Integer = 0

            For k As Integer = 0 To len - 1
                Dim a As Integer = AscW(query(qPos + k))
                Dim b As Integer = AscW(subject(sPos + k))

                If same((CInt(scorer.Codes(If(a >= 0 AndAlso a < 256, a, 0))) << 5) Or
                        CInt(scorer.Codes(If(b >= 0 AndAlso b < 256, b, 0)))) = 0 Then
                    dist += 1
                End If
            Next

            Return dist
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Pass(query As String, qPos As Integer, subject As String, sPos As Integer) As Boolean Implements IHammingFilter.Pass
            Return Distance(query, qPos, subject, sPos) <= MaxDistance
        End Function

        ''' <summary>
        ''' 预编码缓冲区版本的通过判定(热路径,带早退)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PassPacked(query As Byte(), qBase As Integer, queryLen As Integer, qPos As Integer,
                                   subject As Byte(), sBase As Integer, subjectLen As Integer, sPos As Integer) As Boolean Implements IHammingFilter.PassPacked
            Dim len = Math.Min(WindowSize, Math.Min(queryLen - qPos, subjectLen - sPos))

            If len <= 0 Then
                Return False
            End If

            Dim same As Byte() = scorer.SameReduced
            Dim max As Integer = MaxDistance
            Dim dist As Integer = 0
            Dim qa As Integer = qBase + qPos
            Dim sa As Integer = sBase + sPos

            For k As Integer = 0 To len - 1
                If same((CInt(query(qa + k)) << 5) Or CInt(subject(sa + k))) = 0 Then
                    dist += 1

                    If dist > max Then
                        Return False
                    End If
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' 预编码缓冲区版本的距离计算。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function DistancePacked(query As Byte(), qBase As Integer, queryLen As Integer, qPos As Integer,
                                       subject As Byte(), sBase As Integer, subjectLen As Integer, sPos As Integer) As Integer Implements IHammingFilter.DistancePacked
            Dim len = Math.Min(WindowSize, Math.Min(queryLen - qPos, subjectLen - sPos))

            If len <= 0 Then
                Return WindowSize
            End If

            Dim same As Byte() = scorer.SameReduced
            Dim dist As Integer = 0
            Dim qa As Integer = qBase + qPos
            Dim sa As Integer = sBase + sPos

            For k As Integer = 0 To len - 1
                If same((CInt(query(qa + k)) << 5) Or CInt(subject(sa + k))) = 0 Then
                    dist += 1
                End If
            Next

            Return dist
        End Function
    End Class
End Namespace
