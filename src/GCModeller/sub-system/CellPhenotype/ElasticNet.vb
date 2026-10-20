Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports LinearGrn = SMRUCC.genomics.Model.Network.Regulons.LinearRegression.GeneRegulatoryNetwork
Imports LinearGrnModel = SMRUCC.genomics.Model.Network.Regulons.LinearRegression.GeneRegressionModel
Imports LinearGrnOptions = SMRUCC.genomics.Model.Network.Regulons.LinearRegression.GrnFittingOptions
Imports std = System.Math

''' <summary>
''' 虚拟扰动的效应传播方式。
''' </summary>
Public Enum PropagationMode

    ''' <summary>
    ''' 沿调控边级联迭代直到稳态(默认)。
    ''' 
    ''' 被扰动的基因在每一轮迭代之中都会被强制回设到干预值(do 算子语义)，
    ''' 因此它不会被自己的上游调控因子"修复"回去；其余基因按照
    ''' ``x_t = c_t + sum(beta * x_r)`` 的 Jacobi 迭代更新。
    ''' 
    ''' 这是贝叶斯网络引擎"按拓扑序一次性联合采样"在有环线性图上面的等价物。
    ''' </summary>
    Cascade = 0

    ''' <summary>
    ''' 只计算一阶(直接)效应，不进行级联迭代。
    ''' 
    ''' 只有被扰动基因的直接靶基因会产生变化，等价于
    ''' ``delta_target = beta * delta_regulator``。结果保守且绝对不会发散。
    ''' </summary>
    OneHop = 1
End Enum

