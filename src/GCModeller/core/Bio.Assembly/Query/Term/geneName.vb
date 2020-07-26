Namespace ObjectQuery

    Public Module geneName

        Public Function TryParseGeneName(product As String) As String
            If product.TextEquals("conserved hypothetical protein") Then
                Return ""
            End If

            If product.IsPattern("[a-z][a-z0-9]{2,6} protein") Then
                Return product.Split.First
            Else
                Return ""
            End If
        End Function
    End Module
End Namespace