' ============================================================================
' ModelEvaluation.vb
'
' 模型评估：基于底层 LibSVM 的 Prediction.Predict 计算表型模型在训练集
' 之上的表现。分类模型给出准确率，回归模型给出 MSE / RMSE / 相关系数。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 单个表型模型的评估结果
    ''' </summary>
    Public Class TraitEvaluation

        ''' <summary>表型名称</summary>
        Public Property trait_name As String
        ''' <summary>表型的数据类型</summary>
        Public Property data_type As String
        ''' <summary>参与评估的样本数量</summary>
        Public Property sampleCount As Integer
        ''' <summary>分类准确率（回归模型为 0）</summary>
        Public Property accuracy As Double
        ''' <summary>预测正确的样本数</summary>
        Public Property correct As Integer
        ''' <summary>回归模型的均方误差</summary>
        Public Property mse As Double
        ''' <summary>回归模型的均方根误差</summary>
        Public Property rmse As Double
        ''' <summary>回归模型预测值与真实值的相关系数</summary>
        Public Property correlation As Double
        ''' <summary>交叉验证得分</summary>
        Public Property cvScore As Double

        Public Overrides Function ToString() As String
            If accuracy > 0 Then
                Return $"{trait_name}: accuracy={accuracy:F4} ({correct}/{sampleCount})"
            Else
                Return $"{trait_name}: rmse={rmse:F4}, r={correlation:F4}"
            End If
        End Function

    End Class

    ''' <summary>
    ''' 表型模型评估工具
    ''' </summary>
    Public Module ModelEvaluation

        ''' <summary>
        ''' 评估单个表型模型在给定训练数据集之上的表现
        ''' </summary>
        Public Function Evaluate(model As Models.PhenotypeModel, dataset As TraitTrainingSet) As TraitEvaluation
            Dim result As New TraitEvaluation With {
                .trait_name = model.Trait.trait_name,
                .data_type = model.Trait.data_type,
                .cvScore = model.CVScore
            }

            If Not model.IsTrained() Then
                Return result
            End If

            Dim topic As String = model.Trait.trait_name
            Dim rows As SupportVector() = dataset.problems _
                .vectors _
                .Where(Function(v) v.labels.ContainsKey(topic)) _
                .ToArray

            result.sampleCount = rows.Length

            Dim correct As Integer = 0
            Dim expectedValues As New List(Of Double)
            Dim predictedValues As New List(Of Double)

            For Each row As SupportVector In rows
                Dim nodes As Node() = dataset.embedding.ToNodes(row.Properties)
                Dim pred As TraitPrediction = model.Predict(nodes)

                If model.IsRegression() Then
                    Dim expected As Double = Double.Parse(row.labels(topic))

                    expectedValues.Add(expected)
                    predictedValues.Add(pred.value)
                ElseIf pred.predict IsNot Nothing AndAlso
                    pred.predict.Equals(row.labels(topic), StringComparison.OrdinalIgnoreCase) Then

                    correct += 1
                End If
            Next

            If model.IsRegression() Then
                Dim n As Integer = expectedValues.Count
                Dim sumSq As Double = 0

                For i As Integer = 0 To n - 1
                    Dim diff As Double = predictedValues(i) - expectedValues(i)
                    sumSq += diff * diff
                Next

                result.mse = sumSq / n
                result.rmse = System.Math.Sqrt(result.mse)
                result.correlation = Utils.MathUtils.PearsonCorrelation(expectedValues.ToArray, predictedValues.ToArray)
            Else
                result.correct = correct
                result.accuracy = correct / rows.Length
            End If

            Return result
        End Function

        ''' <summary>
        ''' 批量评估模型仓库之中的全部已训练模型
        ''' </summary>
        Public Iterator Function EvaluateAll(loader As ModelLoader, dataset As TraitTrainingSet) As IEnumerable(Of TraitEvaluation)
            For Each model As Models.PhenotypeModel In loader.GetTrainedModels()
                Yield Evaluate(model, dataset)
            Next
        End Function

    End Module
End Namespace
