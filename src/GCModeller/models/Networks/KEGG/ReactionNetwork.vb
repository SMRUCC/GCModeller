#Region "Microsoft.VisualBasic::786fd8b24057566dfbb25ef3e590a6b0, models\Networks\KEGG\ReactionNetwork.vb"

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
'     Function: BuildModel
' 
'     Sub: AssignNodeClass
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module ReactionNetwork

    <Extension>
    Public Sub AssignNodeClassFromPathwayMaps(net As NetworkTables, maps As Map(), Optional delimiter$ = FunctionalNetwork.Delimiter)
        ' 生成了 compound => maps 的包含关系
        Dim compoundIndex As Dictionary(Of String, String()) = maps _
            .Select(Function(pathway)
                        Return pathway.Areas _
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
                                  .Select(Function(map) $"[{map.ID}] {map.Name}") _
                                  .ToArray
                          End Function)

        For Each node As Node In net.Nodes
            If compoundIndex.ContainsKey(node.ID) Then
                node.NodeType = compoundIndex(node.ID).JoinBy(delimiter)
            Else
                node.NodeType = "KEGG Compound"
            End If
        Next
    End Sub

    ''' <summary>
    ''' 将代谢物网络之中的reaction编号转换为pathway的名称
    ''' </summary>
    ''' <param name="net"></param>
    ''' <param name="ko0001"></param>
    <Extension>
    Public Sub AssignNodeClassFromReactionLinks(net As NetworkTables, ko0001 As KOLinks(), Optional delimiter$ = FunctionalNetwork.Delimiter)
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

        For Each node As Node In net.Nodes
            Dim [class] As New List(Of String)
            Dim rn$() = Strings.Split(node.NodeType, delimiter)

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
                node.NodeType = "KEGG Compound"
            Else
                node.NodeType = [class].JoinBy(delimiter)
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
                               Optional enzymeInfo As Dictionary(Of String, String()) = Nothing) As NetworkTables

        Dim blue As String = Color.CornflowerBlue.RGBExpression
        Dim gray As String = Color.LightGray.RGBExpression
        Dim edges As New Dictionary(Of String, NetworkEdge)
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
        Dim nodes As Dictionary(Of Node) = compounds _
            .Select(Function(cpd As NamedValue(Of String))
                        Dim type$

                        If cpdGroups.ContainsKey(cpd.Name) Then
                            type = cpdGroups(cpd.Name) _
                                .JoinBy(delimiter)
                        Else
                            type = "KEGG Compound"
                        End If

                        Return New Node With {
                            .ID = cpd.Name,
                            .NodeType = type,
                            .Properties = New Dictionary(Of String, String) From {
                                {"name", cpd.Value},
                                {"color", blue}
                            }
                        }
                    End Function) _
            .ToDictionary

        Dim extendes As New List(Of Node)
        Dim doAppendReactionEnzyme = Sub(reactionID As IEnumerable(Of String))
                                         Dim reactions = reactionID _
                                            .Select(Function(id) networkBase(id)) _
                                            .Where(Function(r)
                                                       Return Not r.KO.IsNullOrEmpty
                                                   End Function) _
                                            .ToArray

                                         For Each reaction As ReactionTable In reactions
                                             Dim enzymies = enzymeInfo.Takes(reaction.KO) _
                                                 .IteratesALL _
                                                 .Where(Function(s) Not s.StringEmpty) _
                                                 .Distinct _
                                                 .ToArray

                                             For Each enzyme As String In enzymies
                                                 Throw New NotImplementedException
                                             Next
                                         Next
                                     End Sub

        ' 下面的这个for循环对所构建出来的节点列表进行边链接构建
        For Each a As Node In nodes.Values
            Dim reactionA = cpdGroups.TryGetValue(a.ID)

            If reactionA.IsNullOrEmpty Then
                Continue For
            End If

            For Each b As Node In nodes.Values.Where(Function(x) x.ID <> a.ID)
                Dim rB = cpdGroups.TryGetValue(b.ID)

                If rB.IsNullOrEmpty Then
                    Continue For
                End If

                ' a 和 b 是直接相连的
                If Not (commons = reactionA.Intersect(rB).ToArray).IsNullOrEmpty Then
                    Dim edge As New NetworkEdge With {
                        .fromNode = a.ID,
                        .toNode = b.ID,
                        .value = commons.Value.Length,
                        .interaction = commons.Value.JoinBy("|")
                    }

                    With edge.GetNullDirectedGuid(True)
                        If Not edges.ContainsKey(.ByRef) Then
                            Call edges.Add(.ByRef, edge)
                        End If
                    End With

                    Call doAppendReactionEnzyme(commons.Value)
                Else

                    ' 这两个节点之间可能存在一个空位，
                    ' 对所有的节点进行遍历，找出同时链接a和b的节点
                    If extended Then

                        If Not cpdGroups.ContainsKey(a.ID) OrElse Not cpdGroups.ContainsKey(b.ID) Then
                            Continue For
                        Else
                            extendes += cpdGroups.doNetworkExtension(a, b, gray, edges, doAppendReactionEnzyme)
                        End If

                    End If
                End If
            Next
        Next

        extendes = extendes _
            .GroupBy(Function(n) n.ID) _
            .Select(Function(x) x.First) _
            .AsList

        For Each x In extendes
            If Not nodes.ContainsKey(x.ID) Then
                nodes += x
            End If
        Next

        Return New NetworkTables(nodes.Values, edges.Values)
    End Function

    <Extension>
    Private Iterator Function doNetworkExtension(cpdGroups As Dictionary(Of String, String()),
                                                 a As Node, b As Node,
                                                 gray$,
                                                 edges As Dictionary(Of String, NetworkEdge),
                                                 doAppendReactionEnzyem As Action(Of IEnumerable(Of String))) As IEnumerable(Of Node)
        Dim indexA = cpdGroups(a.ID).Indexing
        Dim indexB = cpdGroups(b.ID).Indexing

        For Each x In cpdGroups
            Dim list = x.Value

            If list.Any(Function(r) indexA(r) > -1) AndAlso list.Any(Function(r) indexB(r) > -1) Then

                ' 这是一个间接的拓展链接，将其加入到边列表之中
                ' X也添加进入拓展节点列表之中
                Yield New Node With {
                    .ID = x.Key,
                    .NodeType = list.JoinBy(Delimiter),
                    .Properties = New Dictionary(Of String, String) From {
                        {"name", x.Key},
                        {"color", gray},
                        {"is_extended", True}
                    }
                }

                Dim populate = Iterator Function()
                                   Yield (a.ID, indexA)
                                   Yield (b.ID, indexB)
                               End Function

                For Each n As (ID$, list As Index(Of String)) In populate()
                    Dim edge As New NetworkEdge With {
                        .fromNode = n.ID,
                        .toNode = x.Key,
                        .interaction = n.list.Objects.Intersect(list).JoinBy("|"),
                        .value = - .interaction.Split("|"c).Length
                    }

                    With edge.GetNullDirectedGuid(True)
                        If Not edges.ContainsKey(.ByRef) Then
                            Call edges.Add(.ByRef, edge)
                        End If
                    End With

                    Call doAppendReactionEnzyem(edge.interaction.Split("|"c))
                Next

                Exit For
            End If
        Next
    End Function
End Module
