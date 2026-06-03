' ========================================================================
' 动态规划基因选择
' ========================================================================

''' <summary>
''' 动态规划基因选择算法
''' 在满足基因不重叠（同链）等约束下，选择全局得分最高的基因组合。
''' 本质上是加权区间调度问题（Weighted Interval Scheduling）的变体。
''' 
''' 算法：
''' 1. 将所有候选ORF按终止位置排序
''' 2. 对每个ORF，找到不与其重叠的最近前驱ORF
''' 3. DP递推：dp[i] = max(score[i] + dp[p(i)], dp[i-1])
''' 4. 回溯得到最优基因集
''' </summary>
Public Class DynamicProgramming

    ''' <summary>最小基因间距（同链相邻基因之间的最小间隔bp数）</summary>
    Private Const MinGeneSpacing As Integer = 1

    ''' <summary>
    ''' 执行动态规划，选择最优基因组合
    ''' </summary>
    Public Shared Function SelectGenes(orfs As List(Of CandidateOrf)) As List(Of CandidateOrf)
        If orfs.Count = 0 Then Return New List(Of CandidateOrf)()

        ' 过滤掉得分过低的ORF
        Dim validOrfs = orfs.Where(Function(o) o.TotalScore > 0 AndAlso o.Length >= 90).ToList()
        If validOrfs.Count = 0 Then Return New List(Of CandidateOrf)()

        ' 按终止位置排序
        validOrfs = validOrfs.OrderBy(Function(o) o.[End]).ThenBy(Function(o) o.Start).ToList()

        ' 分链处理：正向链和反向链分别做DP
        Dim fwdOrfs = validOrfs.Where(Function(o) o.Strand = "+"c).ToList()
        Dim revOrfs = validOrfs.Where(Function(o) o.Strand = "-"c).ToList()

        Dim selectedFwd = DpSelect(fwdOrfs)
        Dim selectedRev = DpSelect(revOrfs)

        ' 合并结果
        Dim result = selectedFwd.Concat(selectedRev).OrderBy(Function(o) o.Start).ToList()

        ' 处理跨链重叠（可选：允许反向链基因重叠）
        ' Prodigal允许反向链基因重叠，所以这里直接合并

        Return result
    End Function

    ''' <summary>
    ''' 对单条链的ORF执行加权区间调度DP
    ''' </summary>
    Private Shared Function DpSelect(orfs As List(Of CandidateOrf)) As List(Of CandidateOrf)
        If orfs.Count = 0 Then Return New List(Of CandidateOrf)()

        Dim n = orfs.Count

        ' 按终止位置排序（已排序）
        For i As Integer = 0 To n - 1
            orfs(i).SortIndex = i
        Next

        ' 预计算每个ORF的不重叠前驱索引（二分查找）
        Dim prevIdx(n - 1) As Integer
        For i As Integer = 0 To n - 1
            prevIdx(i) = FindLastNonOverlapping(orfs, i)
        Next

        ' DP数组
        Dim dp(n) As Double
        Dim selected(n - 1) As Boolean

        dp(0) = 0  ' dp[0] = 不选任何ORF的得分 = 0

        For i As Integer = 0 To n - 1
            ' 选项1：不选ORF i
            Dim skipScore = If(i > 0, dp(i), 0)

            ' 选项2：选ORF i
            Dim takeScore = orfs(i).TotalScore
            If prevIdx(i) >= 0 Then
                takeScore += dp(prevIdx(i) + 1)  ' +1因为dp是1-indexed
            End If

            If takeScore > skipScore Then
                dp(i + 1) = takeScore
                selected(i) = True
            Else
                dp(i + 1) = skipScore
                selected(i) = False
            End If
        Next

        ' 回溯
        Dim result As New List(Of CandidateOrf)()
        Dim idx = n - 1
        While idx >= 0
            If selected(idx) Then
                orfs(idx).Selected = True
                result.Add(orfs(idx))
                idx = prevIdx(idx)
            Else
                idx -= 1
            End If
        End While

        result.Reverse()
        Return result
    End Function

    ''' <summary>
    ''' 二分查找：找到不与ORF i重叠的最后一个ORF的索引
    ''' </summary>
    Private Shared Function FindLastNonOverlapping(orfs As List(Of CandidateOrf), i As Integer) As Integer
        Dim targetEnd = orfs(i).Start - MinGeneSpacing
        Dim lo As Integer = 0
        Dim hi As Integer = i - 1
        Dim result As Integer = -1

        While lo <= hi
            Dim mid = (lo + hi) \ 2
            If orfs(mid).[End] < targetEnd Then
                result = mid
                lo = mid + 1
            Else
                hi = mid - 1
            End If
        End While

        Return result
    End Function

End Class
