' /********************************************************************************/
'
'  Rockhopper —— 主流程编排
'
'  复刻原始 Rockhopper `Java/Rockhopper/Rockhopper.vb` 的执行顺序：
'    参考依赖模式：
'       加载基因组 → 比对读段 → 构建覆盖度 → 表达定量（上四分位归一化 + 改良 RPKM）
'       → 差异表达（负二项 + lowess + BH）→ TSS/TTS 边界与 UTR/ncRNA
'       → 操纵子预测 → 结果输出（summary/transcripts/operons/SAM/WIG）
'    de novo 模式：
'       读段 → de Bruijn 组装 → 二次比对精修 → transcripts.txt
'
'  外部 rockhopper.jar 调用（Install / RockhopperExternalCli / DE_NOVO_ASSEMBLY）已移除。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports Core

''' <summary>
''' Rockhopper 主流程。
''' </summary>
Public Module RockhopperPipeline

    ''' <summary>
    ''' 执行一次完整分析，返回进程退出码（0 表示成功）。
    ''' </summary>
    Public Function Run(parameters As RockhopperParameters) As Integer
        If parameters Is Nothing Then Throw New ArgumentNullException(NameOf(parameters))
        Call parameters.Normalize()

        Logging.Verbose = parameters.Verbose

        Dim watch As Stopwatch = Stopwatch.StartNew()

        ' 输出目录与 summary.txt
        If Not Directory.Exists(parameters.OutputDirectory) Then
            Call Directory.CreateDirectory(parameters.OutputDirectory)
        End If
        Dim summaryPath As String = Path.Combine(parameters.OutputDirectory, parameters.SummaryFile)
        Using summaryWriter As New StreamWriter(summaryPath, False)
            Logging.SummaryWriter = summaryWriter
            Try
                If parameters.IsDeNovo Then
                    Call runDeNovo(parameters)
                Else
                    Call runReferenceBased(parameters)
                End If
            Catch ex As Exception
                Call Logging.Output($"{vbLf}[ERROR] {ex.GetType().Name}: {ex.Message}{vbLf}")
                Return 1
            Finally
                Logging.SummaryWriter = Nothing
            End Try
        End Using

        watch.Stop()
        If parameters.Time Then
            Call Logging.Output($"Execution time:{vbTab}{watch.Elapsed.TotalMinutes:F0} minutes {watch.Elapsed.Seconds} seconds{vbLf}")
        End If

        Return 0
    End Function

