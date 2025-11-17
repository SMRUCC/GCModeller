#Region "Microsoft.VisualBasic::15f6d92030985c9fa2786adb4f2d5e7e, R#\seqtoolkit\Annotations\genomics.vb"

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

    '   Total Lines: 175
    '    Code Lines: 132 (75.43%)
    ' Comment Lines: 21 (12.00%)
    '    - Xml Docs: 90.48%
    ' 
    '   Blank Lines: 22 (12.57%)
    '     File Size: 6.71 KB


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
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Model.OperonMapper
Imports SMRUCC.genomics.Visualize.SyntenyVisualize
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("annotation.genomics", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
<RTypeExport("gene_info", GetType(GeneBrief))>
Module genomics

    <ExportAPI("read.gtf")>
    Public Function readGtf(file As String) As GeneBrief()
        Return Gtf.ParseFile(file)
    End Function

    <ExportAPI("read.gff")>
    Public Function readGff(file As String) As GFFTable
        Return GFFTable.LoadDocument(file)
    End Function

    ''' <summary>
    ''' get gff features by id reference
    ''' </summary>
    ''' <param name="gff"></param>
    ''' <param name="id"></param>
    ''' <returns></returns>
    <ExportAPI("gff_features")>
    Public Function gff_features(gff As GFFTable, <RRawVectorArgument> Optional id As Object = Nothing) As Object
        If id Is Nothing Then
            Return gff.features
        Else
            Dim index As Dictionary(Of String, Feature) = gff.CreateGeneObjectIndex
            Dim idset As String() = CLRVector.asCharacter(id)
            Dim subset As Feature() = idset.Select(Function(sid) index(sid)).ToArray

            Return subset
        End If
    End Function

    <ExportAPI("as.tabular")>
    <RApiReturn(GetType(PTT))>
    Public Function asTable(genes As GeneBrief(),
                            Optional title$ = "n/a",
                            Optional size% = 0,
                            Optional format$ = "PTT|GFF|GTF",
                            Optional env As Environment = Nothing) As Object

        Select Case Strings.UCase(format).Split("|"c).FirstOrDefault
            Case "PTT"
                Return New PTT(genes, title, size)
            Case "GFF"
                Return RInternal.debug.stop(New NotImplementedException, env)
            Case "GTF"
                Return RInternal.debug.stop(New NotImplementedException, env)
            Case Else
                Return RInternal.debug.stop($"unsupported table format: '{format}'!", env)
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

    ''' <summary>
    ''' Create the upstream location
    ''' </summary>
    ''' <param name="context">th gene element location context data</param>
    ''' <param name="length">bit length of the upstream location</param>
    ''' <param name="is_relative_offset">
    ''' Does the generates context upstream location is relative to the 
    ''' given context start position or the enitre context region move
    ''' by upstream offset bits?
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("upstream")>
    Public Function getUpstream(<RRawVectorArgument>
                                context As GeneBrief(),
                                Optional length% = 200,
                                Optional is_relative_offset As Boolean = True) As NucleotideLocation()
        Return context _
            .Select(Function(gene)
                        Return gene.getUpStream(length, is_relative_offset)
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gene"></param>
    ''' <param name="length"></param>
    ''' <param name="isRelativeOffset">
    ''' the generated region location is relative to the given context its start position?
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Private Function getUpStream(gene As GeneBrief, length As Integer, isRelativeOffset As Boolean) As NucleotideLocation
        Dim loci As NucleotideLocation = gene.Location

        If isRelativeOffset Then
            If loci.Strand = Strands.Forward Then
                loci = New NucleotideLocation(loci.left - length, loci.left, Strands.Forward) With {
                    .tagStr = loci.ToString & $"|offset=-{length}"
                }
            Else
                loci = New NucleotideLocation(loci.right, loci.right + length, Strands.Reverse) With {
                    .tagStr = loci.ToString & $"|offset=+{length}"
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
            Return RInternal.debug.stop($"Invalid genome context model: {genome.GetType.FullName}!", env)
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
            Return RInternal.debug.stop("the required genomics context data can not be nothing!", env)
        ElseIf TypeOf genomics Is GBFF.File Then
            genomics = DirectCast(genomics, GBFF.File).GbffToPTT(ORF:=False)
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

    ''' <summary>
    ''' load operon set data from the ODB database
    ''' </summary>
    ''' <param name="file">dataset text file that download from https://operondb.jp/</param>
    ''' <returns></returns>
    <ExportAPI("operon_set")>
    Public Function operon_set(Optional file As String = Nothing) As OperonRow()
        If file.StringEmpty(, True) Then
            Return OperonRow.LoadInternalResource.ToArray
        Else
            Return OperonRow.Load(file).ToArray
        End If
    End Function

    <ExportAPI("read.nucmer")>
    Public Function read_nucmer(file As String) As DeltaFile
        Return DeltaFile.LoadDocument(file)
    End Function
End Module
