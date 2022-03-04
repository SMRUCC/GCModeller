
Namespace Tamir.SharpSsh.java.util
    ''' <summary>
    ''' Summary description for Vector.
    ''' </summary>
    Public Class Vector
        Inherits Global.System.Collections.ArrayList

        Public Function size() As Integer
            Return Me.Count
        End Function

        Public Sub addElement(ByVal o As Object)
            Add(o)
        End Sub

        Public Sub add(ByVal o As Object)
            addElement(o)
        End Sub

        Public Sub removeElement(ByVal o As Object)
            Remove(o)
        End Sub

        Public Function remove(ByVal o As Object) As Boolean
            Remove(o)
            Return True
        End Function

        Public Function elementAt(ByVal i As Integer) As Object
            Return Me(i)
        End Function

        Public Function [get](ByVal i As Integer) As Object
            Return elementAt(i)
        End Function

        Public Sub clear()
            Clear()
        End Sub

        Public Function toString() As String
            Return MyBase.ToString()
        End Function
    End Class
End Namespace