#Region "Reference-based analysis"

    Private Sub runReferenceBased(parameters As RockhopperParameters)
        Call Logging.Output($"[{Now}] Loading {parameters.GenomeDirectories.Count} reference genome(s)...{vbLf}")

        Dim genomes As New List(Of Genome)()
        For Each directory As String In parameters.GenomeDirectories
            genomes.Add(FileIO.GenomeReader.Load(directory))
        Next

        ' 复制子（用于重建覆盖度）
        Dim replicons As List(Replicon) = genomes.Select(Function(g, i) New Replicon(g.Name, g.Sequence.Substring(1))).ToList()

        ' 条件文件解析（条件内用逗号分隔，mate-pair 用 % 分隔）
        Dim conditions As List(Of Condition) = buildConditions(parameters)

        Call Logging.Output($"[{Now}] Aligning sequencing reads to reference genome(s)...{vbLf}")
        Dim aligner As New Alignment.Aligner(replicons.ToArray(),
                                             percentMismatches:=parameters.PercentMismatches,
                                             percentSeedLength:=parameters.PercentSeedLength,
                                             stopAfterOneHit:=parameters.StopAfterOneHit,
                                             unstranded:=parameters.Unstranded,
                                             numThreads:=parameters.NumThreads,
                                             maxPairedEndLength:=parameters.MaxPairedEndLength,
                                             singleEndReverseComplement:=parameters.SingleEndOrientationReverseComplement,
                                             pairedEndOrientation:=parameters.PairedEndOrientation)
        aligner.BuildIndex()

        Dim samRecords As New List(Of FileIO.SamRecord)()

        For c As Integer = 0 To parameters.ConditionFiles.Count - 1
            Dim files As String() = parameters.ConditionFiles(c).Split(","c)
            For Each file As String In files
                Dim mates As String() = file.Split("%"c)
                Dim readFile As String = mates(0)
                If Not File.Exists(readFile) Then Continue For

                Dim hits As IEnumerable(Of Alignment.AlignmentHit)
                If mates.Length > 1 AndAlso File.Exists(mates(1)) Then
                    hits = aligner.AlignPaired(FileIO.ReadsReader.ReadAll(mates(0)), FileIO.ReadsReader.ReadAll(mates(1)))
                Else
                    hits = aligner.AlignReads(FileIO.ReadsReader.ReadAll(readFile))
                End If

                Dim hitArray As Alignment.AlignmentHit() = hits.ToArray()
                Dim coverages As Core.AlignmentCoverage() = aligner.BuildCoverages(hitArray, parameters.Unstranded, readFile)

                ' 保存压缩覆盖度（供重复分析复用/调试）
                Dim coveragePath As String = Path.Combine(parameters.OutputDirectory, Path.GetFileNameWithoutExtension(readFile) & ".coverage.txt")
                Call FileIO.CoverageStore.Save(coveragePath, coverages)

                Dim replicate As New Replicate(coverages, parameters.Unstranded)
                conditions(c).AddReplicate(replicate)

                If parameters.OutputSAM Then
                    For Each hit In hitArray
                        samRecords.Add(toSamRecord(hit, replicons(hit.RepliconIndex).Name, hitArray))
                    Next
                End If

                Call Logging.Output($"[{Now}] Aligned {hitArray.Length} reads from {Path.GetFileName(readFile)}{vbLf}")
            Next
            conditions(c).SetMinDiffExpressionLevel()
        Next

        If parameters.OutputSAM Then
            Dim samPath As String = Path.Combine(parameters.OutputDirectory, "alignments.sam")
            Call FileIO.SamWriter.Write(samPath, samRecords, replicons.Select(Function(r) (r.Name, r.Length)))
            Call Logging.Output($"[{Now}] SAM file written to {samPath}{vbLf}")
        End If

        ' 表达定量
        If parameters.ComputeExpression Then
            Call Logging.Output($"[{Now}] Quantifying gene expression...{vbLf}")
            Call Quantification.Quantification.ComputeUpperQuartiles(genomes, conditions)
            Call Quantification.Quantification.SetNormalizedCounts(genomes, conditions)
            Call Quantification.Quantification.ComputeExpressions(genomes, conditions)
        End If

        ' 差异表达
        Dim transcripts As New List(Of Transcript)()
        If parameters.ComputeExpression AndAlso conditions.Count > 1 Then
            Call Logging.Output($"[{Now}] Testing for differential expression...{vbLf}")
            Call Quantification.Quantification.ComputeDifferentialExpressions(genomes, conditions)
        End If

        ' 转录边界与 TSS/TTS
        If parameters.ComputeTranscripts Then
            Call Logging.Output($"[{Now}] Identifying transcript boundaries and ncRNAs...{vbLf}")
            For z As Integer = 0 To genomes.Count - 1
                Dim inferred As List(Of Transcript) = Transcription.TSSsInference.InferBoundaries(
                    genomes(z), replicons(z).Length, conditions, parameters.TranscriptSensitivity)

                Dim geneMap As Dictionary(Of String, Gene) =
                    genomes(z).Genes.Where(Function(g) Not String.IsNullOrEmpty(g.Synonym)) _
                              .GroupBy(Function(g) g.Synonym) _
                              .ToDictionary(Function(g) g.Key, Function(g) g.First())

                For Each t As Transcript In inferred
                    t.Replicon = replicons(z).Name
                    Dim gene As Gene = Nothing
                    If Not String.IsNullOrEmpty(t.Synonym) AndAlso geneMap.TryGetValue(t.Synonym, gene) Then
                        ' 把预测得到的边界写回基因，供 *_transcripts.txt 使用
                        gene.StartT = t.TSSs
                        gene.StopT = t.TTSs
                        If conditions.Count > 0 Then
                            t.Expression = CDbl(gene.GetAvg(0))
                            t.QValue = If(gene.HasQvalue(0), gene.qValues(0), 1.0)
                        End If
                    ElseIf t.IsPredicted Then
                        ' 基因间区预测得到的新 ncRNA / sRNA，单独输出
                        transcripts.Add(t)
                    End If
                Next
            Next
        End If

        ' 结果输出
        For z As Integer = 0 To genomes.Count - 1
            Dim expressionPath As String = Path.Combine(parameters.OutputDirectory, $"{replicons(z).Name}_transcripts.txt")
            Call FileIO.ResultWriter.WriteTranscripts(expressionPath, genomes(z), conditions, parameters.Labels)
            Call Logging.Output($"[{Now}] Transcripts written to {expressionPath}{vbLf}")
        Next

        ' 新预测转录本（ncRNA / sRNA）
        If transcripts.Count > 0 Then
            Dim novelPath As String = Path.Combine(parameters.OutputDirectory, "novel_transcripts.txt")
            Using writer As New StreamWriter(novelPath, False, System.Text.Encoding.ASCII)
                writer.WriteLine($"TSS{vbTab}TTS{vbTab}Strand{vbTab}Length{vbTab}Replicon{vbTab}Expression{vbTab}QValue")
                For Each t As Transcript In transcripts
                    writer.WriteLine($"{t.TSSs}{vbTab}{t.TTSs}{vbTab}{t.Strand}{vbTab}{t.TranscriptLength}{vbTab}{t.Replicon}{vbTab}{t.Expression:F4}{vbTab}{t.QValue:F6}")
                Next
            End Using
            Call Logging.Output($"[{Now}] {transcripts.Count} novel transcripts written to {novelPath}{vbLf}")
        End If

        ' 操纵子预测
        If parameters.ComputeOperons Then
            Call Logging.Output($"[{Now}] Predicting operons...{vbLf}")
            For z As Integer = 0 To genomes.Count - 1
                Dim operons As List(Of Operon) = Operons.OperonPrediction.Predict(genomes(z), conditions.Count)
                Dim pairs As List(Of OperonGenePair) = Operons.OperonPrediction.GenePairs(genomes(z), conditions.Count)

                Dim pairPath As String = Path.Combine(parameters.OutputDirectory, $"{replicons(z).Name}_{parameters.OperonGenePairFile}")
                Using writer As New StreamWriter(pairPath, False)
                    For Each pair In pairs
                        writer.WriteLine(pair.ToString())
                    Next
                End Using

                Dim operonPath As String = Path.Combine(parameters.OutputDirectory, $"{replicons(z).Name}_{parameters.OperonMergedFile}")
                Call FileIO.ResultWriter.WriteOperons(operonPath, operons)
                Call Logging.Output($"[{Now}] {operons.Count} operons written to {operonPath}{vbLf}")
            Next
        End If

        ' 基因组浏览器文件（UTRs / Novel RNAs / 差异表达基因）
        If parameters.ComputeTranscripts Then
            Dim browserDir As String = Path.Combine(parameters.OutputDirectory, parameters.BrowserDirectory)
            If Not Directory.Exists(browserDir) Then Call Directory.CreateDirectory(browserDir)
            For z As Integer = 0 To genomes.Count - 1
                Call FileIO.WigWriter.WriteUTRs(Path.Combine(browserDir, $"{replicons(z).Name}_UTRs.wig"), replicons(z).Name, genomes(z).Genes, replicons(z).Length)
                Call FileIO.WigWriter.WriteRNAs(Path.Combine(browserDir, $"{replicons(z).Name}_RNAs.wig"), replicons(z).Name, genomes(z).Genes, replicons(z).Length)
            Next
        End If

        Call Logging.Output($"{vbLf}Job Done!{vbLf}")
    End Sub

