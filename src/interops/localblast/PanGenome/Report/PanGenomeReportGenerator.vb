#Region "Microsoft.VisualBasic::414ddfb24b4df5cebe0a5ac9b400a679, localblast\PanGenome\PanGenomeReportGenerator.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.PanGenome.My.Resources
Imports SMRUCC.genomics.Analysis.PanGenome.ReportJSON

''' <summary>
''' 泛基因组分析HTML报告的生成器。
''' </summary>
''' <remarks>
''' 页面绘图所需的数据全部先构造成具名的CLR数据对象，然后再通过
''' <see cref="JsonContract.GetJson(Of T)(T, Boolean, Boolean, IEnumerable(Of Type))"/> 序列化为JSON文本，
''' 并内嵌到HTML模板的 <c>&lt;script type="application/json"&gt;</c> 标签之中。
''' 
''' 这样子当基因组名称或者其他文本内容包含单引号、反斜杠、尖括号等特殊字符的时候，
''' 也不会再破坏页面的JavaScript代码。
''' </remarks>
Public Module PanGenomeReportGenerator

    ''' <summary>
    ''' 共线性矩阵热图最多展示的基因组数量
    ''' </summary>
    Const MaxCollinearityGenomes As Integer = 60
    ''' <summary>
    ''' 共线性条形图最多展示的基因组对数量
    ''' </summary>
    Const MaxCollinearityPairs As Integer = 20

    ''' <summary>
    ''' 共线性统计指标：共线基因对数量
    ''' </summary>
    Const CollinearMetric_Pairs As String = "共线基因对"
    ''' <summary>
    ''' 共线性统计指标：共线性区块数量
    ''' </summary>
    Const CollinearMetric_Blocks As String = "共线性区块数"
    ''' <summary>
    ''' 遗传距离热图最多展示的基因组数量
    ''' </summary>
    Const MaxDistanceGenomes As Integer = 200
    ''' <summary>
    ''' PAV热图最多展示的基因家族数量
    ''' </summary>
    Const MaxPAVFamilies As Integer = 200
    ''' <summary>
    ''' PAV热图最多展示的基因组数量
    ''' </summary>
    Const MaxPAVGenomes As Integer = 120
    ''' <summary>
    ''' PCA分析所使用的基因家族数量上限（按照家族的基因总数降序取Top-N）
    ''' </summary>
    Const MaxPCAFamilies As Integer = 5000
    ''' <summary>
    ''' PCA降维的目标维度
    ''' </summary>
    Const PCA_Dimensions As Integer = 3
    ''' <summary>
    ''' PCA散点图的着色维度标题
    ''' </summary>
    Const PCA_ColorLabel As String = "核心基因占比 (%)"

    Public ReadOnly Property DefaultHtmlTemplate As String
        Get
            Return DefaultTemplate.ResourceManager.GetString("Report")
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function DefaultHtmlReport(result As PanGenomeResult) As String
        Return GenerateReport(result, templateContent:=DefaultHtmlTemplate)
    End Function

    ''' <summary>
    ''' 生成泛基因组分析HTML报告
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' 
    <Extension>
    Public Function GenerateReport(result As PanGenomeResult, templateContent As String) As String
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
        ' 3. ECharts 数据 - 基因组基本信息统计（由前端渲染表格与直方图）
        ' ============================================
        ' 基因组统计只需要计算一次：既用于前端的统计表格/直方图，也用于PCA散点图的着色维度
        Dim genomeStats As GenomeStatRow() = BuildGenomeStats(result)

        sb.Replace("{$GENOME_STATS_DATA}", SerializeData(genomeStats))

        ' ============================================
        ' 3.1 基因组三维散点图（基因存在/缺失均衡度熵 / 特有基因占比 / 核心基因占比）
        ' ============================================
        sb.Replace("{$GENOME_ENTROPY_DATA}", SerializeData(BuildGenomeEntropyData(result, genomeStats)))

        ' ============================================
        ' 4. ECharts 数据 - 基因家族饼图
        ' ============================================
        sb.Replace("{$PIE_CHART_DATA}", SerializeData(BuildPieChartData(coreCount, softCoreCount, shellCount, cloudCount)))

        ' ============================================
        ' 5. ECharts 数据 - 泛基因组曲线
        ' ============================================
        sb.Replace("{$PANGENOME_CURVE_DATA}", SerializeData(BuildPangenomeCurve(result)))

        ' ============================================
        ' 6. PAV 矩阵数据
        ' ============================================
        sb.Replace("{$PAV_MATRIX_DATA}", SerializeData(BuildPAVMatrixData(result)))

        ' ============================================
        ' 6.1 PAV 矩阵的 PCA 分析（三维散点图）
        ' ============================================
        sb.Replace("{$PCA_DATA}", SerializeData(BuildPCAData(result, genomeStats)))

        ' ============================================
        ' 7. 遗传距离矩阵
        ' ============================================
        sb.Replace("{$GENETIC_DISTANCE_DATA}", SerializeData(BuildGeneticDistanceData(result)))

        ' ============================================
        ' 8. 结构变异统计
        ' ============================================
        sb.Replace("{$SV_STATS_DATA}", SerializeData(BuildSVStatsData(result)))

        ' ============================================
        ' 9. 共线性区块统计与可视化数据
        ' ============================================
        sb.Replace("{$COLLINEARITY_STATS}", GenerateCollinearityStats(result))
        sb.Replace("{$COLLINEARITY_DATA}", SerializeData(BuildCollinearityData(result)))

        ' ============================================
        ' 10. 详细数据表格
        ' ============================================
        sb.Replace("{$GENE_FAMILY_TABLE}", GenerateGeneFamilyTable(result))

        Return sb.ToString()
    End Function

#Region "JSON serialization helpers"

    ''' <summary>
    ''' 将报告数据对象序列化为可以安全内嵌到HTML页面之中的JSON文本
    ''' </summary>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function SerializeData(Of T)(data As T) As String
        Return EscapeJsonForHtml(data.GetJson(indent:=False))
    End Function

    ''' <summary>
    ''' 转义JSON文本之中的HTML敏感字符，避免 <c>&lt;/script&gt;</c> 或者 <c>&lt;!--</c>
    ''' 这样的内容破坏内嵌脚本标签的解析。
    ''' </summary>
    ''' <remarks>
    ''' 这些字符在合法的JSON文本之中只会出现在字符串字面量内部，
    ''' 因此使用 <c>\uXXXX</c> 进行转义在任何位置都是合法的，
    ''' 并且 <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/> 默认不会输出这些转义。
    ''' </remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function EscapeJsonForHtml(json As String) As String
        If String.IsNullOrEmpty(json) Then
            Return json
        End If

        Return json.Replace("&", "\u0026").Replace("<", "\u003c").Replace(">", "\u003e")
    End Function

    ''' <summary>
    ''' HTML转义：避免基因组名称等文本内容破坏服务端渲染的表格结构
    ''' </summary>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function H(text As String) As String
        If String.IsNullOrEmpty(text) Then
            Return ""
        End If

        Return WebUtility.HtmlEncode(text)
    End Function

    ''' <summary>
    ''' 当数据量超过上限的时候，按照等间距的方式抽样，保证展示结果是确定性的
    ''' </summary>
    Private Function SampleEvenly(names As String(), maxCount As Integer) As String()
        If names.Length <= maxCount Then
            Return names
        End If

        Dim sampled(maxCount - 1) As String
        Dim stepSize As Double = names.Length / CDbl(maxCount)

        For i As Integer = 0 To maxCount - 1
            sampled(i) = names(CInt(Math.Floor(i * stepSize)))
        Next

        Return sampled
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function GetOrZero(counts As Dictionary(Of String, Integer), key As String) As Integer
        Dim value As Integer = 0

        Call counts.TryGetValue(key, value)

        Return value
    End Function

#End Region

#Region "data builders"

    ''' <summary>
    ''' 生成饼图数据
    ''' </summary>
    Private Function BuildPieChartData(core As Integer, softCore As Integer, shell As Integer, cloud As Integer) As CategoryItem()
        Return {
            New CategoryItem With {.name = "核心基因", .value = core, .color = "#10b981"},
            New CategoryItem With {.name = "软核心基因", .value = softCore, .color = "#3b82f6"},
            New CategoryItem With {.name = "壳基因", .value = shell, .color = "#f59e0b"},
            New CategoryItem With {.name = "云基因", .value = cloud, .color = "#8b5cf6"}
        }
    End Function

    ''' <summary>
    ''' 生成泛基因组曲线数据
    ''' </summary>
    Private Function BuildPangenomeCurve(result As PanGenomeResult) As PangenomeCurveDataset
        If result.PangenomeCurveData Is Nothing OrElse result.PangenomeCurveData.Length = 0 Then
            Return New PangenomeCurveDataset With {
                .genomeCounts = New Integer() {},
                .panGenes = New Integer() {},
                .coreGenes = New Integer() {}
            }
        End If

        Return New PangenomeCurveDataset With {
            .genomeCounts = result.PangenomeCurveData.Select(Function(d) d.GenomeCount).ToArray,
            .panGenes = result.PangenomeCurveData.Select(Function(d) d.TotalGenes).ToArray,
            .coreGenes = result.PangenomeCurveData.Select(Function(d) d.CoreGenes).ToArray
        }
    End Function

    ''' <summary>
    ''' 生成PAV矩阵数据
    ''' </summary>
    ''' <remarks>
    ''' 只保留在PAV矩阵之中真实存在的基因家族，保证 <c>families</c> 与 <c>matrix</c> 的行是一一对应的；
    ''' 当基因组/家族数量过多的时候按照等间距方式抽样，避免生成的JSON过于庞大。
    ''' </remarks>
    Private Function BuildPAVMatrixData(result As PanGenomeResult) As PAVMatrixDataset
        Dim allGenomes As String() = result.TotalGenesInGenomes.Keys.OrderBy(Function(x) x).ToArray()
        Dim genomes As String() = SampleEvenly(allGenomes, MaxPAVGenomes)

        Dim families As String() = result.GeneFamilies.Keys _
            .Where(Function(familyId) result.PAVMatrix.ContainsKey(familyId)) _
            .Take(MaxPAVFamilies) _
            .ToArray()

        Dim matrix As Integer()() = families _
            .Select(Function(familyId)
                        Dim row As Dictionary(Of String, Integer) = result.PAVMatrix(familyId)

                        Return genomes _
                            .Select(Function(genome) If(row.ContainsKey(genome), row(genome), 0)) _
                            .ToArray
                    End Function) _
            .ToArray

        Return New PAVMatrixDataset With {
            .genomes = genomes,
            .families = families,
            .matrix = matrix,
            .truncated = allGenomes.Length > genomes.Length OrElse result.GeneFamilies.Count > families.Length
        }
    End Function

    ''' <summary>
    ''' PAV矩阵的PCA分析：取基因总数最多的前<see cref="MaxPCAFamilies"/>个基因家族构建PAV子矩阵，
    ''' 以基因组为样本、基因家族为特征，降维到<see cref="PCA_Dimensions"/>个维度
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <param name="stats">
    ''' 基因组基本信息统计，提供核心基因占比作为散点图的着色维度
    ''' </param>
    ''' <remarks>
    ''' 任何数据不足或者计算失败的情况都会返回一个空的数据集，由前端显示"数据不可用"，
    ''' 保证PCA分析的失败不会导致整个报告的生成过程失败。
    ''' </remarks>
    Private Function BuildPCAData(result As PanGenomeResult, stats As GenomeStatRow()) As PCAScatterDataset
        Dim empty As New PCAScatterDataset With {
            .points = New PCAPoint() {},
            .pc1Label = "PC1",
            .pc2Label = "PC2",
            .pc3Label = "PC3",
            .colorLabel = PCA_ColorLabel,
            .familyCount = 0,
            .explained = New Double() {}
        }
        Dim genomeNames As String() = result.TotalGenesInGenomes.Keys.OrderBy(Function(x) x).ToArray()

        ' PCA.vb 在样本数不足的时候会自动下调maxPC，这里提前做一次保护
        If genomeNames.Length < PCA_Dimensions Then
            Return empty
        End If

        ' 按照家族的基因总数降序取Top-N；同分的时候以家族ID次序稳定化，保证多次生成的结果是可复现的
        Dim families As String() = result.GeneFamilies _
            .Where(Function(kv) result.PAVMatrix.ContainsKey(kv.Key)) _
            .OrderByDescending(Function(kv) kv.Value.Length) _
            .ThenBy(Function(kv) kv.Key, StringComparer.Ordinal) _
            .Take(MaxPCAFamilies) _
            .Select(Function(kv) kv.Key) _
            .ToArray()

        If families.Length = 0 Then
            Return empty
        End If

        Try
            ' 注意：PrincipalComponentAnalysis 会原地修改输入的数值数组，
            ' 因此这里必须新建一个DataFrame，不可以复用 result.GetPAVMatrix() 的结果，
            ' 否则会污染后续的PAV热图与PAV表格数据
            Dim df As New DataFrame With {.rownames = genomeNames}

            For Each familyId As String In families
                Dim pavRow As Dictionary(Of String, Integer) = result.PAVMatrix(familyId)

                Call df.add(familyId,
                            genomeNames _
                                .Select(Function(genome) CDbl(If(pavRow.ContainsKey(genome), pavRow(genome), 0))) _
                                .ToArray)
            Next

            Dim stat As StatisticsObject = df.CommonDataSet()
            Dim pcaResult As MultivariateAnalysisResult = PCA.PrincipalComponentAnalysis(stat, maxPC:=PCA_Dimensions)
            Dim score As DataFrame = pcaResult.GetPCAScore()
            Dim components As String() = score.featureNames

            If components Is Nothing OrElse components.Length < PCA_Dimensions Then
                Call Debug.WriteLine($"[pangenome] PCA got only {If(components Is Nothing, 0, components.Length)} components, skip PCA chart.")

                Return empty
            End If

            ' 得分向量的顺序与输入DataFrame的行顺序(即genomeNames)一致
            Dim pc1 As Double() = GetPCVector(score, components(0), genomeNames.Length)
            Dim pc2 As Double() = GetPCVector(score, components(1), genomeNames.Length)
            Dim pc3 As Double() = GetPCVector(score, components(2), genomeNames.Length)

            If pc1 Is Nothing OrElse pc2 Is Nothing OrElse pc3 Is Nothing Then
                Return empty
            End If

            Dim coreRatios As Dictionary(Of String, Double) = stats _
                .GroupBy(Function(r) r.name) _
                .ToDictionary(Function(g) g.Key, Function(g) g.First.coreRatio)
            Dim points As PCAPoint() = genomeNames _
                .Select(Function(genome, i)
                            Return New PCAPoint With {
                                .name = genome,
                                .pc1 = pc1(i),
                                .pc2 = pc2(i),
                                .pc3 = pc3(i),
                                .coreRatio = GetOrZeroDouble(coreRatios, genome),
                                .geneCount = result.TotalGenesInGenomes(genome)
                            }
                        End Function) _
                .ToArray

            Dim explained As Double() = pcaResult.Contributions.Take(PCA_Dimensions).ToArray

            Call $"[pangenome] PCA done: {points.Length} genomes x {families.Length} families, variance = {explained.Select(Function(x) x.ToString("F2")).JoinBy(", ")}%".debug

            Return New PCAScatterDataset With {
                .points = points,
                .pc1Label = ComponentLabel(components(0), explained, 0),
                .pc2Label = ComponentLabel(components(1), explained, 1),
                .pc3Label = ComponentLabel(components(2), explained, 2),
                .colorLabel = PCA_ColorLabel,
                .familyCount = families.Length,
                .explained = explained
            }
        Catch ex As Exception
            Call Debug.WriteLine($"[pangenome] PCA analysis failed: {ex.Message}")

            Return empty
        End Try
    End Function

    ''' <summary>
    ''' 取出某一个主成分的得分向量；长度与基因组数量不一致的时候返回Nothing
    ''' </summary>
    Private Function GetPCVector(score As DataFrame, component As String, expectedSize As Integer) As Double()
        Dim feature As FeatureVector = score(component)

        If feature Is Nothing OrElse feature.vector Is Nothing OrElse feature.vector.Length <> expectedSize Then
            Return Nothing
        End If

        Dim values As New List(Of Double)(expectedSize)

        For Each item As Object In feature.vector
            Call values.Add(CDbl(item))
        Next

        Return values.ToArray
    End Function

    ''' <summary>
    ''' 生成坐标轴标题，例如 <c>PC1 (42.51%)</c>
    ''' </summary>
    Private Function ComponentLabel(component As String, explained As Double(), index As Integer) As String
        If explained IsNot Nothing AndAlso index < explained.Length Then
            Return $"{component} ({explained(index):F2}%)"
        Else
            Return component
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function GetOrZeroDouble(table As Dictionary(Of String, Double), key As String) As Double
        Dim value As Double = 0

        Call table.TryGetValue(key, value)

        Return value
    End Function

    ''' <summary>
    ''' 生成遗传距离矩阵数据
    ''' </summary>
    Private Function BuildGeneticDistanceData(result As PanGenomeResult) As GeneticDistanceDataset
        If result.GeneticDistanceMatrix Is Nothing OrElse result.GeneticDistanceMatrix.Count = 0 Then
            Return New GeneticDistanceDataset With {
                .genomes = New String() {},
                .data = New Double()() {},
                .truncated = False
            }
        End If

        ' 提取基因组名称
        Dim genomeSet As New HashSet(Of String)()

        For Each pair As KeyValuePair(Of String, Double) In result.GeneticDistanceMatrix
            Dim parts As String() = pair.Key.Split({"_vs_"}, StringSplitOptions.None)

            If parts.Length = 2 Then
                genomeSet.Add(parts(0))
                genomeSet.Add(parts(1))
            End If
        Next

        Dim allGenomes As String() = genomeSet.OrderBy(Function(x) x).ToArray()
        Dim genomes As String() = SampleEvenly(allGenomes, MaxDistanceGenomes)

        Dim matrix As Double()() = genomes _
            .Select(Function(g1)
                        Return genomes _
                            .Select(Function(g2)
                                        If g1 = g2 Then
                                            Return 0.0
                                        End If

                                        ' 距离矩阵是对称的，只需要按照字典序构造一次查找键
                                        Dim key As String = If(String.Compare(g1, g2, StringComparison.Ordinal) < 0,
                                                               $"{g1}_vs_{g2}",
                                                               $"{g2}_vs_{g1}")
                                        Dim distance As Double = 0

                                        If result.GeneticDistanceMatrix.TryGetValue(key, distance) Then
                                            Return distance
                                        Else
                                            Return 0.0
                                        End If
                                    End Function) _
                            .ToArray
                    End Function) _
            .ToArray

        Return New GeneticDistanceDataset With {
            .genomes = genomes,
            .data = matrix,
            .truncated = allGenomes.Length > genomes.Length
        }
    End Function

    ''' <summary>
    ''' 生成结构变异统计数据
    ''' </summary>
    Private Function BuildSVStatsData(result As PanGenomeResult) As CategoryItem()
        If result.StructuralVariations Is Nothing OrElse result.StructuralVariations.Length = 0 Then
            Return New CategoryItem() {}
        End If

        ' 按类型分组统计
        Return result.StructuralVariations _
            .GroupBy(Function(sv) sv.Type) _
            .Select(Function(g)
                        Dim typeName As String = g.Key.ToString()

                        Return New CategoryItem With {
                            .name = typeName,
                            .value = g.Count(),
                            .color = GetSVTypeColor(typeName)
                        }
                    End Function) _
            .ToArray
    End Function

    Private Function GetSVTypeColor(typeName As String) As String
        Select Case typeName
            Case "PAV_Absence" : Return "#ef4444"
            Case "PAV_Presence" : Return "#22c55e"
            Case "CNV_Gain" : Return "#3b82f6"
            Case "CNV_Loss" : Return "#f97316"
            Case "Collinearity_Break" : Return "#8b5cf6"
            Case Else : Return "#6b7280"
        End Select
    End Function

    ''' <summary>
    ''' 构建共线性结果的可视化数据（基因组×基因组共线性矩阵 + Top 基因组对排行）
    ''' </summary>
    ''' <remarks>
    ''' 这里只依赖 <see cref="CollinearBlock.GenePairCount"/> 区块统计信息，
    ''' 因此在基因组数量超过阈值、逐基因的同源配对数据被关闭
    ''' （<see cref="GenomeAnalyzer.RetainOrthologyLinks"/> = False）的时候依然可以正常展示。
    ''' 
    ''' 统计指标优先使用区块之内的共线基因对数量；部分历史归档之中区块的基因对数量全部为0，
    ''' 这个时候自动退化为使用共线性区块个数作为指标，保证可视化结果依然是有意义的。
    ''' </remarks>
    Private Function BuildCollinearityData(result As PanGenomeResult) As CollinearityDataset
        If result.CollinearBlocks Is Nothing OrElse result.CollinearBlocks.Length = 0 Then
            Return New CollinearityDataset With {
                .genomes = New String() {},
                .matrix = New Double()() {},
                .metricLabel = CollinearMetric_Pairs,
                .pairs = New CollinearPairItem() {},
                .truncated = False
            }
        End If

        Dim useBlocks As Boolean = result.CollinearBlocks.Sum(Function(b) CLng(b.GenePairCount)) = 0

        Dim pairTotals As New Dictionary(Of String, Dictionary(Of String, Integer))()
        Dim blockTotals As New Dictionary(Of String, Dictionary(Of String, Integer))()
        Dim chrPairs As New Dictionary(Of String, Dictionary(Of String, String))()

        For Each block As CollinearBlock In result.CollinearBlocks
            Dim g1 As String = block.Genome1
            Dim g2 As String = block.Genome2

            If String.IsNullOrEmpty(g1) OrElse String.IsNullOrEmpty(g2) OrElse g1 = g2 Then
                Continue For
            End If

            Dim genePairs As Integer = block.GenePairCount

            Call AddPairTotal(pairTotals, g1, g2, genePairs)
            Call AddPairTotal(pairTotals, g2, g1, genePairs)
            Call AddPairTotal(blockTotals, g1, g2, 1)
            Call AddPairTotal(blockTotals, g2, g1, 1)

            Call SetChrPair(chrPairs, g1, g2, $"{block.Chr1} ↔ {block.Chr2}")
            Call SetChrPair(chrPairs, g2, g1, $"{block.Chr1} ↔ {block.Chr2}")
        Next

        Dim metricTotals As Dictionary(Of String, Dictionary(Of String, Integer)) = If(useBlocks, blockTotals, pairTotals)
        Dim metricLabel As String = If(useBlocks, CollinearMetric_Blocks, CollinearMetric_Pairs)
        Dim genomeTotals As New Dictionary(Of String, Integer)()

        For Each kvp As KeyValuePair(Of String, Dictionary(Of String, Integer)) In metricTotals
            genomeTotals(kvp.Key) = kvp.Value.Values.Sum()
        Next

        ' 只保留参与过共线性的基因组，按照参与总量降序取Top-N，
        ' 然后再按名称排序，保证矩阵在页面上的展示顺序是稳定的
        Dim selected As String() = genomeTotals _
            .OrderByDescending(Function(kv) kv.Value) _
            .Select(Function(kv) kv.Key) _
            .Take(MaxCollinearityGenomes) _
            .OrderBy(Function(x) x) _
            .ToArray()

        Dim matrix As Double()() = selected _
            .Select(Function(r)
                        Return selected _
                            .Select(Function(c)
                                        If r = c Then
                                            Return 0.0
                                        Else
                                            Return CDbl(GetNestedValue(metricTotals, r, c))
                                        End If
                                    End Function) _
                            .ToArray
                    End Function) _
            .ToArray

        ' 排行数据按照基因组对聚合，只取字典序较小的一个方向以避免重复
        Dim ranked As New List(Of CollinearPairItem)()

        For Each a As KeyValuePair(Of String, Dictionary(Of String, Integer)) In metricTotals
            For Each b As KeyValuePair(Of String, Integer) In a.Value
                If b.Value > 0 AndAlso String.Compare(a.Key, b.Key, StringComparison.Ordinal) < 0 Then
                    Call ranked.Add(New CollinearPairItem With {
                        .genome1 = a.Key,
                        .genome2 = b.Key,
                        .value = CDbl(b.Value),
                        .blocks = GetNestedValue(blockTotals, a.Key, b.Key),
                        .chromosomes = GetChrPair(chrPairs, a.Key, b.Key)
                    })
                End If
            Next
        Next

        Dim topPairs As CollinearPairItem() = ranked _
            .OrderByDescending(Function(p) p.value) _
            .ThenBy(Function(p) p.genome1) _
            .Take(MaxCollinearityPairs) _
            .ToArray

        Return New CollinearityDataset With {
            .genomes = selected,
            .matrix = matrix,
            .metricLabel = metricLabel,
            .pairs = topPairs,
            .truncated = genomeTotals.Count > selected.Length
        }
    End Function

    Private Sub AddPairTotal(totals As Dictionary(Of String, Dictionary(Of String, Integer)),
                             g1 As String, g2 As String, value As Integer)
        Dim row As Dictionary(Of String, Integer) = Nothing

        If Not totals.TryGetValue(g1, row) Then
            row = New Dictionary(Of String, Integer)()
            totals.Add(g1, row)
        End If

        row(g2) = GetOrZero(row, g2) + value
    End Sub

    Private Sub SetChrPair(table As Dictionary(Of String, Dictionary(Of String, String)),
                           g1 As String, g2 As String, chromosomes As String)
        Dim row As Dictionary(Of String, String) = Nothing

        If Not table.TryGetValue(g1, row) Then
            row = New Dictionary(Of String, String)()
            table.Add(g1, row)
        End If

        row(g2) = chromosomes
    End Sub

    Private Function GetNestedValue(totals As Dictionary(Of String, Dictionary(Of String, Integer)),
                                    a As String, b As String) As Integer
        Dim row As Dictionary(Of String, Integer) = Nothing

        If totals.TryGetValue(a, row) AndAlso row.ContainsKey(b) Then
            Return row(b)
        Else
            Return 0
        End If
    End Function

    Private Function GetChrPair(table As Dictionary(Of String, Dictionary(Of String, String)),
                                a As String, b As String) As String
        Dim row As Dictionary(Of String, String) = Nothing
        Dim text As String = Nothing

        If table.TryGetValue(a, row) AndAlso row.TryGetValue(b, text) Then
            Return text
        Else
            Return ""
        End If
    End Function

