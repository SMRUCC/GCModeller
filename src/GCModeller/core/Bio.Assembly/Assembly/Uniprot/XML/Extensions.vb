Imports System.Runtime.CompilerServices

Namespace Assembly.Uniprot.XML

    Public Module Extensions

        <Extension> Public Function proteinFullName(protein As entry) As String
            If protein.protein Is Nothing OrElse
                protein.protein.recommendedName Is Nothing OrElse
                protein.protein.recommendedName.fullName Is Nothing Then
                Return ""
            Else
                Return protein.protein.recommendedName.fullName.value
            End If
        End Function
    End Module
End Namespace