
Namespace Tamir.SharpSsh.java
    ''' <summary>
    ''' Summary description for Platform.
    ''' </summary>
    Public Class Platform
        Public Shared ReadOnly Property Windows As Boolean
            Get
                Return System.Environment.OSVersion.Platform.ToString().StartsWith("Win")
            End Get
        End Property
    End Class
End Namespace
