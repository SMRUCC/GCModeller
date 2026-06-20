Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Analysis.Microarray

''' <summary>
''' DEMO 数据生成器
''' 
''' 模拟多组学+表型数据的因果级联关系：
'''   基因表达 (CHS, CHI, F3H, FLS) -> 黄酮含量 (Quercetin, Kaempferol, Rutin) -> 品质性状 (DPPH, ABTS, 淀粉, 颗粒度)
''' 
''' 真实数据生成机制（用于验证算法能否恢复出已知因果结构）：
'''   - 4 个黄酮合成相关基因表达量，受一个隐藏的"调控因子"驱动
'''   - 基因表达正向驱动 3 种黄酮的绝对含量
'''   - 黄酮含量正向驱动抗氧化活性 (DPPH, ABTS)
'''   - 黄酮含量负向影响淀粉含量（黄酮合成与淀粉竞争碳源）
'''   - 淀粉含量正向影响颗粒度
'''   - 基因对品质的直接效应较弱（仅通过黄酮中介）
''' </summary>
Public Module DataGenerator

    ' 显变量名称（按列顺序）
    Public ReadOnly ManifestVarNames As String() = {
        "CHS_expr", "CHI_expr", "F3H_expr", "FLS_expr",           ' 0-3: 基因表达
        "Quercetin", "Kaempferol", "Rutin",                        ' 4-6: 黄酮含量
        "DPPH", "ABTS", "Starch", "ParticleSize"                   ' 7-10: 品质性状
    }

    ' 潜变量定义（用于 PLS-PM）
    ' 潜变量 0: Gene (基因表达潜变量) - 反映型，显变量 0,1,2,3
    ' 潜变量 1: Flavonoid (黄酮潜变量) - 反映型，显变量 4,5,6
    ' 潜变量 2: Antioxidant (抗氧化潜变量) - 反映型，显变量 7,8
    ' 潜变量 3: StarchQuality (淀粉品质潜变量) - 反映型，显变量 9,10

    ''' <summary>
    ''' 生成 DEMO 数据
    ''' n: 样本量
    ''' seed: 随机种子
    ''' 返回 (n × 11) 的数据矩阵
    ''' </summary>
    Public Function GenerateDemoData(n As Integer, seed As Integer) As Double(,)
        Dim rng As New Random(seed)
        Dim data(n - 1, 10) As Double

        ' 真实参数（用于生成数据，验证算法能否恢复）
        ' 潜变量之间的路径系数
        Dim beta_GeneToFlav = 0.85      ' 基因 -> 黄酮 (强)
        Dim beta_FlavToAnti = 0.75      ' 黄酮 -> 抗氧化 (强)
        Dim beta_FlavToStarch = -0.55   ' 黄酮 -> 淀粉品质 (负向，碳源竞争)
        Dim beta_GeneToAntiDirect = 0.1  ' 基因 -> 抗氧化 (弱直接效应)
        Dim beta_GeneToStarchDirect = -0.05  ' 基因 -> 淀粉品质 (几乎无直接效应)

        For i = 0 To n - 1
            ' 1. 隐藏的调控因子（外生潜变量）
            Dim regulator = Gaussian(rng, 0, 1)

            ' 2. 基因表达 = 调控因子 × 载荷 + 噪声
            ' 4 个基因有不同的载荷，模拟真实生物学差异
            Dim geneLoadings = {0.85, 0.8, 0.75, 0.7}
            Dim genes(3) As Double
            For g = 0 To 3
                genes(g) = geneLoadings(g) * regulator + Gaussian(rng, 0, 0.5)
                data(i, g) = genes(g)
            Next

            ' 3. 计算基因潜变量得分（用于驱动下游）
            Dim geneScore = 0.0
            For g = 0 To 3
                geneScore += genes(g)
            Next
            geneScore /= 4

            ' 4. 黄酮含量 = 基因 × 0.85 + 噪声
            Dim flavScore = beta_GeneToFlav * geneScore + Gaussian(rng, 0, 0.5)
            Dim flavLoadings = {0.85, 0.8, 0.75}
            For f = 0 To 2
                data(i, 4 + f) = flavLoadings(f) * flavScore + Gaussian(rng, 0, 0.4)
            Next

            ' 5. 抗氧化活性 = 黄酮 × 0.75 + 基因 × 0.10 + 噪声
            Dim antiScore = beta_FlavToAnti * flavScore + beta_GeneToAntiDirect * geneScore + Gaussian(rng, 0, 0.4)
            Dim antiLoadings = {0.85, 0.8}
            For a = 0 To 1
                data(i, 7 + a) = antiLoadings(a) * antiScore + Gaussian(rng, 0, 0.4)
            Next

            ' 6. 淀粉品质潜变量 = 黄酮 × (-0.55) + 基因 × (-0.05) + 噪声
            Dim starchScore = beta_FlavToStarch * flavScore + beta_GeneToStarchDirect * geneScore + Gaussian(rng, 0, 0.6)
            Dim starchLoadings = {0.8, 0.75}
            data(i, 9) = starchLoadings(0) * starchScore + Gaussian(rng, 0, 0.4)   ' Starch
            data(i, 10) = starchLoadings(1) * starchScore + Gaussian(rng, 0, 0.4)  ' ParticleSize
        Next

        Return data
    End Function

    ''' <summary>Box-Muller 法生成正态分布随机数</summary>
    Public Function Gaussian(rng As Random, mean As Double, stdDev As Double) As Double
        Dim u1 = 1.0 - rng.NextDouble()
        Dim u2 = 1.0 - rng.NextDouble()
        Dim randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2)
        Return mean + stdDev * randStdNormal
    End Function

    ''' <summary>获取 PLS-PM 潜变量定义</summary>
    Public Iterator Function GetLatentDefinitions() As IEnumerable(Of LatentDefinition)
        ' 潜变量 0: Gene
        Dim geneIdx As New List(Of Integer) From {0, 1, 2, 3}
        Yield New LatentDefinition("Gene", geneIdx.Select(Function(i) ManifestVarNames(i)), MeasurementModels.A)

        ' 潜变量 1: Flavonoid
        Dim flavIdx As New List(Of Integer) From {4, 5, 6}
        Yield New LatentDefinition("Flavonoid", flavIdx.Select(Function(i) ManifestVarNames(i)), MeasurementModels.A)

        ' 潜变量 2: Antioxidant
        Dim antiIdx As New List(Of Integer) From {7, 8}
        Yield New LatentDefinition("Antioxidant", antiIdx.Select(Function(i) ManifestVarNames(i)), MeasurementModels.A)

        ' 潜变量 3: StarchQuality
        Dim starchIdx As New List(Of Integer) From {9, 10}
        Yield New LatentDefinition("StarchQuality", starchIdx.Select(Function(i) ManifestVarNames(i)), MeasurementModels.A)
    End Function

    ''' <summary>获取 PLS-PM 内模型路径定义</summary>
    Public Function GetInnerPaths() As List(Of (Integer, Integer))
        Dim paths As New List(Of (Integer, Integer))()
        ' Gene -> Flavonoid
        paths.Add((0, 1))
        ' Flavonoid -> Antioxidant
        paths.Add((1, 2))
        ' Flavonoid -> StarchQuality
        paths.Add((1, 3))
        ' Gene -> Antioxidant (直接路径，预期较弱)
        paths.Add((0, 2))
        ' Gene -> StarchQuality (直接路径，预期较弱)
        paths.Add((0, 3))
        Return paths
    End Function

    ''' <summary>获取 SEM 路径定义（基于显变量）</summary>
    Public Function GetSEMPaths() As List(Of (Integer, Integer))
        Dim paths As New List(Of (Integer, Integer))()
        ' 使用每个潜变量的代表性显变量构建路径分析
        ' CHS_expr (0) -> Quercetin (4)
        paths.Add((0, 4))
        ' Quercetin (4) -> DPPH (7)
        paths.Add((4, 7))
        ' Quercetin (4) -> Starch (9)
        paths.Add((4, 9))
        ' CHS_expr (0) -> DPPH (7) 直接路径
        paths.Add((0, 7))
        ' CHS_expr (0) -> Starch (9) 直接路径
        paths.Add((0, 9))
        ' Starch (9) -> ParticleSize (10)
        paths.Add((9, 10))
        Return paths
    End Function

    ''' <summary>获取 SEM 使用的变量子集名称</summary>
    Public Function GetSEMVarNames() As String()
        Return {"CHS_expr", "Quercetin", "DPPH", "Starch", "ParticleSize"}
    End Function

    ''' <summary>获取 SEM 使用的变量子集数据</summary>
    Public Function GetSEMDataSubset(data As Double(,)) As Double(,)
        Dim n = data.GetLength(0)
        ' 选取列: 0 (CHS), 4 (Quercetin), 7 (DPPH), 9 (Starch), 10 (ParticleSize)
        Dim cols = {0, 4, 7, 9, 10}
        Dim result(n - 1, cols.Length - 1) As Double
        For i = 0 To n - 1
            For j = 0 To cols.Length - 1
                result(i, j) = data(i, cols(j))
            Next
        Next
        Return result
    End Function

    ''' <summary>将 SEM 路径索引转换（基于子集）</summary>
    Public Function GetSEMPathsOnSubset() As List(Of (Integer, Integer))
        Dim paths As New List(Of (Integer, Integer))()
        ' 子集索引: 0=CHS, 1=Quercetin, 2=DPPH, 3=Starch, 4=ParticleSize
        paths.Add((0, 1))  ' CHS -> Quercetin
        paths.Add((1, 2))  ' Quercetin -> DPPH
        paths.Add((1, 3))  ' Quercetin -> Starch
        paths.Add((0, 2))  ' CHS -> DPPH (直接)
        paths.Add((0, 3))  ' CHS -> Starch (直接)
        paths.Add((3, 4))  ' Starch -> ParticleSize
        Return paths
    End Function

    ''' <summary>打印数据概览</summary>
    Public Sub PrintDataSummary(data As Double(,), varNames As String())
        Dim n = data.GetLength(0)
        Dim p = data.GetLength(1)
        Console.WriteLine("="c, 80)
        Console.WriteLine("DEMO 数据概览")
        Console.WriteLine("="c, 80)
        Console.WriteLine($"样本量 N = {n}, 变量数 = {p}")
        Console.WriteLine()
        Console.WriteLine($"{"变量",-20}{"均值",12}{"标准差",12}{"最小值",12}{"最大值",12}")
        Console.WriteLine("-"c, 80)
        For j = 0 To p - 1
            Dim col(n - 1) As Double
            For i = 0 To n - 1
                col(i) = data(i, j)
            Next
            Dim mean = Statistics.Mean(col)
            Dim std = col.SD
            Dim min = Double.MaxValue, max = Double.MinValue
            For Each v In col
                If v < min Then min = v
                If v > max Then max = v
            Next
            Console.WriteLine($"{varNames(j),-20}{mean,12:F4}{std,12:F4}{min,12:F4}{max,12:F4}")
        Next
        Console.WriteLine()

        ' 相关系数矩阵
        Console.WriteLine("-"c, 80)
        Console.WriteLine("变量间相关系数矩阵")
        Console.WriteLine("-"c, 80)
        Dim corr = Statistics.CorrelationMatrix(data)
        Console.Write($"{"",-20}")
        For j = 0 To p - 1
            Console.Write($"{varNames(j).Substring(0, Math.Min(8, varNames(j).Length)),10}")
        Next
        Console.WriteLine()
        For i = 0 To p - 1
            Console.Write($"{varNames(i).Substring(0, Math.Min(18, varNames(i).Length)),-20}")
            For j = 0 To p - 1
                Console.Write($"{corr(i, j),10:F3}")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
    End Sub

End Module
