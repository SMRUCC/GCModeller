#Region "Microsoft.VisualBasic::0fc8cf48b53f28d8fc7e56cc60cfab0d, R#\seqtoolkit\Annotations\genbankKit.vb"

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

    '   Total Lines: 419
    '    Code Lines: 295
    ' Comment Lines: 70
    '   Blank Lines: 54
    '     File Size: 16.83 KB


    ' Module genbankKit
    ' 
    '     Function: addFeature, addMeta, addproteinSeq, addRNAGene, asGenbank
    '               createFeature, enumerateFeatures, featureMeta, getOrAddNtOrigin, getRNASeq
    '               isPlasmidSource, keyNames, populateGenbanks, readGenbank, writeGenbank
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports featureLocation = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Location
Imports gbffFeature = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' NCBI genbank assembly file I/O toolkit
''' </summary>
<Package("GenBank", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Module genbankKit

    ''' <summary>
    ''' read the given genbank assembly file.
    ''' </summary>
    ''' <param name="file">the file path of the given genbank assembly file.</param>
    ''' <param name="repliconTable"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.genbank")>
    Public Function readGenbank(file As String,
                                Optional repliconTable As Boolean = False,
                                Optional env As Environment = Nothing) As Object

        If Not file.FileExists(True) Then
            Return Internal.debug.stop($"invalid file resource: '{file}'!", env)
        End If

        If repliconTable Then
            Return GenBank.loadRepliconTable(file)
        Else
            Return GBFF.File.Load(file)
        End If
    End Function

    <ExportAPI("is.plasmid")>
    <RApiReturn(GetType(Boolean))>
    Public Function isPlasmidSource(<RRawVectorArgument> gb As Object, Optional env As Environment = Nothing) As Object
        If gb Is Nothing Then
            Return Nothing
        ElseIf TypeOf gb Is list Then
            Return DirectCast(gb, list) _
                .AsGeneric(Of GBFF.File)(env) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CObj(a.Value.isPlasmid)
                              End Function) _
                .DoCall(Function(data)
                            Return New list With {
                                .slots = data
                            }
                        End Function)
        Else
            Dim source As pipeline = pipeline.TryCreatePipeline(Of GBFF.File)(gb, env)

            If source.isError Then
                Return source.getError
            End If

            Return source _
                .populates(Of GBFF.File)(env) _
                .Select(Function(a) a.isPlasmid) _
                .DoCall(AddressOf vector.asVector)
        End If
    End Function

    ''' <summary>
    ''' populate a list of genbank data objects from a given list of files or stream.
    ''' </summary>
    ''' <param name="files">a list of files or file stream</param>
    ''' <param name="autoClose">
    ''' auto close of the <see cref="Stream"/> if the <paramref name="files"/> contains stream object?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("populate.genbank")>
    <RApiReturn(GetType(GBFF.File))>
    Public Function populateGenbanks(<RRawVectorArgument>
                                     files As Object,
                                     Optional autoClose As Boolean = True,
                                     Optional env As Environment = Nothing) As Object
        If files Is Nothing Then
            Return Internal.debug.stop("the required file list can not be nothing!", env)
        End If

        Dim populator =
            Iterator Function() As IEnumerable(Of GBFF.File)
                For Each file As SeqValue(Of Object) In REnv.asVector(Of Object)(files).AsObjectEnumerator.SeqIterator
                    If file.value Is Nothing Then
                        env.AddMessage({$"file object in position {file.i} is nothing!", "index: " & file.i}, MSG_TYPES.WRN)
                    ElseIf TypeOf file.value Is String Then
                        For Each gb As GBFF.File In GBFF.File.LoadDatabase(file, suppressError:=True)
                            Yield gb
                        Next
                    ElseIf TypeOf file.value Is Stream Then
                        For Each gb As GBFF.File In GBFF.File.LoadDatabase(DirectCast(file.value, Stream), suppressError:=True)
                            Yield gb
                        Next

                        If autoClose Then
                            Try
                                Call DirectCast(file.value, Stream).Dispose()
                            Catch ex As Exception

                            End Try
                        End If
                    Else
                        env.AddMessage({$"file object in position {file.i} is not a file...", "index: " & file.i}, MSG_TYPES.WRN)
                    End If
                Next
            End Function

        Return pipeline.CreateFromPopulator(populator())
    End Function

    ''' <summary>
    ''' save the modified genbank file
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="file">the file path of the genbank assembly file to write data.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.genbank")>
    Public Function writeGenbank(gb As GBFF.File, file$, Optional env As Environment = Nothing) As Object
        If gb Is Nothing Then
            Return Internal.debug.stop("write data is nothing!", env)
        Else
            Return gb.Save(file)
        End If
    End Function

    ''' <summary>
    ''' converts tabular data file to genbank assembly object
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.genbank")>
    <RApiReturn(GetType(GBFF.File))>
    Public Function asGenbank(<RRawVectorArgument> x As Object, Optional env As Environment = Nothing) As Object
        If x Is Nothing Then
            env.AddMessage("The genome information data object is nothing!", MSG_TYPES.WRN)
            Return Nothing
        End If

        If TypeOf x Is PTT Then
            Return DirectCast(x, PTT).CreateGenbankObject
        Else
            Return Internal.debug.stop(New NotImplementedException(x.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' create new feature site
    ''' </summary>
    ''' <param name="keyName$"></param>
    ''' <param name="location"></param>
    ''' <param name="data"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("feature")>
    Public Function createFeature(keyName$, location As NucleotideLocation, data As list, Optional env As Environment = Nothing) As Feature
        Dim loci As New Feature With {
            .KeyName = keyName,
            .Location = New featureLocation With {
                .Complement = location.Strand = Strands.Reverse,
                .Locations = {
                    New RegionSegment With {.Left = location.left, .Right = location.right}
                }
            }
        }
        Dim values As String()

        For Each name As String In data.slots.Keys
            values = data.getValue(Of String())(name, env)

            For Each val As String In values.SafeQuery
                loci.Add(name, val)
            Next
        Next

        Return loci
    End Function

    <ExportAPI("add.feature")>
    Public Function addFeature(gb As GBFF.File, feature As Feature) As GBFF.File
        gb.Features.Add(feature)
        Return gb
    End Function

    ''' <summary>
    ''' enumerate all features in the given NCBI genbank database object
    ''' </summary>
    ''' <param name="gb">a NCBI genbank database object</param>
    ''' <returns></returns>
    <ExportAPI("enumerateFeatures")>
    Public Function enumerateFeatures(gb As GBFF.File) As Feature()
        Return gb.Features.ToArray
    End Function

    <ExportAPI("featureKeys")>
    <RApiReturn(GetType(String))>
    Public Function keyNames(<RRawVectorArgument> features As Object, Optional env As Environment = Nothing) As Object
        Dim featureArray As pipeline = pipeline.TryCreatePipeline(Of Feature)(features, env)

        If featureArray.isError Then
            Return featureArray.getError
        End If

        Return featureArray _
            .populates(Of Feature)(env) _
            .Select(Function(feature) feature.KeyName) _
            .ToArray
    End Function

    <ExportAPI("featureMeta")>
    <RApiReturn(GetType(String))>
    Public Function featureMeta(<RRawVectorArgument> features As Object, attrName$, Optional env As Environment = Nothing) As Object
        Dim featureArray As pipeline = pipeline.TryCreatePipeline(Of Feature)(features, env)

        If featureArray.isError Then
            Return featureArray.getError
        End If

        Return featureArray _
            .populates(Of Feature)(env) _
            .Select(Function(feature) feature.Query(attrName)) _
            .ToArray
    End Function

    <ExportAPI("addMeta")>
    Public Function addMeta(feature As Feature, <RListObjectArgument> meta As list, Optional env As Environment = Nothing) As Feature
        Dim metadata As Dictionary(Of String, String) = meta.AsGeneric(Of String)(env)

        For Each attr As KeyValuePair(Of String, String) In metadata
            Call feature.Add(attr)
        Next

        Return feature
    End Function

    ''' <summary>
    ''' get, add or replace the genome origin fasta sequence in the given genbank assembly file.
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="nt"></param>
    ''' <param name="mol_type"></param>
    ''' <returns>
    ''' if the ``<paramref name="nt"/>`` parameter is nothing, 
    ''' means get fasta sequence, otherwise is add/update fasta 
    ''' sequence in the genbank assembly, the returns type of 
    ''' the api will change from the getted fasta sequence to 
    ''' the modified genbank assembly object.
    ''' </returns>
    <ExportAPI("origin.fasta")>
    <RApiReturn(GetType(GBFF.File), GetType(FastaSeq))>
    Public Function getOrAddNtOrigin(gb As GBFF.File, Optional nt As FastaSeq = Nothing, Optional mol_type$ = "genomic DNA") As Object
        If nt Is Nothing Then
            Return gb.Origin.ToFasta
        Else
            Dim source As New gbffFeature With {
                .KeyName = "source",
                .Location = New featureLocation With {
                    .Complement = False,
                    .Locations = {
                        New RegionSegment With {.Left = 1, .Right = nt.Length}
                    }
                }
            }

            gb.Origin = New ORIGIN With {
                .Headers = nt.Headers,
                .SequenceData = nt.SequenceData
            }
            gb.Features.SetSourceFeature(source)

            source.SetValue(FeatureQualifiers.mol_type, mol_type)
            source.SetValue(FeatureQualifiers.organism, nt.Title)

            Return gb
        End If
    End Function

    ''' <summary>
    ''' get all of the RNA gene its gene sequence in fasta sequence format.
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <returns></returns>
    <ExportAPI("getRNA.fasta")>
    Public Function getRNASeq(gb As GBFF.File) As FastaFile
        Dim rnaGenes = gb.Features.Where(Function(region) InStr(region.KeyName, "RNA") > 0).ToArray
        Dim fasta As FastaSeq() = rnaGenes _
            .Select(Function(rna)
                        Dim attrs = {rna.Query(FeatureQualifiers.locus_tag) & " " & rna.KeyName & "|" & rna.Query(FeatureQualifiers.product)}
                        Dim fa As New FastaSeq With {
                            .Headers = attrs,
                            .SequenceData = rna.SequenceData
                        }

                        Return fa
                    End Function) _
            .ToArray

        Return New FastaFile(fasta)
    End Function

    ''' <summary>
    ''' get or set fasta sequence of all CDS feature in the given genbank assembly file. 
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="proteins"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("protein.fasta")>
    <RApiReturn(GetType(GBFF.File))>
    Public Function addproteinSeq(gb As GBFF.File,
                                  <RRawVectorArgument>
                                  Optional proteins As Object = Nothing,
                                  Optional env As Environment = Nothing) As Object

        Dim seqs As IEnumerable(Of FastaSeq)

        If proteins Is Nothing Then
            Return gb.ExportProteins_Short
        Else
            seqs = GetFastaSeq(proteins, env)
        End If

        If seqs Is Nothing Then
            Return Internal.debug.stop("no protein sequence data provided!", env)
        End If

        Dim seqTable = seqs.ToDictionary(Function(fa) fa.Title.Split.First)
        Dim geneId As String
        Dim prot As FastaSeq

        For Each feature In gb.Features.Where(Function(a) a.KeyName = "CDS")
            geneId = feature.Query(FeatureQualifiers.locus_tag)
            prot = seqTable.TryGetValue(geneId)

            If prot Is Nothing Then
                env.AddMessage($"missing protein sequence for '{geneId}'...", MSG_TYPES.WRN)
                Continue For
            End If

            feature.SetValue(FeatureQualifiers.translation, prot.SequenceData)
            feature.SetValue(FeatureQualifiers.product, prot.Title)
        Next

        Return gb
    End Function

    <ExportAPI("add.RNA.gene")>
    Public Function addRNAGene(gb As GBFF.File, <RRawVectorArgument> RNA As Object, Optional env As Environment = Nothing) As Object
        If RNA Is Nothing Then
            Return gb
        End If

        Dim rnaMaps As Dictionary(Of String, BlastnMapping)
        Dim geneId As String
        Dim mapHit As BlastnMapping
        Dim type, product As String
        Dim RNAfeature As gbffFeature

        If TypeOf RNA Is BlastnMapping() Then
            rnaMaps = DirectCast(RNA, BlastnMapping()) _
                .GroupBy(Function(map) map.Reference) _
                .ToDictionary(Function(map) map.Key,
                              Function(map)
                                  Return map.First
                              End Function)
        ElseIf TypeOf RNA Is pipeline AndAlso DirectCast(RNA, pipeline).elementType Like GetType(BlastnMapping) Then
            rnaMaps = DirectCast(RNA, pipeline) _
                .populates(Of BlastnMapping)(env) _
                .GroupBy(Function(map) map.Reference) _
                .ToDictionary(Function(map) map.Key,
                              Function(map)
                                  Return map.First
                              End Function)
        Else
            Return Internal.debug.stop($"invalid data source type: '{RNA.GetType.FullName}'!", env)
        End If

        For Each feature In gb.Features.Where(Function(a) a.KeyName = "gene")
            geneId = feature.Query(FeatureQualifiers.locus_tag)

            If rnaMaps.ContainsKey(geneId) Then
                mapHit = rnaMaps(geneId)
                type = mapHit.ReadQuery.GetTagValue.Value
                product = type.GetTagValue("|").Value
                type = type.Split("|"c).First

                RNAfeature = New gbffFeature With {.KeyName = type, .Location = feature.Location}
                RNAfeature.SetValue(FeatureQualifiers.product, product)
                RNAfeature.SetValue(FeatureQualifiers.locus_tag, geneId)

                Call gb.Features.Delete(type, geneId)
                Call gb.Features.Delete("CDS", geneId)
                Call gb.Features.Add(RNAfeature)
            End If
        Next

        Return gb
    End Function
End Module
