' ============================================================================
' Traitar VB.NET Demo - 主程序
' File: Program.vb
'
' 功能: 演示完整的 Traitar 微生物表型预测流程:
'   1. 读取基因组 GFF 文件
'   2. 读取蛋白质 FASTA 文件
'   3. 调用 HMMER 进行 Pfam 注释（或读取已有的 tblout 结果）
'   4. 构建二值化特征矩阵（系统发育谱）
'   5. 加载 Traitar 模型文件
'   6. 使用投票委员会进行表型预测
'   7. 提取关键特征（Pearson 相关性排序）
'   8. 输出预测结果
' ============================================================================

Imports System.IO
Imports System.Collections.Generic
Imports Traitar.GenomeAnnotation
Imports Traitar.Models
Imports Traitar.Prediction
Imports Traitar.Evaluation
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Traitar.Prediction
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Traitar.Models
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Traitar.GenomeAnnotation

Module TraitarTest

    ''' <summary>运行配置</summary>
    Public Class RunConfig
        Public Property GffPath As String
        Public Property FastaPath As String
        Public Property HmmerPath As String
        Public Property ModelDir As String
        Public Property OutputDir As String = "output"
        Public Property SampleId As String = "Sample_001"
    End Class

    Sub Main(args As String())
        Console.WriteLine()
        Console.WriteLine("=" & New String("="c, 70))
        Console.WriteLine("  Traitar - Microbial Trait Analyzer (VB.NET Implementation)")
        Console.WriteLine("  基于基因组的微生物表型预测系统")
        Console.WriteLine("=" & New String("="c, 70))
        Console.WriteLine()

        ' 解析命令行参数
        Dim config = ParseArgs(args)
        If config Is Nothing Then
            PrintUsage()
            Return
        End If

        Try
            RunPipeline(config)
        Catch ex As Exception
            Console.WriteLine($"[ERROR] {ex.Message}")
            Console.WriteLine(ex.StackTrace)
        End Try

        Console.WriteLine()
        Console.WriteLine("Done.")
    End Sub

    ''' <summary>
    ''' 运行完整的预测流水线
    ''' </summary>
    Private Sub RunPipeline(config As RunConfig)
        ' 创建输出目录
        If Not Directory.Exists(config.OutputDir) Then
            Directory.CreateDirectory(config.OutputDir)
        End If

        ' ========== Step 1: 读取基因组 GFF 文件 ==========
        Console.WriteLine("[Step 1] 读取基因组 GFF 文件...")
        Console.WriteLine($"  文件: {config.GffPath}")
        Dim gffRecords = GffParser.ParseCds(config.GffPath)
        Console.WriteLine($"  解析到 {gffRecords.Count} 条 CDS 记录")
        For Each rec In gffRecords.Take(5)
            Dim name = If(rec.Attributes.ContainsKey("ID"), rec.Attributes("ID"), "?")
            Console.WriteLine($"    - {name}: {rec.SeqId}:{rec.Start}-{rec.End}({rec.Strand})")
        Next
        If gffRecords.Count > 5 Then Console.WriteLine($"    ... (共 {gffRecords.Count} 条)")
        Console.WriteLine()

        ' ========== Step 2: 读取蛋白质 FASTA 文件 ==========
        Console.WriteLine("[Step 2] 读取蛋白质 FASTA 文件...")
        Console.WriteLine($"  文件: {config.FastaPath}")
        Dim proteins = FastaParser.Parse(config.FastaPath)
        Console.WriteLine($"  解析到 {proteins.Count} 条蛋白质序列")
        For Each p In proteins.Take(5)
            Console.WriteLine($"    - {p.Id}: length={p.Sequence.Length}")
        Next
        If proteins.Count > 5 Then Console.WriteLine($"    ... (共 {proteins.Count} 条)")
        Console.WriteLine()

        ' ========== Step 3: Pfam 注释 ==========
        Console.WriteLine("[Step 3] Pfam 家族注释...")
        Dim pfamHits As New List(Of PfamHit)
        Dim PfamAnnotator As New PfamAnnotator
        If Not String.IsNullOrEmpty(config.HmmerPath) AndAlso File.Exists(config.HmmerPath) Then
            Console.WriteLine($"  解析 HMMER tblout 结果: {config.HmmerPath}")
            pfamHits = PfamAnnotator.ParseTblout(config.HmmerPath)
        Else
            Console.WriteLine("  未提供 HMMER 结果文件，尝试调用 HMMER...")
            pfamHits = PfamAnnotator.RunHmmer(config.FastaPath, config.OutputDir)
        End If
        Console.WriteLine($"  获得 {pfamHits.Count} 条 Pfam 命中（通过阈值过滤）")

        ' 按蛋白质分组显示
        Dim hitsByProtein = pfamHits.GroupBy(Function(h) h.ProteinId)
        For Each g In hitsByProtein.Take(5)
            Console.WriteLine($"    {g.Key}:")
            For Each h In g.Take(3)
                Console.WriteLine($"      -> {h.PfamAcc} ({h.PfamName}) score={h.BitScore:F1} E={h.EValue:G2}")
            Next
        Next
        Console.WriteLine()

        ' ========== Step 4: 构建特征矩阵 ==========
        Console.WriteLine("[Step 4] 构建二值化特征矩阵（系统发育谱）...")
        Dim profile = FeatureMatrixBuilder.BuildFromHits(pfamHits)
        Console.WriteLine($"  样本ID: {config.SampleId}")
        Console.WriteLine($"  存在的 Pfam 家族数: {profile.PresentPfams.Count}")
        Console.WriteLine($"  Pfam 家族列表: {String.Join(", ", profile.PresentPfams.OrderBy(Function(x) x))}")
        Console.WriteLine()

        ' 保存特征矩阵
        Dim featureFile = Path.Combine(config.OutputDir, $"{config.SampleId}_features.tsv")
        File.WriteAllText(featureFile,
            $"SampleID{vbTab}{String.Join(vbTab, profile.PresentPfams.OrderBy(Function(x) x))}{vbCrLf}" &
            $"{config.SampleId}{vbTab}" & New String("1"c, profile.PresentPfams.Count).Replace("1", "1" & vbTab).TrimEnd(vbTab) & vbCrLf)
        Console.WriteLine($"  特征矩阵已保存: {featureFile}")
        Console.WriteLine()

        ' ========== Step 5: 加载 Traitar 模型 ==========
        Console.WriteLine("[Step 5] 加载 Traitar 模型文件...")
        Console.WriteLine($"  模型目录: {config.ModelDir}")
        Dim models = TraitarModelLoader.LoadAllModels(config.ModelDir)
        Console.WriteLine($"  加载了 {models.Count} 个表型模型:")
        For Each kv In models
            Console.WriteLine($"    - [{kv.Key}] {kv.Value.Info.Accession} ({kv.Value.Info.Category})")
            Console.WriteLine($"      模型数: {kv.Value.Models.Count}, 非零特征数: {kv.Value.NonZeroFeatures.Count}")
        Next
        Console.WriteLine()

        ' ========== Step 6: 表型预测 ==========
        Console.WriteLine("[Step 6] 表型预测（投票委员会机制）...")
        Dim predictor As New PhenotypePredictor()
        predictor.CommitteeSize = 5
        predictor.MajorityThreshold = 3

        Dim results = predictor.PredictAll(models, profile)
        Console.WriteLine($"  预测完成，共 {results.Count} 个表型:")
        For Each r As PhenotypePredictionResult In results
            Dim status = If(r.FinalPrediction = 1, "PRESENT", "absent ")
            Console.WriteLine($"    [{r.PhenotypeId}] {r.PhenotypeName,-30} -> {status} " &
                              $"(votes: {r.PositiveVotes}/{r.TotalVotes}, avg_score={r.AverageScore:F4})")
        Next
        Console.WriteLine()

        ' ========== Step 7: 特征解释 ==========
        Console.WriteLine("[Step 7] 关键特征提取（Pearson 相关性排序）...")
        Dim explainer As New FeatureExplainer()
        For Each r In results
            If r.FinalPrediction = 1 Then
                Console.WriteLine()
                Console.WriteLine($"  --- 表型 [{r.PhenotypeId}] {r.PhenotypeName} ---")
                Dim keyFeatures = explainer.ExplainKeyFeatures(models(r.PhenotypeId), profile)
                For Each f As KeyFeature In keyFeatures
                    Dim present2 As String = If(f.IsPresent, "[*]", "[ ]")
                    Console.WriteLine($"    {present2} {f.PfamAcc,-12} class={f.Class} PCC={f.PearsonCorrelation:F3} " &
                                      $"sel={f.SelectedCount}/{f.TotalModels} avgW={f.AverageWeight:F4}")
                    Console.WriteLine($"        {f.Description}")
                Next
            End If
        Next
        Console.WriteLine()

        ' ========== Step 8: 输出结果 ==========
        Console.WriteLine("[Step 8] 保存预测结果...")
        SaveResults(results, explainer, models, profile, config)
        Console.WriteLine($"  结果已保存到: {config.OutputDir}")
        Console.WriteLine()

        ' ========== 汇总 ==========
        Console.WriteLine("=" & New String("="c, 70))
        Console.WriteLine("  预测结果汇总")
        Console.WriteLine("=" & New String("="c, 70))
        Dim present = results.FindAll(Function(r) r.FinalPrediction = 1)
        Console.WriteLine($"  样本: {config.SampleId}")
        Console.WriteLine($"  检测到的 Pfam 家族数: {profile.PresentPfams.Count}")
        Console.WriteLine($"  预测表型总数: {results.Count}")
        Console.WriteLine($"  预测为存在的表型数: {present.Count}")
        For Each r As PhenotypePredictionResult In present
            Console.WriteLine($"    -> [{r.PhenotypeId}] {r.PhenotypeName} ({r.Category})")
        Next
        Console.WriteLine()
    End Sub

    ''' <summary>保存预测结果到文件</summary>
    Private Sub SaveResults(results As List(Of PhenotypePredictionResult),
                            explainer As FeatureExplainer,
                            models As Dictionary(Of String, PhenotypeModel),
                            profile As PhyleticProfile,
                            config As RunConfig)
        ' 1. 预测结果汇总
        Dim summaryFile = Path.Combine(config.OutputDir, "phenotype_predictions.txt")
        Using writer As New StreamWriter(summaryFile)
            writer.WriteLine($"# Traitar Phenotype Prediction Results")
            writer.WriteLine($"# Sample: {config.SampleId}")
            writer.WriteLine($"# Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
            writer.WriteLine($"# Pfam families detected: {profile.PresentPfams.Count}")
            writer.WriteLine()
            writer.WriteLine($"{"PhenotypeID",-12}{vbTab}{"Name",-35}{vbTab}{"Category",-15}{vbTab}{"Prediction",-10}{vbTab}{"Votes",-8}{vbTab}{"AvgScore",-12}")
            writer.WriteLine(New String("-"c, 100))
            For Each r In results
                Dim pred = If(r.FinalPrediction = 1, "PRESENT", "absent")
                writer.WriteLine($"{r.PhenotypeId,-12}{vbTab}{r.PhenotypeName,-35}{vbTab}{r.Category,-15}{vbTab}{pred,-10}{vbTab}{r.PositiveVotes}/{r.TotalVotes}{vbTab}{r.AverageScore:F6}")
            Next
        End Using

        ' 2. 关键特征详情
        Dim featuresFile = Path.Combine(config.OutputDir, "key_features.txt")
        Using writer As New StreamWriter(featuresFile)
            writer.WriteLine($"# Traitar Key Features Report")
            writer.WriteLine($"# Sample: {config.SampleId}")
            writer.WriteLine()
            For Each r In results
                If r.FinalPrediction = 1 Then
                    writer.WriteLine(explainer.GenerateReport(r.PhenotypeId, r.PhenotypeName,
                                                              explainer.ExplainKeyFeatures(models(r.PhenotypeId), profile)))
                    writer.WriteLine()
                End If
            Next
        End Using

        ' 3. 各模型详细得分
        Dim detailFile = Path.Combine(config.OutputDir, "model_details.txt")
        Using writer As New StreamWriter(detailFile)
            writer.WriteLine($"# Traitar Model Details")
            writer.WriteLine($"# Sample: {config.SampleId}")
            writer.WriteLine()
            For Each r In results
                writer.WriteLine($"Phenotype [{r.PhenotypeId}] {r.PhenotypeName}")
                writer.WriteLine($"  Final prediction: {If(r.FinalPrediction = 1, "PRESENT", "absent")} " &
                                 $"(votes: {r.PositiveVotes}/{r.TotalVotes})")
                writer.WriteLine($"  Model predictions:")
                For Each mp In r.ModelPredictions
                    writer.WriteLine($"    {mp.ModelId,-10} C={mp.C,-8} score={mp.Score,-12:F6} bias={mp.Bias,-12:F6} pred={mp.Prediction}")
                Next
                writer.WriteLine()
            Next
        End Using
    End Sub

    ''' <summary>解析命令行参数</summary>
    Private Function ParseArgs(args As String()) As RunConfig
        Dim config As New RunConfig()
        Dim i = 0
        While i < args.Length
            Select Case args(i).ToLower()
                Case "--gff" : config.GffPath = args(i + 1) : i += 2
                Case "--fasta" : config.FastaPath = args(i + 1) : i += 2
                Case "--hmmer" : config.HmmerPath = args(i + 1) : i += 2
                Case "--model-dir" : config.ModelDir = args(i + 1) : i += 2
                Case "--output" : config.OutputDir = args(i + 1) : i += 2
                Case "--sample-id" : config.SampleId = args(i + 1) : i += 2
                Case "--help", "-h" : Return Nothing
                Case Else : i += 1
            End Select
        End While

        If String.IsNullOrEmpty(config.GffPath) OrElse
           String.IsNullOrEmpty(config.FastaPath) OrElse
           String.IsNullOrEmpty(config.ModelDir) Then
            Return Nothing
        End If

        Return config
    End Function

    ''' <summary>打印用法</summary>
    Private Sub PrintUsage()
        Console.WriteLine("用法: TraitarDemo --gff <gff_file> --fasta <protein_fasta>")
        Console.WriteLine("                   --model-dir <model_directory>")
        Console.WriteLine("                   [--hmmer <tblout_file>]")
        Console.WriteLine("                   [--output <output_dir>]")
        Console.WriteLine("                   [--sample-id <sample_name>]")
        Console.WriteLine()
        Console.WriteLine("参数:")
        Console.WriteLine("  --gff         基因组 GFF3 注释文件路径")
        Console.WriteLine("  --fasta       蛋白质 FASTA 文件路径")
        Console.WriteLine("  --model-dir   Traitar 模型文件目录（包含 pt2acc.txt 等）")
        Console.WriteLine("  --hmmer       HMMER tblout 结果文件路径（可选，不提供则尝试调用 HMMER）")
        Console.WriteLine("  --output      输出目录（默认: output）")
        Console.WriteLine("  --sample-id   样本ID（默认: Sample_001）")
        Console.WriteLine()
        Console.WriteLine("示例:")
        Console.WriteLine("  TraitarDemo --gff data/genome.gff --fasta data/proteins.fasta")
        Console.WriteLine("              --model-dir models/ --hmmer data/pfam.tblout")
        Console.WriteLine("              --output output/ --sample-id B_subtilis")
    End Sub
End Module