''' <summary>
''' 基于弹性网络线性调控网络的虚拟扰动参数。
''' </summary>
''' <remarks>
''' 扰动值采用 <b>乘性比例</b> 语义(与真实的 CRISPRi/CRISPRa 实验更接近)：
''' 
''' ```
''' Knockout        : target = 0
''' Knockdown       : target = KnockdownResidual * baseline
''' Overexpression  : target = OverexpressionFold * baseline
''' Custom          : target = spec.Value   (原始表达量尺度的绝对值)
''' ```
''' 
''' 这个约定与 <see cref="InterventionSpec.GetInterventionValue"/> 的
''' ``mean +/- k * SD`` 公式不同，因此同一个基因在本引擎与贝叶斯网络引擎
''' 上面的敲降/过表达效应量不能直接横向比较。
''' </remarks>
Public Class ElasticNetPerturbationOptions

    ''' <summary>
    ''' 效应传播方式，默认为级联到稳态。
    ''' </summary>
    ''' <returns></returns>
    Public Property Propagation As PropagationMode = PropagationMode.Cascade

    ''' <summary>
    ''' 级联迭代的最大步数(默认 50)。
    ''' 
    ''' 把它设置为 1 等价于 <see cref="PropagationMode.OneHop"/>。
    ''' 迭代会在相邻两轮的表达量最大变化小于 <see cref="Tolerance"/> 的时候提前结束。
    ''' </summary>
    ''' <returns></returns>
    Public Property Steps As Integer = 50

    ''' <summary>
    ''' Jacobi 迭代的阻尼系数，取值范围 (0, 1]，默认为 1。
    ''' 
    ''' 取值小于 1 的时候每一轮只施加一部分修正量，可以抑制强反馈环上面的振荡。
    ''' 
    ''' 注意：这个系数只影响级联过程，<see cref="ElasticNetInterventionResult.FirstOrderDelta"/>
    ''' 永远是未经阻尼的一阶效应。
    ''' </summary>
    ''' <returns></returns>
    Public Property Damping As Double = 1.0R

    ''' <summary>
    ''' 级联迭代的收敛阈值，即相邻两轮之间表达量的最大允许变化量(默认 1e-6)。
    ''' </summary>
    ''' <returns></returns>
    Public Property Tolerance As Double = 0.000001R

    ''' <summary>
    ''' 敲降之后保留下来的表达量比例，取值范围 [0, 1)，默认 0.2(即约 80% 的敲降)。
    ''' </summary>
    ''' <returns></returns>
    Public Property KnockdownResidual As Double = 0.2R

    ''' <summary>
    ''' 过表达之后的表达量相对于基线的倍数，默认 3。
    ''' </summary>
    ''' <returns></returns>
    Public Property OverexpressionFold As Double = 3.0R

    ''' <summary>
    ''' 是否把迭代过程之中预测出来的表达量裁剪回观测区间。
    ''' 
    ''' 线性模型可以给出负的表达量，打开这个开关之后预测值会被裁剪到
    ''' ``[观测最小值, 观测最大值 + 观测跨度]`` 区间之内(这样既避免了负表达，
    ''' 又不会过早地把级联效应截断)。干预值本身不受这个区间限制。
    ''' </summary>
    ''' <returns></returns>
    Public Property ClampToObserved As Boolean = True

    ''' <summary>
    ''' 显著性判据：变化量超过多少个标准差才算显著，默认 0.5。
    ''' 
    ''' 默认值与 <see cref="BnInterventionAnalyzer"/> 的 ``|FC| &gt; 0.5 * SD`` 判据一致。
    ''' </summary>
    ''' <returns></returns>
    Public Property SignificanceZ As Double = 0.5R

    ''' <summary>
    ''' 是否记录级联过程之中每一步的表达量轨迹(仅在确定性模式下有效)，默认关闭。
    ''' 
    ''' <see cref="InterventionResult.DynamicTrajectory"/> 之中第 0 行是干预之前的基线，
    ''' 之后每一行是一次迭代的结果。轨迹的每个元素都是一个长度为 1 的数组，
    ''' 因此内存占用约为 ``(步数 + 1) * 基因数量 * 40`` 字节：在大网络之上
    ''' 逐基因地做批量扰动的时候建议保持关闭。
    ''' 
    ''' <see cref="ElasticNet.SimulateCascade"/> 会自动开启轨迹记录。
    ''' </summary>
    ''' <returns></returns>
    Public Property RecordTrajectory As Boolean = False

    ''' <summary>
    ''' 蒙特卡洛抽样的随机数种子，默认 42。
    ''' </summary>
    ''' <returns></returns>
    Public Property RandomSeed As Integer = 42

    ''' <summary>
    ''' 校验参数的取值范围，非法的时候抛出 <see cref="ArgumentException"/>。
    ''' </summary>
    Public Sub Validate()
        If Double.IsNaN(Damping) OrElse Damping <= 0.0R OrElse Damping > 1.0R Then
            Throw New ArgumentException($"the damping factor of the cascade iteration must be in the range (0, 1], but {Damping} was given!", NameOf(Damping))
        End If
        If Double.IsNaN(Tolerance) OrElse Tolerance <= 0.0R Then
            Throw New ArgumentException($"the convergence tolerance must be a positive number, but {Tolerance} was given!", NameOf(Tolerance))
        End If
        If Double.IsNaN(KnockdownResidual) OrElse KnockdownResidual < 0.0R OrElse KnockdownResidual >= 1.0R Then
            Throw New ArgumentException($"the residual ratio of the knockdown must be in the range [0, 1), but {KnockdownResidual} was given!", NameOf(KnockdownResidual))
        End If
        If Double.IsNaN(OverexpressionFold) OrElse OverexpressionFold < 1.0R Then
            Throw New ArgumentException($"the overexpression fold must be a number that is greater than or equals to 1, but {OverexpressionFold} was given!", NameOf(OverexpressionFold))
        End If
        If Double.IsNaN(SignificanceZ) OrElse SignificanceZ < 0.0R Then
            Throw New ArgumentException($"the significance cutoff must be a non-negative number, but {SignificanceZ} was given!", NameOf(SignificanceZ))
        End If
    End Sub

    ''' <summary>
    ''' 参数摘要，便于在日志之中追溯分析结果。
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"[elastic net perturbation] {Propagation}, steps<={Steps}, damping={Damping:G3}, " &
               $"knockdown={KnockdownResidual:G3}, overexpression={OverexpressionFold:G3}x, " &
               $"clamp={ClampToObserved}, significance|z|>{SignificanceZ:G3}"
    End Function
End Class

''' <summary>
''' 弹性网络虚拟扰动实验的结果。
''' 
''' 除了 <see cref="InterventionResult"/> 之上的标准字段之外，这里额外携带
''' 级联过程的诊断信息(是否收敛、是否发散、系数矩阵的谱半径、被裁剪的基因等)，
''' 便于判断预测结果是否可信。
''' </summary>
''' <remarks>
''' <see cref="InterventionResult.DynamicTrajectory"/> 的第 0 行是干预之前的基线状态，
''' 因此轨迹长度等于实际迭代步数加一；只有确定性模式并且开启了轨迹记录的时候才有值。
''' </remarks>
Public Class ElasticNetInterventionResult : Inherits InterventionResult

    ''' <summary>
    ''' 只考虑直接调控边时的一阶效应(原始表达量尺度)，与 <see cref="InterventionResult.FoldChanges"/>
    ''' 使用相同的量纲，可以直接相减来判断多跳传播贡献了多少。
    ''' </summary>
    ''' <returns></returns>
    Public Property FirstOrderDelta As Double()

    ''' <summary>
    ''' 实际执行的级联迭代步数。
    ''' </summary>
    ''' <returns></returns>
    Public Property Steps As Integer

    ''' <summary>
    ''' 级联迭代是否在步数上限之内收敛到稳态。
    ''' 
    ''' 一阶模式(<see cref="PropagationMode.OneHop"/> 或者 Steps = 1)之下这个标志没有意义。
    ''' </summary>
    ''' <returns></returns>
    Public Property Converged As Boolean

    ''' <summary>
    ''' 级联迭代是否发散(NaN/Inf 或者数值爆炸)。
    ''' 
    ''' 发散的时候结果会回退到一阶效应，绝不会返回 NaN。
    ''' </summary>
    ''' <returns></returns>
    Public Property Diverged As Boolean

    ''' <summary>
    ''' 系数矩阵 B 的谱半径估计值。
    ''' 
    ''' 大于等于 1 的时候说明这张网络上面的级联传播在数学上是不稳定的，
    ''' 需要降低阻尼或者减小步数。
    ''' </summary>
    ''' <returns></returns>
    Public Property SpectralRadius As Double

    ''' <summary>
    ''' 在级联过程之中被裁剪回观测区间的基因编号。
    ''' </summary>
    ''' <returns></returns>
    Public Property ClippedGenes As String()

    ''' <summary>
    ''' 本次扰动的目标基因之中，没有任何一条非零出边的基因。
    ''' 
    ''' 出现在这个列表里面表示该基因没有被模型建模为调控因子(通常是因为
    ''' 候选调控因子池被截断了)，因此它的扰动效应恒为零，
    ''' 不能把这个零效应解读为"该基因没有调控作用"。
    ''' </summary>
    ''' <returns></returns>
    Public Property UnmodelledRegulatorGenes As String()

    ''' <summary>
    ''' 被扰动基因在扰动之后的目标表达量(原始表达量尺度)。
    ''' </summary>
    ''' <returns></returns>
    Public Property InterventionValue As Double

    ''' <summary>
    ''' 当前的网络是否建立在 z-score 标准化之后的表达量之上。
    ''' </summary>
    ''' <returns></returns>
    Public Property Standardized As Boolean

    ''' <summary>
    ''' 实际执行的蒙特卡洛抽样次数，零表示纯确定性计算。
    ''' </summary>
    ''' <returns></returns>
    Public Property Samples As Integer

    ''' <summary>
    ''' 在本次扰动请求之中没有能在网络里面找到的(或者被跳过的)基因编号。
    ''' </summary>
    ''' <returns></returns>
    Public Property MissingGenes As String()
End Class

''' <summary>
''' 基于弹性网络线性调控网络(<see cref="LinearGrn"/>)的虚拟扰动引擎。
''' 
''' 每一个基因的回归方程 ``x_t = c_t + sum(beta_t,r * x_r)`` 被当作一条结构方程，
''' 于是整个网络构成一个线性动力系统。施加扰动的时候采用 Pearl 的 do 算子语义：
''' 被扰动基因的入边被"切断"，在每一轮迭代之中都被强制固定为干预值，其余基因
''' 迭代求解干预之后的稳态：
''' 
''' ```
''' x_t(k+1) = x_t(k) + lambda * [ (c_t + sum(beta_t,r * x_r(k))) - x_t(k) ]
''' ```
''' 
''' 由于所有模型都带截距并且在同样的样本上面最小二乘拟合，样本均值本身就是一个
''' 近似不动点，因此从基线起步的迭代等价于对变化量做 ``delta(k+1) = B * delta(k)``：
''' 第一步即为一阶直接效应，后续步骤为多跳传播。
''' </summary>
''' <remarks>
''' <para>
''' <b>使用约束与已知偏差：</b>
''' </para>
''' <para>
''' 1. 回归系数是"其它调控因子保持不变"之下的条件效应，与被扰动基因高度共线的
'''    另一批基因在模型里面仍然停留在基线之上，因此共线模块之中的单基因扰动
'''    其效应量是不可辨识的(符号可靠、幅度不可靠)，只有整个模块的效应之和才有意义。
''' </para>
''' <para>
''' 2. 候选调控因子池会被 <see cref="LinearGrnOptions.maxCandidates"/>、
'''    <see cref="LinearGrnOptions.minAbsCorrelation"/> 与
'''    <see cref="LinearGrnOptions.maxRegulatorsPerGene"/> 截断，因此部分真实的
'''    调控边并没有进入模型，级联效应会被系统性地低估。请结合
'''    <see cref="ElasticNetInterventionResult.UnmodelledRegulatorGenes"/> 判断结果。
''' </para>
''' <para>
''' 3. 本引擎只能用于假说生成与靶点排序，不能替代真实的 Perturb-seq 定量实验。
''' </para>
''' </remarks>
Public Class ElasticNet : Implements InsilicoPerturbationExperiment

    ''' <summary>
    ''' 被模拟的线性调控网络。
    ''' </summary>
    ReadOnly net As LinearGrn

    ''' <summary>
    ''' 虚拟扰动的参数配置。
    ''' </summary>
    ReadOnly opt As ElasticNetPerturbationOptions

    ''' <summary>
    ''' 基因编号到表达矩阵行下标的映射。
    ''' </summary>
    ReadOnly geneIndex As Dictionary(Of String, Integer)

    ''' <summary>
    ''' 当前网络是否建立在 z-score 标准化之后的表达量之上。
    ''' </summary>
    ReadOnly isStandardized As Boolean

    ''' <summary>
    ''' 有模型的基因行号，下标与 <see cref="intercepts"/> 对齐。
    ''' </summary>
    Private targetIndex As Integer()

    ''' <summary>
    ''' 每一行的调控因子行号(已经剔除零权重)。
    ''' </summary>
    Private regulatorIndex As Integer()()

    ''' <summary>
    ''' 与 <see cref="regulatorIndex"/> 一一对应的回归系数。
    ''' </summary>
    Private coefficients As Double()()

    ''' <summary>
    ''' 每一个目标基因模型的截距项。
    ''' </summary>
    Private intercepts As Double()

    ''' <summary>
    ''' 每一个目标基因模型的残差标准差(模型空间)，用于蒙特卡洛抽样。
    ''' </summary>
    Private residualSd As Double()

    ''' <summary>
    ''' 每一个基因的非零出边数量。
    ''' </summary>
    Private outDegree As Integer()

    ''' <summary>
    ''' 原始表达量尺度之上，每一个基因的样本均值。
    ''' </summary>
    ReadOnly rawMean As Double()

    ''' <summary>
    ''' 原始表达量尺度之上，每一个基因的样本标准差(ddof = n - 1)。
    ''' </summary>
    ReadOnly rawSd As Double()

    ''' <summary>
    ''' 模型空间之中的裁剪下界(逐基因)。
    ''' </summary>
    Private modelLower As Double()

    ''' <summary>
    ''' 模型空间之中的裁剪上界(逐基因)。
    ''' </summary>
    Private modelUpper As Double()

    ''' <summary>
    ''' 数值爆炸的判据阈值。
    ''' </summary>
    Private modelAbsLimit As Double

    ''' <summary>
    ''' 模型空间之中的基线表达量，即迭代的起点。
    ''' </summary>
    Private modelBaseline As Double()

    ''' <summary>
    ''' 调用方显式指定的基线(原始表达量尺度)，为空表示使用样本均值。
    ''' </summary>
    Private baselineOverride As Double() = Nothing

    ''' <summary>
    ''' 谱半径的惰性缓存。
    ''' </summary>
    Private spectralRadiusCache As Double = Double.NaN

    ''' <summary>
    ''' 从未作为调控因子出现的基因列表的惰性缓存。
    ''' </summary>
    Private unmodelledCache As String() = Nothing

    ''' <summary>
    ''' 基于一个已经拟合完成的线性调控网络构造虚拟扰动引擎。
    ''' </summary>
    ''' <param name="net">已经调用过 <see cref="LinearGrn.Fit"/> 的调控网络对象</param>
    ''' <param name="options">虚拟扰动的参数配置，为空的时候使用默认参数</param>
    Public Sub New(net As LinearGrn, Optional options As ElasticNetPerturbationOptions = Nothing)
        If net Is Nothing Then
            Throw New ArgumentNullException(NameOf(net))
        End If

        Me.net = net
        Me.opt = If(options, New ElasticNetPerturbationOptions())

        Call Me.opt.Validate()

        Me.geneIndex = BuildGeneIndex(net.Genes)
        Me.isStandardized = net.Options.standardize

        Dim rows As DataFrameRow() = net.Expression.expression

        Me.rawMean = New Double(rows.Length - 1) {}
        Me.rawSd = New Double(rows.Length - 1) {}

        For i As Integer = 0 To rows.Length - 1
            Dim v As Double() = rows(i).experiments

            If v Is Nothing OrElse v.Length = 0 Then
                rawMean(i) = 0
                rawSd(i) = 0
            Else
                rawMean(i) = v.Average
                rawSd(i) = SampleStandardDeviation(v)
            End If
        Next

        Call BuildObservedRange(rows)
        Call BuildCoefficientTable()

        Me.modelBaseline = Nothing
        Call RefreshModelBaseline()
    End Sub

    ''' <summary>
    ''' 直接从表达矩阵开始，先推断调控网络再构造虚拟扰动引擎。
    ''' </summary>
    ''' <param name="expr">行是基因、列是样本的表达矩阵</param>
    ''' <param name="grnOptions">线性拟合的参数配置</param>
    ''' <param name="options">虚拟扰动的参数配置</param>
    Public Sub New(expr As Matrix,
                   Optional grnOptions As LinearGrnOptions = Nothing,
                   Optional options As ElasticNetPerturbationOptions = Nothing)
        Call Me.New(New LinearGrn(expr, grnOptions).Fit(), options)
    End Sub

    ''' <summary>
    ''' 被模拟的线性调控网络。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Network As LinearGrn
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return net
        End Get
    End Property

    ''' <summary>
    ''' 虚拟扰动的参数配置。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Options As ElasticNetPerturbationOptions
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return opt
        End Get
    End Property

    ''' <summary>
    ''' 严格模式，默认为 True。
    ''' 
    ''' True：当扰动目标基因不在网络之中时抛出 <see cref="MissingMemberException"/>；
    ''' False：打印一条警告，不执行虚拟扰动，直接以野生型数据作为扰动结果返回，
    ''' 并把结果标记为未定义(<see cref="InterventionResult.Undefined"/> = True)。
    ''' 
    ''' 这个语义与 <see cref="BnInterventionAnalyzer.Strict"/> 保持一致。
    ''' </summary>
    ''' <returns></returns>
    Public Property Strict As Boolean = True

    ''' <summary>
    ''' 当前网络是否建立在 z-score 标准化之后的表达量之上。
    ''' 
    ''' 标准化模式之下回归系数位于 z-score 空间，本引擎会在内部完成
    ''' 原始表达量与模型空间之间的换算，对外的结果始终是原始表达量尺度。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Standardized As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return isStandardized
        End Get
    End Property

    ''' <summary>
    ''' 野生型基线(原始表达量尺度)，默认是每一个基因的样本均值。
    ''' 
    ''' 可以显式赋值以指定对照组基线(例如某个对照样本子集的均值)，
    ''' 赋值 Nothing 可以恢复默认行为。
    ''' </summary>
    ''' <returns></returns>
    Public Property Baseline As Double()
        Get
            If baselineOverride Is Nothing Then
                Return rawMean
            Else
                Return baselineOverride
            End If
        End Get
        Set(value As Double())
            If value Is Nothing Then
                baselineOverride = Nothing
            Else
                If value.Length <> rawMean.Length Then
                    Throw New ArgumentException(
                        $"the size of the given baseline vector({value.Length}) is not equals to the gene size({rawMean.Length}) of the network!",
                        NameOf(value))
                End If

                baselineOverride = CType(value.Clone(), Double())
            End If

            Call RefreshModelBaseline()
        End Set
    End Property

    ''' <summary>
    ''' 从未作为调控因子(非零出边)出现过的基因编号。
    ''' 
    ''' 这些基因的扰动效应恒为零，通常是因为候选调控因子池被截断，
    ''' 不能解读为"该基因没有调控作用"。
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property UnmodelledGenes As String()
        Get
            If unmodelledCache Is Nothing Then
                Dim names As New List(Of String)

                For i As Integer = 0 To outDegree.Length - 1
                    If outDegree(i) = 0 Then
                        Call names.Add(net.Genes(i))
                    End If
                Next

                unmodelledCache = names.ToArray
            End If

            Return unmodelledCache
        End Get
    End Property

    ''' <summary>
    ''' 执行一次虚拟敲除实验(目标基因表达量被置为零)。
    ''' </summary>
    ''' <param name="geneName">被敲除的基因编号</param>
    ''' <param name="nSamples">蒙特卡洛抽样次数，小于等于零的时候执行确定性计算</param>
    ''' <returns></returns>
    Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
        Return Intervene(NewSpec(geneName, InterventionMode.Knockout), nSamples)
    End Function

    ''' <summary>
    ''' 执行一次虚拟过表达实验(目标基因表达量被提升到基线的
    ''' <see cref="ElasticNetPerturbationOptions.OverexpressionFold"/> 倍)。
    ''' </summary>
    ''' <param name="geneName">被过表达的基因编号</param>
    ''' <param name="nSamples">蒙特卡洛抽样次数，小于等于零的时候执行确定性计算</param>
    ''' <returns></returns>
    Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
        Return Intervene(NewSpec(geneName, InterventionMode.Overexpression), nSamples)
    End Function

    ''' <summary>
    ''' 执行一次虚拟敲降实验(目标基因表达量被下调到基线的
    ''' <see cref="ElasticNetPerturbationOptions.KnockdownResidual"/> 倍，默认保留 20%)。
    ''' </summary>
    ''' <param name="geneName">被敲降的基因编号</param>
    ''' <param name="nSamples">蒙特卡洛抽样次数，小于等于零的时候执行确定性计算</param>
    ''' <returns></returns>
    Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
        Return Intervene(NewSpec(geneName, InterventionMode.Knockdown), nSamples)
    End Function

    ''' <summary>
    ''' 按照给定的干预定义执行一次虚拟扰动实验。
    ''' </summary>
    ''' <param name="spec">干预定义</param>
    ''' <param name="nSamples">蒙特卡洛抽样次数，小于等于零的时候执行确定性计算</param>
    ''' <returns></returns>
    Public Function Intervene(spec As InterventionSpec, Optional nSamples As Integer = 0) As InterventionResult
        If spec Is Nothing Then
            Throw New ArgumentNullException(NameOf(spec))
        End If

        Return InterveneCore(New InterventionSpec() {spec}, nSamples, -1)
    End Function

    ''' <summary>
    ''' 同时对多个基因施加扰动(组合扰动)并计算网络的响应。
    ''' 
    ''' 组合扰动可以用来近似共线模块的整体敲降：由于弹性网络会把共线基因的效应
    ''' 平摊到各个成员之上，单基因扰动得到的效应量偏小，只有整组同时扰动才能
    ''' 恢复真实的效应总量。
    ''' </summary>
    ''' <param name="specs">干预定义集合</param>
    ''' <param name="nSamples">蒙特卡洛抽样次数，小于等于零的时候执行确定性计算</param>
    ''' <returns></returns>
    Public Function Intervene(specs As IEnumerable(Of InterventionSpec), Optional nSamples As Integer = 0) As InterventionResult
        Return InterveneCore(specs, nSamples, -1)
    End Function

    ''' <summary>
    ''' 以指定的级联步数执行一次扰动实验，用于观察效应的逐步传播轨迹。
    ''' </summary>
    ''' <param name="spec">干预定义</param>
    ''' <param name="steps">级联迭代的步数，小于 1 的时候按 1 处理</param>
    ''' <returns></returns>
    Public Function SimulateCascade(spec As InterventionSpec, steps As Integer) As InterventionResult
        If spec Is Nothing Then
            Throw New ArgumentNullException(NameOf(spec))
        End If

        Return InterveneCore(New InterventionSpec() {spec}, 0, steps)
    End Function

    ''' <summary>
    ''' 估计系数矩阵 B 的谱半径(20 次幂迭代)。
    ''' 
    ''' 谱半径小于 1 的时候阻尼级联必然收敛，大于等于 1 的时候说明这张网络
    ''' 上面的多跳传播在数学上是不稳定的。
    ''' </summary>
    ''' <returns></returns>
    Public Function EstimateSpectralRadius() As Double
        If Not Double.IsNaN(spectralRadiusCache) Then
            Return spectralRadiusCache
        End If

        Dim n As Integer = net.Genes.Length
        Dim v As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            v(i) = 1.0R + 0.5R * std.Sin(i)
        Next

        Dim norm As Double = VectorNorm(v)

        If norm <= 0 OrElse Not IsFinite(norm) Then
            spectralRadiusCache = 0
            Return 0
        End If

        For i As Integer = 0 To n - 1
            v(i) /= norm
        Next

        Dim rho As Double = 0

        For it As Integer = 1 To 20
            Dim w As Double() = ApplyB(v)

            norm = VectorNorm(w)

            If norm <= 0 OrElse Not IsFinite(norm) Then
                spectralRadiusCache = 0
                Return 0
            End If

            rho = norm

            For i As Integer = 0 To n - 1
                v(i) = w(i) / norm
            Next
        Next

        spectralRadiusCache = rho

        Return rho
    End Function

    ''' <summary>
    ''' 扰动实验的统一执行入口。
    ''' </summary>
    ''' <param name="specs">干预定义集合</param>
    ''' <param name="nSamples">蒙特卡洛抽样次数</param>
    ''' <param name="stepsOverride">级联步数的覆盖值，小于等于零表示使用参数配置</param>
    ''' <returns></returns>
    Private Function InterveneCore(specs As IEnumerable(Of InterventionSpec),
                                   nSamples As Integer,
                                   stepsOverride As Integer) As InterventionResult

        If specs Is Nothing Then
            Throw New ArgumentNullException(NameOf(specs))
        End If

        Dim list As List(Of InterventionSpec) = specs.Where(Function(s) s IsNot Nothing).ToList

        If list.Count = 0 Then
            Throw New ArgumentException("at least one intervention spec is required for the virtual perturbation!", NameOf(specs))
        End If

        Dim clamps As New Dictionary(Of Integer, Double)
        Dim intervened As New List(Of Integer)
        Dim missing As New List(Of String)
        Dim interventionValue As Double = 0
        For Each spec As InterventionSpec In list
            Dim idx As Integer = ResolveGeneIndex(spec)

            If idx < 0 Then
                If Strict Then
                    Throw New MissingMemberException($"missing target gene: {spec.GeneName}")
                End If

                Call $"[ElasticNet] 警告: 在拟合的网络中未找到虚拟扰动的目标基因 '{spec.GeneName}'，所以没有执行虚拟扰动，函数返回了野生型的数据。".debug
                Call missing.Add(If(spec.GeneName, ""))

                Continue For
            End If

            spec.GeneIndex = idx

            Dim target As Double = InterventionTarget(spec, Baseline(idx))

            clamps(idx) = ToModel(target, idx)
            Call intervened.Add(idx)

            interventionValue = target
        Next

        Dim primarySpec As InterventionSpec = CombinedSpec(list)

        If clamps.Count = 0 Then
            Return UndefinedResult(primarySpec, missing)
        End If

        Dim steps As Integer = If(stepsOverride > 0, std.Max(1, stepsOverride), EffectiveSteps())
        Dim firstOrder As Double() = FirstOrderDelta(clamps, intervened)
        Dim converged As Boolean = False
        Dim diverged As Boolean = False
        Dim trajectory As List(Of Double()) = Nothing

        If (opt.RecordTrajectory OrElse stepsOverride > 0) AndAlso nSamples <= 0 Then
            trajectory = New List(Of Double())
        End If

        Dim clipped As New HashSet(Of Integer)
        Dim final As Double() = Propagate(clamps, steps, False, Nothing, trajectory, clipped, converged, diverged)

        Dim wildtypeMeans As Double() = CType(Baseline.Clone(), Double())
        Dim wildtypeSd As Double() = CType(rawSd.Clone(), Double())
        Dim mutantMeans As Double() = Nothing
        Dim mutantSd As Double() = Nothing
        Dim samples As Integer = 0

        If diverged Then
            ' 数值爆炸，稍后回退到一阶效应
        ElseIf nSamples > 0 Then
            Dim mc As MonteCarloResult = RunMonteCarlo(clamps, steps, nSamples)

            If mc.Diverged Then
                diverged = True
            Else
                wildtypeMeans = mc.WildtypeMeans
                wildtypeSd = mc.WildtypeSds
                mutantMeans = mc.MutantMeans
                mutantSd = mc.MutantSds
                samples = nSamples

                For Each idx As Integer In mc.Clipped
                    Call clipped.Add(idx)
                Next
            End If
        Else
            mutantMeans = ToRaw(final)
        End If

        If diverged OrElse mutantMeans Is Nothing Then
            ' 发散的时候结果回退到一阶直接效应，保证返回值永远是有限数值
            diverged = True
            wildtypeMeans = CType(Baseline.Clone(), Double())
            wildtypeSd = CType(rawSd.Clone(), Double())
            mutantMeans = AddDelta(wildtypeMeans, firstOrder)
            mutantSd = Nothing
            samples = 0
            trajectory = Nothing
        End If

        Dim n As Integer = net.Genes.Length
        Dim fold As Double() = New Double(n - 1) {}
        Dim percent As Double() = New Double(n - 1) {}
        Dim zscore As Double() = New Double(n - 1) {}
        Dim significant As Boolean() = New Boolean(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim change As Double = mutantMeans(i) - wildtypeMeans(i)

            fold(i) = change

            If std.Abs(wildtypeMeans(i)) > 0.0000000001R Then
                percent(i) = change / std.Abs(wildtypeMeans(i)) * 100
            Else
                percent(i) = If(std.Abs(change) > 0.01R, Double.NaN, 0.0R)
            End If

            If wildtypeSd(i) > 0 Then
                zscore(i) = change / wildtypeSd(i)
            Else
                zscore(i) = 0
            End If

            Dim threshold As Double

            If mutantSd IsNot Nothing AndAlso mutantSd(i) > 0 Then
                threshold = opt.SignificanceZ * mutantSd(i)
            Else
                threshold = opt.SignificanceZ * wildtypeSd(i)
            End If

            If threshold > 0 Then
                significant(i) = std.Abs(change) > threshold
            Else
                significant(i) = std.Abs(change) > 0.000000001R
            End If
        Next

        Dim dynamic As Double(,)() = Nothing

        If trajectory IsNot Nothing AndAlso trajectory.Count > 0 Then
            dynamic = New Double(trajectory.Count - 1, n - 1)() {}

            For t As Integer = 0 To trajectory.Count - 1
                For i As Integer = 0 To n - 1
                    dynamic(t, i) = New Double() {ToRaw(trajectory(t)(i), i)}
                Next
            Next
        End If

        Dim unmodelled As String() = intervened _
            .Where(Function(i) outDegree(i) = 0) _
            .Select(Function(i) net.Genes(i)) _
            .ToArray
        Dim clippedGenes As String() = clipped _
            .Select(Function(i) net.Genes(i)) _
            .ToArray

        Return New ElasticNetInterventionResult With {
            .Spec = primarySpec,
            .WildtypeMeans = wildtypeMeans,
            .MutantMeans = mutantMeans,
            .FoldChanges = fold,
            .PercentChanges = percent,
            .IsSignificant = significant,
            .GeneNames = GeneNames(),
            .ZScores = zscore,
            .WildtypeSDs = wildtypeSd,
            .DynamicTrajectory = dynamic,
            .Undefined = False,
            .FirstOrderDelta = firstOrder,
            .Steps = steps,
            .Converged = converged,
            .Diverged = diverged,
            .SpectralRadius = EstimateSpectralRadius(),
            .ClippedGenes = clippedGenes,
            .UnmodelledRegulatorGenes = unmodelled,
            .InterventionValue = interventionValue,
            .Standardized = isStandardized,
            .Samples = samples,
            .MissingGenes = missing.ToArray
        }
    End Function

    ''' <summary>
    ''' 执行级联迭代(可选地叠加高斯残差噪声)，返回模型空间之中的最终状态。
    ''' </summary>
    ''' <param name="clamps">被钳制的基因行号到干预值(模型空间)的映射</param>
    ''' <param name="steps">最大迭代步数</param>
    ''' <param name="noiseOn">是否在每一次更新的时候叠加残差噪声</param>
    ''' <param name="rng">随机数发生器，确定性模式下为空</param>
    ''' <param name="trajectory">轨迹记录器，为空的时候不记录</param>
    ''' <param name="clipped">被裁剪的基因行号集合</param>
    ''' <param name="converged">迭代是否收敛</param>
    ''' <param name="diverged">迭代是否发散</param>
    ''' <returns>模型空间之中的最终表达量</returns>
    Private Function Propagate(clamps As Dictionary(Of Integer, Double),
                               steps As Integer,
                               noiseOn As Boolean,
                               rng As Random,
                               trajectory As List(Of Double()),
                               clipped As HashSet(Of Integer),
                               ByRef converged As Boolean,
                               ByRef diverged As Boolean) As Double()

        Dim n As Integer = net.Genes.Length
        Dim x As Double() = CType(modelBaseline.Clone(), Double())
        Dim buffer As Double() = CType(x.Clone(), Double())
        Dim clamped As Boolean() = New Boolean(n - 1) {}

        converged = False
        diverged = False

        If trajectory IsNot Nothing Then
            ' 第 0 步记录干预之前的基线状态，之后每一步记录一次迭代结果
            Call trajectory.Add(CType(x.Clone(), Double()))
        End If

        For Each idx As Integer In clamps.Keys
            clamped(idx) = True
            x(idx) = clamps(idx)
            buffer(idx) = clamps(idx)
        Next

        For k As Integer = 1 To steps
            Dim maxDiff As Double = 0
            Dim maxAbs As Double = 0

            For r As Integer = 0 To targetIndex.Length - 1
                Dim t As Integer = targetIndex(r)

                If clamped(t) Then
                    Continue For
                End If

                Dim value As Double = intercepts(r)
                Dim regs As Integer() = regulatorIndex(r)
                Dim coefs As Double() = coefficients(r)

                For j As Integer = 0 To regs.Length - 1
                    value += coefs(j) * x(regs(j))
                Next

                If noiseOn Then
                    Dim sd As Double = residualSd(r)

                    If sd > 0 Then
                        value += Gauss(rng) * sd
                    End If
                End If

                If opt.ClampToObserved Then
                    Dim lo As Double = modelLower(t)
                    Dim hi As Double = modelUpper(t)

                    If value < lo Then
                        value = lo
                        Call clipped.Add(t)
                    ElseIf value > hi Then
                        value = hi
                        Call clipped.Add(t)
                    End If
                End If

                buffer(t) = x(t) + opt.Damping * (value - x(t))
            Next

            For i As Integer = 0 To n - 1
                Dim magnitude As Double = std.Abs(buffer(i))

                If Not IsFinite(magnitude) Then
                    diverged = True
                    Exit For
                End If

                If magnitude > maxAbs Then
                    maxAbs = magnitude
                End If

                Dim diff As Double = std.Abs(buffer(i) - x(i))

                If diff > maxDiff Then
                    maxDiff = diff
                End If
            Next

            If Not diverged AndAlso maxAbs > modelAbsLimit Then
                diverged = True
            End If

            If diverged Then
                Exit For
            End If

            Dim swap As Double() = x
            x = buffer
            buffer = swap

            If trajectory IsNot Nothing Then
                Call trajectory.Add(CType(x.Clone(), Double()))
            End If

            If maxDiff < opt.Tolerance Then
                converged = True
                Exit For
            End If
        Next

        Return x
    End Function

    ''' <summary>
    ''' 按照各基因模型的残差标准差执行蒙特卡洛抽样。
    ''' </summary>
    ''' <param name="clamps">被钳制的基因行号到干预值(模型空间)的映射</param>
    ''' <param name="steps">最大迭代步数</param>
    ''' <param name="nSamples">抽样次数</param>
    ''' <returns></returns>
    Private Function RunMonteCarlo(clamps As Dictionary(Of Integer, Double),
                                   steps As Integer,
                                   nSamples As Integer) As MonteCarloResult

        Dim n As Integer = net.Genes.Length
        Dim sumWild As Double() = New Double(n - 1) {}
        Dim sumWild2 As Double() = New Double(n - 1) {}
        Dim sumMutant As Double() = New Double(n - 1) {}
        Dim sumMutant2 As Double() = New Double(n - 1) {}
        Dim rng As New Random(opt.RandomSeed)
        Dim clipped As New HashSet(Of Integer)
        Dim noClamp As New Dictionary(Of Integer, Double)
        Dim result As New MonteCarloResult With {.Diverged = False}
        Dim converged As Boolean = False
        Dim diverged As Boolean = False
        Dim accepted As Integer = 0

        For k As Integer = 1 To nSamples
            Dim wild As Double() = Propagate(noClamp, steps, True, rng, Nothing, clipped, converged, diverged)

            If diverged Then
                result.Diverged = True
                Exit For
            End If

            Dim mutant As Double() = Propagate(clamps, steps, True, rng, Nothing, clipped, converged, diverged)

            If diverged Then
                result.Diverged = True
                Exit For
            End If

            For i As Integer = 0 To n - 1
                Dim a As Double = ToRaw(wild(i), i)
                Dim b As Double = ToRaw(mutant(i), i)

                sumWild(i) += a
                sumWild2(i) += a * a
                sumMutant(i) += b
                sumMutant2(i) += b * b
            Next

            accepted += 1
        Next

        result.Clipped = clipped.ToArray
        result.WildtypeMeans = New Double(n - 1) {}
        result.WildtypeSds = New Double(n - 1) {}
        result.MutantMeans = New Double(n - 1) {}
        result.MutantSds = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim meanWild As Double = If(accepted > 0, sumWild(i) / accepted, rawMean(i))
            Dim meanMutant As Double = If(accepted > 0, sumMutant(i) / accepted, rawMean(i))

            result.WildtypeMeans(i) = meanWild
            result.MutantMeans(i) = meanMutant
            result.WildtypeSds(i) = SampleStandardDeviation(sumWild2(i), meanWild, accepted)
            result.MutantSds(i) = SampleStandardDeviation(sumMutant2(i), meanMutant, accepted)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 计算只考虑直接调控边时的一阶效应(原始表达量尺度)。
    ''' </summary>
    ''' <param name="clamps">被钳制的基因行号到干预值(模型空间)的映射</param>
    ''' <param name="intervened">被扰动的基因行号</param>
    ''' <returns>与表达矩阵行数等长的变化量数组</returns>
    Private Function FirstOrderDelta(clamps As Dictionary(Of Integer, Double),
                                     intervened As List(Of Integer)) As Double()

        Dim n As Integer = net.Genes.Length
        Dim delta As Double() = New Double(n - 1) {}

        For Each idx As Integer In intervened
            delta(idx) = clamps(idx) - modelBaseline(idx)
        Next

        Dim moved As Double() = ApplyB(delta)
        Dim result As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            result(i) = DeltaToRaw(moved(i), i)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 把系数矩阵 B 作用到一个向量之上(不含截距与阻尼)。
    ''' </summary>
    ''' <param name="v">模型空间之中的输入向量</param>
    ''' <returns></returns>
    Private Function ApplyB(v As Double()) As Double()
        Dim w As Double() = New Double(v.Length - 1) {}

        For r As Integer = 0 To targetIndex.Length - 1
            Dim regs As Integer() = regulatorIndex(r)
            Dim coefs As Double() = coefficients(r)
            Dim sum As Double = 0

            For j As Integer = 0 To regs.Length - 1
                sum += coefs(j) * v(regs(j))
            Next

            w(targetIndex(r)) = sum
        Next

        Return w
    End Function

    ''' <summary>
    ''' 把 <see cref="LinearGrn.Models"/> 编译成 CSR 风格的系数表。
    ''' 
    ''' 这里直接遍历全部模型而不是使用 <see cref="LinearGrn.Regulations"/>：
    ''' 后者会按照权重阈值过滤并去重，会破坏扰动计算的完整性。
    ''' </summary>
    Private Sub BuildCoefficientTable()
        Dim n As Integer = net.Genes.Length
        Dim models As New Dictionary(Of Integer, LinearGrnModel)

        outDegree = New Integer(n - 1) {}

        For Each model As LinearGrnModel In net.Models
            If model Is Nothing OrElse Not model.IsValid Then
                Continue For
            End If
            If model.regulators Is Nothing OrElse model.weights Is Nothing Then
                Continue For
            End If

            Dim t As Integer = IndexOfGene(model.target)

            If t < 0 Then
                Continue For
            End If

            models(t) = model
        Next

        Dim targets As New List(Of Integer)
        Dim regulators As New List(Of Integer())
        Dim coefs As New List(Of Double())
        Dim interceptList As New List(Of Double)
        Dim sdList As New List(Of Double)

        For Each t As Integer In models.Keys.OrderBy(Function(i) i)
            Dim model As LinearGrnModel = models(t)
            Dim regs As New List(Of Integer)
            Dim weights As New List(Of Double)

            For j As Integer = 0 To model.regulators.Length - 1
                Dim w As Double = model.weights(j)

                If w = 0.0R OrElse Not IsFinite(w) Then
                    Continue For
                End If

                Dim r As Integer = IndexOfGene(model.regulators(j))

                If r < 0 Then
                    Continue For
                End If

                Call regs.Add(r)
                Call weights.Add(w)
                outDegree(r) += 1
            Next

            Call targets.Add(t)
            Call regulators.Add(regs.ToArray)
            Call coefs.Add(weights.ToArray)
            Call interceptList.Add(model.intercept)

            Dim sd As Double = model.rmse

            Call sdList.Add(If(IsFinite(sd) AndAlso sd > 0, sd, 0.0R))
        Next

        targetIndex = targets.ToArray
        regulatorIndex = regulators.ToArray
        coefficients = coefs.ToArray
        intercepts = interceptList.ToArray
        residualSd = sdList.ToArray
    End Sub

    ''' <summary>
    ''' 计算每一个基因的观测区间，并换算到模型空间作为级联迭代的裁剪边界。
    ''' </summary>
    ''' <param name="rows">表达矩阵的行</param>
    Private Sub BuildObservedRange(rows As DataFrameRow())
        Dim n As Integer = rows.Length
        Dim limit As Double = 1000000.0R

        modelLower = New Double(n - 1) {}
        modelUpper = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim v As Double() = rows(i).experiments

            If v Is Nothing OrElse v.Length = 0 Then
                modelLower(i) = Double.NegativeInfinity
                modelUpper(i) = Double.PositiveInfinity
                Continue For
            End If

            Dim lo As Double = v.Min
            Dim hi As Double = v.Max
            ' 允许向上外扩一个观测跨度，避免级联效应过早被裁剪掉
            Dim upper As Double = hi + (hi - lo)

            If isStandardized Then
                Dim sd As Double = rawSd(i)

                If sd <= 0 Then
                    ' 常基因在 z-score 空间里面没有可用的区间
                    modelLower(i) = Double.NegativeInfinity
                    modelUpper(i) = Double.PositiveInfinity
                Else
                    modelLower(i) = (lo - rawMean(i)) / sd
                    modelUpper(i) = (upper - rawMean(i)) / sd
                End If
            Else
                modelLower(i) = lo
                modelUpper(i) = upper
            End If

            If IsFinite(modelLower(i)) AndAlso IsFinite(modelUpper(i)) Then
                limit = std.Max(limit, std.Max(std.Abs(modelLower(i)), std.Abs(modelUpper(i))) * 100)
            End If
        Next

        modelAbsLimit = limit
    End Sub

    ''' <summary>
    ''' 根据当前的基线重新计算模型空间之中的迭代起点。
    ''' </summary>
    Private Sub RefreshModelBaseline()
        Dim values As Double() = Baseline
        Dim n As Integer = values.Length

        modelBaseline = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            modelBaseline(i) = ToModel(values(i), i)
        Next
    End Sub

    ''' <summary>
    ''' 计算一次扰动实际执行的级联步数。
    ''' </summary>
    ''' <returns></returns>
    Private Function EffectiveSteps() As Integer
        If opt.Propagation = PropagationMode.OneHop Then
            Return 1
        Else
            Return std.Max(1, opt.Steps)
        End If
    End Function

    ''' <summary>
    ''' 按照乘性比例语义解析扰动之后的表达量目标值(原始表达量尺度)。
    ''' </summary>
    ''' <param name="spec">干预定义</param>
    ''' <param name="baselineValue">目标基因的野生型基线</param>
    ''' <returns></returns>
    Private Function InterventionTarget(spec As InterventionSpec, baselineValue As Double) As Double
        Dim value As Double

        Select Case spec.Mode
            Case InterventionMode.Knockout
                value = 0
            Case InterventionMode.Overexpression
                value = opt.OverexpressionFold * baselineValue
            Case InterventionMode.Knockdown
                value = opt.KnockdownResidual * baselineValue
            Case Else
                value = spec.Value
        End Select

        If Not IsFinite(value) Then
            Return 0
        End If

        Return std.Max(0.0R, value)
    End Function

    ''' <summary>
    ''' 查找基因编号在表达矩阵之中的行下标。
    ''' </summary>
    ''' <param name="name">基因编号</param>
    ''' <returns>不存在的时候返回 -1</returns>
    Private Function IndexOfGene(name As String) As Integer
        Dim idx As Integer

        If name IsNot Nothing AndAlso geneIndex.TryGetValue(name, idx) Then
            Return idx
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' 解析干预定义之中目标基因在表达矩阵之中的行下标。
    ''' </summary>
    ''' <param name="spec">干预定义</param>
    ''' <returns>不存在的时候返回 -1</returns>
    Private Function ResolveGeneIndex(spec As InterventionSpec) As Integer
        Dim idx As Integer = spec.GeneIndex

        If idx >= 0 AndAlso idx < net.Genes.Length AndAlso
            String.Equals(net.Genes(idx), spec.GeneName, StringComparison.OrdinalIgnoreCase) Then

            Return idx
        End If

        If String.IsNullOrEmpty(spec.GeneName) Then
            Return -1
        End If

        If geneIndex.TryGetValue(spec.GeneName, idx) Then
            Return idx
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' 把多个干预定义合并成一个用于结果对象展示的定义。
    ''' </summary>
    ''' <param name="list">干预定义列表</param>
    ''' <returns></returns>
    Private Shared Function CombinedSpec(list As List(Of InterventionSpec)) As InterventionSpec
        If list.Count = 1 Then
            Return list(0)
        End If

        Return New InterventionSpec With {
            .GeneIndex = -1,
            .GeneName = String.Join(",", list.Select(Function(s) s.GeneName)),
            .Mode = list(0).Mode
        }
    End Function

    ''' <summary>
    ''' 构造一条指定模式的干预定义。
    ''' </summary>
    ''' <param name="geneName">目标基因编号</param>
    ''' <param name="mode">干预模式</param>
    ''' <returns></returns>
    Private Shared Function NewSpec(geneName As String, mode As InterventionMode) As InterventionSpec
        Return New InterventionSpec With {
            .GeneName = geneName,
            .Mode = mode
        }
    End Function

    ''' <summary>
    ''' 构造"没有找到目标基因、未执行虚拟扰动"时的野生型降级结果。
    ''' </summary>
    ''' <param name="spec">干预定义</param>
    ''' <param name="missing">缺失的基因编号</param>
    ''' <returns></returns>
    Private Function UndefinedResult(spec As InterventionSpec, missing As List(Of String)) As ElasticNetInterventionResult
        Dim n As Integer = net.Genes.Length
        Dim wildtype As Double() = CType(Baseline.Clone(), Double())

        Return New ElasticNetInterventionResult With {
            .Spec = spec,
            .WildtypeMeans = wildtype,
            .MutantMeans = CType(wildtype.Clone(), Double()),
            .FoldChanges = New Double(n - 1) {},
            .PercentChanges = New Double(n - 1) {},
            .IsSignificant = New Boolean(n - 1) {},
            .GeneNames = GeneNames(),
            .ZScores = New Double(n - 1) {},
            .WildtypeSDs = CType(rawSd.Clone(), Double()),
            .Undefined = True,
            .FirstOrderDelta = New Double(n - 1) {},
            .Steps = 0,
            .Converged = False,
            .Diverged = False,
            .SpectralRadius = 0,
            .ClippedGenes = New String() {},
            .UnmodelledRegulatorGenes = New String() {},
            .InterventionValue = 0,
            .Standardized = isStandardized,
            .Samples = 0,
            .MissingGenes = missing.ToArray
        }
    End Function

    ''' <summary>
    ''' 网络之中的基因编号列表(返回一份副本，避免外部修改内部状态)。
    ''' </summary>
    ''' <returns></returns>
    Private Function GeneNames() As String()
        Return DirectCast(net.Genes.Clone(), String())
    End Function

    ''' <summary>
    ''' 把模型空间之中的数值换算回原始表达量尺度。
    ''' </summary>
    ''' <param name="modelValue">模型空间之中的数值</param>
    ''' <param name="idx">基因行下标</param>
    ''' <returns></returns>
    Private Function ToRaw(modelValue As Double, idx As Integer) As Double
        If Not isStandardized Then
            Return modelValue
        Else
            Return rawMean(idx) + rawSd(idx) * modelValue
        End If
    End Function

    ''' <summary>
    ''' 把整条模型空间之中的向量换算回原始表达量尺度。
    ''' </summary>
    ''' <param name="modelValues">模型空间之中的向量</param>
    ''' <returns></returns>
    Private Function ToRaw(modelValues As Double()) As Double()
        Dim n As Integer = modelValues.Length
        Dim result As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            result(i) = ToRaw(modelValues(i), i)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 把原始表达量换算到模型空间(系数所在的空间)。
    ''' </summary>
    ''' <param name="rawValue">原始表达量</param>
    ''' <param name="idx">基因行下标</param>
    ''' <returns></returns>
    Private Function ToModel(rawValue As Double, idx As Integer) As Double
        If Not isStandardized Then
            Return rawValue
        End If

        Dim sd As Double = rawSd(idx)

        If sd <= 0 Then
            Return 0
        Else
            Return (rawValue - rawMean(idx)) / sd
        End If
    End Function

    ''' <summary>
    ''' 把模型空间之中的变化量换算回原始表达量尺度的变化量。
    ''' </summary>
    ''' <param name="delta">模型空间之中的变化量</param>
    ''' <param name="idx">基因行下标</param>
    ''' <returns></returns>
    Private Function DeltaToRaw(delta As Double, idx As Integer) As Double
        If Not isStandardized Then
            Return delta
        Else
            Return rawSd(idx) * delta
        End If
    End Function

    ''' <summary>
    ''' 逐元素相加。
    ''' </summary>
    ''' <param name="baseline">基线向量</param>
    ''' <param name="delta">变化量向量</param>
    ''' <returns></returns>
    Private Shared Function AddDelta(baseline As Double(), delta As Double()) As Double()
        Dim n As Integer = baseline.Length
        Dim result As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            result(i) = baseline(i) + delta(i)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 构造基因编号到行下标的映射。
    ''' </summary>
    ''' <param name="names">基因编号列表</param>
    ''' <returns></returns>
    Private Shared Function BuildGeneIndex(names As String()) As Dictionary(Of String, Integer)
        Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To names.Length - 1
            If Not map.ContainsKey(names(i)) Then
                map.Add(names(i), i)
            End If
        Next

        If map.Count <> names.Length Then
            ' 存在仅大小写不同的基因编号，退回大小写敏感的精确匹配，避免合并
            map = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To names.Length - 1
                map(names(i)) = i
            Next
        End If

        Return map
    End Function

    ''' <summary>
    ''' 计算样本标准差(ddof = n - 1)。
    ''' </summary>
    ''' <param name="v">样本向量</param>
    ''' <returns></returns>
    Private Shared Function SampleStandardDeviation(v As Double()) As Double
        If v Is Nothing OrElse v.Length < 2 Then
            Return 0
        End If

        Dim mean As Double = v.Average
        Dim sum As Double = 0

        For i As Integer = 0 To v.Length - 1
            sum += (v(i) - mean) ^ 2
        Next

        Return std.Sqrt(sum / (v.Length - 1))
    End Function

    ''' <summary>
    ''' 从平方和与均值还原样本标准差。
    ''' </summary>
    ''' <param name="sumOfSquares">平方和</param>
    ''' <param name="mean">均值</param>
    ''' <param name="n">样本数量</param>
    ''' <returns></returns>
    Private Shared Function SampleStandardDeviation(sumOfSquares As Double, mean As Double, n As Integer) As Double
        If n < 2 Then
            Return 0
        End If

        Dim variance As Double = (sumOfSquares - n * mean * mean) / (n - 1)

        If variance <= 0 OrElse Not IsFinite(variance) Then
            Return 0
        Else
            Return std.Sqrt(variance)
        End If
    End Function

    ''' <summary>
    ''' 向量的 L2 范数。
    ''' </summary>
    ''' <param name="v"></param>
    ''' <returns></returns>
    Private Shared Function VectorNorm(v As Double()) As Double
        Dim sum As Double = 0

        For i As Integer = 0 To v.Length - 1
            sum += v(i) * v(i)
        Next

        Return std.Sqrt(sum)
    End Function

    ''' <summary>
    ''' 从标准正态分布之中采样一个随机数(Box-Muller 变换)。
    ''' </summary>
    ''' <param name="rng">随机数发生器</param>
    ''' <returns></returns>
    Private Shared Function Gauss(rng As Random) As Double
        Dim u1 As Double = rng.NextDouble()

        While u1 <= 0.0R
            u1 = rng.NextDouble()
        End While

        Return std.Sqrt(-2.0R * std.Log(u1)) * std.Cos(2.0R * std.PI * rng.NextDouble())
    End Function

    ''' <summary>
    ''' 判断一个数值是否是有限值。
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    Private Shared Function IsFinite(x As Double) As Boolean
        Return Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x)
    End Function

    ''' <summary>
    ''' 蒙特卡洛抽样的聚合结果。
    ''' </summary>
    Private Class MonteCarloResult

        ''' <summary>
        ''' 野生型组在各个抽样之上的均值(原始表达量尺度)。
        ''' </summary>
        Public Property WildtypeMeans As Double()

        ''' <summary>
        ''' 野生型组在各个抽样之上的样本标准差。
        ''' </summary>
        Public Property WildtypeSds As Double()

        ''' <summary>
        ''' 扰动组在各个抽样之上的均值(原始表达量尺度)。
        ''' </summary>
        Public Property MutantMeans As Double()

        ''' <summary>
        ''' 扰动组在各个抽样之上的样本标准差。
        ''' </summary>
        Public Property MutantSds As Double()

        ''' <summary>
        ''' 抽样过程之中是否出现了发散。
        ''' </summary>
        Public Property Diverged As Boolean

        ''' <summary>
        ''' 抽样过程之中被裁剪的基因行号。
        ''' </summary>
        Public Property Clipped As Integer()
    End Class
End Class
