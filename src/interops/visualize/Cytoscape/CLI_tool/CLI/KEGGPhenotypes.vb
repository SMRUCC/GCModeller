#Region "Microsoft.VisualBasic::9ab59dbf899486c9fb19a43b1e34e946, visualize\Cytoscape\CLI_tool\CLI\KEGGPhenotypes.vb"

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

' Module CLI
' 
'     Function: __distinctCommon, BuildModelNet, CompoundNetwork, KEGGModulesPhenotypeRegulates, loadEnzymeMaps
'               PathwayNet, SimpleModesNET
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Model.Network.KEGG
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Serialization
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File
Imports SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET

Partial Module CLI

    <Extension>
    Private Function loadEnzymeMaps(enzymeFile As String) As Dictionary(Of String, String())
        Dim annotation = EntityObject.LoadDataSet(enzymeFile)
        Dim getProteinNames = Function(g As IEnumerable(Of EntityObject))
                                  Return g.Select(Function(prot)
                                                      If prot!geneName.StringEmpty Then
                                                          Return prot.ID
                                                      Else
                                                          Return prot!geneName
                                                      End If
                                                  End Function) _
                                          .Distinct _
                                          .ToArray
                              End Function
        Dim enzymes = annotation _
            .Where(Function(d)
                       '  关联是通过代谢反应的酶促关系来完成的
                       Return Not d!KO.StringEmpty
                   End Function) _
            .GroupBy(Function(d) d!KO) _
            .ToDictionary(Function(d) d.Key,
                          Function(g)
                              Return getProteinNames(g)
                          End Function)

        For Each ECnumberGroup In annotation _
            .Where(Function(d) Not d!EC.StringEmpty) _
            .Select(Function(d)
                        Return d!EC.StringSplit("\s*;\s*").Select(Function(id) (id, d))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(EC) EC.Item1)

            enzymes.Add(ECnumberGroup.Key, getProteinNames(ECnumberGroup.Select(Function(t) t.Item2)))
        Next

        Return enzymes
    End Function

    <ExportAPI("/kegg.compound.network")>
    <Usage("/kegg.compound.network /in <compound.csv> /reactions <reaction_table.csv> [/enzyme <annotation.csv> /extended /enzymeRelated /size <default=10000,7000> /out <network.directory>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(NamedValue(Of String))},
              Extensions:="*.csv",
              Description:="The [compound_id => compound_name] information.")>
    <Argument("/reactions", False, CLITypes.File, PipelineTypes.undefined,
              AcceptTypes:={GetType(ReactionTable)},
              Extensions:="*.csv",
              Description:="A csv table of reaction brief information, which it could be generated from the ``/Write.Reaction.Table`` command.")>
    <Argument("/enzyme", True, CLITypes.File,
              AcceptTypes:={GetType(EntityObject)},
              Extensions:="*.csv",
              Description:="A protein annotation table which is generated by the ``/protein.annotations`` command in eggHTS tool.")>
    <Argument("/extended", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If the compounds can not create a network model by link each other through reaction model, then you could enable
              this argument will makes extension connection for create a compound network model.")>
    Public Function CompoundNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim reactions$ = args <= "/reactions"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.kegg_compound.network/"
        Dim maps = EntityObject.LoadDataSet([in]).ToArray
        Dim kegg_compounds As NamedValue(Of String)()
        Dim size$ = args("/size") Or "10000,7000"
        Dim allNames = maps.PropertyNames
        Dim enzyme = (args <= "/enzyme").loadEnzymeMaps

        If maps.Keys.Where(Function(id) id.IsPattern("C\d+")).Count > 1 Then
            kegg_compounds = maps _
                .Select(Function(item)
                            Return New NamedValue(Of String)(item.ID, item(allNames(Scan0)))
                        End Function) _
                .ToArray
        Else
            kegg_compounds = maps _
                .Select(Function(item)
                            Return New NamedValue(Of String)(item("KEGG"), item.ID)
                        End Function) _
                .ToArray
        End If

        kegg_compounds = kegg_compounds _
            .Where(Function(cpd)
                       ' KEGG代谢组学物质鉴定结果之中可能包含有
                       ' NA,即无鉴定结果的数据
                       ' 在这里需要做一下过滤
                       Return cpd.Name.IsPattern("C\d+")
                   End Function) _
            .ToArray

        Dim graph As FileStream.NetworkTables = reactions _
            .LoadCsv(Of ReactionTable) _
            .BuildModel(
                compounds:=kegg_compounds,
                extended:=args("/extended"),
                enzymes:=enzyme,
                enzymaticRelated:=args("/enzymeRelated")
            ).Tabular({"name", "is_extended"}).AnalysisDegrees

        Call graph.VisualizeKEGG(size:=size).SaveAs($"{out}/network.png")
        Call graph.Save(out)

        Return 0
    End Function

    <Extension> Private Function __distinctCommon(source As IEnumerable(Of PredictedRegulationFootprint)) As PredictedRegulationFootprint()
        Dim uids = (From x As PredictedRegulationFootprint In source
                    Let uid As String = $"{x.ORF}.{x.Regulator}.{x.MotifTrace}.{x.MotifId}"
                    Select x, uid
                    Group By uid Into Group).Select(Function(x) x.Group.First.x)
        Return uids
    End Function

    <ExportAPI("/Phenotypes.KEGG")>
    <Description("Regulator phenotype relationship cluster from virtual footprints.")>
    <Usage("/Phenotypes.KEGG /mods <KEGG_Modules/Pathways.DIR> /in <VirtualFootprints.csv> [/pathway /out <outCluster.csv>]")>
    <Group(CLIGrouping.KEGGPhenotype)>
    Public Function KEGGModulesPhenotypeRegulates(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim modsDIR As String = args("/mods")
        Dim isPathway As Boolean = args.GetBoolean("/pathway")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".PhenotypeRegulates.Csv")
        Dim footprints As PredictedRegulationFootprint() =
            inFile.LoadCsv(Of PredictedRegulationFootprint).__distinctCommon
        Dim loadMods As ModuleClassAPI =
            If(isPathway,
            ModuleClassAPI.FromPathway(modsDIR),
            ModuleClassAPI.FromModules(modsDIR))
        Dim regulators As String() =
            LinqAPI.Exec(Of String) <= From x As PredictedRegulationFootprint
                                       In footprints
                                       Select x.Regulator
                                       Distinct
                                       Order By Regulator Ascending
        Dim Entities = (From x As PathwayBrief
                        In loadMods.Modules
                        Let gs As String() = x.GetPathwayGenes
                        Select x.EntryId,
                            gs,
                            hash = regulators.ToDictionary(
                                Function(sId) sId,
                                Function(null) New Value(Of Integer))).ToArray

        For Each x In Entities
            For Each site As PredictedRegulationFootprint In footprints
                If Array.IndexOf(x.gs, site.ORF) > -1 Then
                    x.hash(site.Regulator).Value += 1
                End If
            Next
        Next

        Dim l As Integer = footprints.Length
        Dim sets As EntityClusterModel() = Entities.Select(
            Function(x) New EntityClusterModel With {
                .ID = x.EntryId,
                .Properties = x.hash.ToDictionary(Function(prop) prop.Key,
                                                  Function(prop) prop.Value.Value / l)
        }).ToArray

        Call sets.SaveTo(out.TrimSuffix & ".resultSet.Csv")

        ' 树形聚类
        Dim saveResult = sets.TreeCluster
        Return saveResult.SaveTo(out).CLICode
    End Function

    <ExportAPI("/net.model")>
    <Usage("/net.model /model <kegg.xmlModel.xml> [/out <outDIR> /not-trim]")>
    <Group(CLIGrouping.KEGGPhenotype)>
    Public Function BuildModelNet(args As CommandLine) As Integer
        Dim model As String = args("/model")
        Dim out As String = args.GetValue("/out", model.TrimSuffix & ".NET/")
        Dim bmods As XmlModel = model.LoadXml(Of XmlModel)
        Dim notTrim As Boolean = args.GetBoolean("/not-trim")
        Return ExportPathwayGraphFile(bmods, out, notTrim).CLICode
    End Function

    <ExportAPI("/net.pathway")>
    <Usage("/net.pathway /model <kegg.pathway.xml> [/out <outDIR> /trim]")>
    <Group(CLIGrouping.KEGGPhenotype)>
    Public Function PathwayNet(args As CommandLine) As Integer
        Dim model As String = args("/model")
        Dim out As String = args.GetValue("/out", model.TrimSuffix & ".NET/")
        Dim bmods As XmlModel = model.LoadXml(Of XmlModel)
        Dim trim As Boolean = args.GetBoolean("/trim")
        Return ExportPathwayGraphFile(bmods, out, trim).CLICode
    End Function

    ''' <summary>
    ''' 模块和模块之间的最简单的互做示意图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/modNET.Simple")>
    <Usage("/modNET.Simple /in <mods/pathway_DIR> [/out <outDIR> /pathway]")>
    <Group(CLIGrouping.KEGGPhenotype)>
    Public Function SimpleModesNET(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim outDIR As String = args.GetValue("/out", inDIR & "-SimpleModsNET/")
        Dim mods = If(args.GetBoolean("/pathway"),
            ModuleClassAPI.FromPathway(inDIR),
            ModuleClassAPI.FromModules(inDIR))
        Dim LQuery = (From x As PathwayBrief
                      In mods.Modules
                      Select x,
                          x.EntryId,
                          x.briteID,
                          x.GetPathwayGenes).ToArray
        Dim net As New FileStream.NetworkTables
        net += From x In LQuery
               Let props = New Dictionary(Of String, String) From {
                    {"A", mods.GetA(x.x)},
                    {"B", mods.GetB(x.x)},
                    {"C", mods.GetC(x.x)}
               }
               Select New FileStream.Node With {
                    .ID = x.EntryId,
                    .NodeType = "Module",
                    .Properties = props
               }

        For Each a In LQuery
            For Each b In LQuery
                Dim common As String() = a.GetPathwayGenes.Intersect(b.GetPathwayGenes).ToArray

                If Not common.IsNullOrEmpty Then
                    net += New FileStream.NetworkEdge With {
                        .value = common.Length,
                        .fromNode = a.EntryId,
                        .toNode = b.EntryId,
                        .interaction = "Interact",
                        .Properties = New Dictionary(Of String, String) From {
                            {"Genes", common.JoinBy("; ")}
                        }
                    }
                End If
            Next
        Next

        Dim graph As XGMMLgraph = ExportToFile.Export(net.nodes, net.edges, "KEGG pathway network simple")
        Call graph.Save(outDIR & "/Graph.XGMML", )

        Return net.Save(outDIR)   ' Write the network data to the filesystem.
    End Function
End Module
