
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI

''' <summary>
''' A machine learning vector model for motif analysis
''' </summary>
Public Class SequenceGraph : Implements INamedValue

    ''' <summary>
    ''' the unique reference id of current sequence graph model
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String Implements INamedValue.Key
    Public Property composition As Dictionary(Of Char, Double)
    Public Property graph As Dictionary(Of Char, Dictionary(Of Char, Double))
    Public Property triple As Dictionary(Of String, Double)
    Public Property tuple_distance As Dictionary(Of String, Double)

    ''' <summary>
    ''' get vector by default charset in <see cref="composition"/>
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetVector() As Double()
        Return GetVector(components:=composition.Keys)
    End Function

    Public Function GetVector(components As IReadOnlyCollection(Of Char)) As Double()
        Dim v As New List(Of Double)
        Dim g As Dictionary(Of Char, Double)
        Dim tuple_graph As String() = DistanceGraph.GetTuples(components).ToArray

        Call v.AddRange(components.Select(Function(ci) composition(ci)))

        For Each key As Char In components
            g = graph(key)
            v.AddRange(components.Select(Function(ci) g(ci)))
        Next

        For Each t As String In CodonBiasVector.PopulateTriples(components)
            Call v.Add(triple.TryGetValue(t))
        Next

        For Each t1 As String In tuple_graph
            For Each t2 As String In tuple_graph
                If t1 <> t2 Then
                    Call v.Add(tuple_distance.TryGetValue($"{t1}|{t2}"))
                End If
            Next
        Next

        Return v.ToArray
    End Function

End Class
