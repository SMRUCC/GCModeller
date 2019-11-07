Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class ReactionClassifier

    Dim classes As ReactionClass()
    Dim reactionIndex As Dictionary(Of String, ReactionClass())

    Public Function haveClassification(reaction As Reaction) As Boolean
        Return reactionIndex.ContainsKey(reaction.ID)
    End Function

    Public Iterator Function GetReactantTransform(reaction As Reaction) As IEnumerable(Of (from$, to$))
        Dim classes As ReactionClass() = reactionIndex(reaction.ID)

        For Each [class] As ReactionClass In classes

        Next
    End Function

    Private Function buildIndex() As ReactionClassifier
        reactionIndex = classes _
            .Select(Function(cls)
                        Return cls.reactions.Keys.Select(Function(rid) (rid, cls))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(r) r.rid) _
            .ToDictionary(Function(rid) rid.Key,
                          Function(group)
                              Return group _
                                  .Select(Function(r) r.cls) _
                                  .GroupBy(Function(r) r.entryId) _
                                  .Select(Function(g)
                                              Return g.First
                                          End Function) _
                                  .ToArray
                          End Function)
        Return Me
    End Function

    Public Shared Function FromRepository(directory As String) As ReactionClassifier
        Return New ReactionClassifier With {
            .classes = ReactionClass.ScanRepository(directory).ToArray
        }.buildIndex
    End Function

End Class
