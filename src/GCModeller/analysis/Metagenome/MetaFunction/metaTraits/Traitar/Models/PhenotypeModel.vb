' ============================================================================
' PhenotypeModel.vb
'
' 表型预测模型数据结构：每一种生物表型对应着一个具体的 SVM 模型实例。
'
' 重构之后不再使用旧的「多个 C 参数的子模型投票委员会」机制，而是把整
' 个 SVM 模型（支持向量 + 决策系数 + RangeTransform + 类别编码器）封装为
' 一个 PhenotypeModel，全部训练与预测工作委托给底层的 LibSVM 算法库完成。
' ============================================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace metaTraits.Traitar.Models

    ''' <summary>
    ''' 单一种生物表型所对应的 SVM 模型实例
    ''' </summary>
    Public Class PhenotypeModel

        ''' <summary>该模型所对应的表型元数据</summary>
        Public Property Trait As PhenotypeTrait
        ''' <summary>
        ''' 训练得到的 LibSVM 模型（含支持向量、决策系数、
        ''' RangeTransform 以及类别编码器）
        ''' </summary>
        Public Property Model As SVMModel

        ''' <summary>参与该模型训练的样本数量</summary>
        Public Property SampleCount As Integer
        ''' <summary>交叉验证得分（分类为准确率，回归为相关系数）</summary>
        Public Property CVScore As Double
        ''' <summary>模型状态：``trained`` / ``skipped``</summary>
        Public Property Status As String
        ''' <summary>训练失败（或被跳过）的原因</summary>
        Public Property ErrorMessage As String
        ''' <summary>该表型的关键 Pfam 结构域特征</summary>
        Public Property KeyFeatures As Modules.KeyFeature()

        ''' <summary>该模型是否已经训练成功</summary>
        Public Function IsTrained() As Boolean
            Return Model IsNot Nothing
        End Function

        ''' <summary>该模型是否是一个回归模型（numeric 型表型）</summary>
        Public Function IsRegression() As Boolean
            If Trait Is Nothing Then
                Return False
            End If

            Return Trait.IsRegression()
        End Function

        ''' <summary>
        ''' 使用已经归一化之后的 Pfam 特征向量进行表型预测
        ''' </summary>
        ''' <param name="profile">Pfam 编号 -> 归一化之后的丰度值</param>
        ''' <param name="embedding">Pfam 嵌入配置（提供词表顺序）</param>
        Public Function Predict(profile As IDictionary(Of String, Double), embedding As PfamEmbedding) As TraitPrediction
            Return Predict(embedding.ToNodes(profile))
        End Function

        ''' <summary>
        ''' 使用 1-based 的稠密特征向量进行表型预测
        ''' </summary>
        ''' <param name="features">未经缩放的原始特征向量</param>
        Public Function Predict(features As Node()) As TraitPrediction
            Dim result As New TraitPrediction With {
                .trait_name = If(Trait Is Nothing, Nothing, Trait.trait_name),
                .data_type = If(Trait Is Nothing, Nothing, Trait.data_type),
                .unit = If(Trait Is Nothing, Nothing, Trait.unit),
                .group_1 = If(Trait Is Nothing, Nothing, Trait.group_1_category),
                .group_2 = If(Trait Is Nothing, Nothing, Trait.group_2_subcategory),
                .status = Status
            }

            If Not IsTrained() Then
                result.predict = Nothing
                result.confidence = 0
                Return result
            End If

            ' 预测之前必须应用与训练时完全相同的 range transform
            Dim scaled As Node() = Model.transform.Transform(features)
            Dim pred As SVMPrediction = Microsoft.VisualBasic.MachineLearning.SVM.Prediction.Predict(Model.model, scaled)

            If Model.SVR Then
                ' 回归模型：unifyValue 即为回归预测值
                result.value = pred.unifyValue
                result.score = pred.score
                result.predict = pred.unifyValue.ToString("G6")
                result.confidence = 1.0
            Else
                Dim margin As Double = GetDecisionMargin(pred)

                result.value = pred.class
                result.score = pred.score
                result.votes = pred.vote
                result.confidence = 1.0 / (1.0 + System.Math.Exp(-System.Math.Abs(margin)))

                Dim cls As Microsoft.VisualBasic.DataMining.ComponentModel.Encoder.ColorClass =
                    Model.factors.GetColor(pred.class)

                If cls Is Nothing Then
                    result.predict = pred.class.ToString()
                Else
                    result.predict = cls.name
                End If
            End If

            Return result
        End Function

        ''' <summary>
        ''' 取出预测类别所对应的决策边距值，用于计算预测置信度
        ''' </summary>
        Private Function GetDecisionMargin(pred As SVMPrediction) As Double
            If pred.vote Is Nothing OrElse pred.vote.Length = 0 Then
                Return 0
            End If
            If Model.model.classLabels Is Nothing Then
                Return 0
            End If

            Dim index As Integer = Array.IndexOf(Model.model.classLabels, pred.class)

            If index < 0 OrElse index >= pred.vote.Length Then
                Return 0
            End If

            Return pred.vote(index)
        End Function

        Public Overrides Function ToString() As String
            If Trait Is Nothing Then
                Return "n/a"
            End If

            Return $"[{Status}] {Trait.trait_name}, samples={SampleCount}, cv={CVScore:F4}"
        End Function

    End Class
End Namespace
