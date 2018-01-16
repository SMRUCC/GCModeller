Module Module1

    Sub Main()
        Dim nnn = EID2.Interaction.BuildSharedPathogens(EID2.Interaction.Load("D:\OneDrive\2018-1-15\associations.csv", cargo:="vVirusNameCorrected", carrier:="hHostNameFinal"))

        Call nnn.Save("D:\OneDrive\2018-1-15\net")

        Pause()
    End Sub

End Module
