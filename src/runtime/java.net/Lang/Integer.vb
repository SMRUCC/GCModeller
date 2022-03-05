
Namespace Tamir.SharpSsh.java.lang
    ''' <summary>
    ''' Summary description for Integer.
    ''' </summary>
    Public Class [Integer]
        Private i As Integer

        Public Sub New(ByVal i As Integer)
            Me.i = i
        End Sub

        Public Function intValue() As Integer
            Return i
        End Function

        Public Shared Function parseInt(ByVal s As String) As Integer
            Return Integer.Parse(s)
        End Function
    End Class
End Namespace
