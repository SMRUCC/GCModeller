' ============================================================================
' ScaledSvmLearners.vb
'
' 与 Ensemble 自带的 SvmClassifierLearner 不同，这里的 SVM 基学习器会在
' Fit 内部完成 RangeTransform.Compute/Scale，并保留 SVMModel（含 transform），
' 因此：
'   1. 与现有 PhenotypeSVMTrainer 的特征缩放保持一致；
'   2. 可以直接复用 SvmModelJSON 做逐基模型持久化。
' ============================================================================

Imports System
Imports System.Linq
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DataMining.ComponentModel.Encoder
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace Traitar.Modules

    ''' <summary>
    ''' SVM 分类基学习器（<c>C_SVC</c> + RBF，内部完成特征缩放，概率来自
    ''' <c>svm_predict_probability</c>）。
    ''' </summary>
    Public NotInheritable Class ScaledSvmClassifierLearner : Implements IClassifierLearner

        ''' <summary>惩罚系数 <c>C</c></summary>
        Public Property C As Double = 1.0R
        ''' <summary>RBF 的 <c>gamma</c>；小于等于 0 时自动取 <c>1 / 特征数</c></summary>
        Public Property Gamma As Double = 0.0R
        ''' <summary>特征索引基，默认 1（与现有 SVM 训练一致）</summary>
        Public Property FeatureOffset As Integer = 1

        Public ReadOnly Property Name As String Implements IClassifierLearner.Name
            Get
                Return "svm"
            End Get
        End Property

        Public Function Fit(x As Double()(), yIndex As Integer(), classes As Double()) As IClassifier Implements IClassifierLearner.Fit
            Dim offset As Integer = FeatureOffset
            Dim labels As Double() = yIndex.Select(Function(v) CDbl(v)).ToArray()
            Dim problem As Problem = ScaledSvmHelper.CreateProblem(x, labels, offset, offset)
            Dim transform As RangeTransform = RangeTransform.Compute(problem)
            Dim scaled As Problem = transform.Scale(problem)
            Dim parameter As New Parameter With {
                .svmType = SvmType.C_SVC,
                .kernelType = KernelType.RBF,
                .c = C,
                .gamma = If(Gamma > 0.0R, Gamma, 1.0R / x(0).Length),
                .probability = True,
                .cacheSize = 100
            }
            Dim trained As Microsoft.VisualBasic.MachineLearning.SVM.Model = Training.Train(scaled, parameter)
            Dim svm As New SVMModel With {
                .model = trained,
                .transform = transform,
                .factors = New ClassEncoder(scaled.Y)
            }

            Return New ScaledSvmClassifier(svm, classes, offset, offset)
        End Function

    End Class

    ''' <summary>SVM 分类适配器（保留 <see cref="SVMModel"/> 以支持持久化）</summary>
    Public NotInheritable Class ScaledSvmClassifier : Implements IClassifier

        ''' <summary>训练得到的完整 SVM 模型（含缩放与类别编码）</summary>
        Public ReadOnly Property Fitted As SVMModel

        Private ReadOnly classesArray As Double()
        Private ReadOnly labelOffset As Integer
        Private ReadOnly featureOffset As Integer

        Public Sub New(fitted As SVMModel, classes As Double(),
                       labelOffset As Integer, featureOffset As Integer)
            Me.Fitted = fitted
            Me.classesArray = classes
            Me.labelOffset = labelOffset
            Me.featureOffset = featureOffset
        End Sub

        Public ReadOnly Property Classes As Double() Implements IClassifier.Classes
            Get
                Return classesArray
            End Get
        End Property

        ''' <summary>类别标签的索引基（持久化恢复时使用）</summary>
        Public ReadOnly Property IndexOffset As Integer
            Get
                Return labelOffset
            End Get
        End Property

        ''' <summary>特征索引基（持久化恢复时使用）</summary>
        Public ReadOnly Property InputOffset As Integer
            Get
                Return featureOffset
            End Get
        End Property

        Public Function PredictProbability(x As Double()) As Double() Implements IClassifier.PredictProbability
            Dim nodes As Node() = EnsembleFeatureMatrix.ToNodes(x, featureOffset)
            Dim scaled As Node() = If(Fitted.transform Is Nothing,
                                       nodes,
                                       Fitted.transform.Transform(nodes))
            Dim probability As Double() = Fitted.model.PredictProbability(scaled)
            Dim labels As Integer() = Fitted.model.classLabels

            If labels Is Nothing OrElse labels.Length <> probability.Length Then
                Throw New InvalidOperationException(
                    "the SVM model does not carry class labels; retrain it with Parameter.probability = True!")
            End If

            Dim result As Double() = New Double(classesArray.Length - 1) {}

            For i As Integer = 0 To probability.Length - 1
                Dim slot As Integer = labels(i) - labelOffset

                If slot >= 0 AndAlso slot < result.Length Then
                    result(slot) = probability(i)
                End If
            Next

            Return ProbabilityMath.Normalize(result)
        End Function

    End Class

    ''' <summary>
    ''' SVM 回归基学习器（<c>EPSILON_SVR</c>，内部完成特征缩放）
    ''' </summary>
    Public NotInheritable Class ScaledSvmRegressorLearner : Implements IRegressorLearner

        ''' <summary>惩罚系数 <c>C</c></summary>
        Public Property C As Double = 1.0R
        ''' <summary>RBF 的 <c>gamma</c>；小于等于 0 时自动取 <c>1 / 特征数</c></summary>
        Public Property Gamma As Double = 0.0R
        ''' <summary>不敏感带宽度 <c>epsilon</c></summary>
        Public Property Epsilon As Double = 0.1R
        ''' <summary>特征索引基，默认 1</summary>
        Public Property FeatureOffset As Integer = 1

        Public ReadOnly Property Name As String Implements IRegressorLearner.Name
            Get
                Return "svr"
            End Get
        End Property

        Public Function Fit(x As Double()(), y As Double()) As IRegressor Implements IRegressorLearner.Fit
            Dim offset As Integer = FeatureOffset
            Dim problem As Problem = ScaledSvmHelper.CreateProblem(x, y, offset, 0)
            Dim transform As RangeTransform = RangeTransform.Compute(problem)
            Dim scaled As Problem = transform.Scale(problem)
            Dim parameter As New Parameter With {
                .svmType = SvmType.EPSILON_SVR,
                .kernelType = KernelType.RBF,
                .c = C,
                .gamma = If(Gamma > 0.0R, Gamma, 1.0R / x(0).Length),
                .P = Epsilon,
                .probability = False,
                .cacheSize = 100
            }
            Dim trained As Microsoft.VisualBasic.MachineLearning.SVM.Model = Training.Train(scaled, parameter)
            Dim svm As New SVMModel With {
                .model = trained,
                .transform = transform,
                .factors = New ClassEncoder()
            }

            Return New ScaledSvmRegressor(svm, offset)
        End Function

    End Class

    ''' <summary>SVR 回归适配器（保留 <see cref="SVMModel"/> 以支持持久化）</summary>
    Public NotInheritable Class ScaledSvmRegressor : Implements IRegressor

        ''' <summary>训练得到的完整 SVM 模型（含缩放）</summary>
        Public ReadOnly Property Fitted As SVMModel

        Private ReadOnly featureOffset As Integer

        Public Sub New(fitted As SVMModel, featureOffset As Integer)
            Me.Fitted = fitted
            Me.featureOffset = featureOffset
        End Sub

        ''' <summary>特征索引基（持久化恢复时使用）</summary>
        Public ReadOnly Property InputOffset As Integer
            Get
                Return featureOffset
            End Get
        End Property

        Public Function Predict(x As Double()) As Double Implements IRegressor.Predict
            Dim nodes As Node() = EnsembleFeatureMatrix.ToNodes(x, featureOffset)
            Dim scaled As Node() = If(Fitted.transform Is Nothing,
                                       nodes,
                                       Fitted.transform.Transform(nodes))

            ' 回归模型的 class 字段是哨兵值，真实预测值在 score 里
            Return Fitted.model.Predict(scaled).score
        End Function

    End Class

    ''' <summary>供 ScaledSvm 分类/回归共用的 Problem 构造</summary>
    Friend Module ScaledSvmHelper

        ''' <summary>
        ''' 构造 LibSVM 的 <see cref="Problem"/>：分类时 <paramref name="labelOffset"/>
        ''' 等于特征索引基，回归时传 0 以保留真实目标值。
        ''' </summary>
        Public Function CreateProblem(x As Double()(), labels As Double(),
                                      featureOffset As Integer, labelOffset As Integer) As Problem

            Dim nodes As Node()() = New Node(x.Length - 1)() {}
            Dim placeholder As String() = New String(x.Length - 1) {}

            For i As Integer = 0 To x.Length - 1
                nodes(i) = EnsembleFeatureMatrix.ToNodes(x(i), featureOffset)
                placeholder(i) = CStr(i)
            Next

            Dim problem As New Problem(placeholder, nodes, x(0).Length)
            Dim colored As ColorClass() = New ColorClass(labels.Length - 1) {}

            For i As Integer = 0 To labels.Length - 1
                colored(i) = New ColorClass With {
                    .factor = labels(i) + labelOffset,
                    .name = CStr(labels(i))
                }
            Next

            problem.Y = colored

            Return problem
        End Function

    End Module
End Namespace
