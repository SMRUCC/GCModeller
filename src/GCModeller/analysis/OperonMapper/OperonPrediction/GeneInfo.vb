Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    ''' <summary>
    ''' 基因信息结构，存储单个基因的坐标、方向及功能注释等信息
    ''' </summary>
    Public Structure GeneInfo
        ''' <summary>
        ''' 基因的唯一标识符
        ''' </summary>
        Public Property GeneID As String
        ''' <summary>
        ''' 基因在基因组上的起始位置
        ''' </summary>
        Public Property Start As Integer
        ''' <summary>
        ''' 基因在基因组上的终止位置
        ''' </summary>
        Public Property [End] As Integer
        ''' <summary>
        ''' 基因的长度 (bp)
        ''' </summary>
        Public Property Length As Integer
        ''' <summary>
        ''' 基因所在的链方向 ('+' 或 '-')
        ''' </summary>
        Public Property Strand As Strands
        ''' <summary>
        ''' 基因关联的 Gene Ontology 术语列表
        ''' </summary>
        Public Property GO_Terms As List(Of String)
        Public Property KO_Terms As List(Of String)
        ''' <summary>
        ''' 基因的系统发育谱，键为参考基因组ID，值为该基因是否在该基因组中存在
        ''' </summary>
        ''' <remarks>
        ''' 基因组ID -> 存在状态
        ''' </remarks>
        Public Property PhylogeneticProfile As Dictionary(Of String, Boolean)

    End Structure

    ''' <summary>
    ''' 基因间距离分组枚举，用于分类器的分组训练策略
    ''' 论文 Results 部分指出，根据基因间距离将数据集分为三个子组能有效降低分类误差
    ''' </summary>
    Public Enum IntergenicDistanceGroup
        NA

        ''' <summary>
        ''' 基因间距离小于 40 nt (U40)
        ''' </summary>
        U40   ' < 40 nt
        ''' <summary>
        ''' 基因间距离在 40 nt 到 200 nt 之间 (U200)
        ''' </summary>
        U200  ' 40 - 200 nt
        ''' <summary>
        ''' 基因间距离大于 200 nt (O200)
        ''' </summary>
        O200  ' > 200 nt
    End Enum
End Namespace