Public Class GeneticNode

    Public Property ID As String
    Public Property GO As String()
    Public Property KO As String
    Public Property Sequence As String
    Public Property [Function] As String
    ''' <summary>
    ''' 这个节点的数据源之中的原始编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Xref As String

    Public Overrides Function ToString() As String
        Return [Function]
    End Function

End Class

