Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class ReactionClassifier

    Dim classes As ReactionClass()
    Dim reactionIndex As Dictionary(Of String, ReactionClass)

    Private Function buildIndex() As ReactionClassifier

    End Function

    Public Shared Function FromRepository(directory As String) As ReactionClassifier
        Return New ReactionClassifier With {
            .classes = ReactionClass.ScanRepository(directory).ToArray
        }.buildIndex
    End Function

End Class
