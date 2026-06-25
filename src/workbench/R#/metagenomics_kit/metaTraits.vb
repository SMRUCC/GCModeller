Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.TraitarVB.Models
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("metaTraits")>
Module metaTraitsTool

    Public Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(ReportJSON()), AddressOf result_table)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Public Function result_table(result As ReportJSON(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result.Select(Function(a) a.phenotypeId).ToArray
        }

        Call df.add(NameOf(ReportJSON.accession), From r As ReportJSON In result Select r.accession)
        Call df.add(NameOf(ReportJSON.category), From r As ReportJSON In result Select r.category)
        Call df.add(NameOf(ReportJSON.predict), From r As ReportJSON In result Select r.predict)
        Call df.add(NameOf(ReportJSON.positive), From r As ReportJSON In result Select r.positive)
        Call df.add(NameOf(ReportJSON.negative), From r As ReportJSON In result Select r.negative)
        Call df.add(NameOf(ReportJSON.confidence), From r As ReportJSON In result Select r.confidence)
        Call df.add(NameOf(ReportJSON.scores), From r As ReportJSON In result Select r.scores.JoinBy("; "))
        Call df.add(NameOf(ReportJSON.labels), From r As ReportJSON In result Select r.labels.JoinBy("; "))
        Call df.add("keys", From r As ReportJSON
                            In result
                            Select r.KeyFeatures _
                                .SafeQuery _
                                .Where(Function(k) k.IsKeyFeature) _
                                .Select(Function(k) k.PfamId) _
                                .JoinBy(", "))
        Return df
    End Function

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

    <ExportAPI("phenotype_result")>
    <RApiReturn(GetType(ReportJSON))>
    Public Function phenotype_features(<RRawVectorArgument> predicts As Object, models As ModelLoader, Optional env As Environment = Nothing)
        Dim featSelector As New Modules.FeatureSelection(models)
        Dim predictions As PipeIterator(Of Modules.EnsembleVoting.VotingResult) = pipeline.Stream(Of Modules.EnsembleVoting.VotingResult)(predicts, env)

        ' 为每个阳性表型提取关键特征
        Dim allKeyFeatures As New Dictionary(Of String, Modules.FeatureSelection.KeyFeature())

        For Each kvp As Modules.EnsembleVoting.VotingResult In predictions
            If kvp.IsPositive Then
                Dim phenoId As String = kvp.PhenotypeId
                Dim keyFeats As Modules.FeatureSelection.KeyFeature() = featSelector.LoadKeyFeaturesFromFile(phenoId).ToArray
                allKeyFeatures(phenoId) = keyFeats
            End If
        Next

        Return predictions.ResultTable(allKeyFeatures, models).ToArray
    End Function


End Module
