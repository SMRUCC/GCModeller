Namespace Core

    Public Class BaselineControls : Inherits Controls

        Public Overrides ReadOnly Property coefficient As Double
            Get
                Return baseline
            End Get
        End Property

        Sub New(baseline As Double)
            Me.baseline = baseline
        End Sub

        Sub New()
        End Sub
    End Class
End Namespace