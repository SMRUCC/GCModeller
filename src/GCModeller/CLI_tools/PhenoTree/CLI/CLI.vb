#Region "Microsoft.VisualBasic::96efa40446ea135cf237e1045925561c, ..\GCModeller\CLI_tools\PhenoTree\CLI\CLI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.AnalysisTools.SequenceTools.SequencePatterns
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DataMining.Framework.KMeans
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.COG

<PackageNamespace("Phenotype.Tree.CLI",
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
        Dim out As String = args.GetValue("/out", sites.TrimFileExt & $".{NameOf(GenePhenoClusters)}.tree.csv")
        Dim motifSites As IEnumerable(Of MotifLog) = sites.LoadCsv(Of MotifLog)
        Dim promoters = (From x As MotifLog
                         In motifSites.AsParallel
                         Where x.InPromoterRegion
                         Select x).ToArray
        Dim allProcess As String() = (From name In (From x As MotifLog
                                                    In promoters
                                                    Let names As String() = x.BiologicalProcess.Split(";"c).ToArray(Function(s) s.Trim)
                                                    Select (From ns As String
                                                            In names
                                                            Select bpName = ns,
                                                                ns.ToLower)).MatrixAsIterator
                                      Select name
                                      Group name By name.ToLower Into Group) _
                                           .ToArray(Function(x) x.Group.First.bpName) _
                                           .OrderBy(Function(s) s).ToArray
        Dim Genes = (From x As MotifLog
                     In promoters
                     Select x
                     Group x By x.ID Into Group)
        Dim Entity As New List(Of EntityLDM)

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

            Entity += New EntityLDM With {
                .Name = g.ID,
                .Properties = props
            }
        Next

        VBDebugger.Mute = False

        Dim parallel As Boolean = args.GetBoolean("/parallel")
        Dim result As List(Of EntityLDM) = Entity.TreeCluster(parallel).ToList
        Return result > out
    End Function

    <ExportAPI("/Parts.COGs",
               Usage:="/Parts.COGs /cluster <btree.clusters.csv> /myva <COGs.csv> [/depth <-1> /out <EXPORT_DIR>]")>
    Public Function PartitioningCOGs(args As CommandLine) As Integer
        Dim inFile As String = args - "/cluster"
        Dim depth As Integer = args.GetValue("/depth", -1)
        Dim EXPORT As String = args.GetValue("/out", inFile.TrimFileExt & $".depth={depth}/")
        Dim partitions As List(Of Partition) = inFile.LoadCsv(Of EntityLDM).Partitioning(depth)
        Dim myva As String = args <= "/myva"
        Dim COGs As MyvaCOG() = myva.LoadCsv(Of MyvaCOG)

        Call partitions.SaveTo(EXPORT & "/Partitions.Csv")

        For Each part As Partition In partitions
            Dim myvaCogs = (From x As MyvaCOG In COGs
                            Where Array.IndexOf(part.uids, x.QueryName) > -1
                            Select x).ToList
            Dim func As COG.Function = COG.Function.Default
            Dim stst = COGFunc.GetClass(myvaCogs, func)
            Dim out As String = EXPORT & $"/COGs-{part.Tag}.Csv"

            Call stst.SaveTo(out)
        Next

        Return 0
    End Function
End Module

