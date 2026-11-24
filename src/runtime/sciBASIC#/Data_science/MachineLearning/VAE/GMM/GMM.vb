' ============================================================================
' GMM.vb - 高斯混合模型 (Gaussian Mixture Model)
'
' 生产化重写要点（公开签名一律保持不变）：
'   1. 修正权重初始化：原实现 `For K = 0 To K - 1 : wData(K) = 1.0 / K`
'      的循环变量遮蔽了维度变量，首次迭代产生 1.0/0 = +Infinity，
'      经由 log(Weights) 使整条 EM 链路静默产出 NaN；
'   2. 修正 LogDeterminant 的 `logDet = -logDet`（行交换本不影响 |det|）；
'   3. E 步改用 Cholesky 分解 + 前代求解计算马氏距离，每个成分每轮只需一次
'      O(D³/3) 分解，取代原先的两次 O(D³)（求逆 + 行列式）；
'   4. 采样使用 x = μ + L·z，恢复协方差矩阵的相关性结构；
'   5. 空簇重初始化、权重下限、协方差下限；
'   6. 新增 BIC/AIC/参数计数、输入维度校验、Save/TryLoad；
'   7. 线性代数全部下沉到 Core.Numerics.LinearAlgebraOps → MatrixBridge → JAMA。
' ============================================================================

Imports Microsoft.VisualBasic.DataMining.ComponentModel.EntityModels
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Numerics
Imports std = System.Math

''' <summary>
''' 高斯混合模型 (Gaussian Mixture Model)
''' 使用 EM 算法对数据进行聚类 / 密度估计 / 信号分解
''' </summary>
''' <remarks>
''' 模型形式: p(x) = Σ_k π_k · N(x | μ_k, Σ_k)
''' 其中 π_k 为混合权重, μ_k 为均值向量, Σ_k 为协方差矩阵
''' </remarks>
Public Class GaussianMixtureModel
    Implements IProbabilisticClusterer
    Implements IPriorSampler
    Implements IModelScore
    Implements IModelPersistable

    ''' <summary>模型文件的数据结构版本。</summary>
    Public Const ModelFileSchemaVersion As Integer = 1

#Region "私有字段"

    ' 混合成分数量
    Private _nComponents As Integer

    ' 最大迭代次数
    Private _maxIter As Integer

    ' 收敛阈值 (对数似然变化)
    Private _tol As Double

    ' 协方差正则化项 (防止奇异协方差矩阵)
    Private _regCovar As Double

    ' 随机数种子
    Private _seed As Integer?

    ' 是否使用对角协方差矩阵 (保留以维持 ToString 的既有文案)
    Private _diagCovariance As Boolean

    ' 初始化方式
    Private _initMethod As InitMethod

    ' 扩展选项（协方差结构、下限、空簇策略）
    Private _options As GmmOptions

    ' 可复现的随机数服务
    Private _rng As RandomGaussian

    ' 训练样本数量（BIC 的惩罚项需要）
    Private _sampleCount As Integer

    ' 是否已完成训练
    Private _fitted As Boolean = False

    ' 全协方差 / Tied 模式下每个成分的 Cholesky 下三角因子
    Private _factors As Tensor()

    ' E 步 / M 步复用的缓冲区，避免每轮重新分配 N×K 张量
    Private _logResp As Double(,)
    Private _resp As Double(,)

#End Region

#Region "模型参数 (训练后可获得)"

    ''' <summary>混合权重 π_k, 形状 [K]</summary>
    Public Property Weights As Tensor

    ''' <summary>均值向量 μ_k, 形状 [K, D]</summary>
    Public Property Means As Tensor

    ''' <summary>协方差矩阵 Σ_k, 形状 [K, D, D] (全协方差) 或 [K, D] (对角协方差)</summary>
    Public Property Covariances As Tensor

    ''' <summary>是否已收敛</summary>
    Public Property Converged As Boolean = False

    ''' <summary>实际迭代次数</summary>
    Public Property NIter As Integer = 0

    ''' <summary>最终对数似然</summary>
    Public Property LogLikelihood As Double = Double.NegativeInfinity

    ''' <summary>训练历史对数似然值</summary>
    Public Property LogLikelihoodHistory As New List(Of Double)

#End Region

#Region "新增只读属性"

    ''' <summary>混合成分数量。</summary>
    Public ReadOnly Property NComponents As Integer Implements IProbabilisticClusterer.NComponents
        Get
            Return _nComponents
        End Get
    End Property

    ''' <summary>训练数据的特征维度；模型尚未训练时为 0。</summary>
    Public ReadOnly Property InputDim As Integer
        Get
            If Means Is Nothing Then
                Return 0
            End If

            Return Means.Shape(1)
        End Get
    End Property

    ''' <summary>模型是否已完成训练。</summary>
    Public ReadOnly Property IsFitted As Boolean
        Get
            Return _fitted AndAlso Means IsNot Nothing
        End Get
    End Property

    ''' <summary>协方差结构类型。</summary>
    Public ReadOnly Property CovarianceType As GmmCovarianceType
        Get
            Return _options.CovarianceType
        End Get
    End Property

    ''' <summary>模型自由参数个数（用于 BIC/AIC）。</summary>
    Public ReadOnly Property NParameters As Integer
        Get
            If Means Is Nothing Then
                Return 0
            End If

            Return ModelSelection.GaussianMixtureParameters(_nComponents, InputDim, _options.ToCovarianceKind())
        End Get
    End Property

    ''' <summary>贝叶斯信息准则，数值越小越好。</summary>
    Public ReadOnly Property BIC As Double
        Get
            Return ModelSelection.Bic(LogLikelihood, NParameters, _sampleCount)
        End Get
    End Property

    ''' <summary>赤池信息准则，数值越小越好。</summary>
    Public ReadOnly Property AIC As Double
        Get
            Return ModelSelection.Aic(LogLikelihood, NParameters)
        End Get
    End Property

    ''' <summary>训练时使用的样本数量。</summary>
    Public ReadOnly Property SampleCount As Integer
        Get
            Return _sampleCount
        End Get
    End Property

#End Region

