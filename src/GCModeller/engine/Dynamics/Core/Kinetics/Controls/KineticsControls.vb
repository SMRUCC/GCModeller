Namespace Core

    Public Class KineticsControls : Inherits Controls

        ''' <summary>
        ''' 计算出当前的调控效应单位
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property coefficient As Double
            Get
                If lambda Is Nothing AndAlso inhibition.IsNullOrEmpty Then
                    Return baseline
                End If

                Dim i = inhibition.Sum(Function(v) v.coefficient * v.mass.Value)
                Dim a = lambda(getMass)

                ' 抑制的总量已经大于等于激活的总量的时候，返回零值，
                ' 则反应过程可能不会发生
                Return Math.Max((a + baseline) - i, 0)
            End Get
        End Property

        ReadOnly lambda As Func(Of Func(Of String, Double), Double)
        ReadOnly getMass As Func(Of String, Double)
        ReadOnly raw As Model.Kinetics

        Sub New(env As Vessel, lambda As Model.Kinetics)
            Me.lambda = lambda.CompileLambda
            Me.raw = lambda
            Me.getMass = Function(id)
                             Return env.m_massIndex(id).Value
                         End Function
        End Sub

        Public Overrides Function ToString() As String
            Return raw.formula.ToString
        End Function
    End Class
End Namespace