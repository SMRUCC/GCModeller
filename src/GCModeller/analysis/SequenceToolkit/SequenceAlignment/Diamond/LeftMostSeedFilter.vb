' 最左种子过滤 (Left-Most Seed Filter)
'
' 由于双索引会独立地多次发现同一比对(不同形状、甚至同一形状的相邻起始位置),
' DIAMOND 检查当前命中左侧是否存在更早的种子命中(含之前已处理形状的命中),
' 若有则判定为冗余并丢弃,避免临时命中集合爆炸。
'
' 判定逻辑:对于同一个 (subjectId, 查询坐标) 组合,若已记录一条起始位置更靠左、
' 且落在该命中左侧容差范围内的种子,则当前命中视为冗余。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 最左种子去冗余过滤器。跨形状累积已见过的种子命中,丢弃左侧已覆盖的冗余命中。
    ''' </summary>
    Public Class LeftMostSeedFilter

        ' key: subjectId; value: 每条参考序列上已记录的最左查询位置集合
        Private ReadOnly seen As New Dictionary(Of Integer, HashSet(Of Integer))

        ''' <summary>
        ''' 判定一个种子命中是否应保留(非冗余)。
        ''' 若该 subject 上已存在一条查询坐标更靠左、且在容差范围内的命中,则丢弃。
        ''' </summary>
        ''' <param name="queryPos">种子在查询中的起始位置。</param>
        ''' <param name="subjectId">参考序列编号。</param>
        ''' <param name="tolerance">左侧容差(同一次比对的相邻种子距离上限)。</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Keep(queryPos As Integer, subjectId As Integer, Optional tolerance As Integer = 3) As Boolean
            Dim positions As HashSet(Of Integer) = Nothing

            If Not seen.TryGetValue(subjectId, positions) Then
                positions = New HashSet(Of Integer)
                seen(subjectId) = positions
            End If

            ' 检查左侧容差范围内是否已有更早的种子
            For p As Integer = queryPos - tolerance To queryPos - 1
                If p >= 0 AndAlso positions.Contains(p) Then
                    Return False
                End If
            Next

            positions.Add(queryPos)
            Return True
        End Function

        ''' <summary>清空跨形状累积状态(每条查询处理开始时调用)。</summary>
        Public Sub Reset()
            seen.Clear()
        End Sub
    End Class
End Namespace
