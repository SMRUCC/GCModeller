' ============================================================================
' Program.vb - Prodigal VB.NET 基因预测程序 主入口
' 用法：
'   基因预测模式：ProdigalVB -i input.fasta -o output_prefix [-m model.bin]
'   模型训练模式：ProdigalVB -t -i input.fasta -m model.bin
' ============================================================================

Imports System.IO
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Worker

    ''' <summary>
    ''' 运行模型训练
    ''' </summary>
    Public Sub RunTraining(config As RunConfig)
        Console.WriteLine("模式：无监督模型训练")
        Console.WriteLine($"输入文件: {config.InputFile}")
        Console.WriteLine($"模型输出: {config.ModelFile}")
        Console.WriteLine()

        ' 读取FASTA文件
        Console.WriteLine("读取输入序列...")
        Dim sequences = FastaFile.Read(config.InputFile)
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
    Public Sub RunPrediction(config As RunConfig)
        Console.WriteLine("模式：基因预测")
        Console.WriteLine($"输入文件: {config.InputFile}")
        Console.WriteLine($"输出前缀: {config.OutputPrefix}")
        Console.WriteLine()

        ' 读取FASTA文件
        Console.WriteLine("读取输入序列...")
        Dim sequences = FastaFile.Read(config.InputFile)
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
    Public Class RunConfig
        Public Property InputFile As String = ""
        Public Property OutputPrefix As String = "prodigal_output"
        Public Property ModelFile As String = ""
        Public Property TrainingMode As Boolean = False
        Public Property MinOrfLength As Integer = 90
    End Class

End Module
