Imports System

Namespace Graphics
    Public Class GraphicsDeviceEventArgs
        Inherits EventArgs

        Private ReadOnly contextField As GraphicsContext
        Private ReadOnly descriptionField As DeviceDescription

        Public Sub New(ByVal description As DeviceDescription, ByVal Optional context As GraphicsContext = Nothing)
            descriptionField = description
            contextField = context
        End Sub

        Public ReadOnly Property Description As DeviceDescription
            Get
                Return descriptionField
            End Get
        End Property

        Public ReadOnly Property Context As GraphicsContext
            Get
                Return contextField
            End Get
        End Property
    End Class
End Namespace
