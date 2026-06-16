
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 跨膜转运系统
''' </summary>
Public Class TransportSystem : Inherits SubNetwork

    ReadOnly core As SolverIterator

    Public Overrides Sub RunStep(cell As VirtualCella)
        Throw New NotImplementedException()
    End Sub
End Class
