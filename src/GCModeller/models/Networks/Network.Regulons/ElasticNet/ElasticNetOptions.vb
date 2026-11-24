Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 弹性网络超参数的确定方式
    ''' </summary>
    Public Enum ElasticNetMode

        ''' <summary>
        ''' 固定 l1_ratio，求解整条正则化路径之后，在「非零系数数量不超过每基因上限」
        ''' 的步骤之中挑选 R^2 最高的那一步。
        ''' 
        ''' 这是最快的模式，单个基因的成本与 LASSO 的正则化路径求解相当。
        ''' </summary>
        RegularizationPath = 0

        ''' <summary>
        ''' 每一个目标基因都单独做一次 K 折交叉验证，自动选择 (alpha, l1_ratio)。
        ''' 
        ''' 这是最准确的模式，但是单个基因的成本大约是路径模式的
        ''' ``l1_ratio 网格数量 × alpha 步数 × 折数`` 倍，只适合基因数量很少的数据。
        ''' </summary>
        PerGeneCrossValidation = 1

        ''' <summary>
        ''' 先挑选若干代表性基因(表达方差最大的那些)做一次交叉验证，
        ''' 取超参数的中位数供全部基因复用，然后逐基因做单点拟合。
        ''' 
        ''' 精度与成本之间的折中方案，推荐在基因数量很多的数据上面使用。
        ''' </summary>
        GlobalCrossValidation = 2
    End Enum

    ''' <summary>
    ''' 弹性网络(Elastic Net)拟合基因调控网络的参数
    ''' 
    ''' 弹性网络在最小二乘的损失之上同时加入了 L1 与 L2 惩罚：
    ''' 
    ''' ```
    ''' beta = argmin { ||y - X*beta||^2 / (2n) + alpha * [ l1_ratio * ||beta||_1 + (1 - l1_ratio) * ||beta||_2^2 / 2 ] }
    ''' ```
    ''' 
    ''' 其中 L1 项给出稀疏的调控关系，L2 项则在基因高度共线的时候给出分组效应
    ''' (相关的基因要么一起被保留、要么一起被剔除)，并且允许自变量个数大于样本数量。
    ''' </summary>
    Public Class ElasticNetOptions

        ''' <summary>
        ''' 超参数的确定方式，默认为正则化路径选步。
        ''' </summary>
        ''' <returns></returns>
        Public Property mode As ElasticNetMode = ElasticNetMode.RegularizationPath

        ''' <summary>
        ''' L1 惩罚所占的混合比例，取值 [0, 1]。
        ''' 
        ''' 0 表示纯 Ridge(不会产生稀疏解)，1 表示纯 LASSO。
        ''' 基因表达数据之中存在大量共调控的基因，一般建议取 0.7 附近；
        ''' 特征数量远大于样本数量的时候可以提高到 0.7~0.9。
        ''' </summary>
        ''' <returns></returns>
        Public Property l1Ratio As Double = 0.7

        ''' <summary>
        ''' 正则化路径上面的 alpha 步数(alpha 由大到小排列)
        ''' </summary>
        ''' <returns></returns>
        Public Property numberOfAlphas As Integer = 20

        ''' <summary>
        ''' 单个基因模型的 |系数| 之和上限，默认 0(不限制)。
        ''' 
        ''' 线性网络做虚拟扰动的时候，如果某一行的 |β| 之和大于 1，级联迭代的谱半径
        ''' 就可能超过 1，扰动会被指数放大成"全基因组剧烈变化"的假象。
        ''' 设为 0.9 之后，每一行都是压缩映射(下游变化不会超过上游变化的 0.9 倍)，
        ''' 传播稳定、幅度可控；由于只是按比例缩小系数，R² 与边集都不变。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxRowSum As Double = 0.0

        ''' <summary>
        ''' 正则化路径的终止比例，即最小的 alpha 等于最大的 alpha 乘以这个比例
        ''' </summary>
        ''' <returns></returns>
        Public Property alphaRatio As Double = 0.001

        ''' <summary>
        ''' 坐标下降的收敛容差(系数更新的最大变化量)
        ''' </summary>
        ''' <returns></returns>
        Public Property tolerance As Double = 0.0001

        ''' <summary>
        ''' 坐标下降每一步的最大迭代轮数
        ''' </summary>
        ''' <returns></returns>
        Public Property maxIterations As Integer = 10000

        ''' <summary>
        ''' 交叉验证的折数。样本数量更少的时候会自动收缩到样本数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property cvFolds As Integer = 5

        ''' <summary>
        ''' 交叉验证所搜索的 L1 混合比例网格。
        ''' 为空的时候使用 ElasticNet 库的默认值(0.1, 0.5, 0.7, 0.9, 0.95, 0.99, 1.0)。
        ''' </summary>
        ''' <returns></returns>
        Public Property cvL1Ratios As Double() = Nothing

        ''' <summary>
        ''' 交叉验证之中每一个 L1 比例下的 alpha 路径步数
        ''' </summary>
        ''' <returns></returns>
        Public Property cvAlphaSteps As Integer = 30

        ''' <summary>
        ''' 全局交叉验证模式之下用于估计超参数的代表性基因的数量
        ''' (按照表达方差从大到小挑选)
        ''' </summary>
        ''' <returns></returns>
        Public Property globalCVReplicates As Integer = 20

        ''' <summary>
        ''' 每一个目标基因允许纳入的最大候选调控因子数量(默认 300)。
        ''' 
        ''' 弹性网络本身没有「自变量个数必须小于样本数量」的限制，
        ''' 因此这里是一个纯粹用于控制内存与计算量的预算：
        ''' 求解器每一次都会复制一份 ``n x p`` 的设计矩阵，
        ''' 单次拟合的成本与它近似成正比。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxCandidates As Integer = 300

        ''' <summary>
        ''' 校验参数的取值范围，非法的时候抛出 <see cref="ArgumentException"/>
        ''' </summary>
        Public Sub Validate()
            If Double.IsNaN(l1Ratio) OrElse l1Ratio < 0.0R OrElse l1Ratio > 1.0R Then
                Throw New ArgumentException($"the l1 ratio of the elastic net must be in the range [0, 1], but {l1Ratio} was given!", NameOf(l1Ratio))
            End If
            If numberOfAlphas < 1 Then
                Throw New ArgumentException("the alpha path of the elastic net requires at least 1 step!", NameOf(numberOfAlphas))
            End If
            If Double.IsNaN(alphaRatio) OrElse alphaRatio <= 0.0R OrElse alphaRatio >= 1.0R Then
                Throw New ArgumentException($"the alpha ratio must be in the range (0, 1), but {alphaRatio} was given!", NameOf(alphaRatio))
            End If
            If Double.IsNaN(tolerance) OrElse tolerance <= 0.0R Then
                Throw New ArgumentException("the tolerance of the coordinate descent must be a positive number!", NameOf(tolerance))
            End If
            If maxIterations < 1 Then
                Throw New ArgumentException("the max iteration number of the coordinate descent must be a positive integer!", NameOf(maxIterations))
            End If
            If cvFolds < 2 Then
                Throw New ArgumentException("the cross validation of the elastic net requires at least 2 folds!", NameOf(cvFolds))
            End If
            If cvAlphaSteps < 1 Then
                Throw New ArgumentException("the cross validation requires at least 1 alpha step!", NameOf(cvAlphaSteps))
            End If
            If globalCVReplicates < 1 Then
                Throw New ArgumentException("the global cross validation requires at least 1 representative gene!", NameOf(globalCVReplicates))
            End If
            If maxCandidates < 0 Then
                Throw New ArgumentException("the candidate budget of the elastic net can not be a negative number!", NameOf(maxCandidates))
            End If
        End Sub

        ''' <summary>
        ''' 当前样本数量是否足够进行交叉验证
        ''' </summary>
        ''' <param name="nSamples">样本数量</param>
        ''' <returns></returns>
        Public Function CanUseCrossValidation(nSamples As Integer) As Boolean
            Return nSamples >= 2
        End Function

        ''' <summary>
        ''' 把交叉验证的折数收缩到样本数量之内
        ''' </summary>
        ''' <param name="nSamples">样本数量</param>
        ''' <returns></returns>
        Public Function NormalizedFolds(nSamples As Integer) As Integer
            Return std.Max(2, std.Min(cvFolds, nSamples))
        End Function

        ''' <summary>
        ''' 参数摘要，便于在日志之中追溯分析结果
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim summary As String = $"[elastic net] mode={mode}, l1_ratio={l1Ratio:G4}, alphas={numberOfAlphas}"

            If mode <> ElasticNetMode.RegularizationPath Then
                summary &= $", cv={cvFolds}-folds"
            End If
            If mode = ElasticNetMode.GlobalCrossValidation Then
                summary &= $", replicates={globalCVReplicates}"
            End If

            Return summary & $", candidates<={maxCandidates}"
        End Function
    End Class
End Namespace
