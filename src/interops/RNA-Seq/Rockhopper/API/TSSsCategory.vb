' /********************************************************************************/
'
'  Rockhopper —— TSS 六分类（API 包装）
'
'  算法实现位于 `Transcription.TSSsCategory`；本模块仅把 API 层的
'  <see cref="Transcripts"/> 模型适配到该实现，并保留原有的 ExportAPI 入口名称。
'
' /********************************************************************************/

Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace AnalysisAPI

    ''' <summary>
    ''' 进行 TSS 位点的分类处理。
    ''' </summary>
    Public Module TSSsCategory

        ''' <summary>
        ''' 对单个 TSS 位点进行分类。
        ''' </summary>
        ''' <param name="Transcript">转录本。</param>
        ''' <param name="PTT">注释表（可为 Nothing）。</param>
        ''' <param name="RelatedGene">输出：与之相关的注释基因。</param>
        ''' <param name="PutativeMRNA">是否已由外部 ORF 查找判定为假定 mRNA。</param>
        Public Function Category(Transcript As Transcripts, PTT As PTT,
                                 ByRef RelatedGene As GeneBrief,
                                 Optional PutativeMRNA As Boolean = False) As Transcripts.Categories

            Dim loci As NucleotideLocation = Transcript.GetTULoci()
            Dim tssCategory As Transcription.TssCategory = Transcription.TSSsCategory.Category(
                CInt(Transcript.TSSs),
                CInt(Transcript.ATG),
                CInt(Transcript.TGA),
                loci,
                Transcript.IsRNA,
                Transcript.Is_sRNA,
                Transcript.Synonym,
                PTT,
                PutativeMRNA,
                RelatedGene)

            Return mapCategory(tssCategory)
        End Function

        ''' <summary>转录本视图 → API 分类模型。</summary>
        Public Function CreateModel(Transcript As TranscriptView) As Transcripts
            Dim geneType As Transcription.GeneTypes = Transcription.TSSsCategory.ParseGeneType(Transcript.Type)
            Return New Transcripts With {
                .ATG = If(geneType = Transcription.GeneTypes.misc_RNA, 0, Transcript.gpStart),
                .TGA = If(geneType = Transcription.GeneTypes.misc_RNA, 0, Transcript.gpStop),
                .Name = Transcript.GeneId,
                .Synonym = Transcript.TSS_id,
                .Strand = Transcript.Strand,
                .TSSs = Transcript.pStart,
                .TTSs = Transcript.pStop
            }
        End Function

        ''' <summary>字符串 → 分类枚举。</summary>
        Public Function GetCType(str As String) As Transcripts.Categories
            Return mapCategory(Transcription.TSSsCategory.GetCType(str))
        End Function

        Private Function mapCategory(category As Transcription.TssCategory) As Transcripts.Categories
            Select Case category
                Case Transcription.TssCategory.mTSS : Return Transcripts.Categories.mTSS
                Case Transcription.TssCategory.lmTSS : Return Transcripts.Categories.lmTSS
                Case Transcription.TssCategory.ULmTSS : Return Transcripts.Categories.ULmTSS
                Case Transcription.TssCategory.pmTSS : Return Transcripts.Categories.pmTSS
                Case Transcription.TssCategory.asTSS : Return Transcripts.Categories.asTSS
                Case Transcription.TssCategory.seTSS : Return Transcripts.Categories.seTSS
                Case Transcription.TssCategory.sTSS : Return Transcripts.Categories.sTSS
                Case Else : Return Transcripts.Categories.UnClassified
            End Select
        End Function

    End Module

End Namespace
