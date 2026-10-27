Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace KOBAS

    ''' <summary>
    ''' KOBAS 风格 GSEA 分析的门面。
    ''' </summary>
    ''' <remarks>
    ''' 本模块是 KOBAS 3.0 <c>scripts/run_gsea.py</c> 主流程的 VB.NET 移植：
    ''' <code>
    ''' rank_pro  -> ES_all -> ES_null -> nominal_p -> normalized -> fdr_cal
    ''' </code>
    ''' 与 KOBAS 原实现的差异（详见各子模块的 <c>remarks</c>）：
    ''' 未命中步长按标准 GSEA 定义修正、排序在并列值上稳定、卡方/Fisher 使用标准列联表、
    ''' 基因集尺寸边界统一为闭区间。
    ''' </remarks>
    Public Module KobasGsea

        ''' <summary>
        ''' 对给定的表达矩阵与表型分组执行 GSEA 分析。
        ''' </summary>
        ''' <param name="expr">行是基因、列是样本的表达矩阵。</param>
        ''' <param name="pheno">表型分组信息。</param>
        ''' <param name="background">基因集背景模型（通常由 <see cref="GmtFile"/> 构建）。</param>
        ''' <param name="options">运行配置；为 <c>Nothing</c> 时使用默认配置。</param>
        ''' <returns>按富集分数降序排列的分析结果。</returns>
        ''' <exception cref="InvalidOperationException">
        ''' 表达矩阵与表型不匹配、没有任何基因被注释或所有基因集都被过滤时抛出。
        ''' </exception>
        Public Function Compute(expr As Matrix,
                                pheno As phenotype,
                                background As Background,
                                Optional options As GseaOptions = Nothing) As GseaResult()

            If options Is Nothing Then
                options = New GseaOptions
            End If

            Call ExpressionTable.Validate(expr, pheno)

            Dim ranks As RankedGene() = GeneRanking.GeneRank(expr, pheno, options.Method)
            Dim genes As String() = ranks.Select(Function(gene) gene.GeneID).ToArray
            Dim hitMatrix As HitMatrix = genes.BuildHitMatrix(background, options.MinSize, options.MaxSize, options.Verbose)
            Dim scores As EnrichmentScoreResult() = EnrichmentScore.ComputeAll(ranks, hitMatrix, options.Weight, options.KeepRunningSum)
            Dim es As Double() = scores.Select(Function(score) score.ES).ToArray
            Dim nes As Double() = DirectCast(es.Clone(), Double())
            Dim pvalue As Double() = Enumerable.Repeat(Double.NaN, es.Length).ToArray
            Dim fdr As Double() = Enumerable.Repeat(Double.NaN, es.Length).ToArray

            If options.Permutations > 0 Then
                If options.Verbose Then
                    Call Console.WriteLine($"[KOBAS::GSEA] running {options.Permutations} phenotype permutation(s)...")
                End If

                Dim nulls As Double()() = GseaPermutation.NullDistribution(expr, pheno, ranks, hitMatrix, options)

                pvalue = Significance.NominalP(es, nulls)

                Dim normalized = Significance.Normalize(es, nulls)

                nes = normalized.NES
                fdr = Significance.Fdr(nes, normalized.Null)
            End If

            Dim result As New List(Of GseaResult)

            For i As Integer = 0 To hitMatrix.RowCount - 1
                Dim hits As GeneSetHits = hitMatrix.GeneSets(i)
                Dim mask As Boolean() = hitMatrix.Mask(i)
                Dim score As EnrichmentScoreResult = scores(i)

                result.Add(New GseaResult With {
                    .ID = hits.ID,
                    .Names = hits.Names,
                    .Description = hits.Description,
                    .Size = hits.HitCount,
                    .ES = score.ES,
                    .NES = nes(i),
                    .PValue = pvalue(i),
                    .FDR = fdr(i),
                    .ESIndex = score.ESIndex,
                    .RES = score.RES,
                    .CoreGenes = EnrichmentScore.CoreGenes(ranks, mask, score.ES, score.ESIndex),
                    .HitGenes = hitMatrix.HitGenes(i)
                })
            Next

            If options.Verbose Then
                Call Console.WriteLine($"[KOBAS::GSEA] {result.Count} gene set(s) evaluated.")
            End If

            Return result _
                .OrderByDescending(Function(item) item.ES) _
                .ToArray
        End Function
    End Module
End Namespace
