Public Class SVData

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

    Public ReadOnly Property ClusterSize As Integer
        Get
            Return RelatedGenes.TryCount
        End Get
    End Property

    ''' <summary>
    ''' 描述信息
    ''' </summary>
    ''' <returns></returns>
    Public Property Description As String

End Class

Public Class SVTable : Inherits SVData

    Public Property Category As GeneCategoryType
    Public Property Dispensable As Boolean
    Public Property SingleCopyOrtholog As Boolean

    Sub New()
    End Sub

    Sub New(sv As StructuralVariation)
        SV_ID = sv.SV_ID
        Type = sv.Type
        GenomeName = sv.GenomeName
        FamilyID = sv.FamilyID
        RelatedGenes = sv.RelatedGenes
        Description = sv.Description
    End Sub

End Class
