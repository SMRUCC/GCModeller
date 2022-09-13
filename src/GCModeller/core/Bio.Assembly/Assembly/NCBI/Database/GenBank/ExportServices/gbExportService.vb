#Region "Microsoft.VisualBasic::b54a29cd1ac8c4873eb24384ed4ddf6e, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\ExportServices\gbExportService.vb"

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

    '   Total Lines: 654
    '    Code Lines: 485
    ' Comment Lines: 90
    '   Blank Lines: 79
    '     File Size: 32.33 KB


    '     Module gbExportService
    ' 
    '         Function: __exportNoAnnotation, __exportWithAnnotation, __featureToPTT, BatchExport, BatchExportPlasmid
    '                   CopyGenomeSequence, (+2 Overloads) Distinct, DumpEXPORT, EnsureNonEmptyLocusId, EnumerateGeneFeatures
    '                   ExportGeneFeatures, ExportGeneNtFasta, ExportPTTAsDump, FeatureGenes, GbffToPTT
    '                   InvokeExport, LoadGbkSource, TryParseGBKID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Assembly.NCBI.GenBank

    ''' <summary>
    ''' Genbank export methods collection.
    ''' </summary>
    ''' <remarks></remarks>
    Public Module gbExportService

        <Extension>
        Public Function EnsureNonEmptyLocusId(feature As Feature) As String
            Dim locus_id$ = feature("locus_tag")

            If String.IsNullOrEmpty(locus_id) Then
                locus_id = feature("protein_id")
            End If
            If String.IsNullOrEmpty(locus_id) Then
                locus_id = (From ref As String
                            In feature.QueryDuplicated("db_xref")
                            Let Tokens As String() = ref.Split(CChar(":"))
                            Where String.Equals(Tokens.First, "PSEUDO")
                            Select Tokens.Last).FirstOrDefault
            End If
            If String.IsNullOrEmpty(locus_id) Then
                locus_id = feature("db_xref")
            End If

            Return locus_id
        End Function

        ''' <summary>
        ''' Convert a feature site data in the NCBI GenBank file to the dump information table.
        ''' </summary>
        ''' <param name="obj">CDS标记的特性字段</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function DumpEXPORT(obj As CDS) As GeneTable
            Dim gene As New GeneTable With {
                .locus_id = obj.EnsureNonEmptyLocusId
            }

            Call obj.TryGetValue("product", gene.commonName)
            Call obj.TryGetValue("protein_id", gene.ProteinId)
            Call obj.TryGetValue("gene", gene.geneName)
            Call obj.TryGetValue("translation", gene.Translation)
            Call obj.TryGetValue("function", gene.function)
            Call obj.TryGetValue("transl_table", gene.Transl_table)

            gene.GI = obj.db_xref_GI
            gene.UniprotSwissProt = obj.db_xref_UniprotKBSwissProt
            gene.UniprotTrEMBL = obj.db_xref_UniprotKBTrEMBL
            gene.InterPro = obj.db_xref_InterPro
            gene.GO = obj.db_xref_GO
            gene.species = obj.gb.Definition.Value
            gene.EC_Number = obj.Query(FeatureQualifiers.EC_number)
            gene.SpeciesAccessionID = obj.gb.Locus.AccessionID

            'If gene.Function.StringEmpty Then

            'End If

            Try
                gene.left = obj.Location.ContiguousRegion.left
                gene.right = obj.Location.ContiguousRegion.right
                gene.strand = If(obj.Location.Complement, "-", "+")
            Catch ex As Exception
                Dim msg As String = $"{obj.gb.Accession.AccessionId} location data is null!"
                ex = New Exception(msg)
                Call VBDebugger.Warning(msg)
                Call App.LogException(ex)
            End Try

            Return gene
        End Function

        ''' <summary>
        ''' 尝试去除重复的记录
        ''' </summary>
        ''' <param name="source">原始的Genbank数据库文件所存放的文件夹</param>
        ''' <returns></returns>
        ''' <param name="Trim">是否去除没有序列数据的或者格式已经损坏的数据库文件，并且将原始文件删除？默认都去除</param>
        ''' <remarks></remarks>
        Public Function Distinct(source As String, Optional Trim As Boolean = True) As CsvExports.Plasmid()
            Dim LQuery = (From Path As String
                          In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchAllSubDirectories, "*.gb", "*.gbk").AsParallel
                          Let Genbank = NCBI.GenBank.GBFF.File.Load(Path)
                          Select Path, Genbank).ToArray
            If Trim Then
                For Each PathEntry As String In (From nn In LQuery Where nn.Genbank Is Nothing OrElse Not nn.Genbank.HasSequenceData Select nn.Path).ToArray
                    Call FileIO.FileSystem.DeleteFile(PathEntry)
                Next

                LQuery = (From item In LQuery Where Not item.Genbank Is Nothing OrElse item.Genbank.HasSequenceData Select item).ToArray
            Else
                LQuery = (From item In LQuery Where Not item.Genbank Is Nothing Select item).ToArray
            End If

            '生成摘要数据
            Dim Brief = (From item In LQuery.AsParallel
                         Let BriefInfo = CsvExports.gbEntryBrief.ConvertObject(Of CsvExports.Plasmid)(item.Genbank)
                         Let Signature As String = BriefInfo.GC_Content.ToString & BriefInfo.Length.ToString & BriefInfo.Organism.ToLower
                         Select item.Path, BriefInfo, Signature, SubmitDate = BriefInfo.GetSubmitDate
                         Group By Signature Into Group).ToArray

            '删除原始数据
            Dim Distincted As New List(Of CsvExports.Plasmid)
            For Each Entry In Brief
                If Entry.Group.Count = 1 Then
                    Call Distincted.Add(Entry.Group.First.BriefInfo)
                Else
                    '按照日期排序，只取出最新提交的数据
                    Dim OrderedLQuery = (From item In Entry.Group Select item Order By item.SubmitDate Descending).ToArray
                    Call Distincted.Add(OrderedLQuery.First.BriefInfo)

                    For Each Duplicated In OrderedLQuery.Skip(1)
                        Call FileIO.FileSystem.DeleteFile(Duplicated.Path)
                    Next
                End If
            Next

            Return Distincted.ToArray
        End Function

        ''' <summary>
        ''' 返回去除掉重复的数据之后的AccessionId编号
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Distinct(data As IEnumerable(Of CsvExports.Plasmid)) As CsvExports.Plasmid()
            '生成摘要数据
            Dim Brief = (From BriefInfo In data.AsParallel
                         Let Signature As String = BriefInfo.GC_Content.ToString & BriefInfo.Length.ToString & BriefInfo.Organism.ToLower
                         Select BriefInfo, Signature, SubmitDate = BriefInfo.GetSubmitDate
                         Group By Signature Into Group).ToArray

            '删除原始数据
            Dim Distincted As New List(Of CsvExports.Plasmid)
            For Each Entry In Brief
                If Entry.Group.Count = 1 Then
                    Call Distincted.Add(Entry.Group.First.BriefInfo)
                Else
                    '按照日期排序，只取出最新提交的数据
                    Dim OrderedLQuery = (From item In Entry.Group Select item Order By item.SubmitDate Descending).ToArray
                    Call Distincted.Add(OrderedLQuery.First.BriefInfo)
                End If
            Next

            Return Distincted.ToArray
        End Function

        ''' <summary>
        ''' 将PTT文件夹之中的基因组序列数据复制到目标文件夹之中
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CopyGenomeSequence(source As String, copyTo As String) As Integer
            Dim LQuery = (From Path In source.LoadSourceEntryList("*.fna").AsParallel
                          Let Fasta = New TabularFormat.FastaObjects.GenomeSequence(FastaSeq.Load(Path.Value))
                          Select Fasta.SaveBriefData(copyTo & "/" & Path.Key & ".fasta")).ToArray
            Return LQuery.Count
        End Function

        ''' <summary>
        ''' 假若目标GBK是使用本模块之中的方法保存或者导出来的，则可以使用本方法生成Entry列表；（在返回的结果之中，KEY为文件名，没有拓展名，VALUE为文件的路径）
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="ext">文件类型的拓展名称</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function LoadGbkSource(source$, ParamArray ext As String()) As Dictionary(Of NamedValue(Of String))
            Dim LQuery = From path As String
                         In ls - l - r - wildcards(ext) <= source
                         Select ID = TryParseGBKID(path),
                             path
                         Group By ID Into Group
            Dim out = LQuery.Select(
                Function(o) New NamedValue(Of String) With {
                    .Name = o.ID,
                    .Value = o.Group.First.path
                }).ToDictionary()
            Return out
        End Function

        ''' <summary>
        ''' 将GBK文件之中的基因的位置数据导出为PTT格式的数据，这个函数所导出来的数据包含了蛋白质和RNA，如果<paramref name="ORF"/>为False的话。
        ''' </summary>
        ''' <param name="genbank">导出CDS gene和RNA部分的数据</param>
        ''' <param name="ORF">默认参数值为True，表示只导出蛋白编码基因的位置信息</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function GbffToPTT(genbank As GBFF.File, Optional ORF As Boolean = True) As TabularFormat.PTT
            Dim size = genbank.Origin.SequenceData.Length
            Dim def As String = genbank.Definition.Value
            Dim genes As IEnumerable(Of Feature) = genbank.EnumerateGeneFeatures(ORF)
            Dim genomics As New TabularFormat.PTT With {
                .GeneObjects = genes.FeatureGenes.ToArray,
                .Size = size,
                .Title = def
            }

            If Not genbank.SourceFeature Is Nothing Then
                Dim db_xref = genbank.SourceFeature.Query(FeatureQualifiers.db_xref)

                If db_xref.IsPattern("taxon[:]\d+") Then
                    genomics.Title = $"{def} (ncbi_taxid:{db_xref.Match("\d+")})"
                End If
            End If

            Return genomics
        End Function

        <Extension>
        Public Function EnumerateGeneFeatures(genbank As GBFF.File, Optional ORF As Boolean = True) As IEnumerable(Of Feature)
            Dim assert As Predicate(Of Feature)

            If ORF Then
                assert = Function(feature)
                             Return String.Equals(feature.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                         End Function
            Else
                assert = Function(feature)
                             ' 蛋白质编码基因以及RNA基因
                             Return String.Equals(feature.KeyName, "CDS", StringComparison.OrdinalIgnoreCase) OrElse
                                 InStr(feature.KeyName, "RNA", CompareMethod.Text) > 0
                         End Function
            End If

            Return From feature As Feature
                   In genbank.Features._innerList
                   Where True = assert(feature)
                   Select feature
        End Function

        <Extension>
        Public Function FeatureGenes(genes As IEnumerable(Of Feature)) As IEnumerable(Of GeneBrief)
            Dim PTT_genes = From g As Feature
                            In genes
                            Select gene = g.__featureToPTT
                            Group gene By gene.Synonym Into Group
            Dim array = (From g
                         In PTT_genes
                         Select g.Synonym, ggenes = g.Group.ToArray).ToArray

            For Each duplicated In From ggene
                                   In array
                                   Where ggene.ggenes.Length > 1
                                   Select ggene
                Call VBDebugger.Warning($"""{duplicated.Synonym}"" data was duplicated!")
            Next

            Return From gene In array Select gene.ggenes.First
        End Function

        <Extension>
        Private Function __featureToPTT(featureSite As Feature) As GeneBrief
            Dim loci As NucleotideLocation

            If featureSite.Location.Locations.IsNullOrEmpty Then
                loci = New NucleotideLocation
            Else
                loci = New NucleotideLocation(
                    featureSite.Location.Locations.First.Left,
                    featureSite.Location.Locations.Last.Right,
                    featureSite.Location.Complement)
            End If

            Dim locusId$ = featureSite.Query(FeatureQualifiers.locus_tag)
            Dim gene As New GeneBrief With {
                .Synonym = locusId,
                .PID = featureSite.Query(FeatureQualifiers.protein_id),
                .Product = featureSite.Query(FeatureQualifiers.product),
                .Gene = featureSite.Query(FeatureQualifiers.gene),
                .Location = loci,
                .Length = .Location.FragmentSize
            }
            Dim note As String = featureSite.Query(FeatureQualifiers.note)

            If Not note.StringEmpty Then
                If gene.Product.StringEmpty Then
                    gene.Product = note
                End If

                gene.COG = note _
                    .Split _
                    .FirstOrDefault _
                    .Match("COG\d+", RegexICSng)
            End If
            If String.IsNullOrEmpty(gene.Synonym) Then
                gene.Synonym = featureSite.Query(FeatureQualifiers.gene)
            End If
            If String.IsNullOrEmpty(gene.Synonym) Then
                gene.Synonym = featureSite.Location.UniqueId
            End If

            Return gene
        End Function

        <Extension> Public Function InvokeExport(gbk As GBFF.File, ByRef GeneList As GeneTable()) As KeyValuePair(Of gbEntryBrief, String)
            Dim LQuery = (From FeatureData As Feature
                          In gbk.Features._innerList.AsParallel
                          Where String.Equals(FeatureData.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                          Select New CDS(FeatureData).DumpEXPORT).ToArray
            GeneList = LQuery
            Return New KeyValuePair(Of gbEntryBrief, String)(gbEntryBrief.ConvertObject(Of gbEntryBrief)(gbk), gbk.Origin.SequenceData)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="list"></param>
        ''' <param name="GeneList"></param>
        ''' <param name="GBK"></param>
        ''' <param name="FastaExport">Fasta序列文件的导出文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BatchExport(list As IEnumerable(Of GBFF.File),
                                    ByRef GeneList As GeneTable(),
                                    ByRef GBK As gbEntryBrief(),
                                    FastaExport As String,
                                    Optional FastaWithAnnotation As Boolean = False) As Integer

            Dim ExportList As New Dictionary(Of gbEntryBrief, String)
            Dim GeneChunkList As New List(Of GeneTable)
            Dim FastaFile As New FASTA.FastaFile
            Dim PlasmidList As New FASTA.FastaFile
            Dim GeneSequenceList As New FASTA.FastaFile

            Call "Flushed memory....".__DEBUG_ECHO
            Call FlushMemory()
            Call $"There is ""{list.Count}"" genome source will be export...".__DEBUG_ECHO

            Dim ExportLQuery = (From GBKFF As GBFF.File
                                In list.AsParallel
                                Let GenesTempChunk As GeneTable() = (From FeatureData As Feature
                                                                        In GBKFF.Features._innerList.AsParallel
                                                                     Where String.Equals(FeatureData.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                                                                     Select New CDS(FeatureData).DumpEXPORT).ToArray
                                Let Entry = gbEntryBrief.ConvertObject(Of gbEntryBrief)(GBKFF)
                                Let FastaDump As FASTA.FastaFile =
                                    If(FastaWithAnnotation, __exportWithAnnotation(GenesTempChunk), __exportNoAnnotation(GenesTempChunk))
                                Let Plasmid As FASTA.FastaSeq =
                                    New FASTA.FastaSeq With {
                                        .Headers = New String() {Entry.AccessionID},
                                        .SequenceData = GBKFF.Origin.SequenceData.ToUpper
                                    }
                                Let reader As IPolymerSequenceModel = GBKFF.Origin
                                Let GeneFastaDump = CType((From GeneObject In GBKFF.Features._innerList.AsParallel
                                                           Where String.Equals(GeneObject.KeyName, "gene", StringComparison.OrdinalIgnoreCase)
                                                           Let loc = GeneObject.Location.ContiguousRegion
                                                           Let Sequence As String = reader.CutSequenceLinear(loc.left, loc.right).SequenceData
                                                           Select New FASTA.FastaSeq With {
                                                               .Headers = New String() {GeneObject.Query("locus_tag"), GeneObject.Location.ToString},
                                                               .SequenceData = If(GeneObject.Location.Complement, NucleicAcid.Complement(Sequence), Sequence)
                                                           }).ToArray, FASTA.FastaFile)
                                Select GBKFF,
                                    GenesTempChunk,
                                    Entry,
                                    FastaDump,
                                    Plasmid,
                                    reader,
                                    GeneFastaDump).ToArray

            For Each gene In ExportLQuery
                Call GeneChunkList.AddRange(gene.GenesTempChunk)
                Call ExportList.Add(gene.Entry, gene.Entry.AccessionID)
                Call gene.FastaDump.Save(FastaExport & "/Orf/" & gene.Entry.AccessionID & ".fasta", Encoding.UTF8)
                Call gene.Plasmid.SaveTo(FastaExport & "/Genomes/" & gene.Entry.AccessionID & ".fasta", Encoding.UTF8)
                Call gene.GeneFastaDump.Save(FastaExport & "/Genes/" & gene.Entry.AccessionID & ".fasta", Encoding.UTF8)

                Call FastaFile.AddRange(gene.FastaDump)
                Call PlasmidList.Add(gene.Plasmid)
                Call GeneSequenceList.AddRange(gene.GeneFastaDump)
            Next

            GeneList = GeneChunkList.ToArray
            GBK = (From entryInfo In ExportList Select entryInfo.Key).ToArray

            Try
                Call FastaFile.Save(FastaExport & "/CDS.Gene.fasta", Encoding.UTF8)
                Call PlasmidList.Save(FastaExport & "/Genbank.ORIGINS.fasta", Encoding.UTF8)
                Call GeneSequenceList.Save(FastaExport & "/Gene.Nt.fasta", Encoding.UTF8)
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            Return ExportList.Count
        End Function

        ''' <summary>
        ''' Exports CDS feature
        ''' </summary>
        ''' <param name="gbk"></param>
        ''' <returns></returns>
        <Extension> Public Function ExportGeneFeatures(gbk As GBFF.File) As GeneTable()
            Dim dumps As GeneTable() = LinqAPI.Exec(Of GeneTable) <=
 _
                From feature As Feature
                In gbk.Features._innerList.AsParallel
                Where String.Equals(feature.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                Select gene = New CDS(feature).DumpEXPORT
                Order By gene.locus_id Ascending

            Return dumps
        End Function

        <Extension> Public Function ExportPTTAsDump(PTT As NCBI.GenBank.TabularFormat.PTT) As GeneTable()
            Dim LQuery As GeneTable() = LinqAPI.Exec(Of GeneTable) <=
 _
                From gene As GeneBrief
                In PTT.GeneObjects.AsParallel
                Select New GeneTable With {
                    .CDS = "",
                    .COG = gene.COG,
                    .commonName = gene.Gene,
                    .EC_Number = "-",
                    .function = gene.Product,
                    .GC_Content = 0,
                    .geneName = gene.Gene,
                    .GI = "-",
                    .GO = {},
                    .InterPro = {},
                    .left = gene.Location.left,
                    .length = gene.Location.FragmentSize,
                    .Location = gene.Location,
                    .locus_id = gene.Synonym,
                    .ProteinId = gene.Synonym,
                    .right = gene.Location.right,
                    .species = "",
                    .SpeciesAccessionID = "",
                    .strand = gene.Location.Strand.ToString,
                    .Translation = "",
                    .Transl_table = "",
                    .UniprotSwissProt = "",
                    .UniprotTrEMBL = ""
                }
            Return LQuery
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="list"></param>
        ''' <param name="GeneList"></param>
        ''' <param name="GBK"></param>
        ''' <param name="FastaExport"></param>
        ''' <param name="FastaWithAnnotation">是否将序列的注释信息一同导出来，<see cref="vbTrue"></see>会将功能注释信息和菌株信息一同导出，<see cref="vbFalse"></see>则仅仅会导出基因号，假若没有基因号，则会导出蛋白质编号</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BatchExportPlasmid(list As IEnumerable(Of NCBI.GenBank.GBFF.File),
                                           ByRef GeneList As GeneTable(),
                                           ByRef GBK As Plasmid(),
                                           FastaExport As String,
                                           Optional FastaWithAnnotation As Boolean = False) As Integer
            Dim ExportList As New Dictionary(Of Plasmid, String)
            Dim buf As New List(Of GeneTable)
            Dim FastaFile As New FASTA.FastaFile
            Dim PlasmidList As New FASTA.FastaFile
            Dim GeneSequenceList As New FASTA.FastaFile

            Dim Source As GBFF.File() = (From gb As GBFF.File
                                         In list.AsParallel
                                         Where gb.isPlasmid
                                         Select gb).ToArray

            Call "Flushed memory....".__DEBUG_ECHO
            list = Nothing
            Call FlushMemory()
            Call $"There is ""{Source.Length}"" plasmid source will be export...".__DEBUG_ECHO

            For Each gb As GBFF.File In Source
                Dim cds As GeneTable() = gb.ExportGeneFeatures
                Dim Entry = NCBI.GenBank.CsvExports.Plasmid.Build(gb)

                Call ExportList.Add(Entry, gb.Origin.SequenceData)
                Call buf.AddRange(cds)

                '导出Fasta序列
                Dim FastaDump As FASTA.FastaFile =
                    If(FastaWithAnnotation,
                    __exportWithAnnotation(cds),
                    __exportNoAnnotation(cds))

                If FastaDump.Count > 0 Then
                    Call FastaDump.Save(String.Format("{0}/plasmid_cds/{1}.fasta", FastaExport, gb.Accession.AccessionId), Encoding.UTF8)
                    Call FastaFile.AddRange(FastaDump)
                End If

                Dim Plasmid As New FASTA.FastaSeq With {
                        .Headers = New String() {Entry.AccessionID & "_" & Entry.PlasmidID.Replace("-", "_")},
                        .SequenceData = gb.Origin.SequenceData.ToUpper
                }

                Call PlasmidList.Add(Plasmid)
                Call Plasmid.SaveTo(String.Format("{0}/plasmids/{1}.fasta", FastaExport, gb.Accession.AccessionId))

                Dim reader As IPolymerSequenceModel = gb.Origin
                Dim GeneFastaDump = CType((From GeneObject In gb.Features._innerList.AsParallel
                                           Where String.Equals(GeneObject.KeyName, "gene", StringComparison.OrdinalIgnoreCase)
                                           Let loc = GeneObject.Location.ContiguousRegion
                                           Let Sequence As String = reader.CutSequenceLinear(loc.left, loc.right).SequenceData
                                           Select New FASTA.FastaSeq With {
                                               .Headers = New String() {GeneObject.Query("locus_tag"), GeneObject.Location.ToString},
                                               .SequenceData = If(GeneObject.Location.Complement, NucleicAcid.Complement(Sequence), Sequence)
                                           }).ToArray, FASTA.FastaFile)

                If GeneFastaDump.Count > 0 Then
                    Call GeneSequenceList.AddRange(GeneFastaDump.ToArray)
                    Call GeneFastaDump.Save(String.Format("{0}/plasmid_genes/{1}.fasta", FastaExport, gb.Accession.AccessionId), Encoding.UTF8)
                    Call GeneFastaDump.Clear()
                End If
            Next

            GeneList = buf.ToArray
            GBK = (From item In ExportList Select item.Key).ToArray

            Try
                Call FastaFile.Save(FastaExport & "/CDS_GENE.fasta", Encoding.UTF8)
                Call PlasmidList.Save(FastaExport & "/GBKFF.ORIGINS_plasmid.fasta", Encoding.UTF8)
                Call GeneSequenceList.Save(FastaExport & "/GENE_SEQUENCE.fasta", Encoding.UTF8)
            Catch ex As Exception

            End Try

            Return ExportList.Count
        End Function

        Private Function __exportNoAnnotation(data As GeneTable()) As FASTA.FastaFile
            Dim LQuery As IEnumerable(Of FASTA.FastaSeq) =
                From gene As GeneTable
                In data.AsParallel
                Let fa As FASTA.FastaSeq =
                    New FASTA.FastaSeq With {
                        .Headers = New String() {gene.locus_id},
                        .SequenceData = gene.Translation
                    }
                Select fa
            Return New FASTA.FastaFile(LQuery)
        End Function

        Private Function __exportWithAnnotation(data As GeneTable()) As FASTA.FastaFile
            Dim LQuery = From gene As GeneTable
                         In data.AsParallel
                         Let attrs As String() = {gene.locus_id, gene.geneName, gene.GI, gene.commonName, gene.function, gene.species}
                         Select New FASTA.FastaSeq With {
                             .Headers = attrs,
                             .SequenceData = gene.Translation
                         }
            Return New FASTA.FastaFile(LQuery)
        End Function

        <Extension> Public Function TryParseGBKID(path As String) As String
            Dim Name As String = path.BaseName
            Name = Regex.Replace(Name, "\.\d+", "")
            Return Name.ToUpper
        End Function

        ''' <summary>
        ''' {locus_tag, gene.Location.ToString, products.SafeGetValue(locus_tag)?.Function}.
        ''' (导出每一个基因的核酸序列)
        ''' </summary>
        ''' <param name="gb">Genbank数据库文件</param>
        ''' <param name="geneName">
        ''' If this parameter is specific as True, then this function will try using 
        ''' geneName as the fasta sequence title, or using locus_tag value as default.
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function ExportGeneNtFasta(gb As GBFF.File,
                                          Optional geneName As Boolean = False,
                                          Optional onlyLocus_tag As Boolean = False) As FASTA.FastaFile

            Dim reader As IPolymerSequenceModel = gb.Origin
            Dim list As New List(Of FastaSeq)
            Dim loc As NucleotideLocation = Nothing
            Dim attrs As String() = Nothing
            Dim Sequence As String
            Dim products As Dictionary(Of GeneTable) = gb.ExportGeneFeatures.ToDictionary
            Dim geneFeatures = (From x As Feature
                                In gb.Features._innerList
                                Where String.Equals(x.KeyName, "gene", StringComparison.OrdinalIgnoreCase)
                                Select x).ToArray
            Dim locus_tag As String
            Dim function$

            If geneFeatures.Length = 0 Then
                ' 在gb文件中没有定义gene feature
                ' 则直接导出所有的feature的序列？
                geneFeatures = gb.Features._innerList _
                    .Where(Function(feature) feature.KeyName <> "source") _
                    .ToArray
            End If

            Try
                For Each gene As Feature In geneFeatures
                    If geneName Then
                        locus_tag = gene.Query("gene")
                        If String.IsNullOrEmpty(locus_tag) OrElse String.Equals(locus_tag, "-") Then
                            locus_tag = gene.EnsureNonEmptyLocusId
                        End If
                    Else
                        locus_tag = gene.EnsureNonEmptyLocusId
                    End If

                    [function] = products.SafeGetValue(locus_tag)?.function
                    [function] = If([function].StringEmpty, products.SafeGetValue(locus_tag)?.commonName, [function])
                    loc = gene.Location.ContiguousRegion

                    If onlyLocus_tag Then
                        attrs = {locus_tag}
                    Else
                        attrs = {locus_tag, gene.Location.ToString, [function]}
                    End If

                    Sequence = reader.CutSequenceLinear(loc.left, loc.right).SequenceData
                    Sequence = If(gene.Location.Complement, NucleicAcid.Complement(Sequence), Sequence)

                    list += New FastaSeq(attrs, Sequence)
                Next
            Catch ex As Exception
                ex = New Exception(gb.ToString, ex)
                ex = New Exception(attrs.GetJson, ex)
                ex = New Exception(loc.GetJson, ex)
                ex = New Exception(gb.Accession.GetJson, ex)
                Call App.LogException(ex)
                Throw ex
            End Try

            Return New FASTA.FastaFile(list)
        End Function
    End Module
End Namespace
