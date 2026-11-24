Imports SMRUCC.genomics.Analysis.HTS.GSEA

''' <summary>
''' 多组学 GSVA 的门面（method.md 五步流水线的串联入口）。
''' </summary>
''' <remarks>
''' <code>
''' Dim dataset = Omics.MultiOmicsDataset.Create({
'''     Omics.OmicsLayer.Create("mRNA", rna),
'''     Omics.OmicsLayer.Create("protein", protein)
''' })
''' Dim sets = Omics.GeneSetTable.LoadGmt("kegg.gmt")
''' Dim result = MultiOmicsGsva.Compute(dataset, sets, New MultiOmicsGsvaOptions With {.FusionStrategy = Fusion.FusionMethods.Variance})
''' </code>
''' </remarks>
Public Module MultiOmicsGsva

    ''' <summary>
    ''' 执行完整的多组学 GSVA 分析。
    ''' </summary>
    ''' <param name="dataset">已对齐样本轴的多组学数据集</param>
    ''' <param name="geneSets">基因集</param>
    ''' <param name="options">运行配置</param>
    Public Function Compute(dataset As Omics.MultiOmicsDataset,
                            geneSets As Background,
                            options As MultiOmicsGsvaOptions) As MultiOmicsGsvaResult

        If dataset Is Nothing Then
            Throw New ArgumentNullException(NameOf(dataset))
        End If
        If geneSets Is Nothing Then
            Throw New ArgumentNullException(NameOf(geneSets))
        End If
        If options Is Nothing Then
            options = New MultiOmicsGsvaOptions()
        End If

        Dim clock As Stopwatch = Stopwatch.StartNew()

        If options.Verbose Then
            Call $"[moGSVA] {dataset}".info
            Call $"[moGSVA] {geneSets.clusters.Length} gene set(s), score method={options.ScoreMethod}, fusion={options.FusionStrategy}, lambda={options.ConsistencyLambda}".info
        End If

        ' 消融对照与逐层打分都是 CPU 密集型且各自内部已经并行。
        ' 实测（BRCA 460 样本 / 24 逻辑核）：并发执行 31.8 s、串行执行 35.7 s，
        ' 并发约有 11% 的墙钟收益，但峰值内存由 1.6 GB 升到 2.1 GB（同时持有拼接矩阵
        ' 与各层中间量），样本量更大的数据集会成倍放大内存压力，因此默认保持串行。
        Dim pooledScores As Ablation.PooledBaselineResult = Nothing

        ' 步骤2 + 步骤3：逐组学层估计分布并做富集打分
        Dim scoreSet As PerOmics.OmicsScoreSet = PerOmics.OmicsScorer.Score(dataset, geneSets, options)

        ' 步骤4：组学加权融合 + 跨组学一致性奖励
        Dim fused As Fusion.MultiOmicsFusionResult = Fusion.MultiOmicsFusion.Compute(scoreSet, options)

        Dim result As New MultiOmicsGsvaResult With {
            .SampleID = dataset.SampleID,
            .Pathways = fused.Pathways,
            .Scores = fused.Scores,
            .RawScores = fused.RawScores,
            .Consistency = fused.Consistency,
            .LayerScores = fused.LayerScores,
            .Weights = fused.Weights,
            .Contributions = fused.Contributions,
            .ContributionShare = fused.ContributionShare,
            .DominantOmics = fused.DominantOmics,
            .Coverage = fused.Coverage,
            .SkippedLayers = fused.SkippedLayers,
            .OmicsNames = dataset.OmicsNames,
            .Options = options
        }

        ' 消融对照
        If options.ComputePooled Then
            pooledScores = Ablation.PooledBaseline.Compute(dataset, geneSets, options)
            result.Pooled = Ablation.PooledBaseline.Diagnose(pooledScores, fused.Scores, options.Verbose)
        End If

        ' 步骤5：置换检验 + 多重检验校正
        If options.Permutations > 0 Then
            Call ComputeSignificance(result, dataset, geneSets, options)
        End If

        clock.Stop()
        result.ElapsedMilliseconds = clock.ElapsedMilliseconds

        If options.Verbose Then
            Call $"[moGSVA] done in {result.ElapsedMilliseconds} ms: {result.Pathways.Length} pathway(s) x {dataset.SampleCount} sample(s)".info
        End If

        Return result
    End Function

    ''' <summary>
    ''' 只做逐组学打分与融合（不做消融与置换），便于参数扫描。
    ''' </summary>
    Public Function Fuse(scoreSet As PerOmics.OmicsScoreSet, options As MultiOmicsGsvaOptions) As Fusion.MultiOmicsFusionResult
        Return Fusion.MultiOmicsFusion.Compute(scoreSet, If(options, New MultiOmicsGsvaOptions()))
    End Function

    ''' <summary>
    ''' 在既有评分结果上单独执行置换检验，并把 p / q 值写回结果对象。
    ''' </summary>
    ''' <param name="result">已完成的评分结果</param>
    ''' <param name="dataset">多组学数据集</param>
    ''' <param name="geneSets">基因集</param>
    ''' <param name="options">置换配置（<see cref="MultiOmicsGsvaOptions.Permutations"/> 必须大于 0）</param>
    ''' <remarks>
    ''' 逐组学打分代价较高，若只想补做显著性评估，应复用本方法而不是重跑 <see cref="Compute"/>。
    ''' </remarks>
    Public Function ComputeSignificance(result As MultiOmicsGsvaResult,
                                        dataset As Omics.MultiOmicsDataset,
                                        geneSets As Background,
                                        options As MultiOmicsGsvaOptions) As MultiOmicsGsvaResult
        If result Is Nothing Then
            Throw New ArgumentNullException(NameOf(result))
        End If

        Dim significance As StatTests.PermutationResult = StatTests.PermutationTest.Run(dataset, geneSets, options, result.Scores)

        result.Significance = significance

        If significance Is Nothing OrElse significance.PValues.Count = 0 Then
            Return result
        End If

        Dim keys As String() = significance.Pathways
        Dim raw As Double() = keys.Select(Function(id) significance.PValues(id)).ToArray
        Dim adjusted As Double() = StatTests.MultipleTesting.BenjaminiHochberg(raw)
        Dim pValues As New Dictionary(Of String, Double)(StringComparer.Ordinal)
        Dim qValues As New Dictionary(Of String, Double)(StringComparer.Ordinal)

        For i As Integer = 0 To keys.Length - 1
            pValues(keys(i)) = raw(i)
            qValues(keys(i)) = adjusted(i)
        Next

        result.PValues = pValues
        result.QValues = qValues

        Return result
    End Function

End Module


