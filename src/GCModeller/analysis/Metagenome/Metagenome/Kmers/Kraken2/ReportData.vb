Namespace Kmers.Kraken2

    ''' <summary>
    ''' 用于存储 --output 文件中每一行的数据
    ''' </summary>
    Public Class KrakenOutputRecord
        ''' <summary>
        '''  "C" or "U"
        ''' </summary>
        ''' <returns></returns>
        Public Property StatusCode As String
        Public Property ReadName As String
        Public Property TaxID As Long
        Public Property ReadLength As Integer
        ''' <summary>
        ''' 使用字典来存储 k-mer 的分配详情，键是 TaxID，值是 k-mer 数量
        ''' </summary>
        ''' <returns></returns>
        Public Property LcaMappings As New Dictionary(Of Long, Integer)
    End Class

    ''' <summary>
    ''' 用于存储 --report 文件中每一行的数据
    ''' </summary>
    Public Class KrakenReportRecord
        Public Property Percentage As Double
        ''' <summary>
        ''' 该节点及子节点读数
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadsAtRank As Long
        ''' <summary>
        ''' 直接分配到该节点的读数
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadsDirect As Long
        Public Property RankCode As String
        Public Property TaxID As Long
        Public Property ScientificName As String
    End Class
End Namespace