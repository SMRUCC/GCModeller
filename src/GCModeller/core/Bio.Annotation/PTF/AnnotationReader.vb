
Namespace Ptf

    Public NotInheritable Class AnnotationReader

        Private Sub New()
        End Sub

        Public Shared Function KO(protein As ProteinAnnotation) As String
            If protein.attributes.ContainsKey("ko") Then
                Return protein("ko")
            Else
                Return ""
            End If
        End Function
    End Class
End Namespace