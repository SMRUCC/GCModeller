#Region "Microsoft.VisualBasic::a3f6ca1695e53d285626750bddb8c410, Networks\KEGG\ReactionNetwork\ReactionNetwork.vb"

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

'     Class ReactionNetworkBuilder
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: BuildModel, doNetworkExpansion
' 
'         Sub: addNewEdge, createEdges
' 
'     Module Extensions
' 
'         Function: BuildModel
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ReactionNetwork

    ''' <summary>
    ''' 这个模块是针对一组给定的特定的代谢物编号列表
    ''' 生成对应的小分子代谢物互做网络图
    ''' </summary>
    Public Class ReactionNetworkBuilder : Inherits BuilderBase

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        Sub New(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)))
            Call MyBase.New(br08901, compounds, blue)
        End Sub

        ''' <summary>
        ''' 两个代谢物之间直接创建一条边
        ''' </summary>
        ''' <param name="commons">a list of reaction id</param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Protected Overrides Sub createEdges(commons As String(), a As Node, b As Node)
            Dim models = commons.Select(Function(id) networkBase(id)).Select(Function(r) r.geneNames.JoinIterates(r.KO).JoinIterates(r.EC)).IteratesALL.Select(Function(s) s.StringSplit("[;,]")).IteratesALL.Select(AddressOf Strings.Trim).Where(Function(s) Not s.StringEmpty).ToArray
            Dim KO = models.Where(Function(id) id.IsPattern("K\d+")).ToArray
            Dim EC = models.Select(Function(id) id.Match("\d+\.([-]|(\d+))(\.([-]|(\d+)))+")).Where(Function(id) Not id.StringEmpty).ToArray
            Dim keggRid = models.Select(Function(id) id.Match("R\d+")).Where(Function(id) Not id.StringEmpty).ToArray
            Dim allId As String() = KO.JoinIterates(EC).JoinIterates(keggRid).ToArray
            Dim geneSymbols = models.AsParallel.Where(Function(line) line.InStrAny(allId) = -1).ToArray
            Dim middleNode As String

            If models.Length = 1 Then
                middleNode = models(Scan0)
            Else
                If geneSymbols.IsNullOrEmpty Then
                    If EC.IsNullOrEmpty Then
                        If KO.IsNullOrEmpty Then
                            middleNode = keggRid.GroupBy(Function(id) id).OrderByDescending(Function(g) g.Count).First.Key
                        Else
                            middleNode = KO.GroupBy(Function(id) id).OrderByDescending(Function(g) g.Count).First.Key
                        End If
                    Else
                        middleNode = EC.GroupBy(Function(id) id.Split("."c).Take(2).JoinBy(".")).OrderByDescending(Function(g) g.Count).First.Key & ".-.-"
                    End If
                Else
                    middleNode = geneSymbols.GroupBy(Function(name) name.ToLower).OrderByDescending(Function(g) g.Count).First.First
                End If
            End If

            ' each enzyme is an edge
            ' For Each rid As String In commons
            ' Dim geneNames = networkBase(rid)
            Dim rNode As Node
            Dim rid = middleNode

            If Not nodes.containsKey(rid) Then
                rNode = New Node With {
                    .label = rid,
                    .data = New NodeData With {
                        .label = geneSymbols.Distinct.JoinBy(", "),
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
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, EC.Distinct.JoinBy(", ")},
                        {"kegg", commons.GetJson}
                    }
                },
                .weight = geneSymbols.TryCount
            }.DoCall(AddressOf addNewEdge)

            Call New Edge With {
                .U = rNode,
                .V = b,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, EC.Distinct.JoinBy(", ")},
                        {"kegg", commons.GetJson}
                    }
                },
                .weight = geneSymbols.TryCount
            }.DoCall(AddressOf addNewEdge)
            ' Next
        End Sub
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
