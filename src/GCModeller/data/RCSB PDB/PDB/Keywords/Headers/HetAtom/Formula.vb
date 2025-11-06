Namespace Keywords

    Public Class Formula : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_FORMUL
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef formula As Formula, line As String) As Formula
            If formula Is Nothing Then
                formula = New Formula
            End If
            formula.str.Append(line)
            Return formula
        End Function

    End Class
End Namespace