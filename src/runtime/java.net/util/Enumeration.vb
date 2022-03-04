
Namespace Tamir.SharpSsh.java.util
    ''' <summary>
    ''' Summary description for Enumeration.
    ''' </summary>
    Public Class Enumeration
        Private e As Global.System.Collections.IEnumerator
        Private hasMore As Boolean

        Public Sub New(ByVal e As Global.System.Collections.IEnumerator)
            Me.e = e
            hasMore = e.MoveNext()
        End Sub

        Public Function hasMoreElements() As Boolean
            Return hasMore
        End Function

        Public Function nextElement() As Object
            Dim o As Object = e.Current
            hasMore = e.MoveNext()
            Return o
        End Function
    End Class
End Namespace
