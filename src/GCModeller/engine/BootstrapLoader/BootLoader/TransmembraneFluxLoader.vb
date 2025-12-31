Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace ModelLoader

    Public Class TransmembraneFluxLoader : Inherits FluxLoader

        ReadOnly culturalMedium As String

        Public Sub New(loader As Loader)
            MyBase.New(loader)
            Me.culturalMedium = loader.define.CultureMedium
        End Sub

        Protected Overrides Iterator Function CreateFlux() As IEnumerable(Of Channel)
            Dim cellular_id As String = cell.CellularEnvironmentName

            If loader.define.transmembrane Is Nothing Then
                Return
            End If

            For Each id As String In loader.define.transmembrane.passive.SafeQuery
                Dim left = MassTable.variable(id, cellular_id)
                Dim right = MassTable.variable(id, culturalMedium)
                Dim flux As New Channel(left, right) With {
                    .bounds = New Boundary(10, 10),
                    .forward = Controls.StaticControl(10),
                    .reverse = Controls.StaticControl(10),
                    .ID = $"[passive] transmembrane transportation of {id} from cell {cellular_id} to {culturalMedium}",
                    .name = .ID
                }

                Call loader.fluxIndex(MetabolismNetworkLoader.MembraneTransporter).Add(flux.ID)

                Yield flux
            Next
        End Function

        Protected Overrides Function GetMassSet() As IEnumerable(Of String)
            Return New String() {}
        End Function
    End Class
End Namespace