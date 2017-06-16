Public Interface IGoTerm

    Property Go_ID As String
End Interface

Public Interface IGoTermEnrichment : Inherits IGoTerm

    Property Pvalue As Double
    Property CorrectedPvalue As Double

End Interface

Public Interface IEnrichmentTerm

    Property Term As String
    Property ORF As String()
    Property Pvalue As Double

End Interface