#Region "枚举与构造"

    ''' <summary>参数初始化方式</summary>
    Public Enum InitMethod
        ''' <summary>随机初始化 (从数据中随机采样)</summary>
        Random
        ''' <summary>K-Means++ 风格初始化</summary>
        KMeansPP
    End Enum

    ''' <summary>
    ''' 创建高斯混合模型
    ''' </summary>
    ''' <param name="nComponents">混合成分数量 K</param>
    ''' <param name="maxIter">最大 EM 迭代次数</param>
    ''' <param name="tol">对数似然收敛阈值</param>
    ''' <param name="regCovar">协方差正则化项, 防止矩阵奇异</param>
    ''' <param name="diagCovariance">是否使用对角协方差 (高维数据推荐)</param>
    ''' <param name="initMethod">参数初始化方式</param>
    ''' <param name="seed">随机数种子</param>
    Public Sub New(Optional nComponents As Integer = 3,
                   Optional maxIter As Integer = 200,
                   Optional tol As Double = 0.000001,
                   Optional regCovar As Double = 0.000001,
                   Optional diagCovariance As Boolean = False,
                   Optional initMethod As InitMethod = InitMethod.KMeansPP,
                   Optional seed As Integer? = Nothing)
        Call Configure(nComponents, maxIter, tol, regCovar, initMethod, seed,
                       GmmOptions.FromDiagonalFlag(diagCovariance))
    End Sub

    ''' <summary>
    ''' 使用扩展选项创建高斯混合模型（新增重载，不影响既有构造签名）。
    ''' </summary>
    Public Sub New(nComponents As Integer,
                   options As GmmOptions,
                   Optional maxIter As Integer = 200,
                   Optional tol As Double = 0.000001,
                   Optional regCovar As Double = 0.000001,
                   Optional initMethod As InitMethod = InitMethod.KMeansPP,
                   Optional seed As Integer? = Nothing)
        Call Configure(nComponents, maxIter, tol, regCovar, initMethod, seed,
                       If(options, New GmmOptions()))
    End Sub

    Private Sub Configure(nComponents As Integer, maxIter As Integer, tol As Double, regCovar As Double,
                          initMethod As InitMethod, seed As Integer?, options As GmmOptions)
        If nComponents < 1 Then
            Throw New ArgumentException("混合成分数量必须大于 0")
        End If

        _nComponents = nComponents
        _maxIter = std.Max(1, maxIter)
        _tol = tol
        _regCovar = std.Max(0, regCovar)
        _initMethod = initMethod
        _seed = seed
        _options = If(options, New GmmOptions())
        _diagCovariance = (_options.CovarianceType = GmmCovarianceType.Diagonal OrElse
                           _options.CovarianceType = GmmCovarianceType.Spherical)
    End Sub

#End Region

