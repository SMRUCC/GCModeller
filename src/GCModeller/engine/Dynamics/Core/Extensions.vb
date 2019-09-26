Imports System.Runtime.CompilerServices

Namespace Core

    <HideModuleName>
    Public Module Extensions

        Friend Function ToString(reaction As Channel) As String
            Dim left = reaction.left.Select(AddressOf MassToString).JoinBy(" + ")
            Dim right = reaction.right.Select(AddressOf MassToString).JoinBy(" + ")
            Dim direct$ = If(reaction.Direction = Directions.forward, "=>", "<=")

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
    End Module
End Namespace