#Region "Microsoft.VisualBasic::5576e617451aff23ebe6ce34488f6598, seqtoolkit\Annotations\genomics.vb"

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

    ' Module genomics
    ' 
    '     Function: asPTT, asTable, genes, getUpstream, getUpStream
    '               PTT2Dump, readGtf, writePPTTabular
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("annotation.genomics", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
<RTypeExport("gene_info", GetType(GeneBrief))>
Module genomics

    <ExportAPI("read.gtf")>
    Public Function readGtf(file As String) As GeneBrief()
        Return Gtf.ParseFile(file)
    End Function

    <ExportAPI("as.tabular")>
    Public Function asTable(genes As GeneBrief(),
                            Optional title$ = "n/a",
                            Optional size% = 0,
                            Optional format$ = "PTT|GFF|GTF",
                            Optional env As Environment = Nothing) As Object

        Select Case Strings.UCase(format).Split("|"c).FirstOrDefault
            Case "PTT"
                Return New PTT(genes, title, size)
            Case "GFF"
                Return Internal.debug.stop(New NotImplementedException, env)
            Case "GTF"
                Return Internal.debug.stop(New NotImplementedException, env)
            Case Else
                Return Internal.debug.stop($"unsupported table format: '{format}'!", env)
        End Select
    End Function

    <ExportAPI("as.geneTable")>
    Public Function PTT2Dump(PTT As PTT) As GeneTable()
        Return GenBank.ExportPTTAsDump(PTT)
    End Function

    <ExportAPI("as.PTT")>
    Public Function asPTT(gb As GBFF.File) As PTT
        Return gb.GbffToPTT
    End Function

    <ExportAPI("upstream")>
    Public Function getUpstream(<RRawVectorArgument>
                                context As GeneBrief(),
                                Optional length% = 200,
                                Optional isRelativeOffset As Boolean = False) As NucleotideLocation()
        Return context _
            .Select(Function(gene)
                        Return gene.getUpStream(length, isRelativeOffset)
                    End Function) _
            .ToArray
    End Function

    <Extension>
    Private Function getUpStream(gene As GeneBrief, length As Integer, isRelativeOffset As Boolean) As NucleotideLocation
        Dim loci As NucleotideLocation = gene.Location

        If isRelativeOffset Then
            If loci.Strand = Strands.Forward Then
                loci = New NucleotideLocation(loci.left - length, loci.left, Strands.Forward) With {
                    .tag = loci.ToString & $"|offset=-{length}"
                }
            Else
                loci = New NucleotideLocation(loci.right, loci.right + length, Strands.Reverse) With {
                    .tag = loci.ToString & $"|offset=+{length}"
                }
            End If
        Else
            If loci.Strand = Strands.Forward Then
                loci = loci - length
            Else
                loci = loci + length
            End If
        End If

        Return loci
    End Function

    <ExportAPI("genome.genes")>
    <RApiReturn(GetType(GeneBrief))>
    Public Function genes(<RRawVectorArgument> genome As Object, Optional env As Environment = Nothing) As Object
        If genome Is Nothing Then
            Return {}
        End If

        If TypeOf genome Is PTT Then
            Return DirectCast(genome, PTT).GeneObjects
        ElseIf TypeOf genome Is GBFF.File Then
            Return DirectCast(genome, GBFF.File).EnumerateGeneFeatures(ORF:=False).FeatureGenes.ToArray
        Else
            Return Internal.debug.stop($"Invalid genome context model: {genome.GetType.FullName}!", env)
        End If
    End Function

    <ExportAPI("write.PTT_tabular")>
    Public Function writePPTTabular(<RRawVectorArgument>
                                    genomics As Object,
                                    Optional file$ = Nothing,
                                    Optional encoding As Encodings = Encodings.ASCII,
                                    Optional env As Environment = Nothing) As Object
        Dim dev As StreamWriter

        If file.StringEmpty Then
            ' std_output
            dev = App.StdOut.DefaultValue
        Else
            dev = file.OpenWriter(encoding)
        End If

        If genomics Is Nothing Then
            Return Internal.debug.stop("the required genomics context data can not be nothing!", env)
        End If

        If TypeOf genomics Is PTT Then
            Call DirectCast(genomics, PTT).WriteDocument(dev)
        Else
            Dim geneStream As pipeline = pipeline.TryCreatePipeline(Of GeneBrief)(genomics, env)

            If geneStream.isError Then
                Return geneStream.getError
            End If

            Call dev.WriteTabular(geneStream.populates(Of GeneBrief)(env))

            If geneStream.isError Then
                Return geneStream.getError
            End If
        End If

        Call dev.Flush()

        If Not file.StringEmpty Then
            Call dev.Dispose()
        End If

        Return 0
    End Function
End Module
