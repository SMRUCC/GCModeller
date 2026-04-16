
Namespace Tamir.SharpSsh.java.lang
    ''' <summary>
    ''' Summary description for StringBuffer.
    ''' </summary>
    Public Class StringBuffer
        Private sb As Global.System.Text.StringBuilder

        Public Sub New()
            sb = New Global.System.Text.StringBuilder()
        End Sub

        Public Sub New(ByVal s As String)
            sb = New Global.System.Text.StringBuilder(s)
        End Sub

        Public Sub New(ByVal sb As Global.System.Text.StringBuilder)
            Me.New(sb.ToString())
        End Sub

        Public Sub New(ByVal s As [String])
            Me.New(s.ToStringMethod())
        End Sub

        Public Function append(ByVal s As String) As StringBuffer
            sb.Append(s)
            Return Me
        End Function

        Public Function append(ByVal s As Char) As StringBuffer
            sb.Append(s)
            Return Me
        End Function

        Public Function append(ByVal s As [String]) As StringBuffer
            Return append(s.ToStringMethod())
        End Function

        Public Function delete(ByVal start As Integer, ByVal [end] As Integer) As StringBuffer
            sb.Remove(start, [end] - start)
            Return Me
        End Function

        Public Function ToStringMethod() As String
            Return sb.ToString()
        End Function
    End Class
End Namespace
