Imports System.ComponentModel

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' For convenience, we describe levels of delta-differences for some reference 
    ''' examples (all values multiplied by 1000). 
    ''' (compatible rename of the legacy ``SimilarDiscriptions`` enum, 
    ''' follows the paper's ``delta*`` symbol)
    ''' </summary>
    Public Enum DeltaStarLevels As Integer

        ''' <summary>
        ''' (delta* &lt;= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes).
        ''' </summary>
        <Description("delta* <= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes")>
        Close

        ''' <summary>
        ''' (55 &lt;= delta* &lt;= 85; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena).
        ''' </summary>
        <Description("delta* = [55, 85]; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena")>
        ModeratelySimilar

        ''' <summary>
        ''' (90 &lt;= delta* &lt;= 120; human vs sea urchin, M. genitalium vs M. pneumoniae).
        ''' </summary>
        <Description("delta* = [90, 120]; human vs sea urchin, M. genitalium vs M. pneumoniae")>
        WeaklySimilar

        ''' <summary>
        ''' (125 &lt;= delta* &lt;= 145; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum).
        ''' </summary>
        <Description("delta* = [125, 145]; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum")>
        DistantlySimilar

        ''' <summary>
        ''' (150 &lt;= delta* &lt;= 180; human vs Drosophila, E. coli vs Helicobacter pylori).
        ''' </summary>
        <Description("delta* = [150, 180]; human vs Drosophila, E. coli vs Helicobacter pylori")>
        Distant

        ''' <summary>
        ''' (delta* &gt;= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium).
        ''' </summary>
        <Description("delta* >= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium")>
        VeryDistant
    End Enum
End Namespace
