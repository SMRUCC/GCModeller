
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace SmithWaterman

    Public Module Workspace

        Public Iterator Function CreateHSP(Of T)(sw As GSW(Of T), cutoff As Double) As IEnumerable(Of LocalHSPMatch(Of T))
            For Each match As Match In sw.Matches(cutoff)
                Yield New LocalHSPMatch(Of T)(match, sw.query, sw.subject, sw.symbol)
            Next
        End Function

        <Extension>
        Public Function GetBestAlignment(Of T)(hsp As IEnumerable(Of LocalHSPMatch(Of T))) As LocalHSPMatch(Of T)
            Dim query As IEnumerable(Of LocalHSPMatch(Of T)) =
                From x As LocalHSPMatch(Of T)
                In hsp _
                    .Select(Function(x) DirectCast(x, Match)) _
                    .DoCall(Function(list)
                                Return SimpleChaining.Chaining(list.ToArray, False)
                            End Function)
                Order By x.score Descending
            Dim lstb = query.ToArray

            If Not lstb.IsNullOrEmpty Then
                Return lstb.FirstOrDefault
            Else
                Return Nothing
            End If
        End Function
    End Module
End Namespace
