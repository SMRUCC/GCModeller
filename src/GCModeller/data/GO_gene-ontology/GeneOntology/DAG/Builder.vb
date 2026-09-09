#Region "Microsoft.VisualBasic::39fab25670061db69d6ce3c212709c0f, data\GO_gene-ontology\GeneOntology\DAG\Builder.vb"

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

    '   Total Lines: 137
    '    Code Lines: 111 (81.02%)
    ' Comment Lines: 6 (4.38%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 20 (14.60%)
    '     File Size: 5.18 KB


    '     Module Builder
    ' 
    '         Function: (+2 Overloads) BuildTree, ConstructNode, CreateClusterMembers, GetTermXrefs, TermXrefParser
    '                   UniqueNodes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace DAG

    Public Module Builder

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Friend Function UniqueNodes(cluster As KeyValuePair(Of String, List(Of TermNode))) As TermNode()
            Return cluster.Value _
                .GroupBy(Function(t) t.id) _
                .Select(Function(c)
                            Return c.First
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 安全的空值处理
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 与``SafeQuery``的区别在于：``SafeQuery``在遇到Nothing的时候会向
        ''' 控制台输出一条警告信息，而在``go.obo``之中有大量的词条并没有
        ''' ``is_a``/``relationship``/``xref``这些字段，使用SafeQuery会
        ''' 刷出数以十万计的警告信息，从而严重的拖慢构建的 speed
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function safeArray(Of T)(source As T()) As T()
            If source Is Nothing Then
                Return New T() {}
            Else
                Return source
            End If
        End Function

        ''' <summary>
        ''' 判断obo文本之中的逻辑值标记是否为真，例如``is_obsolete: true``
        ''' </summary>
        ''' <param name="value$"></param>
        ''' <returns></returns>
        Private Function isTrue(value As String) As Boolean
            If String.IsNullOrEmpty(value) Then
                Return False
            Else
                Return value.Trim.Equals("true", StringComparison.OrdinalIgnoreCase)
            End If
        End Function

        ''' <summary>
        ''' 由祖先闭包反查得到子孙索引
        ''' </summary>
        ''' <param name="tree"></param>
        ''' <param name="ancestors">
        ''' ``[term_id => all ancestor term_id]``，即<see cref="AncestorSets(Dictionary(Of String, String()))"/>的产物
        ''' </param>
        ''' <returns>
        ''' ``[term_id => all descendant nodes]``，不包含term自身
        ''' </returns>
        ''' <remarks>
        ''' 这个函数是<see cref="CreateClusterMembers(Graph)"/>的新的实现：
        ''' 只需要对祖先集合做一次反查聚合即可，复杂度为``O(N * A)``，
        ''' 而不像旧版本那样需要枚举出所有指数级数量的祖先路径。
        ''' </remarks>
        Public Function DescendantSets(tree As Dictionary(Of TermNode),
                                        ancestors As Dictionary(Of String, String())) As System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.List(Of TermNode))
            Dim descendants As New System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.List(Of TermNode))

            For Each node As TermNode In tree.Values
                Dim parents As String()

                If ancestors.ContainsKey(node.id) Then
                    parents = ancestors(node.id)
                Else
                    parents = {}
                End If

                For Each parent As String In parents
                    If Not descendants.ContainsKey(parent) Then
                        Call descendants.Add(parent, New System.Collections.Generic.List(Of TermNode))
                    End If

                    descendants(parent).Add(node)
                Next
            Next

            Return descendants
        End Function

        ''' <summary>
        ''' 计算出DAG图之中所有节点的祖先闭包集合
        ''' </summary>
        ''' <param name="parents">
        ''' ``[term_id => parent term_id()]``，即<see cref="ParentIndex(Dictionary(Of TermNode), OntologyRelations())"/>的产物
        ''' </param>
        ''' <returns>
        ''' ``[term_id => all ancestor term_id]``，不包含term自身，与拓扑顺序无关
        ''' </returns>
        ''' <remarks>
        ''' 由于GO是一个有向无环图，一个节点可能会有多个父节点，所以直接递归枚举路径
        ''' 会得到指数级数量的结果。在这里只关心**集合**而不关心路径，所以通过记忆化
        ''' 的DFS，让每一个节点的祖先集合只被计算一次。
        ''' </remarks>
        Public Function AncestorSets(parents As Dictionary(Of String, String())) As Dictionary(Of String, String())
            Dim result As New Dictionary(Of String, String())
            Dim visiting As New HashSet(Of String)

            For Each id As String In parents.Keys.ToArray
                Call walkAncestors(id, parents, result, visiting)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 记忆化的祖先闭包递归求解，<paramref name="visiting"/>用于防止因为数据错误
        ''' 而出现的环导致的无限递归
        ''' </summary>
        Private Function walkAncestors(id As String,
                                       parents As Dictionary(Of String, String()),
                                       result As Dictionary(Of String, String()),
                                       visiting As HashSet(Of String)) As String()

            If result.ContainsKey(id) Then
                Return result(id)
            End If
            If visiting.Contains(id) Then
                ' 出现了环，在这里中断递归
                Return {}
            End If
            If Not parents.ContainsKey(id) Then
                Return {}
            End If

            visiting.Add(id)

            Dim acc As New HashSet(Of String)

            For Each parent As String In parents(id)
                If String.IsNullOrEmpty(parent) OrElse Not parents.ContainsKey(parent) Then
                    ' 悬空的父节点引用，跳过
                    Continue For
                End If

                acc.Add(parent)

                For Each grandParent As String In walkAncestors(parent, parents, result, visiting)
                    acc.Add(grandParent)
                Next
            Next

            visiting.Remove(id)

            Dim all As String() = acc.ToArray

            result(id) = all

            Return all
        End Function

        ''' <summary>
        ''' 构建出``[term_id => parent term_id()]``的索引
        ''' </summary>
        ''' <param name="tree"></param>
        ''' <param name="relations">
        ''' 除了``is_a``之外，还需要参与计算的relationship关系类型列表，
        ''' 默认为<see cref="Graph.DefaultRelations"/>
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 注意：``has_part``/``regulates``之类的关系按照GO官方的约定不参与
        ''' 注释的传播，所以默认只使用``is_a``与``part_of``。
        ''' </remarks>
        <Extension>
        Public Function ParentIndex(tree As Dictionary(Of TermNode),
                                    Optional relations As OntologyRelations() = Nothing) As Dictionary(Of String, String())

            Dim allowRels As OntologyRelations() = relations

            If allowRels Is Nothing OrElse allowRels.Length = 0 Then
                allowRels = {OntologyRelations.part_of}
            End If

            Dim allow As New HashSet(Of OntologyRelations)(allowRels)
            Dim index As New Dictionary(Of String, String())

            For Each node As TermNode In tree.Values
                Dim list As New List(Of String)

                For Each rel As is_a In safeArray(node.is_a)
                    If String.IsNullOrEmpty(rel.term_id) OrElse Not tree.ContainsKey(rel.term_id) Then
                        Continue For
                    End If

                    list.Add(rel.term_id)
                Next

                If allow.Count > 0 Then
                    For Each rel As Relationship In safeArray(node.relationship)
                        If String.IsNullOrEmpty(rel.parent.Name) Then
                            Continue For
                        End If
                        If Not tree.ContainsKey(rel.parent.Name) Then
                            Continue For
                        End If
                        If Not allow.Contains(rel.type) Then
                            Continue For
                        End If

                        list.Add(rel.parent.Name)
                    Next
                End If

                index(node.id) = list.Distinct.ToArray
            Next

            Return index
        End Function

        ''' <summary>
        ''' 建立``[alt_id => primary term id]``映射表
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 旧的注释数据之中可能会使用已经被废弃掉的alt_id编号，
        ''' 通过这个映射表可以将其回填为最新的主编号
        ''' </remarks>
        <Extension>
        Public Function AltIdIndex(file As IEnumerable(Of Term)) As Dictionary(Of String, String)
            Dim index As New Dictionary(Of String, String)

            For Each term As Term In file.SafeQuery
                If term Is Nothing OrElse String.IsNullOrEmpty(term.id) Then
                    Continue For
                End If

                For Each alt As String In safeArray(term.alt_id)
                    If String.IsNullOrEmpty(alt) Then
                        Continue For
                    End If
                    If index.ContainsKey(alt) Then
                        Continue For
                    End If

                    index.Add(alt, term.id)
                Next
            Next

            Return index
        End Function

        ''' <summary>
        ''' 由祖先集合反查得到每一个GO词条的所有的子孙节点
        ''' </summary>
        ''' <param name="tree"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateClusterMembers(tree As Graph) As System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.List(Of TermNode))
            Return tree.DescendantTable(Nothing)
        End Function

        ''' <summary>
        ''' 从obo词条集合之中构建出DAG图的节点集合
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在这里会做如下的几项数据清洗工作：
        ''' 
        ''' 1. 跳过被标记为``is_obsolete``的废弃词条；
        ''' 2. 跳过重复编号的词条，避免<see cref="Dictionary(Of TermNode)"/>添加元素的时候抛出异常；
        ''' 3. 跳过在当前的词表之中不存在的父节点引用，避免产生悬空的``Nothing``引用；
        ''' 4. 将``relationship``关系也解析为节点引用，使得``part_of``之类的边也可以被遍历。
        ''' </remarks>
        <Extension>
        Public Function BuildTree(file As IEnumerable(Of Term)) As Dictionary(Of TermNode)
            Dim tree As New Dictionary(Of TermNode)

            Call VBDebugger.EchoLine("Parse the ontology lineage information and build DAG tree...")

            For Each term As Term In file.SafeQuery
                If term Is Nothing OrElse String.IsNullOrEmpty(term.id) Then
                    Continue For
                End If
                If isTrue(term.is_obsolete) Then
                    Continue For
                End If
                If tree.ContainsKey(term.id) Then
                    Continue For
                End If

                Call tree.Add(term.ConstructNode)
            Next

            ' 将文本形式的父子关系链接为节点对象的引用
            For Each node As TermNode In tree.Values.ToArray
                Dim is_aList As New List(Of is_a)

                For Each rel As is_a In safeArray(node.is_a)
                    If String.IsNullOrEmpty(rel.term_id) OrElse Not tree.ContainsKey(rel.term_id) Then
                        ' 父节点不存在，丢弃掉这个悬空的引用
                        Continue For
                    End If

                    rel.term = tree(rel.term_id)
                    is_aList.Add(rel)
                Next

                node.is_a = is_aList.ToArray

                Dim relList As New List(Of Relationship)

                For Each rel As Relationship In safeArray(node.relationship)
                    If String.IsNullOrEmpty(rel.parent.Name) Then
                        Continue For
                    End If
                    If Not tree.ContainsKey(rel.parent.Name) Then
                        Continue For
                    End If

                    rel.term = tree(rel.parent.Name)
                    relList.Add(rel)
                Next

                node.relationship = relList.ToArray
            Next

            Return tree
        End Function

        ''' <summary>
        ''' Creates a node in this DAG graph
        ''' </summary>
        ''' <param name="term"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ConstructNode(term As Term) As TermNode
            Dim is_a = safeArray(term.is_a) _
                .Select(Function(s) New is_a(s$)) _
                .ToArray
            Dim rels = safeArray(term.relationship) _
                .Select(Function(s) New Relationship(s$)) _
                .ToArray
            Dim synonym = safeArray(term.synonym) _
                .Select(Function(s) New synonym(s$)) _
                .ToArray
            Dim xrefValues = term.GetTermXrefs

            Return New TermNode With {
                .id = term.id,
                .is_a = is_a,
                .relationship = rels,
                .synonym = synonym,
                .xref = xrefValues,
                .namespace = term.namespace,
                .GO_term = term
            }
        End Function

        Public Function TermXrefParser(s As String) As NamedValue(Of String)
            Dim tokens$() = CommandLine.GetTokens(s$)
            Dim id$() = tokens(Scan0).Split(":"c)

            Return New NamedValue(Of String) With {
                .Name = id(Scan0),
                .Value = id.ElementAtOrDefault(1%),
                .Description = tokens.ElementAtOrDefault(1%)
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetTermXrefs(term As Term) As NamedValue(Of String)()
            Return safeArray(term.xref) _
                .Select(AddressOf TermXrefParser) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function BuildTree(path As String) As Dictionary(Of TermNode)
            Return GO_OBO.Open(path).BuildTree
        End Function
    End Module
End Namespace
