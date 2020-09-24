#Region "Microsoft.VisualBasic::1d2742ff1010757a238cd8b51db966f2, CLI_tools\PhenoTree\CLI\CLI.vb"

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
    '     Function: ClusterEnrichment, GenePhenoClusters, PartitioningCOGs, TreePartitions
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports RDotNet.Extensions.VisualBasic.API
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.COG
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG

<Package("Phenotype.Tree.CLI",
                  Category:=APICategories.CLI_MAN,
                  Description:="Cellular phenotype analysis tools.")>
Public Module CLI

    ''' <summary>
    ''' 做出每一个基因的调控位点之后，对全基因组做树形聚类，然后了解每一个聚类之中的基因的表型的相关性
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Cluster.Genes.Phenotype",
               Usage:="/Cluster.Genes.Phenotype /sites <motifSites.csv> [/out <out.tree_cluster.csv> /parallel]")>
    Public Function GenePhenoClusters(args As CommandLine) As Integer
        Dim sites As String = args - "/sites"
        Dim out As String = args.GetValue("/out", sites.TrimSuffix & $".{NameOf(GenePhenoClusters)}.tree.csv")
        Dim motifSites As IEnumerable(Of MotifLog) = sites.LoadCsv(Of MotifLog)
        Dim promoters = (From x As MotifLog
                         In motifSites.AsParallel
                         Where x.InPromoterRegion
                         Select x).ToArray
        Dim allProcess As String() = (From name In (From x As MotifLog
                                                    In promoters
                                                    Let names As String() = x.BiologicalProcess.Split(";"c).Select(Function(s) s.Trim)
                                                    Select (From ns As String
                                                            In names
                                                            Select bpName = ns,
                                                                ns.ToLower)).IteratesALL
                                      Select name
                                      Group name By name.ToLower Into Group) _
                                           .Select(Function(x) x.Group.First.bpName) _
                                           .OrderBy(Function(s) s).ToArray
        Dim Genes = (From x As MotifLog
                     In promoters
                     Select x
                     Group x By x.ID Into Group)
        Dim Entity As New List(Of EntityClusterModel)

        VBDebugger.Mute = True

        For Each g In Genes
            Dim hash As Dictionary(Of String, Integer) = (From x As MotifLog
                                                          In g.Group
                                                          Select x
                                                          Group x By x.BiologicalProcess.Trim Into Count) _
                                                               .ToDictionary(Function(x) x.Trim,
                                                                             Function(x) x.Count)
            Dim sum As Integer = hash.Sum(Function(x) x.Value)
            Dim props As Dictionary(Of String, Double) =
                allProcess.ToDictionary(Function(name) name,
                                        Function(name) hash.TryGetValue(name) / sum)

            Entity += New EntityClusterModel With {
                .ID = g.ID,
                .Properties = props
            }
        Next

        VBDebugger.Mute = False

        Dim parallel As Boolean = args.GetBoolean("/parallel")
        Dim result As List(Of EntityClusterModel) = Entity.TreeCluster(parallel).AsList
        Return result > out
    End Function

    <ExportAPI("/Tree.Partitions", Usage:="/Tree.Partitions /in <btree.network.DIR> [/quantile <0.99> /out <out.DIR>]")>
    Public Function TreePartitions(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim quantile = args.GetValue("/quantile", 0.99)
        Dim out = args.GetValue("/out", [in].TrimDIR & $".cuts,quantile={quantile}/")
        Dim net As NetworkTables = NetworkFileIO.Load([in])
        Dim parts As Partition() = net.BuildTree.CutTrees(quantile).ToArray
        Dim json = parts.PartionTable
        Dim colors As Color() = Designer.GetColors("vb.chart", json.Count + 1)
        Dim memberColors = LinqAPI.Exec(Of Map(Of Index(Of String), String)) <=
 _
            From cluster
            In json.SeqIterator
            Let i = cluster.i
            Let members = cluster.value.Value
            Let color As Color = colors(i)
            Let index = New Index(Of String)(members.Select(Function(x) x.ID))
            Select New Map(Of Index(Of String), String) With {
                .Key = index,
                .Maps = color.ToHtmlColor
            }

        For Each node As Node In net.nodes
            For Each cluster In memberColors
                If cluster.Key.IndexOf(node.ID) > -1 Then
                    node.Add("color", cluster.Maps)
                    Exit For
                End If
            Next
        Next

        Call net.Save(out, Encodings.ASCII.CodePage)

        Return json.GetJson(True).SaveTo(out & "/clusters.json").CLICode
    End Function

    <ExportAPI("/Cluster.Enrichment",
               Usage:="/Cluster.Enrichment /in <partitions.json> /go.anno <proteins.go.annos.csv> [/go.brief <go_brief.csv> /out <out.DIR>]")>
    Public Function ClusterEnrichment(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim anno As String = args("/go.anno")
        Dim goBrief As String = args.GetValue("/go.brief", GCModeller.FileSystem.GO & "/go_brief.csv")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".go.enrichment/")
        Dim GO_anno As IO.File = IO.File.LoadTsv(anno, Encodings.UTF8)
        '  Dim term2gene$ = GO_anno.PushAsTable(False)
        ' Dim go2name$ = clusterProfiler.LoadGoBriefTable(IO.File.Load(goBrief))
        Dim clusters = [in].ReadAllText.LoadJSON(Of Dictionary(Of String, EntityClusterModel()))

        For Each cluster In clusters.Values
            Dim genes$() = cluster.Select(Function(g) g.ID.Split.First).Distinct.ToArray
            Dim list$ = base.c(genes, stringVector:=True)
            '   Dim enrichment = clusterProfiler.enricher(list, "NULL", term2gene, TERM2NAME:=go2name)

            Call genes.SaveTo("x:/test.txt")
        Next
    End Function

    <ExportAPI("/Parts.COGs",
               Usage:="/Parts.COGs /cluster <btree.clusters.csv> /myva <COGs.csv> [/depth <-1> /out <EXPORT_DIR>]")>
    Public Function PartitioningCOGs(args As CommandLine) As Integer
        Dim inFile As String = args - "/cluster"
        Dim depth As Integer = args.GetValue("/depth", -1)
        Dim EXPORT As String = args.GetValue("/out", inFile.TrimSuffix & $".depth={depth}/")
        Dim partitions As List(Of Partition) = inFile.LoadCsv(Of EntityClusterModel).Partitioning(depth)
        Dim myva As String = args <= "/myva"
        Dim COGs As MyvaCOG() = myva.LoadCsv(Of MyvaCOG)

        Call partitions.SaveTo(EXPORT & "/Partitions.Csv")

        For Each part As Partition In partitions
            Dim myvaCogs = (From x As MyvaCOG In COGs
                            Where Array.IndexOf(part.uids, x.QueryName) > -1
                            Select x).AsList
            Dim func As COG.Function = COG.Function.Default
            Dim stst = COGFunction.GetClass(myvaCogs, func)
            Dim out As String = EXPORT & $"/COGs-{part.Tag}.Csv"

            Call stst.SaveTo(out)
        Next

        Return 0
    End Function
End Module
