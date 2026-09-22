' ============================================================================
' LinkPredictionMetrics.vb — 调控关系合理性评估（readme 五.2）
'
' readme 五.2："学习到的权重 learned_W 与已知 TF-Target 集合对比 → AUROC, AUPRC"。
'
' 把"权重恢复质量"形式化为链路预测问题：
'   正例（positive）：先验网络中的 <b>已知 TF-Target 调控关系</b>（PriorGraph.Skeleton）
'   负例（negative）：既不是已知调控关系、也没有 WGCNA 共表达支持的基因对
'   打分（score）  ：|learned_W[i][j]| —— 模型把该突触调得越强，越"相信"这条边存在
'
' 为什么把 WGCNA 补充边排除在负例之外：
'   共表达关联是"未经证实但可能为真"的弱证据。把它们当作负例会惩罚模型对
'   真实（但尚未被文献确认）关系的正确预测，从而系统性低估性能。
'   排除后正/负例集合都更"干净"，AUROC 才能反映权重对已知调控知识的恢复程度。
'
' 指标含义：
'   AUROC  随机取一条正例与一条负例，正例得分更高的概率（0.5 = 无信息）
'   AUPRC  平均精度（在正例稀疏时比 AUROC 更敏感，是小规模调控网络的合适指标）
'   TopK   得分最高的 K 条突触中有多少条命中已知调控关系（precision / recall）
' ============================================================================

Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.SpikingLoop.Graph
Imports std = System.Math

