Public Class SequenceGraph

    Public Property Compositions As Dictionary(Of Char, Double)
    Public Property PrefixGraph As Dictionary(Of Char, Dictionary(Of Char, Double))
    Public Property SuffixGraph As Dictionary(Of Char, Dictionary(Of Char, Double))

    Public Function GetVector(components As String) As Double()

    End Function

End Class