#Region "训练 (EM 算法)"

    ''' <summary>
    ''' 使用 EM 算法拟合模型
    ''' </summary>
    ''' <param name="X">输入数据, 形状 [N, D], N 为样本数, D 为特征维度</param>
    Public Sub Fit(X As Tensor)
        Call FitCore(X, _maxIter, _tol)
    End Sub

    ''' <summary>
    ''' 使用统一的训练选项拟合模型（新增重载）。迭代轮数取自
    ''' <see cref="TrainingOptions.Epochs"/>。
    ''' </summary>
    Public Function Fit(X As Tensor, options As TrainingOptions) As GaussianMixtureModel
        Dim iterations As Integer = If(options Is Nothing, _maxIter, std.Max(1, options.Epochs))

        Call FitCore(X, iterations, _tol)

        Return Me
    End Function

    Private Sub FitCore(X As Tensor, maxIterations As Integer, tolerance As Double)
        Call ValidateInput(X, allowUnfitted:=True)

        Dim sampleCount As Integer = X.Shape(0)
        Dim dimension As Integer = X.Shape(1)

        If sampleCount < _nComponents Then
            Throw New ArgumentException(
                $"样本数 {sampleCount} 少于混合成分数 {_nComponents}，无法估计模型参数")
        End If

        ' 无论第几次调用 Fit，都从干净的状态开始
        Converged = False
        NIter = 0
        LogLikelihood = Double.NegativeInfinity
        LogLikelihoodHistory.Clear()
        _fitted = False
        _sampleCount = sampleCount

        _rng = If(_seed.HasValue, New RandomGaussian(_seed.Value), New RandomGaussian())

        Call InitializeParameters(X, dimension)

        _logResp = New Double(sampleCount - 1, _nComponents - 1) {}
        _resp = New Double(sampleCount - 1, _nComponents - 1) {}

        Dim previous As Double = Double.NegativeInfinity

        For iteration As Integer = 1 To maxIterations
            Call BuildFactors()

            Dim current As Double = EStepCore(X, _logResp)

            If Double.IsNaN(current) OrElse Double.IsInfinity(current) Then
                ' 数值异常时保留上一轮参数，并以未收敛状态结束
                Call DebugTrace(sampleCount, current)
                Exit For
            End If

            LogLikelihoodHistory.Add(current)
            LogLikelihood = current
            NIter = iteration

            If iteration > 1 AndAlso std.Abs(current - previous) < tolerance Then
                Converged = True
                Exit For
            End If

            previous = current

            Call MStepCore(X, _logResp, _resp)
        Next

        ' 用最终的参数重新评估一次，使 LogLikelihood 与当前参数一致
        If NIter > 0 Then
            Call BuildFactors()

            Dim final As Double = EStepCore(X, _logResp)

            If Not Double.IsNaN(final) AndAlso Not Double.IsInfinity(final) Then
                LogLikelihood = final
                LogLikelihoodHistory(LogLikelihoodHistory.Count - 1) = final
            End If
        End If

        _fitted = Means IsNot Nothing AndAlso Not HasInvalidParameters()
    End Sub

    ''' <summary>检测参数中是否混入了 NaN / Infinity。</summary>
    Private Function HasInvalidParameters() As Boolean
        Return HasInvalid(Weights) OrElse HasInvalid(Means) OrElse HasInvalid(Covariances)
    End Function

    Private Shared Function HasInvalid(t As Tensor) As Boolean
        If t Is Nothing Then
            Return True
        End If

        Dim data As Double() = t.Data

        For i As Integer = 0 To data.Length - 1
            If Double.IsNaN(data(i)) OrElse Double.IsInfinity(data(i)) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Shared Sub DebugTrace(sampleCount As Integer, value As Double)
        System.Diagnostics.Debug.WriteLine($"[GMM] 迭代出现异常对数似然 {value}，N={sampleCount}")
    End Sub

    ''' <summary>
    ''' 初始化模型参数
    ''' </summary>
    Private Sub InitializeParameters(X As Tensor, dimension As Integer)
        Dim sampleCount As Integer = X.Shape(0)
        Dim components As Integer = _nComponents

        ' 均匀权重。注意：这里绝不能用循环变量参与 1/K 的计算
        Dim weightsData As Double() = New Double(components - 1) {}

        For k As Integer = 0 To components - 1
            weightsData(k) = 1.0 / components
        Next

        Weights = New Tensor(weightsData, components)

        ' 初始化均值
        Dim meansData As Double() = New Double(components * dimension - 1) {}

        Select Case _initMethod
            Case InitMethod.Random
                For k As Integer = 0 To components - 1
                    Dim picked As Integer = _rng.NextIndex(sampleCount)
                    Dim offset As Integer = k * dimension

                    For d As Integer = 0 To dimension - 1
                        meansData(offset + d) = X(picked, d)
                    Next
                Next
            Case Else
                Call KMeansPlusPlus(X, dimension, meansData)
        End Select

        Means = New Tensor(meansData, components, dimension)

        ' 初始化协方差（使用正确的全局方差与协方差）
        Dim globalMean As Double() = Nothing
        Dim globalVariance As Double() = Nothing
        Call ComputeGlobalStatistics(X, dimension, globalMean, globalVariance)

        If IsDiagonalCovariance() Then
            Dim covData As Double() = New Double(components * dimension - 1) {}

            For k As Integer = 0 To components - 1
                Dim offset As Integer = k * dimension

                For d As Integer = 0 To dimension - 1
                    covData(offset + d) = std.Max(globalVariance(d) + _regCovar, _options.CovarianceFloor)
                Next
            Next

            Covariances = New Tensor(covData, components, dimension)
        Else
            Dim globalCovariance As Tensor = GlobalCovariance(X, dimension, globalMean)
            Dim buffer As Double() = globalCovariance.Data
            Dim covData As Double() = New Double(components * dimension * dimension - 1) {}

            For k As Integer = 0 To components - 1
                Dim offset As Integer = k * dimension * dimension

                For j As Integer = 0 To dimension * dimension - 1
                    covData(offset + j) = buffer(j)
                Next

                For d As Integer = 0 To dimension - 1
                    Dim index As Integer = offset + d * dimension + d
                    covData(index) = std.Max(covData(index) + _regCovar, _options.CovarianceFloor)
                Next
            Next

            Covariances = New Tensor(covData, components, dimension, dimension)
        End If
    End Sub

    ''' <summary>K-Means++ 风格的均值初始化。</summary>
    Private Sub KMeansPlusPlus(X As Tensor, dimension As Integer, meansData As Double())
        Dim sampleCount As Integer = X.Shape(0)
        Dim components As Integer = _nComponents
        Dim xData As Double() = X.Data

        Dim chosen As Integer() = New Integer(components - 1) {}
        chosen(0) = _rng.NextIndex(sampleCount)

        For d As Integer = 0 To dimension - 1
            meansData(d) = xData(chosen(0) * dimension + d)
        Next

        If components = 1 Then
            Return
        End If

        Dim nearest As Double() = New Double(sampleCount - 1) {}
        For i As Integer = 0 To sampleCount - 1
            nearest(i) = Double.MaxValue
        Next

        For k As Integer = 1 To components - 1
            Dim centerRow As Integer = chosen(k - 1)
            Dim total As Double = 0

            For i As Integer = 0 To sampleCount - 1
                Dim acc As Double = 0
                Dim rowOffset As Integer = i * dimension
                Dim centerOffset As Integer = centerRow * dimension

                For d As Integer = 0 To dimension - 1
                    Dim diff As Double = xData(rowOffset + d) - xData(centerOffset + d)
                    acc += diff * diff
                Next

                If acc < nearest(i) Then
                    nearest(i) = acc
                End If

                total += nearest(i)
            Next

            Dim picked As Integer = _rng.NextIndex(sampleCount)

            If total > 0 Then
                Dim threshold As Double = _rng.NextDouble() * total
                Dim cumulative As Double = 0

                For i As Integer = 0 To sampleCount - 1
                    cumulative += nearest(i)

                    If cumulative >= threshold Then
                        picked = i
                        Exit For
                    End If
                Next
            End If

            chosen(k) = picked

            Dim offset As Integer = k * dimension
            For d As Integer = 0 To dimension - 1
                meansData(offset + d) = xData(picked * dimension + d)
            Next
        Next
    End Sub

    Private Shared Sub ComputeGlobalStatistics(X As Tensor, dimension As Integer,
                                               ByRef globalMean As Double(), ByRef globalVariance As Double())
        Dim sampleCount As Integer = X.Shape(0)
        Dim xData As Double() = X.Data

        globalMean = New Double(dimension - 1) {}

        For i As Integer = 0 To sampleCount - 1
            Dim offset As Integer = i * dimension

            For d As Integer = 0 To dimension - 1
                globalMean(d) += xData(offset + d)
            Next
        Next

        For d As Integer = 0 To dimension - 1
            globalMean(d) /= sampleCount
        Next

        globalVariance = New Double(dimension - 1) {}

        For i As Integer = 0 To sampleCount - 1
            Dim offset As Integer = i * dimension

            For d As Integer = 0 To dimension - 1
                Dim diff As Double = xData(offset + d) - globalMean(d)
                globalVariance(d) += diff * diff
            Next
        Next

        For d As Integer = 0 To dimension - 1
            globalVariance(d) /= std.Max(1, sampleCount - 1)
        Next
    End Sub

    Private Shared Function GlobalCovariance(X As Tensor, dimension As Integer, globalMean As Double()) As Tensor
        Dim sampleCount As Integer = X.Shape(0)
        Dim xData As Double() = X.Data
        Dim covData As Double() = New Double(dimension * dimension - 1) {}

        For i As Integer = 0 To sampleCount - 1
            Dim offset As Integer = i * dimension

            For r As Integer = 0 To dimension - 1
                Dim diffR As Double = xData(offset + r) - globalMean(r)

                For c As Integer = r To dimension - 1
                    Dim diffC As Double = xData(offset + c) - globalMean(c)
                    covData(r * dimension + c) += diffR * diffC
                Next
            Next
        Next

        Dim denominator As Double = std.Max(1, sampleCount - 1)

        For r As Integer = 0 To dimension - 1
            For c As Integer = r To dimension - 1
                Dim value As Double = covData(r * dimension + c) / denominator
                covData(r * dimension + c) = value
                covData(c * dimension + r) = value
            Next

            covData(r * dimension + r) += LinearAlgebraOps.MinVariance
        Next

        Return New Tensor(covData, dimension, dimension)
    End Function

    ''' <summary>
    ''' E 步：把 log(π_k) + log N(x | μ_k, Σ_k) 写入
    ''' <paramref name="logResp"/>，再在原位减去每行的 log-sum-exp。
    ''' 返回数据集的对数似然（求和）。
    ''' </summary>
    Private Function EStepCore(X As Tensor, logResp As Double(,)) As Double
        Dim sampleCount As Integer = X.Shape(0)
        Dim dimension As Integer = X.Shape(1)
        Dim components As Integer = _nComponents
        Dim xData As Double() = X.Data
        Dim meansData As Double() = Means.Data
        Dim weightsData As Double() = Weights.Data

        Dim logWeight As Double() = New Double(components - 1) {}
        For k As Integer = 0 To components - 1
            logWeight(k) = LogSumExp.SafeLog(weightsData(k))
        Next

        Dim diff As Double() = New Double(dimension - 1) {}

        For k As Integer = 0 To components - 1
            Dim logDet As Double = ComponentLogDeterminant(k)
            Dim meanOffset As Integer = k * dimension

            For i As Integer = 0 To sampleCount - 1
                Dim rowOffset As Integer = i * dimension

                For d As Integer = 0 To dimension - 1
                    diff(d) = xData(rowOffset + d) - meansData(meanOffset + d)
                Next

                logResp(i, k) = LinearAlgebraOps.LogPdf(dimension, logDet, MahalanobisSquaredAt(k, diff)) + logWeight(k)
            Next
        Next

        Dim logLikelihood As Double = 0

        For i As Integer = 0 To sampleCount - 1
            Dim maxValue As Double = Double.NegativeInfinity

            For k As Integer = 0 To components - 1
                If logResp(i, k) > maxValue Then
                    maxValue = logResp(i, k)
                End If
            Next

            If Double.IsNegativeInfinity(maxValue) Then
                ' 全部为 -Infinity：退化为均匀分配，避免产生 NaN
                Dim uniform As Double = -std.Log(components)

                For k As Integer = 0 To components - 1
                    logResp(i, k) = uniform
                Next

                Continue For
            End If

            Dim acc As Double = 0
            For k As Integer = 0 To components - 1
                acc += std.Exp(logResp(i, k) - maxValue)
            Next

            Dim norm As Double = maxValue + std.Log(acc)
            logLikelihood += norm

            For k As Integer = 0 To components - 1
                logResp(i, k) -= norm
            Next
        Next

        Return logLikelihood
    End Function

    ''' <summary>
    ''' M 步：根据责任度更新权重、均值与协方差，并处理空簇。
    ''' </summary>
    Private Sub MStepCore(X As Tensor, logResp As Double(,), resp As Double(,))
        Dim sampleCount As Integer = X.Shape(0)
        Dim dimension As Integer = X.Shape(1)
        Dim components As Integer = _nComponents
        Dim xData As Double() = X.Data

        Dim nk As Double() = New Double(components - 1) {}
        Dim maxResp As Double() = New Double(sampleCount - 1) {}

        For i As Integer = 0 To sampleCount - 1
            Dim top As Double = 0

            For k As Integer = 0 To components - 1
                Dim value As Double = std.Exp(logResp(i, k))
                resp(i, k) = value
                nk(k) += value

                If value > top Then
                    top = value
                End If
            Next

            maxResp(i) = top
        Next

        ' ---- 空簇重初始化：把最不适配的样本指派给退化成分 ----
        Dim reinitSample As Integer() = New Integer(components - 1) {}
        For k As Integer = 0 To components - 1
            reinitSample(k) = -1
        Next

        If _options.ReinitializeEmptyClusters Then
            For k As Integer = 0 To components - 1
                If nk(k) >= _options.EmptyClusterSize Then
                    Continue For
                End If

                Dim worst As Integer = -1
                Dim worstValue As Double = Double.MaxValue

                For i As Integer = 0 To sampleCount - 1
                    If maxResp(i) >= 0 AndAlso maxResp(i) < worstValue Then
                        worstValue = maxResp(i)
                        worst = i
                    End If
                Next

                If worst < 0 Then
                    Exit For
                End If

                reinitSample(k) = worst
                maxResp(worst) = -1
                nk(k) = 1.0
            Next
        End If

        ' ---- 混合权重 ----
        Dim weightsData As Double() = New Double(components - 1) {}
        Dim total As Double = 0

        For k As Integer = 0 To components - 1
            weightsData(k) = std.Max(nk(k), _options.WeightFloor)
            total += weightsData(k)
        Next

        If total <= 0 Then
            total = 1
        End If

        For k As Integer = 0 To components - 1
            weightsData(k) /= total
        Next

        Weights = New Tensor(weightsData, components)

        ' ---- 均值 ----
        Dim meansData As Double() = New Double(components * dimension - 1) {}

        For k As Integer = 0 To components - 1
            Dim offset As Integer = k * dimension

            If reinitSample(k) >= 0 Then
                Dim rowOffset As Integer = reinitSample(k) * dimension

                For d As Integer = 0 To dimension - 1
                    meansData(offset + d) = xData(rowOffset + d)
                Next

                Continue For
            End If

            Dim denominator As Double = std.Max(nk(k), _options.WeightFloor)

            For d As Integer = 0 To dimension - 1
                Dim acc As Double = 0

                For i As Integer = 0 To sampleCount - 1
                    acc += resp(i, k) * xData(i * dimension + d)
                Next

                meansData(offset + d) = acc / denominator
            Next
        Next

        Means = New Tensor(meansData, components, dimension)

        ' ---- 协方差 ----
        Dim hasReinit As Boolean = False

        For k As Integer = 0 To components - 1
            If reinitSample(k) >= 0 Then
                hasReinit = True
                Exit For
            End If
        Next

        Dim fallback As Tensor = Nothing

        If hasReinit Then
            Dim globalMean As Double() = Nothing
            Dim globalVariance As Double() = Nothing
            Call ComputeGlobalStatistics(X, dimension, globalMean, globalVariance)
            fallback = GlobalCovariance(X, dimension, globalMean)
        End If

        If IsDiagonalCovariance() Then
            Dim covData As Double() = New Double(components * dimension - 1) {}
            Dim fallbackData As Double() = If(fallback Is Nothing, Nothing, fallback.Data)

            For k As Integer = 0 To components - 1
                Dim offset As Integer = k * dimension
                Dim denominator As Double = std.Max(nk(k), _options.WeightFloor)

                For d As Integer = 0 To dimension - 1
                    If reinitSample(k) >= 0 Then
                        covData(offset + d) = std.Max(fallbackData(d * dimension + d), _options.CovarianceFloor)
                        Continue For
                    End If

                    Dim acc As Double = 0
                    Dim mean As Double = meansData(offset + d)

                    For i As Integer = 0 To sampleCount - 1
                        Dim weight As Double = resp(i, k)

                        If weight > 0 Then
                            Dim diff As Double = xData(i * dimension + d) - mean
                            acc += weight * diff * diff
                        End If
                    Next

                    covData(offset + d) = std.Max(acc / denominator + _regCovar, _options.CovarianceFloor)
                Next
            Next

            ' Tied / Spherical 的后处理
            Select Case _options.CovarianceType
                Case GmmCovarianceType.Tied
                    For d As Integer = 0 To dimension - 1
                        Dim pooled As Double = 0

                        For k As Integer = 0 To components - 1
                            pooled += weightsData(k) * covData(k * dimension + d)
                        Next

                        For k As Integer = 0 To components - 1
                            covData(k * dimension + d) = pooled
                        Next
                    Next
                Case GmmCovarianceType.Spherical
                    Dim scalar As Double = 0

                    For d As Integer = 0 To dimension - 1
                        For k As Integer = 0 To components - 1
                            scalar += weightsData(k) * covData(k * dimension + d)
                        Next
                    Next

                    scalar /= dimension

                    For k As Integer = 0 To components * dimension - 1
                        covData(k) = scalar
                    Next
            End Select

            Covariances = New Tensor(covData, components, dimension)
        Else
            Dim covData As Double() = New Double(components * dimension * dimension - 1) {}
            Dim fallbackData As Double() = If(fallback Is Nothing, Nothing, fallback.Data)

            For k As Integer = 0 To components - 1
                Dim offset As Integer = k * dimension * dimension
                Dim denominator As Double = std.Max(nk(k), _options.WeightFloor)

                If reinitSample(k) >= 0 Then
                    For j As Integer = 0 To dimension * dimension - 1
                        covData(offset + j) = fallbackData(j)
                    Next

                    For d As Integer = 0 To dimension - 1
                        covData(offset + d * dimension + d) += _regCovar
                    Next

                    Continue For
                End If

                For r As Integer = 0 To dimension - 1
                    Dim meanR As Double = meansData(offset + r)

                    For c As Integer = r To dimension - 1
                        Dim meanC As Double = meansData(offset + c)
                        Dim acc As Double = 0

                        For i As Integer = 0 To sampleCount - 1
                            Dim weight As Double = resp(i, k)

                            If weight > 0 Then
                                acc += weight *
                                       (xData(i * dimension + r) - meanR) *
                                       (xData(i * dimension + c) - meanC)
                            End If
                        Next

                        Dim value As Double = acc / denominator

                        If r = c Then
                            value = std.Max(value + _regCovar, _options.CovarianceFloor)
                        End If

                        covData(offset + r * dimension + c) = value
                        covData(offset + c * dimension + r) = value
                    Next
                Next
            Next

            Dim covarianceTensor As Tensor = New Tensor(covData, components, dimension, dimension)

            Select Case _options.CovarianceType
                Case GmmCovarianceType.Tied
                    covarianceTensor = ShareCovariance(covarianceTensor, weightsData, dimension, components)
                Case GmmCovarianceType.Full
                    ' 无需后处理
            End Select

            Covariances = covarianceTensor
        End If

        Call BuildFactors()
    End Sub

    ''' <summary>把所有成分的协方差按有效样本比例合并为共享协方差。</summary>
    Private Shared Function ShareCovariance(source As Tensor, weightsData As Double(),
                                            dimension As Integer, components As Integer) As Tensor
        Dim data As Double() = source.Data
        Dim pooled As Double() = New Double(dimension * dimension - 1) {}

        For k As Integer = 0 To components - 1
            Dim offset As Integer = k * dimension * dimension

            For j As Integer = 0 To dimension * dimension - 1
                pooled(j) += weightsData(k) * data(offset + j)
            Next
        Next

        Dim result As Double() = New Double(components * dimension * dimension - 1) {}

        For k As Integer = 0 To components - 1
            Dim offset As Integer = k * dimension * dimension

            For j As Integer = 0 To dimension * dimension - 1
                result(offset + j) = pooled(j)
            Next
        Next

        Return New Tensor(result, components, dimension, dimension)
    End Function

