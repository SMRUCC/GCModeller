
''' <summary>
''' A machine learning vector model for motif analysis
''' </summary>
Public Class SequenceGraph

    Public Property Compositions As Dictionary(Of Char, Double)
    Public Property Graph As Dictionary(Of Char, Dictionary(Of Char, Double))

    Public Function GetVector(components As IReadOnlyCollection(Of Char)) As Double()
        Dim v As New List(Of Double)
        Dim g As Dictionary(Of Char, Double)

        Call v.AddRange(components.Select(Function(ci) Compositions(ci)))

        For Each key As Char In components
            g = Graph(key)
            v.AddRange(components.Select(Function(ci) g(ci)))
        Next

        Return v.ToArray
    End Function

End Class
