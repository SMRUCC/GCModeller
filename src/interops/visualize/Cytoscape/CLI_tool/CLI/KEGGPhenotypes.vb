#Region "Microsoft.VisualBasic::7312f6615fca23295f97a8e71f2cdadf, ..\interops\visualize\Cytoscape\CLI_tool\CLI\KEGGPhenotypes.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Serialization
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
Imports SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET

Partial Module CLI

    <Extension> Private Function __distinctCommon(source As IEnumerable(Of PredictedRegulationFootprint)) As PredictedRegulationFootprint()
        Dim uids = (From x As PredictedRegulationFootprint In source
                    Let uid As String = $"{x.ORF}.{x.Regulator}.{x.MotifTrace}.{x.MotifId}"
                    Select x, uid
                    Group By uid Into Group).ToArray(Function(x) x.Group.First.x)
        Return uids
    End Function

    <ExportAPI("/Phenotypes.KEGG",
               Info:="Regulator phenotype relationship cluster from virtual footprints.",
               Usage:="/Phenotypes.KEGG /mods <KEGG_Modules/Pathways.DIR> /in <VirtualFootprints.csv> [/pathway /out <outCluster.csv>]")>
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
        Dim sets As EntityLDM() = Entities.ToArray(
            Function(x) New EntityLDM With {
                .Name = x.EntryId,
                .Properties = x.hash.ToDictionary(Function(prop) prop.Key,
                                                  Function(prop) prop.Value.Value / l)
        })

        Call sets.SaveTo(out.TrimSuffix & ".resultSet.Csv")

        ' 树形聚类
        Dim saveResult = sets.TreeCluster
        Return saveResult.SaveTo(out).CLICode
    End Function

    <ExportAPI("/net.model", Usage:="/net.model /model <kegg.xmlModel.xml> [/out <outDIR> /not-trim]")>
    <Group(CLIGrouping.KEGGPhenotype)>
    Public Function BuildModelNet(args As CommandLine) As Integer
        Dim model As String = args("/model")
        Dim out As String = args.GetValue("/out", model.TrimSuffix & ".NET/")
        Dim bmods As XmlModel = model.LoadXml(Of XmlModel)
        Dim notTrim As Boolean = args.GetBoolean("/not-trim")
        Return ExportPathwayGraphFile(bmods, out, notTrim).CLICode
    End Function

    <ExportAPI("/net.pathway", Usage:="/net.pathway /model <kegg.pathway.xml> [/out <outDIR> /trim]")>
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
    <ExportAPI("/modNET.Simple",
               Usage:="/modNET.Simple /in <mods/pathway_DIR> [/out <outDIR> /pathway]")>
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
                          x.BriteId,
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
                        .FromNode = a.EntryId,
                        .ToNode = b.EntryId,
                        .Interaction = "Interact",
                        .Properties = New Dictionary(Of String, String) From {
                            {"Genes", common.JoinBy("; ")}
                        }
                    }
                End If
            Next
        Next

        Dim graph As Graph = ExportToFile.Export(net.Nodes, net.Edges, "KEGG pathway network simple")
        Call graph.Save(outDIR & "/Graph.XGMML", )

        Return net > outDIR   ' Write the network data to the filesystem.
    End Function
End Module
