
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 采用ODEs系统表示的代谢网络模型
''' </summary>
Public Class MetabolicNetwork : Inherits SubNetwork

    ReadOnly core As SolverIterator

    Public Overrides Sub RunStep(cell As VirtualCella)
        Throw New NotImplementedException()
    End Sub
End Class
