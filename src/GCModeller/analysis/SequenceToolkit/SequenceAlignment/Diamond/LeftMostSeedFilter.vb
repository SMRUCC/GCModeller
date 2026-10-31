#Region "Microsoft.VisualBasic::ad0bdd14e17ca3fbf2c8eb8aba364134, analysis\SequenceToolkit\SequenceAlignment\Diamond\LeftMostSeedFilter.vb"

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


    ' Code Statistics:

    '   Total Lines: 54
    '    Code Lines: 24 (44.44%)
    ' Comment Lines: 21 (38.89%)
    '    - Xml Docs: 52.38%
    ' 
    '   Blank Lines: 9 (16.67%)
    '     File Size: 2.44 KB


    '     Class LeftMostSeedFilter
    ' 
    '         Function: Keep
    ' 
    '         Sub: Reset
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
    ''' <remarks>
    ''' <para>
    ''' 重要(线程安全):本类型持有可变的累积状态(<see cref="seen"/>),
    ''' <b>绝不可</b>在多条查询之间共享或作为比对器的实例字段复用。
    ''' 历史上 <see cref="DiamondBlastp"/> 曾把它的单实例挂在字段上并让所有并行查询
    ''' 同时调用 <see cref="Reset"/> / <see cref="Keep"/>,造成数据竞争与跨查询状态污染,
    ''' 使同一输入的多次运行结果不一致。
    ''' </para>
    ''' <para>
    ''' 当前多查询流水线(<see cref="DiamondScanner"/>)已经不再使用本过滤器:
    ''' 候选按 (查询, 参考) 唯一化并按无空位得分取优,等价且无需累积状态。
    ''' 保留该类型仅为兼容旧的单查询调用路径,使用时请为每条查询单独 new 一个实例。
    ''' </para>
    ''' </remarks>
    Public Class LeftMostSeedFilter

        ''' <summary>
        ''' key: subjectId; 
        ''' value: 每条参考序列上已记录的最左查询位置集合
        ''' </summary>
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

