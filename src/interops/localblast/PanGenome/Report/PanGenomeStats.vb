Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports SMRUCC.genomics.Analysis.PanGenome.ReportJSON

Public Module PanGenomeStats

    ''' <summary>
    ''' 构建基因组基本信息统计数据（基因总数/特有基因数/核心基因占比）
    ''' </summary>
    ''' 
    <Extension>
    Public Function BuildGenomeStats(result As PanGenomeResult) As GenomeStatRow()
        Dim genomeNames As String() = result.TotalGenesInGenomes.Keys.OrderBy(Function(x) x).ToArray()

        ' 预先统计好每一个基因组在特有基因家族/核心基因家族之中出现的数量，
        ' 避免下面的循环退化成 O(基因组数 x 家族数) 的字典查找
        Dim specificCounts As Dictionary(Of String, Integer) = CountFamiliesPerGenome(result, result.SpecificGeneFamilies, genomeNames)
        Dim coreCounts As Dictionary(Of String, Integer) = CountFamiliesPerGenome(result, result.CoreGeneFamilies, genomeNames)

        Return genomeNames _
            .Select(Function(name)
                        Dim geneCount As Integer = result.TotalGenesInGenomes(name)
                        Dim coreCount As Integer = coreCounts(name)

                        Return New GenomeStatRow With {
                            .name = name,
                            .geneCount = geneCount,
                            .specificCount = specificCounts(name),
                            .coreRatio = If(geneCount > 0, coreCount / geneCount * 100, 0)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 统计每一个基因组在给定的基因家族集合之中出现的家族数量
    ''' </summary>
    ''' <param name="result"></param>
    ''' <param name="families">目标基因家族ID列表（例如特有基因家族、核心基因家族）</param>
    ''' <param name="genomeNames">全部的基因组名称</param>
    ''' <returns>Key为基因组名称，Value为该基因组在<paramref name="families"/>之中出现的家族数量</returns>
    ''' <remarks>
    ''' 原来的实现是 "基因组 x 家族" 的双重循环，在上百个基因组、十几万个基因家族的数据集
    ''' 上面会退化成上千万次的字典查找。这里反过来只遍历每一个家族的PAV行，
    ''' 复杂度降低为 O(家族数 x 该家族出现的基因组数)。
    ''' </remarks>
    Private Function CountFamiliesPerGenome(result As PanGenomeResult,
                                            families As String(),
                                            genomeNames As String()) As Dictionary(Of String, Integer)

        Dim counts As New Dictionary(Of String, Integer)()

        For Each name As String In genomeNames
            counts.Add(name, 0)
        Next

        If families Is Nothing OrElse families.Length = 0 Then
            Return counts
        End If

        For Each familyId As String In families
            Dim pavRow As Dictionary(Of String, Integer) = Nothing

            If Not result.PAVMatrix.TryGetValue(familyId, pavRow) OrElse pavRow Is Nothing Then
                Continue For
            End If

            For Each kvp As KeyValuePair(Of String, Integer) In pavRow
                If kvp.Value > 0 AndAlso counts.ContainsKey(kvp.Key) Then
                    counts(kvp.Key) += 1
                End If
            Next
        Next

        Return counts
    End Function

    ''' <summary>
    ''' 基因组三维散点图：基因存在/缺失均衡度（香农信息熵）的坐标轴标题
    ''' </summary>
    Const Entropy_Label As String = "基因存在/缺失均衡度 H"
    ''' <summary>
    ''' 基因组三维散点图：特有基因占比的坐标轴标题
    ''' </summary>
    Const SpecificRatio_Label As String = "特有基因占比 (%)"
    ''' <summary>
    ''' 基因组三维散点图：核心基因占比的坐标轴标题
    ''' </summary>
    Const CoreRatio_Label As String = "核心基因占比 (%)"

    ''' <summary>
    ''' 基因组级别的三维散点图数据：
    ''' 
    ''' + 维度1: 基因存在/缺失均衡度的香农信息熵 H（基于完整的PAV矩阵）
    ''' + 维度2: 特有基因数 / 基因总数
    ''' + 维度3: 核心基因占比
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <param name="stats">基因组基本信息统计（提供特有基因数与核心基因占比）</param>
    ''' <remarks>
    ''' 熵的定义参考 entropy.md 之中的第一种方法：假设泛基因组总共有N个基因家族，
    ''' 某个基因组之中存在K个、缺失N-K个，则 p = K / N，
    ''' H = -(p*log(p) + (1-p)*log(1-p))，这里使用自然对数。
    ''' 
    ''' p越偏离0.5（即缺失了大量非必需基因）熵越低；反之通过水平基因转移获取了大量
    ''' 附属基因、使得"存在/缺失"的比例相对均衡的时候，熵值会升高。
    ''' </remarks>
    ''' 
    <Extension>
    Public Function BuildGenomeEntropyData(result As PanGenomeResult, stats As GenomeStatRow()) As GenomeEntropyDataset
        Dim empty As New GenomeEntropyDataset With {
            .points = New GenomeEntropyPoint() {},
            .entropyLabel = Entropy_Label,
            .specificRatioLabel = SpecificRatio_Label,
            .coreRatioLabel = CoreRatio_Label,
            .familyCount = 0
        }
        Dim genomeNames As String() = result.TotalGenesInGenomes.Keys.OrderBy(Function(x) x).ToArray()
        Dim familyCount As Integer = result.GeneFamilies.Count

        If genomeNames.Length = 0 OrElse familyCount = 0 Then
            Return empty
        End If

        Try
            ' 基因组名称 -> 下标，避免内层循环之中反复做字符串哈希
            Dim genomeIndex As New Dictionary(Of String, Integer)(genomeNames.Length)

            For i As Integer = 0 To genomeNames.Length - 1
                genomeIndex(genomeNames(i)) = i
            Next

            ' 逐家族(逐PAV行)枚举：只有真正"存在"(拷贝数>0)的条目才会做一次哈希查找，
            ' 复杂度为 O(每个家族的基因组出现数)，远优于 "基因组 x 家族" 的双重全表循环
            Dim presence As Integer() = New Integer(genomeNames.Length - 1) {}

            For Each row As Dictionary(Of String, Integer) In result.PAVMatrix.Values
                If row Is Nothing Then
                    Continue For
                End If

                For Each count As KeyValuePair(Of String, Integer) In row
                    If count.Value > 0 Then
                        Dim index As Integer = -1

                        If genomeIndex.TryGetValue(count.Key, index) Then
                            presence(index) += 1
                        End If
                    End If
                Next
            Next

            Dim statTable As Dictionary(Of String, GenomeStatRow) = stats _
                .GroupBy(Function(r) r.name) _
                .ToDictionary(Function(g) g.Key, Function(g) g.First)
            Dim points As GenomeEntropyPoint() = genomeNames _
                .Select(Function(genome, i)
                            Dim stat As GenomeStatRow = Nothing
                            Dim present As Integer = presence(i)
                            Dim geneCount As Integer = result.TotalGenesInGenomes(genome)
                            Dim specificCount As Integer = 0

                            If statTable.TryGetValue(genome, stat) AndAlso stat IsNot Nothing Then
                                specificCount = stat.specificCount
                            End If

                            Return New GenomeEntropyPoint With {
                                .name = genome,
                                .entropy = ShannonEntropy(present, familyCount),
                                .presentFamilies = present,
                                .absentFamilies = familyCount - present,
                                .specificRatio = If(geneCount > 0, specificCount / geneCount * 100, 0),
                                .coreRatio = If(stat IsNot Nothing, stat.coreRatio, 0),
                                .geneCount = geneCount
                            }
                        End Function) _
                .ToArray

            Call $"[pangenome] genome entropy done: {points.Length} genomes, {familyCount} gene families".debug

            Return New GenomeEntropyDataset With {
                .points = points,
                .entropyLabel = Entropy_Label,
                .specificRatioLabel = SpecificRatio_Label,
                .coreRatioLabel = CoreRatio_Label,
                .familyCount = familyCount
            }
        Catch ex As Exception
            Call Debug.WriteLine($"[pangenome] genome entropy analysis failed: {ex.Message}")

            Return empty
        End Try
    End Function

    ''' <summary>
    ''' 计算基因存在/缺失均衡度的香农信息熵（自然对数）
    ''' </summary>
    ''' <param name="present">该基因组之中存在的基因家族数量 K</param>
    ''' <param name="total">泛基因组的基因家族总数 N</param>
    ''' <remarks>
    ''' p = K/N。当 p 为0或者1的时候（全部缺失或者全部存在）熵定义为0，
    ''' 这里做边界短路以避免 Log(0) 产生 Infinity 或者 NaN，
    ''' 因为 NaN 会导致序列化出来的JSON文本不是合法的JSON。
    ''' </remarks>
    Private Function ShannonEntropy(present As Integer, total As Integer) As Double
        If total <= 0 OrElse present <= 0 OrElse present >= total Then
            Return 0
        End If

        Dim p As Double = CDbl(present) / total
        Dim q As Double = 1 - p

        Return -(p * Math.Log(p) + q * Math.Log(q))
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
    ''' PCA降维的目标维度
    ''' </summary>
    Const PCA_Dimensions As Integer = 3
    ''' <summary>
    ''' PCA散点图的着色维度标题
    ''' </summary>
    Const PCA_ColorLabel As String = "核心基因占比 (%)"

    ''' <summary>
    ''' PAV矩阵的PCA分析：取基因总数最多的前<paramref name="MaxPCAFamilies"/>个基因家族构建PAV子矩阵，
    ''' 以基因组为样本、基因家族为特征，降维到<see cref="PCA_Dimensions"/>个维度
    ''' </summary>
    ''' <param name="result">泛基因组分析结果</param>
    ''' <param name="stats">
    ''' 基因组基本信息统计，提供核心基因占比作为散点图的着色维度
    ''' </param>
    ''' <param name="MaxPCAFamilies">
    ''' PCA分析所使用的基因家族数量上限（按照家族的基因总数降序取Top-N）
    ''' </param>
    ''' <remarks>
    ''' 任何数据不足或者计算失败的情况都会返回一个空的数据集，由前端显示"数据不可用"，
    ''' 保证PCA分析的失败不会导致整个报告的生成过程失败。
    ''' </remarks>
    ''' 
    <Extension>
    Public Function BuildPCAData(result As PanGenomeResult, stats As GenomeStatRow(), Optional MaxPCAFamilies As Integer = 5000) As PCAScatterDataset
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
End Module
