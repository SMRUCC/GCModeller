Namespace ContextModel

    ''' <summary>
    ''' 参考基因组信息结构，用于计算邻域保守性和系统发育距离
    ''' </summary>
    Public Structure GenomeInfo

        ''' <summary>
        ''' 参考基因组的唯一标识符
        ''' </summary>
        Public Property GenomeID As String
        ''' <summary>
        ''' 参考基因组所属的门
        ''' 用于计算基因在该门中的存在概率 pik
        ''' </summary>
        Public Property Phylum As String
        ''' <summary>
        ''' 参考基因组中的基因总数 Nk
        ''' </summary>
        Public Property GeneCount As Integer
        ''' <summary>
        ''' 基因在基因组中的位置索引字典，键为基因ID，值为位置索引
        ''' 用于计算邻域保守性中的 dk(ij) (两基因间的基因数量)
        ''' </summary>
        ''' <remarks>
        ''' 基因ID -> 位置索引
        ''' </remarks>
        Public Property GenePositions As Dictionary(Of String, Integer)

    End Structure
End Namespace