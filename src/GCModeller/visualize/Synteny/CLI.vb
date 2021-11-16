#Region "Microsoft.VisualBasic::979503474740f7ecfa7194cc6ea140dc, visualize\Synteny\CLI.vb"

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
    '     Function: ClusterTree, PlotMapping
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Analysis.SequenceTools.ClusterMatrix
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeGenomics

<CLI> Module CLI

    ''' <summary>
    ''' Plot of the blastn mapping result
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 对于常见的fasta标题，可以使用脚本``tokens | first``
    ''' </remarks>
    <ExportAPI("/mapping.plot")>
    <Usage("/mapping.plot /mapping <blastn_mapping.csv> /query <query.gff3> /ref <subject.gff3> [/Ribbon <default=Spectral:c6> /size <default=6000,4000> /auto.reverse <default=0.9> /grep <default=""-""> /out <Synteny.png>]")>
    Public Function PlotMapping(args As CommandLine) As Integer
        Dim in$ = args <= "/mapping"
        Dim query$ = args <= "/query"
        Dim ref$ = args <= "/ref"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.synteny.png"
        Dim mappings = [in].LoadCsv(Of BlastnMapping).ToArray
        Dim queryGff = GFF.Load(query)
        Dim refGff = GFF.Load(ref)
        Dim grep As TextGrepScriptEngine = TextGrepScriptEngine.Compile(args("/grep"))
        Dim plotModel As DrawingModel = (queryGff, refGff) _
            .SyntenyTuple _
            .LinkFromBlastnMaps(
                maps:=mappings,
                grepOp:=grep,
                ribbonColors:=args("/ribbon") Or "Spectral:c6"
            ) _
            .AutoReverse(args("/auto.reverse") Or 0.9)

        Return New DrawingDevice() _
            .Plot(plotModel,
                  canvasSize:=args("/size") Or "6000,3000",
                  margin:="padding: 300px 100px 1200px 100px"
            ) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/cluster.tree")>
    <Usage("/cluster.tree /in <besthit.csv> /genomes <fasta.directory> [/out <clusters.csv>]")>
    Public Function ClusterTree(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.conservative_clusters.csv"
        Dim localblast As BestHit() = [in].LoadCsv(Of BestHit).ToArray
        Dim genomes = (args <= "/genomes") _
            .EnumerateFiles("*.faa") _
            .Select(Iterator Function(file)
                        Dim fasta As FastaFile = FastaFile.Read(file)
                        Dim descript As NamedValue(Of String)() = fasta _
                            .Select(Function(fa)
                                        Return Strings.Trim(fa.Title).GetTagValue()
                                    End Function) _
                            .ToArray
                        Dim genomeName$ = file.BaseName

                        For Each describ As NamedValue(Of String) In descript
                            Yield New NamedValue(Of String)(describ.Name) With {
                                .Value = describ.Value,
                                .Description = genomeName
                            }
                        Next
                    End Function) _
            .IteratesALL _
            .Where(Function(gene) Not gene.Name.StringEmpty) _
            .GroupBy(Function(gene) gene.Name) _
            .ToDictionary(Function(gene) gene.Key,
                          Function(group)
                              If group.Count > 1 Then
                                  Call group.Key.Warning
                              End If

                              Return group.First
                          End Function)
        Dim proteins = genomes.Keys.ToArray
        Dim clusters = proteins.BuildTree(localblast)

        For Each cluster As Cluster In clusters
            cluster.genomes = cluster.Members _
                .Select(Function(id) genomes(id).Description) _
                .Distinct _
                .ToArray
        Next

        Return clusters.SaveTo(out).CLICode
    End Function
End Module
