
''' <summary>
''' 基因预测结果
''' </summary>
Public Class PredictionResult
    ''' <summary>序列ID</summary>
    Public Property SeqId As String

    ''' <summary>序列长度</summary>
    Public Property SeqLength As Integer

    ''' <summary>预测的基因列表</summary>
    Public Property Genes As List(Of PredictedGene)

    ''' <summary>使用的训练模型</summary>
    Public Property Model As TrainingModel

    Public Sub New()
        Genes = New List(Of PredictedGene)()
    End Sub
End Class