Imports System.Text
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment

Namespace Bayesian.Evaluation

    ''' <summary>
    ''' ROC 曲线、PR 曲线以及阈值扫描。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 论文与 readme 都强调：互作预测属于<b>正负样本严重不平衡</b>的问题，
    ''' 因此 <b>AUC-PR 比 AUC-ROC 更重要</b>，而且应当同时报告「灵敏度–PPV」曲线
    ''' 而不是只给一个孤立的数字。
    ''' </para>
    ''' <para>
    ''' 本模块提供三种输出：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><see cref="AucRoc"/>：梯形法积分的 ROC 曲线下面积；</description></item>
    ''' <item><description><see cref="AucPr"/>：阶梯法积分的精确率–召回率曲线下面积(等价于平均精确率 AP)；</description></item>
    ''' <item><description><see cref="ThresholdSweep"/>：扫描阈值并给出每个阈值之上的灵敏度与 PPV。</description></item>
    ''' </list>
    ''' <para>以及用于概率校准的分箱统计(<see cref="Calibration"/>)。</para>
    ''' </remarks>
    Public Class RocPrCurve

        ''' <summary>
        ''' 曲线上的一个点
        ''' </summary>
        Public Class Point

            ''' <summary>
            ''' 阈值
            ''' </summary>
            Public Property Threshold As Double
            ''' <summary>
            ''' 假阳性率(1 - 特异度)
            ''' </summary>
            Public Property FalsePositiveRate As Double
            ''' <summary>
            ''' 真阳性率(灵敏度)
            ''' </summary>
            Public Property TruePositiveRate As Double
            ''' <summary>
            ''' 阳性预测值(精确率)
            ''' </summary>
            Public Property Precision As Double

            Public Overrides Function ToString() As String
                Return $"t={Threshold:0.###} FPR={FalsePositiveRate:0.000} TPR={TruePositiveRate:0.000} PPV={Precision:0.000}"
            End Function
        End Class

        ''' <summary>
        ''' 校准曲线上的一个分箱
        ''' </summary>
        Public Class CalibrationBin

            ''' <summary>
            ''' 预测概率的下界
            ''' </summary>
            Public Property Lower As Double
            ''' <summary>
            ''' 预测概率的上界
            ''' </summary>
            Public Property Upper As Double
            ''' <summary>
            ''' 箱内的平均预测概率
            ''' </summary>
            Public Property Predicted As Double
            ''' <summary>
            ''' 箱内实际的正样本比例
            ''' </summary>
            Public Property Observed As Double
            ''' <summary>
            ''' 箱内的样本个数
            ''' </summary>
            Public Property Count As Integer

            Public Overrides Function ToString() As String
                Return $"[{Lower:0.0}, {Upper:0.0}) predicted={Predicted:0.000} observed={Observed:0.000} n={Count}"
            End Function
        End Class

        Private ReadOnly _scores As Double()
        Private ReadOnly _labels As Boolean()

        ''' <summary>
        ''' 由打分与标签构造曲线
        ''' </summary>
        ''' <param name="scores">模型的连续打分</param>
        ''' <param name="labels">真实标签，<c>True</c> 表示正样本</param>
        Public Sub New(scores As Double(), labels As Boolean())
            If scores Is Nothing OrElse labels Is Nothing Then
                Throw New ArgumentNullException(NameOf(scores))
            End If
            If scores.Length <> labels.Length Then
                Throw New ArgumentException("scores and labels must be aligned", NameOf(labels))
            End If

            _scores = scores
            _labels = labels
        End Sub

        ''' <summary>
        ''' 正样本个数
        ''' </summary>
        Public ReadOnly Property Positives As Integer
            Get
                Return _labels.Count(Function(l) l)
            End Get
        End Property

        ''' <summary>
        ''' 负样本个数
        ''' </summary>
        Public ReadOnly Property Negatives As Integer
            Get
                Return _labels.Length - Positives
            End Get
        End Property

        ''' <summary>
        ''' 扫描阈值得到曲线上的全部点(按照阈值降序排列)
        ''' </summary>
        Public Function ThresholdSweep() As Point()
            Dim candidates As Double() = _scores.Distinct().OrderByDescending(Function(v) v).ToArray()
            Dim points As New List(Of Point)
            Dim totalPositive As Integer = Positives
            Dim totalNegative As Integer = Negatives

            For Each threshold As Double In candidates
                Dim tp As Integer = 0
                Dim fp As Integer = 0

                For i As Integer = 0 To _scores.Length - 1
                    If _scores(i) >= threshold Then
                        If _labels(i) Then
                            tp += 1
                        Else
                            fp += 1
                        End If
                    End If
                Next

                Call points.Add(New Point With {
                    .Threshold = threshold,
                    .TruePositiveRate = Metrics.SafeDivide(tp, totalPositive),
                    .FalsePositiveRate = Metrics.SafeDivide(fp, totalNegative),
                    .Precision = Metrics.SafeDivide(tp, tp + fp)
                })
            Next

            Return points.ToArray()
        End Function

        ''' <summary>
        ''' ROC 曲线下面积(梯形法)
        ''' </summary>
        Public Function AucRoc() As Double
            Dim points As Point() = ThresholdSweep() _
                .OrderBy(Function(p) p.FalsePositiveRate) _
                .ThenBy(Function(p) p.TruePositiveRate) _
                .ToArray()

            Dim area As Double = 0.0
            Dim previousFpr As Double = 0.0
            Dim previousTpr As Double = 0.0

            For Each point As Point In points
                area += (point.FalsePositiveRate - previousFpr) * (point.TruePositiveRate + previousTpr) / 2.0
                previousFpr = point.FalsePositiveRate
                previousTpr = point.TruePositiveRate
            Next

            ' 曲线右端补到 (1, 1)
            area += (1.0 - previousFpr) * (1.0 + previousTpr) / 2.0

            Return area
        End Function

        ''' <summary>
        ''' 精确率–召回率曲线下面积(阶梯法，等价于平均精确率 AP)
        ''' </summary>
        ''' <remarks>
        ''' 正负样本严重不平衡时这是更敏感的指标；随机猜测的期望值等于正样本比例，
        ''' 而不是 ROC 的 0.5。
        ''' </remarks>
        Public Function AucPr() As Double
            Dim totalPositive As Integer = Positives

            If totalPositive = 0 Then
                Return 0.0
            End If

            Dim ordered As Integer() = Enumerable.Range(0, _scores.Length) _
                .OrderByDescending(Function(i) _scores(i)) _
                .ToArray()
            Dim tp As Integer = 0
            Dim fp As Integer = 0
            Dim previousRecall As Double = 0.0
            Dim area As Double = 0.0

            For Each index As Integer In ordered
                If _labels(index) Then
                    tp += 1
                Else
                    fp += 1
                End If

                Dim recall As Double = tp / CDbl(totalPositive)
                Dim precision As Double = Metrics.SafeDivide(tp, tp + fp)

                area += (recall - previousRecall) * precision
                previousRecall = recall
            Next

            Return area
        End Function

        ''' <summary>
        ''' 在给定的灵敏度处读取阳性预测值(PPV)
        ''' </summary>
        ''' <param name="sensitivity">目标灵敏度</param>
        Public Function PpvAtSensitivity(sensitivity As Double) As Double
            Dim points As Point() = ThresholdSweep() _
                .OrderBy(Function(p) p.TruePositiveRate) _
                .ToArray()
            Dim best As Double = 0.0
            Dim found As Boolean = False

            For Each point As Point In points
                If point.TruePositiveRate >= sensitivity Then
                    If Not found OrElse point.Precision > best Then
                        best = point.Precision
                        found = True
                    End If
                End If
            Next

            Return best
        End Function

        ''' <summary>
        ''' 概率校准的分箱统计
        ''' </summary>
        ''' <param name="bins">分箱个数</param>
        ''' <remarks>
        ''' 校准曲线把预测概率与「实际观测到的正样本比例」画在一起：理想情况下两者重合。
        ''' 对于需要把输出概率用于下游决策的场合，校准比单纯的区分度更重要。
        ''' </remarks>
        Public Function Calibration(Optional bins As Integer = 10) As CalibrationBin()
            If bins < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(bins), "the number of bins must be positive")
            End If

            Dim sum As Double() = New Double(bins - 1) {}
            Dim counts As Integer() = New Integer(bins - 1) {}
            Dim positive As Integer() = New Integer(bins - 1) {}

            For i As Integer = 0 To _scores.Length - 1
                Dim p As Double = _scores(i)

                If p < 0 Then
                    p = 0.0
                ElseIf p > 1 Then
                    p = 1.0
                End If

                Dim index As Integer = CInt(Math.Floor(p * bins))

                If index >= bins Then
                    index = bins - 1
                End If

                sum(index) += p
                counts(index) += 1

                If _labels(i) Then
                    positive(index) += 1
                End If
            Next

            Dim result As New List(Of CalibrationBin)

            For i As Integer = 0 To bins - 1
                If counts(i) = 0 Then
                    Continue For
                End If

                Call result.Add(New CalibrationBin With {
                    .Lower = i / CDbl(bins),
                    .Upper = (i + 1) / CDbl(bins),
                    .Predicted = sum(i) / counts(i),
                    .Observed = positive(i) / CDbl(counts(i)),
                    .Count = counts(i)
                })
            Next

            Return result.ToArray()
        End Function

        ''' <summary>
        ''' 导出为 CSV 文本，便于在电子表格或者绘图脚本之中复核
        ''' </summary>
        Public Function ToTable() As String
            Dim sb As New StringBuilder

            Call sb.AppendLine("threshold,falsePositiveRate,truePositiveRate,precision")

            For Each point As Point In ThresholdSweep()
                Call sb.AppendLine(String.Join(",", {
                    point.Threshold.ToString("0.000000"),
                    point.FalsePositiveRate.ToString("0.000000"),
                    point.TruePositiveRate.ToString("0.000000"),
                    point.Precision.ToString("0.000000")
                }))
            Next

            Return sb.ToString()
        End Function
    End Class
End Namespace
