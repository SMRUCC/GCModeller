Public Interface IGoTerm

    Property Go_ID As String
End Interface

Public Interface IGoTermEnrichment : Inherits IGoTerm

    Property Pvalue As Double
    Property CorrectedPvalue As Double

End Interface

Public Interface IKEGGTerm

    Property ID As String
    Property Term As String
    Property ORF As String()
    Property Pvalue As Double

    ''' <summary>
    ''' KEGG link
    ''' </summary>
    ''' <returns></returns>
    Property Link As String

End Interface