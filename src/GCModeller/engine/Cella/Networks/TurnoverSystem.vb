Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

''' <summary>
''' 物质回收的细胞周转系统
''' </summary>
Public Class TurnoverSystem : Inherits MetabolicNetwork

    Sub New(network As IEnumerable(Of Channel), cell As VirtualCella)
        Call MyBase.New(network, cell)
    End Sub


End Class
