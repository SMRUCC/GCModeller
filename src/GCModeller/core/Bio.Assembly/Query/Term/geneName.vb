Namespace ObjectQuery

    Public Module geneName

        Const geneNamePattern As String = "[a-z][a-z0-9]{2,6}"

        Public Function TryParseGeneName(product As String) As String
            If product.TextEquals("conserved hypothetical protein") Then
                Return ""
            End If

            If product.IsPattern(geneNamePattern & " (protein|transposase)") Then
                Return product.Split.First
            ElseIf product.IsPattern(geneNamePattern) Then
                Return product
            ElseIf product.IsPattern(".+ protein " & geneNamePattern) Then
                Return product.Split.Last
            Else
                Return ""
            End If
        End Function
    End Module
End Namespace