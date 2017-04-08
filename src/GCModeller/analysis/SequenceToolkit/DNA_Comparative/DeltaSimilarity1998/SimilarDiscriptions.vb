Imports System.ComponentModel

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' For convenience, we describe levels of sigma-differences for some reference examples (all values mutliplied by 1000)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SimilarDiscriptions As Integer

        ''' <summary>
        ''' (sigma &lt;= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma <= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes")>
        Close

        ''' <summary>
        ''' (55 &lt;= sigma &lt;= 85; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [55, 85]; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena")>
        ModeratelySimilar

        ''' <summary>
        ''' (90 &lt;= sigma &lt;= 120; human vs sea urchin, M. genitalium vs M. pneumoniae).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [90, 120]; human vs sea urchin, M. genitalium vs M. pneumoniae")>
        WeaklySimilar

        ''' <summary>
        ''' (125 &lt;= sigma &lt;= 145; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [125, 145]; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum")>
        DistantlySimilar

        ''' <summary>
        ''' (150 &lt;= sigma &lt;= 180; human vs Drosophilia, E. coli vs Helicobacter pylori).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [150, 180]; human vs Drosophilia, E. coli vs Helicobacter pylori")>
        Distant

        ''' <summary>
        ''' (sigma >= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma >= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium")>
        VeryDistant
    End Enum
End Namespace