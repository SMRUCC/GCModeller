Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

''' <summary>
''' 物质回收的细胞周转系统
''' </summary>
Public Class TurnoverSystem : Inherits MetabolicNetwork

    Sub New(mass As MassTable, network As IEnumerable(Of Channel), cell As VirtualCella)
        Call MyBase.New(mass, network, cell)
    End Sub


End Class
