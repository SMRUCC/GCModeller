
''' <summary>
''' A machine learning vector model for motif analysis
''' </summary>
Public Class SequenceGraph

    Public Property Compositions As Dictionary(Of Char, Double)
    Public Property PrefixGraph As Dictionary(Of Char, Dictionary(Of Char, Double))
    Public Property SuffixGraph As Dictionary(Of Char, Dictionary(Of Char, Double))

    Public Function GetVector(components As IReadOnlyCollection(Of Char)) As Double()
        Dim v As New List(Of Double)
        Dim f As Dictionary(Of Char, Double)
        Dim s As Dictionary(Of Char, Double)

        Call v.AddRange(components.Select(Function(ci) Compositions(ci)))

        For Each key As Char In components
            f = PrefixGraph(key)
            s = SuffixGraph(key)

            Call v.AddRange(components.Select(Function(ci) f(ci)))
            Call v.AddRange(components.Select(Function(ci) s(ci)))
        Next

        Return v.ToArray
    End Function

End Class
