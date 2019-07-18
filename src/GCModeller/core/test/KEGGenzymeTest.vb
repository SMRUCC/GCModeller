Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Module KEGGenzymeTest

    Sub Main()

        Dim kgml = "D:\GCModeller\src\GCModeller\core\data\ko02060.xml".LoadXml(Of KGML.pathway)

        Dim kolist = kgml.KOlist



        Pause()

        Dim tree As htext = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.GetResource
        Dim entries = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.EnzymeEntry.ParseEntries

        Pause()
    End Sub
End Module
