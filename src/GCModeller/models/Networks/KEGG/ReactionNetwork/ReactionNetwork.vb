#Region "Microsoft.VisualBasic::e4f13a08a537ec8568a1c628afc54496, models\Networks\KEGG\ReactionNetwork.vb"

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

' Module ReactionNetwork
' 
'     Function: BuildModel, doNetworkExtension, populateEnzymies
' 
'     Sub: AssignNodeClassFromPathwayMaps, AssignNodeClassFromReactionLinks, doAppendReactionEnzyme
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ReactionNetwork

    ''' <summary>
    ''' 这个模块是针对一组给定的特定的代谢物编号列表
    ''' 生成对应的小分子代谢物互做网络图
    ''' </summary>
    Public Class ReactionNetworkBuilder

        Public Shared ReadOnly commonIgnores As Index(Of String) = My.Resources _
            .CommonIgnores _
            .LineTokens _
            .Distinct _
            .ToArray

        Dim blue As New SolidBrush(Color.CornflowerBlue)
        Dim gray As New SolidBrush(Color.LightGray)

        ''' <summary>
        ''' 从输入的数据之中构建出网络的节点列表
        ''' </summary>
        Dim nodes As CompoundNodeTable
        ' {KEGG_compound --> reaction ID()}
        Dim cpdGroups As Dictionary(Of String, String())
        Dim networkBase As Dictionary(Of String, ReactionTable)
        Dim edges As New Dictionary(Of String, Edge)
        Dim g As New NetworkGraph
        Dim reactionIDlist As New List(Of String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        Sub New(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)))
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

            nodes = New CompoundNodeTable(compounds, cpdGroups, g, color:=blue)
        End Sub

        Private Sub addNewEdge(edge As Edge)
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

        Friend Sub createEdges(commons As String(), a As Node, b As Node)
            ' each enzyme is an edge
            For Each rid As String In commons
                Dim geneNames = networkBase(rid)
                Dim rNode As Node

                If Not nodes.containsKey(rid) Then
                    rNode = New Node With {
                        .label = rid,
                        .data = New NodeData With {
                            .label = geneNames.geneNames.JoinBy(", "),
                            .origID = rid,
                            .Properties = New Dictionary(Of String, String) From {
                                {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"}
                            }
                        }
                    }

                    Call nodes.add(rNode)
                Else
                    rNode = nodes(rid)
                End If

                Call New Edge With {
                    .U = a,
                    .V = rNode,
                    .data = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, geneNames.EC.JoinBy(", ")}
                        }
                    },
                    .weight = geneNames.geneNames.Length
                }.DoCall(AddressOf addNewEdge)

                Call New Edge With {
                    .U = rNode,
                    .V = b,
                    .data = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, geneNames.EC.JoinBy(", ")}
                        }
                    },
                    .weight = geneNames.geneNames.Length
                }.DoCall(AddressOf addNewEdge)
            Next
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
                                   Optional enzymeRelated As Boolean = True) As NetworkGraph

            Dim commons As Value(Of String()) = {}
            Dim extendes As New List(Of Node)

            edges = New Dictionary(Of String, Edge)
            reactionIDlist = New List(Of String)

            If extended Then
                Call "KEGG compound network will appends with extended compound reactions".__DEBUG_ECHO
            End If

            ' 下面的这个for循环对所构建出来的节点列表进行边链接构建
            For Each a As Node In nodes.values.Where(Function(n) Not n.label Like commonIgnores).ToArray
                Dim reactionA = cpdGroups.TryGetValue(a.label)

                If reactionA.IsNullOrEmpty Then
                    Continue For
                End If

                For Each b As Node In nodes.values _
                    .Where(Function(x)
                               Return x.ID <> a.ID AndAlso Not x.label Like commonIgnores
                           End Function) _
                    .ToArray

                    Dim rB = cpdGroups.TryGetValue(b.label)

                    If rB.IsNullOrEmpty Then
                        Continue For
                    End If

                    ' a 和 b 是直接相连的
                    If Not (commons = reactionA.Intersect(rB).ToArray).IsNullOrEmpty Then
                        Call reactionIDlist.AddRange(commons.Value)
                        Call createEdges(commons, a, b)
                    Else

                        ' 这两个节点之间可能存在一个空位，
                        ' 对所有的节点进行遍历，找出同时链接a和b的节点
                        If extended Then

                            If Not cpdGroups.ContainsKey(a.label) OrElse Not cpdGroups.ContainsKey(b.label) Then
                                Continue For
                            Else
                                extendes += cpdGroups.doNetworkExtension(a, b, gray, AddressOf addNewEdge, nodes, reactionIDlist)
                            End If

                        End If
                    End If
                Next
            Next

            Return doNetworkExpansion(extendes, enzymeInfo, enzymeRelated)
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
                .doAppendReactionEnzyme(enzymeInfo, networkBase, nodes, AddressOf addNewEdge, enzymeRelated)

            Return g
        End Function
    End Class

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' 利用代谢反应的摘要数据构建出代谢物的互作网络
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        ''' <param name="extended">是否对结果进行进一步的拓展，以获取得到一个连通性更加多的大网络？默认不进行拓展</param>
        ''' <param name="enzymes">
        ''' ``{KO => protein names}``
        ''' </param>
        ''' <param name="enzymaticRelated">
        ''' 是否只使用酶促反应进行网络的构建
        ''' </param>
        ''' <param name="filterByEnzymes">
        ''' 是否只使用<paramref name="enzymes"/>的KO编号相关的反应来构建代谢网络
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function BuildModel(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)),
                                   Optional extended As Boolean = False,
                                   Optional enzymes As Dictionary(Of String, String()) = Nothing,
                                   Optional enzymaticRelated As Boolean = True,
                                   Optional filterByEnzymes As Boolean = False) As NetworkGraph

            Dim source As ReactionTable()

            If filterByEnzymes Then
                If enzymes.Count = 0 Then
                    Return Nothing
                Else
                    source = br08901 _
                        .Where(Function(r)
                                   Return Not r.KO.IsNullOrEmpty AndAlso r.KO.Any(AddressOf enzymes.ContainsKey)
                               End Function) _
                        .ToArray
                End If
            Else
                source = br08901.ToArray
            End If

            Dim builderSession As New ReactionNetworkBuilder(br08901:=source, compounds)
            Dim g = builderSession.BuildModel(extended, enzymes, enzymaticRelated)

            Return g
        End Function
    End Module
End Namespace