Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX

    ''' <summary>
    ''' Extensions API for blastX output table
    ''' </summary>
    Public Module Extensions

        ''' <summary>
        ''' Get the top best hit from the blastx output table result
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        <Extension> Public Function TopBest(result As IEnumerable(Of BlastXHit)) As BlastXHit()
            ' 首先按照hits分组，然后再在hits分组中按照query分组，
            ' 计算出得分最高的query作为top best
            Dim hitsGroup = result.GroupBy(Function(hit) hit.HitName).ToArray
            ' 经过group之后，现在hit是唯一的了
            ' 但是query并不是唯一的，一个hits对应一个query，但是一个query却对应多个hits
            Dim top = hitsGroup _
                .Select(Function(hits)
                            Return hits _
                                .GroupBy(Function(q) q.QueryName) _
                                .OrderByDescending(Function(query)
                                                       Return query _
                                                           .Select(Function(q) q.SBHScore * q.hit_length) _
                                                           .Sum
                                                   End Function) _
                                .First
                        End Function) _
                .Select(Function(g)
                            Return New NamedValue(Of BlastXHit()) With {
                                .Name = g.Key,
                                .Value = g.ToArray
                            }
                        End Function) _
                .ToArray

            ' 将query得到的frame进行合并
            Dim bestOne As BlastXHit() = top _
                .Select(Function(q)
                            Return q.Value _
                                .OrderByDescending(Function(hit) hit.coverage) _
                                .First
                        End Function) _
                .ToArray

            ' 对query做group，得到唯一的query
            bestOne = bestOne _
                .GroupBy(Function(q) q.QueryName) _
                .Select(Function(group)
                            Return group.OrderByDescending(Function(hit) hit.SBHScore).First
                        End Function) _
                .ToArray

            Return bestOne
        End Function
    End Module
End Namespace