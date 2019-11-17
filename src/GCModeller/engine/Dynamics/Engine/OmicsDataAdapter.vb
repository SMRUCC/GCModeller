Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine

    Public Class OmicsTuple(Of T)

        Public ReadOnly transcriptome As T
        Public ReadOnly proteome As T
        Public ReadOnly metabolome As T

        Sub New(transcriptome As T, proteome As T, metabolome As T)
            Me.transcriptome = transcriptome
            Me.proteome = proteome
            Me.metabolome = metabolome
        End Sub
    End Class

    Public Class OmicsDataAdapter

        Dim mass As OmicsTuple(Of String())
        Dim flux As OmicsTuple(Of String())

        Dim saveMass As OmicsTuple(Of DataStorageDriver)
        Dim saveFlux As OmicsTuple(Of DataStorageDriver)

        Sub New(model As CellularModule, mass As OmicsTuple(Of DataStorageDriver), flux As OmicsTuple(Of DataStorageDriver))
            saveMass = mass
            saveFlux = flux
        End Sub

        Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double))
            Call saveMass.transcriptome(iteration, data.Subset(mass.transcriptome))
            Call saveMass.proteome(iteration, data.Subset(mass.proteome))
            Call saveMass.metabolome(iteration, data.Subset(mass.metabolome))
        End Sub

        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double))
            Call saveFlux.transcriptome(iteration, data.Subset(flux.transcriptome))
            Call saveFlux.proteome(iteration, data.Subset(flux.proteome))
            Call saveFlux.metabolome(iteration, data.Subset(flux.metabolome))
        End Sub
    End Class
End Namespace