' ============================================================================
' DIABLO_Analysis_Example.vb
' ============================================================================
' VB.NET DIABLO 多组学数据关联分析 - 使用示例
' 演示如何使用 MixOmicsVB 命名空间中的各个模块
' ============================================================================

Imports SMRUCC.genomics.Analysis.Microarray.MixOmics

Module DIABLOExample

    Sub Main1()
        Console.WriteLine("="c, 70)
        Console.WriteLine("DIABLO 多组学数据关联分析 - VB.NET 实现示例")
        Console.WriteLine("="c, 70)
        Console.WriteLine()

        ' ====================================================================
        ' 示例1: 使用模拟数据进行完整DIABLO分析
        ' ====================================================================
        Console.WriteLine("--- 示例1: 模拟数据完整分析 ---")
        Console.WriteLine()

        ' 1.1 生成模拟多组学数据
        Console.WriteLine("[1] 生成模拟多组学数据...")
        Dim simData = DIABLOUtils.SimulateMultiOmicsData(
            nSamples:=60,
            nClasses:=3,
            blockSizes:=New Integer() {100, 80, 50},     ' 三个组学层: 100, 80, 50 个变量
            nInformative:=New Integer() {15, 10, 8},      ' 每层信息变量数
            correlationStrength:=0.8,
            noiseLevel:=0.3,
            seed:=42
        )
        Console.WriteLine($"    样本数: {simData.YLabels.Length}")
        Console.WriteLine($"    类别数: {simData.ClassLabels.Length}")
        For k As Integer = 0 To simData.X.Count - 1
            Console.WriteLine($"    数据块 {k}: {simData.X(k).Rows} x {simData.X(k).Cols} (信息变量: {simData.NInformative(k)})")
        Next
        Console.WriteLine()

        ' 1.2 创建设计矩阵
        Console.WriteLine("[2] 创建设计矩阵...")
        Dim design As Matrix = DIABLOUtils.CreateFullDesign(simData.X.Count)
        Console.WriteLine("    全连接设计矩阵 (3个数据块 + Y):")
        Console.WriteLine(design.ToString())
        Console.WriteLine()

        ' 1.3 设置稀疏度参数 (keepX)
        Console.WriteLine("[3] 设置稀疏度参数...")
        Dim ncomp As Integer = 2
        Dim keepX As New List(Of Integer())()
        keepX.Add(New Integer() {20, 15})   ' 块0: 第1成分保留20个变量, 第2成分保留15个
        keepX.Add(New Integer() {15, 10})   ' 块1: 第1成分保留15个变量, 第2成分保留10个
        keepX.Add(New Integer() {10, 8})    ' 块2: 第1成分保留10个变量, 第2成分保留8个
        Console.WriteLine($"    成分数: {ncomp}")
        For k As Integer = 0 To simData.X.Count - 1
            Console.WriteLine($"    块 {k} keepX: {String.Join(", ", keepX(k))}")
        Next
        Console.WriteLine()

        ' 1.4 拟合DIABLO模型
        Console.WriteLine("[4] 拟合DIABLO模型...")
        Dim model As New DIABLO(
            X:=simData.X,
            YLabels:=simData.YLabels,
            classLabels:=simData.ClassLabels,
            ncomp:=ncomp,
            design:=design,
            keepX:=keepX,
            maxIter:=500,
            tol:=0.000001
        )
        model.Fit()
        Console.WriteLine("    模型拟合完成!")
        Console.WriteLine()

        ' 1.5 查看载荷和潜在变量
        Console.WriteLine("[5] 模型结果:")
        For k As Integer = 0 To simData.X.Count - 1
            Console.WriteLine($"    数据块 {k}:")
            For h As Integer = 0 To ncomp - 1
                Dim loading As Matrix = model.Loadings(k)(h)
                Dim nNonZero As Integer = 0
                For j As Integer = 0 To loading.Rows - 1
                    If Math.Abs(loading(j, 0)) > 0.0000000001 Then nNonZero += 1
                Next
                Console.WriteLine($"      成分 {h + 1}: 非零载荷数 = {nNonZero}/{loading.Rows}")
            Next
        Next
        Console.WriteLine()

        ' 1.6 解释方差
        Console.WriteLine("[6] 解释方差比例:")
        For k As Integer = 0 To simData.X.Count - 1
            Console.Write($"    数据块 {k}: ")
            For h As Integer = 0 To ncomp - 1
                Console.Write($"成分{h + 1}={model.ExplainedVariance(k)(h):F4}  ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()

        ' 1.7 块间相关性
        Console.WriteLine("[7] 块间成分相关性:")
        Dim blockCor As Matrix(,) = model.ComputeBlockCorrelation()
        For k1 As Integer = 0 To simData.X.Count - 1
            For k2 As Integer = k1 + 1 To simData.X.Count - 1
                Console.WriteLine($"    块{k1} vs 块{k2} (成分1): {blockCor(k1, k2)(0, 0):F4}")
            Next
        Next
        Console.WriteLine()

        ' 1.8 训练集预测
        Console.WriteLine("[8] 训练集预测评估:")
        Dim trainPred As Integer() = model.Predict(simData.X, "centroids_dist")
        Dim trainEval = DIABLOCrossValidation.ComputeBER(trainPred, simData.YLabels, simData.ClassLabels.Length)
        Console.WriteLine($"    准确率: {trainEval.Accuracy:F4}")
        Console.WriteLine($"    平衡错误率 (BER): {trainEval.BER:F4}")
        Console.WriteLine()

        ' 1.9 变量重要性
        Console.WriteLine("[9] 变量重要性 (Top 5 per block):")
        Dim varImp As List(Of Double()) = model.ComputeVariableImportance()
        For k As Integer = 0 To simData.X.Count - 1
            Dim sortedIdx As Integer() = Enumerable.Range(0, varImp(k).Length) _
                .OrderByDescending(Function(j) varImp(k)(j)).Take(5).ToArray()
            Console.Write($"    块 {k}: 变量索引 = ")
            For Each idx In sortedIdx
                Console.Write($"{idx}({varImp(k)(idx):F3}) ")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()

        ' 1.10 选中的变量
        Console.WriteLine("[10] 选中的变量索引:")
        Dim selVars As List(Of List(Of Integer())) = model.GetSelectedVariables()
        For k As Integer = 0 To simData.X.Count - 1
            For h As Integer = 0 To ncomp - 1
                Console.WriteLine($"    块{k} 成分{h + 1}: {String.Join(", ", selVars(k)(h))}")
            Next
        Next
        Console.WriteLine()

        ' ====================================================================
        ' 示例2: 交叉验证调优
        ' ====================================================================
        Console.WriteLine("--- 示例2: 交叉验证参数调优 ---")
        Console.WriteLine()

        ' 2.1 调优成分数
        Console.WriteLine("[1] 调优成分数 (5折交叉验证)...")
        Dim cv As New DIABLOCrossValidation(simData.X, simData.YLabels, simData.ClassLabels)
        Dim ncompResult = cv.TuneNComp(
            ncompRange:=New Integer() {1, 2, 3},
            nFolds:=5,
            design:=design
        )
        Console.WriteLine("    成分数  BER       准确率    标准误差")
        For i As Integer = 0 To ncompResult.NCompValues.Length - 1
            Console.WriteLine($"    {ncompResult.NCompValues(i),6}   {ncompResult.BERValues(i):F4}    {ncompResult.AccuracyValues(i):F4}    {ncompResult.BERStdErrors(i):F4}")
        Next
        Console.WriteLine($"    最优成分数: {ncompResult.OptimalNComp}")
        Console.WriteLine()

        ' ====================================================================
        ' 示例3: 自定义设计矩阵
        ' ====================================================================
        Console.WriteLine("--- 示例3: 自定义设计矩阵 ---")
        Console.WriteLine()

        ' 只连接块0与Y、块1与Y，块0与块1之间不直接连接
        Dim customConnections As New List(Of (Integer, Integer, Double)) From {
            (0, 2, 1.0),   ' 块0 与 块2 连接
            (0, 1, 0.5)    ' 块0 与 块1 弱连接
        }
        Dim customDesign As Matrix = DIABLOUtils.CreateCustomDesign(customConnections, 3)
        Console.WriteLine("    自定义设计矩阵:")
        Console.WriteLine(customDesign.ToString())
        Console.WriteLine()

        ' ====================================================================
        ' 示例4: 多块整合分析扩展
        ' ====================================================================
        Console.WriteLine("--- 示例4: 多块整合分析扩展 ---")
        Console.WriteLine()

        ' 4.1 RGCCA协方差分析
        Console.WriteLine("[1] RGCCA块间协方差分析...")
        Dim centeredX As New List(Of Matrix)()
        For Each block In simData.X
            centeredX.Add(block.Center())
        Next
        Dim rgccaCov As Matrix = MultiBlockIntegration.ComputeRGCCACovariance(centeredX, design)
        Console.WriteLine("    块间协方差矩阵:")
        Console.WriteLine(rgccaCov.ToString())
        Console.WriteLine()

        ' 4.2 Procrustes分析
        Console.WriteLine("[2] Procrustes分析 (块0 vs 块1)...")
        Dim scores0 As Matrix = model.LatentVars(0)(0)  ' 块0成分1的得分
        Dim scores1 As Matrix = model.LatentVars(1)(0)  ' 块1成分1的得分
        ' 合并所有成分为得分矩阵
        Dim scoreMat0 As New Matrix(simData.YLabels.Length, ncomp)
        Dim scoreMat1 As New Matrix(simData.YLabels.Length, ncomp)
        For h As Integer = 0 To ncomp - 1
            For i As Integer = 0 To simData.YLabels.Length - 1
                scoreMat0(i, h) = model.LatentVars(0)(h)(i, 0)
                scoreMat1(i, h) = model.LatentVars(1)(h)(i, 0)
            Next
        Next
        Dim procrustesResult = MultiBlockIntegration.ProcrustesAnalysis(scoreMat0, scoreMat1)
        Console.WriteLine($"    Procrustes相关系数: {procrustesResult.ProcrustesCorrelation:F4}")
        Console.WriteLine($"    残差平方和: {procrustesResult.SumOfSquares:F4}")
        Console.WriteLine()

        ' ====================================================================
        ' 示例5: 使用Pipeline进行一键分析
        ' ====================================================================
        Console.WriteLine("--- 示例5: Pipeline一键分析 ---")
        Console.WriteLine()

        Dim pipelineResult = DIABLOPipeline.Run(
            X:=simData.X,
            YLabels:=simData.YLabels,
            classLabels:=simData.ClassLabels,
            ncomp:=0,           ' 自动调优
            keepX:=Nothing,     ' 自动调优
            design:=Nothing,    ' 全连接
            autoTune:=True,
            nFolds:=3           ' 减少折数以加速演示
        )

        Console.WriteLine($"    最优成分数: {pipelineResult.NComp}")
        Console.WriteLine($"    训练集准确率: {pipelineResult.TrainAccuracy:F4}")
        Console.WriteLine($"    训练集BER: {pipelineResult.TrainBER:F4}")
        Console.WriteLine()

        ' ====================================================================
        ' 示例6: 新样本预测
        ' ====================================================================
        Console.WriteLine("--- 示例6: 新样本预测 ---")
        Console.WriteLine()

        ' 模拟5个新样本
        Dim rng As New Random(123)
        Dim Xnew As New List(Of Matrix)()
        For k As Integer = 0 To simData.X.Count - 1
            Dim Xk As New Matrix(5, simData.X(k).Cols)
            For i As Integer = 0 To 4
                For j As Integer = 0 To simData.X(k).Cols - 1
                    Xk(i, j) = rng.NextDouble() * 4.0 - 2.0
                Next
            Next
            Xnew.Add(Xk)
        Next

        Dim predictions As Integer() = model.Predict(Xnew, "centroids_dist")
        Console.WriteLine("    新样本预测类别:")
        For i As Integer = 0 To predictions.Length - 1
            Console.WriteLine($"      样本 {i + 1}: {simData.ClassLabels(predictions(i))}")
        Next
        Console.WriteLine()

        ' ====================================================================
        ' 示例7: AUC计算
        ' ====================================================================
        Console.WriteLine("--- 示例7: AUC计算 ---")
        Console.WriteLine()

        ' 使用训练集的潜在变量作为得分
        Dim allScores As New Matrix(simData.YLabels.Length, simData.ClassLabels.Length)
        For c As Integer = 0 To simData.ClassLabels.Length - 1
            For i As Integer = 0 To simData.YLabels.Length - 1
                ' 简化: 使用到各类质心的距离的负值作为得分
                allScores(i, c) = rng.NextDouble()  ' 演示用随机得分
            Next
        Next
        Dim auc As Double = DIABLOUtils.ComputeMulticlassAUC(allScores, simData.YLabels, simData.ClassLabels.Length)
        Console.WriteLine($"    多分类AUC (One-vs-Rest): {auc:F4}")
        Console.WriteLine()

        Console.WriteLine("="c, 70)
        Console.WriteLine("分析完成!")
        Console.WriteLine("="c, 70)

    End Sub

End Module
