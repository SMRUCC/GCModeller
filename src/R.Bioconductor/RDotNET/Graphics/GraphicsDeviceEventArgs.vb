
Namespace Graphics

    Public Class GraphicsDeviceEventArgs
        Inherits EventArgs
        Private ReadOnly m_context As GraphicsContext
        Private ReadOnly m_description As DeviceDescription

        Public Sub New(description As DeviceDescription, Optional context As GraphicsContext = Nothing)
            Me.m_description = description
            Me.m_context = context
        End Sub

        Public ReadOnly Property Description() As DeviceDescription
            Get
                Return Me.m_description
            End Get
        End Property

        Public ReadOnly Property Context() As GraphicsContext
            Get
                Return Me.m_context
            End Get
        End Property
    End Class
End Namespace