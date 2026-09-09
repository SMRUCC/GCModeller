Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' P-NET 的训练器
''' </summary>
''' <remarks>
''' 训练流程严格遵循论文 Methods 部分的描述：
'''
''' 1. 按照 80% 训练 / 10% 验证 / 10% 测试做**分层**划分，
'''    保证每一个子集之中的类别比例与原始数据集一致；
''' 2. 按照训练集的类别比例计算二元交叉熵的类别权重；
''' 3. 使用 Adam 优化器做 mini-batch 训练，初始学习率 0.001，每 50 个 epoch 阶梯衰减；
''' 4. 损失为各个深度监督预测头的加权二元交叉熵之和。
''' </remarks>
Public Class PNETTrainer

    ''' <summary>
    ''' 被训练的模型
    ''' </summary>
    ''' <returns>P-NET 模型对象</returns>
    Public ReadOnly Property Model As PNETModel

    ''' <summary>
    ''' 训练超参数配置
    ''' </summary>
    ''' <returns>训练配置对象</returns>
    Public ReadOnly Property Config As TrainConfig

    ''' <summary>
    ''' 训练所使用的优化器
    ''' </summary>
    ''' <returns>Adam 优化器</returns>
    Public ReadOnly Property Optimizer As AdamOptimizer

    ''' <summary>
    ''' 正样本类别权重，由 <see cref="FitClassWeights"/> 计算得到
    ''' </summary>
    ''' <returns>正样本权重</returns>
    Public ReadOnly Property PositiveWeight As Double
        Get
            Return _positiveWeight
        End Get
    End Property

    ''' <summary>
    ''' 负样本类别权重，由 <see cref="FitClassWeights"/> 计算得到
    ''' </summary>
    ''' <returns>负样本权重</returns>
    Public ReadOnly Property NegativeWeight As Double
        Get
            Return _negativeWeight
        End Get
    End Property

    Private _positiveWeight As Double = 1.0
    Private _negativeWeight As Double = 1.0

    ''' <summary>
    ''' 创建训练器
    ''' </summary>
    ''' <param name="model">待训练的 P-NET 模型</param>
    ''' <param name="config">训练超参数配置，取 Nothing 时使用默认配置</param>
    Public Sub New(model As PNETModel, Optional config As TrainConfig = Nothing)
        If model Is Nothing Then
            Throw New ArgumentNullException(NameOf(model))
        End If

        Me.Model = model
        Me.Config = If(config, New TrainConfig())

        Dim parameters As List(Of Tensor) = model.GetParameters()
        Dim gradients As List(Of Tensor) = model.GetGradients()

        Me.Optimizer = New AdamOptimizer(
            parameters, gradients,
            Me.Config.LearningRate,
            Me.Config.Beta1,
            Me.Config.Beta2,
            Me.Config.Epsilon
        )
    End Sub

    ''' <summary>
    ''' 按照训练集的类别比例计算二元交叉熵的类别权重
    ''' </summary>
    ''' <param name="data">训练集</param>
    ''' <remarks>
    ''' 权重取值为 <c>N / (2 · N_class)</c>，与 sklearn 的 <c>class_weight='balanced'</c> 一致。
    ''' 若 <see cref="TrainConfig.UseClassWeights"/> 为 False，则两个权重均取 1。
    ''' </remarks>
    Public Sub FitClassWeights(data As PNETSampleSet)
        If Not Config.UseClassWeights Then
            _positiveWeight = 1.0
            _negativeWeight = 1.0
            Return
        End If

        Dim n As Integer = data.Count
        Dim nPos As Integer = data.PositiveCount
        Dim nNeg As Integer = data.NegativeCount

        If nPos = 0 OrElse nNeg = 0 Then
            _positiveWeight = 1.0
            _negativeWeight = 1.0
            Return
        End If

        _positiveWeight = n / (2.0 * nPos)
        _negativeWeight = n / (2.0 * nNeg)
    End Sub

    ''' <summary>
    ''' 按照给定的比例做分层划分
    ''' </summary>
    ''' <param name="data">完整数据集</param>
    ''' <param name="trainRatio">训练集比例，默认 0.8</param>
    ''' <param name="validationRatio">验证集比例，默认 0.1（其余样本归入测试集）</param>
    ''' <returns>划分之后的训练集、验证集与测试集</returns>
    ''' <remarks>
    ''' 分层（stratified）的含义是：在正样本与负样本内部各自独立地做随机划分，
    ''' 从而保证每一个子集之中的类别比例与原始数据集完全一致。
    ''' </remarks>
    Public Function StratifiedSplit(data As PNETSampleSet,
                                   Optional trainRatio As Double = 0.8,
                                   Optional validationRatio As Double = 0.1) As DataSplit

        Dim random As Random = If(Config.Seed.HasValue, New Random(Config.Seed.Value), New Random())
        Dim positives As New List(Of Integer)()
        Dim negatives As New List(Of Integer)()

        For i As Integer = 0 To data.Count - 1
            If data.Labels(i) > 0.5 Then
                positives.Add(i)
            Else
                negatives.Add(i)
            End If
        Next

        Dim trainIdx As New List(Of Integer)()
        Dim valIdx As New List(Of Integer)()
        Dim testIdx As New List(Of Integer)()

        Call SplitClass(positives, trainRatio, validationRatio, random, trainIdx, valIdx, testIdx)
        Call SplitClass(negatives, trainRatio, validationRatio, random, trainIdx, valIdx, testIdx)

        Call Shuffle(trainIdx, random)

        Return New DataSplit With {
            .Train = data.Subset(trainIdx.ToArray()),
            .Validation = data.Subset(valIdx.ToArray()),
            .Test = data.Subset(testIdx.ToArray())
        }
    End Function

    Private Shared Sub SplitClass(indices As List(Of Integer), trainRatio As Double, validationRatio As Double,
                                  random As Random, trainIdx As List(Of Integer),
                                  valIdx As List(Of Integer), testIdx As List(Of Integer))

        Dim order As List(Of Integer) = New List(Of Integer)(indices)

        Call Shuffle(order, random)

        Dim n As Integer = order.Count
        Dim nTrain As Integer = CInt(std.Floor(n * trainRatio))
        Dim nVal As Integer = CInt(std.Floor(n * validationRatio))

        If nTrain = 0 AndAlso n > 0 Then
            nTrain = 1
        End If
        If nTrain + nVal > n Then
            nVal = n - nTrain
        End If

        For i As Integer = 0 To n - 1
            If i < nTrain Then
                trainIdx.Add(order(i))
            ElseIf i < nTrain + nVal Then
                valIdx.Add(order(i))
            Else
                testIdx.Add(order(i))
            End If
        Next
    End Sub

    Private Shared Sub Shuffle(list As List(Of Integer), random As Random)
        For i As Integer = list.Count - 1 To 1 Step -1
            Dim j As Integer = random.Next(i + 1)
            Dim tmp As Integer = list(i)

            list(i) = list(j)
            list(j) = tmp
        Next
    End Sub

    ''' <summary>
    ''' 在训练集上训练模型
    ''' </summary>
    ''' <param name="trainingSet">训练集</param>
    ''' <param name="validation">验证集，给出之后会逐轮记录验证集 AUC 并可选地启用早停</param>
    ''' <returns>训练过程的历史记录</returns>
    Public Function Train(trainingSet As PNETSampleSet, Optional validation As PNETSampleSet = Nothing) As TrainingHistory
        Call FitClassWeights(trainingSet)

        Dim history As New TrainingHistory()
        Dim random As Random = If(Config.Seed.HasValue, New Random(Config.Seed.Value + 1), New Random())
        Dim n As Integer = trainingSet.Count
        Dim batchSize As Integer = std.Min(Config.BatchSize, n)

        If batchSize < 1 Then
            batchSize = 1
        End If

        Dim order As Integer() = New Integer(n - 1) {}

        For i As Integer = 0 To n - 1
            order(i) = i
        Next

        Dim bestAuc As Double = -1.0
        Dim bestEpoch As Integer = 0
        Dim patienceLeft As Integer = Config.EarlyStopPatience

        For epoch As Integer = 0 To Config.Epochs - 1
            Dim lr As Double = Config.GetLearningRate(epoch)

            Optimizer.LearningRate = lr

            Call ShuffleArray(order, random)

            Dim batches As Integer = (n + batchSize - 1) \ batchSize

            For b As Integer = 0 To batches - 1
                Dim startIdx As Integer = b * batchSize
                Dim size As Integer = std.Min(batchSize, n - startIdx)

                If size <= 0 Then
                    Continue For
                End If

                Dim batchIdx As Integer() = New Integer(size - 1) {}

                Array.Copy(order, startIdx, batchIdx, 0, size)

                Dim batch As PNETSampleSet = trainingSet.Subset(batchIdx)
                Dim forward As PNETForwardResult = Model.Forward(batch.Features)

                Call Model.ZeroGrad()
                Call Model.Backward(forward, batch.Labels, _positiveWeight, _negativeWeight)
                Call Optimizer.Step()
            Next

            ' 在整个训练集与验证集上评估当前模型
            Dim trainScores As Double() = Model.Predict(trainingSet.Features)
            Dim trainAuc As Double = Metrics.AUC(trainingSet.Labels, trainScores)
            Dim trainForward As PNETForwardResult = Model.Forward(trainingSet.Features)
            Dim loss As Double = Model.ComputeLoss(trainForward, trainingSet.Labels, _positiveWeight, _negativeWeight)

            history.Loss.Add(loss)
            history.TrainAUC.Add(trainAuc)
            history.LearningRate.Add(lr)

            Dim valAuc As Double = 0.0

            If validation IsNot Nothing AndAlso validation.Count > 0 Then
                valAuc = Metrics.AUC(validation.Labels, Model.Predict(validation.Features))
                history.ValidationAUC.Add(valAuc)
            End If

            If Config.Verbose AndAlso ((epoch + 1) Mod Config.VerboseInterval = 0 OrElse epoch = 0) Then
                Dim msg As String = $"epoch {epoch + 1}/{Config.Epochs}, loss={loss.ToString("F4")}, train_auc={trainAuc.ToString("F4")}"

                If validation IsNot Nothing AndAlso validation.Count > 0 Then
                    msg &= $", val_auc={valAuc.ToString("F4")}"
                End If

                msg &= $", lr={lr.ToString("F6")}"

                Call Console.WriteLine(msg)
            End If

            If validation IsNot Nothing AndAlso validation.Count > 0 AndAlso valAuc > bestAuc Then
                bestAuc = valAuc
                bestEpoch = epoch
                patienceLeft = Config.EarlyStopPatience
            ElseIf Config.EarlyStopPatience > 0 Then
                patienceLeft -= 1

                If patienceLeft <= 0 Then
                    If Config.Verbose Then
                        Call Console.WriteLine($"early stopping at epoch {epoch + 1}")
                    End If

                    Exit For
                End If
            End If
        Next

        history.BestEpoch = bestEpoch
        history.BestValidationAUC = bestAuc

        Return history
    End Function

    ''' <summary>
    ''' 在测试集上评估模型
    ''' </summary>
    ''' <param name="test">测试集</param>
    ''' <param name="threshold">判定为正例的阈值，默认 0.5</param>
    ''' <returns>评估指标结果</returns>
    Public Function Evaluate(test As PNETSampleSet, Optional threshold As Double = 0.5) As EvaluationResult
        Dim scores As Double() = Model.Predict(test.Features)

        Return Metrics.Evaluate(test.Labels, scores, threshold)
    End Function

    Private Shared Sub ShuffleArray(array As Integer(), random As Random)
        For i As Integer = array.Length - 1 To 1 Step -1
            Dim j As Integer = random.Next(i + 1)
            Dim tmp As Integer = array(i)

            array(i) = array(j)
            array(j) = tmp
        Next
    End Sub

End Class
