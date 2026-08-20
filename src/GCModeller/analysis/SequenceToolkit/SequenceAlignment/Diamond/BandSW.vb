' 带状 Smith-Waterman 扩展 (Banded Smith-Waterman Extension)
'
' 通过前期过滤链的少数候选进入有带限制(有空位)的局部 Smith-Waterman 比对。
' DIAMOND 中比对带宽度由种子链确定,只计算对角带内的动态规划单元,
' 复杂度由 O(mn) 降为 O(band * n)。
'
' 本实现复用成熟的 BestLocalAlignment.SmithWaterman (GSW + BLOSUM62) 计算核心,
' 但通过"窗口隔离"达成带状效果:以无空位延伸得到的候选窗口为中心,向四周扩展
' 一个固定边距 (BandMargin) 截取出局部子串,仅在该小窗口上运行 SW。由于窗口
' 已被隔离到种子周围,实际计算量集中在对角带内,等价于带状 SW,且完全复用
' 经过验证的 GSW 内核,保证比对正确性。
'
' 输出的 HSP 坐标从窗口相对坐标偏移回全局(原始查询/参考)坐标。

Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.BestLocalAlignment
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.DynamicProgramming

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

        Private ReadOnly sw As SmithWaterman

        Sub New(Optional bandMargin As Integer = 30, Optional minScore As Double = 0.0, Optional blosum As Blosum = Nothing)
            Me.BandMargin = bandMargin
            Me.MinScore = minScore
            Me.sw = New SmithWaterman(If(blosum, Blosum.FromInnerBlosum62))
        End Sub

        ''' <summary>
        ''' 对一条候选(无空位延伸结果)执行带状 SW,返回全局坐标 HSP。
        ''' 若窗口 SW 得分低于 <see cref="MinScore"/> 则返回 Nothing。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Align(globalQuery As String, globalSubject As String, seed As UngappedHit) As BandHit?
            ' 隔离候选窗口,向四周扩展 BandMargin
            Dim q0 = Math.Max(0, seed.QueryStart - BandMargin)
            Dim q1 = Math.Min(globalQuery.Length - 1, seed.QueryEnd + BandMargin)
            Dim s0 = Math.Max(0, seed.SubjectStart - BandMargin)
            Dim s1 = Math.Min(globalSubject.Length - 1, seed.SubjectEnd + BandMargin)

            If q1 < q0 OrElse s1 < s0 Then
                Return Nothing
            End If

            Dim qWin = globalQuery.Substring(q0, q1 - q0 + 1)
            Dim sWin = globalSubject.Substring(s0, s1 - s0 + 1)

            Dim result = sw.Align(qWin, sWin)
            Dim matches = result.Matches(MinScore)

            If matches.Count = 0 Then
                Return Nothing
            End If

            ' Matches 返回 1-based 坐标(下标 1 = 序列首),需转 0-based 再偏移
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
