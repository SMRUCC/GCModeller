' ============================================================================
' PhenotypeEnsemble.vb
'
' 一个表型的集成学习运行时模型：由 VotingClassifier / VotingRegressor 负责
' 加权聚合，本类型只负责保存类别映射、成员元数据与对外预测入口。
'
' Classifier / Regressor 是运行时对象（不参与 JSON 序列化），
' 持久化由 EnsembleModelDTO 逐基模型导出后重建。
' ============================================================================

Imports System.Collections.Generic
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble

Namespace Traitar.Models

    ''' <summary>基模型在集成中的角色</summary>
    Public Enum EnsembleMemberKind
        Classifier = 0
        Regressor = 1
    End Enum

    ''' <summary>
    ''' 一个基学习器的元数据（算法、权重、交叉验证得分、超参、持久化载荷）。
    ''' </summary>
    Public Class EnsembleMember

        ''' <summary>算法名称，例如 <c>svm</c> / <c>naive-bayes</c> / <c>ridge</c></summary>
        Public Property Algorithm As String
        ''' <summary>分类器或回归器</summary>
        Public Property Kind As EnsembleMemberKind
        ''' <summary>归一化之后的投票权重</summary>
        Public Property Weight As Double
        ''' <summary>交叉验证得分（分类为准确率，回归为 R²）</summary>
        Public Property CVScore As Double
        ''' <summary>训练状态：trained / failed / skipped</summary>
        Public Property Status As String
        ''' <summary>训练失败原因</summary>
        Public Property ErrorMessage As String
        ''' <summary>本次训练使用的超参数快照</summary>
        Public Property Parameters As Dictionary(Of String, Double)
        ''' <summary>算法专用的持久化载荷（由 EnsembleModelDTO 写入）</summary>
        Public Property Payload As String
        ''' <summary>
        ''' 训练快照（仅用于无法导出内部状态的算法，例如朴素贝叶斯与贝叶斯回归，
        ''' 加载时用该快照重训该成员）。
        ''' </summary>
        Public Property Snapshot As String

        Public Overrides Function ToString() As String
            Return $"{Algorithm} w={Weight:F4} cv={CVScore:F4} [{Status}]"
        End Function

    End Class

    ''' <summary>
    ''' 单个表型的集成学习模型
    ''' </summary>
    Public Class PhenotypeEnsemble

        ''' <summary>集成方式，当前为 <c>voting</c></summary>
        Public Property Mode As String
        ''' <summary>投票模式</summary>
        Public Property Voting As VotingMode
        ''' <summary>全局类别标签值（0..K-1）</summary>
        Public Property Classes As Double()
        ''' <summary>与 <see cref="Classes"/> 顺序严格对齐的类别文本</summary>
        Public Property ClassLabels As String()
        ''' <summary>全部基学习器的元数据</summary>
        Public Property Members As EnsembleMember()
        ''' <summary>归一化之后的权重，与 <see cref="Members"/> 顺序对齐</summary>
        Public Property Weights As Double()
        ''' <summary>训练时的特征维度</summary>
        Public Property FeatureCount As Integer

        ''' <summary>分类集成器（运行时对象，不序列化）</summary>
        Public Property Classifier As IClassifier
        ''' <summary>回归集成器（运行时对象，不序列化）</summary>
        Public Property Regressor As IRegressor

        Public ReadOnly Property IsClassifier As Boolean
            Get
                Return Classifier IsNot Nothing
            End Get
        End Property

        Public ReadOnly Property BaseModelCount As Integer
            Get
                If Members Is Nothing Then
                    Return 0
                Else
                    Return Members.Length
                End If
            End Get
        End Property

        ''' <summary>分类集成预测：返回与 <see cref="Classes"/> 对齐的加权概率向量</summary>
        Public Function PredictProbability(x As Double()) As Double()
            If Classifier Is Nothing Then
                Throw New InvalidOperationException("this ensemble is not a classification model!")
            End If

            Return Classifier.PredictProbability(x)
        End Function

        ''' <summary>回归集成预测：返回加权平均值</summary>
        Public Function PredictValue(x As Double()) As Double
            If Regressor Is Nothing Then
                Throw New InvalidOperationException("this ensemble is not a regression model!")
            End If

            Return Regressor.Predict(x)
        End Function

        ''' <summary>取概率向量中最大项对应的类别下标</summary>
        Public Function ArgMaxClass(probability As Double()) As Integer
            Return ProbabilityMath.ArgMax(probability)
        End Function

        ''' <summary>把类别下标转换为原始标签文本</summary>
        Public Function GetClassLabel(index As Integer) As String
            If ClassLabels Is Nothing OrElse index < 0 OrElse index >= ClassLabels.Length Then
                Return Nothing
            End If

            Return ClassLabels(index)
        End Function

        ''' <summary>把原始标签文本转换为类别下标（找不到时返回 -1）</summary>
        Public Function ClassIndexOf(label As String) As Integer
            If ClassLabels Is Nothing Then
                Return -1
            End If

            For i As Integer = 0 To ClassLabels.Length - 1
                If String.Equals(ClassLabels(i), label, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        Public Overrides Function ToString() As String
            Dim kind As String = If(IsClassifier, $"classifier x{BaseModelCount}", $"regressor x{BaseModelCount}")
            Return $"[{Mode}] {kind}"
        End Function

    End Class
End Namespace
