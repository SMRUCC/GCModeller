' ============================================================================
' EnsembleModelDTO.vb
'
' 集成模型的持久化中转层。Ensemble 的 Voting 模型本身不可直接 JSON 序列化，
' 因此这里逐基模型导出算法专用载荷（Payload），并附带权重、类别顺序等元数据；
' 加载时再按算法分派重建各基模型，最后用 VotingClassifier/VotingRegressor
' 的「已训练模型」构造重载恢复集成器。
'
' 无法导出内部状态的算法（朴素贝叶斯、贝叶斯回归）改为保存训练快照（Snapshot），
' 加载时用快照重训该成员，保证加载后的预测与训练时一致。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble
Imports Microsoft.VisualBasic.MachineLearning.RandomForests
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure
Imports Microsoft.VisualBasic.MachineLearning.XGBoost.train
Imports Microsoft.VisualBasic.Serialization.JSON
Imports BayesRegNS = Microsoft.VisualBasic.MachineLearning.BayesianRegression
Imports LogisticNS = Microsoft.VisualBasic.Data.Bootstrapping.Logistic
Imports MultivariateNS = Microsoft.VisualBasic.Data.Bootstrapping.Multivariate
Imports RidgeNS = Microsoft.VisualBasic.Data.Bootstrapping.Ridge
Imports ScaledSvm = SMRUCC.genomics.Analysis.metaTraits.Traitar.Modules

