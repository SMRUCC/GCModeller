
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI

''' <summary>
''' A machine learning vector model for motif analysis
''' </summary>
Public Class SequenceGraph

    Public Property composition As Dictionary(Of Char, Double)
    Public Property graph As Dictionary(Of Char, Dictionary(Of Char, Double))
    Public Property triple As Dictionary(Of String, Double)

    Public Function GetVector(components As IReadOnlyCollection(Of Char)) As Double()
        Dim v As New List(Of Double)
        Dim g As Dictionary(Of Char, Double)

        Call v.AddRange(components.Select(Function(ci) composition(ci)))

        For Each key As Char In components
            g = graph(key)
            v.AddRange(components.Select(Function(ci) g(ci)))
        Next

        For Each t As String In CodonBiasVector.PopulateTriples(components)
            Call v.Add(triple.TryGetValue(t))
        Next

        Return v.ToArray
    End Function

End Class
