Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 合成的演示数据集
''' </summary>
''' <remarks>
''' 除了层级本体与样本之外，这里还保留了数据生成时所使用的"驱动基因"与"驱动通路"真值。
''' 由于样本标签完全由驱动基因上的改变所决定，因此在 demo 测试中可以拿
''' DeepLIFT 归因给出的 Top 基因 / Top 通路与这里记录的真值做比对，
''' 从而验证"模型确实学到了正确的生物学信号"，而不只是拟合了噪声。
''' </remarks>
Public Class DemoDataset

    ''' <summary>
    ''' 合成的类 Reactome 通路层级
    ''' </summary>
    ''' <returns>层级本体对象</returns>
    Public Property Hierarchy As PathwayHierarchy

    ''' <summary>
    ''' 合成的患者样本
    ''' </summary>
    ''' <returns>样本数据集</returns>
    Public Property Samples As PNETSampleSet

    ''' <summary>
    ''' 生成数据时所指定的驱动基因（标签完全由这些基因上的改变决定）
    ''' </summary>
    ''' <returns>驱动基因名数组</returns>
    Public Property DriverGenes As String()

    ''' <summary>
    ''' 驱动基因所对应的改变类型，索引与 <see cref="DriverGenes"/> 一致
    ''' </summary>
    ''' <returns>改变类型数组，取值为 mutation / amplification / deletion</returns>
    Public Property DriverAlterations As String()

    ''' <summary>
    ''' 生成数据时所指定的驱动通路（驱动基因主要聚集在这几条最精细的通路之中）
    ''' </summary>
    ''' <returns>驱动通路名数组</returns>
    Public Property DriverPathways As String()

    ''' <summary>
    ''' 把整个演示数据集落盘到指定目录
    ''' </summary>
    ''' <param name="directory">输出目录</param>
    ''' <remarks>
    ''' 会写出：逐层的 <c>*.gmt</c> 通路层级文件、<c>features.csv</c>、<c>labels.csv</c>
    ''' 以及记录真值的 <c>ground_truth.txt</c>。
    ''' </remarks>
    Public Sub Save(directory As String)
        Call GmtIO.WriteHierarchy(Hierarchy, directory)
        Call Samples.Save(directory)

        Dim sb As New StringBuilder()

        Call sb.AppendLine("# 合成数据时所使用的数据生成真值")

        For i As Integer = 0 To DriverPathways.Length - 1
            Call sb.AppendLine($"driver_pathway{ChrW(9)}{DriverPathways(i)}")
        Next

        For i As Integer = 0 To DriverGenes.Length - 1
            Call sb.AppendLine($"driver_gene{ChrW(9)}{DriverGenes(i)}{ChrW(9)}{DriverAlterations(i)}")
        Next

        Call File.WriteAllText(System.IO.Path.Combine(directory, "ground_truth.txt"), sb.ToString(), Encoding.UTF8)
    End Sub

End Class

''' <summary>
''' P-NET 演示数据合成器
''' </summary>
''' <remarks>
''' 真实场景下 P-NET 的输入来自 Armenia 等人队列的 1013 例前列腺癌样本
''' （体细胞突变经非同义过滤、拷贝数经 GISTIC2.0 调用后取高水平扩增与深缺失的两态编码），
''' 通路层级来自 Reactome 的 3007 条通路。
'''
''' 这里出于演示目的，用代码合成一份结构相同但规模小得多的数据：
'''
''' 1. 合成一个由"基因 → 5 层通路"组成的层级，其中第一条最精细通路被强制
'''    填充为论文中所报道的已知驱动基因（AR、TP53、PTEN、RB1、MDM4、FGFR1、NOTCH1、PDGFA 等）；
''' 2. 以其中若干条精细通路作为"驱动通路"，其成员基因作为"驱动基因"，
'''    先采样标签，再依据标签决定驱动基因上是否出现对应类型的改变，
'''    其余基因上的改变则作为背景噪声；
''' 3. 由于驱动基因聚集在同一条通路之内，标签只能通过"基因 → 通路 → 更粗通路"
'''    这条层级链路被模型学到，因此可以检验生物先验带来的归纳偏置是否真的起作用。
''' </remarks>
Public Module DemoData

    ''' <summary>
    ''' 论文中所报道的若干前列腺癌已知 / 候选驱动基因，
    ''' 合成数据时会被强制放入第一条最精细通路之中
    ''' </summary>
    ''' <returns>基因名数组</returns>
    Public ReadOnly Property KnownDriverGenes As String() = {
        "AR", "TP53", "PTEN", "RB1", "MDM4", "FGFR1", "NOTCH1", "PDGFA",
        "MDM2", "MYC", "CDK12", "BRCA2", "ATM", "PIK3CA", "SPOP", "FOXA1"
    }

    ''' <summary>
    ''' 基因层面的背景改变率，依次为突变、扩增、缺失
    ''' </summary>
    ''' <returns>改变率数组</returns>
    Public ReadOnly Property BackgroundRates As Double() = {0.03, 0.025, 0.025}

    ''' <summary>
    ''' 已知驱动基因在真实样本之中最主要的改变类型
    ''' </summary>
    ''' <returns>基因名到改变类型下标的映射，0 为突变、1 为扩增、2 为缺失</returns>
    ''' <remarks>
    ''' 论文的 Sankey 图揭示了改变类型与基因之间的对应关系：
    ''' AR 主要由扩增驱动、TP53 主要由突变驱动、PTEN 主要由缺失驱动。
    ''' 这里沿用这三类真实对应关系来合成数据，
    ''' 使得 demo 中"改变类型层面的归因结果"可以直接与真实生物学结论相互印证。
    ''' </remarks>
    Public ReadOnly Property KnownAlterations As Dictionary(Of String, Integer) =
        New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase) From {
            {"AR", 1}, {"TP53", 0}, {"PTEN", 2}, {"RB1", 2},
            {"MDM4", 1}, {"FGFR1", 1}, {"NOTCH1", 0}, {"PDGFA", 1},
            {"MDM2", 1}, {"MYC", 1}, {"CDK12", 0}, {"BRCA2", 2},
            {"ATM", 0}, {"PIK3CA", 0}, {"SPOP", 0}, {"FOXA1", 0}
        }

    ''' <summary>
    ''' 改变类型的显示名称，依次为突变、扩增、缺失
    ''' </summary>
    ''' <returns>改变类型名数组</returns>
    Public ReadOnly Property AlterationNames As String() = {"mutation", "amplification", "deletion"}

    ''' <summary>
    ''' 一次性生成层级与样本
    ''' </summary>
    ''' <param name="geneCount">基因数量，默认 60</param>
    ''' <param name="sampleCount">样本数量，默认 600</param>
    ''' <param name="seed">随机数种子，给出之后数据可复现</param>
    ''' <param name="positiveRatio">正样本（转移性 / 耐药）的比例，默认 0.33</param>
    ''' <returns>合成的演示数据集</returns>
    Public Function Create(Optional geneCount As Integer = 60,
                           Optional sampleCount As Integer = 600,
                           Optional seed As Integer? = Nothing,
                           Optional positiveRatio As Double = 0.33) As DemoDataset

        Dim hierarchy As PathwayHierarchy = CreateHierarchy(geneCount, Nothing, seed)
        Dim samples As DemoDataset = CreateDataset(hierarchy, sampleCount, seed, positiveRatio)

        Return samples
    End Function

    ''' <summary>
    ''' 合成一个由基因与 5 层通路组成的类 Reactome 层级
    ''' </summary>
    ''' <param name="geneCount">基因数量</param>
    ''' <param name="levelSizes">
    ''' 由精细到粗排列的各层通路数量，默认取 <c>{20, 12, 7, 4, 2}</c>（即论文所用的 5 层通路）
    ''' </param>
    ''' <param name="seed">随机数种子</param>
    ''' <returns>合成的层级本体</returns>
    ''' <remarks>
    ''' 层级按自底向上的方式生成：
    ''' 先把基因分配到最精细的通路之中（保证每一条通路非空、每一个基因至少属于一条通路），
    ''' 之后再逐层把下一层的节点随机聚合到上一层的若干父通路之中。
    ''' </remarks>
    Public Function CreateHierarchy(Optional geneCount As Integer = 60,
                                    Optional levelSizes As Integer() = Nothing,
                                    Optional seed As Integer? = Nothing) As PathwayHierarchy

        If geneCount < 8 Then
            Throw New ArgumentException("基因数量至少需要为 8", NameOf(geneCount))
        End If
        If levelSizes Is Nothing Then
            levelSizes = New Integer() {20, 12, 7, 4, 2}
        End If

        Dim random As Random = If(seed.HasValue, New Random(seed.Value), New Random())
        Dim geneNames As String() = New String(geneCount - 1) {}
        Dim knownCount As Integer = std.Min(KnownDriverGenes.Length, geneCount)

        For i As Integer = 0 To knownCount - 1
            geneNames(i) = KnownDriverGenes(i)
        Next
        For i As Integer = knownCount To geneCount - 1
            geneNames(i) = $"GENE_{i + 1:D4}"
        Next

        Dim hierarchy As New PathwayHierarchy() With {.GeneNames = geneNames}
        Dim previousSize As Integer = geneCount
        Dim previousMembers As Integer()() = Nothing

        For level As Integer = 0 To levelSizes.Length - 1
            Dim size As Integer = levelSizes(level)
            Dim names As String() = New String(size - 1) {}
            Dim members As List(Of Integer)() = New List(Of Integer)(size - 1) {}

            For i As Integer = 0 To size - 1
                names(i) = $"L{level + 2}_PATHWAY_{i + 1:D3}"
                members(i) = New List(Of Integer)()
            Next

            If level = 0 Then
                ' 第一条最精细通路被强制填充为论文报道的已知驱动基因
                For i As Integer = 0 To knownCount - 1
                    members(0).Add(i)
                Next

                ' 其余基因随机分配到各条通路之中，保证每一个基因至少属于一条通路
                For g As Integer = knownCount To geneCount - 1
                    Dim target As Integer

                    If levelSizes(0) > 1 Then
                        target = random.Next(1, size)
                    Else
                        target = 0
                    End If

                    members(target).Add(g)

                    ' 以一定概率让基因同时属于第二条通路，模拟通路之间的交叉注释
                    If random.NextDouble() < 0.25 AndAlso size > 1 Then
                        Dim extra As Integer = random.Next(0, size)

                        If extra <> target Then
                            members(extra).Add(g)
                        End If
                    End If
                Next

                ' 为空的通路补充若干随机基因，避免出现空通路
                For i As Integer = 1 To size - 1
                    If members(i).Count = 0 Then
                        members(i).Add(random.Next(0, geneCount))
                    End If
                Next
            Else
                ' 把下一层的节点随机聚合到本层的若干父通路之中
                For child As Integer = 0 To previousSize - 1
                    Dim target As Integer = random.Next(0, size)

                    members(target).Add(child)

                    If random.NextDouble() < 0.3 AndAlso size > 1 Then
                        Dim extra As Integer = random.Next(0, size)

                        If extra <> target Then
                            members(extra).Add(child)
                        End If
                    End If
                Next

                For i As Integer = 0 To size - 1
                    If members(i).Count = 0 Then
                        members(i).Add(random.Next(0, previousSize))
                    End If
                Next
            End If

            Dim normalized As Integer()() = New Integer(size - 1)() {}

            For i As Integer = 0 To size - 1
                normalized(i) = members(i).Distinct().OrderBy(Function(x) x).ToArray()
            Next

            Dim levelObject As New HierarchyLevel($"L{level + 2}_pathways", names, normalized)

            hierarchy.Levels.Add(levelObject)

            previousSize = size
        Next

        Return hierarchy.Cleanup()
    End Function

    ''' <summary>
    ''' 在给定的层级之上合成患者样本
    ''' </summary>
    ''' <param name="hierarchy">层级本体</param>
    ''' <param name="sampleCount">样本数量</param>
    ''' <param name="seed">随机数种子</param>
    ''' <param name="positiveRatio">正样本（转移性 / 耐药）比例</param>
    ''' <returns>合成的演示数据集，其中包含用于比对的驱动基因 / 驱动通路真值</returns>
    ''' <remarks>
    ''' 生成过程为：先按给定比例采样标签，之后
    '''
    ''' + 对于驱动基因：阳性样本上以较高的概率（默认 0.45）出现其对应的改变类型，
    '''   阴性样本上则以较低的概率（默认 0.12）出现；
    ''' + 对于其余基因：三种改变类型各自以背景率（约 0.03）随机出现，构成纯噪声。
    '''
    ''' 这样标签只能通过"驱动基因 → 驱动通路 → 更粗通路"这条链路被模型学到，
    ''' 而背景噪声则用来检验稀疏先验的正则化效果。
    ''' </remarks>
    Public Function CreateDataset(hierarchy As PathwayHierarchy,
                                  Optional sampleCount As Integer = 600,
                                  Optional seed As Integer? = Nothing,
                                  Optional positiveRatio As Double = 0.33) As DemoDataset

        Dim random As Random = If(seed.HasValue, New Random(seed.Value + 977), New Random())
        Dim geneCount As Integer = hierarchy.GeneCount
        Dim types As String() = {"mutation", "amplification", "deletion"}

        ' 取最精细的前两条通路作为驱动通路（第一条包含已被强制填充的已知驱动基因）
        Dim driverPathwayCount As Integer = std.Min(2, hierarchy.Levels(0).Count)
        Dim driverPathways As String() = New String(driverPathwayCount - 1) {}
        Dim driverGeneSet As New List(Of Integer)()

        For i As Integer = 0 To driverPathwayCount - 1
            driverPathways(i) = hierarchy.Levels(0).Nodes(i)

            For Each g As Integer In hierarchy.Levels(0).GetMembers(i)
                If Not driverGeneSet.Contains(g) Then
                    driverGeneSet.Add(g)
                End If
            Next
        Next

        Dim driverIdx As Integer() = driverGeneSet.ToArray()
        Dim driverTypes As Integer() = New Integer(driverIdx.Length - 1) {}

        For i As Integer = 0 To driverIdx.Length - 1
            Dim geneName As String = hierarchy.GeneNames(driverIdx(i))

            If KnownAlterations.ContainsKey(geneName) Then
                driverTypes(i) = KnownAlterations(geneName)
            Else
                driverTypes(i) = random.Next(0, 3)
            End If
        Next

        Dim features As New Tensor(sampleCount, geneCount * 3)
        Dim data As Double() = features.Data
        Dim labels As Double() = New Double(sampleCount - 1) {}
        Dim names As String() = New String(sampleCount - 1) {}
        Dim isDriver As Boolean() = New Boolean(geneCount - 1) {}
        Dim driverTypeOf As Integer() = New Integer(geneCount - 1) {}

        For i As Integer = 0 To driverTypeOf.Length - 1
            driverTypeOf(i) = -1
        Next
        For i As Integer = 0 To driverIdx.Length - 1
            isDriver(driverIdx(i)) = True
            driverTypeOf(driverIdx(i)) = driverTypes(i)
        Next

        For s As Integer = 0 To sampleCount - 1
            Dim y As Integer = If(random.NextDouble() < positiveRatio, 1, 0)
            Dim row As Integer = s * geneCount * 3

            labels(s) = y
            names(s) = $"PC_{s + 1:D4}"

            For g As Integer = 0 To geneCount - 1
                Dim offset As Integer = row + g * 3

                For t As Integer = 0 To 2
                    Dim p As Double

                    If isDriver(g) AndAlso driverTypeOf(g) = t Then
                        p = If(y = 1, 0.45, 0.12)
                    Else
                        p = BackgroundRates(t)
                    End If

                    data(offset + t) = If(random.NextDouble() < p, 1.0, 0.0)
                Next
            Next
        Next

        Dim samples As New PNETSampleSet(features, labels, hierarchy.GeneNames, names)
        Dim driverNames As String() = New String(driverIdx.Length - 1) {}
        Dim driverAlterations As String() = New String(driverIdx.Length - 1) {}

        For i As Integer = 0 To driverIdx.Length - 1
            driverNames(i) = hierarchy.GeneNames(driverIdx(i))
            driverAlterations(i) = types(driverTypes(i))
        Next

        Return New DemoDataset With {
            .Hierarchy = hierarchy,
            .Samples = samples,
            .DriverGenes = driverNames,
            .DriverAlterations = driverAlterations,
            .DriverPathways = driverPathways
        }
    End Function

End Module
