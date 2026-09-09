Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 二分类模型的评估指标
''' </summary>
''' <remarks>
''' 论文在评估 P-NET 时使用了 AUC、AUPRC、accuracy、F1、precision 与 recall 等一整套指标：
'''
''' + AUC 基于整条 ROC 曲线，这里采用 Mann-Whitney U 统计量的等价形式计算，并对并列值做秩修正；
''' + AUPRC 采用平均精度（Average Precision）的计算方式，对类别不平衡的场景比 AUC 更敏感；
''' + 点估计指标（accuracy / F1 / precision / recall）在给定阈值之后由混淆矩阵直接计算。
'''
''' 论文中对不同指标所采用的显著性检验（DeLong 检验、Bootstrap 检验、t 检验等）
''' 不在本模块的实现范围之内。
''' </remarks>
Public Module Metrics

    ''' <summary>
    ''' 计算 ROC 曲线下面积（AUC）
    ''' </summary>
    ''' <param name="labels">真实标签数组，元素取值为 0 或者 1</param>
    ''' <param name="scores">模型输出的预测分数（概率）</param>
    ''' <returns>位于 [0, 1] 区间内的 AUC 值；若样本中只存在单一类别则返回 0.5</returns>
    ''' <remarks>
    ''' 采用 Mann-Whitney U 统计量的等价形式：
    ''' 先把所有样本按照分数升序排序（并列值取平均秩），
    ''' 之后 <c>AUC = (R_pos - n_pos(n_pos + 1) / 2) / (n_pos · n_neg)</c>。
    ''' </remarks>
    Public Function AUC(labels As Double(), scores As Double()) As Double
        Dim n As Integer = labels.Length
        Dim nPos As Integer = 0
        Dim nNeg As Integer = 0

        For i As Integer = 0 To n - 1
            If labels(i) > 0.5 Then
                nPos += 1
            Else
                nNeg += 1
            End If
        Next

        If nPos = 0 OrElse nNeg = 0 Then
            Return 0.5
        End If

        Dim order As Integer() = New Integer(n - 1) {}

        For i As Integer = 0 To n - 1
            order(i) = i
        Next

        Array.Sort(order, Function(a As Integer, b As Integer) scores(a).CompareTo(scores(b)))

        Dim rank As Double() = New Double(n - 1) {}
        Dim i2 As Integer = 0

        While i2 < n
            Dim j As Integer = i2

            While j + 1 < n AndAlso scores(order(j + 1)) = scores(order(i2))
                j += 1
            End While

            Dim avgRank As Double = (i2 + j + 2) / 2.0

            For k As Integer = i2 To j
                rank(order(k)) = avgRank
            Next

            i2 = j + 1
        End While

        Dim rankSum As Double = 0.0

        For i As Integer = 0 To n - 1
            If labels(i) > 0.5 Then
                rankSum += rank(i)
            End If
        Next

        Dim area As Double = (rankSum - nPos * (nPos + 1) / 2.0) / (nPos * nNeg)

        Return area
    End Function

    ''' <summary>
    ''' 计算 PR 曲线下面积（AUPRC，即平均精度 Average Precision）
    ''' </summary>
    ''' <param name="labels">真实标签数组，元素取值为 0 或者 1</param>
    ''' <param name="scores">模型输出的预测分数（概率）</param>
    ''' <returns>位于 [0, 1] 区间内的 AUPRC 值</returns>
    ''' <remarks>
    ''' 计算方式为把样本按照预测分数降序排列，之后逐个把样本纳入预测为正例的集合，
    ''' 累加 <c>(recall_n - recall_(n-1)) · precision_n</c>。
    ''' </remarks>
    Public Function AUPRC(labels As Double(), scores As Double()) As Double
        Dim n As Integer = labels.Length
        Dim nPos As Integer = 0

        For i As Integer = 0 To n - 1
            If labels(i) > 0.5 Then
                nPos += 1
            End If
        Next

        If nPos = 0 Then
            Return 0.0
        End If

        Dim order As Integer() = New Integer(n - 1) {}

        For i As Integer = 0 To n - 1
            order(i) = i
        Next

        Array.Sort(order, Function(a As Integer, b As Integer) scores(b).CompareTo(scores(a)))

        Dim tp As Integer = 0
        Dim fp As Integer = 0
        Dim prevRecall As Double = 0.0
        Dim ap As Double = 0.0

        For i As Integer = 0 To n - 1
            If labels(order(i)) > 0.5 Then
                tp += 1
            Else
                fp += 1
            End If

            Dim precision As Double = tp / (tp + fp)
            Dim recall As Double = tp / nPos

            ap += (recall - prevRecall) * precision
            prevRecall = recall
        Next

        Return ap
    End Function

    ''' <summary>
    ''' 计算给定阈值下的混淆矩阵
    ''' </summary>
    ''' <param name="labels">真实标签数组</param>
    ''' <param name="scores">模型输出的预测分数</param>
    ''' <param name="threshold">判定为正例的阈值</param>
    ''' <returns>
    ''' 长度为 4 的整数数组，依次为 TP、FP、TN、FN
    ''' </returns>
    Public Function ConfusionMatrix(labels As Double(), scores As Double(), Optional threshold As Double = 0.5) As Integer()
        Dim tp As Integer = 0
        Dim fp As Integer = 0
        Dim tn As Integer = 0
        Dim fn As Integer = 0

        For i As Integer = 0 To labels.Length - 1
            Dim predicted As Boolean = scores(i) >= threshold
            Dim actual As Boolean = labels(i) > 0.5

            If predicted AndAlso actual Then
                tp += 1
            ElseIf predicted AndAlso Not actual Then
                fp += 1
            ElseIf Not predicted AndAlso Not actual Then
                tn += 1
            Else
                fn += 1
            End If
        Next

        Return New Integer() {tp, fp, tn, fn}
    End Function

    ''' <summary>
    ''' 计算准确率
    ''' </summary>
    ''' <param name="labels">真实标签数组</param>
    ''' <param name="scores">模型输出的预测分数</param>
    ''' <param name="threshold">判定为正例的阈值</param>
    ''' <returns>准确率</returns>
    Public Function Accuracy(labels As Double(), scores As Double(), Optional threshold As Double = 0.5) As Double
        Dim cm As Integer() = ConfusionMatrix(labels, scores, threshold)
        Dim total As Integer = cm(0) + cm(1) + cm(2) + cm(3)

        If total = 0 Then
            Return 0.0
        End If

        Return (cm(0) + cm(2)) / total
    End Function

    ''' <summary>
    ''' 计算精确率（查准率）
    ''' </summary>
    ''' <param name="labels">真实标签数组</param>
    ''' <param name="scores">模型输出的预测分数</param>
    ''' <param name="threshold">判定为正例的阈值</param>
    ''' <returns>精确率，分母为 0 时返回 0</returns>
    Public Function Precision(labels As Double(), scores As Double(), Optional threshold As Double = 0.5) As Double
        Dim cm As Integer() = ConfusionMatrix(labels, scores, threshold)

        If cm(0) + cm(1) = 0 Then
            Return 0.0
        End If

        Return cm(0) / (cm(0) + cm(1))
    End Function

    ''' <summary>
    ''' 计算召回率（查全率、真阳性率）
    ''' </summary>
    ''' <param name="labels">真实标签数组</param>
    ''' <param name="scores">模型输出的预测分数</param>
    ''' <param name="threshold">判定为正例的阈值</param>
    ''' <returns>召回率，分母为 0 时返回 0</returns>
    Public Function Recall(labels As Double(), scores As Double(), Optional threshold As Double = 0.5) As Double
        Dim cm As Integer() = ConfusionMatrix(labels, scores, threshold)

        If cm(0) + cm(3) = 0 Then
            Return 0.0
        End If

        Return cm(0) / (cm(0) + cm(3))
    End Function

    ''' <summary>
    ''' 计算 F1 分数（精确率与召回率的调和平均）
    ''' </summary>
    ''' <param name="labels">真实标签数组</param>
    ''' <param name="scores">模型输出的预测分数</param>
    ''' <param name="threshold">判定为正例的阈值</param>
    ''' <returns>F1 分数，分母为 0 时返回 0</returns>
    Public Function F1(labels As Double(), scores As Double(), Optional threshold As Double = 0.5) As Double
        Dim p As Double = Precision(labels, scores, threshold)
        Dim r As Double = Recall(labels, scores, threshold)

        If p + r = 0.0 Then
            Return 0.0
        End If

        Return 2.0 * p * r / (p + r)
    End Function

    ''' <summary>
    ''' 一次性计算全部评估指标
    ''' </summary>
    ''' <param name="labels">真实标签数组</param>
    ''' <param name="scores">模型输出的预测分数</param>
    ''' <param name="threshold">判定为正例的阈值</param>
    ''' <returns>评估结果对象</returns>
    Public Function Evaluate(labels As Double(), scores As Double(), Optional threshold As Double = 0.5) As EvaluationResult
        Dim cm As Integer() = ConfusionMatrix(labels, scores, threshold)

        Return New EvaluationResult With {
            .AUC = AUC(labels, scores),
            .AUPRC = AUPRC(labels, scores),
            .Accuracy = Accuracy(labels, scores, threshold),
            .Precision = Precision(labels, scores, threshold),
            .Recall = Recall(labels, scores, threshold),
            .F1 = F1(labels, scores, threshold),
            .TP = cm(0),
            .FP = cm(1),
            .TN = cm(2),
            .FN = cm(3)
        }
    End Function

