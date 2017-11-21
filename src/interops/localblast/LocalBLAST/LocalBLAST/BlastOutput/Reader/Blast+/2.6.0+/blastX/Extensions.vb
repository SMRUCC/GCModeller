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

            Return bestOne
        End Function
    End Module
End Namespace