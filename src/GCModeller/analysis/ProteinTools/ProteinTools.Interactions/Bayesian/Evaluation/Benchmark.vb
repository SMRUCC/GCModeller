Imports System.Text
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Model

Namespace Bayesian.Evaluation

    ''' <summary>
    ''' 金标准集合之上的评估与消融实验编排。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' readme 第 5 节要求至少做三件事：
    ''' </para>
    ''' <list type="number">
    ''' <item><description>报告 AUC-ROC、AUC-PR、F1 与校准度；</description></item>
    ''' <item><description>
    ''' 做消融实验：单独使用互信息(APC 校正) / 单独使用似然比 / 贝叶斯融合(含依赖树)，
    ''' 以验证贝叶斯融合带来的增益；
    ''' </description></item>
    ''' <item><description>
    ''' 与金标准结构或者已知互作集合比对，报告 top-L/5 精度。
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' 本类把「模型 → 打分」与「打分 → 指标」这两步解耦，因此既可以评估真实的预测结果，
    ''' 也可以直接喂入任意打分序列来做方法之间的横向比较。
    ''' </para>
    ''' </remarks>
    Public Class Benchmark

        ''' <summary>
        ''' 参与比较的打分特征
        ''' </summary>
        Public Enum FeatureMode
            ''' <summary>
            ''' 论文的完整打分 <c>X(S_k, S_r)</c>，依赖 <c>T*</c>
            ''' </summary>
            LikelihoodRatioTree = 0
            ''' <summary>
            ''' 只取 top-K 跨家族列对的 <c>ln R</c> 之和
            ''' </summary>
            LikelihoodRatioTopK = 1
            ''' <summary>
            ''' APC 校正之后的互信息聚合，作为「仅共进化」的基线
            ''' </summary>
            MutualInformationApc = 2
        End Enum

        ''' <summary>
        ''' 带标签的一个样本
        ''' </summary>
        Public Class LabeledPair
            ''' <summary>
            ''' 两个蛋白互作的联合比对
            ''' </summary>
            Public Property Alignment As Alignment.JointAlignment
            ''' <summary>
            ''' 是否真的互作
            ''' </summary>
            Public Property Label As Boolean

            Public Sub New(alignment As Alignment.JointAlignment, label As Boolean)
                Me.Alignment = alignment
                Me.Label = label
            End Sub
        End Class

        ''' <summary>
        ''' 一个特征模式之下的评估报告
        ''' </summary>
        Public Class Report

            ''' <summary>
            ''' 被评估的特征模式
            ''' </summary>
            Public Property Mode As FeatureMode
            ''' <summary>
            ''' 参与评估的样本数
            ''' </summary>
            Public Property Samples As Integer
            ''' <summary>
            ''' 正样本数
            ''' </summary>
            Public Property Positives As Integer
            ''' <summary>
            ''' ROC 曲线下面积
            ''' </summary>
            Public Property AucRoc As Double
            ''' <summary>
            ''' PR 曲线下面积
            ''' </summary>
            Public Property AucPr As Double
            ''' <summary>
            ''' 指定阈值之下的混淆矩阵
            ''' </summary>
            Public Property Confusion As Metrics.ConfusionMatrix
            ''' <summary>
            ''' 指定灵敏度之下的阳性预测值
            ''' </summary>
            Public Property Ppv60 As Double
            ''' <summary>
            ''' 指定灵敏度之下的阳性预测值
            ''' </summary>
            Public Property Ppv75 As Double

            Public Overrides Function ToString() As String
                Return $"{Mode}: AUC-ROC={AucRoc:0.000}, AUC-PR={AucPr:0.000}, F1={If(Confusion Is Nothing, 0, Confusion.F1):0.000}, " &
                    $"PPV@60%={Ppv60:0.000}, PPV@75%={Ppv75:0.000}"
            End Function
        End Class

        ''' <summary>
        ''' 直接在打分序列之上评估
        ''' </summary>
        ''' <param name="scores">模型打分</param>
        ''' <param name="labels">真实标签</param>
        ''' <param name="threshold">判定阈值</param>
        ''' <param name="mode">特征模式标签(仅用于报告)</param>
        Public Shared Function Evaluate(scores As Double(),
                                        labels As Boolean(),
                                        Optional threshold As Double = 1.0,
                                        Optional mode As FeatureMode = FeatureMode.LikelihoodRatioTree) As Report

            Dim curve As New RocPrCurve(scores, labels)

            Return New Report With {
                .Mode = mode,
                .Samples = scores.Length,
                .Positives = curve.Positives,
                .AucRoc = curve.AucRoc(),
                .AucPr = curve.AucPr(),
                .Confusion = Metrics.Confusion(scores, labels, threshold),
                .Ppv60 = curve.PpvAtSensitivity(0.6),
                .Ppv75 = curve.PpvAtSensitivity(0.75)
            }
        End Function

        ''' <summary>
        ''' 在一组带标签的联合比对之上按指定特征模式打分并评估
        ''' </summary>
        public shared Function Evaluate(pairs As IEnumerable(Of LabeledPair),
                                        Optional mode As FeatureMode = FeatureMode.LikelihoodRatioTree,
                                        Optional options As PpiModelOptions = Nothing,
                                        Optional threshold As Double = 1.0) As Report

            If pairs Is Nothing Then
                Throw New ArgumentNullException(NameOf(pairs))
            End If

            Dim list As LabeledPair() = pairs.ToArray()

            If list.Length = 0 Then
                Throw New ArgumentException("the labelled set can not be empty", NameOf(pairs))
            End If

            Dim usedOptions As PpiModelOptions = If(options, New PpiModelOptions())
            Dim scores As Double() = New Double(list.Length - 1) {}
            Dim labels As Boolean() = New Boolean(list.Length - 1) {}

            For i As Integer = 0 To list.Length - 1
                Dim model As New PpiBayesianModel(list(i).Alignment, usedOptions)

                Select Case mode
                    Case FeatureMode.MutualInformationApc
                        scores(i) = ApcScore(model)
                    Case FeatureMode.LikelihoodRatioTopK
                        scores(i) = New ResidueContacts(model).AggregateScore(usedOptions.TopResiduePairs)
                    Case Else
                        ' 论文的完整打分：直接用模型自身的联合比对作为查询序列
                        scores(i) = ScoreSelfPair(model)
                End Select

                labels(i) = list(i).Label
            Next

            Return Evaluate(scores, labels, threshold, mode)
        End Function

        ''' <summary>
        ''' 计算模型自身配对(即联合比对每一行的真实配对)的论文打分
        ''' </summary>
        ''' <remarks>
        ''' 对于一个「蛋白对」样本，最直接的可比较量是模型在该联合比对之下的对数边缘似然，
        ''' 它与静态打分 <c>X</c> 单调等价(共享同样的 <c>T*</c> 与平滑计数)，
        ''' 而且在只有少量列的时候数值更稳定。
        ''' </remarks>
        Public Shared Function ScoreSelfPair(model As PpiBayesianModel) As Double
            Return model.LogEvidence() - model.LogSingleColumnBase
        End Function

        ''' <summary>
        ''' APC 校正之后的互信息聚合分数
        ''' </summary>
        Public Shared Function ApcScore(model As PpiBayesianModel) As Double
            Dim mi As New Statistics.MutualInformation(model.Counts)
            Dim corrected As Double(,) = mi.AverageProductCorrection()
            Dim alignment As Alignment.JointAlignment = model.Alignment
            Dim total As Double = 0.0

            For i As Integer = 0 To alignment.LengthA - 1
                For j As Integer = alignment.LengthA To alignment.Length - 1
                    Dim value As Double = corrected(i, j)

                    If value > 0 Then
                        total += value
                    End If
                Next
            Next

            Return total
        End Function

        ''' <summary>
        ''' 依次运行三种特征模式，产出一份消融对比表
        ''' </summary>
        public shared Function Ablation(pairs As IEnumerable(Of LabeledPair),
                                        Optional options As PpiModelOptions = Nothing) As Report()

            Dim result As New List(Of Report)

            For Each mode As FeatureMode In {FeatureMode.MutualInformationApc,
                                             FeatureMode.LikelihoodRatioTopK,
                                             FeatureMode.LikelihoodRatioTree}
                Call result.Add(Evaluate(pairs, mode, options))
            Next

            Return result.ToArray()
        End Function

        ''' <summary>
        ''' 把多份报告渲染为对齐的文本表格
        ''' </summary>
        public shared Function ToTable(reports As IEnumerable(Of Report)) As String
            Dim sb As New StringBuilder
            Dim rows As Report() = reports.ToArray()

            Call sb.AppendLine("mode,samples,positives,aucRoc,aucPr,f1,ppvAt60,ppvAt75")

            For Each report As Report In rows
                Call sb.AppendLine(String.Join(",", {
                    report.Mode.ToString(),
                    report.Samples.ToString(),
                    report.Positives.ToString(),
                    report.AucRoc.ToString("0.0000"),
                    report.AucPr.ToString("0.0000"),
                    If(report.Confusion Is Nothing, "0", report.Confusion.F1.ToString("0.0000")),
                    report.Ppv60.ToString("0.0000"),
                    report.Ppv75.ToString("0.0000")
                }))
            Next

            Return sb.ToString()
        End Function
    End Class
End Namespace
