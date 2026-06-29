Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

''' <summary>
''' 跨膜转运系统
''' </summary>
Public Class TransportSystem : Inherits MetabolicNetwork

    Sub New(mass As MassTable, network As IEnumerable(Of Channel), cell As VirtualCella)
        Call MyBase.New(mass, network, cell)
    End Sub

End Class
