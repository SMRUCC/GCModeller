Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class ReactionClassifier

    Dim classes As ReactionClass()
    Dim reactionIndex As Dictionary(Of String, ReactionClass())
    Dim compoundTransformIndex As New Dictionary(Of String, ReactionClass())

    Public Function haveClassification(reaction As Reaction) As Boolean
        Return reactionIndex.ContainsKey(reaction.ID)
    End Function

    Public Iterator Function GetReactantTransform(reaction As Reaction) As IEnumerable(Of (from$, to$))
        Dim classes As ReactionClass() = reactionIndex.TryGetValue(reaction.ID)

        If classes.IsNullOrEmpty Then
            Return
        End If

        Dim model = reaction.ReactionModel

        For Each reactant As String In model.Reactants.Select(Function(r) r.ID)
            For Each product As String In model.Products.Select(Function(r) r.ID)
                If compoundTransformIndex.ContainsKey($"{reactant}_{product}") Then
                    Yield (reactant, product)
                End If
                If compoundTransformIndex.ContainsKey($"{product}_{reactant}") Then
                    Yield (product, reactant)
                End If
            Next
        Next
    End Function

    Private Function buildTupleIndex() As ReactionClassifier
        compoundTransformIndex = classes _
            .Select(Function(cls)
                        Return cls.reactantPairs.Select(Function(tuple) (key:=$"{tuple.from}_{tuple.to}", cls))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(transform) transform.key) _
            .ToDictionary(Function(transform) transform.Key,
                          Function(group)
                              Return group _
                                  .Select(Function(t) t.cls) _
                                  .GroupBy(Function(c) c.entryId) _
                                  .Select(Function(r)
                                              Return r.First
                                          End Function) _
                                  .ToArray
                          End Function)
        Return Me
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
        }.buildIndex _
         .buildTupleIndex
    End Function

End Class
