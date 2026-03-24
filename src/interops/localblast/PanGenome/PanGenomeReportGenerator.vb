Imports System.IO
Imports System.Text

Public Class PanGenomeReportGenerator

    ''' <summary>
    ''' 生成泛基因组分析HTML报告
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <param name="templatePath">HTML模板文件路径</param>
    ''' <param name="outputPath">输出HTML文件路径</param>
    Public Sub GenerateReport(result As PanGenomeResult, templatePath As String, outputPath As String)
        ' 读取模板文件
        Dim templateContent As String = File.ReadAllText(templatePath, Encoding.UTF8)

        ' 创建StringBuilder进行替换
        Dim sb As New StringBuilder(templateContent)

        ' ============================================
        ' 1. 报告基本信息
        ' ============================================
        sb.Replace("{$REPORT_DATE}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        sb.Replace("{$TOTAL_GENOMES}", result.TotalGenesInGenomes.Count.ToString())
        sb.Replace("{$TOTAL_GENE_FAMILIES}", result.GeneFamilies.Count.ToString())

        ' 计算总基因数
        Dim totalGenes As Integer = result.TotalGenesInGenomes.Values.Sum()
        sb.Replace("{$TOTAL_GENES}", totalGenes.ToString())

        ' ============================================
        ' 2. 基因家族分类统计
        ' ============================================
        Dim coreCount As Integer = If(result.CoreGeneFamilies IsNot Nothing, result.CoreGeneFamilies.Length, 0)
        Dim softCoreCount As Integer = If(result.SoftCoreGeneFamilies IsNot Nothing, result.SoftCoreGeneFamilies.Length, 0)
        Dim shellCount As Integer = If(result.ShellGeneFamilies IsNot Nothing, result.ShellGeneFamilies.Length, 0)
        Dim cloudCount As Integer = If(result.CloudGeneFamilies IsNot Nothing, result.CloudGeneFamilies.Length, 0)
        Dim specificCount As Integer = If(result.SpecificGeneFamilies IsNot Nothing, result.SpecificGeneFamilies.Length, 0)
        Dim singleCopyCount As Integer = If(result.SingleCopyOrthologFamilies IsNot Nothing, result.SingleCopyOrthologFamilies.Length, 0)

        sb.Replace("{$CORE_GENES_COUNT}", coreCount.ToString())
        sb.Replace("{$SOFT_CORE_COUNT}", softCoreCount.ToString())
        sb.Replace("{$SHELL_GENES_COUNT}", shellCount.ToString())
        sb.Replace("{$CLOUD_GENES_COUNT}", cloudCount.ToString())
        sb.Replace("{$SPECIFIC_GENES_COUNT}", specificCount.ToString())
        sb.Replace("{$SINGLE_COPY_COUNT}", singleCopyCount.ToString())

        ' 计算百分比
        Dim totalFamilies As Integer = result.GeneFamilies.Count
        If totalFamilies > 0 Then
            sb.Replace("{$CORE_PERCENT}", (coreCount / totalFamilies * 100).ToString("F2"))
            sb.Replace("{$SOFT_CORE_PERCENT}", (softCoreCount / totalFamilies * 100).ToString("F2"))
            sb.Replace("{$SHELL_PERCENT}", (shellCount / totalFamilies * 100).ToString("F2"))
            sb.Replace("{$CLOUD_PERCENT}", (cloudCount / totalFamilies * 100).ToString("F2"))
        Else
            sb.Replace("{$CORE_PERCENT}", "0")
            sb.Replace("{$SOFT_CORE_PERCENT}", "0")
            sb.Replace("{$SHELL_PERCENT}", "0")
            sb.Replace("{$CLOUD_PERCENT}", "0")
        End If

        ' ============================================
        ' 3. 基因组统计表格
        ' ============================================
        sb.Replace("{$GENOME_STATS_TABLE}", GenerateGenomeStatsTable(result))

        ' ============================================
        ' 4. ECharts 数据 - 基因家族饼图
        ' ============================================
        sb.Replace("{$PIE_CHART_DATA}", GeneratePieChartData(coreCount, softCoreCount, shellCount, cloudCount))

        ' ============================================
        ' 5. ECharts 数据 - 泛基因组曲线
        ' ============================================
        sb.Replace("{$PANGENOME_CURVE_DATA}", GeneratePangenomeCurveData(result))

        ' ============================================
        ' 6. PAV 矩阵数据
        ' ============================================
        sb.Replace("{$PAV_MATRIX_DATA}", GeneratePAVMatrixData(result))

        ' ============================================
        ' 7. 遗传距离矩阵
        ' ============================================
        sb.Replace("{$GENETIC_DISTANCE_DATA}", GenerateGeneticDistanceData(result))

        ' ============================================
        ' 8. 结构变异统计
        ' ============================================
        sb.Replace("{$SV_STATS_DATA}", GenerateSVStatsData(result))

        ' ============================================
        ' 9. 共线性区块统计
        ' ============================================
        sb.Replace("{$COLLINEARITY_STATS}", GenerateCollinearityStats(result))

        ' ============================================
        ' 10. 详细数据表格
        ' ============================================
        sb.Replace("{$GENE_FAMILY_TABLE}", GenerateGeneFamilyTable(result))

        ' 写入输出文件
        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 生成基因组统计表格HTML
    ''' </summary>
    Private Function GenerateGenomeStatsTable(result As PanGenomeResult) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("<table class=""data-table"">")
        sb.AppendLine("<thead><tr><th>基因组名称</th><th>基因总数</th><th>特有基因数</th><th>核心基因占比</th></tr></thead>")
        sb.AppendLine("<tbody>")

        For Each kvp In result.TotalGenesInGenomes.OrderBy(Function(x) x.Key)
            Dim genomeName As String = kvp.Key
            Dim geneCount As Integer = kvp.Value

            ' 计算特有基因数
            Dim specificInGenome As Integer = 0
            If result.SpecificGeneFamilies IsNot Nothing Then
                For Each familyId In result.SpecificGeneFamilies
                    If result.PAVMatrix.ContainsKey(familyId) AndAlso result.PAVMatrix(familyId).ContainsKey(genomeName) Then
                        If result.PAVMatrix(familyId)(genomeName) > 0 Then
                            specificInGenome += 1
                        End If
                    End If
                Next
            End If

            ' 计算核心基因占比
            Dim coreCount As Integer = 0
            If result.CoreGeneFamilies IsNot Nothing Then
                For Each familyId In result.CoreGeneFamilies
                    If result.PAVMatrix.ContainsKey(familyId) AndAlso result.PAVMatrix(familyId).ContainsKey(genomeName) Then
                        If result.PAVMatrix(familyId)(genomeName) > 0 Then
                            coreCount += 1
                        End If
                    End If
                Next
            End If

            Dim coreRatio As Double = If(geneCount > 0, coreCount / geneCount * 100, 0)

            sb.AppendLine($"<tr><td class=""genome-name"">{genomeName}</td><td>{geneCount}</td><td>{specificInGenome}</td><td>{coreRatio:F2}%</td></tr>")
        Next

        sb.AppendLine("</tbody></table>")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成饼图数据(JSON格式)
    ''' </summary>
    Private Function GeneratePieChartData(core As Integer, softCore As Integer, shell As Integer, cloud As Integer) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("[")
        sb.AppendLine($"{{name: '核心基因', value: {core}, itemStyle: {{color: '#10b981'}}}},")
        sb.AppendLine($"{{name: '软核心基因', value: {softCore}, itemStyle: {{color: '#3b82f6'}}}},")
        sb.AppendLine($"{{name: '壳基因', value: {shell}, itemStyle: {{color: '#f59e0b'}}}},")
        sb.AppendLine($"{{name: '云基因', value: {cloud}, itemStyle: {{color: '#8b5cf6'}}}}")
        sb.AppendLine("]")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成泛基因组曲线数据(JSON格式)
    ''' </summary>
    Private Function GeneratePangenomeCurveData(result As PanGenomeResult) As String
        If result.PangenomeCurveData Is Nothing OrElse result.PangenomeCurveData.Length = 0 Then
            Return "{genomeCounts: [], panGenes: [], coreGenes: []}"
        End If

        Dim sb As New StringBuilder()
        sb.AppendLine("{")

        ' 基因组数量数组
        sb.Append("genomeCounts: [")
        sb.Append(String.Join(", ", result.PangenomeCurveData.Select(Function(d) d.GenomeCount)))
        sb.AppendLine("],")

        ' 泛基因组大小数组
        sb.Append("panGenes: [")
        sb.Append(String.Join(", ", result.PangenomeCurveData.Select(Function(d) d.TotalGenes)))
        sb.AppendLine("],")

        ' 核心基因组大小数组
        sb.Append("coreGenes: [")
        sb.Append(String.Join(", ", result.PangenomeCurveData.Select(Function(d) d.CoreGenes)))
        sb.AppendLine("]")

        sb.AppendLine("}")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成PAV矩阵数据(JSON格式)
    ''' </summary>
    Private Function GeneratePAVMatrixData(result As PanGenomeResult) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("{")

        ' 基因组列表
        Dim genomeNames As String() = result.TotalGenesInGenomes.Keys.OrderBy(Function(x) x).ToArray()
        sb.Append("genomes: [")
        sb.Append(String.Join(", ", genomeNames.Select(Function(g) $"'{g}'")))
        sb.AppendLine("],")

        ' 矩阵数据 (抽样展示，避免数据过大)
        sb.AppendLine("matrix: [")

        ' 随机抽样最多200个基因家族用于可视化
        Dim familiesToDisplay As List(Of String) = result.GeneFamilies.Keys.Take(200).ToList()
        Dim isFirst As Boolean = True

        For Each familyId In familiesToDisplay
            If Not result.PAVMatrix.ContainsKey(familyId) Then Continue For

            Dim pavRow = result.PAVMatrix(familyId)
            If Not isFirst Then sb.Append(",")
            sb.Append("[")
            sb.Append(String.Join(", ", genomeNames.Select(Function(g) If(pavRow.ContainsKey(g), pavRow(g), 0))))
            sb.Append("]")
            isFirst = False
        Next

        sb.AppendLine("],")

        ' 家族ID列表
        sb.Append("families: [")
        sb.Append(String.Join(", ", familiesToDisplay.Select(Function(f) $"'{f}'")))
        sb.AppendLine("]")

        sb.AppendLine("}")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成遗传距离矩阵数据(JSON格式)
    ''' </summary>
    Private Function GenerateGeneticDistanceData(result As PanGenomeResult) As String
        If result.GeneticDistanceMatrix Is Nothing OrElse result.GeneticDistanceMatrix.Count = 0 Then
            Return "{genomes: [], data: []}"
        End If

        Dim sb As New StringBuilder()
        sb.AppendLine("{")

        ' 提取基因组名称
        Dim genomeSet As New HashSet(Of String)()
        For Each kvp In result.GeneticDistanceMatrix
            Dim parts = kvp.Key.Split({"_vs_"}, StringSplitOptions.None)
            If parts.Length = 2 Then
                genomeSet.Add(parts(0))
                genomeSet.Add(parts(1))
            End If
        Next

        Dim genomeNames As String() = genomeSet.OrderBy(Function(x) x).ToArray()
        sb.Append("genomes: [")
        sb.Append(String.Join(", ", genomeNames.Select(Function(g) $"'{g}'")))
        sb.AppendLine("],")

        ' 构建矩阵数据
        sb.AppendLine("data: [")
        For i As Integer = 0 To genomeNames.Length - 1
            If i > 0 Then sb.Append(",")
            sb.Append("[")
            For j As Integer = 0 To genomeNames.Length - 1
                If j > 0 Then sb.Append(",")

                If i = j Then
                    sb.Append("0")
                Else
                    ' 构建查找键
                    Dim key As String = If(String.Compare(genomeNames(i), genomeNames(j)) < 0,
                                          $"{genomeNames(i)}_vs_{genomeNames(j)}",
                                          $"{genomeNames(j)}_vs_{genomeNames(i)}")

                    If result.GeneticDistanceMatrix.ContainsKey(key) Then
                        sb.Append(result.GeneticDistanceMatrix(key).ToString("F4"))
                    Else
                        sb.Append("0")
                    End If
                End If
            Next
            sb.Append("]")
        Next
        sb.AppendLine("]")

        sb.AppendLine("}")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成结构变异统计数据
    ''' </summary>
    Private Function GenerateSVStatsData(result As PanGenomeResult) As String
        If result.StructuralVariations Is Nothing OrElse result.StructuralVariations.Length = 0 Then
            Return "[]"
        End If

        ' 按类型分组统计
        Dim svStats = result.StructuralVariations.GroupBy(Function(sv) sv.Type).[Select](Function(g) New With {
            .Type = g.Key.ToString(),
            .Count = g.Count()
        }).ToList()

        Dim sb As New StringBuilder()
        sb.AppendLine("[")

        Dim typeColors As New Dictionary(Of String, String) From {
            {"PAV_Absence", "#ef4444"},
            {"PAV_Presence", "#22c55e"},
            {"CNV_Gain", "#3b82f6"},
            {"CNV_Loss", "#f97316"},
            {"Collinearity_Break", "#8b5cf6"}
        }

        Dim isFirst As Boolean = True
        For Each stat In svStats
            If Not isFirst Then sb.Append(",")
            Dim color As String = If(typeColors.ContainsKey(stat.Type), typeColors(stat.Type), "#6b7280")
            sb.AppendLine($"{{name: '{stat.Type}', value: {stat.Count}, itemStyle: {{color: '{color}'}}}}")
            isFirst = False
        Next

        sb.AppendLine("]")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成共线性统计信息
    ''' </summary>
    Private Function GenerateCollinearityStats(result As PanGenomeResult) As String
        If result.CollinearBlocks Is Nothing OrElse result.CollinearBlocks.Length = 0 Then
            Return "<p class='no-data'>未检测到共线性区块</p>"
        End If

        Dim sb As New StringBuilder()
        sb.AppendLine("<div class='stats-grid'>")
        sb.AppendLine($"<div class='stat-card'><span class='stat-value'>{result.CollinearBlocks.Length}</span><span class='stat-label'>共线性区块总数</span></div>")

        ' 统计基因对数量
        Dim totalPairs As Integer = result.CollinearBlocks.Sum(Function(b) If(b.OrthologyLinks IsNot Nothing, b.OrthologyLinks.Length, 0))
        sb.AppendLine($"<div class='stat-card'><span class='stat-value'>{totalPairs}</span><span class='stat-label'>同源基因对总数</span></div>")

        ' 统计涉及的基因组对
        Dim genomePairs = result.CollinearBlocks.Select(Function(b) $"{b.Genome1} vs {b.Genome2}").Distinct().Count()
        sb.AppendLine($"<div class='stat-card'><span class='stat-value'>{genomePairs}</span><span class='stat-label'>比较基因组对数</span></div>")

        sb.AppendLine("</div>")

        ' 列出主要共线性区块
        sb.AppendLine("<table class='data-table'>")
        sb.AppendLine("<thead><tr><th>基因组对</th><th>染色体</th><th>基因对数</th></tr></thead>")
        sb.AppendLine("<tbody>")

        For Each block In result.CollinearBlocks.Take(20)
            Dim pairCount As Integer = If(block.OrthologyLinks IsNot Nothing, block.OrthologyLinks.Length, 0)
            sb.AppendLine($"<tr><td>{block.Genome1} ↔ {block.Genome2}</td><td>{block.Chr1} ↔ {block.Chr2}</td><td>{pairCount}</td></tr>")
        Next

        sb.AppendLine("</tbody></table>")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成基因家族详细表格
    ''' </summary>
    Private Function GenerateGeneFamilyTable(result As PanGenomeResult) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("<table class='data-table sortable'>")
        sb.AppendLine("<thead><tr><th>家族ID</th><th>分类</th><th>基因数</th><th>存在基因组数</th><th>代表性基因</th></tr></thead>")
        sb.AppendLine("<tbody>")

        ' 构建分类字典
        Dim familyCategory As New Dictionary(Of String, String)()

        If result.CoreGeneFamilies IsNot Nothing Then
            For Each id In result.CoreGeneFamilies
                familyCategory(id) = "核心基因"
            Next
        End If

        If result.SoftCoreGeneFamilies IsNot Nothing Then
            For Each id In result.SoftCoreGeneFamilies
                familyCategory(id) = "软核心"
            Next
        End If

        If result.ShellGeneFamilies IsNot Nothing Then
            For Each id In result.ShellGeneFamilies
                familyCategory(id) = "壳基因"
            Next
        End If

        If result.CloudGeneFamilies IsNot Nothing Then
            For Each id In result.CloudGeneFamilies
                familyCategory(id) = "云基因"
            Next
        End If

        ' 生成表格行 (限制显示数量)
        Dim displayCount As Integer = 0
        For Each kvp In result.GeneFamilies
            If displayCount >= 100 Then Exit For

            Dim familyId As String = kvp.Key
            Dim genes As String() = kvp.Value
            Dim category As String = If(familyCategory.ContainsKey(familyId), familyCategory(familyId), "未知")

            ' 计算存在基因组数
            Dim presenceCount As Integer = 0
            If result.PAVMatrix.ContainsKey(familyId) Then
                presenceCount = result.PAVMatrix(familyId).Values.Where(Function(v) v > 0).Count
            End If

            ' 获取代表性基因
            Dim repGene As String = If(genes.Length > 0, genes(0), "-")
            If genes.Length > 1 Then
                repGene &= $" (+{genes.Length - 1} more)"
            End If

            sb.AppendLine($"<tr><td class='family-id'>{familyId}</td><td><span class='badge {GetCategoryClass(category)}'>{category}</span></td><td>{genes.Length}</td><td>{presenceCount}/{result.TotalGenesInGenomes.Count}</td><td class='gene-list'>{repGene}</td></tr>")
            displayCount += 1
        Next

        sb.AppendLine("</tbody></table>")

        If result.GeneFamilies.Count > 100 Then
            sb.AppendLine($"<p class='table-note'>显示前100条记录，共{result.GeneFamilies.Count}个基因家族</p>")
        End If

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 获取分类对应的CSS类名
    ''' </summary>
    Private Function GetCategoryClass(category As String) As String
        Select Case category
            Case "核心基因" : Return "badge-core"
            Case "软核心" : Return "badge-softcore"
            Case "壳基因" : Return "badge-shell"
            Case "云基因" : Return "badge-cloud"
            Case Else : Return "badge-default"
        End Select
    End Function

End Class