End Module

''' <summary>
''' 一次评估所产生的全部指标结果
''' </summary>
Public Class EvaluationResult

    ''' <summary>
    ''' ROC 曲线下面积
    ''' </summary>
    ''' <returns>AUC 值</returns>
    Public Property AUC As Double

    ''' <summary>
    ''' PR 曲线下面积
    ''' </summary>
    ''' <returns>AUPRC 值</returns>
    Public Property AUPRC As Double

    ''' <summary>
    ''' 准确率
    ''' </summary>
    ''' <returns>准确率</returns>
    Public Property Accuracy As Double

    ''' <summary>
    ''' 精确率
    ''' </summary>
    ''' <returns>精确率</returns>
    Public Property Precision As Double

    ''' <summary>
    ''' 召回率
    ''' </summary>
    ''' <returns>召回率</returns>
    Public Property Recall As Double

    ''' <summary>
    ''' F1 分数
    ''' </summary>
    ''' <returns>F1 分数</returns>
    Public Property F1 As Double

    ''' <summary>
    ''' 真阳性数量
    ''' </summary>
    ''' <returns>TP</returns>
    Public Property TP As Integer

    ''' <summary>
    ''' 假阳性数量
    ''' </summary>
    ''' <returns>FP</returns>
    Public Property FP As Integer

    ''' <summary>
    ''' 真阴性数量
    ''' </summary>
    ''' <returns>TN</returns>
    Public Property TN As Integer

    ''' <summary>
    ''' 假阴性数量
    ''' </summary>
    ''' <returns>FN</returns>
    Public Property FN As Integer

    ''' <summary>
    ''' 生成评估结果的文字描述
    ''' </summary>
    ''' <returns>单行指标文本</returns>
    Public Overrides Function ToString() As String
        Return $"AUC={AUC.ToString("F4")}, AUPRC={AUPRC.ToString("F4")}, accuracy={Accuracy.ToString("F4")}, " &
               $"F1={F1.ToString("F4")}, precision={Precision.ToString("F4")}, recall={Recall.ToString("F4")}"
    End Function

End Class
