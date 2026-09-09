#Region "Microsoft.VisualBasic::f403540dcffef174e9ab020702e419b7, data\GO_gene-ontology\GeneOntology\DAG\Graph.vb"

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

    '   Total Lines: 232
    '    Code Lines: 141 (60.78%)
    ' Comment Lines: 55 (23.71%)
    '    - Xml Docs: 98.18%
    ' 
    '   Blank Lines: 36 (15.52%)
    '     File Size: 9.57 KB


    '     Class Graph
    ' 
    '         Properties: header
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: (+2 Overloads) Family, GetClusterMembers, GetDAG, ToString
    '         Structure InheritsChain
    ' 
    '             Properties: [Namespace], Family, Top
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: Level, Strip, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models

Namespace DAG

    ''' <summary>
    ''' GO DAG graph
    ''' </summary>
    ''' <remarks>
    ''' ###### 关于祖先/子孙索引的算法说明
    ''' 
    ''' GO 的本体是一个有向无环图(DAG)，一个子条目可能同时拥有多个父条目，
    ''' 所以从任意一个条目出发向上的路径数量是**指数级**的。旧版本的
    ''' <see cref="Family(String)"/> 会枚举出所有的祖先路径，而
    ''' <see cref="Builder.CreateClusterMembers(Graph)"/> 又会对每一个条目都调用一次
    ''' 该函数，导致在完整的 ``go.obo``(约5万条目)上面构建时会直接卡死或者内存溢出。
    ''' 
    ''' 这里改为：
    ''' 
    ''' 1. 通过记忆化(memoization)的DFS计算出每一个条目的**祖先集合**(去重，不带路径信息)，
    '''    复杂度由指数级降低为 ``O(N * A)``；
    ''' 2. 子孙索引由祖先集合**单次反查聚合**得到，并且是惰性构建的，即只有真的
    '''    需要用到的时候才会去计算。
    ''' </remarks>
    Public Class Graph

        Friend ReadOnly DAG As Dictionary(Of TermNode)
        ''' <summary>
        ''' ``[term_id => parent term_id list]``，默认的关系类型索引
        ''' </summary>
        Friend ReadOnly parents As Dictionary(Of String, String())
        ''' <summary>
        ''' ``[alt_id => primary term id]``
        ''' </summary>
        Friend ReadOnly altIds As Dictionary(Of String, String)

        Private ReadOnly file$
        Private ReadOnly _header As header

        Private ReadOnly parentIndexCache As New Dictionary(Of String, Dictionary(Of String, String()))
        Private ReadOnly ancestorCache As New Dictionary(Of String, Dictionary(Of String, String()))
        Private ReadOnly descendantCache As New Dictionary(Of String, Dictionary(Of String, List(Of TermNode)))

        ''' <summary>
        ''' 在进行GO注释传播的时候所默认使用的关系类型
        ''' </summary>
        ''' <remarks>
        ''' ``is_a`` 关系总是会被包含在传播路径之中，而按照GO官方的注释传播约定，
        ''' ``part_of`` 关系也会参与传播，但是 ``regulates``/``has_part`` 等关系
        ''' 不参与传播。
        ''' </remarks>
        Public Shared ReadOnly DefaultRelations As OntologyRelations() = {OntologyRelations.part_of}

        Public ReadOnly Property header As header
            Get
                Return _header
            End Get
        End Property

        ''' <summary>
        ''' Creates GO DAG graph from ``go.obo`` file.
        ''' </summary>
        ''' <param name="path">File path of the GO database: ``go.obo``</param>
        Sub New(path$)
            Call Me.New(GO_OBO.LoadDocument(path$), trace:=path)
        End Sub

        ''' <summary>
        ''' Build DAG graph from a parsed <see cref="GO_OBO"/> database model.
        ''' </summary>
        ''' <param name="obo"></param>
        Sub New(obo As GO_OBO, <CallerMemberName> Optional trace$ = Nothing)
            Call Me.New(obo.terms, header:=obo.headers, trace:=trace)
        End Sub

        ''' <summary>
        ''' Or build DAG graph tree from a specific GO_term collection <paramref name="terms"/>
        ''' </summary>
        ''' <param name="terms"></param>
        Sub New(terms As IEnumerable(Of Term),
                Optional header As header = Nothing,
                <CallerMemberName> Optional trace$ = Nothing)

            Dim array As Term() = terms.SafeQuery.ToArray

            DAG = Builder.BuildTree(array)
            altIds = Builder.AltIdIndex(array)
            parents = Builder.ParentIndex(DAG, Nothing)
            parentIndexCache(RelationKey(Nothing)) = parents

            _header = header
            file = trace
        End Sub

        Private Shared Function RelationKey(relations As OntologyRelations()) As String
            If relations Is Nothing OrElse relations.Length = 0 Then
                Return "*"
            Else
                Return relations _
                    .Select(Function(r) CInt(r)) _
                    .OrderBy(Function(i) i) _
                    .Distinct _
                    .Select(Function(i) CStr(i)) _
                    .JoinBy(",")
            End If
        End Function

        ''' <summary>
        ''' get term node by its <see cref="TermNode.id"/>
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns>
        ''' returns nothing if the given term <paramref name="id"/> is not 
        ''' exists in current DAG graph.
        ''' </returns>
        Public Function GetTerm(id As String) As TermNode
            If String.IsNullOrEmpty(id) OrElse Not DAG.ContainsKey(id) Then
                Return Nothing
            Else
                Return DAG(id)
            End If
        End Function

        ''' <summary>
        ''' 将<paramref name="id"/>可能为alt_id的编号转换为主编号
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns>
        ''' 如果<paramref name="id"/>既不是主编号也不是alt_id，则原样返回
        ''' </returns>
        Public Function GetTermId(id As String) As String
            If String.IsNullOrEmpty(id) Then
                Return id
            ElseIf DAG.ContainsKey(id) Then
                Return id
            ElseIf altIds.ContainsKey(id) Then
                Return altIds(id)
            Else
                Return id
            End If
        End Function

        Public Function Contains(id As String) As Boolean
            Return Not String.IsNullOrEmpty(id) AndAlso DAG.ContainsKey(id)
        End Function

        ''' <summary>
        ''' 获取得到当前的DAG图之中的所有的节点
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Terms As IEnumerable(Of TermNode)
            Get
                Return DAG.Values
            End Get
        End Property

        ''' <summary>
        ''' 获取得到指定关系类型之下的``[term_id => parent term_id()]``索引
        ''' </summary>
        ''' <param name="relations">
        ''' 除了``is_a``之外还需要参与计算的relationship关系类型列表，
        ''' 默认为<see cref="DefaultRelations"/>
        ''' </param>
        ''' <returns></returns>
        Private Function ParentTable(relations As OntologyRelations()) As Dictionary(Of String, String())
            Dim key$ = RelationKey(relations)

            If Not parentIndexCache.ContainsKey(key) Then
                parentIndexCache(key) = Builder.ParentIndex(DAG, relations)
            End If

            Return parentIndexCache(key)
        End Function

        ''' <summary>
        ''' 记忆化的祖先闭包表：``[term_id => all ancestor term_id]``(不包含自身)
        ''' </summary>
        ''' <param name="relations"></param>
        ''' <returns></returns>
        Friend Function AncestorTable(relations As OntologyRelations()) As Dictionary(Of String, String())
            Dim key$ = RelationKey(relations)

            If Not ancestorCache.ContainsKey(key) Then
                ancestorCache(key) = Builder.AncestorSets(ParentTable(relations))
            End If

            Return ancestorCache(key)
        End Function

        ''' <summary>
        ''' 由祖先闭包反查得到的子孙索引：``[term_id => all descendant nodes]``(不包含自身)
        ''' </summary>
        ''' <param name="relations"></param>
        ''' <returns></returns>
        Friend Function DescendantTable(relations As OntologyRelations()) As Dictionary(Of String, List(Of TermNode))
            Dim key$ = RelationKey(relations)

            If Not descendantCache.ContainsKey(key) Then
                descendantCache(key) = Builder.DescendantSets(DAG, AncestorTable(relations))
            End If

            Return descendantCache(key)
        End Function

        ''' <summary>
        ''' 获取得到指定的GO词条的所有祖先词条编号(不包含自身)
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="includeSelf">是否将<paramref name="id"/>自身也包含进结果集合之中</param>
        ''' <param name="relations">除了``is_a``之外还需要参与计算的relationship关系类型</param>
        ''' <returns></returns>
        Public Function GetAncestors(id As String,
                                     Optional includeSelf As Boolean = False,
                                     Optional relations As OntologyRelations() = Nothing) As String()
            id = GetTermId(id)

            Dim table = AncestorTable(relations)
            Dim ancestors As String() = If(table.ContainsKey(id), table(id), {})

            If Not includeSelf Then
                Return ancestors
            End If

            Dim list As New List(Of String)(ancestors)
            list.Add(id)

            Return list.ToArray
        End Function

        ''' <summary>
        ''' 获取得到指定的GO词条的所有子孙节点(不包含自身)
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="includeSelf">是否将<paramref name="id"/>自身也包含进结果集合之中</param>
        ''' <param name="relations">除了``is_a``之外还需要参与计算的relationship关系类型</param>
        ''' <returns></returns>
        Public Function GetDescendants(id As String,
                                       Optional includeSelf As Boolean = False,
                                       Optional relations As OntologyRelations() = Nothing) As TermNode()
            id = GetTermId(id)

            Dim table = DescendantTable(relations)
            Dim list As New List(Of TermNode)

            If table.ContainsKey(id) Then
                list.AddRange(table(id))
            End If

            If includeSelf Then
                Dim self As TermNode = GetTerm(id)

                If Not self Is Nothing Then
                    list.Add(self)
                End If
            End If

            Return list.ToArray
        End Function

        ''' <summary>
        ''' 判断<paramref name="ancestor"/>是否为<paramref name="id"/>的祖先词条
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="ancestor"></param>
        ''' <param name="relations"></param>
        ''' <returns></returns>
        Public Function IsAncestorOf(id As String, ancestor As String, Optional relations As OntologyRelations() = Nothing) As Boolean
            id = GetTermId(id)
            ancestor = GetTermId(ancestor)

            Dim table = AncestorTable(relations)

            If Not table.ContainsKey(id) Then
                Return False
            Else
                Return Array.IndexOf(table(id), ancestor) > -1
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetDAG() As Dictionary(Of String, TermNode)
            Return New Dictionary(Of String, TermNode)(DAG)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return file.ToFileURL
        End Function

        ''' <summary>
        ''' These terms describe a component of a cell that is part of a larger object, such as an anatomical structure 
        ''' (e.g. rough endoplasmic reticulum or nucleus) or a gene product group (e.g. ribosome, proteasome or a protein dimer).
        ''' </summary>
        Const cellular_component$ = NameOf(cellular_component)
        ''' <summary>
        ''' A biological process term describes a series of events accomplished by one or more organized assemblies of molecular functions. 
        ''' Examples of broad biological process terms are "cellular physiological process" or "signal transduction". Examples of more 
        ''' specific terms are "pyrimidine metabolic process" or "alpha-glucoside transport". The general rule to assist in distinguishing 
        ''' between a biological process and a molecular function is that a process must have more than one distinct steps.
        ''' A biological process Is Not equivalent To a pathway. At present, the GO does Not Try To represent the dynamics Or dependencies 
        ''' that would be required To fully describe a pathway.
        ''' </summary>
        Const biological_process$ = NameOf(biological_process)
        ''' <summary>
        ''' Molecular function terms describes activities that occur at the molecular level, such as "catalytic activity" or "binding activity". 
        ''' GO molecular function terms represent activities rather than the entities (molecules or complexes) that perform the actions, 
        ''' and do not specify where, when, or in what context the action takes place. Molecular functions generally correspond to activities 
        ''' that can be performed by individual gene products, but some activities are performed by assembled complexes of gene products. 
        ''' Examples of broad functional terms are "catalytic activity" and "transporter activity"; examples of narrower functional terms are 
        ''' "adenylate cyclase activity" or "Toll receptor binding".
        ''' It Is easy To confuse a gene product name With its molecular Function; For that reason GO molecular functions are often appended 
        ''' With the word "activity".
        ''' </summary>
        Const molecular_function$ = NameOf(molecular_function)

        ''' <summary>
        ''' 三大namespace根节点的名称
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly RootNames As String() = {biological_process, molecular_function, cellular_component}

        ''' <summary>
        ''' 获取得到当前节点基于``is_a``关系的直接父节点(已经过滤掉了不存在的悬空引用)
        ''' </summary>
        ''' <param name="term"></param>
        ''' <returns></returns>
        Private Function isAParents(term As TermNode) As String()
            If term Is Nothing OrElse term.is_a.IsNullOrEmpty Then
                Return {}
            End If

            Dim list As New List(Of String)

            For Each rel As is_a In term.is_a
                If String.IsNullOrEmpty(rel.term_id) Then
                    Continue For
                End If
                If Not DAG.ContainsKey(rel.term_id) Then
                    ' 父节点在当前的DAG图之中不存在，跳过这个悬空的引用
                    Continue For
                End If

                list.Add(rel.term_id)
            Next

            Return list.ToArray
        End Function

        ''' <summary>
        ''' 向上查找到<paramref name="root"/>这个根节点为止的继承链
        ''' </summary>
        ''' <param name="id"><see cref="Term.id"/></param>
        ''' <param name="root">
        ''' 根节点的<see cref="Term.name"/>，一般为三大namespace的名称：
        ''' ``biological_process``/``molecular_function``/``cellular_component``
        ''' </param>
        ''' <returns>
        ''' 每一条链的<see cref="InheritsChain.Route"/>都是**从自身开始**向上一直到
        ''' ``root``的直接子节点为止(不包含``root``自身)。如果<paramref name="id"/>就是
        ''' ``root``或者``root``不在其祖先链上面，则返回空集合。
        ''' </returns>
        Public Function Family(id As String, root As String) As IEnumerable(Of InheritsChain)
            Dim term As TermNode = DAG(id)

            If term Is Nothing OrElse term.GO_term Is Nothing Then
                Return {}
            End If
            If term.GO_term.name = root Then
                ' 自身就是根节点
                Return {}
            End If

            Dim routes As New List(Of InheritsChain)

            Call familyToRoot(id, root, New HashSet(Of String), routes)

            Return routes
        End Function

        ''' <summary>
        ''' 构建从``id``一直向上到``root``的直接子节点为止的所有路径
        ''' </summary>
        Private Sub familyToRoot(id As String, root As String, guard As HashSet(Of String), routes As List(Of InheritsChain))
            If guard.Contains(id) Then
                ' 出现了环，中断递归
                Return
            End If

            Dim term As TermNode = DAG(id)

            If term Is Nothing OrElse term.GO_term Is Nothing Then
                Return
            End If

            Dim parents As String() = isAParents(term)

            If parents.Length = 0 Then
                ' 已经到达了顶层，但是中途没有遇到目标root节点
                Return
            End If

            guard.Add(id)

            For Each parentId As String In parents
                Dim parent As TermNode = DAG(parentId)

                If parent Is Nothing OrElse parent.GO_term Is Nothing Then
                    Continue For
                End If

                If parent.GO_term.name = root Then
                    routes.Add(New InheritsChain With {
                        .Route = New List(Of TermNode) From {term}
                    })
                Else
                    Dim n As Integer = routes.Count

                    Call familyToRoot(parentId, root, guard, routes)

                    For i As Integer = n To routes.Count - 1
                        routes(i).Route.Insert(0, term)
                    Next
                End If
            Next

            guard.Remove(id)
        End Sub

        ''' <summary>
        ''' Create family tree of the GO terms based on the ``is_a`` relationship.
        ''' </summary>
        ''' <param name="id"><see cref="Term.id"/></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个函数是往顶层查找直到查找到三大namespace为止
        ''' 
        ''' 注意：由于GO是一个有向无环图，所以从这里返回的继承链的数量可能是**指数级**的，
        ''' 只需要祖先/子孙的集合信息的时候，请优先使用<see cref="GetAncestors(String, Boolean, OntologyRelations())"/>
        ''' 或者<see cref="GetDescendants(String, Boolean, OntologyRelations())"/>。
        ''' </remarks>
        Public Function Family(id As String) As IEnumerable(Of InheritsChain)
            Dim term As TermNode = DAG(id)

            If term Is Nothing Then
                Return {}
            End If

            Dim chains As List(Of InheritsChain) = ancestorChains(id, New HashSet(Of String))

            If chains.Count = 0 Then
                ' 当前的这个term就是一个根节点
                Return {New InheritsChain With {
                    .Route = New List(Of TermNode) From {term}
                }}
            Else
                Return chains
            End If
        End Function

        ''' <summary>
        ''' 生成祖先路径：每一条链都是**由近及远**的祖先节点列表，最后一个元素为根节点。
        ''' 如果<paramref name="id"/>没有父节点(即自身就是根节点)，则返回空集合。
        ''' </summary>
        Private Function ancestorChains(id As String, guard As HashSet(Of String)) As List(Of InheritsChain)
            Dim routes As New List(Of InheritsChain)
            Dim term As TermNode = DAG(id)

            If term Is Nothing OrElse guard.Contains(id) Then
                Return routes
            End If

            Dim parents As String() = isAParents(term)

            If parents.Length = 0 Then
                ' id 自身就是根节点，没有祖先链
                Return routes
            End If

            guard.Add(id)

            For Each parentId As String In parents
                Dim parent As TermNode = DAG(parentId)

                If parent Is Nothing Then
                    Continue For
                End If

                Dim subChains As List(Of InheritsChain) = ancestorChains(parentId, guard)

                If subChains.Count = 0 Then
                    ' parent 就是根节点
                    routes.Add(New InheritsChain With {
                        .Route = New List(Of TermNode) From {parent}
                    })
                Else
                    For Each c As InheritsChain In subChains
                        c.Route.Insert(0, parent)
                        routes.Add(c)
                    Next
                End If
            Next

            guard.Remove(id)

            Return routes
        End Function

        ''' <summary>
        ''' 这个函数是往下查找，找出当前的term的所有的通过is_a关系继承得到的子类型
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns>
        ''' 不包含<paramref name="id"/>自身，只包含其子孙节点
        ''' </returns>
        Public Function GetClusterMembers(id As String) As IEnumerable(Of TermNode)
            Dim table = DescendantTable(Nothing)

            If table.ContainsKey(id) Then
                Return table(id)
            Else
                Return {}
            End If
        End Function

        ''' <summary>
        ''' the lineage
        ''' </summary>
        Public Structure InheritsChain

            Dim Route As List(Of TermNode)

            ReadOnly Tree As TermNode()

            ''' <summary>
            ''' 继承链顶端的节点，即最顶层的祖先节点
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Top As TermNode
                Get
                    If Route Is Nothing OrElse Route.Count = 0 Then
                        Return Nothing
                    Else
                        Return Route.Last
                    End If
                End Get
            End Property

            Public ReadOnly Property [Namespace] As String
                Get
                    Dim top = Me.Top

                    If top Is Nothing OrElse top.GO_term Is Nothing Then
                        Return Nothing
                    Else
                        Return top.GO_term.namespace
                    End If
                End Get
            End Property

            Public ReadOnly Property Family As String()
                Get
                    If Route Is Nothing Then
                        Return {}
                    Else
                        Return Route _
                            .AsEnumerable _
                            .Reverse _
                            .Select(Function(t) t.GO_term.name) _
                            .ToArray
                    End If
                End Get
            End Property

            Private Sub New(tree As TermNode())
                Me.Tree = tree
            End Sub

            ''' <summary>
            ''' 这个函数会自动将<paramref name="lv"/>等级减1转换为向量之中的顶点下表值
            ''' </summary>
            ''' <param name="lv%"></param>
            ''' <returns></returns>
            Public Function Level(lv%) As Term
                ' lv 是从1开始的，所以需要在这里减去1才能够转换为数组的下标值
                Dim tree As TermNode() = Me.Tree

                If tree Is Nothing Then
                    ' 没有经过 Strip() 处理，在这里临时计算一次
                    If Route Is Nothing Then
                        Return Nothing
                    End If

                    tree = Route _
                        .AsEnumerable _
                        .Reverse _
                        .ToArray
                End If

                Dim node As TermNode = tree.ElementAtOrDefault(lv - 1)

                If node Is Nothing Then
                    Return Nothing
                Else
                    Return node.GO_term
                End If
            End Function

            ''' <summary>
            ''' 去除掉继承链之中可能重复出现的节点，并且生成<see cref="Level"/>所需要的Tree向量
            ''' </summary>
            ''' <returns></returns>
            Public Function Strip() As InheritsChain
                If Route Is Nothing Then
                    Return New InheritsChain({}) With {
                        .Route = New List(Of TermNode)
                    }
                End If

                Dim route = Route.Distinct.AsList
                Dim tree = route _
                    .AsEnumerable _
                    .Reverse _
                    .ToArray

                Return New InheritsChain(tree) With {
                    .Route = route
                }
            End Function

            Public Overrides Function ToString() As String
                Return $"[{[Namespace]}] {Family.JoinBy(" -> ")}"
            End Function
        End Structure
    End Class
End Namespace