Namespace Traitar.Models

    ''' <summary>随机森林树节点（<see cref="Branch"/> 的字段映射，用于 JSON 持久化）</summary>
    Public Class RandomForestBranchDTO

        Public Property mean As Double
        Public Property mean_snp As Double
        Public Property class_val As Integer
        Public Property status As String
        Public Property Feature As Integer
        Public Property Child1 As Integer
        Public Property Child2 As Integer
        Public Property Parent As Integer
        Public Property list As Integer()

    End Class

    ''' <summary>随机森林的可持久化结构</summary>
    Public Class EnsembleForestDTO

        Public Property MaxTree As Integer
        Public Property MaxBranch As Integer
        Public Property Mtry As Integer
        Public Property LossFunction As String
        Public Property Trees As RandomForestBranchDTO()()

    End Class

    ''' <summary>
    ''' 训练快照：稀疏存储的特征矩阵与标签，仅用于无法导出内部状态的算法。
    ''' </summary>
    Public Class EnsembleTrainingSnapshot

        Public Property Rows As Integer
        Public Property Columns As Integer
        ''' <summary>每行的非零列索引（0-based）</summary>
        Public Property Indices As Integer()()
        ''' <summary>每行的非零值，与 <see cref="Indices"/> 对齐</summary>
        Public Property Values As Double()()
        ''' <summary>分类为 0..K-1 的类别索引，回归为真实目标值</summary>
        Public Property Labels As Double()

        Public Shared Function Create(x As Double()(), labels As Double()) As EnsembleTrainingSnapshot
            Dim indices As Integer()() = New Integer(x.Length - 1)() {}
            Dim values As Double()() = New Double(x.Length - 1)() {}

            For i As Integer = 0 To x.Length - 1
                Dim cols As New List(Of Integer)()
                Dim vals As New List(Of Double)()

                For j As Integer = 0 To x(i).Length - 1
                    If x(i)(j) <> 0.0R Then
                        cols.Add(j)
                        vals.Add(x(i)(j))
                    End If
                Next

                indices(i) = cols.ToArray()
                values(i) = vals.ToArray()
            Next

            Return New EnsembleTrainingSnapshot With {
                .Rows = x.Length,
                .Columns = If(x.Length > 0, x(0).Length, 0),
                .Indices = indices,
                .Values = values,
                .Labels = labels
            }
        End Function

        Public Function ToMatrix() As Double()()
            Dim matrix As Double()() = New Double(Rows - 1)() {}

            For i As Integer = 0 To Rows - 1
                Dim row As Double() = New Double(Columns - 1) {}
                Dim index As Integer() = If(Indices(i), New Integer() {})
                Dim value As Double() = If(Values(i), New Double() {})

                For k As Integer = 0 To Math.Min(index.Length, value.Length) - 1
                    If index(k) >= 0 AndAlso index(k) < Columns Then
                        row(index(k)) = value(k)
                    End If
                Next

                matrix(i) = row
            Next

            Return matrix
        End Function

    End Class

    ''' <summary>集成模型的持久化 DTO</summary>
    Public Class EnsembleModelDTO

        Public Property Mode As String
        Public Property Voting As VotingMode
        Public Property Classes As Double()
        Public Property ClassLabels As String()
        Public Property FeatureCount As Integer
        Public Property Weights As Double()
        Public Property Members As EnsembleMember()

        ''' <summary>从运行时集成模型导出 DTO（逐基模型写入 Payload / Snapshot）</summary>
        Public Shared Function CreateDTO(ensemble As PhenotypeEnsemble) As EnsembleModelDTO
            If ensemble Is Nothing OrElse ensemble.Members Is Nothing Then
                Return Nothing
            End If

            Dim votingClassifier As VotingClassifier = TryCast(ensemble.Classifier, VotingClassifier)
            Dim votingRegressor As VotingRegressor = TryCast(ensemble.Regressor, VotingRegressor)
            Dim members As EnsembleMember() = New EnsembleMember(ensemble.Members.Length - 1) {}

            For i As Integer = 0 To ensemble.Members.Length - 1
                Dim item As EnsembleMember = CopyMember(ensemble.Members(i))
                item.Payload = ExportPayload(votingClassifier, votingRegressor, i, item)
                members(i) = item
            Next

            Return New EnsembleModelDTO With {
                .Mode = ensemble.Mode,
                .Voting = ensemble.Voting,
                .Classes = ensemble.Classes,
                .ClassLabels = ensemble.ClassLabels,
                .FeatureCount = ensemble.FeatureCount,
                .Weights = ensemble.Weights,
                .Members = members
            }
        End Function

        ''' <summary>按 DTO 重建运行时集成模型</summary>
        Public Function CreateEnsemble() As PhenotypeEnsemble
            Dim classifiers As New List(Of IClassifier)()
            Dim regressors As New List(Of IRegressor)()
            Dim weights As New List(Of Double)()

            If Members IsNot Nothing Then
                For i As Integer = 0 To Members.Length - 1
                    Dim item As EnsembleMember = Members(i)
                    Dim model As Object

                    If item.Kind = EnsembleMemberKind.Classifier Then
                        model = RestoreClassifier(item)
                    Else
                        model = RestoreRegressor(item)
                    End If

                    If model IsNot Nothing Then
                        If item.Kind = EnsembleMemberKind.Classifier Then
                            classifiers.Add(DirectCast(model, IClassifier))
                        Else
                            regressors.Add(DirectCast(model, IRegressor))
                        End If

                        weights.Add(If(Weights IsNot Nothing AndAlso i < Weights.Length, Weights(i), 0.0R))
                    End If
                Next
            End If

            Dim normalized As Double() = Normalize(weights)
            Dim ensemble As New PhenotypeEnsemble With {
                .Mode = Mode,
                .Voting = Voting,
                .Classes = Classes,
                .ClassLabels = ClassLabels,
                .Members = Members,
                .Weights = normalized,
                .FeatureCount = FeatureCount
            }

            If classifiers.Count > 0 Then
                ensemble.Classifier = New VotingClassifier(classifiers.ToArray(), Classes, normalized, Voting)
            ElseIf regressors.Count > 0 Then
                ensemble.Regressor = New VotingRegressor(regressors.ToArray(), normalized)
            End If

            Return ensemble
        End Function

        Private Shared Function CopyMember(source As EnsembleMember) As EnsembleMember
            Dim parameters As New Dictionary(Of String, Double)()

            If source.Parameters IsNot Nothing Then
                For Each kvp As KeyValuePair(Of String, Double) In source.Parameters
                    parameters(kvp.Key) = kvp.Value
                Next
            End If

            Return New EnsembleMember With {
                .Algorithm = source.Algorithm,
                .Kind = source.Kind,
                .Weight = source.Weight,
                .CVScore = source.CVScore,
                .Status = source.Status,
                .ErrorMessage = source.ErrorMessage,
                .Parameters = parameters,
                .Payload = source.Payload,
                .Snapshot = source.Snapshot
            }
        End Function

        Private Shared Function ExportPayload(classifier As VotingClassifier,
                                              regressor As VotingRegressor,
                                              index As Integer,
                                              member As EnsembleMember) As String

            If classifier IsNot Nothing AndAlso index < classifier.BaseModels.Length Then
                Return ExportClassifier(classifier.BaseModels(index), member)
            End If

            If regressor IsNot Nothing AndAlso index < regressor.BaseModels.Length Then
                Return ExportRegressor(regressor.BaseModels(index), member)
            End If

            Return Nothing
        End Function

        Private Shared Function ExportClassifier(model As IClassifier, member As EnsembleMember) As String
            Select Case member.Algorithm
                Case "svm"
                    Dim svm As ScaledSvm.ScaledSvmClassifier = TryCast(model, ScaledSvm.ScaledSvmClassifier)

                    If svm IsNot Nothing Then
                        member.Parameters("indexOffset") = svm.IndexOffset
                        member.Parameters("inputOffset") = svm.InputOffset
                        Return SvmModelJSON.CreateJSONModel(svm.Fitted).GetJson
                    End If
                Case "logistic"
                    Dim logistic As LogisticClassifier = TryCast(model, LogisticClassifier)

                    If logistic IsNot Nothing Then
                        Return logistic.FittedModel.GetJson
                    End If
                Case "xgboost"
                    Dim xgboost As XGBoostClassifier = TryCast(model, XGBoostClassifier)

                    If xgboost IsNot Nothing Then
                        Return String.Join(vbLf, ModelSerializer.save_model(xgboost.FittedModel.FittedModel))
                    End If
            End Select

            ' naive-bayes 等无法导出内部状态的算法依赖 Snapshot 重训
            Return Nothing
        End Function

        Private Shared Function ExportRegressor(model As IRegressor, member As EnsembleMember) As String
            Select Case member.Algorithm
                Case "random-forest"
                    Dim forest As RandomForestRegressor = TryCast(model, RandomForestRegressor)

                    If forest IsNot Nothing Then
                        Return ExportForest(forest.FittedModel).GetJson
                    End If
                Case "ridge"
                    Dim ridge As RidgeRegressor = TryCast(model, RidgeRegressor)

                    If ridge IsNot Nothing Then
                        Return ridge.FittedModel.GetJson
                    End If
                Case "mlr"
                    Dim mlr As MlrRegressor = TryCast(model, MlrRegressor)

                    If mlr IsNot Nothing Then
                        Return mlr.FittedModel.GetJson
                    End If
                Case "svr"
                    Dim svr As ScaledSvm.ScaledSvmRegressor = TryCast(model, ScaledSvm.ScaledSvmRegressor)

                    If svr IsNot Nothing Then
                        member.Parameters("inputOffset") = svr.InputOffset
                        Return SvmModelJSON.CreateJSONModel(svr.Fitted).GetJson
                    End If
                Case "xgboost"
                    Dim xgboost As XGBoostRegressor = TryCast(model, XGBoostRegressor)

                    If xgboost IsNot Nothing Then
                        Return String.Join(vbLf, ModelSerializer.save_model(xgboost.FittedModel.FittedModel))
                    End If
            End Select

            Return Nothing
        End Function

        Private Shared Function ExportForest(forest As RanFog) As EnsembleForestDTO
            Dim trees As RandomForestBranchDTO()() = New RandomForestBranchDTO(forest.Trees.Count - 1)() {}

            For i As Integer = 0 To forest.Trees.Count - 1
                Dim branches As Branch() = forest.Trees(i)
                Dim rows As RandomForestBranchDTO() = New RandomForestBranchDTO(branches.Length - 1) {}

                For j As Integer = 0 To branches.Length - 1
                    Dim node As Branch = branches(j)

                    rows(j) = New RandomForestBranchDTO With {
                        .mean = node.mean,
                        .mean_snp = node.mean_snp,
                        .class_val = node.class_val,
                        .status = node.status,
                        .Feature = node.Feature,
                        .Child1 = node.Child1,
                        .Child2 = node.Child2,
                        .Parent = node.Parent,
                        .list = If(node.list, New List(Of Integer)()).ToArray()
                    }
                Next

                trees(i) = rows
            Next

            Return New EnsembleForestDTO With {
                .MaxTree = forest.max_tree,
                .MaxBranch = forest.max_branch,
                .Mtry = forest.mtry,
                .LossFunction = forest.LF_c.ToString(),
                .Trees = trees
            }
        End Function

        Private Function RestoreClassifier(member As EnsembleMember) As IClassifier
            Select Case member.Algorithm
                Case "svm"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Dim svm As Microsoft.VisualBasic.MachineLearning.SVM.SVMModel =
                        member.Payload.LoadJSON(Of SvmModelJSON)().CreateSVMModel()

                    Return New ScaledSvm.ScaledSvmClassifier(
                        svm, Classes, GetParameter(member, "indexOffset", 1), GetParameter(member, "inputOffset", 1))
                Case "logistic"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Return New LogisticClassifier(member.Payload.LoadJSON(Of LogisticNS.LogisticFit)(), Classes)
                Case "xgboost"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Dim gbm As GBM = ModelSerializer.load_model(SplitLines(member.Payload))
                    Dim model As New XGBoostModel(gbm, FeatureCount)

                    Return New XGBoostClassifier(model, Classes)
                Case "naive-bayes"
                    Return RetrainClassifier(member)
            End Select

            Return Nothing
        End Function

        Private Function RestoreRegressor(member As EnsembleMember) As IRegressor
            Select Case member.Algorithm
                Case "random-forest"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Return New RandomForestRegressor(RestoreForest(member.Payload.LoadJSON(Of EnsembleForestDTO)()))
                Case "ridge"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Return New RidgeRegressor(member.Payload.LoadJSON(Of RidgeNS.RidgeFit)())
                Case "mlr"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Return New MlrRegressor(member.Payload.LoadJSON(Of MultivariateNS.MLRFit)())
                Case "svr"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Dim svm As Microsoft.VisualBasic.MachineLearning.SVM.SVMModel =
                        member.Payload.LoadJSON(Of SvmModelJSON)().CreateSVMModel()

                    Return New ScaledSvm.ScaledSvmRegressor(svm, GetParameter(member, "inputOffset", 1))
                Case "xgboost"
                    If String.IsNullOrEmpty(member.Payload) Then
                        Return Nothing
                    End If

                    Dim gbm As GBM = ModelSerializer.load_model(SplitLines(member.Payload))
                    Dim model As New XGBoostModel(gbm, FeatureCount)

                    Return New XGBoostRegressor(model)
                Case "bayesian-regression"
                    Return RetrainRegressor(member)
            End Select

            Return Nothing
        End Function

        Private Shared Function RestoreForest(dto As EnsembleForestDTO) As RanFog
            Dim forest As New RanFog With {
                .max_tree = dto.MaxTree,
                .max_branch = dto.MaxBranch,
                .mtry = dto.Mtry,
                .LF_c = CType([Enum].Parse(GetType(LF_c), dto.LossFunction), LF_c)
            }
            Dim trees As New List(Of Branch())()

            For Each rows As RandomForestBranchDTO() In dto.Trees
                Dim branches As Branch() = New Branch(rows.Length - 1) {}

                For j As Integer = 0 To rows.Length - 1
                    Dim item As RandomForestBranchDTO = rows(j)

                    branches(j) = New Branch With {
                        .mean = item.mean,
                        .mean_snp = item.mean_snp,
                        .class_val = item.class_val,
                        .status = item.status,
                        .Feature = item.Feature,
                        .Child1 = item.Child1,
                        .Child2 = item.Child2,
                        .Parent = item.Parent,
                        .list = If(item.list, New Integer() {}).ToList()
                    }
                Next

                trees.Add(branches)
            Next

            forest.Trees = trees

            Return forest
        End Function

        Private Function RetrainClassifier(member As EnsembleMember) As IClassifier
            If String.IsNullOrEmpty(member.Snapshot) Then
                Return Nothing
            End If

            Dim snapshot As EnsembleTrainingSnapshot = member.Snapshot.LoadJSON(Of EnsembleTrainingSnapshot)()
            Dim learner As New NaiveBayesClassifierLearner With {
                .Smoothing = GetParameter(member, "smoothing", 1.0R)
            }
            Dim yIndex As Integer() = snapshot.Labels.Select(Function(v) CInt(v)).ToArray()

            Return learner.Fit(snapshot.ToMatrix(), yIndex, Classes)
        End Function

        Private Function RetrainRegressor(member As EnsembleMember) As IRegressor
            If String.IsNullOrEmpty(member.Snapshot) Then
                Return Nothing
            End If

            Dim snapshot As EnsembleTrainingSnapshot = member.Snapshot.LoadJSON(Of EnsembleTrainingSnapshot)()
            Dim learner As New BayesianRegressorLearner With {
                .Bins = GetParameter(member, "bins", 10),
                .Smoothing = GetParameter(member, "smoothing", 1.0R),
                .VarianceFloor = GetParameter(member, "varianceFloor", 0.000000001R)
            }

            Return learner.Fit(snapshot.ToMatrix(), snapshot.Labels)
        End Function

        Private Shared Function GetParameter(member As EnsembleMember, name As String, fallback As Integer) As Integer
            If member.Parameters IsNot Nothing AndAlso member.Parameters.ContainsKey(name) Then
                Return CInt(member.Parameters(name))
            End If

            Return fallback
        End Function

        Private Shared Function GetParameter(member As EnsembleMember, name As String, fallback As Double) As Double
            If member.Parameters IsNot Nothing AndAlso member.Parameters.ContainsKey(name) Then
                Return member.Parameters(name)
            End If

            Return fallback
        End Function

        Private Shared Function SplitLines(text As String) As String()
            Return text.Replace(vbCrLf, vbLf).Split(ChrW(10))
        End Function

        Private Shared Function Normalize(weights As List(Of Double)) As Double()
            If weights Is Nothing OrElse weights.Count = 0 Then
                Return New Double() {}
            End If

            Return ProbabilityMath.NormalizeWeights(weights.ToArray(), weights.Count)
        End Function

    End Class
End Namespace
