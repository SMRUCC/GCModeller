' ============================================================================
' Module 5: Nested Cross-Validation & Hyperparameter Tuning
' File: Prediction/NestedCrossValidation.vb
'
' 功能: 无偏地评估模型性能，并寻找最优的正则化参数 C。
'       对应论文中 "嵌套交叉验证与超参调优模块"。
'
' 算法原理:
'   1. K折交叉验证（外层与内层）: 外层评估性能，内层调参
'   2. 网格搜索: 在参数空间 C ∈ [10^-3, ..., 1] 中寻找最优解
'   3. 防止数据泄露: 内层 CV 仅使用外层训练集
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

Namespace Traitar.Prediction

    ''' <summary>
    ''' 交叉验证结果
    ''' </summary>
    Public Class CrossValidationResult
        ''' <summary>最优 C 值</summary>
        Public Property BestC As Double
        ''' <summary>最优模型的准确率</summary>
        Public Property BestAccuracy As Double
        ''' <summary>各 C 值的平均准确率</summary>
        Public Property CToAccuracy As New Dictionary(Of Double, Double)
        ''' <summary>外层各折的准确率</summary>
        Public Property OuterFoldAccuracies As New List(Of Double)
        ''' <summary>总体准确率（外层平均）</summary>
        Public ReadOnly Property OverallAccuracy As Double
            Get
                If OuterFoldAccuracies.Count = 0 Then Return 0.0
                Return OuterFoldAccuracies.Average()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"Best C={BestC} (acc={BestAccuracy:F4}), Overall acc={OverallAccuracy:F4}"
        End Function
    End Class

    ''' <summary>
    ''' SVM 训练委托（用于解耦具体的 SVM 实现）
    ''' 参数: X_train, y_train, C, X_test -> predictions
    ''' </summary>
    Public Delegate Function SvmTrainPredictDelegate(
        XTrain As Double()(), yTrain As Integer(),
        C As Double, XTest As Double()()) As Integer()

    ''' <summary>
    ''' 嵌套交叉验证器
    ''' </summary>
    Public Class NestedCrossValidator

        ''' <summary>外层折数（默认10）</summary>
        Public Property OuterFolds As Integer = 10

        ''' <summary>内层折数（默认10）</summary>
        Public Property InnerFolds As Integer = 10

        ''' <summary>C 参数网格（论文: C ∈ [10^-3, ..., 1]）</summary>
        Public Property CGrid As Double() = New Double() {
            0.001, 0.002, 0.005, 0.007, 0.01, 0.02, 0.05, 0.07, 0.1, 0.2, 0.5, 0.7, 1.0
        }

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

        ''' <summary>
        ''' 执行嵌套交叉验证
        ''' </summary>
        ''' <param name="X">特征矩阵: n_samples × n_features</param>
        ''' <param name="y">标签: n_samples</param>
        ''' <param name="svmDelegate">SVM 训练+预测委托</param>
        Public Function Run(X As Double()(), y As Integer(),
                            svmDelegate As SvmTrainPredictDelegate) As CrossValidationResult
            Dim result As New CrossValidationResult()
            Dim n = X.Length
            Dim rand As New Random(RandomSeed)

            ' 生成打乱的索引
            Dim indices(n - 1) As Integer
            For i = 0 To n - 1
                indices(i) = i
            Next
            Shuffle(indices, rand)

            ' 外层 CV
            Dim outerFoldSize = n \ OuterFolds
            Dim cAccumulator As New Dictionary(Of Double, List(Of Double))
            For Each c In CGrid
                cAccumulator(c) = New List(Of Double)()
            Next

            For outerFold = 0 To OuterFolds - 1
                ' 划分外层训练集/测试集
                Dim outerTestStart = outerFold * outerFoldSize
                Dim outerTestEnd = If(outerFold = OuterFolds - 1, n, outerTestStart + outerFoldSize)

                Dim outerTrainIdx As New List(Of Integer)
                Dim outerTestIdx As New List(Of Integer)
                For i = 0 To n - 1
                    If i >= outerTestStart AndAlso i < outerTestEnd Then
                        outerTestIdx.Add(indices(i))
                    Else
                        outerTrainIdx.Add(indices(i))
                    End If
                Next

                ' 内层 CV: 在外层训练集上寻找最优 C
                Dim bestC = CGrid(0)
                Dim bestInnerAcc = -1.0

                For Each c In CGrid
                    Dim innerAccs As New List(Of Double)
                    Dim innerFoldSize = outerTrainIdx.Count \ InnerFolds

                    For innerFold = 0 To InnerFolds - 1
                        Dim innerTestStart = innerFold * innerFoldSize
                        Dim innerTestEnd = If(innerFold = InnerFolds - 1, outerTrainIdx.Count, innerTestStart + innerFoldSize)

                        Dim innerTrainIdx As New List(Of Integer)
                        Dim innerTestIdx As New List(Of Integer)
                        For i = 0 To outerTrainIdx.Count - 1
                            If i >= innerTestStart AndAlso i < innerTestEnd Then
                                innerTestIdx.Add(outerTrainIdx(i))
                            Else
                                innerTrainIdx.Add(outerTrainIdx(i))
                            End If
                        Next

                        ' 准备内层数据
                        Dim XInnerTrain(innerTrainIdx.Count - 1)() As Double
                        Dim yInnerTrain(innerTrainIdx.Count - 1) As Integer
                        For i = 0 To innerTrainIdx.Count - 1
                            XInnerTrain(i) = X(innerTrainIdx(i))
                            yInnerTrain(i) = y(innerTrainIdx(i))
                        Next

                        Dim XInnerTest(innerTestIdx.Count - 1)() As Double
                        Dim yInnerTest(innerTestIdx.Count - 1) As Integer
                        For i = 0 To innerTestIdx.Count - 1
                            XInnerTest(i) = X(innerTestIdx(i))
                            yInnerTest(i) = y(innerTestIdx(i))
                        Next

                        ' 训练+预测
                        Dim preds = svmDelegate(XInnerTrain, yInnerTrain, c, XInnerTest)
                        Dim correct = 0
                        For i = 0 To preds.Length - 1
                            If preds(i) = yInnerTest(i) Then correct += 1
                        Next
                        innerAccs.Add(CDbl(correct) / preds.Length)
                    Next

                    Dim avgInnerAcc = innerAccs.Average()
                    cAccumulator(c).Add(avgInnerAcc)

                    If avgInnerAcc > bestInnerAcc Then
                        bestInnerAcc = avgInnerAcc
                        bestC = c
                    End If
                Next

                ' 用最优 C 在外层训练集上训练，在外层测试集上评估
                Dim XOuterTrain(outerTrainIdx.Count - 1)() As Double
                Dim yOuterTrain(outerTrainIdx.Count - 1) As Integer
                For i = 0 To outerTrainIdx.Count - 1
                    XOuterTrain(i) = X(outerTrainIdx(i))
                    yOuterTrain(i) = y(outerTrainIdx(i))
                Next

                Dim XOuterTest(outerTestIdx.Count - 1)() As Double
                Dim yOuterTest(outerTestIdx.Count - 1) As Integer
                For i = 0 To outerTestIdx.Count - 1
                    XOuterTest(i) = X(outerTestIdx(i))
                    yOuterTest(i) = y(outerTestIdx(i))
                Next

                Dim outerPreds = svmDelegate(XOuterTrain, yOuterTrain, bestC, XOuterTest)
                Dim outerCorrect = 0
                For i = 0 To outerPreds.Length - 1
                    If outerPreds(i) = yOuterTest(i) Then outerCorrect += 1
                Next
                Dim outerAcc = CDbl(outerCorrect) / outerPreds.Length
                result.OuterFoldAccuracies.Add(outerAcc)
            Next

            ' 计算各 C 值的平均准确率
            For Each c In CGrid
                result.CToAccuracy(c) = cAccumulator(c).Average()
            Next

            ' 选择总体最优 C
            Dim bestOverallC = CGrid(0)
            Dim bestOverallAcc = -1.0
            For Each kv In result.CToAccuracy
                If kv.Value > bestOverallAcc Then
                    bestOverallAcc = kv.Value
                    bestOverallC = kv.Key
                End If
            Next
            result.BestC = bestOverallC
            result.BestAccuracy = bestOverallAcc

            Return result
        End Function

        ''' <summary>Fisher-Yates 洗牌</summary>
        Private Sub Shuffle(arr As Integer(), rand As Random)
            For i = arr.Length - 1 To 1 Step -1
                Dim j = rand.Next(i + 1)
                Dim temp = arr(i)
                arr(i) = arr(j)
                arr(j) = temp
            Next
        End Sub
    End Class
End Namespace
