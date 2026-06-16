Imports SMRUCC.genomics.Metagenomics

Public Class VirtualCella

    Public Property taxonomy_info As Taxonomy
    Public Property grn As GeneRegulatoryNetwork
    Public Property metabolic As MetabolicNetwork
    Public Property translation As TranslationSystem
    Public Property transportation As TransportSystem
    Public Property turnover As TurnoverSystem

    Sub New()
    End Sub

    Sub RunStep()

    End Sub

End Class
