Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.Analysis.HTS.GSVA.Fusion
Imports SMRUCC.genomics.Analysis.HTS.GSVA.Omics
Imports std = System.Math

''' <summary>
''' 多组学 GSVA（moGSVA）测试套件。
''' </summary>
''' <remarks>
''' <para>两套用例：</para>
''' <list type="number">
''' <item><description>
''' 合成可控信号数据：验证融合数学（等权 = 逐层平均）、加权策略性质、
''' 一致性奖励的方向性、贡献分解、置换检验与确定性。
''' </description></item>
''' <item><description>
''' TCGA COAD 真实数据（mRNA + 确定性派生的 protein / methylation）：
''' 与 R GSVA 逐层逐元素比对（Baseline 1 = 逐层打分后平均），
''' 并尝试 MOGSA / multiGSEA 对照（缺失时记为 SKIP）。
''' </description></item>
''' </list>
''' <para>用法：<c>test.exe --mogsva [--dir=...] [--no-real] [--perm=20] [--install-baselines]</c></para>
''' </remarks>
Public Module MoGsvaTest

    Dim m_pass As Integer = 0
    Dim m_failures As New List(Of String)

    ''' <summary>测试入口</summary>
    Public Function Run(args As String()) As Integer
        Dim dataDir As String = MoGsvaData.DefaultDataDir
        Dim installBaselines As Boolean = False
        Dim skipReal As Boolean = False
        Dim permutations As Integer = 15

        ' 中间产物全部写在测试程序的输出目录，避免污染仓库中的数据目录
        Dim workDir As String = Path.Combine(AppContext.BaseDirectory, "mogsva_run")

        If args IsNot Nothing Then
            For Each arg As String In args
                If arg.StartsWith("--dir=", StringComparison.OrdinalIgnoreCase) Then
                    dataDir = arg.Substring("--dir=".Length)
                ElseIf arg.TextEquals("--install-baselines") Then
                    installBaselines = True
                ElseIf arg.TextEquals("--no-real") Then
                    skipReal = True
                ElseIf arg.StartsWith("--perm=", StringComparison.OrdinalIgnoreCase) Then
                    permutations = Integer.Parse(arg.Substring("--perm=".Length))
                End If
            Next
        End If

        Call "==========================================================".println
        Call " moGSVA 多组学 GSVA 测试".println
        Call "==========================================================".println
        Call $"data dir     : {dataDir}".println
        Call $"permutations : {permutations}".println

        Try
            Call SyntheticSuite(permutations)
        Catch ex As Exception
            Call check("合成数据套件", False, $"{ex.GetType.Name}: {ex.Message}")
            Call Console.WriteLine(ex.StackTrace)
        End Try

        If skipReal Then
            Call "[info] 已跳过真实数据套件 (--no-real)".println
        Else
            Try
                Call RealDataSuite(dataDir, workDir, installBaselines, permutations)
            Catch ex As Exception
                Call check("TCGA COAD 真实数据套件", False, $"{ex.GetType.Name}: {ex.Message}")
                Call Console.WriteLine(ex.StackTrace)
            End Try
        End If

        Call "==========================================================".println
        Call $" 通过 {m_pass} 项断言，失败 {m_failures.Count} 项".println

        If m_failures.Count > 0 Then
            For Each message As String In m_failures
                Call $"   - {message}".println
            Next

            Call "==========================================================".println
            Call Environment.Exit(1)
        Else
            Call " 全部通过!".println
            Call "==========================================================".println
        End If

        Return If(m_failures.Count = 0, 0, 1)
    End Function

    ' ==================================================================
    ' A. 合成可控信号数据
    ' ==================================================================

    Private Sub SyntheticSuite(permutations As Integer)
        Call "".println
        Call "--- A. 合成可控信号数据（融合逻辑正答） ---".println

        Dim planted As MoGsvaData.SyntheticDataset = MoGsvaData.BuildSynthetic()
        Dim baseOptions As New MultiOmicsGsvaOptions With {
            .ScoreMethod = Methods.gsva,
            .Kcdf = KCDFs.Gaussian,
            .FusionStrategy = FusionMethods.Equal,
            .ConsistencyLambda = 0,
            .MinSize = 10,
            .MaxSize = 500,
            .ComputePooled = True,
            .Permutations = 0,
            .Verbose = False
        }
        Dim resultEq As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(planted.Dataset, planted.GeneSets, baseOptions)

        Call check("合成数据: 3 个组学层 / 40 个样本",
                   planted.Dataset.LayerCount = 3 AndAlso planted.Dataset.SampleCount = 40,
                   planted.Dataset.ToString)
        Call check("合成数据: 4 条通路全部得到分数",
                   resultEq.Pathways.Length = 4 AndAlso planted.UpRegulated.All(Function(p) resultEq.ScoreVector(p) IsNot Nothing),
                   String.Join(", ", resultEq.Pathways))

        Dim flat As Double() = resultEq.Scores.expression.SelectMany(Function(r) r.experiments).ToArray
        Dim invalid As Integer = flat.Where(Function(x) Double.IsNaN(x) OrElse Double.IsInfinity(x)).Count()

        Call check("合成数据: 融合分数无 NaN / Inf", invalid = 0, $"{invalid} 个异常值 / {flat.Length}")
        Call check("合成数据: GSVA 融合分数落在 [-1, 1]",
                   flat.All(Function(x) x >= -1.000000001 AndAlso x <= 1.000000001),
                   $"range = [{flat.Min:F6}, {flat.Max:F6}]")
        Call check("合成数据: 每个组学层都产出了分数矩阵",
                   resultEq.OmicsNames.Length = 3 AndAlso resultEq.OmicsNames.All(Function(o) resultEq.LayerScores(o) IsNot Nothing),
                   String.Join(", ", resultEq.OmicsNames))
        Call check("合成数据: λ=0 时融合分数等于加权平均 (Scores = RawScores)",
                   MaxAbsDiff(resultEq.Scores, resultEq.RawScores) = 0,
                   $"max|diff| = {MaxAbsDiff(resultEq.Scores, resultEq.RawScores):E3}")

        ' --- 等权融合 = 逐层算术平均（Baseline 1 的恒等式）
        Dim eqDiff As Double = 0

        For Each pathway As String In resultEq.Pathways
            Dim manual As Double() = New Double(planted.SampleID.Length - 1) {}

            For Each omics As String In resultEq.OmicsNames
                Dim v As Double() = RowVector(resultEq.LayerScores(omics), pathway)

                For j As Integer = 0 To manual.Length - 1
                    manual(j) += v(j) / resultEq.OmicsNames.Length
                Next
            Next

            eqDiff = std.Max(eqDiff, MaxAbsDiff(manual, resultEq.ScoreVector(pathway)))
        Next

        Call check("等权融合 == 逐层分数的算术平均", eqDiff < 1.0E-12, $"max|diff| = {eqDiff:E3}")

        ' --- 权重与贡献分解
        Dim weightSumDiff As Double = 0
        Dim contribDiff As Double = 0
        Dim shareViolation As Integer = 0

        For Each pathway As String In resultEq.Pathways
            Dim total As Double = 0

            For Each omics As String In resultEq.OmicsNames
                total += resultEq.Weights(pathway)(omics)
            Next

            weightSumDiff = std.Max(weightSumDiff, std.Abs(total - 1))

            Dim raw As Double() = RowVector(resultEq.RawScores, pathway)

            For j As Integer = 0 To raw.Length - 1
                Dim sum As Double = 0
                Dim absSum As Double = 0

                For Each omics As String In resultEq.OmicsNames
                    Dim c As Double = resultEq.Contributions(pathway)(omics)(j)
                    Dim s As Double = resultEq.ContributionShare(pathway)(omics)(j)

                    sum += c
                    absSum += std.Abs(s)
                Next

                contribDiff = std.Max(contribDiff, std.Abs(sum - raw(j)))

                If std.Abs(absSum - 1) > 1.0E-9 Then
                    shareViolation += 1
                End If
            Next
        Next

        Call check("融合权重逐通路归一化 (Σw = 1)", weightSumDiff < 1.0E-12, $"max|1-Σw| = {weightSumDiff:E3}")
        Call check("贡献分解可加性 (Σ c_k = ES_raw)", contribDiff < 1.0E-12, $"max|diff| = {contribDiff:E3}")
        Call check("贡献占比绝对值和为 1", shareViolation = 0, $"{shareViolation} 个样本违反")

        Dim dominantOk As Boolean = resultEq.Pathways.All(
            Function(p)
                Return resultEq.DominantOmics(p).All(Function(o) resultEq.OmicsNames.Contains(o))
            End Function)

        Call check("主导组学标签落在已知组学集合内", dominantOk, "dominant omics ∈ {mRNA, protein, methylation}")

        ' --- 植入信号方向性
        Dim shiftA As Double = Shift(resultEq, "setA", planted.GroupSize)
        Dim shiftB As Double = Shift(resultEq, "setB", planted.GroupSize)
        Dim shiftD As Double = Shift(resultEq, "setD", planted.GroupSize)

        Call $"[info] group2-group1 shift: setA={shiftA:F4}, setB={shiftB:F4}, setD={shiftD:F4}".println
        Call check("植入信号: setA 融合分数组间差为正", shiftA > 0.02, $"shift = {shiftA:F4}")
        Call check("植入信号: 三层一致的 setA 强于仅单组学的 setB", shiftA > shiftB + 0.01, $"setA={shiftA:F4}, setB={shiftB:F4}")
        Call check("植入信号: setD(methylation 方向相反) 的融合移位弱于 setA", std.Abs(shiftD) < std.Abs(shiftA), $"setA={shiftA:F4}, setD={shiftD:F4}")

        ' --- 一致性奖励
        Dim consOptions As MultiOmicsGsvaOptions = baseOptions.Clone()

        consOptions.ConsistencyLambda = 0.5

        Dim resultCons As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(planted.Dataset, planted.GeneSets, consOptions)
        Dim consA As Double = RowMean(resultCons.Consistency, "setA")
        Dim consD As Double = RowMean(resultCons.Consistency, "setD")

        Call $"[info] mean consistency: setA={consA:F4}, setD={consD:F4}".println
        Call check("一致性奖励: 一致性向量落在 [-1, 1]",
                   resultCons.Consistency.expression.SelectMany(Function(r) r.experiments).All(Function(x) x >= -1.000000001 AndAlso x <= 1.000000001),
                   $"setA={consA:F4}, setD={consD:F4}")
        Call check("一致性奖励: 三层同向的 setA 一致度高于方向矛盾的 setD", consA > consD, $"setA={consA:F4}, setD={consD:F4}")

        Dim ratioA As Double = MeanAbs(resultCons.ScoreVector("setA")) / MeanAbs(resultEq.ScoreVector("setA"))
        Dim ratioD As Double = MeanAbs(resultCons.ScoreVector("setD")) / MeanAbs(resultEq.ScoreVector("setD"))

        Call $"[info] 一致性奖励的相对压缩率: setA={ratioA:F4}, setD={ratioD:F4}".println
        Call check("一致性奖励: 矛盾通路被压缩得比一致通路更狠", ratioD < ratioA, $"setA={ratioA:F4}, setD={ratioD:F4}")
        Call check("一致性奖励: 压缩不会放大分数", ratioA <= 1.000000001 AndAlso ratioD <= 1.000000001, $"setA={ratioA:F4}, setD={ratioD:F4}")

        ' --- 权重策略的数学性质（直接作用于权重计算器）
        Dim demos As New Dictionary(Of String, Double()) From {
            {"steady", {1.0, 1.0, 1.0, 1.0}},
            {"noisy", {0.1, 1.9, 0.2, 1.8}}
        }
        Dim varianceWeights As Dictionary(Of String, Double) = WeightCalculators.Compute(demos, Nothing, FusionMethods.Variance, False)

        Call check("方差加权: 噪声小的组学获得更大权重",
                   varianceWeights("steady") > varianceWeights("noisy"),
                   $"steady={varianceWeights("steady"):F4}, noisy={varianceWeights("noisy"):F4}")

        Dim members As New Dictionary(Of String, Integer) From {{"steady", 20}, {"noisy", 400}}
        Dim balancedWeights As Dictionary(Of String, Double) = WeightCalculators.Compute(demos, members, FusionMethods.Balanced, False)

        Call check("特征数平衡: 成员少的组学获得更大权重",
                   balancedWeights("steady") > balancedWeights("noisy"),
                   $"steady={balancedWeights("steady"):F4}, noisy={balancedWeights("noisy"):F4}")
        Call check("特征数平衡项 1/log2(2+n) 单调递减",
                   WeightCalculators.BalanceFactor(10) > WeightCalculators.BalanceFactor(100),
                   $"n=10 -> {WeightCalculators.BalanceFactor(10):F6}, n=100 -> {WeightCalculators.BalanceFactor(100):F6}")
        Call check("熵: 常数向量的熵为 0", WeightCalculators.BinnedEntropy({0.5, 0.5, 0.5, 0.5}) = 0,
                   $"{WeightCalculators.BinnedEntropy({0.5, 0.5, 0.5, 0.5}):F6}")
        Call check("熵: 分散向量的熵为正", WeightCalculators.BinnedEntropy({0.0, 0.3, 0.6, 1.0}) > 0,
                   $"{WeightCalculators.BinnedEntropy({0.0, 0.3, 0.6, 1.0}):F6}")

        Dim ramp As Double() = {1, 2, 3, 4, 5, 6, 7, 8}
        Dim flatY As Double() = {1, 2, 1, 2, 1, 2, 1, 2}

        Call check("互信息: 自身互信息大于与粗糙变量的互信息",
                   WeightCalculators.MutualInformation(ramp, ramp) > WeightCalculators.MutualInformation(ramp, flatY),
                   $"MI(x,x)={WeightCalculators.MutualInformation(ramp, ramp):F4}, MI(x,y)={WeightCalculators.MutualInformation(ramp, flatY):F4}")

        ' --- 五种融合策略都能端到端跑通，且权重归一化 / 分数有限
        Dim strategies As FusionMethods() = {
            FusionMethods.Equal,
            FusionMethods.Variance,
            FusionMethods.Entropy,
            FusionMethods.MutualInformation,
            FusionMethods.Balanced
        }

        For Each strategy As FusionMethods In strategies
            Dim strategyOptions As MultiOmicsGsvaOptions = baseOptions.Clone()

            strategyOptions.FusionStrategy = strategy
            strategyOptions.ComputePooled = False

            Dim strategyResult As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(planted.Dataset, planted.GeneSets, strategyOptions)
            Dim weightOk As Boolean = strategyResult.Pathways.All(
                Function(p)
                    Dim sum As Double = strategyResult.OmicsNames.Sum(Function(o) strategyResult.Weights(p)(o))

                    Return std.Abs(sum - 1) < 1.0E-12
                End Function)
            Dim valueOk As Boolean = strategyResult.Scores.expression _
                .SelectMany(Function(r) r.experiments) _
                .All(Function(x) Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x))

            Call check($"融合策略 {strategy}: 权重归一化且分数有限",
                       weightOk AndAlso valueOk,
                       $"{strategyResult.Pathways.Length} 条通路")
        Next

        ' --- 消融对照
        Call check("消融对照: 拼接矩阵单组学 GSVA 可运行",
                   resultEq.Pooled IsNot Nothing AndAlso resultEq.Pooled.Available,
                   If(resultEq.Pooled Is Nothing, "null", resultEq.Pooled.ToString))
        Call check("消融对照: 与分组学融合结果存在差异",
                   resultEq.Pooled IsNot Nothing AndAlso resultEq.Pooled.Available AndAlso resultEq.Pooled.MeanAbsoluteDifference > 1.0E-6,
                   $"mean|diff| = {If(resultEq.Pooled Is Nothing, Double.NaN, resultEq.Pooled.MeanAbsoluteDifference):E3}")

        ' --- 置换检验
        Dim permOptions As MultiOmicsGsvaOptions = baseOptions.Clone()

        permOptions.Permutations = permutations
        permOptions.PermutationPathways = {"setA", "setB"}
        permOptions.PermutationGeneLimit = 300
        permOptions.PermutationMethod = Methods.ssgsea

        Dim permuted As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(planted.Dataset, planted.GeneSets, permOptions)
        Dim pA As Double = permuted.PValueOf("setA")

        Call check("置换检验: 产生 p 值表", permuted.PValues IsNot Nothing AndAlso permuted.PValues.Count > 0,
                   $"{If(permuted.PValues, New Dictionary(Of String, Double)()).Count} 条通路")
        Call check("置换检验: p 值落在 (0, 1]",
                   permuted.PValues.All(Function(p) p.Value > 0 AndAlso p.Value <= 1),
                   String.Join(", ", permuted.PValues.Select(Function(p) $"{p.Key}={p.Value:F4}")))
        Call check("置换检验: 植入信号 setA 被检出", pA <= 0.1, $"p(setA) = {pA:F4}")
        Call check("置换检验: q >= p (BH 校正)",
                   permuted.PValues.All(Function(p) permuted.QValueOf(p.Key) >= p.Value - 1.0E-12),
                   $"min q = {permuted.QValues.Values.Min:F4}")
        Call check("置换检验: 零分布均值接近 0",
                   permuted.Significance.NullStatistics("setA").Average < 0.6,
                   $"mean null statistic = {permuted.Significance.NullStatistics("setA").Average:F4} (observed = {permuted.Significance.ObservedStatistics("setA"):F4})")

        ' --- 确定性
        Dim repeated As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(planted.Dataset, planted.GeneSets, baseOptions)
        Dim rebuilt As MoGsvaData.SyntheticDataset = MoGsvaData.BuildSynthetic()

        Call check("确定性: 同配置复跑逐位一致", MaxAbsDiff(resultEq.Scores, repeated.Scores) = 0,
                   $"max|diff| = {MaxAbsDiff(resultEq.Scores, repeated.Scores):E3}")
        Call check("确定性: 固定种子重建合成数据逐位一致",
                   planted.Dataset.Layers(0).Expr.expression _
                       .Select(Function(r, i) r.experiments.SequenceEqual(rebuilt.Dataset.Layers(0).Expr.expression(i).experiments)) _
                       .All(Function(x) x),
                   $"{planted.Dataset.Layers(0).FeatureCount} 行")
    End Sub

    ' ==================================================================
    ' B. TCGA COAD 真实数据
    ' ==================================================================

    Private Sub RealDataSuite(dataDir As String, workDir As String, installBaselines As Boolean, permutations As Integer)
        Call "".println
        Call "--- B. TCGA COAD（真实 mRNA + 确定性派生 protein / methylation） ---".println
        Call $"[info] 中间产物目录: {workDir}".println

        Dim real As MoGsvaData.RealDataset = MoGsvaData.BuildRealDataset(dataDir, MoGsvaData.DefaultSeed, True, True)

        Call check("真实数据: 样本数 = 160", real.SampleID.Length = 160, $"{real.SampleID.Length} 个样本")
        Call check("真实数据: 特征过滤后仍有足够特征", real.GeneCount > 100, $"{real.GeneCount} 个特征（原始 {real.RawGenes}）")
        Call check("真实数据: 三个组学层样本轴一致",
                   real.Dataset.Layers.All(Function(l) l.SampleID.SequenceEqual(real.SampleID)),
                   $"{real.Dataset.LayerCount} 层 × {real.Dataset.SampleCount} 样本")
        Call check("真实数据: 蛋白层 / 甲基化层为基因子集",
                   real.Dataset.GetLayer("protein").FeatureCount < real.Dataset.GetLayer("mRNA").FeatureCount AndAlso
                   real.Dataset.GetLayer("methylation").FeatureCount < real.Dataset.GetLayer("mRNA").FeatureCount,
                   $"mRNA={real.Dataset.GetLayer("mRNA").FeatureCount}, protein={real.Dataset.GetLayer("protein").FeatureCount}, methylation={real.Dataset.GetLayer("methylation").FeatureCount}")

        ' 派生过程必须可复现
        Dim rebuilt As MoGsvaData.RealDataset = MoGsvaData.BuildRealDataset(dataDir, MoGsvaData.DefaultSeed, True, False)
        Dim sameDerived As Boolean = rebuilt.Dataset.GetLayer("protein").FeatureCount = real.Dataset.GetLayer("protein").FeatureCount AndAlso
            rebuilt.Dataset.GetLayer("protein").Expr.expression(0).experiments.SequenceEqual(real.Dataset.GetLayer("protein").Expr.expression(0).experiments)

        Call check("真实数据: 派生组学层可复现", sameDerived, "固定种子重建后首行逐位一致")

        Dim options As New MultiOmicsGsvaOptions With {
            .ScoreMethod = Methods.gsva,
            .Kcdf = KCDFs.Gaussian,
            .FusionStrategy = FusionMethods.Equal,
            .ConsistencyLambda = 0,
            .MinSize = 1,
            .MaxSize = Integer.MaxValue,
            .ComputePooled = True,
            .Verbose = True
        }
        Dim result As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(real.Dataset, real.GeneSets, options)

        Call check("真实数据: 得到通路分数矩阵", result.Pathways.Length > 10, $"{result.Pathways.Length} 条通路")

        Dim flat As Double() = result.Scores.expression.SelectMany(Function(r) r.experiments).ToArray

        Call check("真实数据: 融合分数无 NaN / Inf",
                   flat.Where(Function(x) Double.IsNaN(x) OrElse Double.IsInfinity(x)).Count() = 0,
                   $"{flat.Length} 个分数")
        Call check("真实数据: λ=0 时融合分数等于逐层平均",
                   MaxAbsDiff(result.Scores, result.RawScores) = 0,
                   $"max|diff| = {MaxAbsDiff(result.Scores, result.RawScores):E3}")

        ' ---------------- R 基线 ----------------
        Call "".println
        Call "--- B1. R 基线（逐组学 GSVA + 平均） ---".println

        Call MoGsvaBaseline.PrepareInputs(real, Path.Combine(workDir, "mogsva_input"))

        Dim baseline As MoGsvaBaseline.BaselineResult = MoGsvaBaseline.Run(workDir, installBaselines, True)

        If Not baseline.Available Then
            Call check("R 基线: 生成成功", False, If(baseline.Message, "unknown error"))
            Return
        End If

        Call check("R 基线: 生成成功", True,
                   $"layers = {baseline.LayerScores.Count}, status = {String.Join("; ", baseline.Status.Select(Function(p) $"{p.Key}:{p.Value}"))}")

        For Each layer As OmicsLayer In real.Dataset.Layers
            Dim reference As Dictionary(Of String, Double()) = Nothing

            If Not baseline.LayerScores.TryGetValue(layer.Name, reference) Then
                Call check($"{layer.Name}: 与 R GSVA 逐元素一致", False, "缺少 R 参考分数")
                Continue For
            End If

            Dim cmp As MoGsvaBaseline.CompareResult = MoGsvaBaseline.Compare(result.LayerScores(layer.Name), reference, layer.Direction)

            Call check($"{layer.Name}: 与 R GSVA 逐元素一致",
                       cmp.BadPathways = 0 AndAlso cmp.MissingPathways = 0,
                       $"{cmp.Details}, 越界通路 {cmp.BadPathways}, 缺失 {cmp.MissingPathways}")
        Next

        ' Baseline 1：R 逐层分数的（带方向）平均
        Dim meanDiff As Double = 0
        Dim compared As Integer = 0

        For Each pathway As String In result.Pathways
            Dim referenceMean As Double() = Nothing
            Dim ok As Boolean = True

            For Each layer As OmicsLayer In real.Dataset.Layers
                Dim reference As Dictionary(Of String, Double()) = Nothing

                If Not baseline.LayerScores.TryGetValue(layer.Name, reference) OrElse Not reference.ContainsKey(pathway) Then
                    ok = False
                    Exit For
                End If

                If referenceMean Is Nothing Then
                    referenceMean = New Double(reference(pathway).Length - 1) {}
                End If

                Dim v As Double() = reference(pathway)

                For j As Integer = 0 To referenceMean.Length - 1
                    referenceMean(j) += layer.Direction * v(j) / real.Dataset.LayerCount
                Next
            Next

            If Not ok Then
                Continue For
            End If

            meanDiff = std.Max(meanDiff, MaxAbsDiff(referenceMean, result.ScoreVector(pathway)))
            compared += 1
        Next

        Call check("Baseline 1: 等权融合 == R 逐层打分后平均",
                   compared > 10 AndAlso meanDiff < MoGsvaBaseline.TOLERANCE,
                   $"compared {compared} pathway(s), max|diff| = {meanDiff:E3}")

        ' ---------------- 可选基线 ----------------
        Call "".println
        Call "--- B2. 可选基线（MOGSA / multiGSEA） ---".println

        Call ReportOptionalBaselines(baseline, result)

        ' ---------------- 显著性评估 ----------------
        Call "".println
        Call "--- B3. 置换检验（真实数据，取高活性通路子集） ---".println

        If permutations <= 0 Then
            Call check("真实数据: 跳过置换检验 (--perm=0)", True, "未请求置换")
        Else
            ' 只对活性最强的通路做置换，并在缩小后的基因视图上打分，控制耗时
            Dim topPathways As String() = result.Pathways _
                .OrderByDescending(Function(id) MeanAbs(result.ScoreVector(id))) _
                .Take(10) _
                .ToArray
            Dim permOptions As MultiOmicsGsvaOptions = options.Clone()

            permOptions.Permutations = permutations
            permOptions.PermutationMethod = Methods.ssgsea
            permOptions.PermutationPathways = topPathways
            permOptions.PermutationGeneLimit = 3000
            permOptions.Verbose = True

            Call MultiOmicsGsva.ComputeSignificance(result, real.Dataset, real.GeneSets, permOptions)

            Dim evaluated As String() = If(result.PValues Is Nothing, New String() {}, result.PValues.Keys.ToArray)

            Call check("真实数据: 置换检验产出 p 值", evaluated.Length > 0, $"{evaluated.Length} 条通路（B={permutations}）")
            Call check("真实数据: p 值落在 (0, 1]",
                       evaluated.Length > 0 AndAlso evaluated.All(Function(id) result.PValueOf(id) > 0 AndAlso result.PValueOf(id) <= 1),
                       String.Join(", ", evaluated.Take(5).Select(Function(id) $"{id}={result.PValueOf(id):F3}")))
            Call check("真实数据: q 值不小于 p 值",
                       evaluated.Length > 0 AndAlso evaluated.All(Function(id) result.QValueOf(id) >= result.PValueOf(id) - 1.0E-12),
                       $"min q = {If(result.QValues Is Nothing, Double.NaN, result.QValues.Values.Min):F4}")
            Call check("真实数据: 置换零分布已记录",
                       result.Significance IsNot Nothing AndAlso result.Significance.NullStatistics.Count = evaluated.Length,
                       $"{(If(result.Significance Is Nothing, New Dictionary(Of String, Double()), result.Significance.NullStatistics)).Count} 条通路")
        End If

        ' ---------------- 消融对照 ----------------
        Call "".println
        Call "--- B4. 消融对照（拼接矩阵单组学 GSVA） ---".println
        Call check("真实数据: 拼接矩阵 GSVA 可运行",
                   result.Pooled IsNot Nothing AndAlso result.Pooled.Available,
                   If(result.Pooled Is Nothing, "null", result.Pooled.ToString))
        Call check("真实数据: 拼接基线不同于分组学融合",
                   result.Pooled IsNot Nothing AndAlso result.Pooled.Available AndAlso result.Pooled.MeanAbsoluteDifference > 1.0E-6,
                   $"mean|diff| = {If(result.Pooled Is Nothing, Double.NaN, result.Pooled.MeanAbsoluteDifference):E3}")

        ' ---------------- 结果导出 ----------------
        Dim outDir As String = Path.Combine(workDir, "mogsva_output")
        Dim files As String() = SMRUCC.genomics.Analysis.HTS.GSVA.Export.MultiOmicsResultWriter.Save(result, outDir)

        Call check("结果导出: 写出全部结果表",
                   files.Length >= 8 AndAlso files.All(Function(f) File.Exists(f)),
                   String.Join(", ", files.Select(Function(f) Path.GetFileName(f))))
    End Sub

    Private Sub ReportOptionalBaselines(baseline As MoGsvaBaseline.BaselineResult, result As MultiOmicsGsvaResult)
        ' --- MOGSA：同样是样本级通路打分，可直接与我们的融合分数比较相关
        Dim mogsaStatus As String = Nothing

        If Not baseline.Status.TryGetValue("mogsa", mogsaStatus) Then
            Call check("mogsa: 基线状态已知", False, "R 脚本未报告该基线")
        ElseIf Not mogsaStatus.StartsWith("OK") Then
            Call check("mogsa: 基线跳过（不影响结论）", True, mogsaStatus)
        Else
            Dim reference As Dictionary(Of String, Double()) = MoGsvaBaseline.ReadReferenceMatrix(Path.Combine(baseline.OutputDir, "mogsa_scores.csv"))
            Dim commonPathways As String() = reference.Keys.Where(Function(id) result.ScoreVector(id) IsNot Nothing).ToArray

            Call check("mogsa: 结果矩阵非空", reference.Count > 0, $"{reference.Count} 行")

            If commonPathways.Length < 3 Then
                Call check("mogsa: 与 moGSVA 存在可比通路", False, $"共享通路仅 {commonPathways.Length} 条")
            Else
                Dim r As Double = CorrelationOfMeans(result, reference, commonPathways)

                Call check("mogsa: 与 moGSVA 通路活性可比对（相关性可计算）",
                           Not Double.IsNaN(r),
                           $"pearson r = {r:F4} over {commonPathways.Length} shared pathway(s)")

                If Not Double.IsNaN(r) AndAlso std.Abs(r) < 0.3 Then
                    Call $"[info] MOGSA 与 moGSVA 的相关性偏低 (r = {r:F3})：MOGSA 走 MFA 降维后打分，属范式差异。".println
                End If
            End If
        End If

        ' --- multiGSEA：队列级 p 值（各层 fgsea 后合并），只能比较"通路排序的一致性"
        Dim multiStatus As String = Nothing

        If Not baseline.Status.TryGetValue("multiGSEA", multiStatus) Then
            Call check("multiGSEA: 基线状态已知", False, "R 脚本未报告该基线")
        ElseIf Not multiStatus.StartsWith("OK") Then
            Call check("multiGSEA: 基线跳过（不影响结论）", True, multiStatus)
        Else
            Dim table As Dictionary(Of String, Dictionary(Of String, Double)) =
                MoGsvaBaseline.ReadPValueTable(Path.Combine(baseline.OutputDir, "multigsea_pvalues.csv"))

            Call check("multiGSEA: 结果表非空", table.Count > 0, $"{table.Count} 条通路")

            Dim theirs As String() = table _
                .Where(Function(p) p.Value.ContainsKey("combined_pval") AndAlso Not Double.IsNaN(p.Value("combined_pval"))) _
                .OrderBy(Function(p) p.Value("combined_pval")) _
                .Take(10) _
                .Select(Function(p) p.Key) _
                .ToArray
            Dim ours As String() = result.Pathways _
                .Where(Function(id) result.ScoreVector(id) IsNot Nothing) _
                .OrderByDescending(Function(id) MeanAbs(result.ScoreVector(id))) _
                .Take(10) _
                .ToArray
            Dim overlap As Integer = theirs.Intersect(ours, StringComparer.Ordinal).Count()

            Call check("multiGSEA: 队列级显著通路与 moGSVA 高活性通路排序有重叠",
                       theirs.Length > 0 AndAlso ours.Length > 0 AndAlso overlap >= 1,
                       $"top10 overlap = {overlap} / {std.Min(theirs.Length, ours.Length)}")
            Call $"[info] multiGSEA top10: {String.Join(", ", theirs.Take(5))}".println
            Call $"[info] moGSVA   top10: {String.Join(", ", ours.Take(5))}".println
        End If
    End Sub

    ''' <summary>比较两条通路活性向量（各取样本均值）的 Pearson 相关</summary>
    Private Function CorrelationOfMeans(result As MultiOmicsGsvaResult,
                                        reference As Dictionary(Of String, Double()),
                                        pathways As String()) As Double

        Dim ours As Double() = pathways.Select(Function(id) result.ScoreVector(id).Average).ToArray
        Dim theirs As Double() = pathways.Select(Function(id) reference(id).Average).ToArray

        Return Pearson(ours, theirs)
    End Function

    ' ==================================================================
    ' 工具
    ' ==================================================================

    Private Function Pearson(x As Double(), y As Double()) As Double
        Dim n As Integer = std.Min(x.Length, y.Length)

        If n < 3 Then
            Return Double.NaN
        End If

        Dim mx As Double = x.Take(n).Average
        Dim my As Double = y.Take(n).Average
        Dim sxy As Double = 0
        Dim sxx As Double = 0
        Dim syy As Double = 0

        For i As Integer = 0 To n - 1
            Dim dx As Double = x(i) - mx
            Dim dy As Double = y(i) - my

            sxy += dx * dy
            sxx += dx * dx
            syy += dy * dy
        Next

        If sxx <= 0 OrElse syy <= 0 Then
            Return Double.NaN
        End If

        Return sxy / std.Sqrt(sxx * syy)
    End Function

    Private Function RowVector(mat As Matrix, id As String) As Double()
        Dim row As DataFrameRow = mat.expression.FirstOrDefault(Function(r) r.geneID = id)

        If row Is Nothing Then
            Return Nothing
        Else
            Return row.experiments
        End If
    End Function

    Private Function RowMean(mat As Matrix, id As String) As Double
        Dim v As Double() = RowVector(mat, id)

        If v Is Nothing Then
            Return Double.NaN
        Else
            Return v.Average
        End If
    End Function

    ''' <summary>第二组均值 - 第一组均值</summary>
    Private Function Shift(result As MultiOmicsGsvaResult, pathway As String, groupSize As Integer) As Double
        Dim v As Double() = result.ScoreVector(pathway)

        If v Is Nothing Then
            Return Double.NaN
        End If

        Return v.Skip(groupSize).Average - v.Take(groupSize).Average
    End Function

    Private Function MeanAbs(v As Double()) As Double
        Return v.Select(Function(x) std.Abs(x)).Average
    End Function

    Private Function MaxAbsDiff(a As Matrix, b As Matrix) As Double
        Dim max As Double = 0

        For i As Integer = 0 To std.Min(a.size, b.size) - 1
            max = std.Max(max, MaxAbsDiff(a.expression(i).experiments, b.expression(i).experiments))
        Next

        Return max
    End Function

    Private Function MaxAbsDiff(a As Double(), b As Double()) As Double
        Dim max As Double = 0

        If a Is Nothing OrElse b Is Nothing Then
            Return Double.NaN
        End If

        For i As Integer = 0 To std.Min(a.Length, b.Length) - 1
            Dim d As Double = std.Abs(a(i) - b(i))

            If Double.IsNaN(d) Then
                Return Double.NaN
            End If
            If d > max Then
                max = d
            End If
        Next

        Return max
    End Function

    Private Sub check(name As String, ok As Boolean, detail As String)
        If ok Then
            m_pass += 1
            Call $"  [PASS] {name}  ({detail})".println
        Else
            m_failures.Add($"{name}: {detail}")
            Call $"  [FAIL] {name}  ({detail})".println
        End If
    End Sub

    <Extension>
    Private Sub println(msg As String)
        Call Console.WriteLine(msg)
    End Sub

End Module