#End Region

#Region "协方差与 Cholesky 因子"

    Private Function IsDiagonalCovariance() As Boolean
        Return _options.CovarianceType = GmmCovarianceType.Diagonal OrElse
               _options.CovarianceType = GmmCovarianceType.Spherical
    End Function

    ''' <summary>提取第 k 个成分的完整协方差矩阵 [D, D]。</summary>
    Private Function ComponentCovariance(component As Integer) As Tensor
        Dim dimension As Integer = InputDim
        Dim data As Double() = New Double(dimension * dimension - 1) {}

        For r As Integer = 0 To dimension - 1
            For c As Integer = 0 To dimension - 1
                data(r * dimension + c) = Covariances(component, r, c)
            Next
        Next

        Return New Tensor(data, dimension, dimension)
    End Function

    ''' <summary>在 Cholesky 全部失败时用对角线构造一个对角因子。</summary>
    Private Shared Function DiagonalFactor(matrix As Tensor) As Tensor
        Dim dimension As Integer = matrix.Shape(0)
        Dim data As Double() = New Double(dimension * dimension - 1) {}

        For d As Integer = 0 To dimension - 1
            data(d * dimension + d) = std.Sqrt(std.Max(matrix(d, d), LinearAlgebraOps.MinVariance))
        Next

        Return New Tensor(data, dimension, dimension)
    End Function

    ''' <summary>为全协方差模式重建每个成分的 Cholesky 因子。</summary>
    Private Sub BuildFactors()
        If Means Is Nothing OrElse Covariances Is Nothing Then
            Return
        End If

        If IsDiagonalCovariance() Then
            _factors = Nothing
            Return
        End If

        Dim components As Integer = _nComponents
        _factors = New Tensor(components - 1) {}

        For k As Integer = 0 To components - 1
            Dim matrix As Tensor = ComponentCovariance(k)
            Dim factor As Tensor = LinearAlgebraOps.CholeskyFactor(matrix)

            If factor Is Nothing Then
                Dim jitter As Double = 0
                factor = LinearAlgebraOps.CholeskyFactorWithJitter(matrix, jitter)
            End If

            If factor Is Nothing Then
                factor = DiagonalFactor(matrix)
            End If

            _factors(k) = factor
        Next
    End Sub

    Private Sub EnsureFactors()
        If IsDiagonalCovariance() Then
            Return
        End If

        If _factors Is Nothing Then
            Call BuildFactors()
        End If
    End Sub

    Private Function ComponentLogDeterminant(component As Integer) As Double
        Dim dimension As Integer = InputDim

        If IsDiagonalCovariance() Then
            Dim acc As Double = 0

            For d As Integer = 0 To dimension - 1
                acc += std.Log(std.Max(Covariances(component, d), LinearAlgebraOps.MinVariance))
            Next

            Return acc
        End If

        Return LinearAlgebraOps.LogDeterminantFromFactor(_factors(component))
    End Function

    Private Function MahalanobisSquaredAt(component As Integer, diff As Double()) As Double
        If IsDiagonalCovariance() Then
            Dim acc As Double = 0

            For d As Integer = 0 To diff.Length - 1
                Dim variance As Double = std.Max(Covariances(component, d), LinearAlgebraOps.MinVariance)
                acc += diff(d) * diff(d) / variance
            Next

            Return acc
        End If

        Return LinearAlgebraOps.MahalanobisSquared(_factors(component), diff)
    End Function

#End Region

#Region "概率密度计算"

    ''' <summary>
    ''' 计算所有样本在所有成分下的对数高斯概率密度
    ''' </summary>
    ''' <param name="X">输入数据 [N, D]</param>
    ''' <returns>对数概率密度 [N, K]</returns>
    Public Function ComputeLogGaussianPDF(X As Tensor) As Tensor
        Call EnsureFitted()
        Call ValidateInput(X, allowUnfitted:=False)

        Dim sampleCount As Integer = X.Shape(0)
        Dim dimension As Integer = X.Shape(1)
        Dim components As Integer = _nComponents
        Dim xData As Double() = X.Data
        Dim meansData As Double() = Means.Data

        Dim result As New Tensor(sampleCount, components)
        Dim buffer As Double() = result.Data
        Dim diff As Double() = New Double(dimension - 1) {}

        Call EnsureFactors()

        For k As Integer = 0 To components - 1
            Dim logDet As Double = ComponentLogDeterminant(k)
            Dim meanOffset As Integer = k * dimension

            For i As Integer = 0 To sampleCount - 1
                Dim rowOffset As Integer = i * dimension

                For d As Integer = 0 To dimension - 1
                    diff(d) = xData(rowOffset + d) - meansData(meanOffset + d)
                Next

                buffer(i * components + k) = LinearAlgebraOps.LogPdf(dimension, logDet, MahalanobisSquaredAt(k, diff))
            Next
        Next

        Return result
    End Function

