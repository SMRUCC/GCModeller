Namespace DEG.OGEE.Models

    Public Class datasets
        Public Property sciName As String
        Public Property kingdom As String
        Public Property taxID As String
        Public Property dataSource As String
        Public Property url As String
        Public Property dataType As String
        Public Property dataSubType As String
        Public Property experimentalTechBrief As String
        Public Property experimentalConditionBrief As String
        Public Property definitionOfEssential As String
        Public Property note As String
        Public Property dateadded As String
        Public Property datasetID As String
    End Class

    Public Class gene_essentiality
        Public Property sciName As String
        Public Property kingdom As String
        Public Property datasetID As String
        Public Property locus As String
        Public Property essential As String
        Public Property pubmedID As String
        Public Property taxID As String
        Public Property thumbup As String
        Public Property thumbdown As String
        Public Property valid As String
        Public Property fitnessScore As String
        Public Property id As String
    End Class

    Public Class genes
        Public Property sciName As String
        Public Property kingdom As String
        Public Property taxID As String
        Public Property locus As String
        Public Property symbols As String
        Public Property description As String
        Public Property proteinSeq As String
        Public Property codingSeq As String
        Public Property seqType As String
        Public Property proteinLength As String

    End Class

    ''' <summary>
    ''' Table join of <see cref="genes"/>, <see cref="gene_essentiality"/>, <see cref="datasets"/>
    ''' </summary>
    Public Class geneSetInfo

        Public Property dataset As datasets
        Public Property essentiality As gene_essentiality
        Public Property gene As genes

    End Class
End Namespace
