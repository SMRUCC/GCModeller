
''' <summary>
''' A machine learning vector model for motif analysis
''' </summary>
Public Class SequenceGraph

    Public Property Compositions As Dictionary(Of Char, Double)
    Public Property Graph As Dictionary(Of Char, Dictionary(Of Char, Double))
    Public Property Triple As Dictionary(Of String, Double)

    Public Function GetVector(components As IReadOnlyCollection(Of Char)) As Double()
        Dim v As New List(Of Double)
        Dim g As Dictionary(Of Char, Double)

        Call v.AddRange(components.Select(Function(ci) Compositions(ci)))

        For Each key As Char In components
            g = Graph(key)
            v.AddRange(components.Select(Function(ci) g(ci)))
        Next

        For Each i As Char In components
            For Each j As Char In components
                For Each k As Char In components
                    Call v.Add(Triple.TryGetValue(New String({i, j, k})))
                Next
            Next
        Next

        Return v.ToArray
    End Function

End Class
