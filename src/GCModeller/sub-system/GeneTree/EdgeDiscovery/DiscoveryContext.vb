Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 结构发现阶段在全部靶基因之间共享的上下文：筛选细胞与已经标准化的候选向量。
''' </summary>
''' <remarks>
''' 候选向量只在训练细胞上标准化一次，随后每个靶基因的残差都在同一批细胞上计算，
''' 因此相关系数的分母与尺度完全一致，可以直接比较。
''' </remarks>
Public Class DiscoveryContext

    ''' <summary>
    ''' 用于计算残差相关的细胞位置（相对训练细胞数组 <c>cells</c> 的下标）
    ''' </summary>
    Public Property Cells As Integer()

    ''' <summary>已经标准化的候选调控者向量</summary>
    Public Property Candidates As ResidualScreening.CandidateVector()

    ''' <summary>
    ''' 由配置与表达矩阵创建发现上下文
    ''' </summary>
    ''' <param name="opt">训练配置（必须已经开启 <see cref="EdgeDiscoveryMode.ResidualTwoStage"/>）</param>
    ''' <param name="rows">表达矩阵的行</param>
    ''' <param name="rowIndex">基因编号 → 行号</param>
    ''' <param name="cells">参与训练的细胞列下标</param>
    ''' <returns>没有任何可用候选时返回 <c>Nothing</c></returns>
    Public Shared Function Create(opt As GeneTreeConfig,
                                  rows As DataFrameRow(),
                                  rowIndex As Dictionary(Of String, Integer),
                                  cells As Integer()) As DiscoveryContext

        If opt Is Nothing OrElse Not opt.IsDiscoveryEnabled Then
            Return Nothing
        End If

        Dim size As Integer = If(opt.DiscoverySampleSize > 0,
                                 std.Min(opt.DiscoverySampleSize, cells.Length),
                                 cells.Length)
        Dim positions As Integer() = subsample(cells.Length, size)
        Dim absolute As Integer() = positions.Select(Function(p) cells(p)).ToArray
        Dim candidates As ResidualScreening.CandidateVector() =
            ResidualScreening.PrepareCandidates(opt.DiscoveryCandidates, rows, rowIndex, absolute)

        If candidates.Length = 0 Then
            Return Nothing
        End If

        Return New DiscoveryContext With {
            .Cells = positions,
            .Candidates = candidates
        }
    End Function

    ''' <summary>
    ''' 在 <c>[0, count)</c> 上等距抽取 <paramref name="size"/> 个位置（确定性，便于复现）
    ''' </summary>
    ''' <param name="count">总数</param>
    ''' <param name="size">需要的数量</param>
    ''' <returns></returns>
    Public Shared Function subsample(count As Integer, size As Integer) As Integer()
        If count <= 0 Then
            Return New Integer() {}
        End If
        If size <= 0 OrElse size >= count Then
            Return Enumerable.Range(0, count).ToArray
        End If

        Dim result As Integer() = New Integer(size - 1) {}
        Dim stride As Double = CDbl(count) / size

        For i As Integer = 0 To size - 1
            Dim pos As Integer = CInt(std.Floor(i * stride))

            If pos >= count Then
                pos = count - 1
            End If

            result(i) = pos
        Next

        Return result
    End Function

End Class
