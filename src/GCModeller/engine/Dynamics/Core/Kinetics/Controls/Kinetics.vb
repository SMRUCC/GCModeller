Namespace Core

    Public Class Kinetics : Inherits Controls

        Public Overrides ReadOnly Property coefficient As Double
            Get
                Return lambda(getMass)
            End Get
        End Property

        ReadOnly env As Vessel
        ReadOnly lambda As Func(Of Func(Of String, Double), Double)
        ReadOnly getMass As Func(Of String, Double)
        ReadOnly raw As Model.Kinetics

        Sub New(env As Vessel, lambda As Model.Kinetics)
            Me.env = env
            Me.lambda = lambda.CompileLambda
            Me.raw = lambda
            Me.getMass = Function(id) env.massIndex(id).Value
        End Sub

        Public Overrides Function ToString() As String
            Return raw.formula.ToString
        End Function
    End Class
End Namespace