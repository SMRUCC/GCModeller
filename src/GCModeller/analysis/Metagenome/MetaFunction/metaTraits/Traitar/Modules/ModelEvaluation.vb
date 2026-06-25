' ============================================================================
' ModelEvaluation.vb - 模块8：多标签模型评估模块
'
' 论文对应：
'   "多标签模型评估模块"
'
' 核心功能：
'   1. 混淆矩阵计算：统计TP, TN, FP, FN
'   2. 宏平均：计算宏召回率，进而计算宏观准确率
'   3. 微平均：计算总体准确率，评估整体分类表现
'
' 算法原理：
'   - 多标签分类理论
'   - 宏平均（Macro-average）：对每个表型单独计算指标，然后取平均
'   - 微平均（Micro-average）：汇总所有表型的TP/TN/FP/FN，然后计算指标
' ============================================================================

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 模块8：多标签模型评估模块
    ''' 计算分类器在多个表型上的综合预测准确度
    ''' </summary>
    Public Class ModelEvaluation

        ''' <summary>
        ''' 混淆矩阵
        ''' </summary>
        Public Class ConfusionMatrix
            Public Property TP As Integer  ' True Positive
            Public Property TN As Integer  ' True Negative
            Public Property FP As Integer  ' False Positive
            Public Property FN As Integer  ' False Negative

            ''' <summary>准确率 = (TP + TN) / Total</summary>
            Public ReadOnly Property Accuracy As Double
                Get
                    Dim total As Integer = TP + TN + FP + FN
                    If total = 0 Then Return 0.0
                    Return CDbl(TP + TN) / CDbl(total)
                End Get
            End Property

            ''' <summary>精确率 = TP / (TP + FP)</summary>
            Public ReadOnly Property Precision As Double
                Get
                    If TP + FP = 0 Then Return 0.0
                    Return CDbl(TP) / CDbl(TP + FP)
                End Get
            End Property

            ''' <summary>召回率 = TP / (TP + FN)</summary>
            Public ReadOnly Property Recall As Double
                Get
                    If TP + FN = 0 Then Return 0.0
                    Return CDbl(TP) / CDbl(TP + FN)
                End Get
            End Property

            ''' <summary>F1分数 = 2 × P × R / (P + R)</summary>
            Public ReadOnly Property F1Score As Double
                Get
                    If Precision + Recall = 0 Then Return 0.0
                    Return 2.0 * Precision * Recall / (Precision + Recall)
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return String.Format("TP={0}, TN={1}, FP={2}, FN={3}, Acc={4:F4}, P={5:F4}, R={6:F4}, F1={7:F4}",
                                     TP, TN, FP, FN, Accuracy, Precision, Recall, F1Score)
            End Function
        End Class

        ''' <summary>
        ''' 计算单个表型的混淆矩阵
        ''' </summary>
        ''' <param name="yTrue">真实标签（0/1）</param>
        ''' <param name="yPred">预测标签（0/1）</param>
        ''' <returns>混淆矩阵</returns>
        Public Function ComputeConfusionMatrix(yTrue As Integer(), yPred As Integer()) As ConfusionMatrix
            Dim cm As New ConfusionMatrix()

            For i As Integer = 0 To yTrue.Length - 1
                If yTrue(i) = 1 AndAlso yPred(i) = 1 Then
                    cm.TP += 1
                ElseIf yTrue(i) = 0 AndAlso yPred(i) = 0 Then
                    cm.TN += 1
                ElseIf yTrue(i) = 0 AndAlso yPred(i) = 1 Then
                    cm.FP += 1
                ElseIf yTrue(i) = 1 AndAlso yPred(i) = 0 Then
                    cm.FN += 1
                End If
            Next

            Return cm
        End Function

        ''' <summary>
        ''' 宏平均评估
        ''' 论文：计算宏召回率，进而计算宏观准确率
        '''
        ''' 宏平均 = 对每个表型单独计算指标，然后取算术平均
        ''' 宏观准确率(Macroaccuracy) = 平均(各表型准确率)
        ''' </summary>
        ''' <param name="allTrue">所有表型的真实标签（表型ID -> 标签数组）</param>
        ''' <param name="allPred">所有表型的预测标签（表型ID -> 标签数组）</param>
        ''' <returns>宏平均评估结果</returns>
        Public Function MacroAverageEvaluation(
            allTrue As Dictionary(Of String, Integer()),
            allPred As Dictionary(Of String, Integer())) As (macroAccuracy As Double,
                                                                    macroPrecision As Double,
                                                                    macroRecall As Double,
                                                                    macroF1 As Double,
                                                                    perPhenotype As Dictionary(Of String, ConfusionMatrix))

            Dim perPhenotype As New Dictionary(Of String, ConfusionMatrix)()
            Dim accSum As Double = 0.0
            Dim precSum As Double = 0.0
            Dim recSum As Double = 0.0
            Dim f1Sum As Double = 0.0
            Dim count As Integer = 0

            For Each kvp As KeyValuePair(Of String, Integer()) In allTrue
                Dim phenoId As String = kvp.Key
                If Not allPred.ContainsKey(phenoId) Then Continue For

                Dim yTrue As Integer() = kvp.Value
                Dim yPred As Integer() = allPred(phenoId)

                Dim cm As ConfusionMatrix = ComputeConfusionMatrix(yTrue, yPred)
                perPhenotype(phenoId) = cm

                accSum += cm.Accuracy
                precSum += cm.Precision
                recSum += cm.Recall
                f1Sum += cm.F1Score
                count += 1
            Next

            Dim macroAccuracy As Double = If(count > 0, accSum / count, 0.0)
            Dim macroPrecision As Double = If(count > 0, precSum / count, 0.0)
            Dim macroRecall As Double = If(count > 0, recSum / count, 0.0)
            Dim macroF1 As Double = If(count > 0, f1Sum / count, 0.0)

            Console.WriteLine("[模块8] 宏平均评估结果:")
            Console.WriteLine("       表型数: {0}", count)
            Console.WriteLine("       宏准确率(MacroAccuracy): {0:F4}", macroAccuracy)
            Console.WriteLine("       宏精确率(MacroPrecision): {0:F4}", macroPrecision)
            Console.WriteLine("       宏召回率(MacroRecall): {0:F4}", macroRecall)
            Console.WriteLine("       宏F1(MacroF1): {0:F4}", macroF1)

            Return (macroAccuracy, macroPrecision, macroRecall, macroF1, perPhenotype)
        End Function

        ''' <summary>
        ''' 微平均评估
        ''' 论文：计算总体准确率，评估整体分类表现
        '''
        ''' 微平均 = 汇总所有表型的TP/TN/FP/FN，然后计算指标
        ''' 微观准确率(Accuracy) = (ΣTP + ΣTN) / (ΣTP + ΣTN + ΣFP + ΣFN)
        ''' </summary>
        ''' <param name="allTrue">所有表型的真实标签</param>
        ''' <param name="allPred">所有表型的预测标签</param>
        ''' <returns>微平均评估结果</returns>
        Public Function MicroAverageEvaluation(
            allTrue As Dictionary(Of String, Integer()),
            allPred As Dictionary(Of String, Integer())) As (microAccuracy As Double,
                                                                    microPrecision As Double,
                                                                    microRecall As Double,
                                                                    microF1 As Double,
                                                                    totalCM As ConfusionMatrix)

            Dim totalCM As New ConfusionMatrix()

            For Each kvp As KeyValuePair(Of String, Integer()) In allTrue
                Dim phenoId As String = kvp.Key
                If Not allPred.ContainsKey(phenoId) Then Continue For

                Dim yTrue As Integer() = kvp.Value
                Dim yPred As Integer() = allPred(phenoId)

                Dim cm As ConfusionMatrix = ComputeConfusionMatrix(yTrue, yPred)
                totalCM.TP += cm.TP
                totalCM.TN += cm.TN
                totalCM.FP += cm.FP
                totalCM.FN += cm.FN
            Next

            Console.WriteLine("[模块8] 微平均评估结果:")
            Console.WriteLine("       总TP={0}, TN={1}, FP={2}, FN={3}",
                              totalCM.TP, totalCM.TN, totalCM.FP, totalCM.FN)
            Console.WriteLine("       微准确率(MicroAccuracy): {0:F4}", totalCM.Accuracy)
            Console.WriteLine("       微精确率(MicroPrecision): {0:F4}", totalCM.Precision)
            Console.WriteLine("       微召回率(MicroRecall): {0:F4}", totalCM.Recall)
            Console.WriteLine("       微F1(MicroF1): {0:F4}", totalCM.F1Score)

            Return (totalCM.Accuracy, totalCM.Precision, totalCM.Recall, totalCM.F1Score, totalCM)
        End Function

        ''' <summary>
        ''' 生成完整评估报告
        ''' </summary>
        Public Function GenerateEvaluationReport(
            allTrue As Dictionary(Of String, Integer()),
            allPred As Dictionary(Of String, Integer()),
            phenoNames As Dictionary(Of String, String)) As String

            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("========================================")
            sb.AppendLine("    多标签模型评估报告")
            sb.AppendLine("========================================")
            sb.AppendLine()

            ' 宏平均
            Dim macroResult = MacroAverageEvaluation(allTrue, allPred)
            sb.AppendLine("--- 宏平均 (Macro-Average) ---")
            sb.AppendLine(String.Format("宏观准确率: {0:F4}", macroResult.macroAccuracy))
            sb.AppendLine(String.Format("宏精确率:   {0:F4}", macroResult.macroPrecision))
            sb.AppendLine(String.Format("宏召回率:   {0:F4}", macroResult.macroRecall))
            sb.AppendLine(String.Format("宏F1分数:   {0:F4}", macroResult.macroF1))
            sb.AppendLine()

            ' 微平均
            Dim microResult = MicroAverageEvaluation(allTrue, allPred)
            sb.AppendLine("--- 微平均 (Micro-Average) ---")
            sb.AppendLine(String.Format("微观准确率: {0:F4}", microResult.microAccuracy))
            sb.AppendLine(String.Format("微精确率:   {0:F4}", microResult.microPrecision))
            sb.AppendLine(String.Format("微召回率:   {0:F4}", microResult.microRecall))
            sb.AppendLine(String.Format("微F1分数:   {0:F4}", microResult.microF1))
            sb.AppendLine()

            ' 各表型详细结果
            sb.AppendLine("--- 各表型详细结果 ---")
            sb.AppendLine(String.Format("{0,-10} {1,-30} {2,-8} {3,-8} {4,-8} {5:-8} {6:-8} {7:-8}",
                                        "表型ID", "表型名称", "TP", "TN", "FP", "FN", "准确率", "F1"))
            sb.AppendLine(New String("-"c, 100))

            For Each kvp As KeyValuePair(Of String, ConfusionMatrix) In macroResult.perPhenotype
                Dim name As String = ""
                If phenoNames.ContainsKey(kvp.Key) Then
                    name = phenoNames(kvp.Key)
                End If
                Dim cm As ConfusionMatrix = kvp.Value
                sb.AppendLine(String.Format("{0,-10} {1,-30} {2,-8} {3,-8} {4,-8} {5:-8} {6:-8:F4} {7:-8:F4}",
                                            kvp.Key,
                                            If(name.Length > 30, name.Substring(0, 30), name),
                                            cm.TP, cm.TN, cm.FP, cm.FN,
                                            cm.Accuracy, cm.F1Score))
            Next

            Return sb.ToString()
        End Function

    End Class

End Namespace
