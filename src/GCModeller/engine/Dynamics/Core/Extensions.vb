Imports System.Runtime.CompilerServices

Namespace Core

    <HideModuleName>
    Public Module Extensions

        Friend Function ToString(reaction As Channel) As String
            Dim left = reaction.left.Select(AddressOf MassToString).JoinBy(" + ")
            Dim right = reaction.right.Select(AddressOf MassToString).JoinBy(" + ")
            Dim direct$ = If(reaction.direct = Directions.forward, "=>", "<=")

            Return $"{left} {direct} {right}"
        End Function

        <Extension>
        Private Function MassToString(var As Variable) As String
            If var.IsTemplate Then
                Return $"[{var.Coefficient} {var.Mass.ID}]"
            Else
                Return $"{var.Coefficient} {var.Mass.ID}"
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetReactants(r As Channel) As IEnumerable(Of Variable)
            Return r.left.AsEnumerable
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetProducts(r As Channel) As IEnumerable(Of Variable)
            Return r.right.AsEnumerable
        End Function
    End Module
End Namespace