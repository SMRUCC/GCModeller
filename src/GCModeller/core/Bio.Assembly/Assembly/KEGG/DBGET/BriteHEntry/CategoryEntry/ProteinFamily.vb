Namespace Assembly.KEGG.DBGET.BriteHEntry.ProteinFamily

    Public NotInheritable Class SignalingAndCellularProcesses

        Private Sub New()
        End Sub

        Public Shared Function Transporters() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02000.keg", "K\d+")
        End Function

        Public Shared Function SecretionSystem() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02044.keg", "K\d+")
        End Function

        Public Shared Function BacterialToxins() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02042.keg", "K\d+")
        End Function

        Public Shared Function Exosome() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko04147.keg", "K\d+")
        End Function

        Public Shared Function ProkaryoticDefenseSystem() As BriteTerm()
            Return BriteTerm.GetInformation("https://www.kegg.jp/kegg-bin/get_htext?ko02048.keg", "K\d+")
        End Function
    End Class
End Namespace