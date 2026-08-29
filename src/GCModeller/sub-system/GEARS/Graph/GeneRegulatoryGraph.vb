Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN
Imports SMRUCC.genomics.Analysis.BNLearn.Core

Namespace Graph

    ''' <summary>
    ''' 基因调控图：由先验调控网络构建用于 GNN 消息传递的异质有向图
    ''' </summary>
    ''' <remarks>
    ''' 本类型负责 readme 中 Step 1 的工作：把 <see cref="PriorNetwork"/> 中的
    ''' TF → TargetGene 调控关系转换为 GNN 的图结构，并且可选地根据 control 表达谱的
    ''' 相关性追加共表达边（GEARS 双通道设计中的「共表达协方差图」）。
    '''
    ''' 除了构建 <see cref="GNN.Graph"/> 之外，这里还会预先把入边按照目标节点分组缓存为
    ''' 稀疏邻接结构（源节点索引 / 关系类型 / 归一化权重），供
    ''' <see cref="Layers.GEARSConvLayer"/> 做 O(|E|) 的稀疏聚合，避免 350+ 节点场景下
    ''' 稠密邻接矩阵乘法带来的巨大开销。
    ''' </remarks>
    Public Class GeneRegulatoryGraph

        ''' <summary>基因名称列表，顺序与表达矩阵的行顺序严格一致</summary>
        ''' <returns>基因名称数组</returns>
        Public ReadOnly Property GeneNames As String()

        ''' <summary>基因数量（图中的节点数量）</summary>
        ''' <returns>节点数量</returns>
        Public ReadOnly Property NumGenes As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return GeneNames.Length
            End Get
        End Property

        ''' <summary>GNN 图结构对象</summary>
        ''' <returns><see cref="GNN.Graph"/> 实例</returns>
        Public ReadOnly Property Graph As GNN.Graph

        ''' <summary>基因名称到节点索引的映射（大小写不敏感）</summary>
        ''' <returns>基因名 → 节点索引 的字典</returns>
        Public ReadOnly Property GeneIndex As Dictionary(Of String, Integer)

        ''' <summary>实际生效的调控边数量（不含共表达边与自环）</summary>
        ''' <returns>先验网络中被成功映射到表达矩阵上的边数量</returns>
        Public ReadOnly Property NumPriorEdges As Integer

        ''' <summary>追加的共表达边数量</summary>
        ''' <returns>共表达边数量</returns>
        Public ReadOnly Property NumCoExpressionEdges As Integer

        ''' <summary>图中每种关系类型的边数量统计</summary>
        ''' <returns>长度为 <see cref="EdgeRelationTypes.NumRelationTypes"/> 的计数数组</returns>
        Public ReadOnly Property RelationTypeCounts As Integer()

        ReadOnly inSources As Integer()()
        ReadOnly inTypes As Integer()()
        ReadOnly inWeights As Double()()
        ReadOnly selfWeights As Double()

        ''' <summary>
        ''' 构建基因调控图
        ''' </summary>
        ''' <param name="geneNames">表达矩阵中的基因名称列表（行名），决定节点索引顺序</param>
        ''' <param name="prior">先验调控网络；两端基因不在 <paramref name="geneNames"/> 中的边会被丢弃</param>
        ''' <param name="controlExpr">
        ''' control 条件下的表达矩阵 [gene, sample]；当 <paramref name="coexpressionTopK"/> 大于 0 时
        ''' 用于计算基因间 Pearson 相关并追加共表达边
        ''' </param>
        ''' <param name="coexpressionTopK">
        ''' 每个基因保留的共表达边数量（按 |Pearson r| 降序）；0 表示关闭共表达图
        ''' </param>
        ''' <param name="minCoexpression">
        ''' 共表达边的最小相关系数阈值，低于该阈值的关系不会被加入图中
        ''' </param>
        Public Sub New(geneNames As String(),
                       prior As PriorNetwork,
                       Optional controlExpr As Double(,) = Nothing,
                       Optional coexpressionTopK As Integer = 0,
                       Optional minCoexpression As Double = 0.7)

            Me.GeneNames = geneNames
            Me.GeneIndex = BuildGeneIndex(geneNames)
            Me.RelationTypeCounts = New Integer(EdgeRelationTypes.NumRelationTypes - 1) {}

            Dim n As Integer = geneNames.Length
            Dim sources As New List(Of Integer())()
            Dim types As New List(Of Integer())()
            Dim weights As New List(Of Double())()
            Dim degrees As Double() = New Double(n - 1) {}
            Dim edgeList As New List(Of (src As Integer, dst As Integer, type As Integer, w As Double))()

            ' ---------- 1. 先验调控网络的 TF -> Target 有向边 ----------
            If prior IsNot Nothing Then
                For Each edge As RegulatoryEdge In prior.Edges.SafeQuery
                    Dim fromIdx As Integer = -1
                    Dim toIdx As Integer = -1

                    Call GeneIndex.TryGetValue(edge.TF, fromIdx)
                    Call GeneIndex.TryGetValue(edge.TargetGene, toIdx)

                    ' 两端基因都必须存在于表达矩阵中，且不能是自环
                    If fromIdx < 0 OrElse toIdx < 0 OrElse fromIdx = toIdx Then
                        Continue For
                    End If

                    Dim relType As EdgeRelationType = EdgeRelationTypes.FromEffector(edge.RegulationType)
                    Dim weight As Double = edge.Confidence

                    If Double.IsNaN(weight) OrElse weight <= 0 Then
                        weight = 1.0
                    End If

                    Call edgeList.Add((fromIdx, toIdx, CInt(relType), weight))
                Next
            End If

            Me.NumPriorEdges = edgeList.Count

            ' ---------- 2. GEARS 共表达协方差图 ----------
            Dim coexpCount As Integer = 0

            If coexpressionTopK > 0 AndAlso controlExpr IsNot Nothing Then
                Dim coexp As List(Of (Integer, Integer, Double)) =
                    TopCoExpressionEdges(controlExpr, coexpressionTopK, minCoexpression)

                For Each e In coexp
                    Call edgeList.Add((e.Item1, e.Item2, CInt(EdgeRelationType.CoExpression), e.Item3))
                Next

                coexpCount = coexp.Count
            End If

            Me.NumCoExpressionEdges = coexpCount

            ' ---------- 3. 构建 GNN 图结构与稀疏入边缓存 ----------
            Me.Graph = New GNN.Graph(n, 1)

            For i As Integer = 0 To n - 1
                sources.Add(New List(Of Integer)())
                types.Add(New List(Of Integer)())
                weights.Add(New List(Of Double)())
                ' 自环权重恒定为 1，后面的归一化会把其折算进权重总和
                degrees(i) = 1.0
            Next

            For Each e In edgeList
                Call Me.Graph.AddEdge(e.src, e.dst, CSng(e.w))

                sources(e.dst).Add(e.src)
                types(e.dst).Add(e.type)
                weights(e.dst).Add(e.w)

                degrees(e.dst) += e.w
                RelationTypeCounts(e.type) += 1
            Next

            Me.inSources = New Integer(n - 1)() {}
            Me.inTypes = New Integer(n - 1)() {}
            Me.inWeights = New Double(n - 1)() {}
            Me.selfWeights = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                inSources(i) = sources(i).ToArray()
                inTypes(i) = types(i).ToArray()

                Dim norm As Double() = New Double(weights(i).Count - 1) {}
                Dim total As Double = If(degrees(i) > 0, degrees(i), 1.0)

                For k As Integer = 0 To norm.Length - 1
                    norm(k) = weights(i)(k) / total
                Next

                inWeights(i) = norm
                selfWeights(i) = 1.0 / total
            Next
        End Sub

        ''' <summary>
        ''' 建立基因名称到节点索引的映射
        ''' </summary>
        ''' <param name="geneNames">基因名称列表</param>
        ''' <returns>大小写不敏感的基因名索引字典</returns>
        Private Shared Function BuildGeneIndex(geneNames As String()) As Dictionary(Of String, Integer)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To geneNames.Length - 1
                map(geneNames(i)) = i
            Next

            Return map
        End Function

        ''' <summary>
        ''' 计算基因两两之间的 Pearson 相关系数，为每个基因挑选相关性最强的 Top-K 条共表达边
        ''' </summary>
        ''' <param name="expr">表达矩阵 [gene, sample]</param>
        ''' <param name="topK">每个基因保留的边数量</param>
        ''' <param name="minCorr">相关系数阈值</param>
        ''' <returns>共表达边列表，元素为 (基因A索引, 基因B索引, |相关系数|)</returns>
        Private Function TopCoExpressionEdges(expr As Double(,), topK As Integer, minCorr As Double) As List(Of (Integer, Integer, Double))
            Dim result As New List(Of (Integer, Integer, Double))()
            Dim n As Integer = NumGenes
            Dim nSample As Integer = expr.GetLength(1)

            If nSample < 3 Then
                Return result
            End If

            ' 预先做行标准化，Pearson 相关即退化为标准化向量的点积
            Dim z As Double()() = New Double(n - 1)() {}

            For i As Integer = 0 To n - 1
                Dim row As Double() = New Double(nSample - 1) {}
                Dim sum As Double = 0

                For j As Integer = 0 To nSample - 1
                    row(j) = expr(i, j)
                    sum += row(j)
                Next

                Dim mean As Double = sum / nSample
                Dim ss As Double = 0

                For j As Integer = 0 To nSample - 1
                    row(j) -= mean
                    ss += row(j) * row(j)
                Next

                Dim sd As Double = Math.Sqrt(ss)

                If sd > 0 Then
                    For j As Integer = 0 To nSample - 1
                        row(j) /= sd
                    Next
                Else
                    For j As Integer = 0 To nSample - 1
                        row(j) = 0
                    Next
                End If

                z(i) = row
            Next

            ' 为每个基因收集候选边，之后取 Top-K
            Dim candidates As List(Of (Integer, Double))() = New List(Of (Integer, Double))(n - 1) {}

            For i As Integer = 0 To n - 1
                candidates(i) = New List(Of (Integer, Double))()
            Next

            For a As Integer = 0 To n - 1
                Dim za As Double() = z(a)

                For b As Integer = a + 1 To n - 1
                    Dim zb As Double() = z(b)
                    Dim r As Double = 0

                    For j As Integer = 0 To nSample - 1
                        r += za(j) * zb(j)
                    Next

                    Dim absR As Double = Math.Abs(r)

                    If absR >= minCorr Then
                        candidates(a).Add((b, absR))
                        candidates(b).Add((a, absR))
                    End If
                Next
            Next

            Dim seen As New HashSet(Of Long)()

            For a As Integer = 0 To n - 1
                Dim list As List(Of (Integer, Double)) = candidates(a)

                list.Sort(Function(x, y) y.Item2.CompareTo(x.Item2))

                Dim take As Integer = Math.Min(topK, list.Count)

                For k As Integer = 0 To take - 1
                    Dim b As Integer = list(k).Item1
                    Dim key As Long = If(a < b, CLng(a) * 100000L + b, CLng(b) * 100000L + a)

                    If seen.Add(key) Then
                        result.Add((a, b, list(k).Item2))
                    End If
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 获取指向指定节点的所有入边源节点索引
        ''' </summary>
        ''' <param name="nodeIndex">目标节点索引</param>
        ''' <returns>源节点索引数组</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function InEdgeSources(nodeIndex As Integer) As Integer()
            Return inSources(nodeIndex)
        End Function

        ''' <summary>
        ''' 获取指向指定节点的所有入边关系类型
        ''' </summary>
        ''' <param name="nodeIndex">目标节点索引</param>
        ''' <returns>关系类型索引数组，与 <see cref="InEdgeSources(Integer)"/> 一一对应</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function InEdgeTypes(nodeIndex As Integer) As Integer()
            Return inTypes(nodeIndex)
        End Function

        ''' <summary>
        ''' 获取指向指定节点的所有入边的归一化权重（已折算自环，权重之和小于 1）
        ''' </summary>
        ''' <param name="nodeIndex">目标节点索引</param>
        ''' <returns>归一化权重数组，与 <see cref="InEdgeSources(Integer)"/> 一一对应</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function InEdgeWeights(nodeIndex As Integer) As Double()
            Return inWeights(nodeIndex)
        End Function

        ''' <summary>
        ''' 获取指定节点自环信息的保留权重
        ''' </summary>
        ''' <param name="nodeIndex">节点索引</param>
        ''' <returns>自环权重，取值在 (0, 1] 区间内</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SelfWeight(nodeIndex As Integer) As Double
            Return selfWeights(nodeIndex)
        End Function

        ''' <summary>
        ''' 尝试按照基因名称获取节点索引
        ''' </summary>
        ''' <param name="geneName">基因名称（大小写不敏感）</param>
        ''' <param name="index">返回的节点索引；基因不存在时返回 -1</param>
        ''' <returns>基因存在则返回 True，否则返回 False</returns>
        Public Function TryGetGeneIndex(geneName As String, ByRef index As Integer) As Boolean
            Return GeneIndex.TryGetValue(geneName, index)
        End Function

        ''' <summary>
        ''' 输出图结构的摘要信息
        ''' </summary>
        ''' <returns>描述图规模的字符串</returns>
        Public Overrides Function ToString() As String
            Return $"GEARS graph: {NumGenes} genes, {NumPriorEdges} prior edges, {NumCoExpressionEdges} co-expression edges"
        End Function
    End Class
End Namespace
