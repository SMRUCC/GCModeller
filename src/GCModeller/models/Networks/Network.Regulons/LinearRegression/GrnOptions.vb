Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 用于建立基因调控网络的线性拟合求解方法。
    ''' 
    ''' 模型的形式统一为：
    ''' 
    ''' ```
    ''' target = const + beta_1 * regulator_1 + beta_2 * regulator_2 + ...
    ''' ```
    ''' 
    ''' 其中每一个基因都会轮流作为目标基因(target)被拟合一次。
    ''' </summary>
    Public Enum FittingMethod

        ''' <summary>
        ''' 普通最小二乘(OLS)，即多元线性回归，底层通过 QR 分解求解正规方程
        ''' ``beta = inv(X' * X) * X' * y``。
        ''' </summary>
        ''' <remarks>
        ''' 参与拟合的自变量个数(候选调控因子数量 + 截距项)必须小于样本数量，
        ''' 否则 QR 分解会因为秩不足而抛出 ``Matrix is rank deficient`` 异常。
        ''' 
        ''' 只有这种方法可以给出回归系数的 t 值与显著性 p 值。
        ''' </remarks>
        MultipleLinear = 0

        ''' <summary>
        ''' LASSO 回归(L1 正则化)，在拟合的同时把不重要的调控因子权重收缩为零，
        ''' 从而直接得到稀疏的调控网络。
        ''' </summary>
        ''' <remarks>
        ''' DataFittings 中的 LASSO 求解器内部会对数据进行中心化与标准化，
        ''' 输出的权重与截距已经被还原到了原始的表达量尺度，因此可以和
        ''' <see cref="MultipleLinear"/> 的结果直接比较。
        ''' 
        ''' LASSO 不提供显著性检验，其 ``pvalue`` 统一写为 ``1``。
        ''' </remarks>
        Lasso = 1

        ''' <summary>
        ''' 非负最小二乘(NNLS)，约束所有回归系数非负，即只保留激活(正向)类型的调控关系。
        ''' </summary>
        ''' <remarks>
        ''' NNLS 不带截距项，模型退化为 ``target = sum(beta_i * regulator_i)``，
        ''' 同时也不提供显著性检验。
        ''' </remarks>
        NonNegative = 2
    End Enum

    ''' <summary>
    ''' 基因调控网络的线性拟合参数
    ''' </summary>
    Public Class GrnFittingOptions

        ''' <summary>
        ''' 线性拟合的求解方法，默认为普通最小二乘(OLS)。
        ''' </summary>
        ''' <returns></returns>
        Public Property method As FittingMethod = FittingMethod.MultipleLinear

        ''' <summary>
        ''' 每一个目标基因最多纳入多少个候选调控因子。
        ''' 
        ''' 小于或者等于零的时候按照 <see cref="candidateSampleRatio"/> 自动推算。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxCandidates As Integer = -1

        ''' <summary>
        ''' 自动模式下，候选调控因子数量占样本数量的最大比例(默认为 1/3)。
        ''' 
        ''' 这个比例参数用于保证满足了的内在要求：参与拟合的自变量个数必须显著小于样本数量，
        ''' 否则 <see cref="FittingMethod.MultipleLinear"/> 的 QR 分解会失效，
        ''' 而 DataFittings 的 LASSO 求解器也会因为内部数组越界而崩溃。
        ''' </remarks>
        ''' </summary>
        ''' <returns></returns>
        Public Property candidateSampleRatio As Double = 1.0 / 3.0

        ''' <summary>
        ''' 候选调控因子与目标基因的 pearson 相关性的绝对值下限，
        ''' 低于这个阈值的基因不会被纳入设计矩阵(默认为 0，即不施加相关性过滤)。
        ''' </summary>
        ''' <returns></returns>
        Public Property minAbsCorrelation As Double = 0

        ''' <summary>
        ''' 可选的调控因子集合(例如转录因子列表)。
        ''' 
        ''' 为空的时候，所有的基因都可以作为候选调控因子；
        ''' 非空的时候，只有这个集合之内的基因才会被作为自变量参与拟合，
        ''' 而目标基因仍然可以是表达矩阵之中的任意一行。
        ''' </summary>
        ''' <returns></returns>
        Public Property regulators As String() = Nothing

        ''' <summary>
        ''' 使用 LASSO 求解的时候，每一个目标基因最多允许保留多少个非零的调控关系。
        ''' 小于或者等于零的时候不做限制。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxRegulatorsPerGene As Integer = 20

        ''' <summary>
        ''' 导出调控边的时候的回归系数绝对值阈值，小于这个阈值的边会被丢弃(默认为 0，即保留全部)。
        ''' </summary>
        ''' <returns></returns>
        Public Property minAbsWeight As Double = 0

        ''' <summary>
        ''' 是否只保留互为因果(|权重|在两个方向上都显著且符号一致)的调控关系。
        ''' 
        ''' 开启之后，只有在 ``A -&gt; B`` 与 ``B -&gt; A`` 两个方向都被拟合出来、
        ''' 并且回归系数同号的时候才会保留这条边，边的权重取两个方向的均值。
        ''' </summary>
        ''' <returns></returns>
        Public Property mutualOnly As Boolean = False

        ''' <summary>
        ''' 是否在进行拟合之前把每一个基因的表达向量标准化(z-score)。
        ''' 
        ''' 开启之后回归系数可以在不同基因之间横向比较，代价是丢失了原始的表达量尺度，
        ''' 同时模型的截距项在理论上会退化为零。
        ''' </summary>
        ''' <returns></returns>
        Public Property standardize As Boolean = False

        ''' <summary>
        ''' 是否计算回归系数的 t 值与显著性 p 值(仅对 <see cref="FittingMethod.MultipleLinear"/> 有效)。
        ''' </summary>
        ''' <returns></returns>
        Public Property computeStatistics As Boolean = True

        ''' <summary>
        ''' p 值计算的容差：小于这个数值的 p 值会被修正为零。
        ''' </summary>
        ''' <returns></returns>
        Public Property pvalueTolerance As Double = 0.0000000001

        ''' <summary>
        ''' 并行拟合的最大并发度，小于或者等于零的时候由运行时自行决定。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxDegreeOfParallelism As Integer = -1

        ''' <summary>
        ''' 是否关闭拟合过程之中的进度提示输出。
        ''' </summary>
        ''' <returns></returns>
        Public Property silent As Boolean = False

        ''' <summary>
        ''' 创建一份默认参数的副本
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function DefaultOptions() As GrnFittingOptions
            Return New GrnFittingOptions()
        End Function

        ''' <summary>
        ''' 打印当前的参数配置，便于在日志之中追溯分析结果。
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim regs As String = If(regulators Is Nothing, "(all genes)", $"{regulators.Length} regulator genes")

            Return $"[{method}] candidates={If(maxCandidates > 0, maxCandidates.ToString, $"auto({candidateSampleRatio})")}, " &
                   $"|r|>={minAbsCorrelation}, regulators={regs}, standardize={standardize}"
        End Function
    End Class
End Namespace
