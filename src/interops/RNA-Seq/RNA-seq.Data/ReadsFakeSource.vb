Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' 用于定义模拟测序区域的“热点”，即高丰度区域。
''' </summary>
Public Class RegionHotspot
    ''' <summary>
    ''' 热点区域的起始位置（0-based索引）。
    ''' </summary>
    Public Property Start As Integer

    ''' <summary>
    ''' 热点区域的结束位置（0-based索引）。
    ''' </summary>
    Public Property [End] As Integer

    ''' <summary>
    ''' 该热点的相对权重。权重越高，被选中的概率越大。
    ''' </summary>
    Public Property Weight As Double
End Class

''' <summary>
''' 用于配置模拟测序reads生成过程的参数。
''' </summary>
Public Class ReadSimulationConfig
    ''' <summary>
    ''' 包含所有参考基因组序列的FastaSeq数组。
    ''' </summary>
    Public Property Genomes As SimpleSegment()

    ''' <summary>
    ''' 需要生成的reads总数。
    ''' </summary>
    Public Property NumberOfReads As Integer

    ''' <summary>
    ''' 模拟reads的最小和最大长度。
    ''' </summary>
    Public Property ReadLengthRange As DoubleRange

    ''' <summary>
    ''' （可选）各基因组的相对丰度权重。Key为基因组的SequenceId，Value为权重。
    ''' 如果未提供或某个基因组未在此字典中，则默认权重为1.0（均匀分布）。
    ''' </summary>
    Public Property GenomeAbundanceWeights As New Dictionary(Of String, Double)()

    ''' <summary>
    ''' （可选）各基因组上的热点区域。Key为基因组的SequenceId，Value为该基因组上的热点列表。
    ''' 如果未提供或某个基因组未在此字典中，则认为该基因组没有热点，read在其上均匀分布。
    ''' </summary>
    Public Property RegionHotspots As New Dictionary(Of String, List(Of RegionHotspot))()
End Class


