#Region "Microsoft.VisualBasic::296b7fd3876a4f31f85d02f182e820aa, localblast\PanGenome\UnionFind.vb"

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

    '   Total Lines: 84
    '    Code Lines: 51 (60.71%)
    ' Comment Lines: 23 (27.38%)
    '    - Xml Docs: 91.30%
    ' 
    '   Blank Lines: 10 (11.90%)
    '     File Size: 2.64 KB


    ' Class UnionFind
    ' 
    '     Function: Find, GetClusters
    ' 
    '     Sub: AddElement, Union
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 并查集辅助类，用于高效处理基因家族的聚类
''' </summary>
Public Class UnionFind

    ReadOnly parent As New Dictionary(Of String, String)()
    ReadOnly rank As New Dictionary(Of String, Integer)()

    ''' <summary>
    ''' 添加元素
    ''' </summary>
    ''' <param name="element"></param>
    Public Sub AddElement(element As String)
        If Not parent.ContainsKey(element) Then
            parent.Add(element, element)
        End If
    End Sub

    ''' <summary>
    ''' 批量添加元素（用于一次性初始化上百万个基因节点）
    ''' </summary>
    ''' <param name="elements"></param>
    Public Sub AddElements(elements As IEnumerable(Of String))
        For Each element As String In elements
            If Not parent.ContainsKey(element) Then
                parent.Add(element, element)
            End If
        Next
    End Sub

    ''' <summary>
    ''' 查找根节点
    ''' </summary>
    ''' <param name="element"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这里使用迭代的方式做路径压缩，而不是递归：
    ''' 在处理上百万个基因的时候，合并链可能会非常深，递归版本有栈溢出的风险。
    ''' </remarks>
    Public Function Find(element As String) As String
        If Not parent.ContainsKey(element) Then
            Return Nothing
        End If

        ' 1. 先定位到根节点
        Dim root As String = element

        Do While parent(root) <> root
            root = parent(root)
        Loop

        ' 2. 路径压缩（迭代实现）
        Dim cur As String = element

        Do While parent(cur) <> root
            Dim nxt As String = parent(cur)
            parent(cur) = root
            cur = nxt
        Loop

        Return root
    End Function

    ''' <summary>
    ''' 将一个直系同源分组（例如cd-hit的一个cluster）内的所有基因直接合并为同一个基因家族
    ''' </summary>
    ''' <param name="genes">同一个分组内的基因ID列表</param>
    ''' <remarks>
    ''' 一个包含k个基因的分组只需要 k-1 次合并操作即可完成聚类，
    ''' 不需要先生成 O(k^2) 个两两配对关系再逐个合并，可以节省大量的内存与计算时间。
    ''' 
    ''' 注意：只有已经通过 <see cref="AddElement(String)"/> 注册过的基因才会参与合并，
    ''' 未知的基因ID会被忽略掉（与BBH路径的行为保持一致）。
    ''' </remarks>
    Public Sub UnionRange(genes As IEnumerable(Of String))
        Dim refer As String = Nothing

        For Each gene As String In genes
            If Not parent.ContainsKey(gene) Then
                Continue For
            End If
            If refer Is Nothing Then
                refer = gene
            Else
                Call Union(refer, gene)
            End If
        Next
    End Sub

    ''' <summary>
    ''' 将一个直系同源分组（例如cd-hit的一个cluster）内的所有基因合并为同一个基因家族
    ''' </summary>
    ''' <param name="genes">分组内的基因ID，顺序与原始的两两配对枚举顺序保持一致</param>
    ''' <param name="sources">
    ''' 每一个基因所属的比对来源（一般是replicon编号）。只有来源不同的基因之间才会被合并，
    ''' 同一个来源之内的基因（旁系同源）不会被合并到同一个家族之中，
    ''' 这样子可以和 <see cref="OrthoGroupsHelper.BuildHomologyRelations(Of T)"/> 的行为保持一致。
    ''' </param>
    ''' <remarks>
    ''' 一个包含k个基因的分组只需要 k-1 次合并操作即可完成聚类，
    ''' 不需要先生成 O(k^2) 个两两配对关系再逐个合并。
    ''' </remarks>
    Public Sub UnionRange(genes As String(), Optional sources As String() = Nothing)
        If genes Is Nothing OrElse genes.Length < 2 Then
            Return
        End If

        If sources Is Nothing OrElse sources.Length <> genes.Length Then
            Call UnionRange(DirectCast(genes, IEnumerable(Of String)))
            Return
        End If

        Dim i As Integer
        Dim i0 As Integer = -1
        Dim j0 As Integer = -1

        ' 第一个参与合并的基因（必须是已经注册过的基因）
        For i = 0 To genes.Length - 1
            If parent.ContainsKey(genes(i)) Then
                i0 = i
                Exit For
            End If
        Next
        If i0 < 0 Then
            Return
        End If

        ' 家族ID（并查集的根）取枚举顺序之中第一个与首个基因来源不同的基因
        For i = i0 + 1 To genes.Length - 1
            If parent.ContainsKey(genes(i)) AndAlso sources(i) <> sources(i0) Then
                j0 = i
                Exit For
            End If
        Next
        If j0 < 0 Then
            ' 分组内的所有基因都来源于同一个基因组，不存在跨基因组的同源关系
            Return
        End If

        Dim root As String = genes(j0)

        ' 先让root赢一次把它的rank提升上去，之后的所有基因都会被挂载到它的下面
        Call Union(genes(i0), root)

        For i = 0 To genes.Length - 1
            If i <> j0 Then
                Call Union(root, genes(i))
            End If
        Next
    End Sub

    ''' <summary>
    ''' 合并两个集合
    ''' </summary>
    ''' <param name="referID">推荐使用参考基因ID，这样子比较容易生成有意义的家族ID</param>
    ''' <param name="geneID">待分析的基因组内的基因ID</param>
    Public Sub Union(referID As String, geneID As String)
        Dim root1 = Find(referID)
        Dim root2 = Find(geneID)

        If root1 Is Nothing OrElse root2 Is Nothing OrElse root1 = root2 Then
            Return
        Else
            If Not rank.ContainsKey(root1) Then
                rank.Add(root1, 0)
            End If
            If Not rank.ContainsKey(root2) Then
                rank.Add(root2, 0)
            End If
        End If

        If rank(root1) < rank(root2) Then
            parent(root1) = root2
        ElseIf rank(root1) > rank(root2) Then
            parent(root2) = root1
        Else
            parent(root1) = root2
            rank(root2) += 1
        End If
    End Sub

    ''' <summary>
    ''' 提取聚类结果
    ''' </summary>
    ''' <returns>构建家族映射</returns>
    Public Function GetClusters() As Dictionary(Of String, List(Of String))
        Dim clusters As New Dictionary(Of String, List(Of String))()

        ' 遍历 parent.Keys 中的所有基因
        For Each gene As String In parent.Keys
            Dim root As String = Find(gene)  ' 找到根节点
            If Not clusters.ContainsKey(root) Then
                clusters.Add(root, New List(Of String)())
            End If
            clusters(root).Add(gene)
        Next

        Return clusters
    End Function
End Class
