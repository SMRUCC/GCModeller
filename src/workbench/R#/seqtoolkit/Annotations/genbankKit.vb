#Region "Microsoft.VisualBasic::ebd0f6501151744fbdff69bd0a551b9f, R#\seqtoolkit\Annotations\genbankKit.vb"

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

    '   Total Lines: 557
    '    Code Lines: 382 (68.58%)
    ' Comment Lines: 109 (19.57%)
    '    - Xml Docs: 97.25%
    ' 
    '   Blank Lines: 66 (11.85%)
    '     File Size: 21.64 KB


    ' Module genbankKit
    ' 
    '     Function: accession_id, addFeature, addMeta, addproteinSeq, addRNAGene
    '               asGenbank, create_tabular, createFeature, enumerateFeatures, featureMeta
    '               getOrAddNtOrigin, getRNASeq, isPlasmidSource, keyNames, populateGenbanks
    '               Populates, readGenbank, Taxon_Id, writeGenbank
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports featureLocation = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Location
Imports gbffFeature = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

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
    <RApiReturn(GetType(GBFF.File))>
    Public Function readGenbank(file As String,
                                Optional repliconTable As Boolean = False,
                                Optional env As Environment = Nothing) As Object

        If Not file.FileExists(True) Then
            Return RInternal.debug.stop($"invalid file resource: '{file}'!", env)
        End If

        If repliconTable Then
            Return GenBank.loadRepliconTable(file)
        Else
            Return GBFF.File.Load(file)
        End If
    End Function

    ''' <summary>
    ''' get ncbi taxonomy id from the given genbank assembly file.
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <returns></returns>
    <ExportAPI("taxon_id")>
    Public Function Taxon_Id(gb As GBFF.File) As Object
        Return gb.Taxon
    End Function

    ''' <summary>
    ''' extract the taxonomy lineage information from the genbank file
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <returns></returns>
    <ExportAPI("taxonomy_lineage")>
    Public Function taxonomy(gb As GBFF.File) As Taxonomy
        Return gb.Source.GetTaxonomy
    End Function

    ''' <summary>
    ''' extract all gene features from genbank and cast to tabular data
    ''' </summary>
    ''' <param name="gbff"></param>
    ''' <returns></returns>
    <ExportAPI("as_tabular")>
    <RApiReturn(GetType(GeneTable))>
    Public Function create_tabular(gbff As GBFF.File, Optional ORF As Boolean = True) As Object
        If ORF Then
            Return gbff.ExportGeneFeatures
        Else
            Return gbff _
                .EnumerateGeneFeatures(ORF:=False) _
                .AsParallel _
                .Select(Function(gene) gene.DumpExportFeature) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' get current genbank assembly accession id 
    ''' </summary>
    ''' <param name="genbank"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("accession_id")>
    <RApiReturn(TypeCodes.string)>
    Public Function accession_id(<RRawVectorArgument> genbank As Object, Optional env As Environment = Nothing) As Object
        Dim pull = pipeline.TryCreatePipeline(Of GBFF.File)(genbank, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pull.populates(Of GBFF.File)(env) _
            .Select(Function(gb) gb.Accession.AccessionId) _
            .ToArray
    End Function

    ''' <summary>
    ''' check of the given genbank assembly is the data source of a plasmid or not?
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
    ''' <remarks>
    ''' this function supports of read assembly data directly from *.gz genbank archive file.
    ''' </remarks>
    <ExportAPI("load_genbanks")>
    <RApiReturn(GetType(GBFF.File))>
    Public Function populateGenbanks(<RRawVectorArgument>
                                     files As Object,
                                     Optional autoClose As Boolean = True,
                                     Optional env As Environment = Nothing) As Object
        If files Is Nothing Then
            Return RInternal.debug.stop("the required file list can not be nothing!", env)
        End If

        ' CLRIterator
        Return pipeline.CreateFromPopulator(Populates(files, autoClose, env))
    End Function

    Private Iterator Function Populates(files As Object, autoClose As Boolean, env As Environment) As IEnumerable(Of GBFF.File)
        For Each file As SeqValue(Of Object) In REnv.asVector(Of Object)(files) _
            .AsObjectEnumerator _
            .SeqIterator

            If file.value Is Nothing Then
                Call env.AddMessage({
                    $"file object in position {file.i} is nothing!",
                    "index: " & file.i
                }, MSG_TYPES.WRN)
            ElseIf TypeOf file.value Is String Then
                Dim filepath As String = CStr(file.value)

                If filepath.ExtensionSuffix("gz") Then
                    Try
                        Using s As Stream = filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                            For Each gb As GBFF.File In GBFF.File.LoadDatabase(s.UnGzipStream, suppressError:=True)
                                Yield gb
                            Next
                        End Using
                    Catch ex As Exception
                        Call App.LogException(ex, filepath)
                        Call $"gzip decompress error: {filepath}".warning
                    End Try
                Else
                    For Each gb As GBFF.File In GBFF.File.LoadDatabase(filepath, suppressError:=True)
                        Yield gb
                    Next
                End If
            ElseIf TypeOf file.value Is Stream Then
                For Each gb As GBFF.File In GBFF.File.LoadDatabase(DirectCast(file.value, Stream), suppressError:=True)
                    Yield gb
                Next

                If autoClose Then
                    Try
                        Call DirectCast(file.value, Stream).Dispose()
                    Catch ex As Exception
                        ' just ignores of this file close error
                    End Try
                End If
            Else
                Call env.AddMessage({
                    $"file object in position {file.i} is not a file...",
                    "index: " & file.i
                }, MSG_TYPES.WRN)
            End If
        Next
    End Function

    ''' <summary>
    ''' save the modified genbank file
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="file">the file path of the genbank assembly file to write data.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("write.genbank")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function writeGenbank(gb As GBFF.File, file$, Optional env As Environment = Nothing) As Object
        If gb Is Nothing Then
            Return RInternal.debug.stop("write data is nothing!", env)
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
            Return RInternal.debug.stop(New NotImplementedException(x.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' create new feature site
    ''' </summary>
    ''' <param name="keyName"></param>
    ''' <param name="location"></param>
    ''' <param name="data"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("feature")>
    Public Function createFeature(keyName$, location As NucleotideLocation,
                                  <RListObjectArgument>
                                  Optional data As list = Nothing,
                                  Optional env As Environment = Nothing) As Feature

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

    ''' <summary>
    ''' add feature into a given genbank object
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="feature"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let gb_asm = read.genbank("./xxx.gbff");
    ''' let mics_site = GenBank::feature("micsRNA", nucl_location(5656,33,"+"));
    ''' 
    ''' # use the operator
    ''' gb_asm = gb_asm + mics_site;
    ''' # or use the function
    ''' gb_asm = gb_asm |> add_feature(mics_site);
    ''' </example>
    <ExportAPI("add_feature")>
    <ROperator("+")>
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
    Public Function enumerateFeatures(gb As GBFF.File, Optional keys As String() = Nothing) As Feature()
        If keys.IsNullOrEmpty Then
            Return gb.Features.ToArray
        Else
            With keys.Indexing
                Return gb.Features _
                    .Where(Function(f) .IndexOf(f.KeyName) > -1) _
                    .ToArray
            End With
        End If
    End Function

    ''' <summary>
    ''' get all feature key names 
    ''' </summary>
    ''' <param name="features">a collection of the genbank feature object or a genbank clr object.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("featureKeys")>
    <RApiReturn(GetType(String))>
    Public Function keyNames(<RRawVectorArgument> features As Object, Optional env As Environment = Nothing) As Object
        Dim featureArray As pipeline

        If TypeOf features Is GBFF.File Then
            Dim featureSet = DirectCast(features, GBFF.File).Features _
                .AsEnumerable _
                .ToArray

            featureArray = pipeline.CreateFromPopulator(featureSet)
        Else
            featureArray = pipeline.TryCreatePipeline(Of gbffFeature)(features, env)
        End If

        If featureArray.isError Then
            Return featureArray.getError
        End If

        Return featureArray _
            .populates(Of Feature)(env) _
            .Select(Function(feature) feature.KeyName) _
            .ToArray
    End Function

    ''' <summary>
    ''' extract the feature metadata from a genbank clr feature object
    ''' </summary>
    ''' <param name="features"></param>
    ''' <param name="attrName$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("featureMeta")>
    <RApiReturn(GetType(String))>
    Public Function featureMeta(<RRawVectorArgument> features As Object,
                                Optional attrName$ = Nothing,
                                Optional env As Environment = Nothing) As Object

        Dim featureArray As pipeline = pipeline.TryCreatePipeline(Of gbffFeature)(features, env)

        If featureArray.isError Then
            Return featureArray.getError
        End If

        Dim all As gbffFeature() = featureArray _
            .populates(Of gbffFeature)(env) _
            .ToArray

        If attrName.StringEmpty(, True) Then
            ' populate all features
            If all.Length = 1 Then
                Return New list With {
                    .slots = all(0).PairedValues _
                        .GroupBy(Function(f) f.Name) _
                        .ToDictionary(Function(f) f.Key,
                                      Function(f)
                                          Return CObj(f.Values)
                                      End Function)
                }
            Else
                Dim list As list = list.empty

                For Each feature As gbffFeature In all
                    list.add(feature.ToString, feature.PairedValues _
                        .GroupBy(Function(f) f.Name) _
                        .ToDictionary(Function(f) f.Key,
                                      Function(f)
                                          Return CObj(f.Values)
                                      End Function))
                Next

                Return list
            End If
        Else
            ' populate a specific feature metadata vector
            Return all _
                .Select(Function(feature) feature.Query(attrName)) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' add metadata into a given feature object
    ''' </summary>
    ''' <param name="feature"></param>
    ''' <param name="meta"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
    <ExportAPI("origin_fasta")>
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
    ''' export gene fasta from the given genbank assembly file
    ''' </summary>
    ''' <param name="gb"></param>
    ''' <param name="title"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' fasta title is build with a string template, there are some reserved template keyword for this function:
    ''' 
    ''' 1. ncbi_taxid - is the ncbi taxonomy id that extract from the genbank assembly
    ''' 2. lineage - taxonomy lineage in biom style string, which is extract from the genbank assembly its source information
    ''' 3. gb_asm_id - the ncbi accession id of the genbank assembly
    ''' 4. nucl_loc - the nucleotide sequence location on the genomics sequence
    ''' </remarks>
    <ExportAPI("export_geneNt_fasta")>
    Public Function exportGeneNtFasta(gb As GBFF.File,
                                      Optional title As String = "<gb_asm_id>.<locus_tag> <nucl_loc> <product>|<lineage>",
                                      <RRawVectorArgument(TypeCodes.string)>
                                      Optional key As Object = "gene|CDS") As FastaFile

        Dim keyStr As String = If(CLRVector.asScalarCharacter(key), "gene")
        Dim geneList As gbffFeature() = gb.Features _
            .Where(Function(g) g.KeyName = keyStr) _
            .ToArray
        Dim fastaFile As New FastaFile
        Dim accessionId As String = gb.Accession.AccessionId
        Dim lineage As String = gb.Source.BiomString
        Dim template As New StringTemplate(title, defaults:=New Dictionary(Of String, String) From {
            {"ncbi_taxid", gb.Taxon},
            {"lineage", lineage},
            {"gb_asm_id", accessionId}
        })
        Dim i As i32 = 1

        For Each gene As Feature In geneList
            Call template.SetDefaultKey("nucl_loc", gene.Location.ContiguousRegion.ToString)
            Call template.SetDefaultKey("locus_tag", $"locus_{++i}")

            Call fastaFile.Add(gene.ToGeneFasta(template))
        Next

        Return fastaFile
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
            Return RInternal.debug.stop("no protein sequence data provided!", env)
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
    <RApiReturn(GetType(GBFF.File))>
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
            Return RInternal.debug.stop($"invalid data source type: '{RNA.GetType.FullName}'!", env)
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
