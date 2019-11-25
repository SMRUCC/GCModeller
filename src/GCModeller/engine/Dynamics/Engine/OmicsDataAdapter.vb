Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine

    Public Class OmicsDataAdapter : Implements IOmicsDataAdapter

        Public ReadOnly Property mass As OmicsTuple(Of String()) Implements IOmicsDataAdapter.mass

        Friend flux As OmicsTuple(Of String())

        Dim saveMass As OmicsTuple(Of DataStorageDriver)
        Dim saveFlux As OmicsTuple(Of DataStorageDriver)

        Sub New(model As CellularModule, mass As OmicsTuple(Of DataStorageDriver), flux As OmicsTuple(Of DataStorageDriver))
            Me.saveMass = mass
            Me.saveFlux = flux
            Me.mass = GetMassTuples(model)
            Me.flux = GetFluxTuples(model)
        End Sub

        Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
            Call saveMass.transcriptome(iteration, data.Subset(mass.transcriptome))
            Call saveMass.proteome(iteration, data.Subset(mass.proteome))
            Call saveMass.metabolome(iteration, data.Subset(mass.metabolome))
        End Sub

        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
            Call saveFlux.transcriptome(iteration, data.Subset(flux.transcriptome))
            Call saveFlux.proteome(iteration, data.Subset(flux.proteome))
            Call saveFlux.metabolome(iteration, data.Subset(flux.metabolome))
        End Sub

        Public Shared Function GetMassTuples(model As CellularModule) As OmicsTuple(Of String())
            Dim RNA = model.Genotype.centralDogmas _
                .Select(Function(gene) gene.RNA.Name) _
                .ToArray
            Dim protein = model.Genotype.centralDogmas _
                .Where(Function(gene) Not gene.IsRNAGene) _
                .Select(Function(gene) gene.polypeptide) _
                .ToArray
            Dim metabolites = model.Phenotype.fluxes _
                .Select(Function(flux)
                            Return flux.products.AsList + flux.substrates
                        End Function) _
                .IteratesALL _
                .Select(Function(mass) mass.text) _
                .Distinct _
                .ToArray

            Return New OmicsTuple(Of String())(RNA, protein, metabolites)
        End Function

        Public Shared Function GetFluxTuples(model As CellularModule) As OmicsTuple(Of String())
            Dim transcription As String() = model.Genotype.centralDogmas _
                .Select(AddressOf Loader.GetTranscriptionId) _
                .ToArray
            Dim translation As String() = model.Genotype.centralDogmas _
                .Where(Function(cd) Not cd.IsRNAGene) _
                .Select(AddressOf Loader.GetTranslationId) _
                .ToArray
            Dim proteinComplex = model.Phenotype.proteins _
                .Select(AddressOf Loader.GetProteinMatureId) _
                .AsList
            Dim metabolism = model.Phenotype.fluxes _
                .Select(Function(r) r.ID) _
                .ToArray

            Return New OmicsTuple(Of String())(
                transcriptome:=transcription,
                proteome:=translation,
                metabolome:=proteinComplex + metabolism
            )
        End Function
    End Class
End Namespace