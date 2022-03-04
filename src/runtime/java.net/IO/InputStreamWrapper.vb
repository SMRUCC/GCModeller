Imports Oracle.Java.Tamir.SharpSsh.java.io

Namespace Tamir.Streams
    ''' <summary>
    ''' Summary description for InputStreamWrapper.
    ''' </summary>
    Public Class InputStreamWrapper
        Inherits InputStream

        Private s As Global.System.IO.Stream

        Public Sub New(ByVal s As Global.System.IO.Stream)
            Me.s = s
        End Sub

        Public Overrides Sub Flush()
            Call s.Flush()
        End Sub

        Public Overrides Sub Write(buffer() As System.Byte, offset As System.Int32, count As System.Int32)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function Read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return s.Read(buffer, offset, count)
        End Function

        Public Overrides Function Read(buffer() As System.Byte, offset As System.Int32, count As System.Int32) As System.Int32
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