#End Region

#Region "预测与采样"

    ''' <summary>
    ''' 预测每个样本属于每个成分的责任度 (后验概率)
    ''' </summary>
    ''' <param name="X">输入数据 [N, D]</param>
    ''' <returns>责任度矩阵 [N, K]</returns>
    Public Function PredictProba(X As Tensor) As Tensor Implements IProbabilisticClusterer.PredictProba
        Call EnsureFitted()
        Call ValidateInput(X, allowUnfitted:=False)
        Call EnsureFactors()

        Dim sampleCount As Integer = X.Shape(0)
        Dim logResp As Double(,) = New Double(sampleCount - 1, _nComponents - 1) {}

        Call EStepCore(X, logResp)

        Dim result As New Tensor(sampleCount, _nComponents)
        Dim buffer As Double() = result.Data

        For i As Integer = 0 To sampleCount - 1
            For k As Integer = 0 To _nComponents - 1
                buffer(i * _nComponents + k) = std.Exp(logResp(i, k))
            Next
        Next

        Return result
    End Function

    ''' <summary>
    ''' 预测每个样本最可能属于的成分 (硬聚类)
    ''' </summary>
    ''' <param name="X">输入数据 [N, D]</param>
    ''' <returns>标签数组 [N]</returns>
    Public Function Predict(X As Tensor) As Integer() Implements IProbabilisticClusterer.Predict
        Dim resp As Tensor = PredictProba(X)
        Dim sampleCount As Integer = X.Shape(0)
        Dim components As Integer = _nComponents
        Dim buffer As Double() = resp.Data
        Dim labels As Integer() = New Integer(sampleCount - 1) {}

        For i As Integer = 0 To sampleCount - 1
            Dim maxValue As Double = Double.NegativeInfinity
            Dim maxIndex As Integer = 0
            Dim offset As Integer = i * components

            For k As Integer = 0 To components - 1
                If buffer(offset + k) > maxValue Then
                    maxValue = buffer(offset + k)
                    maxIndex = k
                End If
            Next

            labels(i) = maxIndex
        Next

        Return labels
    End Function

    ''' <summary>
    ''' 计算给定数据的平均对数似然
    ''' </summary>
    Public Function Score(X As Tensor) As Double
        Call EnsureFitted()
        Call ValidateInput(X, allowUnfitted:=False)
        Call EnsureFactors()

        Dim sampleCount As Integer = X.Shape(0)
        Dim logResp As Double(,) = New Double(sampleCount - 1, _nComponents - 1) {}

        Return EStepCore(X, logResp) / sampleCount
    End Function

    ''' <summary>
    ''' 计算给定数据点处的对数概率密度 ln p(x) = ln Σ_k π_k N(x | μ_k, Σ_k)
    ''' </summary>
    ''' <param name="X">输入数据 [N, D]</param>
    ''' <returns>对数概率密度 [N]</returns>
    Public Function ScoreSamples(X As Tensor) As Tensor
        Dim logDensity As Tensor = ComputeLogGaussianPDF(X)
        Dim sampleCount As Integer = X.Shape(0)
        Dim components As Integer = _nComponents
        Dim buffer As Double() = logDensity.Data
        Dim weightsData As Double() = Weights.Data
        Dim logWeight As Double() = New Double(components - 1) {}

        For k As Integer = 0 To components - 1
            logWeight(k) = LogSumExp.SafeLog(weightsData(k))
        Next

        Dim result As New Tensor(sampleCount)
        Dim output As Double() = result.Data

        For i As Integer = 0 To sampleCount - 1
            Dim maxValue As Double = Double.NegativeInfinity
            Dim offset As Integer = i * components

            For k As Integer = 0 To components - 1
                Dim value As Double = buffer(offset + k) + logWeight(k)

                If value > maxValue Then
                    maxValue = value
                End If
            Next

            Dim acc As Double = 0

            For k As Integer = 0 To components - 1
                acc += std.Exp(buffer(offset + k) + logWeight(k) - maxValue)
            Next

            output(i) = maxValue + std.Log(acc)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 从拟合好的模型中采样生成数据
    ''' </summary>
    ''' <param name="n">采样数量</param>
    ''' <param name="seed">随机种子</param>
    ''' <returns>采样数据 [n, D]</returns>
    Public Function Sample(n As Integer, Optional seed As Integer? = Nothing) As Tensor
        Call EnsureFitted()

        If n <= 0 Then
            Throw New ArgumentException("采样数量必须大于 0", NameOf(n))
        End If

        Dim dimension As Integer = InputDim
        Dim components As Integer = _nComponents
        Dim rng As RandomGaussian = If(seed.HasValue, New RandomGaussian(seed.Value), _rng)

        If rng Is Nothing Then
            rng = New RandomGaussian()
        End If

        Call EnsureFactors()

        Dim result As New Tensor(n, dimension)
        Dim output As Double() = result.Data
        Dim weightsData As Double() = Weights.Data
        Dim meansData As Double() = Means.Data

        Dim cumulative As Double() = New Double(components - 1) {}
        Dim acc As Double = 0

        For k As Integer = 0 To components - 1
            acc += weightsData(k)
            cumulative(k) = acc
        Next

        Dim mean As Double() = New Double(dimension - 1) {}

        For i As Integer = 0 To n - 1
            Dim threshold As Double = rng.NextDouble() * acc
            Dim selected As Integer = components - 1

            For k As Integer = 0 To components - 1
                If threshold <= cumulative(k) Then
                    selected = k
                    Exit For
                End If
            Next

            Dim meanOffset As Integer = selected * dimension
            For d As Integer = 0 To dimension - 1
                mean(d) = meansData(meanOffset + d)
            Next

            Dim value As Double()

            If IsDiagonalCovariance() Then
                value = New Double(dimension - 1) {}

                For d As Integer = 0 To dimension - 1
                    Dim variance As Double = std.Max(Covariances(selected, d), LinearAlgebraOps.MinVariance)
                    value(d) = mean(d) + std.Sqrt(variance) * rng.NextNormal()
                Next
            Else
                ' 完整保留协方差的相关性结构：x = μ + L·z
                value = LinearAlgebraOps.SampleGaussian(rng, mean, _factors(selected))
            End If

            Dim rowOffset As Integer = i * dimension
            For d As Integer = 0 To dimension - 1
                output(rowOffset + d) = value(d)
            Next
        Next

        Return result
    End Function

    Private Function SampleFromPrior(count As Integer, seed As Integer?) As Tensor Implements IPriorSampler.Sample
        Return Sample(count, seed)
    End Function

