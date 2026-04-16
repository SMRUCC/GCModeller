Imports Oracle.Java.Tamir.SharpSsh.java.io

Public Class GZIPOutputStream
    Private fileOutputStream As FileOutputStream

    Public Sub New(fileOutputStream As FileOutputStream)
        Me.fileOutputStream = fileOutputStream
    End Sub

    Public Sub write(header() As SByte, v As Integer, length As Integer)
        Throw New NotImplementedException()
    End Sub

    Public Sub close()
        Throw New NotImplementedException()
    End Sub
End Class
