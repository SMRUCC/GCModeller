Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math
Imports RegulationEdge = SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork
Imports EffectorRole = SMRUCC.genomics.Analysis.BNLearn.Effector

Namespace GRN

    ''' <summary>
    ''' A directed regulatory interaction edge of the gene regulatory network.
    ''' (基因调控网络之中的一条有向调控边)
    ''' </summary>
    ''' <remarks>
    ''' 边的权重取净调控弹性 g_ij - h_ij：为正表示源基因激活靶基因，
    ''' 为负表示源基因抑制靶基因，零表示不存在调控关系。
    ''' </remarks>
    Public Class RegulatoryEdge

        ''' <summary>
        ''' 调控因子的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String
        ''' <summary>
        ''' 靶基因的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property target As String
        ''' <summary>
        ''' 净调控弹性 g_ij - h_ij
        ''' </summary>
        ''' <returns></returns>
        Public Property weight As Double
        ''' <summary>
        ''' 合成项之中的动力学阶 g_ij
        ''' </summary>
        ''' <returns></returns>
        Public Property g As Double
        ''' <summary>
        ''' 降解项之中的动力学阶 h_ij
        ''' </summary>
        ''' <returns></returns>
        Public Property h As Double
        ''' <summary>
        ''' 本条调控关系是否在先验知识之中有记录
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorSupported As Boolean = False
        ''' <summary>
        ''' 先验知识所记录的调控方向（Activator/Inhibitor/Unknown）
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorType As String = "Unknown"
        ''' <summary>
        ''' 先验知识所记录的置信度（0~1，零表示没有先验支持）
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorConfidence As Double = 0
        ''' <summary>
        ''' 稀疏化排序时施加的先验加权（0 表示没有先验支持）
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorWeight As Double = 0

        ''' <summary>
        ''' 调控的类型：激活或者抑制
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property interaction As String
            Get
                If weight > 0 Then
                    Return "activates"
                ElseIf weight < 0 Then
                    Return "inhibits"
                Else
                    Return "none"
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{source} {interaction} {target} (net={weight.ToString("G4")})"
        End Function
    End Class

    ''' <summary>
    ''' The evaluation of the prior knowledge against the recovered network topology.
    ''' (先验知识与拟合得到的网络拓扑之间的对照指标)
    ''' </summary>
    Public Class PriorMetrics

        ''' <summary>
        ''' 恢复出来的调控边总数
        ''' </summary>
        ''' <returns></returns>
        Public Property RecoveredEdges As Integer = 0
        ''' <summary>
        ''' 其中被先验知识所支持的边的数量
        ''' </summary>
        ''' <returns></returns>
        Public Property SupportedEdges As Integer = 0
        ''' <summary>
        ''' 有先验支持并且调控方向一致的边的数量
        ''' </summary>
        ''' <returns></returns>
        Public Property SignConsistent As Integer = 0
        ''' <summary>
        ''' 有先验支持但是调控方向相反的边
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SignViolations As New List(Of RegulatoryEdge)
        ''' <summary>
        ''' 没有先验支持的边（新发现的调控关系）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UnsupportedEdges As New List(Of RegulatoryEdge)
        ''' <summary>
        ''' 先验知识之中被成功复现出来的边数量
        ''' </summary>
        ''' <returns></returns>
        Public Property ConfirmedPriorEdges As Integer = 0
        ''' <summary>
        ''' 先验知识之中没有被复现出来的边
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MissedPriorEdges As New List(Of RegulationEdge)

        ''' <summary>
        ''' 先验覆盖率：恢复出来的边之中有多少比例是有先验支持的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Precision As Double
            Get
                If RecoveredEdges = 0 Then Return 0
                Return SupportedEdges / RecoveredEdges
            End Get
        End Property

        ''' <summary>
        ''' 先验方向的整体一致率
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SignConsistency As Double
            Get
                Dim total As Integer = SignConsistent + SignViolations.Count

                If total = 0 Then Return 0
                Return SignConsistent / total
            End Get
        End Property

        ''' <summary>
        ''' 生成可读的对照报告
        ''' </summary>
        ''' <returns></returns>
        Public Function Summary() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"prior supported edges : {SupportedEdges}/{RecoveredEdges} ({Precision.ToString("P1")})")
            Call sb.AppendLine($"new edges (no prior)  : {UnsupportedEdges.Count}")
            Call sb.AppendLine($"prior sign consistent : {SignConsistent}/{SignConsistent + SignViolations.Count} ({SignConsistency.ToString("P1")})")
            Call sb.AppendLine($"prior edges recovered : {ConfirmedPriorEdges}/{ConfirmedPriorEdges + MissedPriorEdges.Count}")

            If SignViolations.Count > 0 Then
                Call sb.AppendLine("sign violations:")

                For Each edge As RegulatoryEdge In SignViolations
                    Call sb.AppendLine($"  {edge.source} -> {edge.target}: prior={edge.PriorType}, net={edge.weight.ToString("G4")}")
                Next
            End If

            Return sb.ToString()
        End Function
    End Class

    ''' <summary>
    ''' The network topology inference of the fitted S-system parameters.
    ''' (由拟合参数反推基因调控网络的拓扑结构)
    ''' </summary>
    ''' <remarks>
    ''' 参看 method.md 的步骤3：基因之间的真实相互作用是稀疏的，
    ''' 故这里通过阈值截断与每基因限边数来恢复网络的拓扑。
    ''' </remarks>
    Public Module NetworkTopology

        ''' <summary>
        ''' 对拟合参数施加稀疏化约束，并返回恢复出来的有向调控边集合
        ''' </summary>
        ''' <param name="parameters">拟合得到的参数集合，稀疏化之后的结果会被回写到该对象之中</param>
        ''' <param name="options"></param>
        ''' <param name="prior">
        ''' 用于标注与加权的先验知识网络；为空的时候自动取 <paramref name="options"/> 之上的先验
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 先验知识在这里只影响「保留哪一条边」的顺序（<c>|net| * (1 + boost * weight)</c>），
        ''' 而阈值判定仍然使用原始的 <c>|net|</c>：
        ''' 这样先验支持的边在达到每基因限边数的时候会被优先保留，
        ''' 但是先验并不会凭空创造出一条净弹性不足的边。
        ''' </remarks>
        Public Function Sparsify(parameters As SSystemParameters,
                                 Optional options As FittingOptions = Nothing,
                                 Optional prior As PriorNet = Nothing) As RegulatoryEdge()
            If parameters Is Nothing Then
                Throw New ArgumentNullException(NameOf(parameters))
            End If
            If options Is Nothing Then
                options = New FittingOptions()
            End If
            If prior Is Nothing AndAlso options.UsePrior Then
                prior = options.Prior
            End If

            Dim n As Integer = parameters.nGenes
            Dim edges As New System.Collections.Generic.List(Of RegulatoryEdge)
            Dim boost As Double = options.PriorRankBoost
            Dim index As Dictionary(Of Long, RegulationEdge) = priorIndex(parameters, prior)
            Dim threshold As Double = options.Sparsity
            Dim limit As Integer = options.MaxEdgesPerGene

            For i As Integer = 0 To n - 1
                Dim candidates As Integer() = candidateList(parameters, i, options.KeepSelfLoop)
                Dim rankScore As Double() = New Double(candidates.Length - 1) {}
                Dim kept As New System.Collections.Generic.List(Of Integer)

                ' 只遍历本行的候选调控因子（每行 ≤ MaxRegulators），
                ' 而不是整行 n 个基因：18k 基因时这一步从 3.3E8 次迭代降到 ~4E5 次
                For c As Integer = 0 To candidates.Length - 1
                    Dim j As Integer = candidates(c)

                    If j = i AndAlso Not options.KeepSelfLoop Then
                        rankScore(c) = -1
                        Continue For
                    End If

                    Dim net As Double = parameters.G(i)(j) - parameters.H(i)(j)

                    If std.Abs(net) < threshold Then
                        rankScore(c) = -1
                        Continue For
                    End If

                    Dim record As RegulationEdge = lookup(index, prior, parameters, j, i)

                    rankScore(c) = std.Abs(net) * (1 + boost * record.EdgeWeight())
                    Call kept.Add(c)
                Next

                ' 每基因限边数：按照「净调控弹性 + 先验加权」之后的分数保留最强的若干条边
                If limit > 0 AndAlso kept.Count > limit Then
                    Dim drop As Integer() = kept _
                        .OrderBy(Function(c) rankScore(c)) _
                        .Take(kept.Count - limit) _
                        .ToArray

                    For Each c As Integer In drop
                        Call kept.Remove(c)
                    Next
                End If

                Dim keep As New HashSet(Of Integer)

                For Each c As Integer In kept
                    Dim j As Integer = candidates(c)
                    Dim record As RegulationEdge = lookup(index, prior, parameters, j, i)
                    Dim net As Double = parameters.G(i)(j) - parameters.H(i)(j)

                    Call keep.Add(j)
                    Call edges.Add(New RegulatoryEdge With {
                        .source = parameters.GeneNames(j),
                        .target = parameters.GeneNames(i),
                        .g = parameters.G(i)(j),
                        .h = parameters.H(i)(j),
                        .weight = net,
                        .PriorSupported = Not record Is Nothing,
                        .PriorType = record.RegulationName(),
                        .PriorConfidence = If(record Is Nothing, 0, record.Confidence),
                        .PriorWeight = record.EdgeWeight()
                    })
                Next

                ' 把没有保留的候选交互置零，使得后续的模型生成只包含稀疏化之后的边
                For Each j As Integer In candidates
                    If Not keep.Contains(j) Then
                        parameters.G(i)(j) = 0
                        parameters.H(i)(j) = 0
                    End If
                Next
            Next

            Return edges.ToArray
        End Function

        ''' <summary>
        ''' 取出一行的候选调控因子；没有候选信息（例如手工构造的参数对象）时回退为整行
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <param name="i"></param>
        ''' <param name="keepSelfLoop"></param>
        ''' <returns></returns>
        Private Function candidateList(parameters As SSystemParameters, i As Integer, keepSelfLoop As Boolean) As Integer()
            Dim candidates As Integer() = parameters.CandidateOf(i)

            If Not candidates.IsNullOrEmpty Then
                Return candidates
            End If

            Dim n As Integer = parameters.nGenes
            Dim all As Integer() = New Integer(n - 1) {}

            For j As Integer = 0 To n - 1
                all(j) = j
            Next

            Return all
        End Function

        ''' <summary>
        ''' 构建「(调控因子, 靶基因) → 先验边」的常数时间索引；
        ''' 避免了每个基因行都做一次 <see cref="PriorKnowledge.FindEdge"/>（线性扫描 + 数组分配）
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <param name="prior"></param>
        ''' <returns></returns>
        Private Function priorIndex(parameters As SSystemParameters, prior As PriorNet) As Dictionary(Of Long, RegulationEdge)
            If prior Is Nothing Then
                Return Nothing
            End If

            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To parameters.nGenes - 1
                Dim name As String = parameters.GeneNames(i)

                If Not String.IsNullOrEmpty(name) AndAlso Not map.ContainsKey(name) Then
                    map(name) = i
                End If
            Next

            Dim index As New Dictionary(Of Long, RegulationEdge)

            For Each edge As RegulationEdge In prior.Edges.SafeQuery()
                Dim tf As Integer
                Dim target As Integer

                If map.TryGetValue(edge.TF, tf) AndAlso map.TryGetValue(edge.TargetGene, target) Then
                    Dim key As Long = (CLng(tf) << 32) Or CLng(target And &HFFFFFFFFL)

                    If Not index.ContainsKey(key) Then
                        index(key) = edge
                    End If
                End If
            Next

            Return index
        End Function

        ''' <summary>
        ''' 查询先验边：优先使用索引，索引不可用时回退到逐行查找
        ''' </summary>
        Private Function lookup(index As Dictionary(Of Long, RegulationEdge), prior As PriorNet,
                                parameters As SSystemParameters, j As Integer, i As Integer) As RegulationEdge

            If index Is Nothing Then
                Return Nothing
            End If

            Dim edge As RegulationEdge = Nothing
            Dim key As Long = (CLng(j) << 32) Or CLng(i And &HFFFFFFFFL)

            If index.TryGetValue(key, edge) Then
                Return edge
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 查询一条调控关系的先验权重；没有先验支持的时候返回零
        ''' </summary>
        ''' <param name="prior"></param>
        ''' <param name="source"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Private Function priorWeightOf(prior As PriorNet, source As String, target As String) As Double
            If prior Is Nothing Then
                Return 0
            End If

            Return prior.FindEdge(source, target).EdgeWeight()
        End Function

        ''' <summary>
        ''' 先验知识对拟合结果的支持情况统计
        ''' </summary>
        ''' <param name="edges">稀疏化之后得到的调控边</param>
        ''' <param name="prior">本次拟合所使用的先验网络</param>
        ''' <returns></returns>
        Public Function PriorMetrics(edges As RegulatoryEdge(), Optional prior As PriorNet = Nothing) As PriorMetrics
            Dim metrics As New PriorMetrics With {
                .RecoveredEdges = edges.SafeQuery().Count()
            }

            For Each edge As RegulatoryEdge In edges.SafeQuery
                If edge.PriorSupported Then
                    metrics.SupportedEdges += 1

                    ' 自环承担的是降解项的自我衰减作用，不参与调控方向的一致性统计
                    If Not String.Equals(edge.source, edge.target, StringComparison.OrdinalIgnoreCase) Then
                        Select Case edge.PriorType
                            Case NameOf(EffectorRole.Activator)
                                If edge.weight > 0 Then
                                    metrics.SignConsistent += 1
                                Else
                                    Call metrics.SignViolations.Add(edge)
                                End If
                            Case NameOf(EffectorRole.Inhibitor)
                                If edge.weight < 0 Then
                                    metrics.SignConsistent += 1
                                Else
                                    Call metrics.SignViolations.Add(edge)
                                End If
                        End Select
                    End If
                Else
                    Call metrics.UnsupportedEdges.Add(edge)
                End If
            Next

            If Not prior Is Nothing Then
                Dim recovered As New System.Collections.Generic.HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                For Each edge As RegulatoryEdge In edges.SafeQuery
                    Call recovered.Add(edge.source & vbTab & edge.target)
                Next

                For Each record As RegulationEdge In prior.Edges.SafeQuery
                    If recovered.Contains(record.TF & vbTab & record.TargetGene) Then
                        metrics.ConfirmedPriorEdges += 1
                    Else
                        Call metrics.MissedPriorEdges.Add(record)
                    End If
                Next
            End If

            Return metrics
        End Function

        ''' <summary>
        ''' 网络的稳定性诊断：稳态要求对数增益矩阵 E = G - H 的特征值实部全为负数。
        ''' 这里给出 Gershgorin 圆盘定理所推出的最大特征值实部上界。
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function StabilityDiagnosis(parameters As SSystemParameters) As Double
            Dim net As Double()() = parameters.NetElasticity()
            Dim n As Integer = net.Length
            Dim bound As Double = Double.NegativeInfinity

            For i As Integer = 0 To n - 1
                Dim radius As Double = 0

                For j As Integer = 0 To n - 1
                    If j <> i Then
                        radius += std.Abs(net(i)(j))
                    End If
                Next

                bound = std.Max(bound, net(i)(i) + radius)
            Next

            Return bound
        End Function

        ''' <summary>
        ''' 生成网络的稳定性诊断报告
        ''' </summary>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Public Function StabilityReport(parameters As SSystemParameters) As String
            Dim bound As Double = StabilityDiagnosis(parameters)

            If bound < 0 Then
                Return $"The network is stable: the Gershgorin upper bound of the maximal eigenvalue real part is {bound.ToString("G4")} (< 0)."
            Else
                Return $"The network may be unstable: the Gershgorin upper bound of the maximal eigenvalue real part is {bound.ToString("G4")} (>= 0)."
            End If
        End Function

        ''' <summary>
        ''' 生成 Cytoscape 可以直接导入的 SIF 文本
        ''' </summary>
        ''' <param name="edges"></param>
        ''' <returns></returns>
        Public Function ToSIF(edges As IReadOnlyCollection(Of RegulatoryEdge)) As String
            If edges.IsNullOrEmpty Then
                Return ""
            End If

            Return edges _
                .Select(Function(e) $"{e.source}{vbTab}{e.interaction}{vbTab}{e.target}") _
                .JoinBy(vbLf)
        End Function
    End Module
End Namespace
