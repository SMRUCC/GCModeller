' ============================================================================
' EnsembleModelDTO.vb
'
' 集成模型的轻量元数据。模型的数值状态（基模型、权重、类别顺序、超参）
' 由 Ensemble\Storage 的 EnsembleModelStore 写出为 model.json + CSV 附表；
' 本类型只保存它不覆盖的部分：类别文本、成员的人类可读元数据与 CV 指标。
'
' 目录布局：
'   <repo>/{nnnnn}_{trait}.ensemble.json    ← 本 DTO
'   <repo>/{nnnnn}_{trait}.ensemble/        ← EnsembleModelStore 模型包
'       ├── model.json
'       └── base/<i>/...
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping.Ensemble

Namespace Traitar.Models

    ''' <summary>集成模型的元数据（与 EnsembleModelStore 模型包配合）</summary>
    Public Class EnsembleModelDTO

        ''' <summary>集成方式，当前为 <c>voting</c></summary>
        Public Property Mode As String
        ''' <summary>投票模式</summary>
        Public Property Voting As VotingMode
        ''' <summary>全局类别标签值（0..K-1）</summary>
        Public Property Classes As Double()
        ''' <summary>与 <see cref="Classes"/> 顺序严格对齐的类别文本</summary>
        Public Property ClassLabels As String()
        ''' <summary>训练时的特征维度</summary>
        Public Property FeatureCount As Integer
        ''' <summary>归一化之后的权重，与 <see cref="Members"/> 顺序对齐</summary>
        Public Property Weights As Double()
        ''' <summary>全部基学习器的元数据</summary>
        Public Property Members As EnsembleMember()

        ''' <summary>从运行时集成模型导出元数据（不含模型数值状态）</summary>
        Public Shared Function CreateDTO(ensemble As PhenotypeEnsemble) As EnsembleModelDTO
            If ensemble Is Nothing Then
                Return Nothing
            End If

            Dim members As EnsembleMember()

            If ensemble.Members Is Nothing Then
                members = New EnsembleMember() {}
            Else
                members = ensemble.Members.Select(AddressOf CopyMember).ToArray()
            End If

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

        ''' <summary>用已从模型包恢复的集成器重建运行时模型</summary>
        ''' <param name="model"><see cref="IClassifier"/> 或 <see cref="IRegressor"/> 实例</param>
        Public Function CreateEnsemble(model As Object) As PhenotypeEnsemble
            If model Is Nothing Then
                Return Nothing
            End If

            Dim ensemble As New PhenotypeEnsemble With {
                .Mode = Mode,
                .Voting = Voting,
                .Classes = Classes,
                .ClassLabels = ClassLabels,
                .Members = Members,
                .Weights = Weights,
                .FeatureCount = FeatureCount
            }

            If TypeOf model Is IClassifier Then
                ensemble.Classifier = DirectCast(model, IClassifier)
            ElseIf TypeOf model Is IRegressor Then
                ensemble.Regressor = DirectCast(model, IRegressor)
            Else
                Throw New InvalidCastException(
                    $"the restored model is a '{model.GetType().FullName}', not an IClassifier/IRegressor!")
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
                .Parameters = parameters
            }
        End Function

    End Class
End Namespace
