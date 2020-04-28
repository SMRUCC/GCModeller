Imports stdNum = System.Math

Namespace ReactionNetwork

    ''' <summary>
    ''' id1|id2 rxn,....
    ''' </summary>
    Public Class MapCache

        Dim index As Dictionary(Of String, String())

        Sub New()
        End Sub

        Private Shared Function indexKey(c1$, c2$) As String
            Dim i1 = Integer.Parse(c1.Substring(1))
            Dim i2 = Integer.Parse(c2.Substring(1))
            Dim key = stdNum.Min(i1, i2) & "|" & stdNum.Max(i1, i2)

            Return key
        End Function

        Public Shared Function CreateFromTable(rxn As IEnumerable(Of ReactionTable)) As MapCache
            Dim cache As New Dictionary(Of String, List(Of String))
            Dim key As String

            For Each reaction As ReactionTable In rxn
                For Each i In reaction.substrates
                    For Each j In reaction.products
                        key = indexKey(i, j)

                        If Not cache.ContainsKey(key) Then
                            cache.Add(key, New List(Of String))
                        End If

                        cache(key).Add(reaction.entry)
                    Next
                Next
            Next

            Return New MapCache() With {
                .index = cache _
                    .ToDictionary(Function(t) t.Key,
                                  Function(t)
                                      Return t.Value.Distinct.ToArray
                                  End Function)
            }
        End Function

    End Class
End Namespace