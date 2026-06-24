' ============================================================================
' FeatureSelection.vb - 模块7：特征选择与关联解释模块
'
' 论文对应：
'   "反向关联：识别决定表型的关键蛋白质家族"
'
' 核心功能：
'   1. 权重过滤：提取SVM线性超平面的正权重向量
'   2. 多数特征筛选：至少3个模型选中该特征才保留
'   3. 皮尔逊相关系数(PCC)：对选中的特征与表型的相关性进行排序
'
' 算法原理：
'   - 多数特征选择：在5个最佳SVM模型的投票委员会中，如果某个蛋白质家族
'     在至少3个模型（即多数）中拥有正的权重，该家族就被认为是决定该表型
'     的重要特征
'   - 相关性排序：利用皮尔逊相关系数，对这些被选出的蛋白质家族与表型的
'     相关性进行排序
' ============================================================================

Imports System.IO

Namespace TraitarVB.Modules

    ''' <summary>
    ''' 模块7：特征选择与关联解释模块
    ''' 识别出对表型预测贡献最大的蛋白质家族，提供生物学解释
    ''' </summary>
    Public Class FeatureSelection

        ReadOnly loader As ModelLoader

        ''' <summary>
        ''' 关键特征信息
        ''' </summary>
        Public Class KeyFeature
            ''' <summary>Pfam家族ID</summary>
            Public Property PfamId As String

            ''' <summary>Pfam描述</summary>
            Public Property Description As String

            ''' <summary>在投票委员会中拥有正权重的模型数</summary>
            Public Property PositiveWeightCount As Integer

            ''' <summary>平均正权重值</summary>
            Public Property AvgPositiveWeight As Double

            ''' <summary>皮尔逊相关系数</summary>
            Public Property PearsonCorrelation As Double

            ''' <summary>是否为关键特征（≥3个模型正权重）</summary>
            Public ReadOnly Property IsKeyFeature As Boolean
                Get
                    Return PositiveWeightCount >= 3
                End Get
            End Property
        End Class

        Sub New(loader As ModelLoader)
            Me.loader = loader
        End Sub

        ''' <summary>
        ''' 从投票委员会中提取关键特征
        ''' 论文：在5个最佳SVM模型的投票委员会中，如果某个蛋白质家族
        '''       在至少3个模型（即多数）中拥有正的权重，该家族就被认为
        '''       是决定该表型的重要特征
        ''' </summary>
        ''' <param name="committee">投票委员会模型列表</param>
        ''' <param name="pfamDescriptions">Pfam描述字典</param>
        ''' <param name="minPositiveModels">最小正权重模型数（默认3）</param>
        ''' <returns>关键特征列表</returns>
        Public Function SelectKeyFeatures(ByVal committee As List(Of SVMClassifier.SVMModel),
                                          ByVal pfamDescriptions As Dictionary(Of String, String),
                                          Optional ByVal minPositiveModels As Integer = 3) As List(Of KeyFeature)

            ' 统计每个特征在多少个模型中拥有正权重
            Dim positiveCounts As New Dictionary(Of String, Integer)()
            Dim weightSums As New Dictionary(Of String, Double)()

            For Each model As SVMClassifier.SVMModel In committee
                For j As Integer = 0 To model.FeatureIds.Count - 1
                    Dim fid As String = model.FeatureIds(j)
                    Dim w As Double = model.Weights(j)

                    If w > 0 Then
                        If Not positiveCounts.ContainsKey(fid) Then
                            positiveCounts(fid) = 0
                            weightSums(fid) = 0.0
                        End If
                        positiveCounts(fid) += 1
                        weightSums(fid) += w
                    End If
                Next
            Next

            ' 筛选关键特征（≥minPositiveModels个模型拥有正权重）
            Dim keyFeatures As New List(Of KeyFeature)()

            For Each kvp As KeyValuePair(Of String, Integer) In positiveCounts
                If kvp.Value >= minPositiveModels Then
                    Dim kf As New KeyFeature()
                    kf.PfamId = kvp.Key
                    kf.PositiveWeightCount = kvp.Value
                    kf.AvgPositiveWeight = weightSums(kvp.Key) / kvp.Value

                    If pfamDescriptions.ContainsKey(kvp.Key) Then
                        kf.Description = pfamDescriptions(kvp.Key)
                    Else
                        kf.Description = ""
                    End If

                    keyFeatures.Add(kf)
                End If
            Next

            ' 按正权重模型数降序、平均权重降序排序
            keyFeatures.Sort(Function(a, b)
                                 If a.PositiveWeightCount <> b.PositiveWeightCount Then
                                     Return b.PositiveWeightCount.CompareTo(a.PositiveWeightCount)
                                 Else
                                     Return b.AvgPositiveWeight.CompareTo(a.AvgPositiveWeight)
                                 End If
                             End Function)

            Console.WriteLine("[模块7] 关键特征选择完成: {0}个特征（≥{1}个模型正权重）",
                              keyFeatures.Count, minPositiveModels)

            Return keyFeatures
        End Function

        ''' <summary>
        ''' 计算皮尔逊相关系数并排序
        ''' 论文：利用皮尔逊相关系数，对这些被选出的蛋白质家族与表型的相关性进行排序
        '''
        ''' 皮尔逊相关系数公式：
        '''   r = Σ((x_i - x̄)(y_i - ȳ)) / sqrt(Σ(x_i - x̄)² × Σ(y_i - ȳ)²)
        ''' </summary>
        ''' <param name="keyFeatures">关键特征列表</param>
        ''' <param name="featureMatrix">特征矩阵（样本×特征）</param>
        ''' <param name="labels">表型标签数组</param>
        ''' <param name="featureIds">特征ID列表（对应矩阵列）</param>
        ''' <returns>按PCC排序的关键特征列表</returns>
        Public Function RankByPearsonCorrelation(
            ByVal keyFeatures As List(Of KeyFeature),
            ByVal featureMatrix As Integer(,),
            ByVal labels As Integer(),
            ByVal featureIds As List(Of String)) As List(Of KeyFeature)

            Dim nSamples As Integer = featureMatrix.GetLength(0)

            ' 计算标签的均值
            Dim yMean As Double = 0.0
            For i As Integer = 0 To nSamples - 1
                yMean += CDbl(labels(i))
            Next
            yMean /= nSamples

            ' 计算标签的离差平方和
            Dim ySumSq As Double = 0.0
            For i As Integer = 0 To nSamples - 1
                Dim d As Double = CDbl(labels(i)) - yMean
                ySumSq += d * d
            Next

            ' 为每个关键特征计算PCC
            For Each kf As KeyFeature In keyFeatures
                Dim colIdx As Integer = featureIds.IndexOf(kf.PfamId)
                If colIdx < 0 Then
                    kf.PearsonCorrelation = 0.0
                    Continue For
                End If

                ' 计算特征列的均值
                Dim xMean As Double = 0.0
                For i As Integer = 0 To nSamples - 1
                    xMean += CDbl(featureMatrix(i, colIdx))
                Next
                xMean /= nSamples

                ' 计算PCC分子和分母
                Dim numerator As Double = 0.0
                Dim xSumSq As Double = 0.0
                For i As Integer = 0 To nSamples - 1
                    Dim dx As Double = CDbl(featureMatrix(i, colIdx)) - xMean
                    Dim dy As Double = CDbl(labels(i)) - yMean
                    numerator += dx * dy
                    xSumSq += dx * dx
                Next

                Dim denominator As Double = Math.Sqrt(xSumSq * ySumSq)
                If denominator > 0.000000000001 Then
                    kf.PearsonCorrelation = numerator / denominator
                Else
                    kf.PearsonCorrelation = 0.0
                End If
            Next

            ' 按PCC降序排序
            keyFeatures.Sort(Function(a, b) b.PearsonCorrelation.CompareTo(a.PearsonCorrelation))

            Console.WriteLine("[模块7] 皮尔逊相关系数排序完成")

            Return keyFeatures
        End Function

        ''' <summary>
        ''' 从non-zero+weights.txt文件加载关键特征
        ''' 文件格式：
        '''   PfamID \t class \t w1 \t w2 \t ... \t w13 \t description \t cor
        ''' </summary>
        Public Function LoadKeyFeaturesFromFile(phenoId As String) As List(Of KeyFeature)
            Dim keyFeatures As New List(Of KeyFeature)()
            Dim filePath As String = Path.Combine(loader._modelsDir, phenoId & "_non-zero+weights.txt")
            Dim lines As String() = System.IO.File.ReadAllLines(filePath)
            If lines.Length < 2 Then Return keyFeatures

            ' 解析表头获取C值列表
            Dim headerParts As String() = lines(0).Split(New Char() {" "c, ControlChars.Tab},
                                                          StringSplitOptions.RemoveEmptyEntries)

            ' 数据行
            For i As Integer = 1 To lines.Length - 1
                Dim line As String = lines(i)
                If String.IsNullOrWhiteSpace(line) Then Continue For

                Dim parts As String() = line.Split(New Char() {" "c, ControlChars.Tab},
                                                    StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 4 Then Continue For

                Dim kf As New KeyFeature()
                kf.PfamId = parts(0)

                ' class (+/-)
                Dim featClass As String = parts(1)

                ' 权重值（从第2列开始，到倒数第2列）
                Dim nWeights As Integer = parts.Length - 4  ' 减去ID, class, description, cor
                Dim positiveCount As Integer = 0
                Dim weightSum As Double = 0.0
                Dim positiveWeightSum As Double = 0.0

                For j As Integer = 0 To nWeights - 1
                    Dim w As Double
                    If Double.TryParse(parts(2 + j), w) Then
                        If w > 0 Then
                            positiveCount += 1
                            positiveWeightSum += w
                        End If
                        weightSum += w
                    End If
                Next

                kf.PositiveWeightCount = positiveCount
                If positiveCount > 0 Then
                    kf.AvgPositiveWeight = positiveWeightSum / positiveCount
                End If

                ' 描述（倒数第2个字段）
                kf.Description = parts(parts.Length - 2)

                ' 皮尔逊相关系数（最后一个字段）
                Dim cor As Double
                If Double.TryParse(parts(parts.Length - 1), cor) Then
                    kf.PearsonCorrelation = cor
                End If

                keyFeatures.Add(kf)
            Next

            ' 按PCC降序排序
            keyFeatures.Sort(Function(a, b) b.PearsonCorrelation.CompareTo(a.PearsonCorrelation))

            Console.WriteLine("[模块7] 从文件加载关键特征: {0}个", keyFeatures.Count)

            Return keyFeatures
        End Function

        ''' <summary>
        ''' 生成关键特征报告
        ''' </summary>
        Public Function GenerateReport(ByVal keyFeatures As List(Of KeyFeature),
                                       Optional ByVal topN As Integer = 20) As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("=== 关键蛋白质家族特征报告 ===")
            sb.AppendLine(String.Format("总特征数: {0}", keyFeatures.Count))
            sb.AppendLine(String.Format("显示前 {0} 个特征:", Math.Min(topN, keyFeatures.Count)))
            sb.AppendLine()
            sb.AppendLine(String.Format("{0,-12} {1,-8} {2,-10} {3,-12} {4,-50}",
                                        "PfamID", "正权重数", "平均权重", "PCC", "描述"))
            sb.AppendLine(New String("-"c, 100))

            For i As Integer = 0 To Math.Min(topN, keyFeatures.Count) - 1
                Dim kf As KeyFeature = keyFeatures(i)
                sb.AppendLine(String.Format("{0,-12} {1,-8} {2,-10:F4} {3,-12:F4} {4,-50}",
                                            kf.PfamId, kf.PositiveWeightCount,
                                            kf.AvgPositiveWeight, kf.PearsonCorrelation,
                                            If(kf.Description.Length > 50,
                                               kf.Description.Substring(0, 50),
                                               kf.Description)))
            Next

            Return sb.ToString()
        End Function

    End Class

End Namespace
