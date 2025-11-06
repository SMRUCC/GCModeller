Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Public Module DataHelper

    Public Function GetTranscriptionId(cd As CentralDogma, cellular_id As String) As String
        Return $"{cd.geneID}@{cellular_id}[Transcription]"
    End Function

    Public Function GetTranslationId(cd As CentralDogma, cellular_id As String) As String
        Return $"{cd.geneID}@{cellular_id}[Translation]"
    End Function

    Public Function GetProteinMatureId(protein As Protein, cellular_id As String) As String
        Return $"{protein.ProteinID}@{cellular_id}[Protein-Mature]"
    End Function

    Public Iterator Function getProteinProcess(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each prot As Protein In model.Phenotype.proteins
                Yield GetProteinMatureId(prot, model.CellularEnvironmentName)
            Next
        Next
    End Function

    Public Iterator Function getTranslation(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each gene As CentralDogma In model.Genotype.centralDogmas
                If gene.RNA.Value = RNATypes.mRNA Then
                    Yield GetTranslationId(gene, model.CellularEnvironmentName)
                End If
            Next
        Next
    End Function

    Public Iterator Function getTranscription(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each gene As CentralDogma In model.Genotype.centralDogmas
                Yield GetTranscriptionId(gene, model.CellularEnvironmentName)
            Next
        Next
    End Function

    Public Iterator Function getFluxIds(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each flux As Reaction In model.Phenotype.fluxes
                Yield flux.ID & "@" & model.CellularEnvironmentName
            Next
        Next
    End Function
End Module
