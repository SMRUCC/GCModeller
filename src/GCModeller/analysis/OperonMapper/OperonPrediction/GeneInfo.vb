Namespace ContextModel

    ''' <summary>
    ''' 基因信息结构
    ''' </summary>
    Public Structure GeneInfo
        Public GeneID As String
        Public Start As Integer
        Public [End] As Integer
        Public Length As Integer
        Public Strand As Char  ' '+'或'-'
        Public GO_Terms As List(Of String)
        Public PhylogeneticProfile As Dictionary(Of String, Boolean) ' 基因组ID -> 存在状态
    End Structure

    ''' <summary>
    ''' 基因组信息结构
    ''' </summary>
    Public Structure GenomeInfo
        Public GenomeID As String
        Public Phylum As String
        Public GeneCount As Integer
        Public GenePositions As Dictionary(Of String, Integer) ' 基因ID -> 位置索引
    End Structure
End Namespace