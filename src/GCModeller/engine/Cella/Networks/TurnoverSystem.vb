Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 物质回收的细胞周转系统
''' </summary>
Public Class TurnoverSystem : Inherits SubNetwork

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
