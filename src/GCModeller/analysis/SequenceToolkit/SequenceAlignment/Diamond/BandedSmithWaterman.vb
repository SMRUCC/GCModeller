' 带状 Smith-Waterman 局部比对 (Banded Smith-Waterman)
'
' DIAMOND 的最后一个阶段只对"通过全部过滤链的极少数候选"做有空位局部比对。
' 原实现每次调用都 new 一个 <see cref="SmithWaterman"/> 对象:
'   - 构造时按窗口大小分配 score / prevCells 两个 (m+1)×(n+1) 的锯齿矩阵;
'   - BuildMatrix 内层逐格调用 Blosum.GetDistance(字符串线性查找);
'   - 之后还要 Matches(threshold) 全矩阵扫描 + 排序 + ToList。
' 在"百万次候选"的规模下,仅矩阵分配一项就足以让 GC 崩溃。
'
' 本实现:
'   1) 只在以种子对角线为中心的带内推进(带宽 2 × BandMargin + 1),
'      计算量从 O(m·n) 降到 O(band · n);
'   2) 使用按线程复用的扁平缓冲(按需扩容,不重复分配);
'   3) 打分走 <see cref="AminoAcidScorer.Scores"/> 扁平查表;
'   4) gap 模型与 <see cref="DynamicProgramming.SmithWaterman.GSW"/> 一致
'      (线性 gap,INDEL_SCORE = -9),保证得分与历史实现可比。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 窗口内局部比对结果(坐标相对窗口起点,0-based)。
    ''' </summary>
    Public Structure LocalAlignmentResult
        Public ReadOnly QueryStart As Integer
        Public ReadOnly QueryEnd As Integer
        Public ReadOnly SubjectStart As Integer
        Public ReadOnly SubjectEnd As Integer
        Public ReadOnly Score As Integer
        Public ReadOnly HasValue As Boolean

        Sub New(qStart As Integer, qEnd As Integer, sStart As Integer, sEnd As Integer, score As Integer)
            Me.QueryStart = qStart
            Me.QueryEnd = qEnd
            Me.SubjectStart = sStart
            Me.SubjectEnd = sEnd
            Me.Score = score
            Me.HasValue = True
        End Sub
    End Structure

    ''' <summary>
    ''' 带状 Smith-Waterman(线性 gap),按线程复用缓冲区。
    ''' </summary>
    Public Class BandedSmithWaterman

        ''' <summary>与 <see cref="DynamicProgramming.SmithWaterman.GSW"/> 一致的线性 gap 罚分。</summary>
        Public Const GAP_SCORE As Integer = -9

        ''' <summary>带半宽(氨基酸数)。</summary>
        Public ReadOnly BandHalfWidth As Integer

        ' 按线程复用的带内得分/方向缓冲
        <ThreadStatic> Private Shared bandScores As Integer()
        <ThreadStatic> Private Shared bandDirs As Byte()

        Sub New(Optional bandHalfWidth As Integer = 30)
            Me.BandHalfWidth = Math.Max(1, bandHalfWidth)
        End Sub

        ''' <summary>
        ''' 在给定窗口内做带状局部比对。
        ''' </summary>
        ''' <param name="query">查询序列的氨基酸编码缓冲区(扁平)。</param>
        ''' <param name="qOffset">查询窗口起点在缓冲区中的绝对下标。</param>
        ''' <param name="qLen">查询窗口长度。</param>
        ''' <param name="subject">参考序列的氨基酸编码缓冲区(扁平)。</param>
        ''' <param name="sOffset">参考窗口起点在缓冲区中的绝对下标。</param>
        ''' <param name="sLen">参考窗口长度。</param>
        ''' <param name="scorer">打分表。</param>
        ''' <param name="minScore">最低得分阈值(0 表示只要正分)。</param>
        Public Function Align(query As Byte(), qOffset As Integer, qLen As Integer,
                              subject As Byte(), sOffset As Integer, sLen As Integer,
                              scorer As AminoAcidScorer,
                              Optional minScore As Integer = 0) As LocalAlignmentResult

            If qLen <= 0 OrElse sLen <= 0 Then
                Return Nothing
            End If

            Dim half As Integer = Math.Min(BandHalfWidth, Math.Max(qLen, sLen))
            Dim stride As Integer = 2 * half + 1
            Dim total As Long = (qLen + 1L) * stride

            If total > Integer.MaxValue Then
                ' 窗口异常巨大时退化为全宽
                half = Math.Max(qLen, sLen)
                stride = 2 * half + 1
                total = (qLen + 1L) * stride
            End If

            If bandScores Is Nothing OrElse bandScores.Length < total Then
                bandScores = New Integer(CInt(total) - 1) {}
                bandDirs = New Byte(CInt(total) - 1) {}
            Else
                ' 仅清理第 0 行(其余行在写入前一定被覆盖)
                Array.Clear(bandScores, 0, stride)
                Array.Clear(bandDirs, 0, stride)
            End If

            Dim scores As Integer() = bandScores
            Dim dirs As Byte() = bandDirs
            Dim table As Integer() = scorer.Scores
            Dim gap As Integer = GAP_SCORE

            Dim bestScore As Integer = 0
            Dim bestI As Integer = 0
            Dim bestJ As Integer = 0
            Dim jLoPrev As Integer = Math.Max(1, -half)
            Dim jHiPrev As Integer = Math.Min(sLen, half)
            Dim pOff As Integer = 0

            For i As Integer = 1 To qLen
                Dim rowOff As Integer = i * stride
                Dim jLo As Integer = Math.Max(1, i - half)
                Dim jHi As Integer = Math.Min(sLen, i + half)
                Dim qc As Byte = query(qOffset + i - 1)

                For j As Integer = jLo To jHi
                    Dim left As Integer = If(j > jLo, scores(rowOff + (j - 1 - jLo)), 0)
                    Dim up As Integer = If(j >= jLoPrev AndAlso j <= jHiPrev, scores(pOff + (j - jLoPrev)), 0)
                    Dim diag As Integer = If(j - 1 >= jLoPrev AndAlso j - 1 <= jHiPrev, scores(pOff + (j - 1 - jLoPrev)), 0)

                    Dim value As Integer = diag + table((CInt(qc) << 5) Or CInt(subject(sOffset + j - 1)))
                    Dim dir As Byte = 1

                    Dim gapLeft As Integer = left + gap

                    If gapLeft > value Then
                        value = gapLeft
                        dir = 3
                    End If

                    Dim gapUp As Integer = up + gap

                    If gapUp > value Then
                        value = gapUp
                        dir = 2
                    End If

                    If value <= 0 Then
                        value = 0
                        dir = 0
                    End If

                    scores(rowOff + (j - jLo)) = value
                    dirs(rowOff + (j - jLo)) = dir

                    If value > bestScore Then
                        bestScore = value
                        bestI = i
                        bestJ = j
                    End If
                Next

                pOff = rowOff
                jLoPrev = jLo
                jHiPrev = jHi
            Next

            ' 行 i 存放于 i * stride 处(第 0 行恒为 0)

            If bestScore <= minScore Then
                Return Nothing
            End If

            ' 回溯:沿着方向位一直退到得分为 0 的格子
            Dim ti As Integer = bestI
            Dim tj As Integer = bestJ

            While ti > 0 AndAlso tj > 0
                Dim jLo = Math.Max(1, ti - half)

                If tj < jLo OrElse tj > Math.Min(sLen, ti + half) Then
                    Exit While
                End If

                Dim dir As Byte = dirs(ti * stride + (tj - jLo))

                If dir = 0 Then
                    Exit While
                ElseIf dir = 1 Then
                    ti -= 1
                    tj -= 1
                ElseIf dir = 2 Then
                    ti -= 1
                Else
                    tj -= 1
                End If
            End While

            ' (ti, tj) 为比对起点前一格,1-based 起点即 (ti+1, tj+1)
            Dim qStart As Integer = ti          ' 0-based
            Dim sStart As Integer = tj
            Dim qEnd As Integer = bestI - 1     ' 0-based
            Dim sEnd As Integer = bestJ - 1

            If qEnd < qStart OrElse sEnd < sStart Then
                Return Nothing
            End If

            Return New LocalAlignmentResult(qStart, qEnd, sStart, sEnd, bestScore)
        End Function
    End Class
End Namespace
