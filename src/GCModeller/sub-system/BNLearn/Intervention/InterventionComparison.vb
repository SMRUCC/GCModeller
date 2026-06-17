' ============================================================
' InterventionComparison.vb - 虚拟扰动结果比较分析导出模块
' ============================================================
' 基于 InterventionResult 数组，生成多种比较分析矩阵并导出为 CSV
'
' 支持的导出矩阵类型：
'   1. FoldChange 矩阵 —— 行=基因，列=扰动条件，值=FoldChange
'   2. PercentChange 矩阵 —— 行=基因，列=扰动条件，值=变化百分比
'   3. Significance 矩阵 —— 行=基因，列=扰动条件，值=1/0（显著/不显著）
'   4. ZScore 矩阵 —— 行=基因，列=扰动条件，值=Z-score
'   5. 综合比较矩阵 —— 包含所有指标的宽表格式
'   6. 扰动相似性矩阵 —— 扰动条件之间的 Pearson 相关系数
'   7. 基因敏感性矩阵 —— 基因对各种扰动的响应谱
'   8. 扰动影响排名表 —— 每个扰动条件下受影响最大的基因排名
'   9. 通路级别汇总矩阵 —— 按通路聚合的扰动效应
'  10. 交叉影响矩阵 —— 哪些扰动影响了哪些通路的基因
' ============================================================

Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.MetabolicModel

