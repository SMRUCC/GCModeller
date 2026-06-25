Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB.Models
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
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
    <RApiReturn(GetType(Modules.EnsembleVoting.VotingResult))>
    Public Function make_predicts(models As ModelLoader, genome As GenomeSample) As Object
        Dim voting As New Modules.EnsembleVoting()
        Dim phenotypeModels = models.LoadPhenotypeSVM
        ' 执行预测
        Dim predictions As Modules.EnsembleVoting.VotingResult() = voting.PredictAllPhenotypes(phenotypeModels, genome.PhyleticProfile).ToArray

        Return predictions
    End Function

    <ExportAPI("phenotype_features")>
    Public Function phenotype_features(<RRawVectorArgument> predicts As Object, models As ModelLoader, Optional env As Environment = Nothing)
        Dim featSelector As New Modules.FeatureSelection(models)
        Dim predictions As PipeIterator(Of Modules.EnsembleVoting.VotingResult) = pipeline.Stream(Of Modules.EnsembleVoting.VotingResult)(predicts, env)

        ' 为每个阳性表型提取关键特征
        Dim allKeyFeatures As New Dictionary(Of String, List(Of Modules.FeatureSelection.KeyFeature))

        For Each kvp As Modules.EnsembleVoting.VotingResult In predictions
            If kvp.IsPositive Then
                Dim phenoId As String = kvp.PhenotypeId
                Dim keyFeats As List(Of Modules.FeatureSelection.KeyFeature) = featSelector.LoadKeyFeaturesFromFile(phenoId)
                allKeyFeatures(phenoId) = keyFeats
            End If
        Next

        Return allKeyFeatures
    End Function


End Module
