Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Module KEGGenzymeTest

    Sub Main()

        Dim tree As htext = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.GetResource
        Dim entries = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.ParseEntries

        Pause()
    End Sub
End Module
