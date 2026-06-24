' ============================================================================
' CrossValidation.vb - 模块5：嵌套交叉验证与超参调优模块
'
' 论文对应：
'   "嵌套交叉验证与超参调优模块"
'
' 核心功能：
'   1. K折交叉验证（外层与内层）：外层评估性能，内层调参
'   2. 网格搜索：在参数空间C ∈ [10^-3, ..., 1]中寻找最优解
'
' 算法原理：
'   - 外层10折交叉验证评估模型
'   - 内层10折交叉验证确定最优参数C
'   - 防止数据泄露
' ============================================================================

Namespace TraitarVB.Modules

    ''' <summary>
    ''' 模块5：嵌套交叉验证与超参调优模块
    ''' 无偏地评估模型性能，并寻找最优的正则化参数C
    ''' </summary>
    Public Class CrossValidation

        Private _svm As SVMClassifier
        Private _random As New Random(42)

        ''' <summary>
        ''' 默认C参数搜索空间
        ''' 论文：C ∈ [10^-3, ..., 1]
        ''' </summary>
        Public ReadOnly DefaultCGrid As Double() = {
            0.001, 0.002, 0.005, 0.007, 0.01, 0.02, 0.05, 0.07, 0.1, 0.2, 0.5, 0.7, 1.0
        }

        Public Sub New()
            _svm = New SVMClassifier()
        End Sub

        ''' <summary>
        ''' K折交叉验证数据划分
        ''' 将数据随机分为K个折，每次用K-1折训练，1折测试
        ''' </summary>
        ''' <param name="nSamples">总样本数</param>
        ''' <param name="k">折数</param>
        ''' <returns>每折的样本索引列表</returns>
        Public Function KFoldSplit(ByVal nSamples As Integer, ByVal k As Integer) As List(Of Integer)()
            ' 生成随机索引
            Dim indices As Integer() = New Integer(nSamples - 1) {}
            For i As Integer = 0 To nSamples - 1
                indices(i) = i
            Next

            ' Fisher-Yates洗牌
            For i As Integer = nSamples - 1 To 1 Step -1
                Dim j As Integer = _random.Next(i + 1)
                Dim temp As Integer = indices(i)
                indices(i) = indices(j)
                indices(j) = temp
            Next

            ' 分成K折
            Dim folds As List(Of Integer)() = New List(Of Integer)(k - 1) {}
            For i As Integer = 0 To k - 1
                folds(i) = New List(Of Integer)()
            Next

            For i As Integer = 0 To nSamples - 1
                folds(i Mod k).Add(indices(i))
            Next

            Return folds
        End Function

        ''' <summary>
        ''' 网格搜索寻找最优C参数
        ''' 论文：在参数空间C ∈ [10^-3, ..., 1]中寻找最优解
        '''
        ''' 使用内层交叉验证评估每个C值的性能
        ''' </summary>
        ''' <param name="X">特征矩阵</param>
        ''' <param name="y">标签数组</param>
        ''' <param name="cGrid">C参数候选列表</param>
        ''' <param name="innerFolds">内层折数</param>
        ''' <returns>最优C值和对应准确率</returns>
        Public Function GridSearchC(ByVal X As Integer(,), ByVal y As Integer(),
                                    ByVal cGrid As Double(),
                                    Optional ByVal innerFolds As Integer = 10) As (bestC As Double, bestAcc As Double)

            Console.WriteLine("[模块5] 网格搜索最优C参数")
            Console.WriteLine("       候选C值: {0}", String.Join(", ", cGrid))
            Console.WriteLine("       内层折数: {0}", innerFolds)

            Dim nSamples As Integer = X.GetLength(0)
            Dim folds As List(Of Integer)() = KFoldSplit(nSamples, innerFolds)

            Dim bestC As Double = cGrid(0)
            Dim bestAcc As Double = -1.0

            For Each cVal As Double In cGrid
                Dim accuracies As New List(Of Double)()

                ' 内层K折交叉验证
                For foldIdx As Integer = 0 To innerFolds - 1
                    ' 划分训练集和验证集
                    Dim trainIndices As New List(Of Integer)()
                    Dim valIndices As List(Of Integer) = folds(foldIdx)

                    For i As Integer = 0 To innerFolds - 1
                        If i <> foldIdx Then
                            trainIndices.AddRange(folds(i))
                        End If
                    Next

                    ' 构建训练集和验证集
                    Dim XTrain As Integer(,) = SubsetMatrix(X, trainIndices)
                    Dim yTrain As Integer() = SubsetArray(y, trainIndices)
                    Dim XVal As Integer(,) = SubsetMatrix(X, valIndices)
                    Dim yVal As Integer() = SubsetArray(y, valIndices)

                    ' 训练模型
                    Dim model As SVMClassifier.SVMModel = _svm.Train(XTrain, yTrain, cVal)

                    ' 验证集预测
                    Dim predictions As Integer() = _svm.PredictBatch(model, XVal)

                    ' 计算准确率
                    Dim acc As Double = ComputeAccuracy(yVal, predictions)
                    accuracies.Add(acc)
                Next

                ' 平均准确率
                Dim meanAcc As Double = 0.0
                For Each a As Double In accuracies
                    meanAcc += a
                Next
                meanAcc /= accuracies.Count

                Console.WriteLine("       C={0}: 平均准确率={1:F4}", cVal, meanAcc)

                If meanAcc > bestAcc Then
                    bestAcc = meanAcc
                    bestC = cVal
                End If
            Next

            Console.WriteLine("[模块5] 最优C={0}, 准确率={1:F4}", bestC, bestAcc)
            Return (bestC, bestAcc)
        End Function

        ''' <summary>
        ''' 嵌套交叉验证
        ''' 论文：外层10折交叉验证评估模型，内层10折交叉验证确定最优参数C
        ''' </summary>
        ''' <param name="X">特征矩阵</param>
        ''' <param name="y">标签数组</param>
        ''' <param name="outerFolds">外层折数（默认10）</param>
        ''' <param name="innerFolds">内层折数（默认10）</param>
        ''' <param name="cGrid">C参数候选列表</param>
        ''' <returns>每折的准确率和最优C值</returns>
        Public Function NestedCrossValidation(ByVal X As Integer(,), ByVal y As Integer(),
                                              Optional ByVal outerFolds As Integer = 10,
                                              Optional ByVal innerFolds As Integer = 10,
                                              Optional ByVal cGrid As Double() = Nothing) As List(Of (foldIdx As Integer, accuracy As Double, bestC As Double))

            If cGrid Is Nothing Then cGrid = DefaultCGrid

            Console.WriteLine("[模块5] 嵌套交叉验证")
            Console.WriteLine("       外层折数: {0}", outerFolds)
            Console.WriteLine("       内层折数: {0}", innerFolds)

            Dim nSamples As Integer = X.GetLength(0)
            Dim outerFoldsData As List(Of Integer)() = KFoldSplit(nSamples, outerFolds)

            Dim results As New List(Of (Integer, Double, Double))()

            For foldIdx As Integer = 0 To outerFolds - 1
                Console.WriteLine("       --- 外层折 {0}/{1} ---", foldIdx + 1, outerFolds)

                ' 划分训练集和测试集
                Dim trainIndices As New List(Of Integer)()
                Dim testIndices As List(Of Integer) = outerFoldsData(foldIdx)

                For i As Integer = 0 To outerFolds - 1
                    If i <> foldIdx Then
                        trainIndices.AddRange(outerFoldsData(i))
                    End If
                Next

                ' 构建训练集和测试集
                Dim XTrain As Integer(,) = SubsetMatrix(X, trainIndices)
                Dim yTrain As Integer() = SubsetArray(y, trainIndices)
                Dim XTest As Integer(,) = SubsetMatrix(X, testIndices)
                Dim yTest As Integer() = SubsetArray(y, testIndices)

                ' 内层网格搜索找最优C
                Dim innerResult As (bestC As Double, bestAcc As Double) =
                    GridSearchC(XTrain, yTrain, cGrid, innerFolds)

                ' 用最优C在全部训练集上训练
                Dim model As SVMClassifier.SVMModel = _svm.Train(XTrain, yTrain, innerResult.bestC)

                ' 测试集预测
                Dim predictions As Integer() = _svm.PredictBatch(model, XTest)
                Dim acc As Double = ComputeAccuracy(yTest, predictions)

                Console.WriteLine("       折 {0}: 最优C={1}, 测试准确率={2:F4}",
                                  foldIdx + 1, innerResult.bestC, acc)

                results.Add((foldIdx, acc, innerResult.bestC))
            Next

            ' 输出总体性能
            Dim meanAcc As Double = 0.0
            For Each r As (Integer, Double, Double) In results
                meanAcc += r.Item2
            Next
            meanAcc /= results.Count
            Console.WriteLine("[模块5] 嵌套交叉验证完成: 平均准确率={0:F4}", meanAcc)

            Return results
        End Function

        ''' <summary>
        ''' 计算准确率
        ''' </summary>
        Public Function ComputeAccuracy(ByVal yTrue As Integer(), ByVal yPred As Integer()) As Double
            If yTrue.Length = 0 Then Return 0.0
            Dim correct As Integer = 0
            For i As Integer = 0 To yTrue.Length - 1
                If yTrue(i) = yPred(i) Then correct += 1
            Next
            Return CDbl(correct) / CDbl(yTrue.Length)
        End Function

        ''' <summary>
        ''' 从矩阵中抽取子集（按行索引）
        ''' </summary>
        Private Function SubsetMatrix(ByVal X As Integer(,), ByVal indices As List(Of Integer)) As Integer(,)
            Dim nFeatures As Integer = X.GetLength(1)
            Dim result As Integer(,) = New Integer(indices.Count - 1, nFeatures - 1) {}
            For i As Integer = 0 To indices.Count - 1
                For j As Integer = 0 To nFeatures - 1
                    result(i, j) = X(indices(i), j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 从数组中抽取子集
        ''' </summary>
        Private Function SubsetArray(ByVal arr As Integer(), ByVal indices As List(Of Integer)) As Integer()
            Dim result As Integer() = New Integer(indices.Count - 1) {}
            For i As Integer = 0 To indices.Count - 1
                result(i) = arr(indices(i))
            Next
            Return result
        End Function

    End Class

End Namespace