Namespace Intervention

    ''' <summary>
    ''' 虚拟扰动结果比较分析导出器
    ''' </summary>
    Public Class InterventionComparisonExporter

        ' ==================== 核心数据提取 ====================
        ReadOnly results As InterventionResult()

        Sub New(results As IEnumerable(Of InterventionResult))
            Me.results = results.ToArray
        End Sub

        ''' <summary>
        ''' 从 InterventionResult 数组中提取所有唯一的基因名（排序后）
        ''' </summary>
        Private Function CollectAllGeneNames() As String()
            Dim geneSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each r In results
                If r.GeneNames IsNot Nothing Then
                    For Each g In r.GeneNames
                        geneSet.Add(g)
                    Next
                End If
            Next
            Dim arr As String() = geneSet.ToArray()
            Array.Sort(arr, StringComparer.OrdinalIgnoreCase)
            Return arr
        End Function

        ''' <summary>
        ''' 从 InterventionResult 数组中提取所有扰动条件标签
        ''' 格式：GeneName_Mode（如 "TP53_Knockout"、"MYC_Overexpression"）
        ''' </summary>
        Private Function CollectConditionLabels() As String()
            Dim labels As New List(Of String)()
            For Each r In results
                If r.Spec IsNot Nothing Then
                    labels.Add(String.Format("{0}_{1}", r.Spec.GeneName, r.Spec.Mode.ToString()))
                End If
            Next
            Return labels.ToArray()
        End Function

        ''' <summary>
        ''' 构建基因名 → 索引 的映射
        ''' </summary>
        Private Function BuildGeneIndexMap(geneNames As String()) As Dictionary(Of String, Integer)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To geneNames.Length - 1
                map(geneNames(i)) = i
            Next
            Return map
        End Function

        ''' <summary>
        ''' 从单个 InterventionResult 中按基因名提取指定指标值
        ''' 返回与 allGeneNames 等长的数组，缺失基因填 NaN
        ''' </summary>
        Private Function ExtractMetric(result As InterventionResult,
                                        allGeneNames As String(),
                                        geneMap As Dictionary(Of String, Integer),
                                        metric As ComparisonMetric) As Double()
            Dim values As Double() = Enumerable.Repeat(Double.NaN, allGeneNames.Length).ToArray()

            If result.GeneNames Is Nothing Then Return values

            For i = 0 To result.GeneNames.Length - 1
                Dim gName As String = result.GeneNames(i)
                Dim idx As Integer = -1
                If geneMap.TryGetValue(gName, idx) Then
                    Select Case metric
                        Case ComparisonMetric.FoldChange
                            values(idx) = result.FoldChanges(i)
                        Case ComparisonMetric.PercentChange
                            values(idx) = result.PercentChanges(i)
                        Case ComparisonMetric.Significance
                            values(idx) = If(result.IsSignificant(i), 1.0, 0.0)
                        Case ComparisonMetric.ZScore
                            ' 优先使用已计算的 ZScores；若为 Nothing 则从 FoldChange 近似
                            If result.ZScores IsNot Nothing AndAlso i < result.ZScores.Length Then
                                values(idx) = result.ZScores(i)
                            Else
                                ' 近似：Z ≈ FoldChange（假设野生型SD≈1，适用于标准化数据）
                                values(idx) = result.FoldChanges(i)
                            End If
                        Case ComparisonMetric.WildtypeMean
                            values(idx) = result.WildtypeMeans(i)
                        Case ComparisonMetric.MutantMean
                            values(idx) = result.MutantMeans(i)
                        Case ComparisonMetric.AbsoluteFoldChange
                            values(idx) = Math.Abs(result.FoldChanges(i))
                    End Select
                End If
            Next

            Return values
        End Function

        ' ==================== 矩阵构建 ====================

        ''' <summary>
        ''' 构建比较矩阵 [nGenes × nConditions]
        ''' </summary>
        Private Function BuildComparisonMatrix(metric As ComparisonMetric) As ComparisonMatrix
            Dim allGenes As String() = CollectAllGeneNames()
            Dim conditions As String() = CollectConditionLabels()
            Dim geneMap As Dictionary(Of String, Integer) = BuildGeneIndexMap(allGenes)

            Dim nG As Integer = allGenes.Length
            Dim nC As Integer = conditions.Length
            Dim matrix As Double(,) = New Double(nG - 1, nC - 1) {}

            For c = 0 To results.Length - 1
                Dim colValues As Double() = ExtractMetric(results(c), allGenes, geneMap, metric)
                For g = 0 To nG - 1
                    matrix(g, c) = colValues(g)
                Next
            Next

            Return New ComparisonMatrix() With {
                .GeneNames = allGenes,
                .ConditionLabels = conditions,
                .Metric = metric,
                .Matrix = matrix
            }
        End Function

        ' ==================== 公开导出方法 ====================

        ''' <summary>
        ''' 导出 FoldChange 比较矩阵（CSV）
        ''' 行 = 基因，列 = 扰动条件，值 = log2 FoldChange
        ''' </summary>
        Public Sub ExportFoldChangeMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.FoldChange)
            WriteComparisonMatrixCSV(cm, filePath,
                "FoldChange comparison matrix: rows=genes, columns=intervention conditions")
        End Sub

        ''' <summary>
        ''' 导出 PercentChange 比较矩阵（CSV）
        ''' 行 = 基因，列 = 扰动条件，值 = 表达变化百分比
        ''' </summary>
        Public Sub ExportPercentChangeMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.PercentChange)
            WriteComparisonMatrixCSV(cm, filePath,
                "PercentChange comparison matrix: rows=genes, columns=intervention conditions")
        End Sub

        ''' <summary>
        ''' 导出显著性矩阵（CSV）
        ''' 行 = 基因，列 = 扰动条件，值 = 1(显著) / 0(不显著)
        ''' </summary>
        Public Sub ExportSignificanceMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.Significance)
            WriteComparisonMatrixCSV(cm, filePath,
                "Significance matrix: 1=significant, 0=not significant", format:="F0")
        End Sub

        ''' <summary>
        ''' 导出 Z-Score 比较矩阵（CSV）
        ''' 行 = 基因，列 = 扰动条件，值 = Z-score
        ''' </summary>
        Public Sub ExportZScoreMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.ZScore)
            WriteComparisonMatrixCSV(cm, filePath,
                "Z-Score comparison matrix: rows=genes, columns=intervention conditions")
        End Sub

        ''' <summary>
        ''' 导出野生型均值矩阵（CSV）
        ''' 行 = 基因，列 = 扰动条件，值 = 野生型表达均值
        ''' </summary>
        Public Sub ExportWildtypeMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.WildtypeMean)
            WriteComparisonMatrixCSV(cm, filePath,
                "Wildtype expression means: rows=genes, columns=intervention conditions")
        End Sub

        ''' <summary>
        ''' 导出突变型均值矩阵（CSV）
        ''' 行 = 基因，列 = 扰动条件，值 = 扰动后表达均值
        ''' </summary>
        Public Sub ExportMutantMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.MutantMean)
            WriteComparisonMatrixCSV(cm, filePath,
                "Mutant expression means: rows=genes, columns=intervention conditions")
        End Sub

        ' ==================== 综合比较矩阵 ====================

        ''' <summary>
        ''' 导出综合比较矩阵（宽表格式 CSV）
        ''' 每行 = 一个基因在一个扰动条件下的完整指标
        ''' 列：Gene, Condition, InterventionGene, InterventionMode,
        '''      WildtypeMean, MutantMean, FoldChange, PercentChange, ZScore, Significant
        ''' </summary>
        Public Sub ExportComprehensiveTable(filePath As String)
            Dim sb As New StringBuilder()

            ' 表头注释
            sb.AppendLine("# Comprehensive intervention comparison table")
            sb.AppendLine(String.Format("# Generated from {0} intervention results", results.Length))

            ' 列标题
            sb.AppendLine(String.Join(",",
                "Gene", "Condition", "InterventionGene", "InterventionMode",
                "WildtypeMean", "MutantMean", "FoldChange", "PercentChange",
                "ZScore", "Significant"))

            ' 数据行
            For Each r In results
                If r.GeneNames Is Nothing Then Continue For
                Dim condLabel As String = String.Format("{0}_{1}", r.Spec.GeneName, r.Spec.Mode.ToString())
                Dim intGene As String = r.Spec.GeneName
                Dim intMode As String = r.Spec.Mode.ToString()

                For i = 0 To r.GeneNames.Length - 1
                    ' 跳过被干预基因自身（其 FoldChange 无意义）
                    If String.Equals(r.GeneNames(i), intGene, StringComparison.OrdinalIgnoreCase) Then Continue For

                    ' 计算 ZScore（优先使用已有值，否则近似）
                    Dim zVal As Double = r.FoldChanges(i)
                    If r.ZScores IsNot Nothing AndAlso i < r.ZScores.Length Then
                        zVal = r.ZScores(i)
                    End If

                    sb.AppendLine(String.Join(",",
                        EscapeCsv(r.GeneNames(i)),
                        EscapeCsv(condLabel),
                        EscapeCsv(intGene),
                        EscapeCsv(intMode),
                        FormatVal(r.WildtypeMeans(i)),
                        FormatVal(r.MutantMeans(i)),
                        FormatVal(r.FoldChanges(i)),
                        FormatVal(r.PercentChanges(i)),
                        FormatVal(zVal),
                        If(r.IsSignificant(i), "1", "0")))
                Next
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 扰动相似性矩阵 ====================

        ''' <summary>
        ''' 导出扰动条件之间的相似性矩阵（CSV）
        ''' 基于 FoldChange 向量的 Pearson 相关系数
        ''' 行/列 = 扰动条件，值 = 相关系数
        ''' 用于聚类分析：哪些基因的敲除产生相似的下游效应
        ''' </summary>
        Public Sub ExportConditionSimilarityMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.FoldChange)
            Dim nC As Integer = cm.ConditionLabels.Length
            Dim nG As Integer = cm.GeneNames.Length

            ' 计算条件之间的 Pearson 相关系数
            Dim simMatrix As Double(,) = New Double(nC - 1, nC - 1) {}

            For i = 0 To nC - 1
                simMatrix(i, i) = 1.0
                For j = i + 1 To nC - 1
                    ' 提取两个条件的 FoldChange 向量（排除 NaN）
                    Dim vecI As New List(Of Double)()
                    Dim vecJ As New List(Of Double)()
                    For g = 0 To nG - 1
                        If Not Double.IsNaN(cm.Matrix(g, i)) AndAlso Not Double.IsNaN(cm.Matrix(g, j)) Then
                            vecI.Add(cm.Matrix(g, i))
                            vecJ.Add(cm.Matrix(g, j))
                        End If
                    Next

                    Dim corr As Double = PearsonCorrelation(vecI.ToArray(), vecJ.ToArray())
                    simMatrix(i, j) = corr
                    simMatrix(j, i) = corr
                Next
            Next

            ' 写入 CSV
            Dim sb As New StringBuilder()
            sb.AppendLine("# Condition similarity matrix (Pearson correlation of FoldChange profiles)")
            sb.AppendLine("# Rows and columns = intervention conditions")

            ' 表头
            sb.Append("Condition")
            For j = 0 To nC - 1
                sb.Append("," & EscapeCsv(cm.ConditionLabels(j)))
            Next
            sb.AppendLine()

            ' 数据
            For i = 0 To nC - 1
                sb.Append(EscapeCsv(cm.ConditionLabels(i)))
                For j = 0 To nC - 1
                    sb.Append("," & FormatVal(simMatrix(i, j)))
                Next
                sb.AppendLine()
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 基因敏感性矩阵 ====================

        ''' <summary>
        ''' 导出基因敏感性谱矩阵（CSV）
        ''' 行 = 基因，列 = 扰动条件，值 = |FoldChange|
        ''' 用于识别"脆弱基因"——对多种扰动都敏感的基因
        ''' </summary>
        Public Sub ExportGeneSensitivityMatrix(filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.AbsoluteFoldChange)

            ' 附加统计列
            Dim nG As Integer = cm.GeneNames.Length
            Dim nC As Integer = cm.ConditionLabels.Length

            Dim sb As New StringBuilder()
            sb.AppendLine("# Gene sensitivity profile: |FoldChange| under each intervention")
            sb.AppendLine("# Additional columns: SensitivityScore, AffectedCount, MaxEffect, MeanEffect")

            ' 表头
            sb.Append("Gene")
            For j = 0 To nC - 1
                sb.Append("," & EscapeCsv(cm.ConditionLabels(j)))
            Next
            sb.Append(",SensitivityScore,AffectedCount,MaxEffect,MeanEffect")
            sb.AppendLine()

            ' 数据
            For g = 0 To nG - 1
                sb.Append(EscapeCsv(cm.GeneNames(g)))

                Dim affectedCount As Integer = 0
                Dim maxEffect As Double = 0
                Dim sumEffect As Double = 0
                Dim validCount As Integer = 0

                For c = 0 To nC - 1
                    Dim val As Double = cm.Matrix(g, c)
                    sb.Append("," & FormatVal(val))

                    If Not Double.IsNaN(val) Then
                        validCount += 1
                        sumEffect += val
                        If val > maxEffect Then maxEffect = val
                        If val > 1.0 Then affectedCount += 1  ' |FC| > 1 视为受影响
                    End If
                Next

                ' 敏感性评分 = 受影响次数 / 总条件数
                Dim sensitivityScore As Double = If(nC > 0, CDbl(affectedCount) / nC, 0)
                Dim meanEffect As Double = If(validCount > 0, sumEffect / validCount, 0)

                sb.Append(String.Format(",{0:F4},{1},{2:F4},{3:F4}",
                    sensitivityScore, affectedCount, maxEffect, meanEffect))
                sb.AppendLine()
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 扰动影响排名表 ====================

        ''' <summary>
        ''' 导出每个扰动条件下受影响最大的基因排名表（CSV）
        ''' 按 |FoldChange| 降序排列
        ''' </summary>
        Public Sub ExportInterventionRanking(filePath As String, Optional topN As Integer = 50)
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Format("# Top {0} affected genes per intervention condition", topN))
            sb.AppendLine("# Ranked by |FoldChange| descending")

            sb.AppendLine(String.Join(",",
                "Condition", "InterventionGene", "InterventionMode", "Rank",
                "Gene", "FoldChange", "PercentChange", "ZScore", "Significant"))

            For Each r In results
                If r.GeneNames Is Nothing Then Continue For
                Dim condLabel As String = String.Format("{0}_{1}", r.Spec.GeneName, r.Spec.Mode.ToString())
                Dim intGene As String = r.Spec.GeneName

                ' 构建排名列表（排除被干预基因自身）
                Dim ranked As New List(Of (Gene As String, FC As Double, PC As Double, ZS As Double, Sig As Boolean))()
                For i = 0 To r.GeneNames.Length - 1
                    If String.Equals(r.GeneNames(i), intGene, StringComparison.OrdinalIgnoreCase) Then Continue For
                    Dim zVal As Double = r.FoldChanges(i)
                    If r.ZScores IsNot Nothing AndAlso i < r.ZScores.Length Then zVal = r.ZScores(i)
                    ranked.Add((r.GeneNames(i), r.FoldChanges(i), r.PercentChanges(i), zVal, r.IsSignificant(i)))
                Next

                ' 按 |FoldChange| 降序排列
                ranked.Sort(Function(a, b) Math.Abs(b.FC).CompareTo(Math.Abs(a.FC)))

                ' 输出 TopN
                Dim nOut As Integer = Math.Min(topN, ranked.Count)
                For rank = 0 To nOut - 1
                    Dim item = ranked(rank)
                    sb.AppendLine(String.Join(",",
                        EscapeCsv(condLabel),
                        EscapeCsv(intGene),
                        EscapeCsv(r.Spec.Mode.ToString()),
                        (rank + 1).ToString(),
                        EscapeCsv(item.Gene),
                        FormatVal(item.FC),
                        FormatVal(item.PC),
                        FormatVal(item.ZS),
                        If(item.Sig, "1", "0")))
                Next
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 通路级别汇总矩阵 ====================

        ''' <summary>
        ''' 导出通路级别扰动效应汇总矩阵（CSV）
        ''' 将基因级别的 FoldChange 按通路聚合（取均值/中位数/显著基因数）
        ''' 行 = 通路，列 = 扰动条件
        ''' </summary>
        Public Sub ExportPathwaySummaryMatrix(pathwayInfo As Dictionary(Of String, MetabolicPathway), filePath As String)
            Dim cm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.FoldChange)
            Dim sigCm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.Significance)
            Dim geneMap As Dictionary(Of String, Integer) = BuildGeneIndexMap(cm.GeneNames)

            Dim nC As Integer = cm.ConditionLabels.Length
            Dim pathwayIDs As String() = pathwayInfo.Keys.ToArray()
            Array.Sort(pathwayIDs)

            Dim sb As New StringBuilder()
            sb.AppendLine("# Pathway-level intervention effect summary")
            sb.AppendLine("# Values: mean|FC| of genes in pathway (significant_gene_count)")

            ' 表头
            sb.Append("PathwayID,PathwayName,N_Genes")
            For c = 0 To nC - 1
                sb.Append("," & EscapeCsv(cm.ConditionLabels(c)))
            Next
            sb.AppendLine()

            ' 每个通路
            For Each pid As String In pathwayIDs
                Dim pInfo As MetabolicPathway = pathwayInfo(pid)
                Dim pathwayGeneIndices As New List(Of Integer)()

                ' 找到属于该通路的基因在矩阵中的行索引
                For Each gName In pInfo.genes
                    Dim idx As Integer = -1
                    If geneMap.TryGetValue(gName, idx) Then
                        pathwayGeneIndices.Add(idx)
                    End If
                Next

                sb.Append(String.Format("{0},{1},{2}",
                    EscapeCsv(pid), EscapeCsv(pInfo.name), pathwayGeneIndices.Count))

                ' 对每个扰动条件计算通路级别的汇总
                For c = 0 To nC - 1
                    Dim absFCs As New List(Of Double)()
                    Dim sigCount As Integer = 0

                    For Each gIdx In pathwayGeneIndices
                        Dim fc As Double = cm.Matrix(gIdx, c)
                        If Not Double.IsNaN(fc) Then
                            absFCs.Add(Math.Abs(fc))
                        End If
                        Dim sig As Double = sigCm.Matrix(gIdx, c)
                        If sig > 0.5 Then sigCount += 1
                    Next

                    Dim meanAbsFC As Double = If(absFCs.Count > 0, absFCs.Average(), 0)
                    sb.Append(String.Format(",{0:F4}({1})", meanAbsFC, sigCount))
                Next

                sb.AppendLine()
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 交叉影响矩阵 ====================

        ''' <summary>
        ''' 导出扰动-通路交叉影响矩阵（CSV）
        ''' 行 = 扰动条件，列 = 通路
        ''' 值 = 该扰动对该通路中显著受影响基因的比例
        ''' 用于热图可视化
        ''' </summary>
        Public Sub ExportCrossImpactMatrix(pathwayInfo As Dictionary(Of String, MetabolicPathway), filePath As String)
            Dim sigCm As ComparisonMatrix = BuildComparisonMatrix(ComparisonMetric.Significance)
            Dim geneMap As Dictionary(Of String, Integer) = BuildGeneIndexMap(sigCm.GeneNames)

            Dim nC As Integer = sigCm.ConditionLabels.Length
            Dim pathwayIDs As String() = pathwayInfo.Keys.ToArray()
            Array.Sort(pathwayIDs)
            Dim nP As Integer = pathwayIDs.Length

            ' 构建交叉矩阵 [nConditions × nPathways]
            Dim crossMatrix As Double(,) = New Double(nC - 1, nP - 1) {}

            For c = 0 To nC - 1
                For p = 0 To nP - 1
                    Dim pGenes As String() = pathwayInfo(pathwayIDs(p)).genes
                    Dim sigCount As Integer = 0
                    Dim totalCount As Integer = 0

                    For Each gName In pGenes
                        Dim idx As Integer = -1
                        If geneMap.TryGetValue(gName, idx) Then
                            totalCount += 1
                            If sigCm.Matrix(idx, c) > 0.5 Then sigCount += 1
                        End If
                    Next

                    crossMatrix(c, p) = If(totalCount > 0, CDbl(sigCount) / totalCount, 0)
                Next
            Next

            ' 写入 CSV
            Dim sb As New StringBuilder()
            sb.AppendLine("# Cross-impact matrix: fraction of significantly affected genes per pathway")
            sb.AppendLine("# Rows = intervention conditions, Columns = pathways")

            sb.Append("Condition_InterventionGene_Mode")
            For Each pid As String In pathwayIDs
                sb.Append("," & EscapeCsv(pid))
            Next
            sb.AppendLine()

            For c = 0 To nC - 1
                sb.Append(EscapeCsv(sigCm.ConditionLabels(c)))
                For p = 0 To nP - 1
                    sb.Append("," & FormatVal(crossMatrix(c, p)))
                Next
                sb.AppendLine()
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 一键全量导出 ====================

        ''' <summary>
        ''' 一键导出所有比较分析矩阵到指定目录
        ''' </summary>
        Public Sub ExportAll(outputDir As String,
                              Optional pathwayInfo As Dictionary(Of String, MetabolicPathway) = Nothing,
                              Optional topN As Integer = 50)

            If Not Directory.Exists(outputDir) Then
                Directory.CreateDirectory(outputDir)
            End If

            ' 1. FoldChange 矩阵
            ExportFoldChangeMatrix(Path.Combine(outputDir, "foldchange_matrix.csv"))

            ' 2. PercentChange 矩阵
            ExportPercentChangeMatrix(Path.Combine(outputDir, "percentchange_matrix.csv"))

            ' 3. 显著性矩阵
            ExportSignificanceMatrix(Path.Combine(outputDir, "significance_matrix.csv"))

            ' 4. Z-Score 矩阵
            ExportZScoreMatrix(Path.Combine(outputDir, "zscore_matrix.csv"))

            ' 5. 野生型均值矩阵
            ExportWildtypeMatrix(Path.Combine(outputDir, "wildtype_means_matrix.csv"))

            ' 6. 突变型均值矩阵
            ExportMutantMatrix(Path.Combine(outputDir, "mutant_means_matrix.csv"))

            ' 7. 综合比较宽表
            ExportComprehensiveTable(Path.Combine(outputDir, "comprehensive_comparison.csv"))

            ' 8. 扰动相似性矩阵
            ExportConditionSimilarityMatrix(Path.Combine(outputDir, "condition_similarity.csv"))

            ' 9. 基因敏感性谱
            ExportGeneSensitivityMatrix(Path.Combine(outputDir, "gene_sensitivity.csv"))

            ' 10. 扰动影响排名
            ExportInterventionRanking(Path.Combine(outputDir, "intervention_ranking.csv"), topN)

            ' 11-12. 通路级别分析（如果提供了通路信息）
            If pathwayInfo IsNot Nothing AndAlso pathwayInfo.Count > 0 Then
                ExportPathwaySummaryMatrix(pathwayInfo, Path.Combine(outputDir, "pathway_summary.csv"))
                ExportCrossImpactMatrix(pathwayInfo, Path.Combine(outputDir, "cross_impact_matrix.csv"))
            End If
        End Sub

        ' ==================== 辅助类型 ====================

        ''' <summary>
        ''' 比较指标枚举
        ''' </summary>
        Public Enum ComparisonMetric
            FoldChange
            PercentChange
            Significance
            ZScore
            WildtypeMean
            MutantMean
            AbsoluteFoldChange
        End Enum

        ''' <summary>
        ''' 比较矩阵数据结构
        ''' </summary>
        Public Class ComparisonMatrix
            ''' <summary>基因名列表（行标题）</summary>
            Public Property GeneNames As String()
            ''' <summary>扰动条件标签列表（列标题）</summary>
            Public Property ConditionLabels As String()
            ''' <summary>指标类型</summary>
            Public Property Metric As ComparisonMetric
            ''' <summary>矩阵数据 [gene, condition]</summary>
            Public Property Matrix As Double(,)
        End Class

        ' ==================== 内部工具方法 ====================

        ''' <summary>
        ''' 将比较矩阵写入 CSV 文件
        ''' </summary>
        Private Sub WriteComparisonMatrixCSV(cm As ComparisonMatrix, filePath As String,
                                              Optional comment As String = "",
                                              Optional format As String = "F4")
            Dim sb As New StringBuilder()

            ' 注释行
            If Not String.IsNullOrEmpty(comment) Then
                sb.AppendLine("# " & comment)
            End If
            sb.AppendLine(String.Format("# Metric: {0}", cm.Metric.ToString()))
            sb.AppendLine(String.Format("# Genes: {0}, Conditions: {1}", cm.GeneNames.Length, cm.ConditionLabels.Length))

            ' 表头
            sb.Append("Gene")
            For j = 0 To cm.ConditionLabels.Length - 1
                sb.Append("," & EscapeCsv(cm.ConditionLabels(j)))
            Next
            sb.AppendLine()

            ' 数据行
            For i = 0 To cm.GeneNames.Length - 1
                sb.Append(EscapeCsv(cm.GeneNames(i)))
                For j = 0 To cm.ConditionLabels.Length - 1
                    Dim val As Double = cm.Matrix(i, j)
                    If Double.IsNaN(val) Then
                        sb.Append(",NA")
                    Else
                        sb.Append("," & val.ToString(format, System.Globalization.CultureInfo.InvariantCulture))
                    End If
                Next
                sb.AppendLine()
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ''' <summary>
        ''' CSV 字段转义（处理逗号、引号、换行）
        ''' </summary>
        Private Function EscapeCsv(field As String) As String
            If String.IsNullOrEmpty(field) Then Return ""
            If field.Contains(",") OrElse field.Contains("""") OrElse field.Contains(vbCr) OrElse field.Contains(vbLf) Then
                Return """" & field.Replace("""", """""") & """"
            End If
            Return field
        End Function

        ''' <summary>
        ''' 格式化数值（NaN → "NA"）
        ''' </summary>
        Private Function FormatVal(val As Double, Optional fmt As String = "F4") As String
            If Double.IsNaN(val) OrElse Double.IsInfinity(val) Then Return "NA"
            Return val.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture)
        End Function

        ''' <summary>
        ''' 计算 Pearson 相关系数
        ''' </summary>
        Private Function PearsonCorrelation(x As Double(), y As Double()) As Double
            If x Is Nothing OrElse y Is Nothing OrElse x.Length <> y.Length OrElse x.Length < 3 Then
                Return Double.NaN
            End If

            Dim n As Integer = x.Length
            Dim sumX As Double = 0, sumY As Double = 0
            For i = 0 To n - 1
                sumX += x(i)
                sumY += y(i)
            Next
            Dim meanX As Double = sumX / n
            Dim meanY As Double = sumY / n

            Dim covXY As Double = 0, varX As Double = 0, varY As Double = 0
            For i = 0 To n - 1
                Dim dx As Double = x(i) - meanX
                Dim dy As Double = y(i) - meanY
                covXY += dx * dy
                varX += dx * dx
                varY += dy * dy
            Next

            If varX < 1.0E-30 OrElse varY < 1.0E-30 Then Return Double.NaN
            Return covXY / Math.Sqrt(varX * varY)
        End Function

    End Class

End Namespace
