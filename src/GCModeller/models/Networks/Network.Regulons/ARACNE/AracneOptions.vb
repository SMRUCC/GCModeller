Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem.Core

Namespace ARACNE

    ''' <summary>
    ''' 互信息(Mutual Information)的估计方法。
    ''' 
    ''' 不同的估计方法在“估计精度 / 计算开销”这条轴上面处于不同的位置，
    ''' 依据表达矩阵的规模选择合适的方法即可。
    ''' </summary>
    Public Enum MiEstimator

        ''' <summary>
        ''' 高斯 copula 估计器(默认)：假设 ``(X, Y)`` 服从二元正态分布，
        ''' 先对表达量做中心化与标准化(或者秩转换)，再计算 pearson 相关系数 ``r``，
        ''' 则
        ''' 
        ''' ```
        ''' MI(X, Y) = -0.5 * log2(1 - r^2)
        ''' ```
        ''' 
        ''' 这是 readme 之中推荐的快速估计方法：任意一个基因对的互信息都退化为一次
        ''' 长度为样本数的向量内积 ``O(n)``，配合 SIMD 与多核可以轻松处理
        ''' ``10^6 ~ 10^7`` 数量级的基因对，因此它是大样本单细胞数据的默认路径。
        ''' </summary>
        GaussianCopula = 0

        ''' <summary>
        ''' 自适应分区估计器(ARACNE 原论文 Algorithm 1)。
        ''' 
        ''' 从整个散点图开始，反复把“点数最多的 bin”沿着极差更大的那个轴
        ''' 在中位数处分半，直到 bin 的数量达到上限或者每个 bin 内部的点数
        ''' 低于下限；最后用联合/边缘计数计算
        ''' 
        ''' ```
        ''' MI(X, Y) = sum_ij p_ij * log2(p_ij / (p_i * p_j))
        ''' ```
        ''' 
        ''' 由于分区的过程与表达量的具体尺度无关，这个估计器可以捕捉到
        ''' 线性的 copula 估计器所无法捕捉的<b>非线性</b>依赖关系，
        ''' 是真正意义上的 ARACNE 互信息估计器，代价是每个基因对需要
        ''' ``O(n log n)`` 级别的计算量。
        ''' </summary>
        AdaptivePartitioning = 1

        ''' <summary>
        ''' 分位数等频直方图估计器：把两个变量分别按照分位数切成相同数量的
        ''' 等频 bin，再用联合/边缘计数计算互信息，并且做 Miller-Madow 偏差校正。
        ''' 
        ''' 这是 readme 伪代码第二步之中“按分位数离散化”的最直接实现，
        ''' 计算量介于 <see cref="AdaptivePartitioning"/> 与 <see cref="GaussianCopula"/> 之间。
        ''' </summary>
        EqualFrequencyHistogram = 2

        ''' <summary>
        ''' 高斯核密度估计器：使用高斯核分别估计联合密度与两个边缘密度，
        ''' 再用 ``MI = (1/n) * sum log2(f(x,y) / (f(x)f(y)))`` 计算互信息
        ''' (带宽使用 Silverman 经验公式，并且采用留一法避免样本自身的偏置)。
        ''' 
        ''' 它的精度最高，但是每个基因对的计算量是 ``O(n^2)``，
        ''' 因此只有当样本数量不超过 <see cref="AracneOptions.kernelMaxSamples"/>
        ''' 的时候才会被真正使用，否则会退回 <see cref="GaussianCopula"/>
        ''' 并且给出警告信息。
        ''' </summary>
        KernelDensity = 3
    End Enum

    ''' <summary>
    ''' 互信息显著性阈值 ``tau`` 的确定方式。
    ''' </summary>
    Public Enum MiThresholdMode

        ''' <summary>
        ''' 置换零分布(默认)：把每一个基因的表达向量沿着样本维度独立地随机打乱，
        ''' 破坏基因之间的依赖关系但是保留各自的边缘分布，然后重新计算互信息，
        ''' 得到“没有任何调控关系”的零分布；取其分位数(默认 95%)作为 ``tau``。
        ''' 
        ''' 这样就可以避免硬编码阈值：不同数据集的样本数量、离散化粒度、
        ''' 估计器偏置都会被零分布同样地“抵消”掉一部分。
        ''' </summary>
        Permutation = 0

        ''' <summary>
        ''' 使用 <see cref="AracneOptions.miThreshold"/> 之中给定的固定阈值。
        ''' </summary>
        Fixed = 1
    End Enum

    ''' <summary>
    ''' 数据处理不等式(Data Processing Inequality)的执行策略。
    ''' </summary>
    Public Enum DpiMode

        ''' <summary>
        ''' 不做 DPI 过滤，直接把阈值过滤之后的互信息网络作为结果。
        ''' </summary>
        Off = 0

        ''' <summary>
        ''' 标准的 ARACNE DPI：对于三元组 ``(i, j, k)``，如果最弱的边
        ''' ``MI(i, j)`` 同时比另外两条边低超过容差比例 ``epsilon``，则判定
        ''' ``(i, j)`` 是一条由 ``k`` 传递出来的间接边并予以剔除。
        ''' </summary>
        Standard = 1

        ''' <summary>
        ''' 结合 TF 先验知识的 DPI(readme 第四章第 3 节)：
        ''' 
        ''' + 如果三元组之中最弱的边连接的是<b>两个非转录因子</b>，则即使它只是
        '''   略微弱于另外两条边也坚决剔除(容差取 0)；
        ''' + 如果最弱的边涉及到转录因子，则按照 <see cref="AracneOptions.tolerance"/>
        '''   给出的容差决定去留(接近平局的时候保留)。
        ''' </summary>
        TfGuided = 2
    End Enum

    ''' <summary>
    ''' ARACNE 输出的边的方向赋予策略。
    ''' </summary>
    Public Enum DirectionMode

        ''' <summary>
        ''' 保持 ARACNE 的原始语义：输出无向边。
        ''' </summary>
        Undirected = 0

        ''' <summary>
        ''' 利用 TF 先验列表给边赋予方向(readme 第四章第 3 节)：
        ''' 
        ''' + ``u in TF 且 v not in TF`` ==> ``u -> v``
        ''' + 两个端点都是 TF ==> 双向边(``TF-TF`` 调控环)
        ''' + 两个端点都不是 TF ==> 保留为无向的候选边
        ''' </summary>
        TfDirected = 1
    End Enum

    ''' <summary>
    ''' curated 先验边在 DPI 判定之中所受保护的<b>分级方式</b>。
    ''' 
    ''' 三种模式都只影响“保护强度”，先验边的阈值放宽
    ''' (<see cref="AracneOptions.priorThresholdFactor"/>)与 top-K 保护是独立的。
    ''' </summary>
    Public Enum PriorProtectionMode

        ''' <summary>
        ''' 不做 DPI 保护：先验边与普通边一样，只按容差 ``epsilon`` 判定。
        ''' </summary>
        Off = 0

        ''' <summary>
        ''' 置信度分级保护(默认，确定性、可复现)。
        ''' 
        ''' 置信度 ``c`` 的先验边获得的 DPI 容差为
        ''' 
        ''' ```
        ''' eps_c = eps + (1 - eps) * priorProtectionStrength * c
        ''' ```
        ''' 
        ''' 也就是置信度越高，允许它“比另外两条边弱”的幅度越大：
        ''' High 的边很难被 DPI 删掉，Medium 次之，Low 只在证据几乎持平的时候才被保护。
        ''' 由于没有随机性，同一份数据多次运行的结果完全一致。
        ''' </summary>
        Graded = 1

        ''' <summary>
        ''' 按置信度做<b>概率保护</b>：每条先验边以 ``p = c`` 的概率获得完全保护
        ''' (被抽中的边永远不被 DPI 删除)，以 ``1 - p`` 的概率按标准容差判定。
        ''' 
        ''' 即：High(0.95) 几乎总被保护、Medium(0.70) 约七成概率被保护、
        ''' Low(0.45) 只有不到一半的概率被保护。
        ''' 
        ''' 抽样使用 <see cref="AracneOptions.randomSeed"/> 驱动的伪随机数，
        ''' 因此结果可复现，而不是每次运行都不同。
        ''' </summary>
        Sampled = 2
    End Enum

    ''' <summary>
    ''' ARACNE 网络推断的参数配置。
    ''' 
    ''' 所有的参数都带有合理的默认值，无参构造即可跑通完整的流程：
    ''' 
    ''' ```
    ''' MI(高斯 copula) -> 置换阈值(tau) -> DPI 去间接边 -> TF 定向
    ''' ```
    ''' </summary>
    Public Class AracneOptions

        ''' <summary>
        ''' 互信息的估计方法，默认为 <see cref="MiEstimator.GaussianCopula"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property miEstimator As MiEstimator = MiEstimator.GaussianCopula

        ''' <summary>
        ''' <see cref="MiEstimator.AdaptivePartitioning"/> 所使用的 bin 数量上限，
        ''' 小于或者等于零的时候按照样本数量自动推算 ``round(sqrt(n))``
        ''' (结果被限制在 ``[4, 64]`` 之内)。
        ''' </summary>
        ''' <returns></returns>
        Public Property adaptiveMaxBins As Integer = 0

        ''' <summary>
        ''' <see cref="MiEstimator.AdaptivePartitioning"/> 之中每个 bin 的最小点数，
        ''' 小于或者等于零的时候自动推算。
        ''' </summary>
        ''' <returns></returns>
        Public Property adaptiveMinPoints As Integer = 0

        ''' <summary>
        ''' <see cref="MiEstimator.EqualFrequencyHistogram"/> 所使用的等频 bin 数量，默认 4。
        ''' </summary>
        ''' <returns></returns>
        Public Property histogramBins As Integer = 4

        ''' <summary>
        ''' <see cref="MiEstimator.KernelDensity"/> 的高斯核带宽，
        ''' 小于或者等于零的时候使用 Silverman 经验公式自动推算。
        ''' </summary>
        ''' <returns></returns>
        Public Property kernelBandwidth As Double = 0

        ''' <summary>
        ''' <see cref="MiEstimator.KernelDensity"/> 允许的最大样本数量(默认 2000)。
        ''' 
        ''' 核密度估计的计算量是 ``O(n^2)``，超过这个数量的时候会随机抽取样本
        ''' (固定随机种子，结果可复现)或者退回
        ''' <see cref="MiEstimator.GaussianCopula"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property kernelMaxSamples As Integer = 2000

        ''' <summary>
        ''' 互信息显著性阈值 ``tau`` 的确定方式，默认 <see cref="MiThresholdMode.Permutation"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property thresholdMode As MiThresholdMode = MiThresholdMode.Permutation

        ''' <summary>
        ''' <see cref="MiThresholdMode.Fixed"/> 模式之下的互信息阈值，默认 0(不做过滤)。
        ''' </summary>
        ''' <returns></returns>
        Public Property miThreshold As Double = 0

        ''' <summary>
        ''' 置换零分布所采样的基因对数量，小于或者等于零的时候自动推算。
        ''' </summary>
        ''' <returns></returns>
        Public Property permutationPairs As Integer = 0

        ''' <summary>
        ''' 置换零分布所取的分位数，默认 0.95(readme 建议 95% 或者 99%)。
        ''' </summary>
        ''' <returns></returns>
        Public Property permutationQuantile As Double = 0.95

        ''' <summary>
        ''' 置换零分布之中每个基因对的打乱次数，默认 1。
        ''' 
        ''' 每一次打乱都会对基因对的<b>两个</b>表达向量分别独立洗牌，
        ''' 因此即使取 1 也已经完全破坏了基因之间的依赖结构。
        ''' </summary>
        ''' <returns></returns>
        Public Property permutationReplicates As Integer = 1

        ''' <summary>
        ''' DPI 的容差比例 ``epsilon``，默认 0.1(与 ARACNE 论文一致)：
        ''' 最弱的边必须比另外两条边都低超过 10% 才会被判定为间接边，
        ''' 接近平局的三元组会保留下来。
        ''' </summary>
        ''' <returns></returns>
        Public Property tolerance As Double = 0.1R

        ''' <summary>
        ''' DPI 的执行策略。
        ''' 
        ''' 为空(默认)的时候根据是否提供了 TF 列表自动选择：
        ''' 提供了 TF 列表则使用 <see cref="DpiMode.TfGuided"/>，否则使用
        ''' <see cref="DpiMode.Standard"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property dpiMode As DpiMode? = Nothing

        ''' <summary>
        ''' DPI 最多迭代多少轮，默认 5 轮。
        ''' 
        ''' 每一轮内部都是基于当轮开始时的邻接关系做判定(与参考实现一致的“单轮判定”语义)，
        ''' 被剔除的边会在该轮结束之后统一生效；如果某一轮没有任何边被剔除，
        ''' 或者已经达到轮数上限，则停止迭代。
        ''' </summary>
        ''' <returns></returns>
        Public Property dpiMaxPasses As Integer = 5

        ''' <summary>
        ''' 边的方向赋予策略，默认 <see cref="DirectionMode.TfDirected"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property directionMode As DirectionMode = DirectionMode.TfDirected

        ''' <summary>
        ''' 转录因子列表(基因编号，大小写不敏感)。
        ''' 
        ''' 提供之后会启用三项优化(readme 第四章)：
        ''' 
        ''' 1. <b>计算空间缩减</b>：互信息只需要计算 ``TF x All`` 与 ``TF-TF``，
        '''    计算量由 ``O(N^2)`` 降为 ``O(T * N)``；
        ''' 2. <b>TF 导向的 DPI</b>：见 <see cref="DpiMode.TfGuided"/>；
        ''' 3. <b>方向赋予</b>：见 <see cref="DirectionMode.TfDirected"/>。
        ''' 
        ''' 为空或者为空数组的时候按全基因 ``N x N`` 的上三角计算。
        ''' </summary>
        ''' <returns></returns>
        Public Property transcriptionFactors As String() = Nothing

        ''' <summary>
        ''' 是否对表达量做 ``log2(x + 1)`` 变换，默认关闭
        ''' (从 <see cref="Matrix"/> 读入的数据通常已经是 log 尺度)。
        ''' </summary>
        ''' <returns></returns>
        Public Property log2Transform As Boolean = False

        ''' <summary>
        ''' 是否在计算互信息之前把每一个基因的表达向量标准化(z-score)，默认关闭。
        ''' 
        ''' 高斯 copula 估计器内部总是会做中心化与标准化(等价于计算 pearson 相关)，
        ''' 因此这个开关主要影响离散化/核密度估计器。
        ''' </summary>
        ''' <returns></returns>
        Public Property standardize As Boolean = False

        ''' <summary>
        ''' 是否把表达量转换成秩(rank)之后再计算互信息，默认关闭。
        ''' 
        ''' 开启之后 copula 估计器等于计算 spearman 相关，对表达量的单调变换
        ''' 与离群值更加稳健(单细胞数据的零膨胀会让 pearson 相关明显偏低)。
        ''' </summary>
        ''' <returns></returns>
        Public Property rankTransform As Boolean = False

        ''' <summary>
        ''' 只保留在至少这个比例的样本之中有表达的基因，默认 0(不做过滤)。
        ''' </summary>
        ''' <returns></returns>
        Public Property minNonZeroRatio As Double = 0

        ''' <summary>
        ''' 按照方差保留变化最剧烈的若干个基因，默认 0(保留全部)。
        ''' 
        ''' 真实的单细胞表达矩阵动辄两万个基因，全部纳入推断既慢又没有意义，
        ''' 建议取 1000 ~ 3000。
        ''' 
        ''' 注意：<see cref="transcriptionFactors"/> 之中列出的基因<b>不受</b>这个上限的
        ''' 截断(转录因子是调控网络的起点，即使方差排名靠后也必须保留)，
        ''' 因此最终保留的基因数量可能略多于本参数。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxGenes As Integer = 0

        ''' <summary>
        ''' 每一个调控因子最多保留多少条最强的边(top-K 剪枝)，默认 0(不剪枝)。
        ''' 
        ''' 剪枝可以显著压低 DPI 三元组的数量以及内存占用，
        ''' 建议在大规模数据之上取 100 ~ 500。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxNeighbors As Integer = 0

        ''' <summary>
        ''' curated 数据库之中的 ``TF -&gt; TargetGene`` 调控关系先验(可选)。
        ''' 
        ''' 提供之后会启用四项基于先验的算法优化(readme 第四章的进一步工程化)：
        ''' 
        ''' 1. <b>计算空间补全</b>：先验之中出现、但是转录因子列表没有覆盖的调控因子
        '''    会被自动补充进 TF 集合(由 <see cref="priorTfAugment"/> 控制)，
        '''    避免因为 TF 注释不全而漏掉真实的调控因子；
        ''' 2. <b>阈值放宽</b>：先验支持的基因对即使互信息低于置换阈值 ``tau``，
        '''    只要高于 ``tau * priorThresholdFactor`` 也会被保留下来，
        '''    用于召回被严格阈值切掉的弱调控关系；
        ''' 3. <b>top-K 保护</b>：先验支持的边不受 <see cref="maxNeighbors"/> 剪枝的影响；
        ''' 4. <b>DPI 保护</b>：先验支持的边不会被 DPI 判定为间接边而剔除，
        '''    这正是 ARACNE 的一个已知缺陷(反馈环路或者协同调控之中，
        '''    真实的直接边可能弱于传递边)。
        ''' 
        ''' 另外，先验的因果符号(激活 / 抑制)会被写入输出边的
        ''' <see cref="AracneEdge.regulation"/> 字段。
        ''' 
        ''' 需要强调的是：上述保护与放宽的<b>强度</b>是按先验的置信度分级的
        ''' (见 <see cref="priorProtectionMode"/> 与 <see cref="priorProtectionStrength"/>)，
        ''' 高置信度的边享有更大范围的保护窗口，低置信度的边只享有很窄的窗口，
        ''' 数据证据明显不足的时候仍然会被 DPI 剔除。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorNetwork As RegulatoryEdge() = Nothing

        ''' <summary>
        ''' 参与推断的最低先验置信度(资格门槛)，默认 0.45(即 Low 及以上)。
        ''' 
        ''' 低于这个门槛的先验关系会被完全忽略(既不放宽阈值，也不保护)；
        ''' 达到门槛之后，保护的<i>强度</i>仍然与置信度成正比，见
        ''' <see cref="priorProtectionMode"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorMinConfidence As Double = 0.45R

        ''' <summary>
        ''' 先验支持的边所使用的放宽阈值系数(置信度取满时的值)，默认 0.5。
        ''' 
        ''' 置信度为 ``c`` 的先验边，其实际阈值是
        ''' 
        ''' ```
        ''' tau_prior(c) = tau * (1 - (1 - priorThresholdFactor) * c)
        ''' ```
        ''' 
        ''' 也就是说：``c = 1`` 的时候门槛降到 ``tau * priorThresholdFactor``，
        ''' 而置信度很低的先验几乎享受不到放宽(``tau_prior ~ tau``)。
        ''' 取 1 表示完全不放宽，取 0 表示满置信度的先验只要互信息为正就保留。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorThresholdFactor As Double = 0.5R

        ''' <summary>
        ''' 是否保护先验支持的边不被 DPI 剔除，默认开启。
        ''' 
        ''' 关闭等价于 <see cref="PriorProtectionMode.Off"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property protectPriorEdges As Boolean = True

        ''' <summary>
        ''' 先验边在 DPI 判定之中所受保护的<b>分级方式</b>，默认
        ''' <see cref="PriorProtectionMode.Graded"/>。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorProtectionMode As PriorProtectionMode = PriorProtectionMode.Graded

        ''' <summary>
        ''' 分级保护的强度，取值 0~1，默认 0.5。
        ''' 
        ''' 定义 DPI 的“保护窗口”：记标准容差为 ``eps``、先验置信度为 ``c``，则
        ''' 
        ''' ```
        ''' 标准容差      : eps
        ''' 保护后的容差  : eps + (1 - eps) * priorProtectionStrength * c
        ''' ```
        ''' 
        ''' 一条先验边只有在“比另外两条边弱超过这个容差”的时候才会被 DPI 剔除，
        ''' 也就是说置信度越高，允许它偏离直接关系证据的幅度越大：
        ''' 
        ''' + ``c = 0.95`` (High)、``strength = 0.5``：容差从 0.10 放宽到约 0.53；
        ''' + ``c = 0.70`` (Medium)：容差放宽到约 0.42；
        ''' + ``c = 0.45`` (Low)：容差放宽到约 0.30。
        ''' 
        ''' 取 1 表示满置信度的先验边<b>永远不会</b>被 DPI 剔除(二值保护)，
        ''' 取 0 表示只按标准容差判定(等价于不保护)。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorProtectionStrength As Double = 0.5R

        ''' <summary>
        ''' 是否把先验之中的调控因子自动补充进 TF 集合，默认开启。
        ''' 
        ''' 转录因子注释(TF 列表)本身也可能不完整，而 curated 先验之中的调控因子
        ''' 从定义上就是转录因子，因此把它们并入 TF 集合可以同时改善
        ''' 计算空间的覆盖度与 DPI/定向的语义。
        ''' </summary>
        ''' <returns></returns>
        Public Property priorTfAugment As Boolean = True

        ''' <summary>
        ''' 并行计算互信息的最大并发度，小于或者等于零的时候由运行时决定。
        ''' </summary>
        ''' <returns></returns>
        Public Property maxDegreeOfParallelism As Integer = -1

        ''' <summary>
        ''' 置换抽样与核密度抽样所使用的随机种子，固定种子保证结果可以复现。
        ''' </summary>
        ''' <returns></returns>
        Public Property randomSeed As Integer = 1234567

        ''' <summary>
        ''' 是否关闭推断过程之中的进度与阶段统计输出。
        ''' </summary>
        ''' <returns></returns>
        Public Property silent As Boolean = False

        ''' <summary>
        ''' 有效的最小样本数量要求，样本数少于这个数值的时候直接抛出异常。
        ''' </summary>
        ''' <returns></returns>
        Public Property minSamples As Integer = 20

        ''' <summary>
        ''' 创建一份默认参数的副本
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function DefaultOptions() As AracneOptions
            Return New AracneOptions()
        End Function

        ''' <summary>
        ''' 根据参数配置决定实际生效的 DPI 模式
        ''' </summary>
        ''' <returns></returns>
        Public Function EffectiveDpiMode() As DpiMode
            If dpiMode.HasValue Then
                Return dpiMode.Value
            ElseIf transcriptionFactors IsNot Nothing AndAlso transcriptionFactors.Length > 0 Then
                Return ARACNE.DpiMode.TfGuided
            Else
                Return ARACNE.DpiMode.Standard
            End If
        End Function

        ''' <summary>
        ''' 打印当前的参数配置，便于在日志之中追溯分析结果。
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim tfs As String = If(transcriptionFactors Is Nothing, "(none)", $"{transcriptionFactors.Length} transcription factors")

            Dim prior As String = If(priorNetwork Is Nothing OrElse priorNetwork.Length = 0,
                "(none)",
                $"{priorNetwork.Length} prior edges (conf>={priorMinConfidence.ToString("G2")}, factor={priorThresholdFactor.ToString("G2")})")

            Return $"ARACNE[mi={miEstimator}, tau={thresholdMode}" &
                If(thresholdMode = MiThresholdMode.Fixed, $"({miThreshold.ToString("G4")})", $"({permutationQuantile.ToString("P0")})") &
                $", dpi={EffectiveDpiMode()}(eps={tolerance.ToString("G3")}), direction={directionMode}, tfs={tfs}, prior={prior}]"
        End Function
    End Class
End Namespace
