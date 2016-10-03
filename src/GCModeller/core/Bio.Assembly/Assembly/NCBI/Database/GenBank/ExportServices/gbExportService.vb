#Region "Microsoft.VisualBasic::536eef4154323abe2a014f8c8b221d72, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\ExportServices\gbExportService.vb"

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

Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports System.Text.RegularExpressions
Imports System.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels

Namespace Assembly.NCBI.GenBank

    ''' <summary>
    ''' Genbank export methods collection.
    ''' </summary>
    ''' <remarks></remarks>
    Public Module gbExportService

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
        Public Function Distinct(data As Generic.IEnumerable(Of CsvExports.Plasmid)) As CsvExports.Plasmid()
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
                          Let Fasta = New TabularFormat.FastaObjects.GenomeSequence(SequenceModel.FASTA.FastaToken.Load(Path.Value))
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
        <Extension> Public Function LoadGbkSource(source As String, ParamArray ext As String()) As Dictionary(Of String, String)
            Dim LQuery = (From path As String
                          In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchAllSubDirectories, ext)
                          Select ID = TryParseGBKID(path),
                              path
                          Group By ID Into Group).ToArray
            Dim Dict = LQuery.ToDictionary(keySelector:=Function(item) item.ID, elementSelector:=Function(item) item.Group.First.path)
            Return Dict
        End Function

        ''' <summary>
        ''' 将GBK文件之中的基因的位置数据导出为PTT格式的数据
        ''' </summary>
        ''' <param name="Genbank">导出gene和RNA部分的数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function GbkffExportToPTT(Genbank As NCBI.GenBank.GBFF.File) As TabularFormat.PTT
            Dim genes As Feature() = (From feature As Feature
                                      In Genbank.Features._innerList
                                      Where String.Equals(feature.KeyName, "gene", StringComparison.OrdinalIgnoreCase) OrElse
                                          InStr(feature.KeyName, "RNA", CompareMethod.Text) > 0
                                      Select feature).ToArray
            Return genes.__toGenes(Genbank.Origin.SequenceData.Length, Genbank.Definition.Value)
        End Function

        <Extension> Private Function __toGenes(genes As Feature(), size As Integer, def As String) As TabularFormat.PTT
            Dim PTTGenes = (From GeneObject As Feature
                            In genes
                            Select Gene = __featureToPTT(GeneObject)
                            Group Gene By Gene.Synonym Into Group).ToArray
            Dim LQuery = (From GeneGr In PTTGenes Where GeneGr.Group.Count > 1 Select GeneGr).ToArray
            For Each DulGeneObject In LQuery
                Call VBDebugger.Warning($"""{DulGeneObject.Synonym}"" data was duplicated!")
            Next
            Return New TabularFormat.PTT With {
                .GeneObjects = (From gene In PTTGenes Select gene.Group.First).ToArray,
                .Size = size,
                .Title = def
            }
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="gb">只导出CDS部分的数据</param>
        ''' <returns></returns>
        <Extension> Public Function GbffToORF_PTT(gb As GBFF.File) As TabularFormat.PTT
            Dim Genes As Feature() = (From Feature As Feature
                                      In gb.Features._innerList
                                      Where String.Equals(Feature.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                                      Select Feature).ToArray
            Return Genes.__toGenes(gb.Origin.SequenceData.Length, gb.Definition.Value)
        End Function

        Private Function __featureToPTT(featureSite As Feature) As TabularFormat.ComponentModels.GeneBrief
            Dim loci As NucleotideLocation
            If featureSite.Location.Locations.IsNullOrEmpty Then
                loci = New NucleotideLocation
            Else
                loci = New NucleotideLocation(
                    featureSite.Location.Locations.First.Left,
                    featureSite.Location.Locations.Last.Right,
                    featureSite.Location.Complement)
            End If

            Dim locusId As String = featureSite.Query(FeatureQualifiers.locus_tag)
            Dim GB As New TabularFormat.ComponentModels.GeneBrief With {
                .Synonym = locusId,
                .PID = featureSite.Query(FeatureQualifiers.protein_id),
                .Product = featureSite.Query(FeatureQualifiers.product),
                .Gene = featureSite.Query(FeatureQualifiers.gene),
                .Location = loci
            }
            GB.Length = GB.Location.FragmentSize

            If String.IsNullOrEmpty(GB.Synonym) Then
                GB.Synonym = featureSite.Query(FeatureQualifiers.gene)
            End If
            If String.IsNullOrEmpty(GB.Synonym) Then
                GB.Synonym = featureSite.Location.UniqueId
            End If

            Return GB
        End Function

        <Extension> Public Function InvokeExport(gbk As GBFF.File, ByRef GeneList As GeneDumpInfo()) As KeyValuePair(Of gbEntryBrief, String)
            Dim LQuery = (From FeatureData As Feature
                          In gbk.Features._innerList.AsParallel
                          Where String.Equals(FeatureData.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                          Select GeneDumpInfo.DumpEXPORT(New CDS(FeatureData))).ToArray
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
                                    ByRef GeneList As GeneDumpInfo(),
                                    ByRef GBK As gbEntryBrief(),
                                    FastaExport As String,
                                    Optional FastaWithAnnotation As Boolean = False) As Integer

            Dim ExportList As Dictionary(Of CsvExports.gbEntryBrief, String) = New Dictionary(Of gbEntryBrief, String)
            Dim GeneChunkList As List(Of GeneDumpInfo) = New List(Of GeneDumpInfo)
            Dim FastaFile As FASTA.FastaFile = New FASTA.FastaFile
            Dim PlasmidList As FASTA.FastaFile = New FASTA.FastaFile
            Dim GeneSequenceList As FASTA.FastaFile = New FASTA.FastaFile

            Call "Flushed memory....".__DEBUG_ECHO
            Call FlushMemory()
            Call $"There is ""{list.Count}"" genome source will be export...".__DEBUG_ECHO

            Dim ExportLQuery = (From GBKFF As GBFF.File
                                In list.AsParallel
                                Let GenesTempChunk As GeneDumpInfo() = (From FeatureData As Feature
                                                                        In GBKFF.Features._innerList.AsParallel
                                                                        Where String.Equals(FeatureData.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                                                                        Select GeneDumpInfo.DumpEXPORT(New CDS(FeatureData))).ToArray
                                Let Entry = gbEntryBrief.ConvertObject(Of gbEntryBrief)(GBKFF)
                                Let FastaDump As FASTA.FastaFile =
                                    If(FastaWithAnnotation, __exportWithAnnotation(GenesTempChunk), __exportNoAnnotation(GenesTempChunk))
                                Let Plasmid As FASTA.FastaToken =
                                    New FASTA.FastaToken With {
                                        .Attributes = New String() {Entry.AccessionID},
                                        .SequenceData = GBKFF.Origin.SequenceData.ToUpper
                                    }
                                Let Reader = New SequenceModel.NucleotideModels.SegmentReader(GBKFF.Origin.SequenceData, False)
                                Let GeneFastaDump = CType((From GeneObject In GBKFF.Features._innerList.AsParallel
                                                           Where String.Equals(GeneObject.KeyName, "gene", StringComparison.OrdinalIgnoreCase)
                                                           Let loc = GeneObject.Location.ContiguousRegion
                                                           Let Sequence As String = Reader.GetSegmentSequence(loc.Left, loc.Right)
                                                           Select New FASTA.FastaToken With {
                                                               .Attributes = New String() {GeneObject.Query("locus_tag"), GeneObject.Location.ToString},
                                                               .SequenceData = If(GeneObject.Location.Complement, NucleicAcid.Complement(Sequence), Sequence)
                                                           }).ToArray, FASTA.FastaFile)
                                Select GBKFF,
                                    GenesTempChunk,
                                    Entry,
                                    FastaDump,
                                    Plasmid,
                                    Reader,
                                    GeneFastaDump).ToArray

            For Each item In ExportLQuery
                Call GeneChunkList.AddRange(item.GenesTempChunk)
                Call ExportList.Add(item.Entry, item.Entry.AccessionID)
                Call item.FastaDump.Save(FastaExport & "/Orf/" & item.Entry.AccessionID & ".fasta")
                Call item.Plasmid.SaveTo(FastaExport & "/Genomes/" & item.Entry.AccessionID & ".fasta")
                Call item.GeneFastaDump.Save(FastaExport & "/Genes/" & item.Entry.AccessionID & ".fasta")

                Call FastaFile.AddRange(item.FastaDump)
                Call PlasmidList.Add(item.Plasmid)
                Call GeneSequenceList.AddRange(item.GeneFastaDump)
            Next

            GeneList = GeneChunkList.ToArray
            GBK = (From entryInfo In ExportList Select entryInfo.Key).ToArray

            Try
                Call FastaFile.Save(FastaExport & "/CDS.Gene.fasta")
                Call PlasmidList.Save(FastaExport & "/Genbank.ORIGINS.fasta")
                Call GeneSequenceList.Save(FastaExport & "/Gene.Nt.fasta")
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
        <Extension> Public Function ExportGeneAnno(gbk As GBFF.File) As GeneDumpInfo()
            Dim dumps As GeneDumpInfo() = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
                From feature As Feature
                In gbk.Features._innerList.AsParallel
                Where String.Equals(feature.KeyName, "CDS", StringComparison.OrdinalIgnoreCase)
                Select GeneDumpInfo.DumpEXPORT(New CDS(feature))

            Return dumps
        End Function

        <Extension> Public Function ExportPTTAsDump(PTT As NCBI.GenBank.TabularFormat.PTT) As GeneDumpInfo()
            Dim LQuery As GeneDumpInfo() = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
                From gene As GeneBrief
                In PTT.GeneObjects.AsParallel
                Select New GeneDumpInfo With {
                    .CDS = "",
                    .COG = gene.COG,
                    .CommonName = gene.Gene,
                    .EC_Number = "-",
                    .Function = gene.Product,
                    .GC_Content = 0,
                    .GeneName = gene.Gene,
                    .GI = "-",
                    .GO = "-",
                    .InterPro = {},
                    .Left = gene.Location.Left,
                    .Length = gene.Location.FragmentSize,
                    .Location = gene.Location,
                    .LocusID = gene.Synonym,
                    .ProteinId = gene.Synonym,
                    .Right = gene.Location.Right,
                    .Species = "",
                    .SpeciesAccessionID = "",
                    .Strand = gene.Location.Strand.ToString,
                    .Translation = "",
                    .Transl_Table = "",
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
                                           ByRef GeneList As GeneDumpInfo(),
                                           ByRef GBK As Plasmid(),
                                           FastaExport As String,
                                           Optional FastaWithAnnotation As Boolean = False) As Integer
            Dim ExportList As New Dictionary(Of Plasmid, String)
            Dim buf As New List(Of GeneDumpInfo)
            Dim FastaFile As New FASTA.FastaFile
            Dim PlasmidList As New FASTA.FastaFile
            Dim GeneSequenceList As New FASTA.FastaFile

            Dim Source As GBFF.File() = (From gb As GBFF.File
                                         In list.AsParallel
                                         Where gb.IsPlasmidSource
                                         Select gb).ToArray

            Call "Flushed memory....".__DEBUG_ECHO
            list = Nothing
            Call FlushMemory()
            Call $"There is ""{Source.Length}"" plasmid source will be export...".__DEBUG_ECHO

            For Each gb As GBFF.File In Source
                Dim cds As GeneDumpInfo() = gb.ExportGeneAnno
                Dim Entry = NCBI.GenBank.CsvExports.Plasmid.Build(gb)

                Call ExportList.Add(Entry, gb.Origin.SequenceData)
                Call buf.AddRange(cds)

                '导出Fasta序列
                Dim FastaDump As FASTA.FastaFile =
                    If(FastaWithAnnotation,
                    __exportWithAnnotation(cds),
                    __exportNoAnnotation(cds))

                If FastaDump.Count > 0 Then
                    Call FastaDump.Save(String.Format("{0}/plasmid_cds/{1}.fasta", FastaExport, gb.Accession.AccessionId))
                    Call FastaFile.AddRange(FastaDump)
                End If

                Dim Plasmid As New FASTA.FastaToken With {
                        .Attributes = New String() {Entry.AccessionID & "_" & Entry.PlasmidID.Replace("-", "_")},
                        .SequenceData = gb.Origin.SequenceData.ToUpper
                }

                Call PlasmidList.Add(Plasmid)
                Call Plasmid.SaveTo(String.Format("{0}/plasmids/{1}.fasta", FastaExport, gb.Accession.AccessionId))

                Dim Reader As New SegmentReader(gb.Origin.SequenceData, False)
                Dim GeneFastaDump = CType((From GeneObject In gb.Features._innerList.AsParallel
                                           Where String.Equals(GeneObject.KeyName, "gene", StringComparison.OrdinalIgnoreCase)
                                           Let loc = GeneObject.Location.ContiguousRegion
                                           Let Sequence As String = Reader.GetSegmentSequence(loc.Left, loc.Right)
                                           Select New FASTA.FastaToken With {
                                               .Attributes = New String() {GeneObject.Query("locus_tag"), GeneObject.Location.ToString},
                                               .SequenceData = If(GeneObject.Location.Complement, NucleicAcid.Complement(Sequence), Sequence)
                                           }).ToArray, FASTA.FastaFile)

                If GeneFastaDump.Count > 0 Then
                    Call GeneSequenceList.AddRange(GeneFastaDump.ToArray)
                    Call GeneFastaDump.Save(String.Format("{0}/plasmid_genes/{1}.fasta", FastaExport, gb.Accession.AccessionId))
                    Call GeneFastaDump.FlushData()
                End If
            Next

            GeneList = buf.ToArray
            GBK = (From item In ExportList Select item.Key).ToArray

            Try
                Call FastaFile.Save(FastaExport & "/CDS_GENE.fasta")
                Call PlasmidList.Save(FastaExport & "/GBKFF.ORIGINS_plasmid.fasta")
                Call GeneSequenceList.Save(FastaExport & "/GENE_SEQUENCE.fasta")
            Catch ex As Exception

            End Try

            Return ExportList.Count
        End Function

        Private Function __exportNoAnnotation(data As GeneDumpInfo()) As FASTA.FastaFile
            Dim LQuery As IEnumerable(Of FASTA.FastaToken) =
                From gene As GeneDumpInfo
                In data.AsParallel
                Let fa As FASTA.FastaToken =
                    New FASTA.FastaToken With {
                        .Attributes = New String() {gene.LocusID},
                        .SequenceData = gene.Translation
                    }
                Select fa
            Return New FASTA.FastaFile(LQuery)
        End Function

        Private Function __exportWithAnnotation(data As GeneDumpInfo()) As FASTA.FastaFile
            Dim LQuery = From gene As GeneDumpInfo
                         In data.AsParallel
                         Let attrs As String() = {gene.LocusID, gene.GeneName, gene.GI, gene.CommonName, gene.Function, gene.Species}
                         Select New FASTA.FastaToken With {
                             .Attributes = attrs,
                             .SequenceData = gene.Translation
                         }
            Return New FASTA.FastaFile(LQuery)
        End Function

        <Extension> Public Function TryParseGBKID(path As String) As String
            Dim Name As String = IO.Path.GetFileNameWithoutExtension(path)
            Name = Regex.Replace(Name, "\.\d+", "")
            Return Name.ToUpper
        End Function

        ''' <summary>
        ''' {locus_tag, gene.Location.ToString, products.SafeGetValue(locus_tag)?.Function}.
        ''' (导出每一个基因的核酸序列)
        ''' </summary>
        ''' <param name="gb">Genbank数据库文件</param>
        ''' <returns></returns>
        <Extension>
        Public Function ExportGeneNtFasta(gb As GBFF.File, Optional geneName As Boolean = False) As FASTA.FastaFile
            Dim Reader As New NucleotideModels.SegmentReader(gb.Origin.SequenceData, False)
            Dim list As New List(Of FASTA.FastaToken)
            Dim loc As NucleotideLocation = Nothing
            Dim attrs As String() = Nothing
            Dim Sequence As String
            Dim products As Dictionary(Of GeneDumpInfo) = gb.ExportGeneAnno.ToDictionary

            Try
                For Each gene As Feature In (From x As Feature
                                             In gb.Features._innerList.AsParallel
                                             Where String.Equals(x.KeyName, "gene", StringComparison.OrdinalIgnoreCase)
                                             Select x)

                    Dim locus_tag As String

                    If geneName Then
                        locus_tag = gene.Query("gene")
                        If String.IsNullOrEmpty(locus_tag) OrElse String.Equals(locus_tag, "-") Then
                            locus_tag = gene.Query("locus_tag")
                        End If
                    Else
                        locus_tag = gene.Query("locus_tag")
                    End If

                    loc = gene.Location.ContiguousRegion
                    attrs = {locus_tag, gene.Location.ToString, products.SafeGetValue(locus_tag)?.Function}
                    Sequence = Reader.GetSegmentSequence(loc.Left, loc.Right)
                    Sequence = If(gene.Location.Complement, NucleotideModels.NucleicAcid.Complement(Sequence), Sequence)

                    list += New FASTA.FastaToken(attrs, Sequence)
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
