
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

''' <summary>
''' 采用ODEs系统表示的代谢网络模型
''' </summary>
Public Class MetabolicNetwork : Inherits SubNetwork

    Protected ReadOnly core As SolverIterator
    Protected ReadOnly massEnv As MassTable
    Protected ReadOnly cellular As Vessel

    Sub New(network As IEnumerable(Of Channel), cell As VirtualCella)
        Call MyBase.New(cell)

        cellular = New Vessel
        cellular.load(network)
    End Sub

    Public Overrides Sub RunStep()
        Call core.Tick()
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Throw New NotImplementedException()
    End Function
End Class
