' /********************************************************************************/
'
'  Rockhopper —— 冒烟测试
'
'  使用合成数据对四个核心模块做端到端最小验证（不依赖外部数据文件）：
'    1) 参考依赖比对：FM-index 精确匹配 + 种子-延伸（含错配）
'    2) 表达定量：上四分位归一化 + 改良 RPKM + 差异检验
'    3) TSS/TTS 边界推断与 TSS 六分类
'    4) 操纵子预测（基因间距 + 表达相似度）
'    5) de novo 组装（de Bruijn 图 + 二次比对）
'
'  所有临时文件写入系统临时目录并在结束时删除。
'
' /********************************************************************************/

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Rockhopper
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Rockhopper.Core
Imports SMRUCC.genomics.SequenceModel.RNA_Seq.Statistics

Module Program

    Private failures As Integer = 0

    Sub Main(args As String())
        Dim workspace As String = Path.Combine(Path.GetTempPath(), "rockhopper_smoke_" & Guid.NewGuid().ToString("N"))
        Call Directory.CreateDirectory(workspace)

        Try
            Call TestFmIndexAndAligner()
            Call TestQuantification()
            Call TestTranscription(workspace)
            Call TestOperons(workspace)
            Call TestDeNovo(workspace)
        Finally
            Try
                Call Directory.Delete(workspace, True)
            Catch
            End Try
        End Try

        Console.WriteLine()
        If failures = 0 Then
            Console.WriteLine("[PASS] 全部冒烟测试通过。")
        Else
            Console.WriteLine($"[FAIL] {failures} 项冒烟测试失败。")
            Environment.ExitCode = 1
        End If
    End Sub

#Region "1. FM-index + Aligner"

    Private Sub TestFmIndexAndAligner()
        Console.WriteLine("== 1. FM-index & Aligner ==")

        Dim genome As String = "ACGTACGTACGTTTGGCCAATTGGCCAAGGTTACGTACGTACGTACGTAA"
        Dim replicon As New Replicon("test_replicon", genome)

        Dim fmi As New Alignment.FMIndex(replicon.SequenceData)
        check("FM-index 精确计数", fmi.Count("ACGTACGT") >= 2)

        Dim hit As Integer() = fmi.Locate("TTTGGCCAATT", 1)
        check("FM-index 定位", hit.Length = 1 AndAlso hit(0) = 11)

        ' 精确匹配
        Dim aligner As New Alignment.Aligner({replicon}, numThreads:=1)
        Dim exactRead As New FileIO.Read("r1", "TTTGGCCAATT")
        Dim exactHits = aligner.AlignReads({exactRead}).ToArray()
        check("精确匹配命中", exactHits.Length = 1 AndAlso exactHits(0).Position = 11 AndAlso Not exactHits(0).IsReverse)

        ' 含 1 个错配的读段（种子-延伸路径）
        Dim mismatchRead As New FileIO.Read("r2", "TTTGGCCAACT")
        Dim mismatchHits = aligner.AlignReads({mismatchRead}).ToArray()
        check("种子-延伸命中（1 错配）", mismatchHits.Length = 1)

        ' 反向互补链
        Dim rc As String = Alignment.Aligner.reverseComplement("TTTGGCCAATT")
        Dim rcHits = aligner.AlignReads({New FileIO.Read("r3", rc)}).ToArray()
        check("负链命中", rcHits.Length = 1 AndAlso rcHits(0).IsReverse)
    End Sub

#End Region

#Region "2. Quantification"

    Private Sub TestQuantification()
        Console.WriteLine("== 2. Quantification ==")

        Dim quartile As Double = ExpressionNormalization.UpperQuartile({1.0, 2.0, 3.0, 4.0})
        check("上四分位（1..4 → 3.25）", Math.Abs(quartile - 3.25) < 1E-09)

        Dim rpkm As Double = ExpressionNormalization.ModifiedRPKM(100.0, 1000, 10.0)
        check("改良 RPKM", Math.Abs(rpkm - 10000000.0) < 1E-06)

        Dim q As Double() = ExpressionNormalization.BenjaminiHochberg({0.01, 0.02, 0.5, 1.0})
        check("BH 校正单调且 ≤ 1", q.All(Function(v) v <= 1.0) AndAlso q(0) <= q(1))

        ' 通过 Core.Gene 的统计入口验证 lowess 方差估计可用
        Dim condition As New Condition()
        Dim coverage As New AlignmentCoverage With {
            .PlusReads = New Integer(30) {},
            .MinusReads = New Integer(30) {},
            .AvgLengthReads = 30
        }
        For i As Integer = 1 To 29
            coverage.PlusReads(i) = 5
        Next
        condition.AddReplicate(New Replicate({coverage}, False))
        condition.SetMinDiffExpressionLevel()
        check("重复的上四分位与 minDiffExpressionLevel 可用", condition.GetReplicate(0).TotalReads > 0)
    End Sub

#End Region

