' ============================================================================
' Program.vb - Prodigal VB.NET 基因预测程序 主入口
' 用法：
'   基因预测模式：ProdigalVB -i input.fasta -o output_prefix [-m model.bin]
'   模型训练模式：ProdigalVB -t -i input.fasta -m model.bin
' ============================================================================

Imports System.IO
Imports Prodigal.ProdigalVB

Module Program

    ''' <summary>
    ''' 程序主入口
    ''' </summary>
    Sub Main(args As String())
        Console.WriteLine()
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗")
        Console.WriteLine("║  Prodigal VB.NET - 原核生物基因预测程序                      ║")
        Console.WriteLine("║  基于 Prodigal (PROkaryotic DYnamic Programming              ║")
        Console.WriteLine("║        Gene-finding ALgorithm) 算法                          ║")
        Console.WriteLine("║  版本 1.0                                                   ║")
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝")
        Console.WriteLine()

        ' 解析命令行参数
        Dim config = ParseArgs(args)
        If config Is Nothing Then
            PrintUsage()
            Return
        End If

        If config.TrainingMode Then
            ' 模型训练模式
            RunTraining(config)
        Else
            ' 基因预测模式
            RunPrediction(config)
        End If

        Console.WriteLine()
        Console.WriteLine("程序执行完毕。")
    End Sub

    ''' <summary>
    ''' 运行模型训练
    ''' </summary>
    Private Sub RunTraining(config As RunConfig)
        Console.WriteLine("模式：无监督模型训练")
        Console.WriteLine($"输入文件: {config.InputFile}")
        Console.WriteLine($"模型输出: {config.ModelFile}")
        Console.WriteLine()

        ' 读取FASTA文件
        Console.WriteLine("读取输入序列...")
        Dim sequences = FastaReader.Read(config.InputFile)
        Console.WriteLine($"  读取到 {sequences.Count} 条序列，总长度 {sequences.Sum(Function(s) s.Length):N0} bp")

        If sequences.Count = 0 Then
            Console.WriteLine("错误：输入文件中没有有效序列")
            Return
        End If

        ' 执行无监督训练
        Dim model = TrainingEngine.Train(sequences)

        ' 保存模型
        Console.WriteLine($"保存模型到: {config.ModelFile}")
        ModelSerializer.Save(model, config.ModelFile)
        Console.WriteLine("模型保存成功！")

        ' 输出模型摘要
        Console.WriteLine()
        Console.WriteLine("模型摘要:")
        Console.WriteLine($"  GC含量: {model.GcContent:P1}")
        Console.WriteLine($"  平均基因长度: {model.AvgGeneLength:F0} bp")
        Console.WriteLine($"  训练迭代次数: {model.IterationCount}")
        Console.WriteLine($"  训练基因数: {model.TrainingGeneCount}")
        Console.WriteLine($"  起始密码子频率: ATG={model.StartCodonFreq("ATG"):P1}, " &
                          $"GTG={model.StartCodonFreq("GTG"):P1}, TTG={model.StartCodonFreq("TTG"):P1}")
    End Sub

    ''' <summary>
    ''' 运行基因预测
    ''' </summary>
    Private Sub RunPrediction(config As RunConfig)
        Console.WriteLine("模式：基因预测")
        Console.WriteLine($"输入文件: {config.InputFile}")
        Console.WriteLine($"输出前缀: {config.OutputPrefix}")
        Console.WriteLine()

        ' 读取FASTA文件
        Console.WriteLine("读取输入序列...")
        Dim sequences = FastaReader.Read(config.InputFile)
        Console.WriteLine($"  读取到 {sequences.Count} 条序列，总长度 {sequences.Sum(Function(s) s.Length):N0} bp")

        If sequences.Count = 0 Then
            Console.WriteLine("错误：输入文件中没有有效序列")
            Return
        End If

        ' 获取或训练模型
        Dim model As TrainingModel

        If Not String.IsNullOrEmpty(config.ModelFile) AndAlso File.Exists(config.ModelFile) Then
            ' 加载已有模型
            Console.WriteLine($"加载训练模型: {config.ModelFile}")
            model = ModelSerializer.Load(config.ModelFile)
            Console.WriteLine("模型加载成功！")
        Else
            ' 从输入序列无监督训练
            Console.WriteLine("未指定模型文件，从输入序列进行无监督训练...")
            model = TrainingEngine.Train(sequences)
        End If

        ' 执行基因预测
        Console.WriteLine()
        Console.WriteLine("开始基因预测...")
        Dim pipeline As New PredictionPipeline(config.MinOrfLength)
        Dim results = pipeline.Predict(sequences, model)

        ' 输出结果
        Console.WriteLine()
        Console.WriteLine("输出预测结果...")

        Dim gffFile = config.OutputPrefix & ".gff"
        Dim proteinFile = config.OutputPrefix & "_proteins.faa"
        Dim nucleotideFile = config.OutputPrefix & "_nucleotides.fna"
        Dim scoreFile = config.OutputPrefix & "_scores.tsv"

        ResultWriter.WriteGff3(results, gffFile)
        Console.WriteLine($"  GFF3文件: {gffFile}")

        ResultWriter.WriteProteinFasta(results, proteinFile)
        Console.WriteLine($"  蛋白质FASTA: {proteinFile}")

        ResultWriter.WriteNucleotideFasta(results, nucleotideFile)
        Console.WriteLine($"  核苷酸FASTA: {nucleotideFile}")

        ResultWriter.WriteScoreTable(results, scoreFile)
        Console.WriteLine($"  得分表: {scoreFile}")

        ' 如果训练了新模型且指定了模型文件，保存模型
        If model.Trained AndAlso Not String.IsNullOrEmpty(config.ModelFile) AndAlso Not File.Exists(config.ModelFile) Then
            ModelSerializer.Save(model, config.ModelFile)
            Console.WriteLine($"  训练模型: {config.ModelFile}")
        End If

        ' 打印摘要
        ResultWriter.PrintSummary(results)
    End Sub

    ''' <summary>
    ''' 运行配置
    ''' </summary>
    Private Class RunConfig
        Public Property InputFile As String = ""
        Public Property OutputPrefix As String = "prodigal_output"
        Public Property ModelFile As String = ""
        Public Property TrainingMode As Boolean = False
        Public Property MinOrfLength As Integer = 90
    End Class

    ''' <summary>
    ''' 解析命令行参数
    ''' </summary>
    Private Function ParseArgs(args As String()) As RunConfig
        If args.Length = 0 Then Return Nothing

        Dim config As New RunConfig()
        Dim i As Integer = 0

        While i < args.Length
            Select Case args(i).ToLower()
                Case "-i", "--input"
                    i += 1
                    If i < args.Length Then config.InputFile = args(i)

                Case "-o", "--output"
                    i += 1
                    If i < args.Length Then config.OutputPrefix = args(i)

                Case "-m", "--model"
                    i += 1
                    If i < args.Length Then config.ModelFile = args(i)

                Case "-t", "--train"
                    config.TrainingMode = True

                Case "-l", "--min-length"
                    i += 1
                    If i < args.Length Then config.MinOrfLength = Integer.Parse(args(i))

                Case "-h", "--help"
                    Return Nothing

                Case Else
                    Console.WriteLine($"未知参数: {args(i)}")
                    Return Nothing
            End Select
            i += 1
        End While

        If String.IsNullOrEmpty(config.InputFile) Then
            Console.WriteLine("错误：必须指定输入文件 (-i)")
            Return Nothing
        End If

        If config.TrainingMode AndAlso String.IsNullOrEmpty(config.ModelFile) Then
            config.ModelFile = Path.ChangeExtension(config.InputFile, ".model")
        End If

        Return config
    End Function

    ''' <summary>
    ''' 打印使用说明
    ''' </summary>
    Private Sub PrintUsage()
        Console.WriteLine("用法:")
        Console.WriteLine()
        Console.WriteLine("  基因预测模式（默认）:")
        Console.WriteLine("    ProdigalVB -i input.fasta -o output_prefix [-m model.bin]")
        Console.WriteLine()
        Console.WriteLine("  模型训练模式:")
        Console.WriteLine("    ProdigalVB -t -i input.fasta -m model.bin")
        Console.WriteLine()
        Console.WriteLine("参数:")
        Console.WriteLine("  -i, --input       输入FASTA文件路径（必需）")
        Console.WriteLine("  -o, --output      输出文件前缀（默认: prodigal_output）")
        Console.WriteLine("  -m, --model       训练模型文件路径")
        Console.WriteLine("                    预测模式：加载已有模型（如未指定则自动训练）")
        Console.WriteLine("                    训练模式：模型保存路径")
        Console.WriteLine("  -t, --train       训练模式（只训练模型，不输出基因预测）")
        Console.WriteLine("  -l, --min-length  最小ORF长度，核苷酸数（默认: 90）")
        Console.WriteLine("  -h, --help        显示此帮助信息")
        Console.WriteLine()
        Console.WriteLine("输出文件（预测模式）:")
        Console.WriteLine("  <prefix>.gff              GFF3格式基因注释")
        Console.WriteLine("  <prefix>_proteins.faa     预测蛋白质序列")
        Console.WriteLine("  <prefix>_nucleotides.fna  预测基因核苷酸序列")
        Console.WriteLine("  <prefix>_scores.tsv       详细得分表")
        Console.WriteLine()
        Console.WriteLine("示例:")
        Console.WriteLine("  ProdigalVB -i genome.fasta -o mygenome")
        Console.WriteLine("  ProdigalVB -t -i genome.fasta -m mygenome.model")
        Console.WriteLine("  ProdigalVB -i genome.fasta -o mygenome -m mygenome.model")
    End Sub

End Module
