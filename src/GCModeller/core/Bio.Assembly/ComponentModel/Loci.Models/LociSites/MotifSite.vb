Namespace ComponentModel.Loci

    ''' <summary>
    ''' Motif site model on both DNA/RNA and protein sequence.
    ''' </summary>
    Public Interface IMotifSite

        ''' <summary>
        ''' loci types
        ''' </summary>
        ''' <returns></returns>
        Property Type As String
        ''' <summary>
        ''' loci name
        ''' </summary>
        ''' <returns></returns>
        Property Name As String
        Property Site As Location
    End Interface

    ''' <summary>
    ''' This motif site have the scoring calculation value
    ''' </summary>
    Public Interface IMotifScoredSite : Inherits IMotifSite

        ''' <summary>
        ''' The site score of this SNP site
        ''' </summary>
        ''' <returns></returns>
        Property Score As Double
    End Interface
End Namespace