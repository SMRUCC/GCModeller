Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine

    Public Class OmicsTuple(Of T)

        Public ReadOnly Property transcriptome As T
        Public ReadOnly Property proteome As T
        Public ReadOnly Property metabolome As T

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

        End Sub

        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double))

        End Sub
    End Class
End Namespace