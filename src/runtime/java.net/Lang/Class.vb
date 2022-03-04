
Namespace Tamir.SharpSsh.java.lang
    ''' <summary>
    ''' Summary description for Class.
    ''' </summary>
    Public Class [Class]
        Private t As Global.System.Type

        Private Sub New(ByVal t As Global.System.Type)
            Me.t = t
        End Sub

        Private Sub New(ByVal typeName As String)
            Me.New(Global.System.Type.GetType(typeName))
        End Sub

        Public Shared Function forName(ByVal name As String) As [Class]
            Return New [Class](name)
        End Function

        Public Function newInstance() As Object
            Return Global.System.Activator.CreateInstance(t)
        End Function
    End Class
End Namespace
