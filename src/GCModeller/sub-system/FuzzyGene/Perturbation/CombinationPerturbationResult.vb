Imports SMRUCC.genomics.BioSystem.Intervention

Namespace Perturbation

    ''' <summary>组合扰动分析结果。</summary>
    Public Class CombinationPerturbationResult

        ''' <summary>联合扰动的标准结果。</summary>
        Public Property Joint As InterventionResult

        ''' <summary>加性期望（对照 + 各单基因扰动效应之和）。</summary>
        Public Property AdditiveMeans As Double()

        ''' <summary>非加性效应（联合结果 − 加性期望）：正为协同、负为拮抗。</summary>
        Public Property NonAdditive As Double()

        ''' <summary>基因名。</summary>
        Public Property GeneNames As String()

        ''' <summary>取非加性效应最强的前 n 个基因。</summary>
        Public Function GetTopSynergy(n As Integer) As List(Of (GeneName As String, Effect As Double))
            Dim result As New List(Of (GeneName As String, Effect As Double))
            Dim order As Integer() = Enumerable.Range(0, NonAdditive.Length) _
                .OrderByDescending(Function(i) System.Math.Abs(NonAdditive(i))) _
                .ToArray()

            For i As Integer = 0 To System.Math.Min(n, order.Length) - 1
                Dim idx As Integer = order(i)
                Call result.Add((GeneName:=GeneNames(idx), Effect:=NonAdditive(idx)))
            Next

            Return result
        End Function

        Public Overrides Function ToString() As String
            Return $"combination: {If(Joint Is Nothing, "?", Joint.Spec.GeneName)}, " &
                   $"top synergy = {String.Join("; ", GetTopSynergy(5).Select(Function(x) $"{x.GeneName}:{x.Effect:G3}"))}"
        End Function

    End Class
End Namespace