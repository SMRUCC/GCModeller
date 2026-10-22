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
        ''' 弹性网络(L1 + L2 正则化)，兼具 LASSO 的稀疏性与 Ridge 的稳定性。
        ''' </summary>
        ''' <remarks>
        ''' L2 惩罚项在基因高度共线的时候给出分组效应(一组相关的基因要么一起被保留、
        ''' 要么一起被剔除)，并且允许自变量个数大于样本数量，因此特别适合
        ''' 基因表达数据之上的调控网络推断。
        ''' 
        ''' 具体的超参数(``l1_ratio``、alpha 的确定方式)由
        ''' <see cref="GrnFittingOptions.elasticNet"/> 控制；该方法同样不提供
        ''' 显著性检验，其 ``pvalue`` 统一写为 ``1``。
        ''' </remarks>
        ElasticNet = 2
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
        ''' 弹性网络的超参数配置，只在 <see cref="FittingMethod.ElasticNet"/> 模式下生效。
        ''' </summary>
        ''' <returns></returns>
        Public Property elasticNet As New ElasticNetOptions()

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
        ''' 这个上限是必须的：参与拟合的自变量个数应当显著小于样本数量，否则
        ''' <see cref="FittingMethod.MultipleLinear"/> 的 QR 分解会因为秩不足而失效，
        ''' 而 DataFittings 的 LASSO 求解器也会因为内部数组的维度缺陷而越界崩溃。
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
        ''' 需要<b>优先</b>纳入候选调控因子池的基因集合(例如转录因子列表)。
        ''' 
        ''' 与 <see cref="regulators"/> 的"限定"语义不同：这里列出的基因不受
        ''' <see cref="maxCandidates"/> 与 <see cref="minAbsCorrelation"/> 的截断影响，
        ''' 总是会被追加进每一个目标基因的候选集(目标基因自身除外)。
        ''' 
        ''' 为空的时候不产生任何影响。大小写不敏感，表达矩阵之中不存在的基因会被忽略。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorityRegulators As String() = Nothing

        ''' <summary>
        ''' 优先注入的基因数量上限，默认 0(不限制)。
        ''' 
        ''' <see cref="priorityRegulators"/> 列出 1600+ 个基因的时候，每个目标基因的
        ''' 候选数会从 <see cref="maxCandidates"/> 膨胀到 1900 左右，而求解器要为每个
        ''' 基因分配 ``p x n`` 的列数组(大对象堆)。并发较高时这会触发
        ''' <c>OutOfMemoryException</c>，表现为大量基因"没有任何调控关系"。
        ''' 把本属性设为与 <see cref="maxCandidates"/> 相同的量级，可以避免候选集爆炸。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorityBudget As Integer = 0

        ''' <summary>
        ''' curated 数据库之中的 <c>TF -> target</c> 调控关系先验，默认空。
        ''' 
        ''' 提供之后会启用两项机制：
        ''' 
        ''' 1. <b>候选注入</b>：对于某个目标基因，先验中指向它的 TF 会被追加进候选集，
        '''    即使它被 top-k 相关性排名挤出了名单也会保留下来；
        ''' 2. <b>自适应惩罚</b>：先验支持的边在 L1 惩罚上乘以 <see cref="priorPenalty"/>，
        '''    在稀疏化过程中更容易存活。
        ''' 
        ''' 先验为空时，拟合行为与不提供先验时<b>逐位一致</b>。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorNetwork As PriorEdge() = Nothing

        ''' <summary>
        ''' 采纳先验的最低置信度，默认 0.9(即只使用 High 置信度的边)。
        ''' 
        ''' 低于该阈值的边既不参与候选注入，也不参与惩罚放松。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorMinConfidence As Double = 0.9R

        ''' <summary>
        ''' 先验支持的边所享有的 L1 惩罚乘子，默认 1.0(不放松)。
        ''' 
        ''' 取值越小，先验边越容易被保留；0.3 是一个稳健的默认选择
        ''' (先验提供倾斜，但仍然由数据决定去留)。设为 1.0 等价于关闭方案 B。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorPenalty As Double = 1.0R

        ''' <summary>
        ''' 是否使用<b>偏相关</b>(而不是零阶 Pearson)来给候选调控因子排序，默认关闭。
        ''' 
        ''' 开启之后，会先找出表达谱最"像全局程序"的若干个 hub 基因
        ''' (与随机抽样基因的平均 |r| 最大的一批，通常是线粒体/核糖体基因)，
        ''' 把每个基因对这批 hub 做线性回归、取残差，再在残差上计算相关。
        ''' 
        ''' 这等价于以 hub 基因为条件集的偏相关(Frisch-Waugh-Lovell 残差回归，
        ''' 与 <c>WGCNA/PartialCorrelation.vb::FromData</c> 使用的是同一个算法)，
        ''' 可以显著削弱"所有基因共享同一个全局因子"造成的伪边。
        ''' 
        ''' 注意：只有当候选池大于 <see cref="maxCandidates"/> 或者设置了
        ''' <see cref="minAbsCorrelation"/> 的时候才会触发候选筛选；
        ''' 候选池本身很小(例如限定为 300 个靶基因)时本选项不起作用。
        ''' </summary>
        ''' <returns></returns>
        Public Property partialCorrelation As Boolean = False

        ''' <summary>
        ''' <see cref="partialCorrelation"/> 开启时，用作条件集的 hub 基因数量，默认 5。
        ''' </summary>
        ''' <returns></returns>
        Public Property partialConditionSize As Integer = 5

        ''' <summary>
        ''' 为<b>负相关</b>的候选调控因子保留的名额，默认 -1(不启用)。
        ''' 
        ''' 共表达网络里负相关的边极少，L1 会优先保留强正相关的候选，
        ''' 于是敲降一个基因只会把整片基因一起拖下去(全下凋)。
        ''' 设置成例如 15 之后，每个目标基因的候选集里会强制包含 15 个负相关基因
        ''' (其余名额仍给正相关)，网络里才会出现抑制型边，扰动响应才可能同时有升有降。
        ''' 
        ''' 该配额只在触发候选筛选的时候生效(候选池大于 <see cref="maxCandidates"/>
        ''' 或设置了 <see cref="minAbsCorrelation"/>)。
        ''' </summary>
        ''' <returns></returns>
        Public Property negativeCandidateQuota As Integer = -1

        ''' <summary>
        ''' 候选调控因子的相关性筛选后端。
        ''' 
        ''' 为空(默认)的时候使用内置的逐基因扫描(行为与原实现逐位一致)；
        ''' 非空的时候由后端一次性算出每个目标基因的候选集，
        ''' 从而把 "每个基因各扫一遍全表" 降为一次分块矩阵乘法。
        ''' 
        ''' 后端的 <c>Initialize</c> 会在 <see cref="GeneRegulatoryNetwork.Fit"/> 开始时
        ''' 被调用一次，<c>Query</c> 会被并行拟合过程并发调用，因此实现必须是线程安全的只读。
        ''' </summary>
        ''' <returns></returns>
        Public Property correlationBackend As ICorrelationScreener = Nothing

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

            If priorityRegulators IsNot Nothing AndAlso priorityRegulators.Length > 0 Then
                regs &= $", {priorityRegulators.Length} priority regulators"
            End If
            Dim summary As String = $"[{method}] candidates={If(maxCandidates > 0, maxCandidates.ToString, $"auto({candidateSampleRatio})")}, " &
                   $"|r|>={minAbsCorrelation}, regulators={regs}, standardize={standardize}"

            If method = FittingMethod.ElasticNet Then
                summary &= ", " & elasticNet.ToString
            End If

            Return summary
        End Function
    End Class
End Namespace
