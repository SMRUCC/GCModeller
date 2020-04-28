Imports System.Text
Imports stdNum = System.Math

Namespace ReactionNetwork

    ''' <summary>
    ''' id1|id2 rxn,....
    ''' </summary>
    Public Class MapCache

        Dim index As Dictionary(Of String, String())

        Sub New()
        End Sub

        Public Function FindPoints(c1$, c2$) As String()
            Dim key = indexKey(c1, c2)

            If index.ContainsKey(key) Then
                Return index(key)
            Else
                Return {}
            End If
        End Function

        Public Function FindAllPoints(compounds As IEnumerable(Of String)) As String()
            Dim list = compounds.ToArray
            Dim result As New List(Of String)

            For Each a In list
                For Each b In list
                    result.AddRange(FindPoints(a, b))
                Next
            Next

            Return result.Distinct.ToArray
        End Function

        Private Shared Function indexKey(c1$, c2$) As String
            Dim i1 = Integer.Parse(c1.Substring(1))
            Dim i2 = Integer.Parse(c2.Substring(1))
            Dim key = stdNum.Min(i1, i2) & "|" & stdNum.Max(i1, i2)

            Return key
        End Function

        Public Function Text() As String
            Dim sb As New StringBuilder

            For Each index As KeyValuePair(Of String, String()) In Me.index
                sb.AppendLine(index.Key & " " & index.Value.JoinBy(","))
            Next

            Return sb.ToString
        End Function

        Public Shared Function ParseText(text As String()) As MapCache
            Dim cache As New Dictionary(Of String, String())
            Dim values As String()
            Dim key As String

            For Each line As String In text
                values = line.Split
                key = values(Scan0)
                values = values.Split(","c)

                Call cache.Add(key, values)
            Next

            Return New MapCache With {.index = cache}
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