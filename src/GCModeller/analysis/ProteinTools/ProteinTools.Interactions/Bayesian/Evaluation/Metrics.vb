Namespace Bayesian.Evaluation

    ''' <summary>
    ''' 分类与分布比较的基础度量。
    ''' </summary>
    ''' <remarks>
    ''' 这些纯函数同时被 <see cref="RocPrCurve"/>、<see cref="Benchmark"/> 以及测试工程使用，
    ''' 因此全部实现为不依赖任何外部数值库的静态方法，便于用解析解或者手算值做对照。
    ''' </remarks>
    Public Module Metrics

        ''' <summary>
        ''' 混淆矩阵
        ''' </summary>
        Public Class ConfusionMatrix

            ''' <summary>
            ''' 真阳性
            ''' </summary>
            Public Property TruePositive As Integer
            ''' <summary>
            ''' 假阳性
            ''' </summary>
            Public Property FalsePositive As Integer
            ''' <summary>
            ''' 真阴性
            ''' </summary>
            Public Property TrueNegative As Integer
            ''' <summary>
            ''' 假阴性
            ''' </summary>
            Public Property FalseNegative As Integer

            ''' <summary>
            ''' 正样本总数
            ''' </summary>
            Public ReadOnly Property Positives As Integer
                Get
                    Return TruePositive + FalseNegative
                End Get
            End Property

            ''' <summary>
            ''' 负样本总数
            ''' </summary>
            Public ReadOnly Property Negatives As Integer
                Get
                    Return FalsePositive + TrueNegative
                End Get
            End Property

            ''' <summary>
            ''' 灵敏度(召回率)
            ''' </summary>
            Public ReadOnly Property Sensitivity As Double
                Get
                    Return SafeDivide(TruePositive, Positives)
                End Get
            End Property

            ''' <summary>
            ''' 阳性预测值(精确率)
            ''' </summary>
            Public ReadOnly Property Precision As Double
                Get
                    Return SafeDivide(TruePositive, TruePositive + FalsePositive)
                End Get
            End Property

            ''' <summary>
            ''' 特异度
            ''' </summary>
            Public ReadOnly Property Specificity As Double
                Get
                    Return SafeDivide(TrueNegative, Negatives)
                End Get
            End Property

            ''' <summary>
            ''' F1 分数
            ''' </summary>
            Public ReadOnly Property F1 As Double
                Get
                    Dim p As Double = Precision
                    Dim r As Double = Sensitivity

                    If p + r <= 0 Then
                        Return 0.0
                    Else
                        Return 2 * p * r / (p + r)
                    End If
                End Get
            End Property

            ''' <summary>
            ''' 准确率
            ''' </summary>
            Public ReadOnly Property Accuracy As Double
                Get
                    Return SafeDivide(TruePositive + TrueNegative, Positives + Negatives)
                End Get
            End Property

            ''' <summary>
            ''' Matthews 相关系数，在正负样本严重不平衡时比准确率更有意义
            ''' </summary>
            Public ReadOnly Property MCC As Double
                Get
                    Dim tp As Double = TruePositive
                    Dim tn As Double = TrueNegative
                    Dim fp As Double = FalsePositive
                    Dim fn As Double = FalseNegative
                    Dim denominator As Double = Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn))

                    If denominator <= 0 Then
                        Return 0.0
                    Else
                        Return (tp * tn - fp * fn) / denominator
                    End If
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return $"TP={TruePositive}, FP={FalsePositive}, TN={TrueNegative}, FN={FalseNegative}; " &
                    $"Se={Sensitivity:0.000}, PPV={Precision:0.000}, F1={F1:0.000}"
            End Function
        End Class

        ''' <summary>
        ''' 在给定的阈值之上统计混淆矩阵
        ''' </summary>
        ''' <param name="scores">模型的连续打分</param>
        ''' <param name="labels">真实标签，<c>True</c> 表示正样本</param>
        ''' <param name="threshold">判定阈值，打分 <c>≥ threshold</c> 时判为正</param>
        Public Function Confusion(scores As Double(), labels As Boolean(), threshold As Double) As ConfusionMatrix
            If scores Is Nothing OrElse labels Is Nothing OrElse scores.Length <> labels.Length Then
                Throw New ArgumentException("scores and labels must be aligned")
            End If

            Dim matrix As New ConfusionMatrix

            For i As Integer = 0 To scores.Length - 1
                Dim predicted As Boolean = scores(i) >= threshold

                If predicted Then
                    If labels(i) Then
                        matrix.TruePositive += 1
                    Else
                        matrix.FalsePositive += 1
                    End If
                Else
                    If labels(i) Then
                        matrix.FalseNegative += 1
                    Else
                        matrix.TrueNegative += 1
                    End If
                End If
            Next

            Return matrix
        End Function

        ''' <summary>
        ''' 两个离散分布之间的 KL 散度(单位 nat)，第二个分布之中为零的位置会被跳过
        ''' </summary>
        Public Function KLDivergence(p As Double(), q As Double()) As Double
            If p Is Nothing OrElse q Is Nothing OrElse p.Length <> q.Length Then
                Throw New ArgumentException("the two distributions must have the same length")
            End If

            Dim value As Double = 0.0

            For i As Integer = 0 To p.Length - 1
                If p(i) > 0 AndAlso q(i) > 0 Then
                    value += p(i) * Math.Log(p(i) / q(i))
                End If
            Next

            Return value
        End Function

        ''' <summary>
        ''' 两个分布之间的总变差距离
        ''' </summary>
        Public Function TotalVariation(p As Double(), q As Double()) As Double
            If p Is Nothing OrElse q Is Nothing OrElse p.Length <> q.Length Then
                Throw New ArgumentException("the two distributions must have the same length")
            End If

            Dim value As Double = 0.0

            For i As Integer = 0 To p.Length - 1
                value += Math.Abs(p(i) - q(i))
            Next

            Return value / 2.0
        End Function

        ''' <summary>
        ''' 安全的除法，分母为零的时候返回 0
        ''' </summary>
        Public Function SafeDivide(numerator As Double, denominator As Double) As Double
            If denominator <= 0 Then
                Return 0.0
            Else
                Return numerator / denominator
            End If
        End Function
    End Module
End Namespace
