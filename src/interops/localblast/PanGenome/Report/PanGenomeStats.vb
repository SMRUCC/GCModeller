Imports System.Runtime.CompilerServices
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
End Module