#End Region

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
        Dim totalPairs As Integer = result.CollinearBlocks.Sum(Function(b) b.GenePairCount)
        sb.AppendLine($"<div class='stat-card'><span class='stat-value'>{totalPairs}</span><span class='stat-label'>同源基因对总数</span></div>")

        ' 统计涉及的基因组对
        Dim genomePairs As Integer = result.CollinearBlocks.Select(Function(b) $"{b.Genome1} vs {b.Genome2}").Distinct().Count()
        sb.AppendLine($"<div class='stat-card'><span class='stat-value'>{genomePairs}</span><span class='stat-label'>比较基因组对数</span></div>")

        sb.AppendLine("</div>")

        ' 列出主要共线性区块（按照基因对数降序）
        sb.AppendLine("<div class='table-scroll' style='max-height: 420px;'>")
        sb.AppendLine("<table class='data-table'>")
        sb.AppendLine("<thead><tr><th>基因组对</th><th>染色体</th><th>基因对数</th></tr></thead>")
        sb.AppendLine("<tbody>")

        For Each block In result.CollinearBlocks _
                .OrderByDescending(Function(b) b.GenePairCount) _
                .Take(20)
            Dim pairCount As Integer = block.GenePairCount
            sb.AppendLine($"<tr><td>{H(block.Genome1)} ↔ {H(block.Genome2)}</td><td>{H(block.Chr1)} ↔ {H(block.Chr2)}</td><td>{pairCount}</td></tr>")
        Next

        sb.AppendLine("</tbody></table>")
        sb.AppendLine("</div>")
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 生成基因家族详细表格
    ''' </summary>
    Private Function GenerateGeneFamilyTable(result As PanGenomeResult) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("<div class='table-scroll' style='max-height: 640px;'>")
        sb.AppendLine("<table class='data-table'>")
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

            sb.AppendLine($"<tr><td class='family-id'>{H(familyId)}</td><td><span class='badge {GetCategoryClass(category)}'>{category}</span></td><td>{genes.Length}</td><td>{presenceCount}/{result.TotalGenesInGenomes.Count}</td><td class='gene-list'>{H(repGene)}</td></tr>")
            displayCount += 1
        Next

        sb.AppendLine("</tbody></table>")
        sb.AppendLine("</div>")

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

End Module
