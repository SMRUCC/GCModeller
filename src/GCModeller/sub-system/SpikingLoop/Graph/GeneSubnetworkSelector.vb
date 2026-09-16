' ============================================================================
' GeneSubnetworkSelector.vb — P0：核心子网络筛选
'
' readme 六的路线图把 P0 定为"选定通路子网络（几百~几千基因）"，并明确警告：
'   "不要直接上全基因组 2 万基因——BPTT 在超大规模稀疏图上梯度易消失/爆炸"。
'
' 本类负责在建模之前把参与运算的基因规模压到可控范围：
'   1. 候选集 = 先验调控网络中出现的全部基因（TF ∪ Target）∩ 表达矩阵中真实存在的基因；
'   2. 打分 = 0.5 · 归一化调控度数 + 0.5 · 归一化表达方差
'      —— 度数高的基因是网络的枢纽节点，方差高的基因携带可建模的动态信息；
'   3. 按分数降序取前 MaxGenes 个基因，生成子表达矩阵与子先验网络；
'   4. 未命中表达矩阵的先验基因会被记录在诊断信息中（而不是静默丢弃）。
' ============================================================================

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports std = System.Math

Namespace Graph

    ''' <summary>子网络筛选结果</summary>
    Public Class SubnetworkSelection

        ''' <summary>入选基因（顺序与 <see cref="Expression"/> 的行顺序一致）</summary>
        Public Property GeneNames As String()

        ''' <summary>子表达矩阵（行 = 入选基因，列 = 全部样本，保留时间点标签）</summary>
        Public Property Expression As GeneExpressionData

        ''' <summary>子先验网络（仅保留两端均在入选基因集合内的调控边）</summary>
        Public Property Prior As PriorNetwork

        ''' <summary>先验网络中的候选基因总数</summary>
        Public Property NumCandidates As Integer

        ''' <summary>先验网络中未能映射到表达矩阵的基因（诊断用）</summary>
        Public Property MissingGenes As String()

        Public ReadOnly Property NumGenes As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>子网络中保留下来的调控边数</summary>
        Public ReadOnly Property NumEdges As Integer
            Get
                If Prior Is Nothing Then Return 0
                Return Prior.Edges.Count
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim nMissing = If(MissingGenes Is Nothing, 0, MissingGenes.Length)
            Return $"Subnetwork({NumGenes} genes / {NumCandidates} candidates, {NumEdges} prior edges, " &
                   $"{nMissing} missing)"
        End Function

    End Class

    ''' <summary>核心子网络筛选器</summary>
    Public Class GeneSubnetworkSelector

        Private ReadOnly _config As SpikingLoopConfig

        Public Sub New(config As SpikingLoopConfig)
            If config Is Nothing Then
                Throw New ArgumentNullException(NameOf(config))
            End If
            _config = config
        End Sub

        ''' <summary>
        ''' 从表达矩阵与先验网络中选出核心子网络。
        ''' </summary>
        ''' <param name="expr">完整的基因表达矩阵（[gene, sample]，行名为基因名）</param>
        ''' <param name="prior">先验调控网络</param>
        Public Function Extract(expr As GeneExpressionData, prior As PriorNetwork) As SubnetworkSelection
            If expr Is Nothing Then
                Throw New ArgumentNullException(NameOf(expr))
            End If
            If prior Is Nothing Then
                Throw New ArgumentNullException(NameOf(prior))
            End If

            ' ---- 1. 候选基因：先验网络两端基因 ∩ 表达矩阵 ----
            Dim exprIndex = BuildExpressionIndex(expr)
            Dim candidates As New List(Of String)()
            Dim missing As New List(Of String)()
            Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each gene In prior.GetAllGeneNames()
                If Not exprIndex.ContainsKey(gene) Then
                    missing.Add(gene)
                    Continue For
                End If
                If seen.Add(gene) Then
                    candidates.Add(expr.GeneNames(exprIndex(gene)))
                End If
            Next

            If candidates.Count = 0 Then
                Throw New InvalidOperationException(
                    "先验网络与表达矩阵没有任何交集基因，无法构建 SNN-GRN（请检查基因命名是否一致）")
            End If

            ' ---- 2. 打分：调控度数 + 表达方差 ----
            Dim degrees = CountDegrees(prior, candidates)
            Dim variances = ComputeVariances(expr, exprIndex, candidates)
            Dim maxDeg = If(degrees.Max() > 0.0, degrees.Max(), 1.0)
            Dim maxVar = If(variances.Max() > 0.0, variances.Max(), 1.0)

            Dim scored(candidates.Count - 1) As (gene As String, score As Double, degree As Double)
            For i = 0 To candidates.Count - 1
                Dim score = 0.5 * (degrees(i) / maxDeg) + 0.5 * (variances(i) / maxVar)
                scored(i) = (candidates(i), score, degrees(i))
            Next

            ' 排序键：(分数降序, 度数降序, 基因名升序)——末项保证结果完全确定
            Dim ordered = scored _
                .OrderByDescending(Function(a) a.score) _
                .ThenByDescending(Function(a) a.degree) _
                .ThenBy(Function(a) a.gene, StringComparer.OrdinalIgnoreCase) _
                .ToArray()

            Dim take = ordered.Length
            If _config.MaxGenes > 0 AndAlso _config.MaxGenes < take Then
                take = _config.MaxGenes
            End If

            Dim selected(take - 1) As String
            For i = 0 To take - 1
                selected(i) = ordered(i).gene
            Next

            ' ---- 3. 生成子表达矩阵与子先验网络 ----
            Dim subExpr = expr.GetSubMatrix(selected)
            If subExpr Is Nothing Then
                Throw New InvalidOperationException("子矩阵构造失败：没有任何基因被保留")
            End If

            Dim subPrior = FilterPrior(prior, subExpr.GeneNames)

            Return New SubnetworkSelection With {
                .GeneNames = subExpr.GeneNames,
                .Expression = subExpr,
                .Prior = subPrior,
                .NumCandidates = candidates.Count,
                .MissingGenes = missing.ToArray()
            }
        End Function

