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
    Public Property reaction_type As ValueData()
    Public Property localization As ValueData()
    Public Property natural_substrates_products As ValueData()
    Public Property substrates_products As ValueData()
    Public Property turnover_number As ValueData()
    Public Property km_value As ValueData()
    Public Property ph_optimum As ValueData()
    Public Property ph_range As ValueData()
    Public Property specific_activity As ValueData()
    Public Property temperature_optimum As ValueData()
    Public Property temperature_range As ValueData()
    Public Property cofactor As ValueData()
    Public Property activating_compound As ValueData()
    Public Property inhibitor As ValueData()
    Public Property metals_ions As ValueData()
    Public Property molecular_weight As ValueData()
    Public Property posttranslational_modification As ValueData()
    Public Property subunits As ValueData()
    Public Property application As ValueData()
    Public Property protein_variants As ValueData()
    Public Property cloned As ValueData()
    Public Property crystallization As ValueData()
    Public Property purification As ValueData()
    Public Property renatured As ValueData()
    Public Property general_stability As ValueData()
    Public Property temperature_stability As ValueData()
    Public Property reference As Dictionary(Of String, ReferenceData)
    Public Property ki_value As ValueData()
    Public Property expression As ValueData()
    Public Property general_information As ValueData()


End Class

Public Class ReferenceData

    Public Property id As String
    Public Property title As String
    Public Property authors As String()
    Public Property journal As String
    Public Property year As String
    Public Property pages As String
    Public Property vol As String
    Public Property pmid As String

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
