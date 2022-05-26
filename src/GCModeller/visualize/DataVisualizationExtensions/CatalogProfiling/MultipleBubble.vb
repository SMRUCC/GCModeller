Namespace CatalogProfiling

    ''' <summary>
    ''' kegg enrichment bubble in multiple groups
    ''' 
    ''' 1. x axis is related to the -log10(pvalue)
    ''' 2. y axis is the category of the kegg pathway maps
    ''' 3. bubble size is the impact factor or pathway hit score
    ''' 4. color of the bubble can be related to the another score
    ''' </summary>
    Public Class MultipleBubble

        ReadOnly data As Dictionary(Of String, BubbleTerm())

    End Class
End Namespace