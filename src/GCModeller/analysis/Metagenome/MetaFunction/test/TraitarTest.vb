' ============================================================================
' Program.vb - Traitar VB.NET 主程序入口
'
' 论文复现：From Genomes to Phenotypes: Traitar, the Microbial Trait Analyzer
'
' 功能：
'   1. 读取基因组GFF文件和蛋白FASTA文件
'   2. 调用HMMER进行Pfam注释（或解析预计算的HMMER输出）
'   3. 构建系统发育谱（0/1特征向量）
'   4. 加载模型文件（pt2acc.txt, {id}_bias.txt, {id}_feats.txt等）
'   5. 使用投票委员会进行表型预测
'   6. 输出预测结果和关键特征
'
' 用法：
'   TraitarVB.exe --gff <gff_file> --fasta <protein_fasta> --models <models_dir> [options]
'
'   或直接使用预计算的HMMER输出：
'   TraitarVB.exe --domtblout <hmmsearch_output> --models <models_dir> [options]
' ============================================================================

Imports System.IO
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB

Namespace TraitarVB

    Public Class Program

        ' 默认参数
        Public Const DEFAULT_BITSCORE_THRESHOLD As Double = 25.0
        Public Const DEFAULT_EVALUE_THRESHOLD As Double = 0.01

        Public Shared Sub Main2(ByVal args As String())

            Console.WriteLine(New String("="c, 70))
            Console.WriteLine("  Traitar VB.NET - 微生物表型预测器")
            Console.WriteLine("  基于论文: From Genomes to Phenotypes (mSystems, 2016)")
            Console.WriteLine(New String("="c, 70))
            Console.WriteLine()

            ' 解析命令行参数
            Dim config As New RunConfig()
            If Not ParseArguments(args, config) Then
                PrintUsage()
                Return
            End If

            ' 执行预测流程
            Try
                RunPrediction(config)
            Catch ex As Exception
                Console.WriteLine("错误: " & ex.Message)
                Console.WriteLine(ex.StackTrace)
            End Try

            Console.WriteLine()
            Console.WriteLine("按任意键退出...")
            If Not Console.IsInputRedirected Then
                Console.ReadKey()
            End If
        End Sub

        ''' <summary>
        ''' 运行配置
        ''' </summary>
        Public Class RunConfig
            Public Property GffPath As String = ""
            Public Property FastaPath As String = ""
            Public Property DomtbloutPath As String = ""
            Public Property ModelsDir As String = ""
            Public Property OutputDir As String = "output"
            Public Property BitScoreThreshold As Double = DEFAULT_BITSCORE_THRESHOLD
            Public Property EValueThreshold As Double = DEFAULT_EVALUE_THRESHOLD
            Public Property HmmsearchPath As String = "hmmsearch"
            Public Property PfamDbPath As String = ""
            Public Property RunHmmsearch As Boolean = False
            Public Property Verbose As Boolean = True
        End Class

        ''' <summary>
        ''' 解析命令行参数
        ''' </summary>
        Private Shared Function ParseArguments(ByVal args As String(), ByVal config As RunConfig) As Boolean
            If args.Length = 0 Then Return False

            Dim i As Integer = 0
            Do While i < args.Length
                Select Case args(i).ToLower()
                    Case "--gff"
                        i += 1
                        If i < args.Length Then config.GffPath = args(i)
                    Case "--fasta"
                        i += 1
                        If i < args.Length Then config.FastaPath = args(i)
                    Case "--domtblout"
                        i += 1
                        If i < args.Length Then config.DomtbloutPath = args(i)
                    Case "--models"
                        i += 1
                        If i < args.Length Then config.ModelsDir = args(i)
                    Case "--output", "-o"
                        i += 1
                        If i < args.Length Then config.OutputDir = args(i)
                    Case "--bitscore"
                        i += 1
                        If i < args.Length Then Double.TryParse(args(i), config.BitScoreThreshold)
                    Case "--evalue"
                        i += 1
                        If i < args.Length Then Double.TryParse(args(i), config.EValueThreshold)
                    Case "--hmmsearch"
                        i += 1
                        If i < args.Length Then config.HmmsearchPath = args(i)
                    Case "--pfam-db"
                        i += 1
                        If i < args.Length Then config.PfamDbPath = args(i)
                    Case "--run-hmmsearch"
                        config.RunHmmsearch = True
                    Case "--verbose", "-v"
                        config.Verbose = True
                    Case "--help", "-h"
                        Return False
                    Case Else
                        Console.WriteLine("未知参数: " & args(i))
                End Select
                i += 1
            Loop

            ' 验证必要参数
            If config.ModelsDir = "" Then
                Console.WriteLine("错误: 必须指定 --models 参数")
                Return False
            End If

            If config.DomtbloutPath = "" AndAlso config.FastaPath = "" AndAlso config.GffPath = "" Then
                Console.WriteLine("错误: 必须指定 --domtblout 或 --fasta 或 --gff 参数")
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' 打印用法
        ''' </summary>
        Private Shared Sub PrintUsage()
            Console.WriteLine("用法:")
            Console.WriteLine("  TraitarVB.exe --models <models_dir> [输入选项] [其他选项]")
            Console.WriteLine()
            Console.WriteLine("输入选项（三选一）:")
            Console.WriteLine("  --domtblout <file>   预计算的HMMER domtblout输出文件")
            Console.WriteLine("  --fasta <file>       蛋白质FASTA文件（需要配合--run-hmmsearch）")
            Console.WriteLine("  --gff <file>         GFF3注释文件")
            Console.WriteLine()
            Console.WriteLine("必需参数:")
            Console.WriteLine("  --models <dir>       模型文件目录（包含pt2acc.txt等）")
            Console.WriteLine()
            Console.WriteLine("可选参数:")
            Console.WriteLine("  --output <dir>       输出目录（默认: output）")
            Console.WriteLine("  --bitscore <val>     比特分数阈值（默认: 25.0）")
            Console.WriteLine("  --evalue <val>       E值阈值（默认: 0.01）")
            Console.WriteLine("  --run-hmmsearch      对FASTA文件运行hmmsearch")
            Console.WriteLine("  --hmmsearch <path>   hmmsearch可执行文件路径")
            Console.WriteLine("  --pfam-db <path>     Pfam数据库HMM文件路径")
            Console.WriteLine("  --verbose, -v        详细输出")
            Console.WriteLine("  --help, -h           显示帮助")
            Console.WriteLine()
            Console.WriteLine("示例:")
            Console.WriteLine("  TraitarVB.exe --domtblout sample.domtblout --models ./models --output ./output")
            Console.WriteLine("  TraitarVB.exe --fasta proteins.faa --models ./models --run-hmmsearch --pfam-db Pfam-A.hmm")
        End Sub

        ''' <summary>
        ''' 运行预测流程
        ''' </summary>
        Private Shared Sub RunPrediction(ByVal config As RunConfig)

            ' ================================================================
            ' 步骤1: 基因组注释与特征化（模块1）
            ' ================================================================
            Console.WriteLine("--- 步骤1: 基因组注释与特征化 ---")
            Dim annotator As New Modules.GenomeAnnotation(
                "prodigal", config.HmmsearchPath, config.PfamDbPath)

            Dim sample As Models.GenomeSample

            If config.DomtbloutPath <> "" Then
                ' 直接解析预计算的HMMER输出
                Console.WriteLine("解析HMMER domtblout文件: " & config.DomtbloutPath)
                sample = annotator.AnnotateFromDomtblout(
                    config.DomtbloutPath,
                    config.BitScoreThreshold,
                    config.EValueThreshold)
            ElseIf config.FastaPath <> "" Then
                ' 从FASTA文件注释
                Console.WriteLine("从蛋白质FASTA文件注释: " & config.FastaPath)
                sample = annotator.AnnotateFromFasta(
                    config.FastaPath,
                    config.BitScoreThreshold,
                    config.EValueThreshold)
            ElseIf config.GffPath <> "" Then
                ' 从GFF文件注释
                Console.WriteLine("从GFF文件注释: " & config.GffPath)
                sample = annotator.AnnotateFromGFF(
                    config.GffPath,
                    config.BitScoreThreshold,
                    config.EValueThreshold)
            Else
                Throw New Exception("未指定输入文件")
            End If

            Console.WriteLine("样本ID: " & sample.SampleId)
            Console.WriteLine("Pfam家族数: " & sample.PfamCount)
            Console.WriteLine()

            ' ================================================================
            ' 步骤2: 加载模型文件
            ' ================================================================
            Console.WriteLine("--- 步骤2: 加载模型文件 ---")
            Dim modelLoader As New ModelLoader(config.ModelsDir)
            modelLoader.LoadAll()

            Console.WriteLine("表型数: " & modelLoader.PhenotypeCount)
            Console.WriteLine()

            ' ================================================================
            ' 步骤3: 集成投票预测（模块6）
            ' ================================================================
            Console.WriteLine("--- 步骤3: 集成投票预测 ---")
            Dim voting As New Modules.EnsembleVoting()

            ' 构建表型ID -> 模型列表的映射
            Dim phenotypeModels As New Dictionary(Of String, List(Of Modules.SVMClassifier.SVMModel))
            For Each kvp As KeyValuePair(Of String, Models.PhenotypeModel) In modelLoader.Phenotypes
                Dim phenoId As String = kvp.Key
                Dim phenoModel As Models.PhenotypeModel = kvp.Value

                ' 将PhenotypeModel转换为SVMModel列表
                Dim svmModels As New List(Of Modules.SVMClassifier.SVMModel)
                For Each subModel As Models.SVMSubModel In phenoModel.SubModels
                    Dim svmModel As New Modules.SVMClassifier.SVMModel()
                    svmModel.C = subModel.C
                    svmModel.Bias = subModel.Bias
                    svmModel.FeatureIds = New List(Of String)(subModel.Weights.Keys)
                    svmModel.Weights = New Double(subModel.Weights.Count - 1) {}
                    Dim idx As Integer = 0
                    For Each wKvp As KeyValuePair(Of String, Double) In subModel.Weights
                        svmModel.FeatureIds(idx) = wKvp.Key
                        svmModel.Weights(idx) = wKvp.Value
                        idx += 1
                    Next
                    svmModels.Add(svmModel)
                Next

                phenotypeModels(phenoId) = svmModels
            Next

            ' 执行预测
            Dim predictions As Dictionary(Of String, Modules.EnsembleVoting.VotingResult) =
                voting.PredictAllPhenotypes(phenotypeModels, sample.PhyleticProfile)

            Console.WriteLine()

            ' ================================================================
            ' 步骤4: 特征选择与关联解释（模块7）
            ' ================================================================
            Console.WriteLine("--- 步骤4: 关键特征分析 ---")
            Dim featSelector As New Modules.FeatureSelection()

            ' 为每个阳性表型提取关键特征
            Dim allKeyFeatures As New Dictionary(Of String, List(Of Modules.FeatureSelection.KeyFeature))

            For Each kvp As KeyValuePair(Of String, Modules.EnsembleVoting.VotingResult) In predictions
                If kvp.Value.IsPositive Then
                    Dim phenoId As String = kvp.Key
                    Dim keyFile As String = Path.Combine(config.ModelsDir, phenoId & "_non-zero+weights.txt")

                    If File.Exists(keyFile) Then
                        Dim keyFeats As List(Of Modules.FeatureSelection.KeyFeature) =
                            featSelector.LoadKeyFeaturesFromFile(keyFile)
                        allKeyFeatures(phenoId) = keyFeats
                    End If
                End If
            Next

            Console.WriteLine()

            ' ================================================================
            ' 步骤5: 输出结果
            ' ================================================================
            Console.WriteLine("--- 步骤5: 输出结果 ---")
            If Not Directory.Exists(config.OutputDir) Then
                Directory.CreateDirectory(config.OutputDir)
            End If

            OutputResults(config.OutputDir, sample, modelLoader, predictions, allKeyFeatures, featSelector)

            Console.WriteLine()
            Console.WriteLine("预测完成！结果已保存到: " & config.OutputDir)
        End Sub

        ''' <summary>
        ''' 输出结果
        ''' </summary>
        Private Shared Sub OutputResults(
            ByVal outputDir As String,
            ByVal sample As Models.GenomeSample,
            ByVal modelLoader As ModelLoader,
            ByVal predictions As Dictionary(Of String, Modules.EnsembleVoting.VotingResult),
            ByVal allKeyFeatures As Dictionary(Of String, List(Of Modules.FeatureSelection.KeyFeature)),
            ByVal featSelector As Modules.FeatureSelection)

            ' 1. 预测结果摘要
            Dim summaryPath As String = Path.Combine(outputDir, "prediction_summary.txt")
            Using writer As New StreamWriter(summaryPath)
                writer.WriteLine(New String("="c, 70))
                writer.WriteLine("  Traitar VB.NET 表型预测结果")
                writer.WriteLine(New String("="c, 70))
                writer.WriteLine()
                writer.WriteLine("样本ID: " & sample.SampleId)
                writer.WriteLine("Pfam家族数: " & sample.PfamCount)
                writer.WriteLine("检测到的Pfam家族:")
                For Each pid As String In sample.GetPresentPfamIds()
                    writer.WriteLine("  " & pid)
                Next
                writer.WriteLine()
                writer.WriteLine("--- 表型预测结果 ---")
                writer.WriteLine(String.Format("{0,-10} {1,-40} {2,-15} {3,-10} {4,-10}",
                                                "表型ID", "表型名称", "类别", "预测", "置信度"))
                writer.WriteLine(New String("-"c, 90))

                Dim positiveCount As Integer = 0
                For Each kvp As KeyValuePair(Of String, Models.PhenotypeModel) In modelLoader.Phenotypes
                    Dim phenoId As String = kvp.Key
                    Dim phenoModel As Models.PhenotypeModel = kvp.Value

                    Dim result As Modules.EnsembleVoting.VotingResult = Nothing
                    If predictions.ContainsKey(phenoId) Then
                        result = predictions(phenoId)
                    End If

                    Dim predStr As String = "N/A"
                    Dim confStr As String = "N/A"
                    If result IsNot Nothing Then
                        predStr = If(result.IsPositive, "存在(+)", "不存在(-)")
                        confStr = result.Confidence.ToString("F2")
                        If result.IsPositive Then positiveCount += 1
                    End If

                    writer.WriteLine(String.Format("{0,-10} {1,-40} {2,-15} {3,-10} {4,-10}",
                                                    phenoId,
                                                    If(phenoModel.PhenotypeName.Length > 40,
                                                       phenoModel.PhenotypeName.Substring(0, 40), phenoModel.PhenotypeName),
                                                    phenoModel.Category,
                                                    predStr, confStr))
                Next

                writer.WriteLine()
                writer.WriteLine(String.Format("阳性表型数: {0}/{1}", positiveCount, modelLoader.PhenotypeCount))
            End Using
            Console.WriteLine("  预测摘要: " & summaryPath)

            ' 2. 阳性表型详细报告
            Dim detailPath As String = Path.Combine(outputDir, "positive_phenotypes_detail.txt")
            Using writer As New StreamWriter(detailPath)
                writer.WriteLine(New String("="c, 70))
                writer.WriteLine("  阳性表型详细报告")
                writer.WriteLine(New String("="c, 70))
                writer.WriteLine()

                For Each kvp As KeyValuePair(Of String, Modules.EnsembleVoting.VotingResult) In predictions
                    If Not kvp.Value.IsPositive Then Continue For

                    Dim phenoId As String = kvp.Key
                    Dim phenoModel As Models.PhenotypeModel = modelLoader.Phenotypes(phenoId)

                    writer.WriteLine(New String("-"c, 35))
                    writer.WriteLine("表型ID: " & phenoId)
                    writer.WriteLine("表型名称: " & phenoModel.PhenotypeName)
                    writer.WriteLine("表型类别: " & phenoModel.Category)
                    writer.WriteLine("预测结果: 存在(POSITIVE)")
                    writer.WriteLine("投票统计: 正票=" & kvp.Value.PositiveVotes &
                                     ", 负票=" & kvp.Value.NegativeVotes &
                                     ", 总票数=" & kvp.Value.TotalVotes)
                    writer.WriteLine("置信度: " & kvp.Value.Confidence.ToString("F4"))
                    writer.WriteLine()

                    ' 关键特征
                    If allKeyFeatures.ContainsKey(phenoId) Then
                        writer.WriteLine("关键蛋白质家族特征:")
                        writer.WriteLine(featSelector.GenerateReport(allKeyFeatures(phenoId), 10))
                    End If

                    writer.WriteLine()
                Next
            End Using
            Console.WriteLine("  阳性详情: " & detailPath)

            ' 3. 特征向量
            Dim featurePath As String = Path.Combine(outputDir, "phyletic_profile.txt")
            Using writer As New StreamWriter(featurePath)
                writer.WriteLine("样本ID: " & sample.SampleId)
                writer.WriteLine("Pfam家族数: " & sample.PfamCount)
                writer.WriteLine()
                writer.WriteLine("存在的Pfam家族:")
                For Each pid As String In sample.GetPresentPfamIds()
                    Dim desc As String = ""
                    If modelLoader.PfamDescriptions.ContainsKey(pid) Then
                        desc = modelLoader.PfamDescriptions(pid)
                    End If
                    writer.WriteLine(pid & ControlChars.Tab & desc)
                Next
            End Using
            Console.WriteLine("  特征向量: " & featurePath)

        End Sub

    End Class

End Namespace
