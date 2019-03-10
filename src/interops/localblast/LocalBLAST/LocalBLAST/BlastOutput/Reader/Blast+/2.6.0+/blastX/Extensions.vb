#Region "Microsoft.VisualBasic::61fd9ca48eb6fe37474306e4b7710688, LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\BlastX\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Module Extensions
    ' 
    '         Function: TopBest
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