Namespace Evaluation

    ''' <summary>权重恢复质量报告</summary>
    Public Class LinkPredictionReport

        ''' <summary>ROC 曲线下面积</summary>
        Public Property Auroc As Double

        ''' <summary>PR 曲线下面积（平均精度）</summary>
        Public Property Auprc As Double

        ''' <summary>正例数（已知 TF-Target 边）</summary>
        Public Property NumPositives As Integer

        ''' <summary>负例数（无任何先验支持的基因对）</summary>
        Public Property NumNegatives As Integer

        ''' <summary>Top-K 的 K</summary>
        Public Property TopK As Integer

        ''' <summary>Top-K 中命中已知调控关系的数量</summary>
        Public Property TopKHits As Integer

        ''' <summary>Top-K 精确率</summary>
        Public Property TopKPrecision As Double

        ''' <summary>Top-K 召回率</summary>
        Public Property TopKRecall As Double

        ''' <summary>训练前先验权重的 L1 和</summary>
        Public Property PriorL1 As Double

        ''' <summary>训练后学习权重的 L1 和（与 PriorL1 对比可观察权重的整体缩放）</summary>
        Public Property LearnedL1 As Double

        ''' <summary>
        ''' 是否处于"硬结构掩码"约束下（先验之外的突触被冻结为 0）。
        ''' 此时负例得分恒为 0、正例得分非零，AUROC/AUPRC 会结构性饱和到 1.0，
        ''' 该数值本身不含信息。应改看 Top-K 精确率与逐边权重漂移
        ''' （<c>weight_comparison</c> 导出表），或在消融实验中关闭掩码后再比较 AUROC。
        ''' </summary>
        Public Property MaskConstrained As Boolean

        Public Overrides Function ToString() As String
            Dim note = ""
            If MaskConstrained AndAlso Not Double.IsNaN(Auroc) AndAlso Auroc >= 0.9999 Then
                note = "  [提示] 结构掩码冻结了先验之外的突触，AUROC/AUPRC 结构性饱和，请改看 Top-K 与权重漂移"
            End If

            Return $"AUROC={Auroc:F4}, AUPRC={Auprc:F4}, " &
                   $"Top{TopK}={TopKHits}/{TopK} (P={TopKPrecision:F3}, R={TopKRecall:F3}), " &
                   $"pos={NumPositives}, neg={NumNegatives}, L1 {PriorL1:F3}→{LearnedL1:F3}{note}"
        End Function

    End Class

    ''' <summary>调控关系（链路预测）评估</summary>
    Public Module LinkPredictionMetrics

        ''' <summary>
        ''' 用学习到的突触权重对"已知 TF-Target 集合"做链路预测评估。
        ''' </summary>
        ''' <param name="graph">先验图（提供骨架掩码、邻接与先验权重）</param>
        ''' <param name="learnedWeight">训练后的权重矩阵 [N, N]</param>
        ''' <param name="topK">Top-K 突触的 K 值</param>
        Public Function Evaluate(graph As PriorGraph, learnedWeight As Tensor,
                                 Optional topK As Integer = 50) As LinkPredictionReport

            If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))
            If learnedWeight Is Nothing Then Throw New ArgumentNullException(NameOf(learnedWeight))

            Dim n = graph.Units
            If learnedWeight.Length <> n * n Then
                Throw New ArgumentException(
                    $"权重矩阵大小({learnedWeight.Length})与基因数({n})不匹配", NameOf(learnedWeight))
            End If
            If graph.Skeleton Is Nothing OrElse graph.Adjacency Is Nothing Then
                Throw new InvalidOperationException("先验图缺少骨架掩码或邻接矩阵，无法进行链路预测评估")
            End If

            Dim w = learnedWeight.Data
            Dim sk = graph.Skeleton.Data
            Dim ad = graph.Adjacency.Data

            Dim labeled As New List(Of (score As Double, label As Boolean))()
            Dim allPairs As New List(Of (score As Double, positive As Boolean))()
            Dim posCount = 0
            Dim negCount = 0

            For i = 0 To n - 1
                Dim off = i * n
                For j = 0 To n - 1
                    If i = j Then Continue For   ' 自环不参与链路预测

                    Dim score = std.Abs(w(off + j))
                    Dim isKnown = sk(off + j) <> 0.0

                    allPairs.Add((score, isKnown))

                    If isKnown Then
                        labeled.Add((score, True))
                        posCount += 1
                    ElseIf ad(off + j) = 0.0 Then
                        labeled.Add((score, False))
                        negCount += 1
                    End If
                    ' 其余为 WGCNA 补充边：既非正例也不作负例（见文件头说明）
                Next
            Next

            Dim report As New LinkPredictionReport With {
                .NumPositives = posCount,
                .NumNegatives = negCount,
                .TopK = topK,
                .PriorL1 = graph.WeightL1,
                .LearnedL1 = RegressionLosses.L1Sum(learnedWeight),
                .MaskConstrained = graph.NumFrozenSynapses > 0
            }

            If posCount > 0 AndAlso negCount > 0 Then
                labeled.Sort(Function(a, b) b.score.CompareTo(a.score))

                Dim auc = 0.0
                Dim ap = 0.0
                Dim tp = 0
                Dim fp = 0
                Dim prevTp = 0
                Dim prevFp = 0
                Dim prevRecall = 0.0
                Dim k = 0

                While k < labeled.Count
                    Dim s = labeled(k).score

                    ' 同一得分的样本必须整组处理，否则并列值会让 AUC 依赖排序细节
                    While k < labeled.Count AndAlso labeled(k).score = s
                        If labeled(k).label Then tp += 1 Else fp += 1
                        k += 1
                    End While

                    auc += (fp - prevFp) * (tp + prevTp) / 2.0

                    Dim precision = tp / CDbl(tp + fp)
                    Dim recall = tp / CDbl(posCount)
                    ap += (recall - prevRecall) * precision

                    prevTp = tp
                    prevFp = fp
                    prevRecall = recall
                End While

                report.Auroc = auc / (posCount * CDbl(negCount))
                report.Auprc = ap
            Else
                report.Auroc = Double.NaN
                report.Auprc = Double.NaN
            End If

            ' ---- Top-K：全局得分最高的 K 条突触中有多少是已知调控关系 ----
            If topK > 0 AndAlso posCount > 0 Then
                allPairs.Sort(Function(a, b) b.score.CompareTo(a.score))
                Dim take = std.Min(topK, allPairs.Count)
                Dim hits = 0
                For k = 0 To take - 1
                    If allPairs(k).positive Then hits += 1
                Next

                report.TopKHits = hits
                report.TopKPrecision = hits / CDbl(take)
                report.TopKRecall = hits / CDbl(posCount)
            End If

            Return report
        End Function

    End Module

End Namespace
