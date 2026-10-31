' 带状 Smith-Waterman 扩展 (Banded Smith-Waterman Extension)
'
' 通过前期过滤链的少数候选进入有带限制(有空位)的局部 Smith-Waterman 比对。
' DIAMOND 中比对带宽度由种子链确定,只计算对角带内的动态规划单元,
' 复杂度由 O(mn) 降为 O(band * n)。
'
' 窗口隔离策略保持不变:以无空位延伸得到的候选窗口为中心,向四周扩展
' <see cref="BandMargin"/> 截取局部子串,只在该窗口上计算。
' 但窗口内的动态规划现在由 <see cref="BandedSmithWaterman"/> 在"带内"完成,
' 不再构造 <see cref="SmithWaterman"/> 对象与其 (m+1)×(n+1) 锯齿矩阵 ——
' 百万级候选规模下,后者的对象/矩阵分配本身就是主要开销。
'
' 如需回退到历史实现(GSW 全窗口计算),把 enableBandedSw 置为 False 即可。

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman

Namespace DIAMOND

    ''' <summary>
    ''' 一次带状 SW 比对的结果(HSP),坐标均为全局 0-based。
    ''' </summary>
    Public Structure BandHit
        Public ReadOnly QueryStart As Integer
        Public ReadOnly QueryEnd As Integer   ' 含
        Public ReadOnly SubjectStart As Integer
        Public ReadOnly SubjectEnd As Integer ' 含
        Public ReadOnly Score As Double
        Public ReadOnly QueryFragment As String
        Public ReadOnly SubjectFragment As String

        Sub New(qStart As Integer, qEnd As Integer, sStart As Integer, sEnd As Integer, score As Double, qFrag As String, sFrag As String)
            Me.QueryStart = qStart
            Me.QueryEnd = qEnd
            Me.SubjectStart = sStart
            Me.SubjectEnd = sEnd
            Me.Score = score
            Me.QueryFragment = qFrag
            Me.SubjectFragment = sFrag
        End Sub
    End Structure

    Public Class BandSW

        ''' <summary>
        ''' 在候选窗口四周扩展的边距(氨基酸数),用于捕获带外的有空位延伸。
        ''' 该值即"带状"的半带宽:实际计算窗口 = 无空位命中 + 2 * BandMargin。
        ''' </summary>
        Public ReadOnly BandMargin As Integer

        ''' <summary>产出 HSP 的最低得分阈值。</summary>
        Public ReadOnly MinScore As Double

        ''' <summary>是否使用带内 SW 实现(False 表示回退到历史 GSW 全窗口实现)。</summary>
        Public ReadOnly EnableBandedSW As Boolean

        Private ReadOnly scorer As AminoAcidScorer
        Private ReadOnly banded As BandedSmithWaterman

        ' 字符串路径下按线程复用的窗口编码缓冲
        <ThreadStatic> Private Shared qWindow As Byte()
        <ThreadStatic> Private Shared sWindow As Byte()

        Sub New(Optional bandMargin As Integer = 30,
                Optional minScore As Double = 0.0,
                Optional blosum As Blosum = Nothing,
                Optional enableBandedSw As Boolean = True)

            Me.BandMargin = bandMargin
            Me.MinScore = minScore
            Me.EnableBandedSW = enableBandedSw
            Me.scorer = If(blosum Is Nothing, AminoAcidScorer.DefaultScorer, New AminoAcidScorer(blosum))
            Me.banded = New BandedSmithWaterman(bandMargin)
        End Sub

        ''' <summary>
        ''' 对一条候选(无空位延伸结果)执行带状 SW,返回全局坐标 HSP。
        ''' 若窗口 SW 得分低于 <see cref="MinScore"/> 则返回 Nothing。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Align(globalQuery As String, globalSubject As String, seed As UngappedHit) As BandHit?
            Dim q0 = Math.Max(0, seed.QueryStart - BandMargin)
            Dim q1 = Math.Min(globalQuery.Length - 1, seed.QueryEnd + BandMargin)
            Dim s0 = Math.Max(0, seed.SubjectStart - BandMargin)
            Dim s1 = Math.Min(globalSubject.Length - 1, seed.SubjectEnd + BandMargin)

            If q1 < q0 OrElse s1 < s0 Then
                Return Nothing
            End If

            Dim qLen = q1 - q0 + 1
            Dim sLen = s1 - s0 + 1

            If Not EnableBandedSW Then
                Return AlignLegacy(globalQuery, globalSubject, q0, qLen, s0, sLen)
            End If

            ' 窗口编码到线程缓冲(仅窗口长度,量级为数百字节)
            If qWindow Is Nothing OrElse qWindow.Length < qLen Then
                qWindow = New Byte(Math.Max(qLen, 512) - 1) {}
            End If

            If sWindow Is Nothing OrElse sWindow.Length < sLen Then
                sWindow = New Byte(Math.Max(sLen, 512) - 1) {}
            End If

            Dim codes As Byte() = scorer.Codes

            For k As Integer = 0 To qLen - 1
                Dim c As Integer = AscW(globalQuery(q0 + k))
                qWindow(k) = If(c < 0 OrElse c > 255, codes(0), codes(c))
            Next

            For k As Integer = 0 To sLen - 1
                Dim c As Integer = AscW(globalSubject(s0 + k))
                sWindow(k) = If(c < 0 OrElse c > 255, codes(0), codes(c))
            Next

            Dim res = banded.Align(qWindow, 0, qLen, sWindow, 0, sLen, scorer, CInt(Math.Ceiling(MinScore)))

            If Not res.HasValue Then
                Return Nothing
            End If

            Return New BandHit(
                q0 + res.QueryStart,
                q0 + res.QueryEnd,
                s0 + res.SubjectStart,
                s0 + res.SubjectEnd,
                res.Score,
                globalQuery.Substring(q0 + res.QueryStart, res.QueryEnd - res.QueryStart + 1),
                globalSubject.Substring(s0 + res.SubjectStart, res.SubjectEnd - res.SubjectStart + 1)
            )
        End Function

        ''' <summary>
        ''' 预编码缓冲区版本(热路径):直接在扁平氨基酸编码缓冲上做带内 SW,
        ''' 免去窗口子串与逐字符编码。
        ''' </summary>
        ''' <param name="querySeq">查询序列字符串(用于截取比对片段)。</param>
        ''' <param name="queryPacked">查询序列的氨基酸编码扁平缓冲区。</param>
        ''' <param name="qBase">查询序列首残基在缓冲区中的下标。</param>
        ''' <param name="subjectSeq">参考序列字符串。</param>
        ''' <param name="subjectPacked">参考库的氨基酸编码扁平缓冲区。</param>
        ''' <param name="sBase">参考序列首残基在缓冲区中的下标。</param>
        ''' <param name="seed">无空位延伸结果(坐标相对各自序列起点)。</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Align(querySeq As String, queryPacked As Byte(), qBase As Integer,
                              subjectSeq As String, subjectPacked As Byte(), sBase As Integer,
                              seed As UngappedHit) As BandHit?

            Dim q0 = Math.Max(0, seed.QueryStart - BandMargin)
            Dim q1 = Math.Min(querySeq.Length - 1, seed.QueryEnd + BandMargin)
            Dim s0 = Math.Max(0, seed.SubjectStart - BandMargin)
            Dim s1 = Math.Min(subjectSeq.Length - 1, seed.SubjectEnd + BandMargin)

            If q1 < q0 OrElse s1 < s0 Then
                Return Nothing
            End If

            Dim qLen = q1 - q0 + 1
            Dim sLen = s1 - s0 + 1

            If Not EnableBandedSW Then
                Return AlignLegacy(querySeq, subjectSeq, q0, qLen, s0, sLen)
            End If

            Dim res = banded.Align(queryPacked, qBase + q0, qLen, subjectPacked, sBase + s0, sLen, scorer, CInt(Math.Ceiling(MinScore)))

            If Not res.HasValue Then
                Return Nothing
            End If

            Return New BandHit(
                q0 + res.QueryStart,
                q0 + res.QueryEnd,
                s0 + res.SubjectStart,
                s0 + res.SubjectEnd,
                res.Score,
                querySeq.Substring(q0 + res.QueryStart, res.QueryEnd - res.QueryStart + 1),
                subjectSeq.Substring(s0 + res.SubjectStart, res.SubjectEnd - res.SubjectStart + 1)
            )
        End Function

        ''' <summary>
        ''' 历史实现:窗口内完整 GSW(Smith-Waterman + BLOSUM62)。
        ''' 保留用于结果对照与回退。
        ''' </summary>
        Private Function AlignLegacy(globalQuery As String, globalSubject As String,
                                     q0 As Integer, qLen As Integer, s0 As Integer, sLen As Integer) As BandHit?
            Dim qWin = globalQuery.Substring(q0, qLen)
            Dim sWin = globalSubject.Substring(s0, sLen)

            Dim result As GSW(Of Char) = New SmithWaterman(qWin, sWin).BuildMatrix()
            Dim matches As List(Of Match) = result.Matches(MinScore).ToList

            If matches.Count = 0 Then
                Return Nothing
            End If

            Dim best = matches(0)
            Dim relQStart = best.fromA - 1
            Dim relQEnd = best.toA - 1
            Dim relSStart = best.fromB - 1
            Dim relSEnd = best.toB - 1

            Return New BandHit(
                q0 + relQStart,
                q0 + relQEnd,
                s0 + relSStart,
                s0 + relSEnd,
                best.score,
                qWin.Substring(relQStart, relQEnd - relQStart + 1),
                sWin.Substring(relSStart, relSEnd - relSStart + 1)
            )
        End Function
    End Class
End Namespace
