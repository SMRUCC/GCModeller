Imports System.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports std = System.Math

Namespace StatTests

    ''' <summary>
    ''' 置换检验结果。
    ''' </summary>
    Public Class PermutationResult

        ''' <summary>逐通路的经验 p 值</summary>
        Public Property PValues As Dictionary(Of String, Double)

        ''' <summary>逐通路的观测统计量（max_j |ES_multi|）</summary>
        Public Property ObservedStatistics As Dictionary(Of String, Double)

        ''' <summary>逐通路的零分布</summary>
        Public Property NullStatistics As Dictionary(Of String, Double())

        ''' <summary>实际执行的置换次数</summary>
        Public Property Iterations As Integer

        ''' <summary>参与检验的通路</summary>
        Public Property Pathways As String()

        ''' <summary>使用的置换打分方法</summary>
        Public Property ScoreMethod As Methods = Methods.ssgsea

        ''' <summary>未能在零分布中出现的通路（例如置换后成员被完全过滤）</summary>
        Public Property MissingPathways As String()

        Public Overrides Function ToString() As String
            Return $"permutation test: {Iterations} iteration(s), {If(Pathways, New String() {}).Length} pathway(s), method={ScoreMethod}"
        End Function

    End Class

    ''' <summary>
    ''' 样本级置换检验（method.md 步骤5）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 置换策略：<b>保持组学来源标签不变，只打乱 feature → pathway 的映射</b>。
    ''' 具体实现是对每个组学层独立地置换特征标签（基因符号），
    ''' 矩阵数值与样本顺序完全不动，因此样本间的真实结构被保留，
    ''' 而被破坏的只有"哪个基因属于哪条通路"这一关系。
    ''' </para>
    ''' <para>
    ''' 检验统计量取每条通路在所有样本上的最大绝对融合分数
    ''' <c>T = max_j |ES_multi(j)|</c>；经验 p 值用
    ''' <c>(1 + #{T_null >= T_obs}) / (1 + B)</c> 计算。
    ''' </para>
    ''' </remarks>
    Public Module PermutationTest

        ''' <summary>
        ''' 执行置换检验。
        ''' </summary>
        ''' <param name="dataset">多组学数据集</param>
        ''' <param name="geneSets">基因集</param>
        ''' <param name="options">运行配置（置换次数、种子、规模裁剪等）</param>
        ''' <param name="observed">观测到的融合分数矩阵（通路 × 样本），只用于确定参与检验的通路集合</param>
        Public Function Run(dataset As Omics.MultiOmicsDataset,
                            geneSets As Background,
                            options As MultiOmicsGsvaOptions,
                            observed As Matrix) As PermutationResult

            If dataset Is Nothing Then
                Throw New ArgumentNullException(NameOf(dataset))
            End If
            If geneSets Is Nothing Then
                Throw New ArgumentNullException(NameOf(geneSets))
            End If
            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If
            If observed Is Nothing Then
                Throw New ArgumentNullException(NameOf(observed))
            End If

            Dim iterations As Integer = std.Max(0, options.Permutations)

            If iterations = 0 Then
                Return Nothing
            End If

            Dim targets As String() = SelectPathways(observed, options)

            If targets.Length = 0 Then
                Return Nothing
            End If

            ' 只保留目标通路，减少每次置换的打分负担
            Dim subsetSets As Background = SubsetGeneSets(geneSets, targets)

            If subsetSets.clusters.Length = 0 Then
                Return Nothing
            End If

            ' 需要与融合结果对齐的通路集合（subsetSets 里可能缺少某些目标通路）
            Dim evaluated As String() = subsetSets.clusters.Select(Function(c) c.ID).ToArray
            Dim view As Omics.MultiOmicsDataset = BuildGeneView(dataset, subsetSets, options)

            Dim permOptions As MultiOmicsGsvaOptions = options.Clone()

            permOptions.ScoreMethod = options.PermutationMethod
            permOptions.MinSize = 1
            permOptions.MaxSize = Integer.MaxValue
            permOptions.Verbose = False

            ' 观测统计量必须用与置换<b>完全相同</b>的打分方法重新计算：
            ' 主流程可能用 GSVA（核密度），而置换用 ssGSEA（基于秩），
            ' 两者的 ES 尺度不可比，直接拿主流程分数当观测值会让 p 值失去意义。
            Dim observedFusion As Fusion.MultiOmicsFusionResult = Fusion.MultiOmicsFusion.Compute(
                PerOmics.OmicsScorer.Score(view, subsetSets, permOptions),
                permOptions)
            Dim observedStats As New Dictionary(Of String, Double)(StringComparer.Ordinal)
            Dim nulls As New Dictionary(Of String, List(Of Double))(StringComparer.Ordinal)

            For Each pathway As String In evaluated
                Dim v As Double() = observedFusion.ScoreVector(pathway)

                If v Is Nothing Then
                    Continue For
                End If

                observedStats(pathway) = MaxAbs(v)
                nulls(pathway) = New List(Of Double)
            Next

            For b As Integer = 0 To iterations - 1
                Dim rng As New Random(options.Seed + b * 7919)
                Dim permuted As Omics.MultiOmicsDataset = PermuteDataset(view, rng)

                Try
                    Dim scores As PerOmics.OmicsScoreSet = PerOmics.OmicsScorer.Score(permuted, subsetSets, permOptions)
                    Dim permutedFusion As Fusion.MultiOmicsFusionResult = Fusion.MultiOmicsFusion.Compute(scores, permOptions)

                    For Each pathway As String In evaluated
                        Dim buffer As List(Of Double) = Nothing

                        If Not nulls.TryGetValue(pathway, buffer) Then
                            Continue For
                        End If

                        Dim v As Double() = permutedFusion.ScoreVector(pathway)

                        If v IsNot Nothing Then
                            buffer.Add(MaxAbs(v))
                        End If
                    Next
                Catch ex As Exception
                    ' 某一次置换失败不应该中断整个检验，只是减少该次的自由度
                    If options.Verbose Then
                        Call $"[moGSVA] permutation #{b + 1} failed: {ex.Message}".warning
                    End If
                End Try

                If options.Verbose AndAlso (b + 1) Mod 10 = 0 Then
                    Call $"[moGSVA] permutation {b + 1}/{iterations} done.".info
                End If
            Next

            Dim pValues As New Dictionary(Of String, Double)(StringComparer.Ordinal)
            Dim nullStats As New Dictionary(Of String, Double())(StringComparer.Ordinal)
            Dim missing As New List(Of String)

            For Each pathway As String In evaluated
                Dim buffer As List(Of Double) = Nothing

                If Not nulls.TryGetValue(pathway, buffer) Then
                    Continue For
                End If

                If buffer.Count = 0 Then
                    missing.Add(pathway)
                    Continue For
                End If

                Dim statistic As Double = observedStats(pathway)
                Dim hits As Integer = buffer.Where(Function(x) x >= statistic).Count()

                pValues(pathway) = (1.0 + hits) / (1.0 + buffer.Count)
                nullStats(pathway) = buffer.ToArray
            Next

            Return New PermutationResult With {
                .PValues = pValues,
                .ObservedStatistics = observedStats,
                .NullStatistics = nullStats,
                .Iterations = iterations,
                .Pathways = pValues.Keys.ToArray(),
                .ScoreMethod = options.PermutationMethod,
                .MissingPathways = missing.ToArray()
            }
        End Function

        Private Function SelectPathways(observed As Matrix, options As MultiOmicsGsvaOptions) As String()
            Dim available As New HashSet(Of String)(observed.rownames, StringComparer.Ordinal)

            If options.PermutationPathways IsNot Nothing AndAlso options.PermutationPathways.Length > 0 Then
                Return options.PermutationPathways _
                    .Where(Function(id) available.Contains(id)) _
                    .Distinct(StringComparer.Ordinal) _
                    .ToArray
            End If

            Dim limit As Integer = options.MaxPermutationPathways

            If limit <= 0 OrElse limit > observed.size Then
                limit = observed.size
            End If

            Return observed.rownames.Take(limit).ToArray
        End Function

        Private Function SubsetGeneSets(geneSets As Background, pathways As String()) As Background
            Dim keep As New HashSet(Of String)(pathways, StringComparer.Ordinal)

            Return New Background With {
                .id = geneSets.id,
                .name = geneSets.name,
                .comments = geneSets.comments,
                .build = geneSets.build,
                .clusters = geneSets.clusters _
                    .Where(Function(c) c IsNot Nothing AndAlso keep.Contains(c.ID)) _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' 缩小置换视图的规模：优先保留目标通路的成员，再用固定种子补充背景特征。
        ''' </summary>
        Private Function BuildGeneView(dataset As Omics.MultiOmicsDataset,
                                       geneSets As Background,
                                       options As MultiOmicsGsvaOptions) As Omics.MultiOmicsDataset

            Dim limit As Integer = options.PermutationGeneLimit

            If limit <= 0 Then
                Return dataset
            End If

            Dim members As New HashSet(Of String)(
                geneSets.clusters _
                    .SelectMany(Function(c) If(c.members, New BackgroundGene() {}).Select(Function(g) g.accessionID)) _
                    .Where(Function(id) Not String.IsNullOrEmpty(id)),
                StringComparer.Ordinal)
            Dim keep As New List(Of String)(members)
            Dim rest As String() = dataset.Layers _
                .SelectMany(Function(l) l.Genes) _
                .Distinct(StringComparer.Ordinal) _
                .Where(Function(g) Not members.Contains(g)) _
                .ToArray

            If keep.Count < limit AndAlso rest.Length > 0 Then
                Dim rng As New Random(options.Seed)
                Dim pool As String() = rest.OrderBy(Function(g) rng.Next()).ToArray

                keep.AddRange(pool.Take(limit - keep.Count))
            End If

            If options.Verbose Then
                Call $"[moGSVA] permutation view keeps {keep.Count} feature(s) from {dataset.FeatureCount}.".info
            End If

            Return dataset.SubsetGenes(keep)
        End Function

        ''' <summary>
        ''' 对每一层独立置换特征标签（数据不动，只打乱 feature → pathway 的对应关系）。
        ''' </summary>
        Private Function PermuteDataset(dataset As Omics.MultiOmicsDataset, rng As Random) As Omics.MultiOmicsDataset
            Dim layers As Omics.OmicsLayer() = dataset.Layers _
                .Select(Function(l) PermuteLayer(l, rng)) _
                .ToArray

            Return New Omics.MultiOmicsDataset With {
                .Layers = layers,
                .SampleID = dataset.SampleID,
                .DroppedSamples = dataset.DroppedSamples
            }
        End Function

        Private Function PermuteLayer(layer As Omics.OmicsLayer, rng As Random) As Omics.OmicsLayer
            Dim labels As String() = DirectCast(layer.Genes.Clone(), String())

            For i As Integer = labels.Length - 1 To 1 Step -1
                Dim j As Integer = rng.Next(i + 1)
                Dim tmp As String = labels(i)

                labels(i) = labels(j)
                labels(j) = tmp
            Next

            Dim rows As DataFrameRow() = layer.Expr.expression _
                .Select(Function(r, i) New DataFrameRow With {
                    .geneID = labels(i),
                    .experiments = r.experiments
                }) _
                .ToArray

            Return New Omics.OmicsLayer With {
                .Name = layer.Name,
                .Expr = New Matrix With {
                    .sampleID = layer.Expr.sampleID,
                    .tag = $"{layer.Expr.tag}:permuted",
                    .expression = rows
                },
                .Normalization = layer.Normalization
            }
        End Function

        Private Function MaxAbs(v As Double()) As Double
            Dim best As Double = 0

            For Each x As Double In v
                Dim a As Double = std.Abs(x)

                If Double.IsNaN(a) Then
                    Continue For
                End If
                If a > best Then
                    best = a
                End If
            Next

            Return best
        End Function

    End Module

End Namespace
