Namespace ComponentModel.Loci

    Public Interface IMotifSite

        Property Type As String
        Property Name As String
        Property Site As Location
    End Interface

    ''' <summary>
    ''' This motif site have the scoring calculation value
    ''' </summary>
    Public Interface IMotifScoredSite : Inherits IMotifSite

        Property Score As Double
    End Interface
End Namespace