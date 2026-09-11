' /********************************************************************************/
'
'  Rockhopper —— 对外 API（ExportAPI 封装层）
'
'  由原始 `API/RockhopperAPI.vb` 迁移而来。与原实现的差异：
'    * **移除了全部外部 rockhopper.jar 调用**：Install / CliCommon /
'      RockhopperExternalCli / DE_NOVO_ASSEMBLY（按重写计划不再依赖本机 java 与 jar）；
'    * `Microsoft.VisualBasic.DocumentFormat.Csv` → `Microsoft.VisualBasic.Data.Framework.IO`；
'    * `LANS.SystemsBiology.*` → `SMRUCC.genomics.*`；
'    * KEGG 富集迁移到 `Regulation.KEGGEnrichment`。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports DataFrameFile = Microsoft.VisualBasic.Data.Framework.IO.File
Imports RowObject = Microsoft.VisualBasic.Data.Framework.IO.RowObject

Namespace AnalysisAPI

    <[Package]("Rockhopper",
               Description:="Computational analysis of bacterial RNA-Seq data",
               Publisher:="McClure, R.; Balasubramanian, D.; Sun, Y.; Bobrovskyy, M.; Sumby, P.; Genco, C. A.; Vanderpool, C. K.; Tjaden, B.",
               Cites:="McClure, R., et al. (2013). ""Computational analysis of bacterial RNA-Seq data."" Nucleic Acids Res 41(14): e140.",
               Url:="http://cs.wellesley.edu/~btjaden/Rockhopper")>
    Public Module RockhopperAPI

        ''' <summary>
        ''' 运行 Rockhopper 分析（等价于命令行调用）。
        ''' </summary>
        <ExportAPI("Run.Analysis")>
        Public Function RunProgram(argv As IEnumerable(Of String)) As Integer
            Return CLI.Main(argv.ToArray())
        End Function

        ''' <summary>
        ''' 读取 *_transcripts.txt。
        ''' </summary>
        <ExportAPI("Load.Transcripts")>
        Public Function LoadTranscriptResult(Path As String) As Transcripts()
            Return TSSsAnalysis.LoadResult(Path)
        End Function

        ''' <summary>
        ''' 写出转录本结果 CSV。
        ''' </summary>
        <ExportAPI("Write.Csv.Transcripts")>
        Public Function SaveData(data As IEnumerable(Of Transcripts), SaveTo As String) As Boolean
            Return data.ToCsvDoc(False).Save(SaveTo, Microsoft.VisualBasic.Text.Encodings.UTF8)
        End Function

        ''' <summary>
        ''' 读取操纵子结果并转换为 DOOR 模型。
        ''' </summary>
        <ExportAPI("Load.Operons")>
        Public Function LoadOperonsAsDoor(Path As String, PTT As PTT) As DOOR
            Dim predicted As Operon() = TSSsAnalysis.LoadOperonResult(Path)
            predicted = TSSsAnalysis.SubstituteID(predicted, PTT)
            Return TSSsAnalysis.GenerateDoorOperon(predicted, PTT)
        End Function

        ''' <summary>
        ''' 比较两个条件下的操纵子结构，返回不一致的基因。
        ''' </summary>
        ''' <remarks>返回空数组表示两个条件的预测结果一致。</remarks>
        Public Function OperonDiffTest(Condition1 As DOOR, Condition2 As DOOR) As String()
            Dim test1 As String() = (From Operon In Condition1.DOOROperonView.Operons Order By Operon.TestGuid Ascending Select Operon.TestGuid).ToArray()
            Dim test2 As List(Of String) = (From Operon In Condition2.DOOROperonView.Operons Select Operon.TestGuid).ToList()
            Return (From guid As String In test1 Where test2.IndexOf(guid) = -1 Select guid).ToArray()
        End Function

        ''' <summary>
        ''' 条件间 TSS/TTS 差异。
        ''' </summary>
        <ExportAPI("TSSs.Different")>
        Public Function DifferentTSSs(condition1 As IEnumerable(Of Transcripts), condition2 As IEnumerable(Of Transcripts)) As Regulation.TSSsDifferent()
            Return TSSsAnalysis.DifferentTSSs(condition1.ToArray(), condition2.ToArray())
        End Function

        ''' <summary>写出差异结果 CSV。</summary>
        <ExportAPI("Write.Csv.Different")>
        Public Function SaveDifferent(data As IEnumerable(Of Regulation.TSSsDifferent), SaveTo As String) As Boolean
            Return data.ToCsvDoc(False).Save(SaveTo, Microsoft.VisualBasic.Text.Encodings.UTF8)
        End Function

        ''' <summary>
        ''' KEGG 通路富集分析（差异 TSS/TTS 基因）。
        ''' </summary>
        <ExportAPI("KEGG.Different")>
        Public Function KEGGAnalysis(TSSS As IEnumerable(Of Regulation.TSSsDifferent), KEGG As String) As DataFrameFile
            Dim report As List(Of Regulation.EnrichmentRow) = Regulation.KEGGEnrichment.BuildReport(TSSS)
            Dim rows As New List(Of RowObject)()
            For Each row As Regulation.EnrichmentRow In report
                rows.Add(New RowObject(row.Group, row.Pathway, CStr(row.Count), String.Join("; ", row.Genes)))
            Next
            Return New DataFrameFile(rows)
        End Function

        ''' <summary>
        ''' 对 TSS 位点进行六分类并输出明细表。
        ''' </summary>
        <ExportAPI("TSSs.Category")>
        Public Function TSSsCategories(TSSs As IEnumerable(Of Transcripts), PTT As PTT, Optional Fasta As FastaSeq = Nothing) As DataFrameFile
            Dim rows As New List(Of RowObject)()
            For Each transcript As Transcripts In TSSs
                Dim relatedGene As GeneBrief = Nothing
                Dim category As Transcripts.Categories = TSSsCategory.Category(transcript, PTT, relatedGene)
                rows.Add(New RowObject(
                    transcript.GetTULoci().ToString,
                    transcript.Synonym,
                    category.ToString,
                    CStr(transcript.ATG),
                    CStr(transcript.TGA),
                    transcript.Strand,
                    CStr(transcript.TSSs),
                    CStr(transcript.TTSs),
                    CStr(transcript.Minus35BoxLoci),
                    CStr(transcript.Leaderless),
                    CStr(transcript.Is_sRNA),
                    CStr(transcript.IsRNA),
                    CStr(transcript.IsPredictedRNA),
                    If(relatedGene Is Nothing, "", relatedGene.Synonym),
                    If(relatedGene Is Nothing, "", relatedGene.Location.ToString)))
            Next
            Return New DataFrameFile(rows)
        End Function

        ''' <summary>
        ''' 解析 5'UTR / 启动子 / TSS / TTS 区域序列并保存为 FASTA。
        ''' </summary>
        <ExportAPI("Parsing.Region")>
        Public Function ParsingRegionSequence(data As IEnumerable(Of Transcripts), Genome As FastaSeq, Export As String) As Boolean
            Dim folder As String = If(String.IsNullOrEmpty(Export), "./", Export)
            If Not System.IO.Directory.Exists(folder) Then Call System.IO.Directory.CreateDirectory(folder)

            Dim structures As GeneStructures = New GeneStructures With {
                .PromoterBox = TSSsAnalysis.ParsingPromoterBox(data.ToArray(), Genome),
                ._5UTR = TSSsAnalysis.Parsing5UTR(data.ToArray(), Genome),
                .TTSs = TSSsAnalysis.ParsingTTSs(data.ToArray(), Genome),
                .TSSs = TSSsAnalysis.ParsingTSSs(data.ToArray(), Genome)
            }

            Return structures.Save(folder, System.Text.Encoding.ASCII)
        End Function

        ''' <summary>
        ''' de novo 结果（transcripts.txt）转 FASTA。
        ''' </summary>
        Public Function TransFasta(denovol As String, Optional Saved As String = "") As FastaFile
            Dim fasta As FastaFile = DeNovolTranscript.LoadDocument(denovol)
            If Not String.IsNullOrEmpty(Saved) Then Call fasta.Save(Saved)
            Return fasta
        End Function

        ''' <summary>
        ''' 批量把各子目录下的 de novo transcripts.txt 导出为 FASTA。
        ''' </summary>
        <ExportAPI("TransFasta")>
        Public Function TransFastaBatch(SourceDir As String, ExportDir As String) As Boolean
            If Not System.IO.Directory.Exists(ExportDir) Then Call System.IO.Directory.CreateDirectory(ExportDir)

            Dim files As String() = System.IO.Directory.GetFiles(SourceDir, "*transcripts.txt", System.IO.SearchOption.AllDirectories)
            For Each path As String In files
                Dim name As String = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path))
                Dim fasta As FastaFile = DeNovolTranscript.LoadDocument(path)
                Call fasta.Save(System.IO.Path.Combine(ExportDir, name & ".fasta"))
            Next
            Return True
        End Function

        ''' <summary>
        ''' 统计目录下 FASTA 文件的序列长度分布。
        ''' </summary>
        <ExportAPI("Length-Distrib")>
        Public Function LengthDistributions(DIR As String, Optional FilterKey As String = "*.fasta") As DataFrameFile
            Dim rows As New List(Of RowObject)()
            If Not System.IO.Directory.Exists(DIR) Then Return New DataFrameFile(rows)

            For Each path As String In System.IO.Directory.GetFiles(DIR, FilterKey, System.IO.SearchOption.AllDirectories)
                Dim name As String = System.IO.Path.GetFileNameWithoutExtension(path)
                Dim distribution As Dictionary(Of Integer, Integer) =
                    FastaFile.Read(path).GroupBy(Function(f) f.Length).ToDictionary(Function(g) g.Key, Function(g) g.Count())

                For Each length In distribution.Keys.OrderBy(Function(k) k)
                    rows.Add(New RowObject(name, CStr(length), CStr(distribution(length))))
                Next
            Next

            Return New DataFrameFile(rows)
        End Function

        ''' <summary>
        ''' 由基因关联视图构造转录本模型（对应原始 Run.Analysis 的模型生成步骤）。
        ''' </summary>
        <ExportAPI("CreateModel")>
        Public Function GenerateModelData(data As IEnumerable(Of Core.Transcript)) As Transcripts()
            Return data.Select(Function(t) New Transcripts With {
                .TSSs = t.TSSs,
                .ATG = t.ATG,
                .TGA = t.TGA,
                .TTSs = t.TTSs,
                .Strand = t.Strand.ToString,
                .Name = t.Name,
                .Synonym = t.Synonym,
                .Product = t.Product,
                .Expression = CLng(t.Expression),
                .QValue = t.QValue
            }).ToArray()
        End Function

        ''' <summary>
        ''' 4 个序列结构集合（5'UTR / 启动子 / TSS / TTS）。
        ''' </summary>
        Public Structure GeneStructures

            Public Property PromoterBox As FastaFile
            Public Property _5UTR As FastaFile
            Public Property TTSs As FastaFile
            Public Property TSSs As FastaFile

            Public Function Save(Optional ExportDir As String = "", Optional encoding As System.Text.Encoding = Nothing) As Boolean
                If encoding Is Nothing Then encoding = System.Text.Encoding.ASCII
                If String.IsNullOrEmpty(ExportDir) Then ExportDir = "./"
                If Not System.IO.Directory.Exists(ExportDir) Then Call System.IO.Directory.CreateDirectory(ExportDir)

                Dim a As Boolean = saveFasta(PromoterBox, System.IO.Path.Combine(ExportDir, "PromoterBox.fasta"), encoding)
                Dim b As Boolean = saveFasta(_5UTR, System.IO.Path.Combine(ExportDir, "5_UTR.fasta"), encoding)
                Dim c As Boolean = saveFasta(TTSs, System.IO.Path.Combine(ExportDir, "TTSs.fasta"), encoding)
                Dim d As Boolean = saveFasta(TSSs, System.IO.Path.Combine(ExportDir, "TSSs.fasta"), encoding)

                Return a AndAlso b AndAlso c AndAlso d
            End Function

            Private Function saveFasta(fasta As FastaFile, path As String, encoding As System.Text.Encoding) As Boolean
                If fasta Is Nothing Then Return True
                Return fasta.Save(path, encoding)
            End Function

            Public Shared Function Load(Dir As String) As GeneStructures
                Return New GeneStructures With {
                    .PromoterBox = FastaFile.Read(System.IO.Path.Combine(Dir, "PromoterBox.fasta")),
                    .TSSs = FastaFile.Read(System.IO.Path.Combine(Dir, "TSSs.fasta")),
                    .TTSs = FastaFile.Read(System.IO.Path.Combine(Dir, "TTSs.fasta")),
                    ._5UTR = FastaFile.Read(System.IO.Path.Combine(Dir, "5_UTR.fasta"))
                }
            End Function

        End Structure

    End Module

End Namespace
