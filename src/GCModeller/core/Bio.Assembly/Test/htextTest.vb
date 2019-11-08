Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery

Module htextTest

    Sub Main()
        Dim RC = ReactionClass.LoadFromResource.ToArray

        Call ReactionClassWebQuery.DownloadReactionClass("D:\biodeep\biodeep_v2\data\KEGG\reaction_class")

        Pause()
    End Sub
End Module
