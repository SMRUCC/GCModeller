Public Interface IGoTerm

    Property Go_ID As String
End Interface

Public Interface IGoTermEnrichment : Inherits IGoTerm

    Property Pvalue As Double
    Property CorrectedPvalue As Double

End Interface