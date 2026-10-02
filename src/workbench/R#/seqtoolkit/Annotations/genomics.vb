#Region "Microsoft.VisualBasic::77f82e6fbe753fca0ec1a7963aeaa4ca, R#\seqtoolkit\Annotations\genomics.vb"

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

'   Total Lines: 258
'    Code Lines: 183 (70.93%)
' Comment Lines: 43 (16.67%)
'    - Xml Docs: 95.35%
' 
'   Blank Lines: 32 (12.40%)
'     File Size: 9.72 KB


' Module genomics
' 
'     Function: asPTT, asTable, extract_gff_seqs, genes, getUpstream
'               getUpStream, gff_features, operon_set, PTT2Dump, read_nucmer
'               readGff, readGtf, SourceFeatures, write_gff3, writePPTTabular
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Annotation
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.SyntenyVisualize
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Genomics context data annotation toolkit
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the api for read/write and manipulate the 
''' genomics context annotation data:
''' 
''' + read the genomics context annotation table file: ``read.gff``, 
'''   ``read.gtf``, ``read.nucmer``;
''' + export the genomics context annotation data: ``write.gff3``, 
'''   ``as.tabular``, ``write.PTT_tabular``;
''' + query the genomics context features: ``gff_features``, 
'''   ``source_features``, ``type_features``, ``genes_features``, ``upstream``;
''' + extract the sequence region data: ``extract_gff_seqs``.
''' </remarks>
<Package("annotation.genomics", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
<RTypeExport("gene_info", GetType(GeneBrief))>
Module genomics

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(Feature()), AddressOf as_featureTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function as_featureTable(features As Feature(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = features.Keys
        }

        Call df.add(NameOf(Feature.seqname), From fi As Feature In features Select fi.seqname)
        Call df.add(NameOf(Feature.source), From fi As Feature In features Select fi.source)
        Call df.add(NameOf(Feature.feature), From fi As Feature In features Select fi.feature)
        Call df.add(NameOf(Feature.synonym), From fi As Feature In features Select fi.synonym)
        Call df.add(NameOf(Feature.start), From fi As Feature In features Select fi.start)
        Call df.add(NameOf(Feature.ends), From fi As Feature In features Select fi.ends)
        Call df.add("length", From fi As Feature In features Select fi.Length)
        Call df.add(NameOf(Feature.strand), From fi As Feature In features Select fi.strand.Description)
        Call df.add(NameOf(Feature.score), From fi As Feature In features Select fi.score)
        Call df.add(NameOf(Feature.frame), From fi As Feature In features Select fi.frame)
        Call df.add(NameOf(Feature.comments), From fi As Feature In features Select fi.comments)

        Dim attrCols As String() = features _
            .Select(Function(fi) fi.attributes.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray

        For Each key As String In attrCols
            Call df.add(key, From fi As Feature
                             In features
                             Select fi.attributes.TryGetValue(key))
        Next

        Return df
    End Function

    ''' <summary>
    ''' read the gtf format genomics context annotation table file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the gtf format genomics context annotation table 
    ''' file.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="GeneBrief"/> gene annotation object that 
    ''' loaded from the given gtf table file.
    ''' </returns>
    <ExportAPI("read.gtf")>
    Public Function readGtf(file As String) As GeneBrief()
        Return Gtf.ParseFile(file)
    End Function

    ''' <summary>
    ''' read the gff3 file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the gff3 format genomics context annotation table 
    ''' file.
    ''' </param>
    ''' <returns>
    ''' a <see cref="GFFTable"/> genomics feature annotation table object that 
    ''' loaded from the given gff3 table file.
    ''' </returns>
    <ExportAPI("read.gff")>
    Public Function readGff(file As String) As GFFTable
        Return GFFTable.LoadDocument(file)
    End Function

    ''' <summary>
    ''' save the genomics feature annotation table object as a gff3 format 
    ''' table file
    ''' </summary>
    ''' <param name="gff">
    ''' a <see cref="GFFTable"/> genomics feature annotation table object for 
    ''' save to the target file.
    ''' </param>
    ''' <param name="file">
    ''' the file path of the generated gff3 format table file.
    ''' </param>
    ''' <returns>
    ''' a boolean value of the file save result: TRUE means the genomics 
    ''' feature annotation data has been written into the target file 
    ''' successfully.
    ''' </returns>
    <ExportAPI("write.gff3")>
    Public Function write_gff3(gff As GFFTable, file As String) As Boolean
        Return gff.Save(file)
    End Function

    ''' <summary>
    ''' get the genomics features from the given gff table by its source name
    ''' </summary>
    ''' <param name="gff">
    ''' a <see cref="GFFTable"/> genomics feature annotation table object for 
    ''' query the feature data.
    ''' </param>
    ''' <param name="source">
    ''' the source name of the target features, example as ``ena``.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="Feature"/> genomics feature object that its 
    ''' source name matches the given source name.
    ''' </returns>
    <ExportAPI("source_features")>
    <RApiReturn(GetType(Feature))>
    Public Function SourceFeatures(gff As GFFTable, source As String) As Object
        Return gff.FilterBySource(source).ToArray
    End Function

    ''' <summary>
    ''' get the genomics features from the given gff table by its feature type
    ''' </summary>
    ''' <param name="gff">
    ''' a <see cref="GFFTable"/> genomics feature annotation table object for 
    ''' query the feature data.
    ''' </param>
    ''' <param name="type">
    ''' the feature type of the target features, example as ``gene``, ``CDS`` 
    ''' or ``mRNA``.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="Feature"/> genomics feature object that its 
    ''' feature type matches the given type name.
    ''' </returns>
    <ExportAPI("type_features")>
    <RApiReturn(GetType(Feature))>
    Public Function SourceType(gff As GFFTable, type As String) As Object
        Return gff.FilterByType(type).ToArray
    End Function

    ''' <summary>
    ''' get gff features by id reference
    ''' </summary>
    ''' <param name="gff">
    ''' a <see cref="GFFTable"/> genomics feature annotation table object for 
    ''' query the feature data.
    ''' </param>
    ''' <param name="id">
    ''' a character vector of the feature id for get the subset of the target 
    ''' features. If this parameter is not specified, then all of the features 
    ''' inside the given gff table will be returned.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="Feature"/> genomics feature object.
    ''' </returns>
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

    ''' <summary>
    ''' export the gene annotation data as the tabular format table object
    ''' </summary>
    ''' <param name="genes">
    ''' a vector of the <see cref="GeneBrief"/> gene annotation object for 
    ''' export as the tabular table.
    ''' </param>
    ''' <param name="title">
    ''' the title text of the generated tabular table.
    ''' </param>
    ''' <param name="size">
    ''' the genome size in bp of the generated tabular table.
    ''' </param>
    ''' <param name="format">
    ''' the tabular table format of the generated output. Only the ``PTT`` 
    ''' format is implemented at this moment.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="PTT"/> tabular table object of the given gene annotation 
    ''' data;
    ''' 
    ''' this function returns a R# error message object if the given table 
    ''' format is not supported.
    ''' </returns>
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

    ''' <summary>
    ''' export the PTT tabular table object as a collection of the gene table 
    ''' records
    ''' </summary>
    ''' <param name="PTT">
    ''' a <see cref="PTT"/> tabular table object for export as the gene table 
    ''' records.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="GeneTable"/> gene table record object.
    ''' </returns>
    <ExportAPI("as.geneTable")>
    Public Function PTT2Dump(PTT As PTT) As GeneTable()
        Return GenBank.ExportPTTAsDump(PTT)
    End Function

    ''' <summary>
    ''' export the genbank database file object as a PTT tabular table object
    ''' </summary>
    ''' <param name="gb">
    ''' a <see cref="GBFF.File"/> ncbi genbank database file object for export 
    ''' as the PTT tabular table.
    ''' </param>
    ''' <returns>
    ''' a <see cref="PTT"/> tabular table object of the gene annotation data 
    ''' in the given genbank database file.
    ''' </returns>
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
    ''' <returns>
    ''' a vector of the <see cref="NucleotideLocation"/> upstream region 
    ''' location of each gene in the given gene context data collection.
    ''' </returns>
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

    ''' <summary>
    ''' Extract all gene features from a given genomics context assembly data
    ''' </summary>
    ''' <param name="genome">
    ''' the genomics context assembly data for extract its gene features, 
    ''' which can be a <see cref="PTT"/> tabular table object or a 
    ''' <see cref="GBFF.File"/> ncbi genbank database file object.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="GeneBrief"/> gene annotation object;
    ''' 
    ''' this function returns a R# error message object if the given genomics 
    ''' context data is not a supported data model.
    ''' </returns>
    <ExportAPI("genes_features")>
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

    ''' <summary>
    ''' write the genomics context annotation data as the PTT tabular format 
    ''' table file
    ''' </summary>
    ''' <param name="genomics">
    ''' the genomics context annotation data for write, which can be a 
    ''' <see cref="GBFF.File"/> ncbi genbank database file object, a 
    ''' <see cref="PTT"/> tabular table object, or a collection of the 
    ''' <see cref="GeneBrief"/> gene annotation object.
    ''' </param>
    ''' <param name="file">
    ''' the file path of the generated PTT tabular table file. If this 
    ''' parameter is not specified, then the table data will be written into 
    ''' the standard output console.
    ''' </param>
    ''' <param name="encoding">
    ''' the text encoding value of the generated PTT tabular table file.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a ZERO number value when the tabular table data has been written 
    ''' successfully;
    ''' 
    ''' this function returns a R# error message object if the given genomics 
    ''' context data is nothing or can not be cast to a supported data model.
    ''' </returns>
    <ExportAPI("write.PTT_tabular")>
    Public Function writePPTTabular(<RRawVectorArgument>
                                    genomics As Object,
                                    Optional file$ = Nothing,
                                    Optional encoding As Encodings = Encodings.ASCII,
                                    Optional env As Environment = Nothing) As Object
        Dim dev As System.IO.StreamWriter

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
    ''' read the nucmer alignment delta format file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the nucmer alignment delta format file.
    ''' </param>
    ''' <returns>
    ''' a <see cref="DeltaFile"/> nucmer alignment result object that loaded 
    ''' from the given delta file.
    ''' </returns>
    <ExportAPI("read.nucmer")>
    Public Function read_nucmer(file As String) As DeltaFile
        Return DeltaFile.LoadDocument(file)
    End Function

    ''' <summary>
    ''' extract the sequence region data of the genomics features in the given 
    ''' gff3 table from the reference sequence data
    ''' </summary>
    ''' <param name="gff3">
    ''' a <see cref="GFFTable"/> genomics feature annotation table object, 
    ''' which provides the location region information of the target sequence 
    ''' extraction.
    ''' </param>
    ''' <param name="seqs">
    ''' the reference sequence data source for extract the feature region 
    ''' sequence, which can be a <see cref="FastaFile"/> object, a collection 
    ''' of the <see cref="FastaSeq"/> object, or a character vector of the raw 
    ''' sequence data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="FastaSeq"/> feature region sequence object;
    ''' 
    ''' this function returns NULL if the given reference sequence data can 
    ''' not be cast to a fasta sequence collection.
    ''' </returns>
    <ExportAPI("extract_gff_seqs")>
    Public Function extract_gff_seqs(gff3 As GFFTable, <RRawVectorArgument> seqs As Object, Optional env As Environment = Nothing) As Object
        Dim pull As IEnumerable(Of FastaSeq) = GetFastaSeq(seqs, env)

        If pull Is Nothing Then
            Return Nothing
        Else
            Return gff3.ExtractSequence(pull).ToArray
        End If
    End Function

End Module
