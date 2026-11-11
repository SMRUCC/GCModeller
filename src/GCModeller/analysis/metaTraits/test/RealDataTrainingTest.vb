' ============================================================================
' RealDataTrainingTest.vb
'
' 真实数据训练测试：
'   Pfam 注释      K:/cultral_medium/pfam/<Species>/Pfam.csv
'   表型标注       K:/cultral_medium/ncbi_species_summary_filtered.tsv
'   表型定义       G:/GCModeller/data/phenotype_traits_and_types.csv
'
' 33 个基因组属于小样本场景，测试只验证流程正确与结构合法，不做高精度断言。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Models
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Modules

Friend Module RealDataTrainingTest

    Private Const PFAM_DIR As String = "K:\cultral_medium\pfam"
    Private Const TRAIT_TSV As String = "K:\cultral_medium\ncbi_species_summary_filtered.tsv"
    Private Const TRAIT_TABLE As String = "G:\GCModeller\data\phenotype_traits_and_types.csv"

    Public Sub Run()
        If Not Directory.Exists(PFAM_DIR) OrElse
           Not File.Exists(TRAIT_TSV) OrElse
           Not File.Exists(TRAIT_TABLE) Then
            Call DebugAudit.Mark("case:ensemble-real-data", "SKIPPED (data files not found)")
            Return
        End If

        Dim before As Integer = Assert.Failures
        Dim dataset As TraitTrainingSet

        Try
            dataset = PhenotypeDatasetBuilder.FromFiles(PFAM_DIR, TRAIT_TABLE, TRAIT_TSV)
        Catch ex As Exception
            Call Assert.Test(False, $"failed to build dataset: {ex.Message}")
            Call DebugAudit.Mark("case:ensemble-real-data", "FAIL (dataset)")
            Return
        End Try

        Call DebugAudit.Mark("real-data",
                             $"{dataset.problems.vectors.Length} genomes, {dataset.traits.Length} traits, {dataset.embedding.GetDimensionCount()} pfam dims")

        Call Assert.Test(dataset.problems.vectors.Length > 0, "数据集包含有效基因组")
        Call Assert.Test(dataset.traits.Length > 0, "数据集包含表型定义")

        Dim trainer As New PhenotypeEnsembleTrainer With {
            .Parallel = True,
            .ParallelLearners = False,
            .Folds = 3,
            .MinSamples = 8,
            .TopN = 6,
            .AutoWeight = True,
            .Verbose = False,
            .Compact = True
        }
        Dim models As Dictionary(Of String, PhenotypeModel) = trainer.TrainAll(dataset)
        Dim trained As Integer = models.Values.Where(Function(m) m.IsTrained()).Count()

        Call Assert.Test(models.Count > 0, "至少选择一个表型进行训练")
        Call Assert.Test(trained > 0, "至少一个集成模型训练成功")

        Dim dir As String = Path.Combine(AppContext.BaseDirectory, "real_data_models")

        If Directory.Exists(dir) Then
            Call Directory.Delete(dir, True)
        End If

        Call ModelLoader.SaveDirectory(models, dataset.embedding, dir, dataset.SampleCount(), verbose:=False)

        Dim loader As ModelLoader = ModelLoader.LoadDirectory(dir, verbose:=False)

        Call Assert.AreEqual(models.Count, loader.Models.Count, "模型仓库往返数量一致")
        Call Assert.AreEqual(trained, loader.Models.Values.Where(Function(m) m.IsTrained()).Count(),
                                 "训练成功的模型往返之后仍可预测")

        Dim sample As SupportVector = dataset.problems.vectors(0)
        Dim predictions As TraitPrediction() = New PhenotypePredictor(loader).PredictAll(sample.Properties)

        Call Assert.AreEqual(models.Count, predictions.Length, "预测结果覆盖全部表型")
        Call Assert.Test(predictions.Count(Function(p) Not String.IsNullOrEmpty(p.predict)) >= trained,
                             "训练成功的模型都给出了预测值")

        Call DebugAudit.Mark("real-data-summary", $"trained={trained}/{models.Count}, models={dir}")

        Dim delta As Integer = Assert.Failures - before

        Call DebugAudit.Mark("case:ensemble-real-data", If(delta = 0, "PASS", $"FAIL ({delta} assertion(s))"))
    End Sub

End Module
