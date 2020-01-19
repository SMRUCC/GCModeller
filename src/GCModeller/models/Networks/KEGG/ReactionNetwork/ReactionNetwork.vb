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
                                .Select(Function(id) (id, x))
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

            nodes = New CompoundNodeTable(compounds, cpdGroups, color:=blue)
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

        ''' <summary>
        ''' 利用代谢反应的摘要数据构建出代谢物的互作网络
        ''' </summary>
        ''' <param name="delimiter$"></param>
        ''' <param name="extended">是否对结果进行进一步的拓展，以获取得到一个连通性更加多的大网络？默认不进行拓展</param>
        ''' <param name="enzymeInfo">
        ''' ``{KO => protein names}``
        ''' </param>
        ''' <returns></returns>
        Public Function BuildModel(Optional delimiter$ = FunctionalNetwork.Delimiter,
                                   Optional extended As Boolean = False,
                                   Optional enzymeInfo As Dictionary(Of String, String()) = Nothing,
                                   Optional enzymeRelated As Boolean = True) As NetworkGraph

            Dim commons As Value(Of String()) = {}
            Dim extendes As New List(Of Node)

            g = New NetworkGraph
            edges = New Dictionary(Of String, Edge)
            reactionIDlist = New List(Of String)

            Call nodes.values.DoEach(AddressOf g.AddNode)

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
                        Dim edge As New Edge With {
                            .U = a,
                            .V = b,
                            .data = New EdgeData With {
                                .weight = commons.Value.Length,
                                .Properties = New Dictionary(Of String, String) From {
                                    {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, commons.Value.JoinBy("|")}
                                }
                            }
                        }

                        Call reactionIDlist.AddRange(commons.Value)
                        Call addNewEdge(edge)
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
        ''' <param name="delimiter$"></param>
        ''' <param name="extended">是否对结果进行进一步的拓展，以获取得到一个连通性更加多的大网络？默认不进行拓展</param>
        ''' <param name="enzymeInfo">
        ''' ``{KO => protein names}``
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function BuildModel(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)),
                                   Optional delimiter$ = FunctionalNetwork.Delimiter,
                                   Optional extended As Boolean = False,
                                   Optional enzymeInfo As Dictionary(Of String, String()) = Nothing,
                                   Optional enzymeRelated As Boolean = True) As NetworkGraph

            Dim builderSession As New ReactionNetworkBuilder(br08901, compounds)
            Dim g = builderSession.BuildModel(delimiter, extended, enzymeInfo, enzymeRelated)

            Return g
        End Function
    End Module
End Namespace