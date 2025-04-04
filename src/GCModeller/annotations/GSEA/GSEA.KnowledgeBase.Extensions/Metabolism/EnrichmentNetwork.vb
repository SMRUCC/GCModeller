﻿#Region "Microsoft.VisualBasic::ceaf606911cde7fcd76f6a51e0b8cec4, annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\EnrichmentNetwork.vb"

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

    '   Total Lines: 288
    '    Code Lines: 214 (74.31%)
    ' Comment Lines: 41 (14.24%)
    '    - Xml Docs: 68.29%
    ' 
    '   Blank Lines: 33 (11.46%)
    '     File Size: 13.11 KB


    '     Module EnrichmentNetwork
    ' 
    '         Function: buildModels, CreateGSEASet, getTCode, getUniqueModels, (+2 Overloads) KEGGModels
    '                   PathwayNetworkGraph
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase.Metabolism.Metpa
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Namespace Metabolism

    ''' <summary>
    ''' Model of network based enrichment analysis(KEGG pathway targetted)
    ''' </summary>
    ''' <remarks>
    ''' factors for the enrichment data analysis includes the network 
    ''' topology impact factors, example likes: degree centroid/relative 
    ''' betweeness, etc
    ''' </remarks>
    Public Module EnrichmentNetwork

        ' Dim reactions As Dictionary(Of String, ReactionTable()) = ReactionTable.Load(reactionList).CreateIndex

        ''' <summary>
        ''' 直接基于已有的物种KEGG数据信息进行富集计算数据集的创建
        ''' </summary>
        ''' <returns></returns>
        Public Function KEGGModels(models As Map(),
                                   isKo_ref As Boolean,
                                   reactions As Dictionary(Of String, ReactionTable()),
                                   orgName As String,
                                   multipleOmics As Boolean,
                                   ignoreEnzymes As Boolean) As Metpa.metpa

            Dim tcode As String = models.getTCode

            If Not orgName.StringEmpty Then
                For Each pathway As Map In models
                    pathway.name = pathway.name.Replace(orgName, "").Trim(" "c, "-"c)
                Next
            End If

            Return models _
                .getUniqueModels _
                .ToArray _
                .buildModels(If(isKo_ref, "map", tcode), multipleOmics, ignoreEnzymes, reactions)
        End Function

        <Extension>
        Private Function getTCode(Of T As PathwayBrief)(models As T()) As String
            Dim group_code As IGrouping(Of String, String) = models _
                .Select(Function(pwy) pwy.EntryId.Match("[a-z]+")) _
                .GroupBy(Function(tag) tag) _
                .OrderByDescending(Function(tag) tag.Count) _
                .First
            Dim tcode As String = group_code.Key

            If tcode = "" Then
                ' is the reference map
                Return "map"
            Else
                Return tcode
            End If
        End Function

        ''' <summary>
        ''' 直接基于已有的物种KEGG数据信息进行富集计算数据集的创建
        ''' </summary>
        ''' <returns></returns>
        Public Function KEGGModels(models As Pathway(),
                                   isKo_ref As Boolean,
                                   reactions As Dictionary(Of String, ReactionTable()),
                                   orgName As String,
                                   multipleOmics As Boolean,
                                   ignoreEnzymes As Boolean) As Metpa.metpa

            Dim tcode As String = models.getTCode

            If Not orgName.StringEmpty Then
                For Each pathway As Pathway In models
                    pathway.name = pathway.name.Replace(orgName, "").Trim(" "c, "-"c)
                Next
            End If

            Return models _
                .getUniqueModels _
                .ToArray _
                .buildModels(If(isKo_ref, "map", tcode), multipleOmics, ignoreEnzymes, reactions)
        End Function

        ''' <summary>
        ''' models collection may contains empty data
        ''' or duplicated data,
        ''' needs to do data filter at here at first!
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Function getUniqueModels(Of T As PathwayBrief)(models As T()) As IEnumerable(Of T)
            Return From pwy As T
                   In models
                   Where Not pwy.EntryId.StringEmpty
                   Group By pwy.EntryId Into Group
                   Select Group.First()
        End Function

        <Extension>
        Private Function buildModels(Of T As PathwayBrief)(models As T(),
                                                           keggId As String,
                                                           multipleOmics As Boolean,
                                                           ignoreEnzymes As Boolean,
                                                           reactions As Dictionary(Of String, ReactionTable())) As Metpa.metpa

            Dim pathIds As pathIds = pathIds.FromPathways(models)
            Dim msetList As New msetList With {
                .list = models.ToDictionary(
                    Function(a) If(keggId.StringEmpty, a.EntryId, keggId & a.briteID),
                    Function(a)
                        Dim compounds = a.GetCompoundSet.ToArray
                        Dim genes = a.GetPathwayGenes.ToArray
                        Dim mset As New mset With {
                            .kegg_id = compounds.Select(Function(c) c.Name).ToArray,
                            .metaboliteNames = compounds.Select(Function(c) c.Value).ToArray,
                            .clusterId = a.name
                        }

                        If multipleOmics Then
                            mset.kegg_id = compounds _
                                .Select(Function(c) c.Name) _
                                .JoinIterates(genes.Select(Function(g) g.Description)) _
                                .ToArray
                            mset.metaboliteNames = compounds _
                                .Select(Function(c) c.Value) _
                                .JoinIterates(genes.Select(Function(g) g.Value)) _
                                .ToArray
                        End If

                        Return mset
                    End Function)
            }
            Dim uniqueCompounds As Integer = msetList.CountUnique(models)

            VBDebugger.Mute = True

            Dim graphs As NamedValue(Of NetworkGraph)() = models _
                .Populate(Not VBDebugger.debugMode) _
                .Select(Function(model)
                            Return model.PathwayNetworkGraph(keggId, multipleOmics, ignoreEnzymes, reactions)
                        End Function) _
                .ToArray

            With pathIds.ids.Indexing
                graphs = graphs _
                    .OrderBy(Function(g) .IndexOf(g.Name)) _
                    .ToArray
            End With

            ' 20230223
            '
            ' if there is no graph edges exists in the graph object
            ' then all of the dgr/rbc topology factor value will
            ' be the same
            '
            ' which means the nodes in the graph shares the same weight
            ' or impact to the enrichment result
            '
            Dim rbc As New rbcList With {.list = rbcList.calcRbc(graphs, multipleOmics)}
            Dim dgr As New dgrList With {.pathways = dgrList.calcDgr(graphs, multipleOmics)}
            ' removes empty graph set
            Dim graphSet = graphs _
                .Where(Function(g) g.Value.graphEdges.Any) _
                .ToDictionary(Function(map) map.Name,
                              Function(map)
                                  Return graph.Create(map.Value, map.Name)
                              End Function)

            Return New Metpa.metpa() With {
                .dgrList = dgr,
                .rbcList = rbc,
                .graphList = New graphList With {.graphs = graphSet},
                .msetList = msetList,
                .pathIds = pathIds,
                .unique_count = uniqueCompounds
            }
        End Function

        ''' <summary>
        ''' reconstruct of the kegg pathway and then create gsea background dataset for biodeep package.
        ''' </summary>
        ''' <param name="maps"></param>
        ''' <returns></returns>
        Public Function CreateGSEASet(proteins As PtfFile,
                                      compounds As Dictionary(Of String, String),
                                      reactions As Dictionary(Of String, ReactionTable()),
                                      maps As String,
                                      multipleOmics As Boolean,
                                      ignoreEnzymes As Boolean,
                                      classTable As Dictionary(Of String, ReactionClassTable())) As (Metpa.metpa, DataSet())

            Dim keggId As String = proteins.AsEnumerable.Where(Function(a) a.attributes.ContainsKey("kegg")).First!kegg.Split(":"c).First
            Dim models As Pathway() = MapRepository _
                .GetMapsAuto(maps) _
                .KEGGReconstruction(proteins.AsEnumerable, 0) _
                .Select(Function(pathway)
                            Return pathway.AssignCompounds(
                                reactions:=reactions,
                                names:=compounds,
                                classes:=classTable
                            )
                        End Function) _
                .Where(Function(a) Not a.compound.IsNullOrEmpty) _
                .OrderByDescending(Function(a)
                                       Return a.compound.Length
                                   End Function) _
                .ToArray
            Dim ranks = models _
                .EvaluateCompoundUniqueRank _
                .Transpose _
                .ToArray

            models = models.UniquePathwayCompounds.ToArray

            Return (models.buildModels(keggId, multipleOmics, ignoreEnzymes, reactions), ranks)
        End Function

        <Extension>
        Private Function PathwayNetworkGraph(Of T As PathwayBrief)(model As T,
                                                                   keggId$,
                                                                   multipleOmics As Boolean,
                                                                   ignoreEnzymes As Boolean,
                                                                   reactions As Dictionary(Of String, ReactionTable())) As NamedValue(Of NetworkGraph)

            Dim allCompoundNames As NamedValue(Of String)() = model.GetCompoundSet.ToArray
            Dim enzymes As Dictionary(Of String, String()) = model _
                .GetPathwayGenes _
                .Where(Function(gene) Not (gene.Value.StringEmpty OrElse gene.Name.StringEmpty)) _
                .GroupBy(Function(gene) gene.Value) _
                .ToDictionary(Function(enzyme) enzyme.Key,
                              Function(enzyme)
                                  Return enzyme _
                                      .Select(Function(gene) gene.Name) _
                                      .Distinct _
                                      .ToArray
                              End Function)
            Dim pull As IEnumerable(Of ReactionTable)

            If TypeOf model Is Map Then
                pull = DirectCast(CObj(model), Map).GetReactions(reactions, non_enzymatic:=True)
            ElseIf TypeOf model Is Pathway Then
                If ignoreEnzymes Then
                    pull = DirectCast(CObj(model), Pathway).GetReactionsIgnoreEnzymes(reactions)
                Else
                    pull = DirectCast(CObj(model), Pathway).GetReactions(reactions, non_enzymatic:=True)
                End If
            Else
                Throw New NotImplementedException(model.GetType.FullName)
            End If

            Dim g As NetworkGraph = pull _
                .BuildModel(
                    compounds:=allCompoundNames,
                    extended:=False,
                    enzymes:=enzymes,
                    enzymaticRelated:=False,
                    filterByEnzymes:=False,
                    ignoresCommonList:=False,
                    enzymeBridged:=multipleOmics,
                    strictReactionNetwork:=True
                )
            Dim nameId As String = keggId & model.briteID

            Call g.ComputeBetweennessCentrality(base:=1)
            Call g.ComputeNodeDegrees(base:=1)

            Return New NamedValue(Of NetworkGraph) With {
                .Name = nameId,
                .Value = g
            }
        End Function
    End Module
End Namespace
