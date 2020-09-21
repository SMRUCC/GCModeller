#Region "Microsoft.VisualBasic::9b4e8ee48a094465f436d60bee905bd2, KEGG\ReactionNetwork\Builder\ReactionNetwork.vb"

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
    '         Sub: createEdges
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
        Sub New(br08901 As IEnumerable(Of ReactionTable),
                compounds As IEnumerable(Of NamedValue(Of String)),
                Optional ignoresCommonList As Boolean = True)

            Call MyBase.New(br08901, compounds, blue, ignoresCommonList)
        End Sub

        ''' <summary>
        ''' 两个代谢物之间直接创建一条边
        ''' </summary>
        ''' <param name="commons">a list of reaction id</param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Protected Overrides Sub createEdges(commons As String(), a As Node, b As Node)
            ' each enzyme is an edge
            ' For Each rid As String In commons
            ' Dim geneNames = networkBase(rid)
            Dim rNode As Node
            Dim rid = commons.Select(Function(id) networkBase(id)).GetGeneSymbols

            If Not nodes.containsKey(rid.label) Then
                rNode = New Node With {
                    .label = rid.label,
                    .data = New NodeData With {
                        .label = rid.geneSymbols.Distinct.JoinBy(", "),
                        .origID = rid.label,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"},
                            {"kegg", rid.KO.FirstOrDefault Or (rid.keggRid.First.AsDefault)}
                        }
                    }
                }

                Call nodes.add(rNode)
            Else
                rNode = nodes(rid.label)
            End If

            Call New Edge With {
                .U = a,
                .V = rNode,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rid.EC.Distinct.JoinBy(", ")},
                        {"kegg", commons.GetJson}
                    }
                },
                .weight = rid.geneSymbols.TryCount
            }.DoCall(AddressOf addNewEdge)

            Call New Edge With {
                .U = rNode,
                .V = b,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rid.EC.Distinct.JoinBy(", ")},
                        {"kegg", commons.GetJson}
                    }
                },
                .weight = rid.geneSymbols.TryCount
            }.DoCall(AddressOf addNewEdge)
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
                                   Optional filterByEnzymes As Boolean = False,
                                   Optional ignoresCommonList As Boolean = True) As NetworkGraph

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

            Dim builderSession As New ReactionNetworkBuilder(
                br08901:=source,
                compounds:=compounds,
                ignoresCommonList:=ignoresCommonList
            )
            Dim g As NetworkGraph = builderSession.BuildModel(extended, enzymes, enzymaticRelated)

            Return g
        End Function
    End Module
End Namespace
