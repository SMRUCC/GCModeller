
Namespace Tamir.SharpSsh.java.util
    ''' <summary>
    ''' Summary description for JavaString.
    ''' </summary>
    Public Class JavaString
        Inherits [String]

        Public Sub New(ByVal s As String)
            MyBase.New(s)
        End Sub

        Public Sub New(ByVal o As Object)
            MyBase.New(o)
        End Sub

        Public Sub New(ByVal arr As Byte())
            MyBase.New(arr)
        End Sub

        Public Sub New(ByVal arr As Byte(), ByVal offset As Integer, ByVal len As Integer)
            MyBase.New(arr, offset, len)
        End Sub
    End Class
End Namespace