#End Region

#Region "线性代数辅助函数"

    ''' <summary>
    ''' 矩阵求逆。内部委托到 <see cref="LinearAlgebraOps"/>，即
    ''' NumericMatrix + JAMA 实现，奇异时按对角线抖动回退。
    ''' </summary>
    Public Shared Function MatrixInverse(mat As Tensor, n As Integer) As Tensor
        If mat Is Nothing Then
            Throw New ArgumentNullException(NameOf(mat))
        End If

        If mat.Rank <> 2 OrElse mat.Shape(0) <> n OrElse mat.Shape(1) <> n Then
            Throw New ArgumentException($"矩阵求逆要求 {n}x{n} 的方阵，实际为 [{String.Join(", ", mat.Shape)}]")
        End If

        Return LinearAlgebraOps.Inverse(mat)
    End Function

    ''' <summary>
    ''' 计算矩阵的 ln|A|。奇异矩阵返回 <see cref="Double.NegativeInfinity"/>。
    ''' 内部委托到 <see cref="LinearAlgebraOps"/>（Cholesky 优先，LU 回退）。
    ''' </summary>
    Public Shared Function LogDeterminant(mat As Tensor, n As Integer) As Double
        If mat Is Nothing Then
            Throw New ArgumentNullException(NameOf(mat))
        End If

        If mat.Rank <> 2 OrElse mat.Shape(0) <> n OrElse mat.Shape(1) <> n Then
            Throw New ArgumentException($"行列式要求 {n}x{n} 的方阵，实际为 [{String.Join(", ", mat.Shape)}]")
        End If

        Return LinearAlgebraOps.LogDeterminant(mat)
    End Function

