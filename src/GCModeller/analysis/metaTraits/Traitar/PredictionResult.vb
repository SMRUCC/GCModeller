' ============================================================================
' PredictionResult.vb
'
' 表型预测的结果数据结构：既可以表达分类结果（boolean / categorical），
' 也可以表达回归结果（numeric (continuous)）。
' ============================================================================

Namespace metaTraits.Traitar

    Public Enum PredictionResults
        NA
        [TRUE]
        [FALSE]
    End Enum

    ''' <summary>
    ''' 某一种表型在某一个样本之上的预测结果
    ''' </summary>
    Public Class TraitPrediction

        ''' <summary>表型名称</summary>
        Public Property trait_name As String
        ''' <summary>表型的数据类型（boolean / categorical / numeric (continuous)）</summary>
        Public Property data_type As String
        ''' <summary>计量单位</summary>
        Public Property unit As String
        ''' <summary>主分类</summary>
        Public Property group_1 As String
        ''' <summary>次分类</summary>
        Public Property group_2 As String

        ''' <summary>
        ''' 预测结果：分类模型为预测的类别标签文本（如 ``true`` / ``rod``），
        ''' 回归模型为预测数值的文本形式
        ''' </summary>
        Public Property predict As String
        ''' <summary>
        ''' 数值形式的结果：分类模型为类别的 factor 编码，回归模型为回归值
        ''' </summary>
        Public Property value As Double
        ''' <summary>决策函数的输出值（分类为边距，回归为回归值）</summary>
        Public Property score As Double
        ''' <summary>预测的置信度，取值区间 [0,1]</summary>
        Public Property confidence As Double
        ''' <summary>每一个类别所获得的决策值（多分类投票）</summary>
        Public Property votes As Double()
        ''' <summary>该表型模型的状态：trained / skipped</summary>
        Public Property status As String

        ''' <summary>
        ''' 该表型是否是回归（数值型）表型
        ''' </summary>
        Public Function IsRegression() As Boolean
            Return PhenotypeTraits.ParseDataType(data_type) = TraitDataType.Numeric
        End Function

        ''' <summary>
        ''' 当表型为 boolean 型时，把预测结果转换为 <see cref="PredictionResults"/>
        ''' </summary>
        Public Function GetBooleanResult() As PredictionResults
            If predict Is Nothing Then
                Return PredictionResults.NA
            End If

            If predict.Equals("true", StringComparison.OrdinalIgnoreCase) OrElse
                predict.Equals("yes", StringComparison.OrdinalIgnoreCase) Then

                Return PredictionResults.TRUE
            ElseIf predict.Equals("false", StringComparison.OrdinalIgnoreCase) OrElse
                predict.Equals("no", StringComparison.OrdinalIgnoreCase) Then

                Return PredictionResults.FALSE
            Else
                Return PredictionResults.NA
            End If
        End Function

        Public Overrides Function ToString() As String
            If IsRegression() Then
                Return $"{trait_name} = {predict}{unit}"
            Else
                Return $"{trait_name} = {predict}, confidence={confidence:F4}"
            End If
        End Function

    End Class
End Namespace
