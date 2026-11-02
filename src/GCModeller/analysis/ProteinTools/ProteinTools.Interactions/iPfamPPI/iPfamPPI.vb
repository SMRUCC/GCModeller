Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Model
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.SwissTCS

Namespace iPfamPPI

    ''' <summary>
    ''' 基于 iPfam 结构域互作注释与直系同源转移的互作注释模块。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 比较基因组学之中的直系同源只能说明「两个蛋白在其他物种里同样有可能互作」，
    ''' 而互作关系本身必须来自实验或者数据库。iPfam 提供的正是后者：
    ''' 它记录了哪些 Pfam 结构域在真实的 PDB 复合物之中彼此接触。
    ''' </para>
    ''' <para>
    ''' 因此本模块的定位与论文的贝叶斯网络方法是互补的：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' 贝叶斯网络方法只依赖序列比对，可以在没有任何实验数据的前提下做 ab initio 预测；
    ''' </description></item>
    ''' <item><description>
    ''' iPfam + interolog 走的是「结构证据 + 同源保守」的路线，覆盖度高但只能注释已有证据的家族。
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' 两个方向的预测结果最终都汇聚为 <see cref="CrossTalks"/>，
    ''' 这样既可以直接与 SwissTCS 的实验概率比较，也可以复用现有的网络可视化代码。
    ''' </para>
    ''' </remarks>
    Public Module iPfamPPI

        ''' <summary>
        ''' 一条基因的结构域注释
        ''' </summary>
        Public Class DomainAnnotation

            ''' <summary>
            ''' 基因编号
            ''' </summary>
            Public Property GeneId As String
            ''' <summary>
            ''' 该基因包含的 Pfam 登录号集合
            ''' </summary>
            Public Property Domains As String()

            Public Sub New(geneId As String, domains As String())
                Me.GeneId = geneId
                Me.Domains = domains
            End Sub
        End Class

        ''' <summary>
        ''' 读取 iPfam 的「结构域–结构域」互作表
        ''' </summary>
        Public Function LoadRecords(path As String) As iPfamRecord()
            Return iPfamRecord.Load(path)
        End Function

        ''' <summary>
        ''' 由结构域注释与 iPfam 记录推断基因层次的互作关系。
        ''' </summary>
        ''' <param name="annotations">目标基因组之中每个基因所含的 Pfam 结构域</param>
        ''' <param name="records">iPfam 的结构域互作记录</param>
        ''' <param name="minScore">结构域互作记录的最低得分</param>
        ''' <remarks>
        ''' 只要基因 A 含有结构域 <c>dA</c>、基因 B 含有结构域 <c>dB</c>，并且
        ''' <c>(dA, dB)</c> 在 iPfam 之中有互作记录，就生成一条候选互作。
        ''' 多个结构域对支持同一对基因时，取支持的最强得分作为证据强度。
        ''' </remarks>
        Public Function Predict(annotations As IEnumerable(Of DomainAnnotation),
                                records As IEnumerable(Of iPfamRecord),
                                Optional minScore As Double = 0.0) As CrossTalks()

            If annotations Is Nothing OrElse records Is Nothing Then
                Return New CrossTalks() {}
            End If

            Dim index As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)
            Dim genes As New List(Of String)

            For Each annotation As DomainAnnotation In annotations
                If annotation Is Nothing OrElse String.IsNullOrEmpty(annotation.GeneId) Then
                    Continue For
                End If

                Dim list As List(Of String) = Nothing

                If Not index.TryGetValue(annotation.GeneId, list) Then
                    list = New List(Of String)
                    index(annotation.GeneId) = list
                    Call genes.Add(annotation.GeneId)
                End If

                If annotation.Domains IsNot Nothing Then
                    Call list.AddRange(annotation.Domains.Where(Function(d) Not String.IsNullOrEmpty(d)))
                End If
            Next

            ' 结构域 -> 含有该结构域的基因
            Dim byDomain As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

            For Each gene As String In genes
                For Each domain As String In index(gene).Distinct()
                    Dim list As List(Of String) = Nothing

                    If Not byDomain.TryGetValue(domain, list) Then
                        list = New List(Of String)
                        byDomain(domain) = list
                    End If

                    Call list.Add(gene)
                Next
            Next

            Dim best As New Dictionary(Of String, CrossTalks)(StringComparer.Ordinal)
            Dim duplicate As New HashSet(Of String)

            For Each record As iPfamRecord In records
                If record Is Nothing OrElse record.Score < minScore Then
                    Continue For
                End If

                Dim left As List(Of String) = Nothing
                Dim right As List(Of String) = Nothing

                If Not byDomain.TryGetValue(record.PfamA, left) Then
                    Continue For
                End If
                If Not byDomain.TryGetValue(record.PfamB, right) Then
                    Continue For
                End If

                For Each a As String In left
                    For Each b As String In right
                        If String.Equals(a, b) Then
                            Continue For
                        End If

                        Dim key As String = PairKey(a, b)

                        If duplicate.Contains(key) Then
                            Continue For
                        End If

                        Dim score As Double = Logistic(record.Score)

                        Call best.Add(key, New CrossTalks With {
                            .Kinase = a,
                            .Regulator = b,
                            .Score = score,
                            .Probability = score,
                            .Source = "ipfam",
                            .TopContact = record.PfamA & "/" & record.PfamB,
                            .InteractionType = "domain-domain"
                        })
                        Call duplicate.Add(key)
                    Next
                Next
            Next

            Return best.Values.OrderByDescending(Function(c) c.Probability).ToArray()
        End Function

        ''' <summary>
        ''' 用直系同源关系把已知互作迁移到目标基因组
        ''' </summary>
        ''' <param name="known">已知互作对</param>
        ''' <param name="orthologs">通过阈值筛选的直系同源关系</param>
        Public Function TransferWithInterolog(known As IEnumerable(Of CrossTalks),
                                              orthologs As IEnumerable(Of InterologModel.OrthologPair)) As CrossTalks()

            If known Is Nothing Then
                Return New CrossTalks() {}
            End If

            Dim model As New InterologModel()
            Dim source As New List(Of (a As String, b As String))

            For Each item As CrossTalks In known
                Dim left As String = item.TrimedKinaseId()
                Dim right As String = item.TrimedRegulatorId()

                If Not String.IsNullOrEmpty(left) AndAlso Not String.IsNullOrEmpty(right) Then
                    Call source.Add((left, right))
                End If
            Next

            Dim transferred As InterologModel.TransferredInteraction() = model.Transfer(source, orthologs)
            Dim result As New List(Of CrossTalks)

            For Each item As InterologModel.TransferredInteraction In transferred
                Call result.Add(New CrossTalks With {
                    .Kinase = item.ProteinA,
                    .Regulator = item.ProteinB,
                    .Probability = item.Support,
                    .Score = Double.NaN,
                    .Source = "interolog",
                    .TopContact = item.SourceA & "/" & item.SourceB,
                    .InteractionType = "interolog"
                })
            Next

            DebugAudit.Mark("ipfam.interolog", $"{result.Count} interactions transferred from {source.Count} source pairs")

            Return result.ToArray()
        End Function

        ''' <summary>
        ''' 把 iPfam 的原始得分(接触原子对数等)压缩到 <c>[0, 1]</c>
        ''' </summary>
        ''' <param name="score">原始得分</param>
        ''' <remarks>
        ''' iPfam 的得分没有统一的量纲，这里使用一个平滑的 logistic 变换，
        ''' 只要求「得分越高、概率越大」这一单调性。
        ''' </remarks>
        Public Function Logistic(score As Double) As Double
            If Double.IsNaN(score) Then
                Return 0.0
            End If

            Return 1.0 / (1.0 + Math.Exp(-score / 10.0))
        End Function

        ''' <summary>
        ''' 生成无序互作对的键，保证 <c>(a,b)</c> 与 <c>(b,a)</c> 相同
        ''' </summary>
        Public Function PairKey(a As String, b As String) As String
            If String.CompareOrdinal(a, b) <= 0 Then
                Return a & "|" & b
            Else
                Return b & "|" & a
            End If
        End Function

        ''' <summary>
        ''' 把预测结果导出为与 SwissTCS 兼容的 CSV 文本
        ''' </summary>
        Public Function ToTable(interactions As IEnumerable(Of CrossTalks)) As String
            Dim sb As New StringBuilder

            Call sb.AppendLine("kinase,regulator,probability,score,source")

            For Each item As CrossTalks In interactions
                Call sb.AppendLine(String.Join(",", {
                    item.Kinase,
                    item.Regulator,
                    item.Probability.ToString("0.000000"),
                    If(Double.IsNaN(item.Score), "", item.Score.ToString("0.000000")),
                    If(item.Source, "")
                }))
            Next

            Return sb.ToString()
        End Function
    End Module
End Namespace
