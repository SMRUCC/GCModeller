Imports System.Runtime.CompilerServices
Imports Trait = SMRUCC.genomics.ComponentModel.Annotation.PhenotypeTrait

' ============================================================================
' ReportJSON.vb
'
' 最终的 JSON 报告数据结构：既可以表达分类结果（boolean / categorical），
' 也可以表达回归结果（numeric (continuous)）。
' ============================================================================

Namespace Traitar

    Public Class ReportJSON

        ''' <summary>表型标识（即表型名称）</summary>
        Public Property phenotypeId As String
        ''' <summary>表型名称</summary>
        Public Property accession As String
        ''' <summary>表型分类（主分类 / 次分类）</summary>
        Public Property category As String
        ''' <summary>计量单位</summary>
        Public Property unit As String
        ''' <summary>表型的数据类型</summary>
        Public Property data_type As String

        ''' <summary>
        ''' 预测结果：分类模型为类别标签文本，回归模型为预测数值文本
        ''' </summary>
        Public Property predict As String
        ''' <summary>boolean 型表型所对应的枚举结果</summary>
        Public Property result As PredictionResults
        ''' <summary>预测置信度，取值区间 [0,1]</summary>
        Public Property confidence As Double
        ''' <summary>决策函数的输出值</summary>
        Public Property score As Double
        ''' <summary>每一个类别所获得的决策值</summary>
        Public Property votes As Double()
        ''' <summary>模型的状态：trained / skipped</summary>
        Public Property status As String
        ''' <summary>该模型的交叉验证得分</summary>
        Public Property cvScore As Double
        ''' <summary>参与训练的样本数量</summary>
        Public Property sampleCount As Integer

        ''' <summary>该表型的关键 Pfam 结构域特征</summary>
        Public Property KeyFeatures As Modules.KeyFeature()

        Public Overrides Function ToString() As String
            Return $"{accession} = {predict}{unit}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToPhenotype() As Trait
            Return New Trait With {
                .accession = accession,
                .category = category,
                .confidence = confidence,
                .cvScore = cvScore,
                .data_type = data_type,
                .result = predict,
                .score = score,
                .unit = unit
            }
        End Function

    End Class
End Namespace
