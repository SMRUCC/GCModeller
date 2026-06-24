' ============================================================================
' Module 3: Phenotype Predictor with Voting Committee
' File: Prediction/PhenotypePredictor.vb
'
' 功能: 基于加载的 Traitar 模型，对新基因组的 Pfam 特征向量进行表型预测。
'       对应论文中 "投票委员会机制" 和 "phypat+PGL 分类器"。
'
' 算法原理:
'   1. 对每个表型，从 13 个 C 值的 SVM 模型中选出准确率最高的 5 个（投票委员会）
'   2. 对每个模型计算: score = Σ(weight_pfam × x_pfam) + bias
'   3. 若 score > 0，该模型预测表型存在(1)，否则不存在(0)
'   4. 投票: 若 ≥3 个模型预测存在，则最终判定表型存在
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports Traitar.Models
Imports Traitar.GenomeAnnotation

Namespace Traitar.Prediction

    ''' <summary>
    ''' 单个模型的预测结果
    ''' </summary>
    Public Class ModelPrediction
        ''' <summary>模型ID（如 "0.5_0"）</summary>
        Public Property ModelId As String
        ''' <summary>正则化参数 C</summary>
        Public Property C As Double
        ''' <summary>原始得分 score = Σ(w·x) + b</summary>
        Public Property Score As Double
        ''' <summary>偏置项</summary>
        Public Property Bias As Double
        ''' <summary>该模型的预测（1=存在, 0=不存在）</summary>
        Public Property Prediction As Integer

        Public Overrides Function ToString() As String
            Return $"{ModelId}: score={Score:F4}, pred={Prediction}"
        End Function
    End Class

    ''' <summary>
    ''' 一个表型的完整预测结果
    ''' </summary>
    Public Class PhenotypePredictionResult
        ''' <summary>表型ID</summary>
        Public Property PhenotypeId As String
        ''' <summary>表型名称</summary>
        Public Property PhenotypeName As String
        ''' <summary>表型类别</summary>
        Public Property Category As String
        ''' <summary>各模型的预测详情</summary>
        Public Property ModelPredictions As New List(Of ModelPrediction)
        ''' <summary>投票委员会选中的模型ID列表</summary>
        Public Property CommitteeModelIds As New List(Of String)
        ''' <summary>投"存在"票数</summary>
        Public Property PositiveVotes As Integer
        ''' <summary>投"不存在"票数</summary>
        Public Property NegativeVotes As Integer
        ''' <summary>最终预测（1=存在, 0=不存在）</summary>
        Public Property FinalPrediction As Integer
        ''' <summary>平均得分</summary>
        Public Property AverageScore As Double

        Public Overrides Function ToString() As String
            Dim predStr = If(FinalPrediction = 1, "PRESENT", "ABSENT")
            Return $"[{PhenotypeId}] {PhenotypeName}: {predStr} (votes: {PositiveVotes}+/{NegativeVotes}-, avg_score={AverageScore:F4})"
        End Function
    End Class

    ''' <summary>
    ''' 表型预测器（使用投票委员会机制）
    ''' </summary>
    Public Class PhenotypePredictor

        ''' <summary>投票委员会大小（论文中为 5）</summary>
        Public Property CommitteeSize As Integer = 5

        ''' <summary>多数表决阈值（≥3 票即判定存在）</summary>
        Public Property MajorityThreshold As Integer = 3

        ''' <summary>
        ''' 预测单个表型
        ''' </summary>
        ''' <param name="phenotypeModel">表型模型</param>
        ''' <param name="profile">基因组特征向量</param>
        Public Function Predict(phenotypeModel As PhenotypeModel,
                                profile As PhyleticProfile) As PhenotypePredictionResult
            Dim result As New PhenotypePredictionResult With {
                .PhenotypeId = phenotypeModel.PhenotypeId,
                .PhenotypeName = If(phenotypeModel.Info IsNot Nothing, phenotypeModel.Info.Accession, ""),
                .Category = If(phenotypeModel.Info IsNot Nothing, phenotypeModel.Info.Category, "")
            }

            ' 1. 对每个模型计算预测得分
            Dim allPredictions As New List(Of ModelPrediction)
            For Each kv In phenotypeModel.Models
                Dim svmModel = kv.Value
                Dim score = ComputeScore(svmModel, profile)
                Dim pred = If(score > 0, 1, 0)

                allPredictions.Add(New ModelPrediction With {
                    .ModelId = svmModel.ModelId,
                    .C = svmModel.C,
                    .Score = score,
                    .Bias = svmModel.Bias,
                    .Prediction = pred
                })
            Next

            ' 2. 选出投票委员会（论文中选准确率最高的5个）
            '    由于模型文件中未包含准确率信息，这里采用启发式策略:
            '    优先选择偏置非零的模型（表示该模型有区分能力）
            '    若不足，则选择权重非零特征数最多的模型
            Dim committee = SelectCommittee(allPredictions, phenotypeModel)
            result.CommitteeModelIds = committee.Select(Function(p) p.ModelId).ToList()

            ' 3. 投票
            result.PositiveVotes = committee.Count(Function(p) p.Prediction = 1)
            result.NegativeVotes = committee.Count(Function(p) p.Prediction = 0)

            ' 4. 最终预测: 多数表决
            result.FinalPrediction = If(result.PositiveVotes >= MajorityThreshold, 1, 0)

            ' 5. 平均得分
            If committee.Count > 0 Then
                result.AverageScore = committee.Average(Function(p) p.Score)
            End If

            ' 保存所有模型预测（用于调试）
            result.ModelPredictions = allPredictions

            Return result
        End Function

        ''' <summary>
        ''' 选出投票委员会
        ''' 启发式策略:
        '''   1. 优先选择偏置非零的模型
        '''   2. 若超过 CommitteeSize，则按 |bias| 降序取前 N 个
        '''   3. 若不足，则从偏置为零的模型中按非零权重数补充
        ''' </summary>
        Private Function SelectCommittee(allPredictions As List(Of ModelPrediction),
                                         phenotypeModel As PhenotypeModel) As List(Of ModelPrediction)
            ' 分离偏置非零和偏置为零的模型
            Dim nonZeroBias = allPredictions.Where(Function(p) p.Bias <> 0.0).ToList()
            Dim zeroBias = allPredictions.Where(Function(p) p.Bias = 0.0).ToList()

            ' 按偏置绝对值降序排序
            nonZeroBias.Sort(Function(a, b) Math.Abs(b.Bias).CompareTo(Math.Abs(a.Bias)))

            Dim committee As New List(Of ModelPrediction)
            ' 取偏置非零的前 CommitteeSize 个
            For i = 0 To Math.Min(CommitteeSize - 1, nonZeroBias.Count - 1)
                committee.Add(nonZeroBias(i))
            Next

            ' 若不足，从偏置为零的模型中补充（按非零权重数排序）
            If committee.Count < CommitteeSize AndAlso zeroBias.Count > 0 Then
                Dim zeroBiasWithCounts = zeroBias.Select(Function(p)
                    Dim svm = phenotypeModel.Models(p.ModelId)
                    Dim nzCount = svm.Weights.Values.Count(Function(w) w <> 0.0)
                    Return Tuple.Create(p, nzCount)
                End Function).ToList()
                zeroBiasWithCounts.Sort(Function(a, b) b.Item2.CompareTo(a.Item2))

                For Each t In zeroBiasWithCounts
                    If committee.Count >= CommitteeSize Then Exit For
                    committee.Add(t.Item1)
                Next
            End If

            Return committee
        End Function

        ''' <summary>
        ''' 计算单个 SVM 模型的预测得分
        ''' score = Σ(weight_pfam × x_pfam) + bias
        ''' 其中 x_pfam ∈ {0, 1}（Pfam 是否存在）
        ''' </summary>
        Private Function ComputeScore(svmModel As SvmModel,
                                       profile As PhyleticProfile) As Double
            Dim score = svmModel.Bias

            ' 遍历模型中的所有非零权重特征
            For Each kv In svmModel.Weights
                Dim pfamAcc = kv.Key
                Dim weight = kv.Value
                If weight = 0.0 Then Continue For

                ' 若该 Pfam 在基因组中存在，则加上权重
                If profile.HasPfam(pfamAcc) Then
                    score += weight
                End If
            Next

            Return score
        End Function

        ''' <summary>
        ''' 对多个表型进行预测
        ''' </summary>
        Public Function PredictAll(models As Dictionary(Of String, PhenotypeModel),
                                    profile As PhyleticProfile) As List(Of PhenotypePredictionResult)
            Dim results As New List(Of PhenotypePredictionResult)
            For Each kv In models
                Dim result = Predict(kv.Value, profile)
                results.Add(result)
            Next
            Return results
        End Function

        ''' <summary>
        ''' 仅返回预测为"存在"的表型
        ''' </summary>
        Public Function GetPresentPhenotypes(results As List(Of PhenotypePredictionResult)) _
            As List(Of PhenotypePredictionResult)
            Return results.FindAll(Function(r) r.FinalPrediction = 1)
        End Function
    End Class
End Namespace
