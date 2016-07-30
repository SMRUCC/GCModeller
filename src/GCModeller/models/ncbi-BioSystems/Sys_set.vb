''' <summary>
''' 
''' </summary>
Public Class Sys_set
    Public Property sysid As sysid
    Public Property source As source
    Public Property externalaccn As String
    Public Property recordurl As String
    Public Property names As String()
    Public Property description As String

End Class

Public Class sysid
    Public Property bsid As String
    Public Property version As String
End Class

Public Class source

    Public Property source As sourceInner

    Public Class sourceInner
        Public Property db As String
        Public Property tag As tag
    End Class
End Class

Public Class tag
    Public Property id As String
End Class