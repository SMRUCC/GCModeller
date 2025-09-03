Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    Public Class MassFactor : Implements IEnumerable(Of Factor)

        Public Property compartments As Dictionary(Of String, Factor)

        Public ReadOnly Property id As String

        Default Public ReadOnly Property getFactor(compart_id As String) As Factor
            Get
                Return compartments.TryGetValue(compart_id)
            End Get
        End Property

        Public ReadOnly Property size As Integer
            Get
                Return compartments.Where(Function(c) c.Value > 0).Count
            End Get
        End Property

        Sub New(id As String, list As IEnumerable(Of Factor))
            Me.id = id
            Me.compartments = list.ToDictionary(Function(c) c.cellular_compartment)
        End Sub

        Public Sub reset(value As Double)
            For Each factor As Factor In compartments.Values
                Call factor.reset(value)
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub reset(compart_id As String, value As Double)
            Call compartments(compart_id).reset(value)
        End Sub

        Public Overrides Function ToString() As String
            Dim locs As String() = compartments _
                .Where(Function(c) c.Value > 0) _
                .Select(Function(c) c.Key) _
                .ToArray

            Return $"{id}@{locs.JoinBy(", ")}"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Factor) Implements IEnumerable(Of Factor).GetEnumerator
            For Each factor As Factor In compartments.Values
                Yield factor
            Next
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class
End Namespace