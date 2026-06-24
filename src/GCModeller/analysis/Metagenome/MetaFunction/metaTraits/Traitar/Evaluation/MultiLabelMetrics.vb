' ============================================================================
' Module 5: Multi-label Model Evaluation
' File: Evaluation/MultiLabelMetrics.vb
'
' 功能: 计算分类器在多个表型上的综合预测准确度。
'       对应论文中 "多标签模型评估模块"。
'
' 算法原理:
'   1. 混淆矩阵: 对每个表型统计 TP, TN, FP, FN
'   2. 宏平均 (Macro-average): 计算每个表型的召回率，再取平均，进而计算宏观准确率
'   3. 微平均 (Micro-average): 汇总所有表型的 TP/TN/FP/FN，计算总体准确率
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

Namespace Traitar.Evaluation

    ''' <summary>
    ''' 单个表型的混淆矩阵
    ''' </summary>
    Public Class ConfusionMatrix
        Public Property PhenotypeId As String
        Public Property PhenotypeName As String
        Public Property TP As Integer  ' True Positive
        Public Property TN As Integer  ' True Negative
        Public Property FP As Integer  ' False Positive
        Public Property FN As Integer  ' False Negative

        ''' <summary>召回率 (Sensitivity) = TP / (TP + FN)</summary>
        Public ReadOnly Property Recall As Double
            Get
                Dim denom = TP + FN
                If denom = 0 Then Return 0.0
                Return CDbl(TP) / denom
            End Get
        End Property

        ''' <summary>特异度 (Specificity) = TN / (TN + FP)</summary>
        Public ReadOnly Property Specificity As Double
            Get
                Dim denom = TN + FP
                If denom = 0 Then Return 0.0
                Return CDbl(TN) / denom
            End Get
        End Property

        ''' <summary>精确率 (Precision) = TP / (TP + FP)</summary>
        Public ReadOnly Property Precision As Double
            Get
                Dim denom = TP + FP
                If denom = 0 Then Return 0.0
                Return CDbl(TP) / denom
            End Get
        End Property

        ''' <summary>准确率 = (TP + TN) / (TP + TN + FP + FN)</summary>
        Public ReadOnly Property Accuracy As Double
            Get
                Dim denom = TP + TN + FP + FN
                If denom = 0 Then Return 0.0
                Return CDbl(TP + TN) / denom
            End Get
        End Property

        ''' <summary>F1 = 2 * P * R / (P + R)</summary>
        Public ReadOnly Property F1 As Double
            Get
                Dim denom = Precision + Recall
                If denom = 0 Then Return 0.0
                Return 2.0 * Precision * Recall / denom
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 多标签评估结果
    ''' </summary>
    Public Class MultiLabelMetrics
        ''' <summary>各表型的混淆矩阵</summary>
        Public Property PerPhenotype As New List(Of ConfusionMatrix)

        ''' <summary>宏平均准确率 = mean(各表型召回率) × mean(各表型特异度) × 2 ... (论文公式)</summary>
        Public ReadOnly Property MacroAccuracy As Double
            Get
                If PerPhenotype.Count = 0 Then Return 0.0
                Dim meanRecall = PerPhenotype.Average(Function(cm) cm.Recall)
                Dim meanSpecificity = PerPhenotype.Average(Function(cm) cm.Specificity)
                ' 论文中的 Macroaccuracy = (meanRecall + meanSpecificity) / 2
                Return (meanRecall + meanSpecificity) / 2.0
            End Get
        End Property

        ''' <summary>微平均准确率 = Σ(TP+TN) / Σ(TP+TN+FP+FN)</summary>
        Public ReadOnly Property MicroAccuracy As Double
            Get
                Dim totalTP = PerPhenotype.Sum(Function(cm) cm.TP)
                Dim totalTN = PerPhenotype.Sum(Function(cm) cm.TN)
                Dim totalFP = PerPhenotype.Sum(Function(cm) cm.FP)
                Dim totalFN = PerPhenotype.Sum(Function(cm) cm.FN)
                Dim denom = totalTP + totalTN + totalFP + totalFN
                If denom = 0 Then Return 0.0
                Return CDbl(totalTP + totalTN) / denom
            End Get
        End Property

        ''' <summary>宏平均 F1</summary>
        Public ReadOnly Property MacroF1 As Double
            Get
                If PerPhenotype.Count = 0 Then Return 0.0
                Return PerPhenotype.Average(Function(cm) cm.F1)
            End Get
        End Property

        ''' <summary>微平均 F1</summary>
        Public ReadOnly Property MicroF1 As Double
            Get
                Dim totalTP = PerPhenotype.Sum(Function(cm) cm.TP)
                Dim totalFP = PerPhenotype.Sum(Function(cm) cm.FP)
                Dim totalFN = PerPhenotype.Sum(Function(cm) cm.FN)
                Dim microP = If(totalTP + totalFP > 0, CDbl(totalTP) / (totalTP + totalFP), 0.0)
                Dim microR = If(totalTP + totalFN > 0, CDbl(totalTP) / (totalTP + totalFN), 0.0)
                If microP + microR = 0 Then Return 0.0
                Return 2.0 * microP * microR / (microP + microR)
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 多标签评估器
    ''' </summary>
    Public Class MultiLabelEvaluator

        ''' <summary>
        ''' 计算多标签评估指标
        ''' </summary>
        ''' <param name="trueLabels">真实标签: (sampleId, phenotypeId) -> 0/1</param>
        ''' <param name="predLabels">预测标签: (sampleId, phenotypeId) -> 0/1</param>
        ''' <param name="phenotypeNames">表型ID -> 名称</param>
        Public Function Evaluate(trueLabels As Dictionary(Of Tuple(Of String, String), Integer),
                                  predLabels As Dictionary(Of Tuple(Of String, String), Integer),
                                  phenotypeNames As Dictionary(Of String, String)) As MultiLabelMetrics
            Dim metrics As New MultiLabelMetrics()

            ' 收集所有表型ID
            Dim phenotypeIds As New HashSet(Of String)
            For Each key In trueLabels.Keys
                phenotypeIds.Add(key.Item2)
            Next

            ' 对每个表型计算混淆矩阵
            For Each pid In phenotypeIds
                Dim cm As New ConfusionMatrix With {
                    .PhenotypeId = pid,
                    .PhenotypeName = If(phenotypeNames.ContainsKey(pid), phenotypeNames(pid), pid)
                }

                For Each key In trueLabels.Keys
                    If key.Item2 <> pid Then Continue For
                    Dim trueLabel = trueLabels(key)
                    Dim predLabel = If(predLabels.ContainsKey(key), predLabels(key), 0)

                    If trueLabel = 1 AndAlso predLabel = 1 Then
                        cm.TP += 1
                    ElseIf trueLabel = 0 AndAlso predLabel = 0 Then
                        cm.TN += 1
                    ElseIf trueLabel = 0 AndAlso predLabel = 1 Then
                        cm.FP += 1
                    ElseIf trueLabel = 1 AndAlso predLabel = 0 Then
                        cm.FN += 1
                    End If
                Next

                metrics.PerPhenotype.Add(cm)
            Next

            Return metrics
        End Function

        ''' <summary>
        ''' 生成评估报告
        ''' </summary>
        Public Function GenerateReport(metrics As MultiLabelMetrics) As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("=" & New String("="c, 70))
            sb.AppendLine("多标签模型评估报告")
            sb.AppendLine("=" & New String("="c, 70))
            sb.AppendLine()
            sb.AppendLine($"总体指标:")
            sb.AppendLine($"  Macro-Accuracy (宏平均准确率): {metrics.MacroAccuracy:F4}")
            sb.AppendLine($"  Micro-Accuracy (微平均准确率): {metrics.MicroAccuracy:F4}")
            sb.AppendLine($"  Macro-F1:                      {metrics.MacroF1:F4}")
            sb.AppendLine($"  Micro-F1:                      {metrics.MicroF1:F4}")
            sb.AppendLine()
            sb.AppendLine($"各表型详细指标:")
            sb.AppendLine($"  {"PhenotypeID",-12} {"TP",-5} {"TN",-5} {"FP",-5} {"FN",-5} {"Acc",-7} {"P",-7} {"R",-7} {"F1",-7} Name")
            sb.AppendLine($"  " & New String("-"c, 80))
            For Each cm In metrics.PerPhenotype
                sb.AppendLine($"  {cm.PhenotypeId,-12} {cm.TP,-5} {cm.TN,-5} {cm.FP,-5} {cm.FN,-5} " &
                              $"{cm.Accuracy,-7:F3} {cm.Precision,-7:F3} {cm.Recall,-7:F3} {cm.F1,-7:F3} {cm.PhenotypeName}")
            Next

            Return sb.ToString()
        End Function
    End Class
End Namespace
