Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB.Models
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB.Modules
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("metaTraits")>
Module metaTraitsTool

    <ExportAPI("load.meta_traits")>
    <RApiReturn(GetType(metaTraitData))>
    Public Function load_metatraits(file As String, Optional env As Environment = Nothing) As Object
        Return TraitAnnotation.CreateProfiles(TraitAnnotation.ParseTable(file)).ToArray
    End Function

    <ExportAPI("load.trait_models")>
    Public Function load_traitModels(repo As String) As ModelLoader
        Dim modelLoader As ModelLoader = New ModelLoader(repo).LoadAll()
        Console.WriteLine("表型数: " & modelLoader.PhenotypeCount)
        Return modelLoader
    End Function

    <ExportAPI("make_predicts")>
    Public Function make_predicts(models As ModelLoader, genome As GenomeSample) As Object
        Dim voting As New Modules.EnsembleVoting()
        Dim phenotypeModels = models.LoadPhenotypeSVM
        ' 执行预测
        Dim predictions As Dictionary(Of String, Modules.EnsembleVoting.VotingResult) = voting.PredictAllPhenotypes(phenotypeModels, genome.PhyleticProfile)

        Return predictions
    End Function
End Module
