''' <summary>
''' moGSVA 的运行配置。
''' </summary>
Public Class MultiOmicsGsvaOptions

    ''' <summary>
    ''' 组学内部的富集打分方法，推荐 <see cref="Methods.gsva"/>（默认）。
    ''' </summary>
    Public Property ScoreMethod As Methods = Methods.gsva

    ''' <summary>
    ''' 核密度估计类型：<see cref="KCDFs.Gaussian"/>（连续数据，默认）/
    ''' <see cref="KCDFs.Poisson"/>（计数数据）/ <see cref="KCDFs.none"/>（经验 CDF）。
    ''' </summary>
    Public Property Kcdf As KCDFs? = KCDFs.Gaussian

    ''' <summary>
    ''' 组学加权融合策略。
    ''' </summary>
    Public Property FusionStrategy As Fusion.FusionMethods = Fusion.FusionMethods.Equal

    ''' <summary>
    ''' 跨组学一致性奖励项的超参 λ（0 表示关闭，等价于纯粹的加权平均基线）。
    ''' </summary>
    Public Property ConsistencyLambda As Double = 0.25

    ''' <summary>
    ''' 是否叠加「组学特征数平衡项」<c>1/log2(2+|S_k|)</c>，用于抑制成员多的组学主导融合结果。
    ''' </summary>
    ''' <remarks>
    ''' 默认关闭：<see cref="Fusion.FusionMethods.Equal"/> 必须严格等于等权（Baseline 1），
    ''' 特征数平衡属于额外的风险控制项，需要显式开启。
    ''' </remarks>
    Public Property BalanceFeatureSize As Boolean = False

    ''' <summary>基因集最小尺寸（成员数，映射到特征之后）</summary>
    Public Property MinSize As Integer = 5

    ''' <summary>基因集最大尺寸（成员数，映射到特征之后）</summary>
    Public Property MaxSize As Integer = 500

    ''' <summary>ssGSEA 打分路径的 alpha 参数</summary>
    Public Property SsGseaAlpha As Double = 0.25

    ''' <summary>ssGSEA 打分路径是否做分数归一化</summary>
    Public Property SsGseaNormalize As Boolean = True

    ''' <summary>
    ''' 置换检验次数（0 表示跳过显著性评估）。
    ''' </summary>
    Public Property Permutations As Integer = 0

    ''' <summary>
    ''' 置换检验使用的打分方法，默认 ssGSEA（基于秩，避免重复做核密度估计）。
    ''' </summary>
    Public Property PermutationMethod As Methods = Methods.ssgsea

    ''' <summary>
    ''' 参与置换检验的通路子集；为空时取融合结果中的前
    ''' <see cref="MaxPermutationPathways"/> 条通路。
    ''' </summary>
    Public Property PermutationPathways As String()

    ''' <summary>
    ''' 置换检验视图中保留的特征数上限（0 表示不裁剪）。
    ''' 裁剪时优先保留目标通路的成员，再用固定种子补充背景特征。
    ''' </summary>
    Public Property PermutationGeneLimit As Integer = 0

    ''' <summary>未显式指定通路子集时，参与置换检验的最大通路数</summary>
    Public Property MaxPermutationPathways As Integer = 40

    ''' <summary>随机种子（派生数据、置换打乱均使用它保证可复现）</summary>
    Public Property Seed As Integer = 20261026

    ''' <summary>是否计算「拼接矩阵直接做单组学 GSVA」的消融对照</summary>
    Public Property ComputePooled As Boolean = True

    ''' <summary>是否输出过程日志</summary>
    Public Property Verbose As Boolean = True

    Public Function Clone() As MultiOmicsGsvaOptions
        Return New MultiOmicsGsvaOptions With {
            .ScoreMethod = ScoreMethod,
            .Kcdf = Kcdf,
            .FusionStrategy = FusionStrategy,
            .ConsistencyLambda = ConsistencyLambda,
            .BalanceFeatureSize = BalanceFeatureSize,
            .MinSize = MinSize,
            .MaxSize = MaxSize,
            .SsGseaAlpha = SsGseaAlpha,
            .SsGseaNormalize = SsGseaNormalize,
            .Permutations = Permutations,
            .PermutationMethod = PermutationMethod,
            .PermutationPathways = If(PermutationPathways Is Nothing, Nothing, DirectCast(PermutationPathways.Clone(), String())),
            .PermutationGeneLimit = PermutationGeneLimit,
            .MaxPermutationPathways = MaxPermutationPathways,
            .Seed = Seed,
            .ComputePooled = ComputePooled,
            .Verbose = Verbose
        }
    End Function

    ''' <summary>
    ''' 关闭一致性奖励项（用于生成 Baseline 1：逐组学打分后简单平均）。
    ''' </summary>
    Public Function NoConsistency() As MultiOmicsGsvaOptions
        Dim copy As MultiOmicsGsvaOptions = Clone()

        copy.ConsistencyLambda = 0

        Return copy
    End Function

End Class


