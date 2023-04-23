#Region "Microsoft.VisualBasic::da19cc85d45f1a3ee39ad9bbe8b157c6, GCModeller\models\Networks\KEGG\ReactionNetwork\Builder\ReactionNetwork.vb"

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

'   Total Lines: 225
'    Code Lines: 148
' Comment Lines: 53
'   Blank Lines: 24
'     File Size: 9.55 KB


'     Class ReactionNetworkBuilder
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: compoundEdge, enzymeBridgedEdges
' 
'         Sub: createEdges
' 
'     Module Extensions
' 
'         Function: BuildModel, GetReactions
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace ReactionNetwork

    ''' <summary>
    ''' 这个模块是针对一组给定的特定的代谢物编号列表
    ''' 生成对应的小分子代谢物互做网络图
    ''' </summary>
    Public Class ReactionNetworkBuilder : Inherits BuilderBase

        ReadOnly enzymeBridged As Boolean = True

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        ''' <param name="enzymeBridged">
        ''' This option will affects the network connected style:
        ''' 
        ''' + true:   compound -- enzyme -- compound
        ''' + false:  compound -- compound
        ''' </param>
        Sub New(br08901 As IEnumerable(Of ReactionTable),
                compounds As IEnumerable(Of NamedValue(Of String)),
                Optional ignoresCommonList As Boolean = True,
                Optional enzymeBridged As Boolean = True,
                Optional edgeFilter As EdgeFilterEngine = EdgeFilterEngine.ReactionLinkFilter,
                Optional randomLayout As Boolean = True)

            Call MyBase.New(br08901, compounds, blue, ignoresCommonList, edgeFilter, randomLayout)

            Me.enzymeBridged = enzymeBridged
        End Sub

        ''' <summary>
        ''' 两个代谢物之间直接创建一条边
        ''' </summary>
        ''' <param name="commons">a list of reaction id</param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Protected Overrides Sub createEdges(commons As String(), a As Node, b As Node)
            If enzymeBridged Then
                For Each edge In enzymeBridgedEdges(commons, a, b)
                    Call addNewEdge(edge)
                Next
            Else
                Call addNewEdge(compoundEdge(commons, a, b))
            End If
        End Sub

        ''' <summary>
        ''' two compound is connected via a reaction link, the link id is the reaction id
        ''' </summary>
        ''' <param name="commons"></param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Private Function compoundEdge(commons As String(), a As Node, b As Node) As Edge
            Return New Edge With {
                .U = a,
                .V = b,
                .weight = commons.Length,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {"kegg", commons.GetJson}
                    }
                }
            }
        End Function

        ''' <summary>
        ''' tow compound is connected via a enzyme gene node
        ''' </summary>
        ''' <param name="commons"></param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Private Iterator Function enzymeBridgedEdges(commons As String(), a As Node, b As Node) As IEnumerable(Of Edge)
            ' each enzyme is an edge
            ' For Each rid As String In commons
            ' Dim geneNames = networkBase(rid)
            Dim rNode As Node
            Dim rid = commons.Select(Function(id) networkBase(id)).GetGeneSymbols

            If Not nodes.containsKey(rid.label) Then
                rNode = New Node With {
                    .label = rid.label,
                    .data = New NodeData With {
                        .label = rid.label,
                        .origID = rid.label,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"},
                            {"kegg", rid.KO.FirstOrDefault Or (rid.keggRid.First.AsDefault)},
                            {"gene_symbols", rid.geneSymbols.Distinct.JoinBy(", ")}
                        }
                    }
                }

                Call nodes.add(rNode)
            Else
                rNode = nodes(rid.label)
            End If

            Yield New Edge With {
                .U = a,
                .V = rNode,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rid.EC.Distinct.JoinBy(", ")},
                        {"kegg", commons.GetJson}
                    }
                },
                .weight = rid.geneSymbols.TryCount
            }

            Yield New Edge With {
                .U = rNode,
                .V = b,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rid.EC.Distinct.JoinBy(", ")},
                        {"kegg", commons.GetJson}
                    }
                },
                .weight = rid.geneSymbols.TryCount
            }
        End Function
    End Class

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' 利用代谢反应的摘要数据构建出代谢物的互作网络
        ''' </summary>
        ''' <param name="br08901">Metabolic links which is represented via the kegg 
        ''' reaction model data. A metabolic network will be build based on this 
        ''' reaction data.
        ''' (代谢反应数据)</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        ''' <param name="extended">是否对结果进行进一步的拓展，以获取得到一个连通性更加多的大网络？默认不进行拓展</param>
        ''' <param name="enzymes">
        ''' ``{KO => protein names}``
        ''' </param>
        ''' <param name="enzymaticRelated">
        ''' 是否只使用酶促反应进行网络的构建
        ''' </param>
        ''' <param name="filterByEnzymes">
        ''' Just re-construct a kegg metabolic network which its all reaction 
        ''' is related with the input <paramref name="enzymes"/> list?
        ''' (是否只使用<paramref name="enzymes"/>的KO编号相关的反应来构建代谢网络)
        ''' </param>
        ''' <param name="enzymeBridged">
        ''' This option will affects the network connected style:
        ''' 
        ''' + true:   compound -- enzyme -- compound
        ''' + false:  compound -- compound
        ''' 
        ''' set this parameter value to TRUE could create a metabolic network 
        ''' that used for run multiple omics data analysis.
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function BuildModel(br08901 As IEnumerable(Of ReactionTable), compounds As IEnumerable(Of NamedValue(Of String)),
                                   Optional extended As Boolean = False,
                                   Optional enzymes As Dictionary(Of String, String()) = Nothing,
                                   Optional enzymaticRelated As Boolean = True,
                                   Optional filterByEnzymes As Boolean = False,
                                   Optional ignoresCommonList As Boolean = True,
                                   Optional enzymeBridged As Boolean = True,
                                   Optional strictReactionNetwork As Boolean = False,
                                   Optional randomLayout As Boolean = True) As NetworkGraph

            Dim source As ReactionTable()

            If filterByEnzymes Then
                ' required of filter by enzymes
                ' but the given enzyme set is empty
                ' so no network could be re-constructed!
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

            ' do reaction links uniques
            source = source _
                .GroupBy(Function(r) r.entry) _
                .Select(Function(r) r.First) _
                .ToArray

            Dim builderSession As New ReactionNetworkBuilder(
                br08901:=source,
                compounds:=compounds,
                ignoresCommonList:=ignoresCommonList,
                enzymeBridged:=enzymeBridged,
                randomLayout:=randomLayout
            )
            Dim g As NetworkGraph = builderSession.BuildModel(
                extended:=extended,
                enzymeInfo:=enzymes,
                enzymeRelated:=enzymaticRelated,
                strictReactionNetwork:=strictReactionNetwork
            )

            Return g
        End Function

        ''' <summary>
        ''' Pull out all of the pathway related reactions data
        ''' </summary>
        ''' <param name="pathway"></param>
        ''' <param name="reactions">
        ''' A repository for the kegg reaction data models
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' we are not going to add the non-enzymics reaction into each pathway map
        ''' because this operation will caused all of the pathway map contains the 
        ''' similar compound profile which is bring by all of the non-enzymics reactions.
        ''' </remarks>
        <Extension>
        Public Iterator Function GetReactions(pathway As Map,
                                              reactions As Dictionary(Of String, ReactionTable()),
                                              Optional non_enzymatic As Boolean = False) As IEnumerable(Of ReactionTable)

            For Each id As String In pathway.GetMembers.Where(Function(si) si.IsPattern("K\d+"))
                If reactions.ContainsKey(id) Then
                    For Each item As ReactionTable In reactions(id)
                        Yield item
                    Next
                End If
            Next

            For Each gene As NamedValue(Of String) In pathway.GetPathwayGenes
                Dim ko As String = gene.Value

                If Not ko.StringEmpty AndAlso reactions.ContainsKey(ko) Then
                    For Each item As ReactionTable In reactions(ko)
                        Yield item
                    Next
                End If

                If reactions.ContainsKey(gene.Name) Then
                    For Each item As ReactionTable In reactions(gene.Name)
                        Yield item
                    Next
                End If

                If Not gene.Description.StringEmpty Then
                    If reactions.ContainsKey(gene.Description) Then
                        For Each item As ReactionTable In reactions(gene.Description)
                            Yield item
                        Next
                    End If
                End If
            Next

            If non_enzymatic Then
                For Each rxn As ReactionTable In pathway.MatchesNonEnzymatics(reactions)
                    Yield rxn
                Next
            End If
        End Function

        <Extension>
        Public Iterator Function MatchesNonEnzymatics(pathway As PathwayBrief, reactions As Dictionary(Of String, ReactionTable())) As IEnumerable(Of ReactionTable)
            Dim compounds As Index(Of String) = pathway.GetCompoundSet _
                .Select(Function(c) c.Name) _
                .Indexing

            If compounds.Count > 0 Then
                ' populate out all current pathway related
                ' non-enzymatic reactions
                For Each item As ReactionTable In reactions.Values _
                    .IteratesALL _
                    .GroupBy(Function(a) a.entry) _
                    .Select(Function(a) a.First)

                    If item.geneNames.IsNullOrEmpty AndAlso
                        item.EC.IsNullOrEmpty AndAlso
                        item.KO.IsNullOrEmpty Then

                        If item.MatchAllCompoundsId(compounds) Then
                            Yield item
                        End If
                    End If
                Next
            End If
        End Function

        ''' <summary>
        ''' Pull out all of the pathway related reactions data
        ''' </summary>
        ''' <param name="pathway"></param>
        ''' <param name="reactions">
        ''' A repository for the kegg reaction data models
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' we are not going to add the non-enzymics reaction into each pathway map
        ''' because this operation will caused all of the pathway map contains the 
        ''' similar compound profile which is bring by all of the non-enzymics reactions.
        ''' </remarks>
        <Extension>
        Public Iterator Function GetReactions(pathway As Pathway,
                                              reactions As Dictionary(Of String, ReactionTable()),
                                              Optional non_enzymatic As Boolean = False) As IEnumerable(Of ReactionTable)

            For Each ko As NamedValue In pathway.KOpathway.JoinIterates(pathway.modules)
                If reactions.ContainsKey(ko.name) Then
                    For Each item As ReactionTable In reactions(ko.name)
                        Yield item
                    Next
                End If
            Next

            For Each gene As GeneName In pathway.genes.SafeQuery
                Dim ko As String = gene.KO

                If Not ko.StringEmpty AndAlso reactions.ContainsKey(ko) Then
                    For Each item As ReactionTable In reactions(ko)
                        Yield item
                    Next
                End If

                If reactions.ContainsKey(gene.geneId) Then
                    For Each item As ReactionTable In reactions(gene.geneId)
                        Yield item
                    Next
                End If
                If reactions.ContainsKey(gene.geneName) Then
                    For Each item As ReactionTable In reactions(gene.geneName)
                        Yield item
                    Next
                End If

                If Not gene.EC Is Nothing Then
                    For Each id As String In gene.EC
                        If Not id.StringEmpty AndAlso reactions.ContainsKey(id) Then
                            For Each item As ReactionTable In reactions(id)
                                Yield item
                            Next
                        End If
                    Next
                End If
            Next

            If non_enzymatic AndAlso Not pathway.compound Is Nothing Then
                For Each rxn As ReactionTable In pathway.MatchesNonEnzymatics(reactions)
                    Yield rxn
                Next
            End If
        End Function
    End Module
End Namespace
