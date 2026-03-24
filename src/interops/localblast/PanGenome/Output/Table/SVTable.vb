Public Class SVTable

    Public Property SV_ID As String
    Public Property Type As SVType
    ''' <summary>
    ''' 发生变异的基因组
    ''' </summary>
    ''' <returns></returns>
    Public Property GenomeName As String
    ''' <summary>
    ''' 关联的基因家族ID
    ''' </summary>
    ''' <returns></returns>
    Public Property FamilyID As String
    ''' <summary>
    ''' 涉及的基因ID
    ''' </summary>
    ''' <returns></returns>
    Public Property RelatedGenes As String()
    ''' <summary>
    ''' 描述信息
    ''' </summary>
    ''' <returns></returns>
    Public Property Description As String

End Class
