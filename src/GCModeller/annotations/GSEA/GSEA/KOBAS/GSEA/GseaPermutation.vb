Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace KOBAS

    ''' <summary>
    ''' 表型标签置换零分布（KOBAS <c>gsea.ES_null</c> / <c>ES_for_permutation</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' KOBAS 的置换方式是「保持基因集不变、打乱样本的表型标签后重新排序基因」，
    ''' 本模块与此一致。为了提高速度，这里做了两点工程化处理：
    ''' <list type="bullet">
    ''' <item>基因集成员关系先投影到「原始基因下标」上，每轮置换只需要对排序顺序做一次
    ''' 布尔查表，不必重新构建参数字典。</item>
    ''' <item>每一轮置换的随机数由 <c>Seed + permutation * 7919</c> 独立派生，
    ''' 因此可以并行执行且结果与串行完全一致（可复现）。</item>
    ''' </list>
    ''' </remarks>
    Public Module GseaPermutation

        ''' <summary>
        ''' 计算所有基因集的富集分数零分布。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <param name="pheno">表型分组。</param>
        ''' <param name="ranks">原始排序结果（用于建立基因集成员与原始基因下标的映射）。</param>
        ''' <param name="hitMatrix">基因集命中矩阵（列顺序与 <paramref name="ranks"/> 一致）。</param>
        ''' <param name="options">运行配置。</param>
        ''' <returns>
        ''' 长度为基因集数量的二维数组 <c>null[s][p]</c>：第 <c>s</c> 个基因集在
        ''' 第 <c>p</c> 次置换之下的富集分数。置换次数为 0 时返回 <c>Nothing</c>。
        ''' </returns>
        Public Function NullDistribution(expr As Matrix,
                                         pheno As phenotype,
                                         ranks As RankedGene(),
                                         hitMatrix As HitMatrix,
                                         options As GseaOptions) As Double()()

            If options.Permutations <= 0 Then
                Return Nothing
            End If

            Dim labels As Integer() = pheno.labels
            Dim values As Double()() = expr.expression.Select(Function(row) row.experiments).ToArray
            Dim geneCount As Integer = ranks.Length
            Dim setCount As Integer = hitMatrix.RowCount
            Dim sampleCount As Integer = labels.Length

            ' 基因集成员关系投影到原始基因下标：[set][geneIndex]
            Dim membership As Boolean()() = New Boolean(setCount - 1)() {}

            For s As Integer = 0 To setCount - 1
                Dim row As Boolean() = New Boolean(geneCount - 1) {}
                Dim mask As Boolean() = hitMatrix.Mask(s)

                For j As Integer = 0 To geneCount - 1
                    row(ranks(j).Index) = mask(j)
                Next

                membership(s) = row
            Next

            Dim nulls As Double()() = New Double(setCount - 1)() {}
            Dim permutations As Integer = options.Permutations
            Dim weight As Double = options.Weight
            Dim method As GeneRankingMethod = options.Method
            Dim seed As Integer = options.Seed

            For s As Integer = 0 To setCount - 1
                nulls(s) = New Double(permutations - 1) {}
            Next

            Dim progress As Integer = 0

            Call System.Threading.Tasks.Parallel.For(0, permutations,
                Sub(p)
                    Dim rng As New Random(seed + p * 7919)
                    Dim shuffle As Integer() = Enumerable.Range(0, sampleCount).ToArray
                    Dim permutedLabels As Integer() = New Integer(sampleCount - 1) {}

                    ' Fisher-Yates 洗牌
                    For i As Integer = sampleCount - 1 To 1 Step -1
                        Dim k As Integer = rng.Next(i + 1)
                        Dim temp As Integer = shuffle(i)

                        shuffle(i) = shuffle(k)
                        shuffle(k) = temp
                    Next

                    For i As Integer = 0 To sampleCount - 1
                        permutedLabels(i) = labels(shuffle(i))
                    Next

                    Dim scores As Double() = GeneRanking.Scores(values, permutedLabels, method)
                    Dim order As Integer() = Enumerable.Range(0, geneCount) _
                        .OrderByDescending(Function(i) scores(i)) _
                        .ToArray
                    Dim scoreInOrder As Double() = New Double(geneCount - 1) {}
                    Dim hitInOrder As Boolean() = New Boolean(geneCount - 1) {}

                    For j As Integer = 0 To geneCount - 1
                        scoreInOrder(j) = scores(order(j))
                    Next

                    For s As Integer = 0 To setCount - 1
                        Dim row As Boolean() = membership(s)

                        For j As Integer = 0 To geneCount - 1
                            hitInOrder(j) = row(order(j))
                        Next

                        nulls(s)(p) = EnrichmentScore.Compute(scoreInOrder, hitInOrder, weight, keepRunningSum:=False).ES
                    Next

                    If options.Verbose Then
                        Dim done As Integer = System.Threading.Interlocked.Increment(progress)

                        If done Mod 100 = 0 OrElse done = permutations Then
                            Call Console.WriteLine($"[KOBAS::GSEA] permutation {done}/{permutations}")
                        End If
                    End If
                End Sub)

            Return nulls
        End Function
    End Module
End Namespace
