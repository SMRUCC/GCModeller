#Region "Microsoft.VisualBasic::df2fd10d42575d833c934936fd67c006, analysis\ProteinTools\ProteinMatrix\Linclust\GreedyCover.vb"

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

    '   Total Lines: 99
    '    Code Lines: 62 (62.63%)
    ' Comment Lines: 22 (22.22%)
    '    - Xml Docs: 27.27%
    ' 
    '   Blank Lines: 15 (15.15%)
    '     File Size: 4.49 KB


    '     Module GreedyCover
    ' 
    '         Function: Cluster
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' Linclust 阶段五:贪心集合覆盖聚类
'
' 输入:一张有向边图(成员 -> 中心)。
' 1. 读入所有中心 -> 成员有向边,并补上反向边(使关系对称)。
' 2. 把所有序列按长度降序排序。
' 3. 循环:取顶部序列 s,把所有与 s 有边相连且仍在列表中的序列一并移除,构成一个新簇,s 为簇代表。
' 4. 重复直到列表为空。

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm

Namespace Linclust

    Public Module GreedyCover

        ''' <summary>
        ''' 阶段五贪心集合覆盖。
        ''' </summary>
        ''' <param name="edges">有向边列表(From=成员, To=中心, Score=成员相对中心的比对 score)</param>
        ''' <param name="seqLengths">序列 ID -> 序列长度</param>
        ''' <returns>聚类结果(每簇代表为最长成员;每簇 memberScores 承载成员比对 score)</returns>
        Public Function Cluster(edges As List(Of (From As Integer, [To] As Integer, Score As Double)), seqLengths As Dictionary(Of Integer, Integer)) As List(Of Cluster)
            ' 构建无向邻接表(补全反向边)
            Dim adj As New Dictionary(Of Integer, HashSet(Of Integer))
            ' 成员 -> 相对代表的 score 查表(从有向边收集,key=(成员,中心))
            Dim edgeScores As New Dictionary(Of (Integer, Integer), Double)

            Dim addEdge = Sub(a As Integer, b As Integer)
                              If Not adj.ContainsKey(a) Then
                                  adj(a) = New HashSet(Of Integer)
                              End If
                              adj(a).Add(b)
                          End Sub

            For Each e In edges
                addEdge(e.From, e.[To])
                addEdge(e.[To], e.From)
                ' 记录 (成员,中心) -> score,供下文构建 memberScores 时查找
                edgeScores((e.From, e.[To])) = e.Score
            Next

            ' 剩余待处理序列,按长度降序(长度相同按 ID 升序保证确定性)
            Dim remaining As New List(Of Integer)(seqLengths.Keys)
            remaining.Sort(Function(x, y)
                               Dim c = seqLengths(y).CompareTo(seqLengths(x))  ' 长度降序
                               If c <> 0 Then
                                   Return c
                               End If
                               Return x.CompareTo(y)
                           End Function)

            Dim inList As New HashSet(Of Integer)(remaining)
            Dim clusters As New List(Of Cluster)

            For Each s As Integer In TqdmWrapper.Wrap(remaining)
                If Not inList.Contains(s) Then
                    Continue For
                End If

                ' s 即新簇代表;收集所有与 s 有边且仍在列表中的序列
                Dim members As New List(Of Integer) From {s}
                inList.Remove(s)

                ' 成员序列 ID -> 相对代表 s 的比对 score
                Dim scores As New Dictionary(Of Integer, Double)

                If adj.ContainsKey(s) Then
                    For Each nb In adj(s)
                        If inList.Contains(nb) Then
                            members.Add(nb)
                            inList.Remove(nb)
                            ' 从边查表取得该成员相对代表 s 的 score(若缺失则补 0)
                            If edgeScores.ContainsKey((nb, s)) Then
                                scores(nb) = edgeScores((nb, s))
                            Else
                                scores(nb) = 0.0
                            End If
                        End If
                    Next
                End If

                ' 代表序列自身记为该簇内成员比对 score 的最大值
                ' (语义上代表其家族内最优同源强度;簇内无成员时记为 0)
                Dim reprScore As Double = 0.0
                If scores.Count > 0 Then
                    reprScore = scores.Values.Max
                End If
                scores(s) = reprScore

                clusters.Add(New Cluster With {
                    .representative = s,
                    .members = members,
                    .memberScores = scores
                })
            Next

            Return clusters
        End Function
    End Module
End Namespace

