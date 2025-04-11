Namespace Keywords

    ''' <summary>
    ''' Sequence Advisory
    ''' </summary>
    Public Class SEQADV : Inherits Keyword

        Public Property Text As New List(Of String)

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SEQADV"
            End Get
        End Property

        Friend Shared Function Append(ByRef seq As SEQADV, line As String) As SEQADV
            If seq Is Nothing Then
                seq = New SEQADV
            End If
            seq.Text.Add(line)
            Return seq
        End Function

    End Class
End Namespace