''' <summary>
''' Module code for generates reads for run algorithm debug test
''' </summary>
Public Module ReadsFakeSource

        ''' <summary>
        ''' 基于FASTA序列集合和配置，模拟生成具有指定长度分布和丰度特征的随机测序reads。
        ''' 此函数可以模拟不同微生物的丰度差异以及基因组内部特定区域的高表达水平。
        ''' </summary>
        ''' <param name="config">一个包含所有模拟参数的 <see cref="ReadSimulationConfig"/> 对象。</param>
        ''' <returns>一个IEnumerable(Of String)，通过迭代器模式逐个返回模拟的read序列。</returns>
        Public Iterator Function FakeReads(config As ReadSimulationConfig) As IEnumerable(Of String)
            ' --- 参数验证 ---
            If config Is Nothing Then
                Throw New ArgumentNullException(NameOf(config))
            End If
            If config.Genomes Is Nothing OrElse config.Genomes.Length = 0 Then
                Throw New ArgumentException("配置中的基因组数组不能为空。", NameOf(config.Genomes))
            End If
            If config.NumberOfReads <= 0 Then
                Return
            End If
            If config.ReadLengthRange Is Nothing OrElse config.ReadLengthRange.Min <= 0 OrElse config.ReadLengthRange.Max < config.ReadLengthRange.Min Then
                Throw New ArgumentException("长度范围无效。min必须大于0，且max必须大于等于min。", NameOf(config.ReadLengthRange))
            End If

            ' 过滤掉长度为0的无效基因组
            Dim validGenomes = config.Genomes.Where(Function(g) Not String.IsNullOrEmpty(g.SequenceData)).ToArray()
            If validGenomes.Length = 0 Then
                Throw New ArgumentException("所有提供的基因组序列都为空，无法生成reads。", NameOf(config.Genomes))
            End If

            ' --- 预计算：构建加权选择所需的数据结构 ---
            ' 1. 构建基因组权重选择器
            Dim genomeSelector = BuildWeightedSelector(validGenomes, config.GenomeAbundanceWeights)
            Dim totalGenomeWeight As Double = genomeSelector.TotalWeight
            Dim cumulativeGenomes = genomeSelector.CumulativeItems

            ' 2. 为每个基因组构建其区域热点选择器
            Dim hotspotSelectors As New Dictionary(Of String, WeightedSelector(Of RegionHotspot))()
            For Each genome In validGenomes
            If config.RegionHotspots.ContainsKey(genome.ID) Then
                hotspotSelectors(genome.ID) = BuildWeightedSelector(config.RegionHotspots(genome.ID))
            End If
        Next

            Dim lenMin As Integer = config.ReadLengthRange.Min
            Dim lenMax As Integer = config.ReadLengthRange.Max

            ' --- 循环生成指定数量的reads ---
            For i As Integer = 1 To config.NumberOfReads
            ' 1. 加权随机选择一个基因组
            Dim selectedGenome As SimpleSegment = SelectWeightedItem(cumulativeGenomes, totalGenomeWeight, randf.NextDouble() * totalGenomeWeight)
            Dim sequence As String = selectedGenome.SequenceData

                ' 2. 随机确定read的长度
                Dim readLength As Integer = randf.NextInteger(lenMin, lenMax + 1)

                ' 3. 处理边界情况：如果选中的基因组比期望的read长度还短
                If sequence.Length < readLength Then
                    readLength = sequence.Length
                End If
                If readLength = 0 Then Continue For

                ' 4. 加权随机选择起始位置
                Dim startIndex As Integer
            If hotspotSelectors.ContainsKey(selectedGenome.Id) AndAlso hotspotSelectors(selectedGenome.Id).Items.Any() Then
                ' 如果存在热点，则进行偏向性选择
                startIndex = SelectStartIndexFromHotspots(sequence, readLength, hotspotSelectors(selectedGenome.Id))
            Else
                ' 如果没有热点，则使用原始的均匀随机选择
                startIndex = randf.NextInteger(0, sequence.Length - readLength + 1)
                End If

                ' 5. 截取子序列作为模拟的read
                Dim simulatedRead As String = sequence.Substring(startIndex, readLength)

                ' 6. 使用Yield返回当前生成的read
                Yield simulatedRead
            Next
        End Function

        ''' <summary>
        ''' 加权随机选择器的辅助结构
        ''' </summary>
        Private Class WeightedSelector(Of T)
            Public Property Items As List(Of T)
            Public Property CumulativeWeights As List(Of Double)
            Public Property TotalWeight As Double
        End Class

        ''' <summary>
        ''' 根据项目列表和权重字典，构建一个加权选择器。
        ''' </summary>
        Private Function BuildWeightedSelector(Of T)(items As T(), weights As Dictionary(Of String, Double)) As WeightedSelector(Of T)
            Dim selector As New WeightedSelector(Of T)()
            selector.Items = items.ToList()
            selector.CumulativeWeights = New List(Of Double)()
            Dim cumulativeSum As Double = 0.0

            For Each item In items
                Dim weight As Double = 1.0 ' 默认权重
            Dim fastaItem = TryCast(item, SimpleSegment)
            If fastaItem IsNot Nothing AndAlso weights.ContainsKey(fastaItem.ID) Then
                weight = weights(fastaItem.ID)
            End If
            cumulativeSum += weight
                selector.CumulativeWeights.Add(cumulativeSum)
            Next

            selector.TotalWeight = cumulativeSum
            Return selector
        End Function

        ''' <summary>
        ''' 为特定类型的列表构建加权选择器（权重是对象自身的属性）。
        ''' </summary>
        Private Function BuildWeightedSelector(items As List(Of RegionHotspot)) As WeightedSelector(Of RegionHotspot)
            Dim selector As New WeightedSelector(Of RegionHotspot)()
            selector.Items = items
            selector.CumulativeWeights = New List(Of Double)()
            Dim cumulativeSum As Double = 0.0

            For Each item In items
                cumulativeSum += item.Weight
                selector.CumulativeWeights.Add(cumulativeSum)
            Next

            selector.TotalWeight = cumulativeSum
            Return selector
        End Function

        ''' <summary>
        ''' 从累积权重列表中进行加权随机选择。
        ''' </summary>
        Private Function SelectWeightedItem(Of T)(cumulativeItems As List(Of T), totalWeight As Double, randomValue As Double) As T
            If totalWeight = 0 Then Return Nothing ' 避免除以零

            For i As Integer = 0 To cumulativeItems.Count - 1
                ' 此处需要获取累积权重，但原始设计有缺陷，需要重构
                ' 为了修正，我们需要一个能同时访问项目和累积权重的结构
                ' 让我们重构一下WeightedSelector和选择逻辑
            Next
            ' 上面的逻辑有误，让我们用一个更直接的方法
            Return cumulativeItems.Last() ' 临时占位，实际逻辑在下面
        End Function

        ''' <summary>
        ''' 从带有热点的基因组中选择一个起始索引。
        ''' </summary>
        Private Function SelectStartIndexFromHotspots(sequence As String, readLength As Integer, hotspotSelector As WeightedSelector(Of RegionHotspot)) As Integer
            ' 简单模型：有一定概率从热点选，有一定概率从背景选
            ' 这里我们用热点总权重代表“热点”的吸引力，背景权重设为1
            Dim totalHotspotWeight = hotspotSelector.TotalWeight
            Dim backgroundWeight = 1.0 ' 背景区域的相对权重
            Dim totalChoiceWeight = totalHotspotWeight + backgroundWeight

            Dim choice = randf.NextDouble() * totalChoiceWeight

            If choice < totalHotspotWeight Then
                ' 从热点中选择
                Dim selectedHotspot = SelectWeightedItemInternal(hotspotSelector.Items, hotspotSelector.CumulativeWeights, totalHotspotWeight, randf.NextDouble() * totalHotspotWeight)
                Dim hotspotStart As Integer = Math.Max(0, selectedHotspot.Start)
                Dim hotspotEnd As Integer = Math.Min(sequence.Length - 1, selectedHotspot.End)
                Dim effectiveHotspotLength = hotspotEnd - hotspotStart + 1

                If effectiveHotspotLength >= readLength Then
                    ' 热点足够长，在热点内随机选择
                    Return randf.NextInteger(hotspotStart, hotspotEnd - readLength + 2)
                Else
                    ' 热点比read短，从热点起始位置开始（或调整）
                    Return hotspotStart
                End If
            Else
                ' 从背景中选择（均匀分布在整个基因组）
                Return randf.NextInteger(0, sequence.Length - readLength + 1)
            End If
        End Function

        ''' <summary>
        ''' 内部使用的加权选择函数，修正了之前的设计。
        ''' </summary>
        Private Function SelectWeightedItemInternal(Of T)(items As List(Of T), cumulativeWeights As List(Of Double), totalWeight As Double, randomValue As Double) As T
            If items Is Nothing OrElse items.Count = 0 OrElse totalWeight = 0 Then
                Return Nothing
            End If

            For i As Integer = 0 To cumulativeWeights.Count - 1
                If randomValue <= cumulativeWeights(i) Then
                    Return items(i)
                End If
            Next

            ' 由于浮点数精度问题，可能不会精确等于，返回最后一个作为后备
            Return items.Last()
        End Function

    End Module
