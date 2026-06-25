' ============================================================================
' EnsembleVoting.vb - 模块6：集成投票与预测模块
'
' 论文对应：
'   "集成投票与预测模块"
'
' 核心功能：
'   1. 集成学习：选取表现最好的5个SVM模型
'   2. 多数表决算法：5个模型中至少有3个预测为正，则最终判定为表型存在
'
' 算法原理：
'   - 投票委员会机制：选出交叉验证中准确率最高的5个SVM模型
'   - 多数表决：5个模型中至少3个预测为正，则最终判定为表型存在
' ============================================================================

Namespace TraitarVB.Modules

    ''' <summary>
    ''' 模块6：集成投票与预测模块
    ''' 综合多个模型的预测结果，对新样本进行最终的表型判定
    ''' </summary>
    Public Class EnsembleVoting

        ' 投票委员会大小（论文：5个最佳SVM模型）
        Public Const COMMITTEE_SIZE As Integer = 5

        ' 多数表决阈值（5个中至少3个）
        Public Const MAJORITY_THRESHOLD As Integer = 3

        Private _svm As New SVMClassifier()

        ''' <summary>
        ''' 投票结果
        ''' </summary>
        Public Class VotingResult

            Public Property PhenotypeId As String

            ''' <summary>最终预测标签（1=存在，0=不存在）</summary>
            Public Property FinalLabel As Integer

            ''' <summary>正票数</summary>
            Public Property PositiveVotes As Integer

            ''' <summary>负票数</summary>
            Public Property NegativeVotes As Integer

            ''' <summary>总票数</summary>
            Public Property TotalVotes As Integer

            ''' <summary>置信度（正票比例）</summary>
            Public Property Confidence As Double

            ''' <summary>各模型的预测得分</summary>
            Public Property ModelScores As New List(Of Double)()

            ''' <summary>各模型的预测标签</summary>
            Public Property ModelLabels As New List(Of Integer)()

            ''' <summary>是否为正预测</summary>
            Public ReadOnly Property IsPositive As Boolean
                Get
                    Return FinalLabel = 1
                End Get
            End Property
        End Class

        ''' <summary>
        ''' 从一组模型中选择表现最好的N个模型组成投票委员会
        ''' 论文：选出交叉验证中准确率最高的5个SVM模型
        ''' </summary>
        ''' <param name="models">所有候选模型</param>
        ''' <param name="accuracies">各模型在交叉验证中的准确率</param>
        ''' <param name="committeeSize">委员会大小（默认5）</param>
        ''' <returns>投票委员会模型列表</returns>
        Public Function SelectVotingCommittee(models As List(Of SVMClassifier.SVMModel),
                                              accuracies As List(Of Double),
                                              Optional committeeSize As Integer = COMMITTEE_SIZE) As List(Of SVMClassifier.SVMModel)

            If models.Count <= committeeSize Then
                Return New List(Of SVMClassifier.SVMModel)(models)
            End If

            ' 按准确率降序排序
            Dim indexed As New List(Of (Integer, Double))()
            For i As Integer = 0 To models.Count - 1
                indexed.Add((i, accuracies(i)))
            Next

            indexed.Sort(Function(a, b) b.Item2.CompareTo(a.Item2))

            Dim committee As New List(Of SVMClassifier.SVMModel)()
            For i As Integer = 0 To Math.Min(committeeSize, models.Count) - 1
                committee.Add(models(indexed(i).Item1))
            Next

            Console.WriteLine("[模块6] 投票委员会选择完成: {0}个模型", committee.Count)
            For i As Integer = 0 To committee.Count - 1
                Console.WriteLine("       模型 {0}: C={1}, 准确率={2:F4}",
                                  i + 1, committee(i).C, indexed(i).Item2)
            Next

            Return committee
        End Function

        ''' <summary>
        ''' 使用投票委员会对单个样本进行预测
        ''' 论文：5个模型中至少有3个预测为正，则最终判定为表型存在
        ''' </summary>
        ''' <param name="committee">投票委员会模型列表</param>
        ''' <param name="features">样本特征</param>
        ''' <returns>投票结果</returns>
        Public Function PredictWithCommittee(committee As List(Of SVMClassifier.SVMModel),
                                             features As Dictionary(Of String, Integer)) As VotingResult

            Dim result As New VotingResult()
            result.TotalVotes = committee.Count

            Dim positiveVotes As Integer = 0
            Dim negativeVotes As Integer = 0

            For Each model As SVMClassifier.SVMModel In committee
                Dim score As Double = _svm.PredictScore(model, features)
                Dim label As Integer = If(score > 0, 1, 0)

                result.ModelScores.Add(score)
                result.ModelLabels.Add(label)

                If label = 1 Then
                    positiveVotes += 1
                Else
                    negativeVotes += 1
                End If
            Next

            result.PositiveVotes = positiveVotes
            result.NegativeVotes = negativeVotes
            result.Confidence = CDbl(positiveVotes) / CDbl(committee.Count)

            ' 多数表决：正票数 >= 阈值则判定为正
            If positiveVotes >= MAJORITY_THRESHOLD Then
                result.FinalLabel = 1
            Else
                result.FinalLabel = 0
            End If

            Return result
        End Function

        ''' <summary>
        ''' 使用所有模型进行投票（当没有准确率信息时）
        ''' 论文模型文件包含13个不同C值的模型
        ''' 使用所有活跃模型（有非零偏置或非零权重）投票，多数表决
        ''' </summary>
        ''' <param name="allModels">所有模型</param>
        ''' <param name="features">样本特征</param>
        ''' <returns>投票结果</returns>
        Public Function PredictWithAllModels(allModels As SVMClassifier.SVMModel(),
                                             features As Dictionary(Of String, Integer)) As VotingResult

            ' 筛选活跃模型（有非零偏置或非零权重）
            Dim activeModels As New List(Of SVMClassifier.SVMModel)()
            For Each model As SVMClassifier.SVMModel In allModels
                Dim hasNonZeroBias As Boolean = Math.Abs(model.Bias) > 0.000000000001
                Dim hasNonZeroWeights As Boolean = False
                If model.Weights IsNot Nothing Then
                    For Each w As Double In model.Weights
                        If Math.Abs(w) > 0.000000000001 Then
                            hasNonZeroWeights = True
                            Exit For
                        End If
                    Next
                End If
                If hasNonZeroBias OrElse hasNonZeroWeights Then
                    activeModels.Add(model)
                End If
            Next

            ' 如果没有活跃模型，使用全部模型
            If activeModels.Count = 0 Then
                activeModels = New List(Of SVMClassifier.SVMModel)(allModels)
            End If

            Dim result As New VotingResult()
            result.TotalVotes = activeModels.Count

            Dim positiveVotes As Integer = 0
            Dim negativeVotes As Integer = 0

            For Each model As SVMClassifier.SVMModel In activeModels
                Dim score As Double = _svm.PredictScore(model, features)
                Dim label As Integer = If(score > 0, 1, 0)

                result.ModelScores.Add(score)
                result.ModelLabels.Add(label)

                If label = 1 Then
                    positiveVotes += 1
                Else
                    negativeVotes += 1
                End If
            Next

            result.PositiveVotes = positiveVotes
            result.NegativeVotes = negativeVotes
            result.Confidence = CDbl(positiveVotes) / CDbl(activeModels.Count)

            ' 多数表决：超过半数即为阳性
            Dim threshold As Integer = activeModels.Count \ 2 + 1
            If positiveVotes >= threshold Then
                result.FinalLabel = 1
            Else
                result.FinalLabel = 0
            End If

            Return result
        End Function

        ''' <summary>
        ''' 批量预测多个表型
        ''' </summary>
        ''' <param name="phenotypeModels">表型ID -> 模型列表</param>
        ''' <param name="features">样本特征</param>
        ''' <returns>表型ID -> 投票结果</returns>
        Public Iterator Function PredictAllPhenotypes(
            phenotypeModels As Dictionary(Of String, SVMClassifier.SVMModel()),
            features As Dictionary(Of String, Integer)) As IEnumerable(Of VotingResult)

            Console.WriteLine("[模块6] 开始预测所有表型...")
            Dim positiveCount As Integer = 0

            For Each kvp As KeyValuePair(Of String, SVMClassifier.SVMModel()) In phenotypeModels
                Dim phenoId As String = kvp.Key
                Dim models As SVMClassifier.SVMModel() = kvp.Value

                ' 使用所有模型投票
                Dim result As VotingResult = PredictWithAllModels(models, features)
                result.PhenotypeId = phenoId

                If result.IsPositive Then positiveCount += 1

                Yield result
            Next

            Console.WriteLine("[模块6] 预测完成: {0}/{1} 个表型预测为阳性",
                              positiveCount, phenotypeModels.Count)
        End Function

        ''' <summary>
        ''' 获取预测结果的详细报告
        ''' </summary>
        Public Function GetDetailedReport(phenoId As String,
                                          phenoName As String,
                                          phenoCategory As String,
                                          result As VotingResult) As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine(String.Format("表型ID: {0}", phenoId))
            sb.AppendLine(String.Format("表型名称: {0}", phenoName))
            sb.AppendLine(String.Format("表型类别: {0}", phenoCategory))
            sb.AppendLine(String.Format("预测结果: {0}", If(result.IsPositive, "存在(POSITIVE)", "不存在(NEGATIVE)")))
            sb.AppendLine(String.Format("投票统计: 正票={0}, 负票={1}, 总票数={2}",
                                        result.PositiveVotes, result.NegativeVotes, result.TotalVotes))
            sb.AppendLine(String.Format("置信度: {0:F2}", result.Confidence))
            sb.AppendLine("各模型得分:")
            For i As Integer = 0 To result.ModelScores.Count - 1
                sb.AppendLine(String.Format("  模型 {0}: score={1:F4}, label={2}",
                                            i + 1, result.ModelScores(i), result.ModelLabels(i)))
            Next
            Return sb.ToString()
        End Function

    End Class

End Namespace