#End Region

#Region "持久化"

    '<inheritdoc/>
    Private ReadOnly Property SchemaVersionValue As Integer Implements IModelPersistable.SchemaVersion
        Get
            Return ModelFileSchemaVersion
        End Get
    End Property

    ''' <summary>把模型参数写入指定路径。</summary>
    Public Sub Save(path As String) Implements IModelPersistable.Save
        Call EnsureFitted()

        Dim parameters As New ModelParameters("GaussianMixtureModel", ModelFileSchemaVersion)

        Call parameters.Add("weights", Weights)
        Call parameters.Add("means", Means)
        Call parameters.Add("covariances", Covariances)
        Call parameters.AddValue("nComponents", _nComponents)
        Call parameters.AddValue("inputDim", InputDim)
        Call parameters.AddValue("covarianceType", CInt(_options.CovarianceType))
        Call parameters.AddValue("regCovar", _regCovar)
        Call parameters.AddValue("sampleCount", _sampleCount)
        Call parameters.AddValue("logLikelihood", LogLikelihood)
        Call parameters.AddValue("converged", If(Converged, 1.0, 0.0))
        Call parameters.AddValue("nIter", NIter)
        Call parameters.Save(path)
    End Sub

    ''' <summary>从指定路径还原模型参数。</summary>
    Public Function TryLoad(path As String, ByRef errorMessage As String) As Boolean Implements IModelPersistable.TryLoad
        errorMessage = Nothing

        Try
            Dim parameters As ModelParameters = ModelParameters.Load(path)

            If Not String.Equals(parameters.ModelType, "GaussianMixtureModel", StringComparison.Ordinal) Then
                errorMessage = $"模型类型不符: 期望 GaussianMixtureModel，实际 {parameters.ModelType}"
                Return False
            End If

            Dim components As Integer = CInt(parameters.GetValue("nComponents", 0))
            Dim dimension As Integer = CInt(parameters.GetValue("inputDim", 0))

            If components <= 0 OrElse dimension <= 0 Then
                errorMessage = "模型文件缺少有效的维度信息"
                Return False
            End If

            Dim covarianceKind As Integer = CInt(parameters.GetValue("covarianceType", 0))
            Dim diagonal As Boolean = covarianceKind = CInt(GmmCovarianceType.Diagonal) OrElse
                                      covarianceKind = CInt(GmmCovarianceType.Spherical)

            Dim weights As Tensor = parameters.RequireTensor("weights", New Integer() {components})
            Dim means As Tensor = parameters.RequireTensor("means", New Integer() {components, dimension})
            Dim covariances As Tensor

            If diagonal Then
                covariances = parameters.RequireTensor("covariances", New Integer() {components, dimension})
            Else
                covariances = parameters.RequireTensor("covariances", New Integer() {components, dimension, dimension})
            End If

            _nComponents = components
            _options = New GmmOptions(CType(covarianceKind, GmmCovarianceType))
            _diagCovariance = diagonal
            _regCovar = parameters.GetValue("regCovar", _regCovar)
            _sampleCount = CInt(parameters.GetValue("sampleCount", 0))

            Weights = weights
            Means = means
            Covariances = covariances

            LogLikelihood = parameters.GetValue("logLikelihood", Double.NegativeInfinity)
            Converged = parameters.GetValue("converged", 0) > 0.5
            NIter = CInt(parameters.GetValue("nIter", 0))

            _fitted = True
            _factors = Nothing

            Call BuildFactors()

            Return True
        Catch ex As Exception
            errorMessage = ex.Message
            Return False
        End Try
    End Function

