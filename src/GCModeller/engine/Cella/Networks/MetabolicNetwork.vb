
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 采用ODEs系统表示的代谢网络模型
''' </summary>
Public Class MetabolicNetwork : Inherits SubNetwork

    ReadOnly core As SolverIterator

    Sub New(cell As VirtualCella)
        Call MyBase.New(cell)
    End Sub

    Public Overrides Sub RunStep()
        Call core.Tick()
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Throw New NotImplementedException()
    End Function
End Class
