Imports Tamir.SharpSsh.java.io

Namespace Tamir.Streams
    ''' <summary>
    ''' Summary description for InputStreamWrapper.
    ''' </summary>
    Public Class InputStreamWrapper
        Inherits InputStream

        Private s As Stream

        Public Sub New(ByVal s As Stream)
            Me.s = s
        End Sub

        Public Overrides Function ReadMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return s.Read(buffer, offset, count)
        End Function
    End Class
End Namespace
