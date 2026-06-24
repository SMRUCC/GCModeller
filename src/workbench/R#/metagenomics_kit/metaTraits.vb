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

    ''' <summary>
    ''' 输出结果
    ''' </summary>
    Public Sub OutputResults(
        ByVal outputDir As String,
        ByVal sample As Models.GenomeSample,
        ByVal modelLoader As ModelLoader,
        ByVal predictions As Dictionary(Of String, Modules.EnsembleVoting.VotingResult),
        ByVal allKeyFeatures As Dictionary(Of String, List(Of Modules.FeatureSelection.KeyFeature)),
        ByVal featSelector As Modules.FeatureSelection)

        ' 1. 预测结果摘要
        Dim summaryPath As String = Path.Combine(outputDir, "prediction_summary.txt")
        Using writer As New StreamWriter(summaryPath)
            writer.WriteLine(New String("="c, 70))
            writer.WriteLine("  Traitar VB.NET 表型预测结果")
            writer.WriteLine(New String("="c, 70))
            writer.WriteLine()
            writer.WriteLine("样本ID: " & sample.SampleId)
            writer.WriteLine("Pfam家族数: " & sample.PfamCount)
            writer.WriteLine("检测到的Pfam家族:")
            For Each pid As String In sample.GetPresentPfamIds()
                writer.WriteLine("  " & pid)
            Next
            writer.WriteLine()
            writer.WriteLine("--- 表型预测结果 ---")
            writer.WriteLine(String.Format("{0,-10} {1,-40} {2,-15} {3,-10} {4,-10}",
                                            "表型ID", "表型名称", "类别", "预测", "置信度"))
            writer.WriteLine(New String("-"c, 90))

            Dim positiveCount As Integer = 0
            For Each kvp As KeyValuePair(Of String, Models.PhenotypeModel) In modelLoader.Phenotypes
                Dim phenoId As String = kvp.Key
                Dim phenoModel As Models.PhenotypeModel = kvp.Value

                Dim result As Modules.EnsembleVoting.VotingResult = Nothing
                If predictions.ContainsKey(phenoId) Then
                    result = predictions(phenoId)
                End If

                Dim predStr As String = "N/A"
                Dim confStr As String = "N/A"
                If result IsNot Nothing Then
                    predStr = If(result.IsPositive, "存在(+)", "不存在(-)")
                    confStr = result.Confidence.ToString("F2")
                    If result.IsPositive Then positiveCount += 1
                End If

                writer.WriteLine(String.Format("{0,-10} {1,-40} {2,-15} {3,-10} {4,-10}",
                                                phenoId,
                                                If(phenoModel.PhenotypeName.Length > 40,
                                                   phenoModel.PhenotypeName.Substring(0, 40), phenoModel.PhenotypeName),
                                                phenoModel.Category,
                                                predStr, confStr))
            Next

            writer.WriteLine()
            writer.WriteLine(String.Format("阳性表型数: {0}/{1}", positiveCount, modelLoader.PhenotypeCount))
        End Using
        Console.WriteLine("  预测摘要: " & summaryPath)

        ' 2. 阳性表型详细报告
        Dim detailPath As String = Path.Combine(outputDir, "positive_phenotypes_detail.txt")
        Using writer As New StreamWriter(detailPath)
            writer.WriteLine(New String("="c, 70))
            writer.WriteLine("  阳性表型详细报告")
            writer.WriteLine(New String("="c, 70))
            writer.WriteLine()

            For Each kvp As KeyValuePair(Of String, Modules.EnsembleVoting.VotingResult) In predictions
                If Not kvp.Value.IsPositive Then Continue For

                Dim phenoId As String = kvp.Key
                Dim phenoModel As Models.PhenotypeModel = modelLoader.Phenotypes(phenoId)

                writer.WriteLine(New String("-"c, 35))
                writer.WriteLine("表型ID: " & phenoId)
                writer.WriteLine("表型名称: " & phenoModel.PhenotypeName)
                writer.WriteLine("表型类别: " & phenoModel.Category)
                writer.WriteLine("预测结果: 存在(POSITIVE)")
                writer.WriteLine("投票统计: 正票=" & kvp.Value.PositiveVotes &
                                 ", 负票=" & kvp.Value.NegativeVotes &
                                 ", 总票数=" & kvp.Value.TotalVotes)
                writer.WriteLine("置信度: " & kvp.Value.Confidence.ToString("F4"))
                writer.WriteLine()

                ' 关键特征
                If allKeyFeatures.ContainsKey(phenoId) Then
                    writer.WriteLine("关键蛋白质家族特征:")
                    writer.WriteLine(featSelector.GenerateReport(allKeyFeatures(phenoId), 10))
                End If

                writer.WriteLine()
            Next
        End Using
        Console.WriteLine("  阳性详情: " & detailPath)

        ' 3. 特征向量
        Dim featurePath As String = Path.Combine(outputDir, "phyletic_profile.txt")
        Using writer As New StreamWriter(featurePath)
            writer.WriteLine("样本ID: " & sample.SampleId)
            writer.WriteLine("Pfam家族数: " & sample.PfamCount)
            writer.WriteLine()
            writer.WriteLine("存在的Pfam家族:")
            For Each pid As String In sample.GetPresentPfamIds()
                Dim desc As String = ""
                If modelLoader.PfamDescriptions.ContainsKey(pid) Then
                    desc = modelLoader.PfamDescriptions(pid)
                End If
                writer.WriteLine(pid & ControlChars.Tab & desc)
            Next
        End Using
        Console.WriteLine("  特征向量: " & featurePath)

    End Sub
End Module
