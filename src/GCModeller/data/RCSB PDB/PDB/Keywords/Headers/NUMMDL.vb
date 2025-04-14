Namespace Keywords

    Public Class NUMMDL : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "NUMMDL"
            End Get
        End Property

        Public Property NUMMDL As Integer

        Friend Shared Function Parse(ByRef NUMMDL As NUMMDL, str As String) As NUMMDL
            If NUMMDL Is Nothing Then
                NUMMDL = New NUMMDL
            End If
            NUMMDL.NUMMDL = CInt(Val(str))
            Return NUMMDL
        End Function

        Public Overrides Function ToString() As String
            Return $"#{NUMMDL} NUMMDL"
        End Function

    End Class
End Namespace