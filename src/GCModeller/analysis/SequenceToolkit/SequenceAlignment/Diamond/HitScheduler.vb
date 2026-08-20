' 排序启发式分块触发 (Sorted Heuristic Scheduling)
'
' 对单个查询的所有候选目标,按无空位延伸得分建立降序排序,并以分块方式处理:
' 每次取一个高分块,对其中的候选触发昂贵的带状 Smith-Waterman;一旦当前块不再
' 产生满足报告标准的比对,即停止后续块的处理,进一步压缩昂贵的动态规划计算量。
'
' 本调度器不直接执行 SW,而是产出候选的优先处理顺序,并支持"早停"判定,
' 由 <see cref="DiamondBlastp"/> 顶层入口驱动实际比对。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 待调度候选(已通过全部前期过滤,带无空位延伸得分)。
    ''' </summary>
    Public Structure ScheduledHit
        Public ReadOnly QueryPos As Integer
        Public ReadOnly SubjectId As Integer
        Public ReadOnly SubjectPos As Integer
        Public ReadOnly Ungapped As UngappedHit
        Public ReadOnly Hamming As Integer

        Sub New(queryPos As Integer, subjectId As Integer, subjectPos As Integer, ungapped As UngappedHit, hamming As Integer)
            Me.QueryPos = queryPos
            Me.SubjectId = subjectId
            Me.SubjectPos = subjectPos
            Me.Ungapped = ungapped
            Me.Hamming = hamming
        End Sub
    End Structure

    Public Class HitScheduler

        ''' <summary>每个处理块的大小(候选数)。</summary>
        Public ReadOnly BlockSize As Integer

        ''' <summary>
        ''' 命中得分需达到该值才会被纳入调度(低于此值的无空位延伸结果直接丢弃)。
        ''' </summary>
        Public ReadOnly MinUngappedScore As Integer

        Sub New(Optional blockSize As Integer = 16, Optional minUngappedScore As Integer = 0)
            Me.BlockSize = blockSize
            Me.MinUngappedScore = minUngappedScore
        End Sub

        ''' <summary>
        ''' 将候选按无空位延伸得分降序排序并分块,逐块产出候选列表。
        ''' 调用方对每块执行带状 SW,并通过 <see cref="ShouldStop"/> 决定是否早停。
        ''' </summary>
        Public Iterator Function Schedule(candidates As IEnumerable(Of ScheduledHit)) As IEnumerable(Of List(Of ScheduledHit))
            Dim ordered = candidates _
                .Where(Function(c) c.Ungapped.Score >= MinUngappedScore) _
                .OrderByDescending(Function(c) c.Ungapped.Score) _
                .ToArray

            For i As Integer = 0 To ordered.Length - 1 Step BlockSize
                Dim block As New List(Of ScheduledHit)

                For j As Integer = i To Math.Min(i + BlockSize - 1, ordered.Length - 1)
                    block.Add(ordered(j))
                Next

                Yield block
            Next
        End Function

        ''' <summary>
        ''' 早停判定:若当前块产出的达标带状 SW 比对数为 0,则可停止后续块。
        ''' (DIAMOND 中一旦当前块不再产生满足报告标准的比对即停止。)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ShouldStop(producedInBlock As Integer) As Boolean
            Return producedInBlock <= 0
        End Function
    End Class
End Namespace
