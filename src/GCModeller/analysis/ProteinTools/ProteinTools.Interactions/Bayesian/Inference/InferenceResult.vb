Imports System.Text
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment

Namespace Bayesian.Inference

    ''' <summary>
    ''' Gibbs 分配采样的结果容器。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 论文明确指出：采样频率 <c>f(x, y)</c> <b>不能</b>直接当作互作的后验概率。
    ''' 一个激酶以 1/4 的频率分别与 4 个响应调节蛋白配对，可能恰恰说明它能与这四者都互作
    ''' (orphan 的情形允许多个伙伴)，而不是说明它只有 1/4 的概率互作。
    ''' 因此本结果类同时提供两个视角：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' <see cref="CognateProbability"/>：某一行「保持原始配对」的采样频率，
    ''' 用于判定 cognate 关系是否成立。
    ''' </description></item>
    ''' <item><description>
    ''' <see cref="Partners"/>：该行的 A 成员与各个候选 B 成员的配对频率，用于发现 orphan 的多个伙伴。
    ''' </description></item>
    ''' </list>
    ''' </remarks>
    Public Class InferenceResult

        ''' <summary>
        ''' 一个候选伙伴的配对频率
        ''' </summary>
        Public Class Partner

            ''' <summary>
            ''' 家族 A 一侧的成员(序列名)
            ''' </summary>
            Public Property ProteinA As String
            ''' <summary>
            ''' 家族 B 一侧的成员(序列名)
            ''' </summary>
            Public Property ProteinB As String
            ''' <summary>
            ''' 所属基因组
            ''' </summary>
            Public Property Genome As String
            ''' <summary>
            ''' 采样频率
            ''' </summary>
            Public Property Frequency As Double
            ''' <summary>
            ''' 该频率在候选伙伴之中的排名(从 1 开始)
            ''' </summary>
            Public Property Rank As Integer

            Public Overrides Function ToString() As String
                Return $"{ProteinA} -- {ProteinB} : {Frequency:0.0000} (rank {Rank})"
            End Function
        End Class

        ''' <summary>
        ''' 每一行「保持原始配对」的采样频率
        ''' </summary>
        Public Property CognateProbability As Double()
        ''' <summary>
        ''' 每一行对应的全部候选伙伴及其频率
        ''' </summary>
        Public Property Partners As List(Of Partner)()
        ''' <summary>
        ''' 每一行的基因组编号
        ''' </summary>
        Public Property Genomes As String()
        ''' <summary>
        ''' 每一行家族 A 一侧的序列名
        ''' </summary>
        Public Property NamesA As String()
        ''' <summary>
        ''' 每一行家族 B 一侧的序列名
        ''' </summary>
        Public Property NamesB As String()
        ''' <summary>
        ''' 独立运行的次数
        ''' </summary>
        Public Property Runs As Integer
        ''' <summary>
        ''' 每一次运行的迭代次数
        ''' </summary>
        Public Property Iterations As Integer
        ''' <summary>
        ''' 每一次独立运行最终的对数似然，用于观察收敛情况
        ''' </summary>
        Public Property RunLogEvidence As Double()
        ''' <summary>
        ''' 被采样的基因组个数
        ''' </summary>
        Public Property GenomeCount As Integer

        ''' <summary>
        ''' 按照 cognate 概率从高到低排序的行下标
        ''' </summary>
        Public Function Ranking() As Integer()
            If CognateProbability Is Nothing Then
                Return New Integer() {}
            End If

            Return Enumerable.Range(0, CognateProbability.Length) _
                .OrderByDescending(Function(i) CognateProbability(i)) _
                .ToArray()
        End Function

        ''' <summary>
        ''' 导出为可写入 CSV 的表格文本
        ''' </summary>
        Public Function ToTable() As String
            Dim sb As New StringBuilder

            Call sb.AppendLine("genome,proteinA,proteinB,cognateProbability,bestPartner,bestFrequency")

            If CognateProbability Is Nothing Then
                Return sb.ToString()
            End If

            For i As Integer = 0 To CognateProbability.Length - 1
                Dim best As Partner = Nothing

                If Partners IsNot Nothing AndAlso Partners(i) IsNot Nothing AndAlso Partners(i).Count > 0 Then
                    best = Partners(i)(0)
                End If

                Call sb.AppendLine(String.Join(",", {
                    Quote(Genomes(i)),
                    Quote(NamesA(i)),
                    Quote(NamesB(i)),
                    CognateProbability(i).ToString("0.000000"),
                    Quote(If(best Is Nothing, "", best.ProteinB)),
                    If(best Is Nothing, "0", best.Frequency.ToString("0.000000"))
                }))
            Next

            Return sb.ToString()
        End Function

        Private Shared Function Quote(text As String) As String
            Return """" & (If(text, "")).Replace("""", """""") & """"
        End Function

        Public Overrides Function ToString() As String
            Dim count As Integer = If(CognateProbability Is Nothing, 0, CognateProbability.Length)

            Return $"InferenceResult[{count} candidate pairs, {Runs} run(s) x {Iterations} iteration(s)]"
        End Function
    End Class
End Namespace
