Imports System.Diagnostics
Imports System.IO
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits.Traitar
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits.Traitar.Models
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits.Traitar.Modules
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString

Module metaTraitsTest

    ''' <summary>表型定义表</summary>
    Const TRAIT_TABLE As String = "G:/GCModeller/src/GCModeller/analysis/Metagenome/MetaFunction/metaTraits/phenotype_traits_and_types.csv"
    ''' <summary>训练用的表型标注数据（GTDB + NCBI）</summary>
    Const GTDB_TRAITS As String = "C:\Users\Administrator\Downloads\gtdb_species_summary_filtered.tsv"
    Const NCBI_TRAITS As String = "C:\Users\Administrator\Downloads\ncbi_species_summary_filtered.tsv"
    ''' <summary>各微生物基因组所预测的蛋白质组 Pfam 结构域组成</summary>
    Const PFAM_DIR As String = "C:\Users\Administrator\Downloads\pfam"
    ''' <summary>训练得到的表型模型仓库的输出目录</summary>
    Const MODEL_DIR As String = "C:\Users\Administrator\Downloads\traitar_svm"

    Sub Main()
        Call trainingTest()
        Call predicttest()
        Call tuningTest()
    End Sub

    Sub readFiles()
        Dim annos = TraitAnnotation.ParseTable("C:\Users\Administrator\Downloads\ncbi_species_summary_no_predictions.tsv").ToArray
        Dim microbials = TraitAnnotation.CreateProfiles(annos).OrderByDescending(Function(a) a.traits.Length).ToArray

        Call microbials.First.GetJson.SaveTo("Z:/metaTrait.json")

        Pause()
    End Sub

    ''' <summary>
    ''' 在非交互式的控制台环境之下 Pause() 会抛出异常，这里做一层保护
    ''' </summary>
    Private Sub Wait()
        Try
            Call Pause()
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' 读取各微生物基因组蛋白质组的 Pfam 结构域组成
    ''' </summary>
    Private Function LoadPfamSets() As Dictionary(Of String, PfamString())
        Return PFAM_DIR _
            .ListDirectory _
            .ToDictionary(Function(d) d.BaseName,
                          Function(d)
                              Return $"{d}/Pfam.csv".LoadCsv(Of PfamString)(mute:=True).ToArray
                          End Function)
    End Function

    ''' <summary>
    ''' 装配训练数据集：解析表型定义表、表型标注数据与 Pfam 蛋白质组注释，
    ''' 并做 per-genome 归一化的 Pfam 嵌入
    ''' </summary>
    Private Function LoadDataSet() As TraitTrainingSet
        Dim traits As PhenotypeTrait() = PhenotypeTraits.LoadTable(TRAIT_TABLE)
        Dim trainingSet As TraitAnnotation() = TraitAnnotation _
            .ParseTable(GTDB_TRAITS) _
            .JoinIterates(TraitAnnotation.ParseTable(NCBI_TRAITS)) _
            .ToArray
        Dim pfams As Dictionary(Of String, PfamString()) = LoadPfamSets()

        Console.WriteLine($"load {traits.Length} phenotype traits, {trainingSet.Length} annotations, {pfams.Count} genomes")
        Console.WriteLine("build pfam vocabulary...")

        ' 以该基因组内的最大出现次数做归一化，捕捉结构域的重复（拷贝数）信息
        Dim embedding As PfamEmbedding = TraitProblemBuilder _
            .CreateEmbedding(pfams, minGenomes:=1, encoding:=PfamEncoding.NormalizedCount)

        Console.WriteLine($"  {embedding}")

        Dim dataset As TraitTrainingSet = TraitProblemBuilder.Build(trainingSet, pfams, traits, embedding)

        Console.WriteLine($"  {dataset}")

        Return dataset
    End Function

    ''' <summary>
    ''' 表型 SVM 模型训练：
    ''' Pfam 结构域命中计数 -> per-genome 归一化嵌入 -> 逐表型训练 LibSVM 模型
    ''' </summary>
    Sub trainingTest()
        Dim clock As New Stopwatch
        Dim dataset As TraitTrainingSet = LoadDataSet()

        Console.WriteLine("training svm models...")

        Dim trainer As New PhenotypeSVMTrainer With {
            .verbose = True,
            .nrfold = 5
        }

        Call clock.Start()

        Dim models As Dictionary(Of String, PhenotypeModel) = trainer.TrainAll(dataset)
        Dim loader As ModelLoader = ModelLoader.SaveDirectory(models, dataset.embedding, MODEL_DIR, dataset.SampleCount())

        Call clock.Stop()

        Dim trained As Integer = loader.TrainedCount
        Dim skipped As Integer = loader.PhenotypeCount - trained

        Console.WriteLine($"training finish in {clock.Elapsed.TotalSeconds.ToString("F1")}s: {trained} trained, {skipped} skipped")
        Console.WriteLine($"model repository: {MODEL_DIR}")

        Call Wait()
    End Sub

    ''' <summary>
    ''' 表型预测测试：加载模型仓库，对样本基因组做全表型预测并输出报告
    ''' </summary>
    Sub predicttest()
        Dim genome As String = "Carnobacterium_divergens"
        Dim loader As ModelLoader = ModelLoader.LoadDirectory(MODEL_DIR)
        Dim predictor As New PhenotypePredictor(loader)

        Console.WriteLine($"loaded {loader}")

        Dim proteins As PfamString() = $"{PFAM_DIR}/{genome}/Pfam.csv".LoadCsv(Of PfamString)(mute:=True).ToArray
        Dim profile As Dictionary(Of String, Double) = loader.Embedding.EmbedProteins(proteins)
        Dim predictions As TraitPrediction() = predictor.PredictSorted(profile)
        Dim reports As ReportJSON() = predictions _
            .ResultTable(loader) _
            .ToArray
        Dim file As String = Path.Combine(MODEL_DIR, $"{genome}.phenotypes.json")

        Call System.IO.File.WriteAllText(file, reports.GetJson)

        Console.WriteLine($"predict {reports.Length} phenotypes of {genome}")
        Console.WriteLine($"report saved to: {file}")
        Console.WriteLine()

        ' 输出置信度最高的若干个布尔型表型预测
        Dim booleans As TraitPrediction() = predictions _
            .Where(Function(p) p IsNot Nothing AndAlso
                       p.data_type IsNot Nothing AndAlso
                       p.data_type.Equals("boolean", StringComparison.OrdinalIgnoreCase) AndAlso
                       p.status = "trained") _
            .Take(20) _
            .ToArray

        Console.WriteLine("[top boolean traits]")

        For Each pred As TraitPrediction In booleans
            Console.WriteLine($"  {pred.trait_name,-45} = {pred.predict,-6} (confidence {pred.confidence.ToString("F4")})")
        Next

        ' 输出数值型（回归）表型的预测结果
        Dim numerics As TraitPrediction() = predictions _
            .Where(Function(p) p IsNot Nothing AndAlso p.IsRegression() AndAlso p.status = "trained") _
            .ToArray

        Console.WriteLine($"[numeric traits: {numerics.Length}]")

        For Each pred As TraitPrediction In numerics
            Console.WriteLine($"  {pred.trait_name,-45} = {pred.predict}{pred.unit}")
        Next

        Call Wait()
    End Sub

    ''' <summary>
    ''' 超参数网格搜索测试：针对交叉验证得分偏低的数值型（回归）表型，
    ''' 在 C 与 gamma 之上做 2 的幂网格搜索，对比调优前后的交叉验证得分
    ''' </summary>
    Sub tuningTest()
        Dim dataset As TraitTrainingSet = LoadDataSet()
        Dim loader As ModelLoader = ModelLoader.LoadDirectory(MODEL_DIR)
        Dim clock As New Stopwatch
        Dim targets As PhenotypeModel() = loader _
            .GetTrainedModels() _
            .Where(Function(m) m.IsRegression() AndAlso m.CVScore < 0.5) _
            .ToArray

        Console.WriteLine($"tune {targets.Length} numeric traits (cv < 0.5)...")

        Call clock.Start()

        For Each model As PhenotypeModel In targets
            Dim tuned As ParameterSearchResult = ParameterSearch.SearchTrait(
                dataset,
                model.Trait,
                kernel:=Microsoft.VisualBasic.MachineLearning.SVM.KernelType.RBF,
                nrfold:=5,
                minC:=-5, maxC:=7, stepC:=4,
                minG:=-11, maxG:=1, stepG:=4)

            If tuned Is Nothing Then
                Continue For
            End If

            Console.WriteLine($"  {model.Trait.trait_name,-45} cv {model.CVScore.ToString("F4")} -> {tuned.Score.ToString("F4")}, {tuned}")
        Next

        Call clock.Stop()

        Console.WriteLine($"tuning finish in {clock.Elapsed.TotalSeconds.ToString("F1")}s")

        Call Wait()
    End Sub
End Module
