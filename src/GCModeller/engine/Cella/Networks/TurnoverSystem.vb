Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 物质回收的细胞周转系统
''' </summary>
Public Class TurnoverSystem : Inherits SubNetwork

    ReadOnly core As SolverIterator

    Public Overrides Sub RunStep(cell As VirtualCella)
        Call core.Tick()
    End Sub
End Class