#End Region

#Region "辅助方法"

    Private Sub EnsureFitted()
        If Not _fitted OrElse Means Is Nothing OrElse Weights Is Nothing OrElse Covariances Is Nothing Then
            Throw New InvalidOperationException("模型尚未训练, 请先调用 Fit 方法")
        End If
    End Sub

    Private Sub ValidateInput(X As Tensor, allowUnfitted As Boolean)
        If X Is Nothing Then
            Throw New ArgumentNullException(NameOf(X))
        End If

        If X.Rank <> 2 Then
            Throw New ArgumentException("输入数据必须是二维张量 [N, D]")
        End If

        If X.Shape(0) <= 0 Then
            Throw New ArgumentException("输入数据不得为空")
        End If

        If Not allowUnfitted Then
            Dim dimension As Integer = InputDim

            If dimension > 0 AndAlso X.Shape(1) <> dimension Then
                Throw New ArgumentException($"输入维度不符: 期望 {dimension}, 实际 {X.Shape(1)}")
            End If
        End If
    End Sub

    ''' <summary>
    ''' 获取第 k 个成分的均值向量 (1D 张量)
    ''' </summary>
    Public Function GetComponentMean(k As Integer) As Tensor
        Call EnsureFitted()

        Dim dimension As Integer = InputDim
        Dim data As Double() = New Double(dimension - 1) {}

        For d As Integer = 0 To dimension - 1
            data(d) = Means(k, d)
        Next

        Return New Tensor(data, dimension)
    End Function

    ''' <summary>
    ''' 获取第 k 个成分的方差向量 (对角协方差情况)
    ''' </summary>
    Public Function GetComponentVariance(k As Integer) As Tensor
        Call EnsureFitted()

        Dim dimension As Integer = InputDim
        Dim data As Double() = New Double(dimension - 1) {}

        For d As Integer = 0 To dimension - 1
            If IsDiagonalCovariance() Then
                data(d) = Covariances(k, d)
            Else
                data(d) = Covariances(k, d, d)
            End If
        Next

        Return New Tensor(data, dimension)
    End Function

    ''' <summary>
    ''' 获取第 k 个成分的标准差向量
    ''' </summary>
    Public Function GetComponentStdDev(k As Integer) As Tensor
        Dim variance As Tensor = GetComponentVariance(k)
        Dim data As Double() = New Double(variance.Length - 1) {}

        For i As Integer = 0 To data.Length - 1
            data(i) = std.Sqrt(std.Max(variance.Data(i), 0))
        Next

        Return New Tensor(data, data.Length)
    End Function

    Public Overrides Function ToString() As String
        Return $"GMM(K={_nComponents}, diagCov={_diagCovariance}, converged={Converged}, nIter={NIter}, logLik={LogLikelihood:F4})"
    End Function

    Public Shared Function Predicts(rowdatas() As ClusterEntity, components As Integer, threshold As Double, strict As Boolean) As GaussianMixtureModel
        If rowdatas Is Nothing OrElse rowdatas.Length = 0 Then
            Throw New ArgumentException("输入数据不得为空", NameOf(rowdatas))
        End If

        Dim model As New GaussianMixtureModel(components, tol:=threshold)
        Dim width As Integer = rowdatas(0).Length
        Dim samples As Double() = New Double(rowdatas.Length * width - 1) {}

        For i As Integer = 0 To rowdatas.Length - 1
            Array.Copy(rowdatas(i).entityVector, 0, samples, i * width, width)
        Next

        Dim X As New Tensor(samples, rowdatas.Length, width)
        Call model.Fit(X)

        Return model
    End Function

    Public Shared Function Predicts(samples() As Double, components As Integer, threshold As Double, verbose As Boolean) As GaussianMixtureModel
        Dim model As New GaussianMixtureModel(components, tol:=threshold)
        Dim X As New Tensor(samples, samples.Length, 1)

        Call model.Fit(X)

        Return model
    End Function

#End Region

#Region "显式接口实现"

    Private ReadOnly Property ScoreLogLikelihood As Double Implements IModelScore.LogLikelihood
        Get
            Return LogLikelihood
        End Get
    End Property

    Private ReadOnly Property ScoreParameters As Integer Implements IModelScore.ParametersCount
        Get
            Return NParameters
        End Get
    End Property

#End Region

End Class
