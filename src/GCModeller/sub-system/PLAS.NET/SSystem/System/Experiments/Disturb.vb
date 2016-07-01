Imports System.Xml.Serialization

Namespace Kernel.ObjectModels

    Public Class Disturb

        Dim Target As Var
        Dim Kernel As Kernel
        Dim model As Experiment
        Dim nextTime As Double

        ''' <summary>
        ''' 周期性的实验之中所剩余的刺激
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LeftKicks As Integer
        Public ReadOnly Property IsReady As Boolean
            Get
                Return model.Start <= Kernel.RuntimeTicks
            End Get
        End Property

        ReadOnly __disturb As Func(Of Double, Double, Double)

        Sub New(model As Experiment)
            Me.model = model
            LeftKicks = model.Kicks
            __disturb = Methods(model.DisturbType)
        End Sub

        Public Sub Tick()
            If Kernel.RuntimeTicks > nextTime Then
                Target.Value = __disturb(Target.Value, model.Value)
                nextTime = model.Interval + Kernel.RuntimeTicks
                _LeftKicks -= 1
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return model.ToString
        End Function

        Public Sub [Set](Kernel As Kernel)
            Me.Kernel = Kernel
            Me.Target = Kernel.GetValue(model.Id)
        End Sub
    End Class
End Namespace