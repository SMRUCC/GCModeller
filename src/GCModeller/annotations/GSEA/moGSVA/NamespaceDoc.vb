Namespace Global.SMRUCC.genomics.Analysis.HTS.GSVA

    ''' <summary>
    ''' 多组学基因集变异分析（multiple-omics GSVA, moGSVA）
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 本命名空间在既有的单组学 <c>GSVA</c> 模块之上叠加一层「组学维度」，
    ''' 用于处理拼接合并后的多组学矩阵（mRNA / methylation / protein / metabolome ...）。
    ''' 算法严格遵循 <c>method.md</c> 的五步流水线：
    ''' </para>
    ''' <list type="number">
    ''' <item>
    ''' <term>步骤1 输入规范与预处理对齐</term>
    ''' <description>
    ''' 见 <see cref="Omics.MultiOmicsDataset"/>：K 个组学层的样本轴严格对齐，
    ''' 特征表保留 (feature_id, omics_source, gene) 三元索引；
    ''' <see cref="Omics.OmicsNormalizer"/> 提供仅在组学内部生效的稳健标准化。
    ''' </description>
    ''' </item>
    ''' <item>
    ''' <term>步骤2 样本级分布估计</term>
    ''' <description>
    ''' 直接复用 <c>SMRUCC.genomics.Analysis.HTS.GSVA.GSVA.gsva</c> 的高斯核/泊松核 KDE
    ''' 或经验 CDF，逐组学层调用，因此分布估计天然只在组学内部进行。
    ''' </description>
    ''' </item>
    ''' <item>
    ''' <term>步骤3 组学特异性富集打分</term>
    ''' <description>
    ''' 见 <see cref="PerOmics.OmicsScorer"/>，产出每层的「通路 × 样本」富集分数 ES_k。
    ''' </description>
    ''' </item>
    ''' <item>
    ''' <term>步骤4 组学加权融合</term>
    ''' <description>
    ''' 见 <see cref="Fusion.WeightCalculators"/>（等权/方差/熵/互信息/特征数平衡）与
    ''' <see cref="Fusion.ConsistencyReward"/>（跨组学一致性奖励项），
    ''' 统一入口 <see cref="Fusion.MultiOmicsFusion"/>。
    ''' </description>
    ''' </item>
    ''' <item>
    ''' <term>步骤5 显著性评估</term>
    ''' <description>
    ''' 见 <see cref="Statistics.PermutationTest"/>（保持组学标签不变，仅打乱 feature→pathway 映射）
    ''' 与 <see cref="Statistics.MultipleTesting"/>（BH-FDR）。
    ''' </description>
    ''' </item>
    ''' </list>
    ''' <para>
    ''' 消融对照（把拼接矩阵当作单组学直接做一次 GSVA）见 <see cref="Ablation.PooledBaseline"/>；
    ''' 结果导出见 <see cref="Export.MultiOmicsResultWriter"/>。
    ''' </para>
    ''' </remarks>
    Friend Class NamespaceDoc
    End Class

End Namespace
