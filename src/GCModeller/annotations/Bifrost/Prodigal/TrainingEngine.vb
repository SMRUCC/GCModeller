
' ========================================================================
' 无监督训练引擎
' ========================================================================

Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 无监督训练引擎
''' 从输入序列本身学习统计特征，不需要外接训练集。
''' 
''' 训练流程：
''' 1. 使用默认参数做一次快速预测
''' 2. 根据预测到的"高置信度"基因，估计编码区/非编码区统计量
''' 3. 重新构建编码模型和起始位点模型
''' 4. 用新模型重新打分和预测
''' 5. 迭代直到模型参数和基因预测结果稳定
''' </summary>
Public Class TrainingEngine

    ''' <summary>最大训练迭代次数</summary>
    Private Const MaxIterations As Integer = 5

    ''' <summary>收敛阈值（基因数量变化）</summary>
    Private Const ConvergenceThreshold As Integer = 5

    ''' <summary>训练用最小基因数</summary>
    Private Const MinTrainingGenes As Integer = 20

    ''' <summary>
    ''' 执行无监督训练，生成训练模型
    ''' </summary>
    Public Shared Function Train(sequences As IReadOnlyCollection(Of FastaSeq)) As TrainingModel
        Console.WriteLine("开始无监督训练...")

        Dim model As New TrainingModel()

        ' 计算整体GC含量
        Dim totalGc As Double = 0
        Dim totalLen As Integer = 0
        For Each seq In sequences
            totalGc += SequenceUtils.ComputeGcContent(seq.SequenceData) * seq.Length
            totalLen += seq.Length
        Next
        model.GcContent = If(totalLen > 0, totalGc / totalLen, 0.5)
        Console.WriteLine($"  GC含量: {model.GcContent:P1}")

        ' 初始化默认模型
        CodingModel.InitializeDefaultModel(model, model.GcContent)
        Console.WriteLine("  已初始化默认编码区模型")

        ' 迭代训练
        Dim prevGeneCount As Integer = 0

        For iteration = 1 To MaxIterations
            Console.WriteLine($"  迭代 {iteration}/{MaxIterations}...")

            ' 第一步：用当前模型预测基因
            Dim allOrfs As New List(Of CandidateOrf)()
            For Each seq In sequences
                Dim finder As New OrfFinder(90)
                Dim orfs = finder.FindOrfs(seq)
                ScoringEngine.ScoreForSequence(orfs, model, seq.SequenceData)
                allOrfs.AddRange(orfs)
            Next

            ' 第二步：选择高置信度基因作为训练集
            Dim selectedGenes = DynamicProgramming.SelectGenes(allOrfs)
            Console.WriteLine($"    预测基因数: {selectedGenes.Count}")

            If selectedGenes.Count < MinTrainingGenes Then
                Console.WriteLine($"    训练基因数不足({selectedGenes.Count} < {MinTrainingGenes})，使用默认模型")
                Exit For
            End If

            ' 第三步：用选中的基因重新构建模型
            ' 过滤：选择得分较高的基因
            Dim trainingGenes = SelectTrainingGenes(selectedGenes, sequences)
            Console.WriteLine($"    训练基因数: {trainingGenes.Count}")

            ' 构建编码区模型
            CodingModel.BuildModel(model, sequences, trainingGenes)

            ' 构建RBS模型
            RbsModel.BuildModel(model, sequences, trainingGenes)

            ' 构建起始密码子模型
            StartCodonModel.BuildModel(model, trainingGenes)

            ' 更新平均基因长度
            model.AvgGeneLength = trainingGenes.Average(Function(g) CDbl(g.Length))
            model.TrainingGeneCount = trainingGenes.Count
            model.IterationCount = iteration

            ' 检查收敛
            If iteration > 1 AndAlso Math.Abs(selectedGenes.Count - prevGeneCount) < ConvergenceThreshold Then
                Console.WriteLine("    模型已收敛")
                Exit For
            End If
            prevGeneCount = selectedGenes.Count
        Next

        model.Trained = True
        Console.WriteLine($"训练完成！迭代次数: {model.IterationCount}, 训练基因数: {model.TrainingGeneCount}")

        Return model
    End Function

    ''' <summary>
    ''' 选择用于训练的高置信度基因
    ''' 策略：选择得分在上半部分的基因，且长度>120bp
    ''' </summary>
    Private Shared Function SelectTrainingGenes(genes As List(Of CandidateOrf),
                                                  sequences As IReadOnlyCollection(Of FastaSeq)) As List(Of CandidateOrf)
        ' 按总得分降序排序
        Dim sorted = genes.OrderByDescending(Function(g) g.TotalScore).ToList()

        ' 选择上半部分且长度>120bp的基因
        Dim cutoff = Math.Max(MinTrainingGenes, sorted.Count \ 2)
        Dim trainingGenes = sorted.Take(cutoff).Where(Function(g) g.Length >= 120).ToList()

        ' 如果还不够，降低标准
        If trainingGenes.Count < MinTrainingGenes Then
            trainingGenes = sorted.Where(Function(g) g.Length >= 90).Take(MinTrainingGenes).ToList()
        End If

        Return trainingGenes
    End Function

End Class
