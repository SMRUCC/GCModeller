Namespace Assembly.KEGG.DBGET.BriteHEntry.ProteinFamily

    Public NotInheritable Class SignalingAndCellularProcesses

        Private Sub New()
        End Sub

        Public Shared Function Transporters() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02000.keg", "K\d+")
        End Function
    End Class
End Namespace