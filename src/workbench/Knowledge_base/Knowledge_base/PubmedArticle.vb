Public Class PubmedArticle
    Public Property MedlineCitation As MedlineCitation
    Public Property PubmedData As PubmedData
End Class

Public Class MedlineCitation
    Public Property Status As String
    Public Property Owner As String
    Public Property PMID As PMID
    Public Property Article As Article
    Public Property MedlineJournalInfo As MedlineJournalInfo
    Public Property ChemicalList As Chemical()
    Public Property CitationSubset As String
End Class

Public Class PubmedData

End Class