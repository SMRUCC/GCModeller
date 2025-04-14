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

        Dim strs As New List(Of String)

        Friend Shared Function Append(ByRef helix As Helix, str As String) As Helix
            If helix Is Nothing Then
                helix = New Helix
            End If
            helix.strs.Add(str)
            Return helix
        End Function
    End Class

    Public Class Sheet : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SHEET
            End Get
        End Property

        Dim strs As New List(Of String)

        Friend Shared Function Append(ByRef sheet As Sheet, str As String) As Sheet
            If sheet Is Nothing Then
                sheet = New Sheet
            End If
            sheet.strs.Add(str)
            Return sheet
        End Function
    End Class

End Namespace