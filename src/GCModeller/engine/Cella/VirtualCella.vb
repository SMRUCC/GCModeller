Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
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

    Sub RunStep(iteration As Integer)
        If iteration Mod 3 = 0 Then
            Call grn.RunStep()
        End If
        If iteration Mod 2 = 0 Then
            Call translation.RunStep()
        End If

        Call metabolic.RunStep()
        Call transportation.RunStep()
        Call turnover.RunStep()
    End Sub

    Public Shared Function FromModel(cell As VirtualCell) As VirtualCella
        Dim grn As New List(Of RegulatoryLink)
        Dim cella As New VirtualCella With {
            .taxonomy_info = cell.taxonomy
        }

        cella.grn = New GeneRegulatoryNetwork(cella, grn)

        Return cella
    End Function

End Class
