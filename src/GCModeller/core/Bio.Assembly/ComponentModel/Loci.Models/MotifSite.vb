Namespace ComponentModel.Loci

    Public Interface IMotifSite

        Property Type As String
        Property Name As String
        Property Site As Location
    End Interface

    Public Interface IMotifSiteScore : Inherits IMotifSite

        Property Score As Double
    End Interface
End Namespace