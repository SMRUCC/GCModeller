Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

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
    Public Property ReadLengthRange As IntRange

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