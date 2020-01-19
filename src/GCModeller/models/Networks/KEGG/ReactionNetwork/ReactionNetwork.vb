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
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace ReactionNetwork

    ''' <summary>
    ''' 这个模块是针对一组给定的特定的代谢物编号列表
    ''' 生成对应的小分子代谢物互做网络图
    ''' </summary>
    Public Module ReactionNetworkBuilder

        <Extension>
        Public Sub AssignNodeClassFromPathwayMaps(net As NetworkGraph, maps As Map(), Optional delimiter$ = FunctionalNetwork.Delimiter)
            ' 生成了 compound => maps 的包含关系
            Dim compoundIndex As Dictionary(Of String, String()) = maps _
            .Select(Function(pathway)
                        Return pathway.shapes _
                            .Select(Function(a) a.IDVector) _
                            .IteratesALL _
                            .Where(Function(id) id.IsPattern("C\d+")) _
                            .Select(Function(id) (id, pathway))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(link) link.Item1) _
            .ToDictionary(Function(compound) compound.Key,
                          Function(mapList)
                              Return mapList _
                                  .Select(Function(l) l.Item2) _
                                  .Select(Function(map) $"[{map.id}] {map.Name}") _
                                  .ToArray
                          End Function)

            For Each node As Node In net.vertex
                If compoundIndex.ContainsKey(node.ID) Then
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = compoundIndex(node.ID).JoinBy(delimiter)
                Else
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                End If
            Next
        End Sub

        ''' <summary>
        ''' 将代谢物网络之中的reaction编号转换为pathway的名称
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="ko0001"></param>
        <Extension>
        Public Sub AssignNodeClassFromReactionLinks(net As NetworkGraph, ko0001 As KOLinks(), Optional delimiter$ = FunctionalNetwork.Delimiter)
            ' 生成了reaction => pathway的对应关系
            Dim index = ko0001 _
            .Where(Function(ko) Not ko.reactions.IsNullOrEmpty) _
            .Select(Function(ko) ko.reactions.Select(Function(rn) (rn, ko))) _
            .IteratesALL _
            .GroupBy(Function(id) id.Item1) _
            .ToDictionary(Function(id) id.Key,
                          Function(rn)
                              Return rn.Select(Function(x) x.Item2).ToArray
                          End Function)

            For Each node As Node In net.vertex
                Dim [class] As New List(Of String)
                Dim rn$() = Strings.Split(node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), delimiter)

                For Each id In rn
                    If index.ContainsKey(id) Then
                        [class] += index(id) _
                        .Select(Function(ko) ko.pathways.Select(Function(x) x.text)) _
                        .IteratesALL _
                        .Distinct
                    End If
                Next

                [class] = [class].Distinct.AsList

                If [class].IsNullOrEmpty Then
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                Else
                    node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = [class].JoinBy(delimiter)
                End If
            Next
        End Sub

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
        Public Function BuildModel(br08901 As IEnumerable(Of ReactionTable),
                               compounds As IEnumerable(Of NamedValue(Of String)),
                               Optional delimiter$ = FunctionalNetwork.Delimiter,
                               Optional extended As Boolean = False,
                               Optional enzymeInfo As Dictionary(Of String, String()) = Nothing,
                               Optional enzymeRelated As Boolean = True) As NetworkGraph

            Dim blue As New SolidBrush(Color.CornflowerBlue)
            Dim gray As New SolidBrush(Color.LightGray)
            Dim edges As New Dictionary(Of String, Edge)
            Dim g As New NetworkGraph
            ' 构建网络的基础数据
            ' 是依据KEGG代谢反应信息来定义的
            Dim networkBase As Dictionary(Of String, ReactionTable) = br08901 _
            .GroupBy(Function(r) r.entry) _
            .ToDictionary(Function(r) r.Key,
                          Function(g)
                              Return g.First
                          End Function)

            ' {KEGG_compound --> reaction ID()}
            Dim cpdGroups As Dictionary(Of String, String()) = networkBase.Values _
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
            Dim commons As Value(Of String()) = {}

            ' 从输入的数据之中构建出网络的节点列表
            Dim nodes As New CompoundNodeTable(compounds, cpdGroups, color:=blue)
            Dim addNewEdge = Sub(edge As Edge)
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
            Dim extendes As New List(Of Node)
            Dim reactionIDlist As New List(Of String)

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
                                extendes += cpdGroups.doNetworkExtension(a, b, gray, edges, nodes, reactionIDlist)
                            End If

                        End If
                    End If
                Next
            Next

            extendes = extendes _
            .GroupBy(Function(n) n.label) _
            .Select(Function(x) x.First) _
            .AsList

            For Each x In extendes
                If Not nodes.containsKey(x.label) Then
                    nodes += x
                End If
            Next

            If Not enzymeRelated Then
                ' 使用所有的代谢反应来构建酶催化网络
                reactionIDlist = networkBase.Keys.AsList
            End If

            Call reactionIDlist _
            .Distinct _
            .doAppendReactionEnzyme(enzymeInfo, networkBase, nodes, addNewEdge, enzymeRelated)

            Return g
        End Function

        <Extension>
        Private Function populateEnzymies(reaction As ReactionTable, enzymeInfo As Dictionary(Of String, String())) As String()
            Dim list As New List(Of String)

            If Not reaction.KO.IsNullOrEmpty Then
                list += enzymeInfo.Takes(reaction.KO) _
                .IteratesALL _
                .Where(Function(s) Not s.StringEmpty)
            End If
            If Not reaction.EC.IsNullOrEmpty Then
                list += enzymeInfo.Takes(reaction.EC) _
                .IteratesALL _
                .Where(Function(s) Not s.StringEmpty)
            End If

            Return list.Distinct.ToArray
        End Function

        <Extension>
        Private Sub doAppendReactionEnzyme(reactionID As IEnumerable(Of String),
                                       enzymeInfo As Dictionary(Of String, String()),
                                       networkBase As Dictionary(Of String, ReactionTable),
                                       nodes As Dictionary(Of Node),
                                       addNewEdge As Action(Of Edge),
                                       enzymeRelated As Boolean)

            Dim reactions As ReactionTable()
            Dim usedEnzymies As New List(Of String)

            If enzymeInfo.IsNullOrEmpty Then
                Return
            Else
                reactions = reactionID _
                .Select(Function(id) networkBase(id)) _
                .Where(Function(r)
                           Return Not r.KO.IsNullOrEmpty OrElse Not r.EC.IsNullOrEmpty
                       End Function) _
                .ToArray
            End If

            For Each reaction As ReactionTable In reactions _
            .Where(Function(rn)
                       If enzymeRelated Then
                           Return rn.substrates.Any(AddressOf nodes.ContainsKey) OrElse
                                  rn.products.Any(AddressOf nodes.ContainsKey)
                       Else
                           Return True
                       End If
                   End Function)

                Dim enzymies = reaction.populateEnzymies(enzymeInfo)

                If enzymies.IsNullOrEmpty Then
                    Continue For
                Else
                    usedEnzymies += enzymies
                End If

                If Not nodes.ContainsKey(reaction.entry) Then
                    nodes.Add(New Node With {
                    .ID = reaction.entry,
                    .NodeType = "reaction",
                    .Properties = New Dictionary(Of String, String) From {
                         {"name", reaction.name},
                         {"color", "yellow"}
                    }
                })
                End If

                For Each enzyme As String In enzymies
                    If Not nodes.ContainsKey(enzyme) Then
                        nodes.Add(New Node With {
                        .ID = enzyme,
                        .NodeType = "enzyme",
                        .Properties = New Dictionary(Of String, String) From {
                            {"name", enzyme},
                            {"color", "red"}
                        }
                    })
                    End If

                    Dim edge As New NetworkEdge With {
                   .fromNode = enzyme,
                   .toNode = reaction.entry,
                   .Interaction = "catalyst",
                   .Value = 1
                }

                    Call addNewEdge(edge)
                Next

                ' link between reaction with compounds
                ' 不添加新的代谢物节点
                ' 只添加边链接
                For Each compound In reaction.products
                    Dim edge As New NetworkEdge With {
                   .fromNode = reaction.entry,
                   .toNode = compound,
                   .Interaction = "reaction",
                   .Value = 1
                }

                    If Not nodes.ContainsKey(compound) Then
                        ' nodes.Add(New Node With {.ID = compound, .NodeType = "KEGG Compound", .Properties = New Dictionary(Of String, String) From {{"name", compound}, {"color", gray}, {"is_extended", True}}})
                    Else
                        Call addNewEdge(edge)
                    End If
                Next

                For Each compound In reaction.substrates
                    Dim edge As New NetworkEdge With {
                   .toNode = reaction.entry,
                   .fromNode = compound,
                   .Interaction = "reaction",
                   .Value = 1
                }

                    If Not nodes.ContainsKey(compound) Then
                        ' nodes.Add(New Node With {.ID = compound, .NodeType = "KEGG Compound", .Properties = New Dictionary(Of String, String) From {{"name", compound}, {"color", gray}, {"is_extended", True}}})
                    Else
                        Call addNewEdge(edge)
                    End If
                Next
            Next

            'For Each unusedEnzyme In enzymeInfo.Values.IteratesALL.Distinct.Indexing - usedEnzymies
            '    If Not nodes.ContainsKey(unusedEnzyme.value) Then
            '        nodes.Add(New Node With {
            '            .ID = unusedEnzyme.value,
            '            .NodeType = "enzyme",
            '            .Properties = New Dictionary(Of String, String) From {
            '                {"name", unusedEnzyme.value},
            '                {"color", "red"}
            '            }
            '        })
            '    End If
            'Next
        End Sub

        Public ReadOnly commonIgnores As Index(Of String) = My.Resources _
        .CommonIgnores _
        .LineTokens _
        .Distinct _
        .ToArray

        <Extension>
        Private Iterator Function doNetworkExtension(cpdGroups As Dictionary(Of String, String()),
                                                 a As Node, b As Node,
                                                 gray As SolidBrush,
                                                 addEdge As Action(Of Edge),
                                                 nodes As CompoundNodeTable,
                                                 reactionIDlist As List(Of String)) As IEnumerable(Of Node)
            Dim indexA = cpdGroups(a.label).Indexing
            Dim indexB = cpdGroups(b.label).Indexing

            For Each x In cpdGroups.Where(Function(compound)
                                              ' C00001 是水,很多代谢过程都存在的
                                              ' 在这里就没有必要添加进来了
                                              Return Not compound.Key Like commonIgnores
                                          End Function)
                Dim list As String() = x.Value

                If list.Any(Function(r) indexA(r) > -1) AndAlso list.Any(Function(r) indexB(r) > -1) Then

                    ' 这是一个间接的拓展链接，将其加入到边列表之中
                    ' X也添加进入拓展节点列表之中
                    Yield New Node With {
                    .label = x.Key,
                    .data = New NodeData With {
                        .color = gray,
                        .label = x.Key,
                        .origID = x.Key,
                        .Properties = New Dictionary(Of String, String) From {
                            {"name", x.Key},
                            {"is_extended", True},
                            {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, list.JoinBy(Delimiter)}
                        }
                    }
                }

                    Dim populate = Iterator Function()
                                       Yield (a.label, indexA)
                                       Yield (b.label, indexB)
                                   End Function

                    For Each n As (ID$, list As Index(Of String)) In populate()
                        Dim interactions As String() = n.list.Objects.Intersect(list).ToArray
                        Dim edge As New Edge With {
                        .U = nodes(n.ID),
                        .V = nodes(x.Key),
                        .data = New EdgeData With {
                            .length = interactions.Length,
                            .Properties = New Dictionary(Of String, String) From {
                                {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, interactions.JoinBy("|")}
                            }
                        }
                    }

                        Call addEdge(edge)
                        Call reactionIDlist.AddRange(interactions)
                    Next
                End If
            Next
        End Function
    End Module
End Namespace