#Region "辅助"

        Private Shared Function BuildExpressionIndex(expr As GeneExpressionData) As Dictionary(Of String, Integer)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To expr.GeneNames.Length - 1
                If Not map.ContainsKey(expr.GeneNames(i)) Then
                    map(expr.GeneNames(i)) = i
                End If
            Next
            Return map
        End Function

        ''' <summary>统计每个候选基因在完整先验网络中的调控度数（作为 TF 的出度 + 作为 Target 的入度）</summary>
        Private Shared Function CountDegrees(prior As PriorNetwork, candidates As List(Of String)) As Double()
            Dim pos As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To candidates.Count - 1
                pos(candidates(i)) = i
            Next

            Dim deg(candidates.Count - 1) As Double
            For Each edge As RegulatoryEdge In prior.Edges.SafeQuery()
                Dim i = -1
                If pos.TryGetValue(edge.TF, i) Then deg(i) += 1.0

                Dim j = -1
                If pos.TryGetValue(edge.TargetGene, j) Then deg(j) += 1.0
            Next
            Return deg
        End Function

        ''' <summary>候选基因在样本维度上的表达方差（衡量其携带的动态信息量）</summary>
        Private Shared Function ComputeVariances(expr As GeneExpressionData, exprIndex As Dictionary(Of String, Integer),
                                                 candidates As List(Of String)) As Double()
            Dim n = expr.NSample
            Dim result(candidates.Count - 1) As Double

            For k = 0 To candidates.Count - 1
                Dim row = exprIndex(candidates(k))

                Dim sum = 0.0
                For j = 0 To n - 1
                    sum += expr.Matrix(row, j)
                Next
                Dim mean = sum / n

                Dim ss = 0.0
                For j = 0 To n - 1
                    Dim d = expr.Matrix(row, j) - mean
                    ss += d * d
                Next

                result(k) = If(n > 1, ss / (n - 1), 0.0)
            Next

            Return result
        End Function

        ''' <summary>只保留两端基因都在子网络内的调控边</summary>
        Private Shared Function FilterPrior(prior As PriorNetwork, genes As String()) As PriorNetwork
            Dim keep As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each g In genes
                keep.Add(g)
            Next

            Dim sub_ As New PriorNetwork()
            For Each edge As RegulatoryEdge In prior.Edges.SafeQuery()
                If keep.Contains(edge.TF) AndAlso keep.Contains(edge.TargetGene) Then
                    sub_.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
                End If
            Next

            Return sub_
        End Function

#End Region

    End Class

End Namespace
