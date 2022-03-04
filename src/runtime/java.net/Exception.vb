
Namespace Tamir.SharpSsh.java
    ''' <summary>
    ''' Summary description for Exception.
    ''' </summary>
    Public Class Exception
        Inherits System.Exception

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal msg As String)
            MyBase.New(msg)
        End Sub

        Public Overridable Function toString() As String
            Return MyBase.ToString()
        End Function
    End Class
End Namespace
