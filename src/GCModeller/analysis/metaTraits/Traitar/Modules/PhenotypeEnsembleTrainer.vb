' ============================================================================
' PhenotypeEnsembleTrainer.vb
'
' 每个表型训练一个 Voting 集成模型：
'   1. 按数据类型与类别数量选择基学习器（见 EnsembleLearnerFactory）；
'   2. 用 K 折交叉验证估计各基学习器的指标并归一化为投票权重；
'   3. 表型之间可并行、单表型内基学习器也可并行（默认只开表型维，避免线程超订）；
'   4. 单个基学习器失败只记录状态，不中断整个集成模型的训练。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.metaTraits.Traitar.Models

Namespace Traitar.Modules

    ''' <summary>
    ''' 基于 Ensemble/Voting 的表型集成学习训练器
    ''' </summary>
    Public Class PhenotypeEnsembleTrainer

        ''' <summary>是否并行训练不同表型（默认开启）</summary>
        Public Property Parallel As Boolean = True
        ''' <summary>
        ''' 是否并行训练同一个表型内的基学习器。
        ''' 默认关闭：外层已经按表型并行，内层再并行会造成线程超订；
        ''' 只训练单个表型时可以手动打开。
        ''' </summary>
        Public Property ParallelLearners As Boolean = False
        ''' <summary>交叉验证折数（内部会按样本数自适应）</summary>
        Public Property Folds As Integer = 5
        ''' <summary>交叉验证随机种子</summary>
        Public Property Seed As Integer = 42
        ''' <summary>是否按交叉验证指标自动加权</summary>
        Public Property AutoWeight As Boolean = True
        ''' <summary>手动权重；长度与基学习器数量一致时优先使用</summary>
        Public Property ManualWeights As Double()
        ''' <summary>是否输出训练日志</summary>
        Public Property Verbose As Boolean = True
        ''' <summary>单个表型进入训练所需的最小样本数</summary>
        Public Property MinSamples As Integer = 8
        ''' <summary>只训练前 N 个表型；小于等于 0 表示不限制</summary>
        Public Property TopN As Integer = 0
        ''' <summary>表型名过滤器（区分大小写的子串匹配，不区分大小写）</summary>
        Public Property TraitFilter As String
        ''' <summary>精简模式：减少随机森林树数与 XGBoost 轮数，便于小样本快速评估</summary>
        Public Property Compact As Boolean = False

        ''' <summary>
        ''' 训练全部表型，返回「表型名 -&gt; 模型」字典。
        ''' </summary>
        Public Function TrainAll(dataset As TraitTrainingSet) As Dictionary(Of String, Models.PhenotypeModel)
            If dataset Is Nothing Then
                Throw New ArgumentNullException(NameOf(dataset))
            End If

            Dim traits As PhenotypeTrait() = FilterTraits(dataset)
            Dim results As New Dictionary(Of String, Models.PhenotypeModel)()
            Dim models As Models.PhenotypeModel() = New Models.PhenotypeModel(traits.Length - 1) {}

            If Parallel AndAlso traits.Length > 1 Then
                Call System.Threading.Tasks.Parallel.For(0, traits.Length,
                    Sub(i)
                        models(i) = TrainOne(dataset, traits(i))
                    End Sub)
            Else
                For i As Integer = 0 To traits.Length - 1
                    models(i) = TrainOne(dataset, traits(i))
                Next
            End If

            For i As Integer = 0 To traits.Length - 1
                results(traits(i).trait_name) = models(i)
            Next

            Return results
        End Function

        ''' <summary>
        ''' 训练单个表型。样本不足、类型未知或标签异常时返回 <c>skipped</c> 模型，
        ''' 不抛出异常。
        ''' </summary>
        Public Function TrainOne(dataset As TraitTrainingSet, trait As PhenotypeTrait) As Models.PhenotypeModel
            Dim result As New Models.PhenotypeModel With {.Trait = trait}

            If trait Is Nothing OrElse String.IsNullOrWhiteSpace(trait.trait_name) Then
                result.Status = "skipped"
                result.ErrorMessage = "empty trait definition"
                Return result
            End If

            Dim topic As String = trait.trait_name
            Dim rows As SupportVector() = dataset.problems.vectors _
                .Where(Function(v) v IsNot Nothing AndAlso
                                   v.labels IsNot Nothing AndAlso
                                   v.labels.ContainsKey(topic)) _
                .ToArray()

            result.SampleCount = rows.Length

            If trait.GetDataType() = TraitDataType.Unknown Then
                result.Status = "skipped"
                result.ErrorMessage = "unknown trait data type"
                Return result
            End If

            Dim required As Integer = Math.Max(2, MinSamples)

            If rows.Length < required Then
                result.Status = "skipped"
                result.ErrorMessage = $"insufficient samples ({rows.Length} < {required})"
                Return result
            End If

            Dim dims As Integer = dataset.embedding.GetDimensionCount()
            Dim x As Double()() = EnsembleFeatureMatrix.ToMatrix(rows, dataset.embedding.dimensionNames)
            Dim labels As String() = rows.Select(Function(v) v.labels(topic)).ToArray()
            Dim model As Models.PhenotypeModel

            If trait.IsRegression() Then
                model = TrainRegression(result, x, labels, dims)
            Else
                model = TrainClassification(result, x, labels, dims)
            End If

            If Verbose Then
                Console.WriteLine($"  {topic}: {model}")
            End If

            Return model
        End Function

        Private Function TrainClassification(result As Models.PhenotypeModel,
                                             x As Double()(),
                                             labels As String(),
                                             dims As Integer) As Models.PhenotypeModel

            Dim classLabels As String() = labels _
                .Distinct(StringComparer.Ordinal) _
                .OrderBy(Function(s) s, StringComparer.Ordinal) _
                .ToArray()

            If classLabels.Length < 2 Then
                result.Status = "skipped"
                result.ErrorMessage = "only one class label"
                Return result
            End If

            Dim yIndex As Integer() = labels _
                .Select(Function(s) Array.IndexOf(classLabels, s)) _
                .ToArray()
            Dim classes As Double() = Enumerable _
                .Range(0, classLabels.Length) _
                .Select(Function(i) CDbl(i)) _
                .ToArray()
            Dim specs As EnsembleLearnerSpec() = EnsembleLearnerFactory.CreateClassifiers(classLabels.Length, dims, Compact)
            Dim learners As IClassifierLearner() = specs.Select(Function(s) s.Classifier).ToArray()
            Dim weight As EnsembleWeightResult = ResolveWeights(
                learners.Length,
                Function() EnsembleWeighting.EstimateClassifierWeights(learners, x, yIndex, classes, Folds, Seed))
            Dim trained As New List(Of IClassifier)()
            Dim used As New List(Of EnsembleLearnerSpec)()
            Dim scores As New List(Of Double)()

            ' 逐个训练并在成员维隔离失败（例如 MLR 在秩亏矩阵上会抛异常）
            For i As Integer = 0 To learners.Length - 1
                Try
                    Dim model As IClassifier = learners(i).Fit(x, yIndex, classes)

                    trained.Add(model)
                    used.Add(specs(i))
                    scores.Add(If(i < weight.Scores.Length, weight.Scores(i), 0.0R))
                Catch ex As Exception
                    ' 单个基学习器失败只剔除该成员，不影响整个集成模型
                End Try
            Next

            If trained.Count = 0 Then
                result.Status = "skipped"
                result.ErrorMessage = "all base classifiers failed"
                Return result
            End If

            Dim finalWeights As Double() = EnsembleWeighting.CreateResult(scores.ToArray()).Weights
            Dim voting As New VotingClassifier(trained.ToArray(), classes, finalWeights, VotingMode.Soft)
            Dim y As Double() = yIndex.Select(Function(v) CDbl(v)).ToArray()
            Dim members As EnsembleMember() = BuildMembers(used, scores.ToArray(), finalWeights, x, y, EnsembleMemberKind.Classifier)

            result.Ensemble = New PhenotypeEnsemble With {
                .Mode = "voting",
                .Voting = VotingMode.Soft,
                .Classes = voting.Classes,
                .ClassLabels = classLabels,
                .Members = members,
                .Weights = voting.Weights,
                .FeatureCount = dims,
                .Classifier = voting
            }
            result.Status = "trained"
            result.CVScore = AverageScore(members)

            Return result
        End Function

        Private Function TrainRegression(result As Models.PhenotypeModel,
                                         x As Double()(),
                                         labels As String(),
                                         dims As Integer) As Models.PhenotypeModel

            Dim y As Double() = New Double(labels.Length - 1) {}

            For i As Integer = 0 To labels.Length - 1
                Dim value As Double = 0

                If Not Double.TryParse(labels(i), value) Then
                    result.Status = "skipped"
                    result.ErrorMessage = $"invalid numeric label '{labels(i)}'"
                    Return result
                End If

                y(i) = value
            Next

            Dim specs As EnsembleLearnerSpec() = EnsembleLearnerFactory.CreateRegressors(dims, Compact)
            Dim learners As IRegressorLearner() = specs.Select(Function(s) s.Regressor).ToArray()
            Dim weight As EnsembleWeightResult = ResolveWeights(
                learners.Length,
                Function() EnsembleWeighting.EstimateRegressorWeights(learners, x, y, Folds, Seed))
            Dim trained As New List(Of IRegressor)()
            Dim used As New List(Of EnsembleLearnerSpec)()
            Dim scores As New List(Of Double)()

            For i As Integer = 0 To learners.Length - 1
                Try
                    Dim model As IRegressor = learners(i).Fit(x, y)

                    trained.Add(model)
                    used.Add(specs(i))
                    scores.Add(If(i < weight.Scores.Length, weight.Scores(i), 0.0R))
                Catch ex As Exception
                    ' 单个基学习器失败（例如 MLR 秩亏）只剔除该成员
                End Try
            Next

            If trained.Count = 0 Then
                result.Status = "skipped"
                result.ErrorMessage = "all base regressors failed"
                Return result
            End If

            Dim finalWeights As Double() = EnsembleWeighting.CreateResult(scores.ToArray()).Weights
            Dim voting As New VotingRegressor(trained.ToArray(), finalWeights)
            Dim members As EnsembleMember() = BuildMembers(used, scores.ToArray(), finalWeights, x, y, EnsembleMemberKind.Regressor)

            result.Ensemble = New PhenotypeEnsemble With {
                .Mode = "voting",
                .Voting = VotingMode.Soft,
                .Classes = Nothing,
                .ClassLabels = Nothing,
                .Members = members,
                .Weights = voting.Weights,
                .FeatureCount = dims,
                .Regressor = voting
            }
            result.Status = "trained"
            result.CVScore = AverageScore(members)

            Return result
        End Function

        ''' <summary>
        ''' 手动权重优先；否则按自动加权；两者都关闭时退回等权。
        ''' </summary>
        Private Function ResolveWeights(count As Integer,
                                        estimate As Func(Of EnsembleWeightResult)) As EnsembleWeightResult

            If ManualWeights IsNot Nothing AndAlso ManualWeights.Length = count Then
                Return EnsembleWeighting.CreateResult(ManualWeights)
            End If

            If AutoWeight Then
                Return estimate()
            End If

            Return EnsembleWeighting.CreateResult(Enumerable.Repeat(0.0R, count).ToArray())
        End Function

        Private Function BuildMembers(specs As IList(Of EnsembleLearnerSpec),
                                      scores As Double(),
                                      weights As Double(),
                                      x As Double()(),
                                      y As Double(),
                                      kind As EnsembleMemberKind) As EnsembleMember()

            Dim members As EnsembleMember() = New EnsembleMember(specs.Count - 1) {}

            For i As Integer = 0 To specs.Count - 1
                Dim member As New EnsembleMember With {
                    .Algorithm = specs(i).Name,
                    .Kind = kind,
                    .Weight = If(i < weights.Length, weights(i), 0.0R),
                    .CVScore = If(i < scores.Length, scores(i), 0.0R),
                    .Parameters = specs(i).Parameters,
                    .Status = "trained"
                }

                ' 这两个算法的内部状态无法导出，保存训练快照以便加载时重训
                If specs(i).Algorithm = EnsembleAlgorithm.NaiveBayes OrElse
                   specs(i).Algorithm = EnsembleAlgorithm.BayesianRegression Then
                    member.Snapshot = EnsembleTrainingSnapshot.Create(x, y).GetJson
                End If

                members(i) = member
            Next

            Return members
        End Function

        Private Function AverageScore(members As EnsembleMember()) As Double
            If members Is Nothing OrElse members.Length = 0 Then
                Return 0.0R
            End If

            Return members.Average(Function(m) m.CVScore)
        End Function

        ''' <summary>按 TraitFilter 过滤本次需要训练的表型</summary>
        Public Function FilterTraits(traits As PhenotypeTrait()) As PhenotypeTrait()
            Dim query As IEnumerable(Of PhenotypeTrait) = If(traits, New PhenotypeTrait() {})

            If Not String.IsNullOrEmpty(TraitFilter) Then
                query = query.Where(
                    Function(t) t.trait_name IsNot Nothing AndAlso
                                t.trait_name.IndexOf(TraitFilter, StringComparison.OrdinalIgnoreCase) >= 0)
            End If

            Return query.ToArray()
        End Function

        ''' <summary>
        ''' 过滤表型并按样本数降序排列（样本充足的优先训练），再应用 TopN。
        ''' </summary>
        Public Function FilterTraits(dataset As TraitTrainingSet) As PhenotypeTrait()
            Dim traits As PhenotypeTrait() = FilterTraits(dataset.traits)
            Dim counts As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For Each trait As PhenotypeTrait In traits
                counts(trait.trait_name) = dataset.problems.vectors.Count(
                    Function(v) v.labels IsNot Nothing AndAlso v.labels.ContainsKey(trait.trait_name))
            Next

            Dim ordered As IEnumerable(Of PhenotypeTrait) = traits _
                .OrderByDescending(Function(t) If(counts.ContainsKey(t.trait_name), counts(t.trait_name), 0)) _
                .ThenBy(Function(t) t.trait_name, StringComparer.Ordinal)

            If TopN > 0 Then
                ordered = ordered.Take(TopN)
            End If

            Return ordered.ToArray()
        End Function

    End Class
End Namespace