#End Region

#Region "De novo analysis"

    Private Sub runDeNovo(parameters As RockhopperParameters)
        Dim assembler As New Assembly.DeNovoAssembler(parameters.ConditionFiles, parameters.OutputDirectory, parameters.ExpressionFile) With {
            .K = parameters.K,
            .MinReadLength = parameters.MinReadLength,
            .CapacityPower = parameters.CapacityPower,
            .MinReadsMapping = parameters.MinReadsMapping,
            .MinTranscriptLength = parameters.MinTranscriptLength,
            .MinSeedExpression = parameters.MinSeedExpression,
            .MinExpression = parameters.MinExpression,
            .NumThreads = parameters.NumThreads,
            .StopAfterOneHit = parameters.StopAfterOneHit,
            .OutputSAM = parameters.OutputSAM,
            .Unstranded = parameters.Unstranded,
            .Verbose = parameters.Verbose,
            .Time = parameters.Time,
            .Labels = parameters.Labels,
            .MaxPairedEndLength = parameters.MaxPairedEndLength,
            .SingleEndOrientationReverseComplement = parameters.SingleEndOrientationReverseComplement,
            .PairedEndOrientation = parameters.PairedEndOrientation,
            .NumConditions = parameters.ConditionFiles.Count
        }
        Call assembler.Run()
    End Sub

#End Region

#Region "Helpers"

    ''' <summary>
    ''' 依据命令行条件文件构造条件对象（初始不含重复）。
    ''' </summary>
    Private Function buildConditions(parameters As RockhopperParameters) As List(Of Condition)
        Dim conditions As New List(Of Condition)()
        For i As Integer = 0 To parameters.ConditionFiles.Count - 1
            Dim name As String = If(parameters.Labels IsNot Nothing AndAlso parameters.Labels.Length = parameters.ConditionFiles.Count,
                                    parameters.Labels(i), (i + 1).ToString)
            conditions.Add(New Condition With {.Name = name})
        Next
        Return conditions
    End Function

    Private Function toSamRecord(hit As Alignment.AlignmentHit, referenceName As String, all As Alignment.AlignmentHit()) As FileIO.SamRecord
        Return New FileIO.SamRecord With {
            .QNAME = hit.ReadID,
            .FLAG = If(hit.IsReverse, 16, 0),
            .RNAME = referenceName,
            .POS = hit.Position,
            .MAPQ = 255,
            .CIGAR = If(String.IsNullOrEmpty(hit.Cigar), "*", hit.Cigar),
            .SEQ = hit.Sequence,
            .QUAL = If(hit.Quality, "*")
        }
    End Function

#End Region

End Module
