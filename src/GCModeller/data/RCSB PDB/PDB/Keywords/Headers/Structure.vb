Namespace Keywords

    Public Class Sequence : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SEQRES
            End Get
        End Property

        Dim cache As New List(Of String)

        Friend Shared Function Append(ByRef res As Sequence, str As String) As Sequence
            If res Is Nothing Then
                res = New Sequence
            End If
            res.cache.Add(str)
            Return res
        End Function

    End Class

    Public Class Helix : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_HELIX
            End Get
        End Property
    End Class

    Public Class Sheet : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SHEET
            End Get
        End Property
    End Class

End Namespace