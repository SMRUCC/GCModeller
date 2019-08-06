Namespace Serialization.BinaryDumping

    ''' <summary>
    ''' A Common framework for visit a object
    ''' </summary>
    Public Class ObjectVisitor

        ReadOnly type As Type

        Sub New(type As Type)
            Me.type = type
        End Sub
    End Class
End Namespace