
Namespace Tamir.SharpSsh.java.util
    ''' <summary>
    ''' Summary description for Hashtable.
    ''' </summary>
    Public Class Hashtable
        Friend h As Global.System.Collections.Hashtable

        Public Sub New()
            h = New Global.System.Collections.Hashtable()
        End Sub

        Public Sub New(ByVal h As Global.System.Collections.Hashtable)
            Me.h = h
        End Sub

        Public Sub put(ByVal key As Object, ByVal item As Object)
            h.Add(key, item)
        End Sub

        Public Function [get](ByVal key As Object) As Object
            Return h(key)
        End Function

        Public Function keys() As Enumeration
            Return New Enumeration(h.Keys.GetEnumerator())
        End Function

        Default Public Property Item(ByVal key As Object) As Object
            Get
                Return [get](key)
            End Get
            Set(ByVal value As Object)
                h(key) = value
            End Set
        End Property
    End Class
End Namespace
