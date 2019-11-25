Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine.ModelLoader

    Public MustInherit Class FluxLoader

        Public ReadOnly Property MassTable As MassTable
            Get
                Return loader.massTable
            End Get
        End Property

        Protected ReadOnly loader As Loader

        Protected Sub New(loader As Loader)
            Me.loader = loader
        End Sub

        Public MustOverride Function CreateFlux(cell As CellularModule) As IEnumerable(Of Channel)

    End Class
End Namespace