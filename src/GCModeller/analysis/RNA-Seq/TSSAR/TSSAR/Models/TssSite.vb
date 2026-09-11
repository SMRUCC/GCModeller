Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace Models

    ''' <summary>
    ''' 一条被注释为转录起始位点（TSS）的结果记录。
    ''' </summary>
    Public Class TssSite : Implements ILocationSegment

        ''' <summary>
        ''' 参考序列（染色体/contig）名称。
        ''' </summary>
        Public Property Chromosome As String

        ''' <summary>
        ''' TSS 在基因组上面的 1-based 位置。
        ''' </summary>
        Public Property Position As Integer

        ''' <summary>
        ''' TSS 所在的链方向。
        ''' </summary>
        Public Property Strand As Strands

        ''' <summary>
        ''' 多窗口几何平均之后的最终 p 值。
        ''' </summary>
        Public Property PValue As Double

        ''' <summary>
        ''' 归一化之后 [+] 与 [-] 文库的 read 起始计数差（peak difference）。
        ''' </summary>
        Public Property PeakDifference As Double

        ''' <summary>
        ''' [+] 文库在该位置的原始 read 起始计数。
        ''' </summary>
        Public Property PlusCoverage As Double

        ''' <summary>
        ''' [-] 文库在该位置的原始 read 起始计数。
        ''' </summary>
        Public Property MinusCoverage As Double

        ''' <summary>
        ''' 基于基因注释的上下文分类。
        ''' </summary>
        Public Property Type As TssTypes = TssTypes.Orphan

        ''' <summary>
        ''' 关联基因的 locus_tag（若存在）。
        ''' </summary>
        Public Property Gene As String

        ''' <summary>
        ''' 关联基因的位置字符串（若存在）。
        ''' </summary>
        Public Property GeneLocation As String

        ''' <summary>
        ''' 当分类为 <see cref="TssTypes.Primary"/> 时，TSS 到关联基因起始密码子的距离（5'UTR 长度）。
        ''' 非 Primary 时为 0。
        ''' </summary>
        Public Property UtrLength As Integer

        ''' <summary>
        ''' TEX 处理效率估计：``[+] / ([+] + [-])``，取值区间 ``[0, 1]``。
        ''' 该值越高说明该位点的初级转录本富集越明显。
        ''' </summary>
        Public Property TexEfficiency As Double

        ''' <summary>
        ''' 该 TSS 在输出 BED 文件之中的编号（``TSS_00001`` 格式）。
        ''' </summary>
        Public Property TssId As String

        Public ReadOnly Property Location As Location Implements ILocationSegment.Location
            Get
                Return New NucleotideLocation(Position, Position, Strand)
            End Get
        End Property

        Public ReadOnly Property UniqueId As String Implements ILocationSegment.UniqueId
            Get
                Return $"{Chromosome}:{Position}({Strand})"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Chromosome}:{Position} {Strand} p={PValue:G4} d={PeakDifference:F4} [{Type}]"
        End Function
    End Class

    ''' <summary>
    ''' 由于零膨胀 Poisson 回归不能收敛（无法建模）而被排除出分析的基因组区间。
    ''' </summary>
    Public Class DumpRegion

        ''' <summary>
        ''' 参考序列名称。
        ''' </summary>
        Public Property Chromosome As String

        ''' <summary>
        ''' 区间起始（1-based）。
        ''' </summary>
        Public Property Start As Integer

        ''' <summary>
        ''' 区间结束（1-based，包含）。
        ''' </summary>
        Public Property Ends As Integer

        ''' <summary>
        ''' 所属链方向。
        ''' </summary>
        Public Property Strand As Strands

        Public ReadOnly Property Length As Integer
            Get
                Return Ends - Start + 1
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Chromosome}:{Start}-{Ends} {Strand}"
        End Function
    End Class
End Namespace
