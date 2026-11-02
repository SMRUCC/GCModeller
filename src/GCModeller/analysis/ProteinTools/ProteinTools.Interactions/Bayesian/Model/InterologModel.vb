Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment

Namespace Bayesian.Model

    ''' <summary>
    ''' 直系同源(interolog)迁移模型。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 配对联合比对的质量决定了共进化信号的上限：当某个蛋白在目标物种之中缺少足够多的同源序列时，
    ''' 相依树与似然比都几乎无法提供信息(readme 第 7 节的第 1 条经验提示)。
    ''' 此时可以退化为「互作关系跨物种保守」的假设，把已知物种之中的互作经过直系同源映射
    ''' 转移到目标基因组，这就是 interolog 方法。
    ''' </para>
    ''' <para>
    ''' 论文在系统发育置换检验(第 5 节)之中给出的 orthology 判据被直接复用：
    ''' <b>互为 best reciprocal hit</b>、<b>比对覆盖度 ≥ 80%</b>、<b>氨基酸一致性 ≥ 80%</b>。
    ''' </para>
    ''' </remarks>
    Public Class InterologModel

        ''' <summary>
        ''' 一条同源关系
        ''' </summary>
        Public Class OrthologPair

            ''' <summary>
            ''' 源蛋白编号
            ''' </summary>
            Public Property Query As String
            ''' <summary>
            ''' 目标蛋白编号
            ''' </summary>
            Public Property Subject As String
            ''' <summary>
            ''' 比对覆盖度(相对较长的那条序列)
            ''' </summary>
            Public Property Coverage As Double
            ''' <summary>
            ''' 氨基酸一致性
            ''' </summary>
            Public Property Identity As Double
            ''' <summary>
            ''' 是否为互为最优的 reciprocal best hit
            ''' </summary>
            Public Property ReciprocalBest As Boolean

            Public Overrides Function ToString() As String
                Return $"{Query} <-> {Subject} (cov={Coverage:0.###}, id={Identity:0.###})"
            End Function
        End Class

        ''' <summary>
        ''' 迁移得到的互作关系
        ''' </summary>
        Public Class TransferredInteraction

            ''' <summary>
            ''' 目标基因组之中的第一个蛋白
            ''' </summary>
            Public Property ProteinA As String
            ''' <summary>
            ''' 目标基因组之中的第二个蛋白
            ''' </summary>
            Public Property ProteinB As String
            ''' <summary>
            ''' 源互作对
            ''' </summary>
            Public Property SourceA As String
            ''' <summary>
            ''' 源互作对的第二个成员
            ''' </summary>
            Public Property SourceB As String
            ''' <summary>
            ''' 两侧直系同源关系之中较小的覆盖度
            ''' </summary>
            Public Property Support As Double

            Public Overrides Function ToString() As String
                Return $"{ProteinA} -- {ProteinB} (interolog of {SourceA}/{SourceB}, support={Support:0.###})"
            End Function
        End Class

        Private _minCoverage As Double = 0.8
        Private _minIdentity As Double = 0.8

        ''' <summary>
        ''' 比对覆盖度阈值
        ''' </summary>
        Public Property MinCoverage As Double
            Get
                Return _minCoverage
            End Get
            Set(value As Double)
                _minCoverage = value
            End Set
        End Property

        ''' <summary>
        ''' 氨基酸一致性阈值
        ''' </summary>
        Public Property MinIdentity As Double
            Get
                Return _minIdentity
            End Get
            Set(value As Double)
                _minIdentity = value
            End Set
        End Property

        ''' <summary>
        ''' 由同源搜索结果之中筛出满足阈值条件的直系同源关系
        ''' </summary>
        ''' <param name="hits">同源关系</param>
        Public Function Filter(hits As IEnumerable(Of OrthologPair)) As OrthologPair()
            If hits Is Nothing Then
                Return New OrthologPair() {}
            End If

            Return hits _
                .Where(Function(h) h IsNot Nothing) _
                .Where(Function(h) h.Coverage >= _minCoverage) _
                .Where(Function(h) h.Identity >= _minIdentity) _
                .OrderByDescending(Function(h) h.Identity) _
                .ToArray()
        End Function

        ''' <summary>
        ''' 把源物种之中的互作关系迁移到目标基因组
        ''' </summary>
        ''' <param name="interactions">源物种的已知互作对</param>
        ''' <param name="orthologs">已经通过阈值筛选的直系同源关系</param>
        Public Function Transfer(interactions As IEnumerable(Of (a As String, b As String)),
                                 orthologs As IEnumerable(Of OrthologPair)) As TransferredInteraction()

            If interactions Is Nothing OrElse orthologs Is Nothing Then
                Return New TransferredInteraction() {}
            End If

            Dim bySource As New Dictionary(Of String, OrthologPair)(StringComparer.Ordinal)
            Dim result As New List(Of TransferredInteraction)

            For Each hit As OrthologPair In orthologs
                If Not bySource.ContainsKey(hit.Subject) Then
                    bySource(hit.Subject) = hit
                End If
            Next

            For Each pair In interactions
                Dim left As OrthologPair = Nothing
                Dim right As OrthologPair = Nothing

                If Not bySource.TryGetValue(pair.a, left) Then
                    Continue For
                End If
                If Not bySource.TryGetValue(pair.b, right) Then
                    Continue For
                End If

                Call result.Add(New TransferredInteraction With {
                    .ProteinA = left.Query,
                    .ProteinB = right.Query,
                    .SourceA = pair.a,
                    .SourceB = pair.b,
                    .Support = Math.Min(left.Coverage, right.Coverage)
                })
            Next

            Return result.OrderByDescending(Function(t) t.Support).ToArray()
        End Function

        ''' <summary>
        ''' 判断当前的配对联合比对是否「信息量不足」，从而应当退化为 interolog 转移。
        ''' </summary>
        ''' <param name="alignment">配对联合比对</param>
        ''' <param name="minSequences">最少需要的同源序列条数</param>
        ''' <param name="minPositiveFraction">似然比为正的列对比例下限</param>
        Public Shared Function ShouldFallback(alignment As JointAlignment,
                                              Optional minSequences As Integer = 10,
                                              Optional minPositiveFraction As Double = 0.05) As Boolean

            If alignment Is Nothing Then
                Return True
            End If
            If alignment.Rows < minSequences Then
                Return True
            End If

            Dim counts As New ColumnCounts(alignment, includeWithinBlockPairs:=False)
            Dim logR As New Statistics.LikelihoodRatioMatrix(counts)
            Dim fraction As Double = logR.PositiveFraction()

            DebugAudit.Mark("interolog.fallback_check", $"rows={alignment.Rows}, positiveFraction={fraction:0.0000}")

            Return fraction < minPositiveFraction
        End Function
    End Class
End Namespace
