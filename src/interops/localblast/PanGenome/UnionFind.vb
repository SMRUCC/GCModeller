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
    ''' 查找根节点
    ''' </summary>
    ''' <param name="element"></param>
    ''' <returns></returns>
    Public Function Find(element As String) As String
        If Not parent.ContainsKey(element) Then
            Return Nothing
        End If

        ' 路径压缩
        If parent(element) <> element Then
            parent(element) = Find(parent(element))
        End If
        Return parent(element)
    End Function

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
