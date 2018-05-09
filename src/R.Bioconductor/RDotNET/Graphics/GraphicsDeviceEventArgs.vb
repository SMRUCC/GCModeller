Namespace Graphics
    Public Class GraphicsDeviceEventArgs
        Inherits EventArgs

        Public ReadOnly Property Description() As DeviceDescription
        Public ReadOnly Property Context() As GraphicsContext

        Public Sub New(description As DeviceDescription, Optional context As GraphicsContext = Nothing)
            Me.Description = description
            Me.Context = context
        End Sub

    End Class
End Namespace