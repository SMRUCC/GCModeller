' ============================================================================
' EnsembleTrainingTest.vb
'
' 合成数据自测：二分类 / 三分类 / 回归三个表型，覆盖集成训练、加权、
' 预测字段语义、持久化往返、并行/串行一致性与 SVM 兼容视图。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Models
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Modules

Friend Module EnsembleTrainingTest

    Private Const DIMS As Integer = 20

    Public Sub Run()
        Dim dataset As TraitTrainingSet = BuildSyntheticDataset()

        Call DebugAudit.Mark("synthetic-data",
                             $"{dataset.problems.vectors.Length} samples, {dataset.embedding.GetDimensionCount()} dims, {dataset.traits.Length} traits")

        Dim before As Integer = Assert.Failures

        Call TestTraining(dataset)
        Call TestPrediction(dataset)
        Call TestPersistence(dataset)
        Call TestSvmCompat(dataset)

        Dim delta As Integer = Assert.Failures - before

        Call DebugAudit.Mark("case:ensemble-synthetic", If(delta = 0, "PASS", $"FAIL ({delta} assertion(s))"))
    End Sub

    ''' <summary>训练流程：基学习器数量、权重归一化与集成器创建</summary>
    Private Sub TestTraining(dataset As TraitTrainingSet)
        Dim trainer As New PhenotypeEnsembleTrainer With {
            .Parallel = True,
            .ParallelLearners = False,
            .Folds = 3,
            .MinSamples = 8,
            .AutoWeight = True,
            .Verbose = False,
            .Compact = True
        }
        Dim models As Dictionary(Of String, PhenotypeModel) = trainer.TrainAll(dataset)

        Call Assert.AreEqual(3, models.Count, "训练产出 3 个表型模型")

        Dim binary As PhenotypeModel = models("demo_binary")
        Dim multi As PhenotypeModel = models("demo_multi")
        Dim numeric As PhenotypeModel = models("demo_numeric")

        Call Assert.AreEqual("trained", binary.Status, "二分类模型训练成功")
        Call Assert.AreEqual("trained", multi.Status, "多分类模型训练成功")
        Call Assert.AreEqual("trained", numeric.Status, "回归模型训练成功")

        Call Assert.AreEqual(4, binary.Ensemble.BaseModelCount, "二分类使用 4 个基学习器")
        Call Assert.AreEqual(2, multi.Ensemble.BaseModelCount, "多分类使用 2 个基学习器")
        Call Assert.AreEqual(6, numeric.Ensemble.BaseModelCount, "回归使用 6 个基学习器")

        Call Assert.Test(binary.Ensemble.Classifier IsNot Nothing, "分类集成器已创建")
        Call Assert.Test(numeric.Ensemble.Regressor IsNot Nothing, "回归集成器已创建")
        Call Assert.Test(Math.Abs(binary.Ensemble.Weights.Sum() - 1.0) < 0.000001, "分类权重已归一化")
        Call Assert.Test(Math.Abs(numeric.Ensemble.Weights.Sum() - 1.0) < 0.000001, "回归权重已归一化")
        Call Assert.Test(binary.Ensemble.Members.All(Function(m) m.Weight >= 0), "权重非负")
        Call Assert.Test(binary.Ensemble.Members.All(Function(m) m.CVScore >= 0), "记录交叉验证指标")
        Call Assert.Test(binary.Ensemble.ClassLabels.Length = 2, "二分类类别文本完整")
        Call Assert.Test(multi.Ensemble.ClassLabels.Length = 3, "多分类类别文本完整")
    End Sub

    ''' <summary>预测字段语义与训练集拟合质量</summary>
    Private Sub TestPrediction(dataset As TraitTrainingSet)
        Dim trainer As New PhenotypeEnsembleTrainer With {
            .Parallel = True,
            .Folds = 3,
            .MinSamples = 8,
            .Verbose = False,
            .Compact = True
        }
        Dim models As Dictionary(Of String, PhenotypeModel) = trainer.TrainAll(dataset)
        Dim binary As PhenotypeModel = models("demo_binary")
        Dim correct As Integer = 0

        For Each row As SupportVector In dataset.problems.vectors
            Dim pred As TraitPrediction = binary.Predict(row.Properties, dataset.embedding)

            If String.Equals(pred.predict, row.labels("demo_binary"), StringComparison.OrdinalIgnoreCase) Then
                correct += 1
            End If
        Next

        Dim accuracy As Double = correct / dataset.problems.vectors.Length

        Call Assert.Test(accuracy >= 0.8, $"二分类训练集准确率 {accuracy:F3} >= 0.8")

        Dim sample As SupportVector = dataset.problems.vectors(0)
        Dim prediction As TraitPrediction = binary.Predict(sample.Properties, dataset.embedding)

        Call Assert.Test(prediction.votes IsNot Nothing AndAlso
                             prediction.votes.Length = binary.Ensemble.Classes.Length,
                             "分类输出与类别对齐的概率向量")
        Call Assert.Test(Math.Abs(prediction.votes.Sum() - 1.0) < 0.001, "概率向量已归一化")
        Call Assert.Test(prediction.confidence > 0 AndAlso prediction.confidence <= 1.0, "置信度落在 (0,1]")
        Call Assert.Test(Not String.IsNullOrEmpty(prediction.predict), "预测输出类别标签文本")

        Dim numeric As PhenotypeModel = models("demo_numeric")
        Dim expected As New List(Of Double)()
        Dim predicted As New List(Of Double)()

        For Each row As SupportVector In dataset.problems.vectors
            Dim value As Double = 0

            If Double.TryParse(row.labels("demo_numeric"), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                expected.Add(value)
                predicted.Add(numeric.Predict(row.Properties, dataset.embedding).value)
            End If
        Next

        Dim mean As Double = expected.Average()
        Dim sst As Double = expected.Sum(Function(v) (v - mean) ^ 2)
        Dim sse As Double = expected.Select(Function(v, i) (v - predicted(i)) ^ 2).Sum()
        Dim r2 As Double = If(sst > 0, 1.0 - sse / sst, 0.0)

        Call Assert.Test(r2 > 0.5, $"回归训练集 R² {r2:F3} > 0.5")
        Call Assert.Test(numeric.Predict(sample.Properties, dataset.embedding).confidence = 1.0,
                             "回归预测置信度固定为 1.0")
    End Sub

    ''' <summary>持久化往返与并行/串行预测一致性</summary>
    Private Sub TestPersistence(dataset As TraitTrainingSet)
        Dim trainer As New PhenotypeEnsembleTrainer With {
            .Parallel = True,
            .Folds = 3,
            .MinSamples = 8,
            .Verbose = False,
            .Compact = True
        }
        Dim models As Dictionary(Of String, PhenotypeModel) = trainer.TrainAll(dataset)
        Dim dir As String = Path.Combine(AppContext.BaseDirectory, "synthetic_ensemble")

        If Directory.Exists(dir) Then
            Call Directory.Delete(dir, True)
        End If

        Call ModelLoader.SaveDirectory(models, dataset.embedding, dir, dataset.SampleCount(), verbose:=False)

        Dim loader As ModelLoader = ModelLoader.LoadDirectory(dir, verbose:=False)

        Call Assert.AreEqual(3, loader.Models.Count, "往返之后仍有 3 个表型模型")
        Call Assert.Test(loader.GetModel("demo_binary").Ensemble IsNot Nothing, "集成模型已从 DTO 恢复")
        Call Assert.Test(loader.GetModel("demo_binary").Model Is Nothing, "集成模型不写入 SVM 兼容视图")

        Dim sample As SupportVector = dataset.problems.vectors(0)
        Dim expectedBinary As TraitPrediction = models("demo_binary").Predict(sample.Properties, dataset.embedding)
        Dim actualBinary As TraitPrediction = loader.GetModel("demo_binary").Predict(sample.Properties, dataset.embedding)

        Call Assert.AreEqual(expectedBinary.predict, actualBinary.predict, "分类预测往返一致")

        Dim expectedNumeric As TraitPrediction = models("demo_numeric").Predict(sample.Properties, dataset.embedding)
        Dim actualNumeric As TraitPrediction = loader.GetModel("demo_numeric").Predict(sample.Properties, dataset.embedding)

        Call Assert.Test(Math.Abs(expectedNumeric.value - actualNumeric.value) < 0.001, "回归预测往返一致")

        Dim predictor As New PhenotypePredictor(loader)
        Dim serial As TraitPrediction() = predictor.PredictAll(sample.Properties, parallel:=False)
        Dim parallel As TraitPrediction() = predictor.PredictAll(sample.Properties, parallel:=True)

        Call Assert.AreEqual(serial.Length, parallel.Length, "并行预测结果数量一致")

        For i As Integer = 0 To serial.Length - 1
            Call Assert.AreEqual(serial(i).predict, parallel(i).predict,
                                     $"表型 {serial(i).trait_name} 并行/串行一致")
        Next
    End Sub

    ''' <summary>SVM 单模型兼容路径</summary>
    Private Sub TestSvmCompat(dataset As TraitTrainingSet)
        Dim trainer As New PhenotypeSVMTrainer With {
            .verbose = False,
            .nrfold = 2,
            .autoTune = TuneMode.None
        }
        Dim models As Dictionary(Of String, PhenotypeModel) = trainer.TrainAll(dataset)
        Dim model As PhenotypeModel = models("demo_binary")

        Call Assert.Test(model.Model IsNot Nothing, "SVM 兼容模型已训练")
        Call Assert.Test(model.Ensemble Is Nothing, "SVM 模型不包含集成模型")

        Dim sample As SupportVector = dataset.problems.vectors(0)
        Dim prediction As TraitPrediction = model.Predict(sample.Properties, dataset.embedding)

        Call Assert.Test(prediction IsNot Nothing, "SVM 兼容路径可以预测")
        Call Assert.Test(Not String.IsNullOrEmpty(prediction.predict), "SVM 预测输出类别标签")
    End Sub

    ''' <summary>构造二分类 / 三分类 / 回归三个表型的合成训练集</summary>
    Private Function BuildSyntheticDataset() As TraitTrainingSet
        Dim random As New Random(2026)
        Dim names As String() = Enumerable.Range(1, DIMS).Select(Function(i) $"PF{i:00000}").ToArray()
        Dim vectors As New List(Of SupportVector)()

        ' 二分类
        For i As Integer = 0 To 39
            Dim label As String = If(i Mod 2 = 0, "true", "false")
            Dim profile As New Dictionary(Of String, Double)()

            For j As Integer = 0 To DIMS - 1
                Dim value As Double = random.NextDouble()

                If label = "true" AndAlso j < 4 Then value += 1.5
                If label = "false" AndAlso j >= 4 AndAlso j < 8 Then value += 1.5

                profile(names(j)) = value
            Next

            vectors.Add(New SupportVector With {
                .id = $"binary_{i}",
                .Properties = profile,
                .labels = New Dictionary(Of String, String) From {{"demo_binary", label}}
            })
        Next

        ' 三分类
        For i As Integer = 0 To 59
            Dim cls As Integer = i Mod 3
            Dim profile As New Dictionary(Of String, Double)()

            For j As Integer = 0 To DIMS - 1
                Dim value As Double = random.NextDouble()

                If cls = 0 AndAlso j < 4 Then value += 2.0
                If cls = 1 AndAlso j >= 4 AndAlso j < 8 Then value += 2.0
                If cls = 2 AndAlso j >= 8 AndAlso j < 12 Then value += 2.0

                profile(names(j)) = value
            Next

            vectors.Add(New SupportVector With {
                .id = $"multi_{i}",
                .Properties = profile,
                .labels = New Dictionary(Of String, String) From {{"demo_multi", $"class{cls}"}}
            })
        Next

        ' 回归
        For i As Integer = 0 To 39
            Dim profile As New Dictionary(Of String, Double)()
            Dim y As Double = 0

            For j As Integer = 0 To DIMS - 1
                Dim value As Double = random.NextDouble()

                profile(names(j)) = value

                If j < 5 Then y += value * (j + 1)
            Next

            vectors.Add(New SupportVector With {
                .id = $"numeric_{i}",
                .Properties = profile,
                .labels = New Dictionary(Of String, String) From {
                    {"demo_numeric", y.ToString("F6", CultureInfo.InvariantCulture)}
                }
            })
        Next

        Dim embedding As New PfamEmbedding With {
            .dimensionNames = names,
            .encoding = PfamEncoding.NormalizedCount,
            .minGenomes = 1
        }
        Dim table As New ProblemTable With {
            .dimensionNames = names,
            .vectors = vectors.ToArray()
        }
        Dim traits As PhenotypeTrait() = {
            New PhenotypeTrait With {
                .trait_name = "demo_binary", .data_type = "boolean", .unit = "boolean",
                .group_1_category = "Demo", .group_2_subcategory = "Binary"
            },
            New PhenotypeTrait With {
                .trait_name = "demo_multi", .data_type = "categorical", .unit = "class",
                .group_1_category = "Demo", .group_2_subcategory = "Multi"
            },
            New PhenotypeTrait With {
                .trait_name = "demo_numeric", .data_type = "numeric (continuous)", .unit = "value",
                .group_1_category = "Demo", .group_2_subcategory = "Regression"
            }
        }

        Return New TraitTrainingSet With {
            .embedding = embedding,
            .problems = table,
            .traits = traits
        }
    End Function

End Module
