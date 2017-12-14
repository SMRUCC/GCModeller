Namespace Metagenomics

    Public Enum TaxonomyRanks As Integer
        NA
        ''' <summary>
        ''' 1. 界
        ''' </summary>
        Kingdom = 100
        ''' <summary>
        ''' 2. 门
        ''' </summary>
        Phylum
        ''' <summary>
        ''' 3A. 纲
        ''' </summary>
        [Class]
        ''' <summary>
        ''' 4B. 目
        ''' </summary>
        Order
        ''' <summary>
        ''' 5C. 科
        ''' </summary>
        Family
        ''' <summary>
        ''' 6D. 属
        ''' </summary>
        Genus
        ''' <summary>
        ''' 7E. 种
        ''' </summary>
        Species
        Strain
    End Enum
End Namespace