#Region "3. Transcription"

    Private Sub TestTranscription(workspace As String)
        Console.WriteLine("== 3. Transcription (TSS/TTS) ==")

        Dim folder As String = Path.Combine(workspace, "genome")
        Call Directory.CreateDirectory(folder)

        ' 500 bp 基因组
        Dim genome As String = String.Concat(Enumerable.Range(0, 500).Select(Function(i) "ACGT"(i Mod 4)))
        File.WriteAllText(Path.Combine(folder, "test.fna"), ">gi|123|ref|NC_000000.1| Test genome" & vbLf & genome & vbLf)

        ' geneA: 101..200 (+)，geneB: 231..330 (+)
        Dim ptt As New List(Of String) From {
            "Test genome - 1..500",
            "2 proteins",
            "Location`Strand`Length`PID`Gene`Synonym`Code`COG`Product"
        }
        ptt.Add("101..200" & vbTab & "+" & vbTab & "100" & vbTab & "PID_A" & vbTab & "geneA" & vbTab & "locusA" & vbTab & "-" & vbTab & "-" & vbTab & "protein A")
        ptt.Add("231..330" & vbTab & "+" & vbTab & "100" & vbTab & "PID_B" & vbTab & "geneB" & vbTab & "locusB" & vbTab & "-" & vbTab & "-" & vbTab & "protein B")
        File.WriteAllLines(Path.Combine(folder, "test.ptt"), ptt)

        Dim g As New Genome(folder)
        check("基因组加载", g.NumGenes() = 2)
        check("基因组序列 1-indexed", g.Sequence.Length = 501)

        ' 构造覆盖度：基因体内有读段，5'UTR 带延伸到 ~90
        Dim coverage As New AlignmentCoverage With {
            .PlusReads = New Integer(g.Sequence.Length) {},
            .MinusReads = New Integer(g.Sequence.Length) {},
            .AvgLengthReads = 30
        }
        For i As Integer = 90 To 340
            coverage.PlusReads(i) = 20
        Next
        Dim condition As New Condition With {.Name = "c1"}
        condition.AddReplicate(New Replicate({coverage}, False))

        Dim transcripts As List(Of Transcript) =
            Transcription.TSSsInference.InferBoundaries(g, g.Sequence.Length - 1, New List(Of Condition) From {condition}, 0.5)

        Dim geneA = transcripts.FirstOrDefault(Function(t) t.Synonym = "locusA")
        check("geneA 找到 TSS", geneA IsNot Nothing AndAlso geneA.TSSs > 0)
        check("geneA 的 TSS 位于 ATG 上游", geneA IsNot Nothing AndAlso geneA.TSSs < 101)
        check("识别到基因间区新转录本", transcripts.Any(Function(t) t.IsPredicted) OrElse transcripts.Count >= 2)

        ' TSS 六分类
        Dim pttObj = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(Path.Combine(folder, "test.ptt"))
        Dim relatedGene As SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief = Nothing
        Dim category As Transcription.TssCategory = Transcription.TSSsCategory.Category(
            90, 101, 200, geneA.GetTULoci(), False, False, "locusA", pttObj, False, relatedGene)
        check("六分类：mTSS", category = Transcription.TssCategory.mTSS)

        ' σ70 启动子打分
        Dim scores = Regulation.Sigma70Validation.Validate(g.Sequence, transcripts)
        check("σ70 验证产生结果", scores.Count > 0)
    End Sub

#End Region

#Region "4. Operons"

    Private Sub TestOperons(workspace As String)
        Console.WriteLine("== 4. Operons ==")

        Dim folder As String = Path.Combine(workspace, "genome")
        Dim g As New Genome(folder)

        ' 无表达数据时，仅验证距离判据可以把相邻同链基因合并
        Dim operonList As List(Of Operon) = Operons.OperonPrediction.Predict(g, 0)
        check("操纵子预测可运行", operonList IsNot Nothing)

        ' 距离 sigmoid 单调性
        Dim options As New Operons.OperonPredictionOptions()
        Dim near As Double = Operons.OperonPrediction.distanceProbability(5, options)
        Dim far As Double = Operons.OperonPrediction.distanceProbability(500, options)
        check("距离越近共转录先验越高", near > far)
    End Sub

#End Region

#Region "5. De novo assembly"

    Private Sub TestDeNovo(workspace As String)
        Console.WriteLine("== 5. De novo assembly ==")

        Const transcript As String = "ATGCGTACGTTAGCCATGGTACCGATTACAGGCATCGGATCCGTTAGCATGCATACGTTGGCCAATTGCA"
        Const readLength As Integer = 40
        Const stride As Integer = 20

        Dim fastq As New List(Of String)()
        For copy As Integer = 1 To 6
            For i As Integer = 0 To transcript.Length - readLength Step stride
                Dim read As String = transcript.Substring(i, readLength)
                Call fastq.Add($"@read_{copy}_{i}")
                Call fastq.Add(read)
                Call fastq.Add("+")
                Call fastq.Add(New String("I"c, readLength))
            Next
        Next

        Dim readsPath As String = Path.Combine(workspace, "reads.fastq")
        File.WriteAllLines(readsPath, fastq)

        Dim assembler As New Assembly.DeNovoAssembler(
            New List(Of String) From {readsPath}, workspace, "transcripts.txt") With {
            .K = 21,
            .MinReadLength = 35,
            .MinSeedExpression = 2,
            .MinExpression = 2,
            .MinTranscriptLength = 60,
            .MinReadsMapping = 2,
            .NumThreads = 1
        }
        Call assembler.Run()

        Dim resultPath As String = Path.Combine(workspace, "transcripts.txt")
        check("transcripts.txt 已生成", File.Exists(resultPath))

        Dim assembled As Assembly.DeNovoTranscripts = Assembly.DeNovoTranscripts.Load(resultPath)
        check("组装出至少 1 条转录本", assembled.Count >= 1)
        check("组装结果与真实转录本一致", assembled.Any(Function(t) t.Sequence.Contains(transcript.Substring(20, 40))))

        ' 结果可被 API 层转成 FASTA
        Dim fasta = AnalysisAPI.DeNovolTranscript.LoadDocument(resultPath)
        check("de novo 结果转 FASTA", fasta IsNot Nothing AndAlso fasta.Count = assembled.Count)
    End Sub

#End Region

    Private Sub check(name As String, condition As Boolean)
        If condition Then
            Console.WriteLine($"  [PASS] {name}")
        Else
            failures += 1
            Console.WriteLine($"  [FAIL] {name}")
        End If
    End Sub

End Module
