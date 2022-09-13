#Region "Microsoft.VisualBasic::1d31761be3f278303cf57d7a092ebb1e, GCModeller\models\Networks\KEGG\ReactionNetwork\Builder\BuilderBase.vb"

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

    '   Total Lines: 247
    '    Code Lines: 171
    ' Comment Lines: 41
    '   Blank Lines: 35
    '     File Size: 10.23 KB


    '     Class BuilderBase
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: BuildModel, doExpansion, doNetworkExpansion
    ' 
    '         Sub: addNewEdge
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 1 Then
Imports System.Data
#End If

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ReactionNetwork

    Public MustInherit Class BuilderBase

        ''' <summary>
        ''' 正常的通过这个模型对象的构造函数创建的代谢物节点的颜色为蓝色
        ''' </summary>
        Protected Shared ReadOnly blue As New SolidBrush(Color.CornflowerBlue)
        ''' <summary>
        ''' 通过expansion操作添加的额外的代谢物的节点的颜色为灰色
        ''' </summary>
        Protected Shared ReadOnly gray As New SolidBrush(Color.LightGray)

        ''' <summary>
        ''' some primary metabolite connected too much reactions, ignores these metabolites
        ''' </summary>
        Friend ReadOnly commonIgnores As Index(Of String) = IgnoreList.InOrganicPrimary

        ''' <summary>
        ''' 从输入的数据之中构建出网络的节点列表
        ''' </summary>
        Protected ReadOnly nodes As CompoundNodeTable
        ' {KEGG_compound --> reaction ID()}
        Protected ReadOnly cpdGroups As Dictionary(Of String, String())
        Protected ReadOnly networkBase As Dictionary(Of String, ReactionTable)

        Protected ReadOnly g As New NetworkGraph

        Protected edges As New Dictionary(Of String, Edge)
        Protected reactionIDlist As New List(Of String)

        Protected ReadOnly strictFilter As EdgeFilter

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        Protected Sub New(br08901 As IEnumerable(Of ReactionTable),
                          compounds As IEnumerable(Of NamedValue(Of String)),
                          color As Brush,
                          ignoresCommonList As Boolean,
                          filterEngine As EdgeFilterEngine,
                          randomLayout As Boolean)

            ' 构建网络的基础数据
            ' 是依据KEGG代谢反应信息来定义的
            networkBase = br08901 _
                .GroupBy(Function(r) r.entry) _
                .ToDictionary(Function(r) r.Key,
                              Function(group)
                                  Return group.First
                              End Function)

            ' {KEGG_compound --> reaction ID()}
            cpdGroups = networkBase.Values _
                .Select(Function(x)
                            Return x.substrates _
                                .JoinIterates(x.products) _
                                .Select(Function(id)
                                            Return (id, x)
                                        End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(x) x.Item1) _
                .ToDictionary(Function(x) x.Key,
                              Function(reactions)
                                  Return reactions _
                                      .Select(Function(x) x.Item2.entry) _
                                      .Distinct _
                                      .ToArray
                              End Function)

            If Not ignoresCommonList Then
                commonIgnores = {}
            End If

            nodes = New CompoundNodeTable(
                compounds:=compounds,
                cpdGroups:=cpdGroups,
                ignores:=commonIgnores,
                g:=g,
                color:=color,
                randomLayout:=randomLayout
            )
            strictFilter = EdgeFilter.CreateFilter(
                nodes:=nodes,
                networkBase:=networkBase,
                commonIgnores:=commonIgnores,
                engine:=filterEngine
            )
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="commons">a list of reaction id</param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Protected MustOverride Sub createEdges(commons As String(), a As Node, b As Node)

        Protected Sub addNewEdge(edge As Edge)
            Dim ledge As IInteraction = edge

            If (Not nodes.containsKey(ledge.source)) OrElse (Not nodes.containsKey(ledge.target)) Then
                Throw New InvalidExpressionException(edge.ToString)
            End If
            If ledge.source Like commonIgnores OrElse ledge.target Like commonIgnores Then
                ' 跳过水
                Return
            End If

            With edge.GetNullDirectedGuid(True)
                If Not .DoCall(AddressOf edges.ContainsKey) Then
                    Call edges.Add(.ByRef, edge)
                    Call g.AddEdge(edge)
                End If
            End With
        End Sub

        ''' <summary>
        ''' 利用代谢反应的摘要数据构建出代谢物的互作网络
        ''' </summary>
        ''' <param name="extended">是否对结果进行进一步的拓展，以获取得到一个连通性更加多的大网络？默认不进行拓展</param>
        ''' <param name="enzymeInfo">
        ''' ``{KO => protein names}``
        ''' </param>
        ''' <returns></returns>
        Public Function BuildModel(Optional extended As Boolean = False,
                                   Optional enzymeInfo As Dictionary(Of String, String()) = Nothing,
                                   Optional enzymeRelated As Boolean = True,
                                   Optional strictReactionNetwork As Boolean = False) As NetworkGraph

            Dim commons As Value(Of String()) = {}
            Dim extendes As New List(Of Node)

            edges = New Dictionary(Of String, Edge)
            reactionIDlist = New List(Of String)

            If extended Then
                Call "KEGG compound network will appends with extended compound reactions".__DEBUG_ECHO
            End If

            Dim compoundNodesAll As Node() = nodes.values _
                .Where(Function(n)
                           Return n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound" AndAlso Not n.label Like commonIgnores
                       End Function) _
                .ToArray

            ' 下面的这个for循环对所构建出来的节点列表进行边链接构建
            For Each a As Node In compoundNodesAll
                Dim reactionA = cpdGroups.TryGetValue(a.label)

                If reactionA.IsNullOrEmpty Then
                    Continue For
                End If

                For Each b As Node In compoundNodesAll.Where(Function(x) x.ID <> a.ID AndAlso cpdGroups.ContainsKey(x.label))
                    Dim rB As String() = cpdGroups(b.label)

                    ' a 和 b 是直接相连的
                    If Not (commons = reactionA.Intersect(rB).ToArray).IsNullOrEmpty Then
                        If strictReactionNetwork Then
                            commons = strictFilter.filter(commons).ToArray
                        End If

                        If commons.Value.IsNullOrEmpty Then
                            If extended Then
                                Call extendes.AddRange(doExpansion(a, b))
                            End If
                        Else
                            Call reactionIDlist.AddRange(commons.Value)
                            Call createEdges(commons, a, b)
                        End If
                    Else
                        If extended Then
                            Call extendes.AddRange(doExpansion(a, b))
                        End If
                    End If
                Next
            Next

            Return doNetworkExpansion(extendes, enzymeInfo, enzymeRelated)
        End Function

        Private Function doExpansion(a As Node, b As Node) As IEnumerable(Of Node)
            ' 这两个节点之间可能存在一个空位，
            ' 对所有的节点进行遍历，找出同时链接a和b的节点
            If Not cpdGroups.ContainsKey(a.label) OrElse Not cpdGroups.ContainsKey(b.label) Then
                Return {}
            Else
                Return cpdGroups.doNetworkExtension(
                    a:=a,
                    b:=b,
                    gray:=gray,
                    addEdge:=AddressOf addNewEdge,
                    nodes:=nodes,
                    reactionIDlist:=reactionIDlist
                )
            End If
        End Function

        Private Function doNetworkExpansion(extends As List(Of Node), enzymeInfo As Dictionary(Of String, String()), enzymeRelated As Boolean) As NetworkGraph
            extends = extends _
               .GroupBy(Function(n) n.label) _
               .Select(Function(x) x.First) _
               .AsList

            For Each x In extends
                If Not nodes.containsKey(x.label) Then
                    nodes.add(x)
                End If
            Next

            If Not enzymeRelated Then
                ' 使用所有的代谢反应来构建酶催化网络
                reactionIDlist = networkBase.Keys.AsList
            End If

            Call reactionIDlist _
                .Distinct _
                .doAppendReactionEnzyme(
                    enzymeInfo:=enzymeInfo,
                    networkBase:=networkBase,
                    nodes:=nodes,
                    addNewEdge:=AddressOf addNewEdge,
                    enzymeRelated:=enzymeRelated
                )

            Return g
        End Function
    End Class
End Namespace
