#Region "Microsoft.VisualBasic::310e9901e55bd5ee81673623af435fef, GCModeller\annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\EnrichmentNetwork.vb"

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

'   Total Lines: 6
'    Code Lines: 2
' Comment Lines: 3
'   Blank Lines: 1
'     File Size: 122 B


' Class EnrichmentNetwork
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase.Metabolism.Metpa
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
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

        ''' <summary>
        ''' 直接基于已有的物种KEGG数据信息进行富集计算数据集的创建
        ''' </summary>
        ''' <returns></returns>
        Public Function KEGGModels(in$, isKo_ref As Boolean, out As String, reactionList As String, orgName As String) As Integer
            Dim models As Pathway()
            Dim reactions As Dictionary(Of String, ReactionTable()) = ReactionTable.Load(reactionList).CreateIndex

            If isKo_ref Then
                models = (ls - l - r - "*.xml" <= [in]).Select(Function(file) file.LoadXml(Of PathwayMap).ToPathway).ToArray
            Else
                models = (ls - l - r - "*.xml" <= [in]).Select(Function(file) file.LoadXml(Of Pathway)).ToArray
            End If

            If Not orgName.StringEmpty Then
                For Each pathway As Pathway In models
                    pathway.name = pathway.name.Replace(orgName, "").Trim(" "c, "-"c)
                Next
            End If

            Return models.buildModels(If(isKo_ref, "map", ""), reactions).GetJson.SaveTo(out)
        End Function

        <Extension>
        Private Function buildModels(models As Pathway(), keggId As String, reactions As Dictionary(Of String, ReactionTable())) As Metpa.metpa
            Dim pathIds As New pathIds With {
                .ids = models.Select(Function(m) If(keggId.StringEmpty, m.EntryId, keggId & m.briteID)).ToArray,
                .pathwayNames = models.Select(Function(m) m.name.Replace(" - Reference pathway", "")).ToArray
            }
            Dim uniqueCompounds As Integer = models _
                .Select(Function(a) a.compound) _
                .IteratesALL _
                .GroupBy(Function(a) a.name) _
                .Count

            Dim msetList As New msetList With {
                .list = models _
                     .ToDictionary(Function(a) If(keggId.StringEmpty, a.EntryId, keggId & a.briteID), Function(a)
                                                                                                          Dim mset As New mset With {
                                    .kegg_id = a.compound.Select(Function(c) c.name).ToArray,
                                    .metaboliteNames = a.compound.Select(Function(c) c.text).ToArray
                                 }

                                                                                                          Return mset
                                                                                                      End Function)
            }

            VBDebugger.Mute = True

            Dim graphs As NamedValue(Of NetworkGraph)() = models _
                .Populate(Not VBDebugger.debugMode) _
                .Select(Function(model)
                            Return model.createPathwayNetworkGraph(keggId, reactions)
                        End Function) _
                .ToArray

            With pathIds.ids.Indexing
                graphs = graphs.OrderBy(Function(g) .IndexOf(g.Name)).ToArray
            End With

            Dim rbc As New rbcList With {.list = graphs.Select(AddressOf rbcList.calcRbc).ToDictionary(Function(map) map.Name, Function(map) map.Value)}
            Dim dgr As New dgrList With {.pathways = graphs.Select(AddressOf dgrList.calcDgr).ToDictionary(Function(map) map.Name, Function(map) map.Value)}

            Return New Metpa.metpa() With {
                .dgrList = dgr,
                .rbcList = rbc,
                .graphList = New graphList With {.graphs = graphs.ToDictionary(Function(map) map.Name, Function(map) New graph(map.Value))},
                .msetList = msetList,
                .pathIds = pathIds,
                .unique_count = uniqueCompounds
            }
        End Function

        ''' <summary>
        ''' reconstruct of the kegg pathway and then create gsea background dataset for biodeep package.
        ''' </summary>
        ''' <param name="ptf"></param>
        ''' <param name="compoundList"></param>
        ''' <param name="reactionList"></param>
        ''' <param name="maps"></param>
        ''' <param name="classList"></param>
        ''' <param name="out"></param>
        ''' <returns></returns>
        Public Function CreateGSEASet(ptf As String, compoundList As String, reactionList As String, maps As String, classList As String, out As String) As Integer
            Dim proteins = PtfFile.Load(ptf)
            Call proteins.ToString.__DEBUG_ECHO
            Dim keggId As String = proteins.AsEnumerable.Where(Function(a) a.attributes.ContainsKey("kegg")).First!kegg.Split(":"c).First
            Dim classTable As Dictionary(Of String, ReactionClassTable()) = classList _
                .LoadCsv(Of ReactionClassTable) _
                .DoCall(AddressOf ReactionClassTable.ReactionIndex)
            Dim compounds As Dictionary(Of String, String) = compoundList.LoadJSON(Of Dictionary(Of String, String))
            Dim reactions = ReactionTable.Load(reactionList).CreateIndex
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

            Call models _
                .EvaluateCompoundUniqueRank _
                .Transpose _
                .ToArray _
                .SaveTo($"{out.TrimSuffix}.compound_unique.csv", metaBlank:="0")

            models = models.UniquePathwayCompounds.ToArray

            Return models.buildModels(keggId, reactions).GetJson.SaveTo(out)
        End Function

        <Extension>
        Private Function createPathwayNetworkGraph(model As Pathway, keggId$, reactions As Dictionary(Of String, ReactionTable())) As NamedValue(Of NetworkGraph)
            Dim allCompoundNames = model.compound _
                .Select(Function(a) New NamedValue(Of String)(a.name, a.text)) _
                .ToArray
            Dim g As NetworkGraph = model _
                .GetReactions(reactions) _
                .BuildModel(
                    compounds:=allCompoundNames,
                    extended:=False,
                    enzymes:=Nothing,
                    enzymaticRelated:=False,
                    filterByEnzymes:=False,
                    ignoresCommonList:=False,
                    enzymeBridged:=False,
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