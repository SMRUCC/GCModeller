' ============================================================================
' FeatureSelection.vb
'
' 特征选择：识别出对某一个表型预测贡献最大的 Pfam 结构域。
'
' 重构之后不再依赖线性模型的权重字典（RBF 核没有线性权重），改为：
'   1. 先计算每一个 Pfam 维度与表型标签之间的皮尔逊相关系数进行排序
'      （分类模型对每个类别分别计算 point-biserial 相关系数后取绝对值最大者）
'   2. 再对排名靠前的若干个特征做排列重要性（permutation importance）验证
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 关键 Pfam 结构域特征
    ''' </summary>
    Public Class KeyFeature

        ''' <summary>Pfam 家族编号</summary>
        Public Property PfamId As String
        ''' <summary>Pfam 家族描述</summary>
        Public Property Description As String
        ''' <summary>该特征与表型标签之间的皮尔逊相关系数</summary>
        Public Property PearsonCorrelation As Double
        ''' <summary>排列重要性：打乱该特征之后模型表现的下降量</summary>
        Public Property Importance As Double
        ''' <summary>该特征在目标类别样本之中的均值</summary>
        Public Property MeanInGroup As Double
        ''' <summary>该特征在其它样本之中的均值</summary>
        Public Property MeanOutGroup As Double
        ''' <summary>相关系数所对应的类别（分类模型）</summary>
        Public Property ClassLabel As String

        ''' <summary>该特征是否与表型正相关</summary>
        Public ReadOnly Property IsPositiveCorrelated As Boolean
            Get
                Return PearsonCorrelation > 0
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{PfamId} r={PearsonCorrelation:F4}, importance={Importance:F4}"
        End Function

    End Class

    ''' <summary>
    ''' 关键 Pfam 特征提取
    ''' </summary>
    Public Class FeatureSelection

        ReadOnly loader As ModelLoader

        ''' <summary>
        ''' 特征相关性排名的随机种子，保证排列重要性的结果可复现
        ''' </summary>
        Public Property seed As Integer = 20200423

        Sub New(loader As ModelLoader)
            Me.loader = loader
        End Sub

        ''' <summary>
        ''' 提取指定表型模型的关键 Pfam 特征
        ''' </summary>
        ''' <param name="model">目标表型模型</param>
        ''' <param name="dataset">训练数据集（提供样本用于计算相关性与排列重要性）</param>
        ''' <param name="topN">返回的特征数量上限</param>
        ''' <param name="descriptions">Pfam 编号 -> 家族描述</param>
        ''' <param name="withPermutation">
        ''' 是否对排名靠前的特征额外计算排列重要性（默认开启，
        ''' 只对 topN 个特征计算，开销很小）
        ''' </param>
        Public Function SelectKeyFeatures(model As Models.PhenotypeModel,
                                          dataset As TraitTrainingSet,
                                          Optional topN As Integer = 20,
                                          Optional descriptions As Dictionary(Of String, String) = Nothing,
                                          Optional withPermutation As Boolean = True) As List(Of KeyFeature)

            Dim result As New List(Of KeyFeature)()

            If model Is Nothing OrElse Not model.IsTrained() Then
                Return result
            End If
            If dataset Is Nothing OrElse dataset.problems Is Nothing OrElse dataset.embedding Is Nothing Then
                Return result
            End If

            Dim topic As String = model.Trait.trait_name
            Dim rows As SupportVector() = dataset.problems _
                .vectors _
                .Where(Function(v) v.labels.ContainsKey(topic)) _
                .ToArray

            If rows.Length < 2 Then
                Return result
            End If

            Dim dims As String() = dataset.embedding.dimensionNames
            Dim labels As String() = rows.Select(Function(v) v.labels(topic)).ToArray
            Dim classes As String()

            If model.IsRegression() Then
                classes = {""}
            Else
                classes = labels.Distinct.ToArray
            End If

            ' 每一个维度：取与各个类别之间绝对相关系数最大的那一个
            For i As Integer = 0 To dims.Length - 1
                Dim id As String = dims(i)
                Dim column As Double() = rows _
                    .Select(Function(v) v(id)) _
                    .ToArray

                ' 全零（该 Pfam 在所有样本之中都不存在）的维度没有判别能力
                If column.All(Function(x) x = 0.0) Then
                    Continue For
                End If

                Dim best As Double = 0
                Dim bestClass As String = ""

                For Each cls As String In classes
                    Dim target As Double()

                    If model.IsRegression() Then
                        target = labels.Select(Function(s) Double.Parse(s)).ToArray
                    Else
                        target = labels.Select(Function(s) If(s.Equals(cls, StringComparison.OrdinalIgnoreCase), 1.0, 0.0)).ToArray
                    End If

                    Dim r As Double = Utils.MathUtils.PearsonCorrelation(column, target)

                    If System.Math.Abs(r) > System.Math.Abs(best) Then
                        best = r
                        bestClass = cls
                    End If

                    If model.IsRegression() Then
                        Exit For
                    End If
                Next

                result.Add(New KeyFeature With {
                    .PfamId = id,
                    .PearsonCorrelation = best,
                    .ClassLabel = bestClass,
                    .Description = If(descriptions IsNot Nothing AndAlso descriptions.ContainsKey(id), descriptions(id), Nothing)
                })
            Next

            result.Sort(Function(a, b) -System.Math.Abs(a.PearsonCorrelation).CompareTo(System.Math.Abs(b.PearsonCorrelation)))

            If result.Count > topN Then
                result = result.GetRange(0, topN)
            End If

            If withPermutation Then
                Call FillImportance(model, dataset, rows, result)
            End If

            Return result
        End Function

        ''' <summary>
        ''' 计算排列重要性：把某一个 Pfam 维度在样本之间随机打乱，
        ''' 观察模型表现（分类准确率 / 回归 MSE）的下降量
        ''' </summary>
        Private Sub FillImportance(model As Models.PhenotypeModel,
                                   dataset As TraitTrainingSet,
                                   rows As SupportVector(),
                                   features As List(Of KeyFeature))

            Dim baseline As Double = MeasurePerformance(model, dataset, rows)
            Dim rand As New Random(seed)

            For Each feature As KeyFeature In features
                Dim id As String = feature.PfamId
                Dim raw As Double() = rows.Select(Function(v) v(id)).ToArray
                Dim shuffled As Double() = raw.ToArray

                Call Shuffle(shuffled, rand)

                Dim backup As Double() = raw.ToArray

                For i As Integer = 0 To rows.Length - 1
                    rows(i).Properties(id) = shuffled(i)
                Next

                Dim degraded As Double = MeasurePerformance(model, dataset, rows)

                ' 还原
                For i As Integer = 0 To rows.Length - 1
                    rows(i).Properties(id) = backup(i)
                Next

                feature.Importance = baseline - degraded
                feature.MeanInGroup = raw.Average()
            Next
        End Sub

        ''' <summary>
        ''' 度量模型在给定样本之上的表现：分类返回准确率，回归返回 -MSE
        ''' </summary>
        Private Function MeasurePerformance(model As Models.PhenotypeModel,
                                            dataset As TraitTrainingSet,
                                            rows As SupportVector()) As Double

            Dim topic As String = model.Trait.trait_name
            Dim correct As Integer = 0
            Dim sumSq As Double = 0

            For Each row As SupportVector In rows
                Dim nodes As Node() = dataset.embedding.ToNodes(row.Properties)
                Dim pred As TraitPrediction = model.Predict(nodes)

                If model.IsRegression() Then
                    Dim diff As Double = pred.value - Double.Parse(row.labels(topic))
                    sumSq += diff * diff
                ElseIf pred.predict IsNot Nothing AndAlso
                    pred.predict.Equals(row.labels(topic), StringComparison.OrdinalIgnoreCase) Then

                    correct += 1
                End If
            Next

            If model.IsRegression() Then
                Return -sumSq / rows.Length
            Else
                Return correct / rows.Length
            End If
        End Function

        Private Shared Sub Shuffle(values As Double(), rand As Random)
            For i As Integer = values.Length - 1 To 1 Step -1
                Dim j As Integer = rand.Next(i + 1)
                Dim tmp As Double = values(i)

                values(i) = values(j)
                values(j) = tmp
            Next
        End Sub

    End Class
End Namespace
