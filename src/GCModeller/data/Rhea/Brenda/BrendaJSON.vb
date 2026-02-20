Public Class BrendaJSON

    Public Property release As String
    Public Property version As String
    Public Property data As Dictionary(Of String, BrendaEnzymeData)

End Class

Public Class BrendaEnzymeData

    Public Property id As String
    Public Property recommended_name As String
    Public Property systematic_name As String
    Public Property reaction As ValueData()
    Public Property synonyms As ValueData()
    Public Property protein As Dictionary(Of String, ProteinData)

End Class

Public Class ProteinData

    Public Property id As String
    Public Property organism As String
    Public Property references As String()
    Public Property comment As String
    Public Property source As String
    Public Property accessions As String()


End Class

Public Class ValueData

    Public Property value As String
    Public Property proteins As String()
    Public Property references As String()
    Public Property comment As String

End Class
