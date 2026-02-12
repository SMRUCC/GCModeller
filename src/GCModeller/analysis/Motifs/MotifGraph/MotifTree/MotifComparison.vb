Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MotifComparison : Inherits ComparisonProvider

    ReadOnly motifs As New Dictionary(Of String, Probability)

    Public Sub New(motifs As IEnumerable(Of Probability), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        For Each motif As Probability In motifs.SafeQuery
            Call Me.motifs.Add(motif.name, motif)
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return SmithWatermanAlignment(motifs(x), motifs(y))
    End Function

    Private Function SmithWatermanAlignment(pwm1 As Probability, pwm2 As Probability) As Double

    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetObject(id As String) As Object
        Return motifs.TryGetValue(id)
    End Function
End Class
