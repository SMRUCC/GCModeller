Imports SMRUCC.genomics.ComponentModel.Annotation

Public Class SVData : Implements IOrthologyCluster

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
    Public Property FamilyID As String Implements IOrthologyCluster.FamilyID
    ''' <summary>
    ''' 涉及的基因ID
    ''' </summary>
    ''' <returns></returns>
    Public Property RelatedGenes As String() Implements IOrthologyCluster.GeneCluster
    Public Property CopyNumber As Integer
    Public Property Median As Double
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

    Public ReadOnly Property ClusterSize As Integer
        Get
            Return RelatedGenes.TryCount
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(sv As StructuralVariation)
        SV_ID = sv.SV_ID
        Type = sv.Type
        GenomeName = sv.GenomeName
        FamilyID = sv.FamilyID
        RelatedGenes = sv.RelatedGenes
        Description = sv.Description
        CopyNumber = sv.CopyNumber
        Median = sv.Median
    End Sub

End Class
