Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports GEARSGraph = SMRUCC.genomics.Analysis.GEARS.Graph

Namespace Graph

    ''' <summary>
    ''' 先验调控网络的静态图编码：最短路径距离、拉普拉斯位置编码与度中心性。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 对应 method.md §2.2 步骤 4 与 §3.1 的 Graphormer 编码设计：
    ''' <list type="bullet">
    ''' <item><description><b>SPD</b>：以桶索引矩阵 <c>[N, N]</c> 存储，供注意力偏置查表；
    ''' 超距与不连通基因对分别占用独立桶，避免稠密浮点距离矩阵。</description></item>
    ''' <item><description><b>LapPE</b>：对称归一化拉普拉斯的前 K 个非平凡特征向量，
    ''' 让注意力能够区分不同的"图位置"。</description></item>
    ''' <item><description><b>度中心性</b>：入度（被多少 TF 调控）与出度（调控多少靶基因）
    ''' 分别编码，符合有向调控图的语义。</description></item>
    ''' </list>
    ''' </para>
    ''' <para>
    ''' 全部编码只依赖图结构、与训练无关，因此构造一次后缓存复用。
    ''' 距离与邻接均按<b>无向</b>方式计算：调控边的方向信息由消息传递支路与
    ''' 入/出度编码承载，最短路径取无向距离可以显著提升连通性，
    ''' 避免"有向图下绝大多数基因对不连通"导致偏置桶失去区分度。
    ''' </para>
    ''' </remarks>
    Public Class GraphEncoding

        ''' <summary>自身到自身的距离桶编号</summary>
        Public Const SelfBucket As Integer = 0

        ''' <summary>
        ''' 拉普拉斯特征分解的最大节点数；超过该规模时跳过 LapPE（置零）并给出提示。
        ''' </summary>
        ''' <remarks>
        ''' 稠密特征分解是 O(N³)，N=2000 时约需数十秒且占用 32MB 临时矩阵，
        ''' 属于一次性开销但会拖慢调试；超过阈值时退化为"仅度编码"。
        ''' 二期可用稀疏近似（如随机游走位置编码）替代。
        ''' </remarks>
        Public Const LapPeMaxNodes As Integer = 1500

        ''' <summary>基因数量</summary>
        Public Property NGene As Integer

        ''' <summary>最短路径桶的总数 = SpdMaxDist + 3</summary>
        Public Property SpdBucketCount As Integer

        ''' <summary>SPD 桶索引矩阵 <c>[N, N]</c></summary>
        Public Property SpdBuckets As Integer(,)

        ''' <summary>拉普拉斯位置编码 <c>[N, PeDim]</c>；跳过计算时列数为 0</summary>
        Public Property LapPe As Double(,)

        ''' <summary>实际使用的 LapPE 维度</summary>
        Public Property LapPeDim As Integer

        ''' <summary>入度（被调控次数）</summary>
        Public Property InDegree As Integer()

        ''' <summary>出度（调控靶基因数）</summary>
        Public Property OutDegree As Integer()

        ''' <summary>入度的桶编号（超过上限的度统一归入最后一桶）</summary>
        Public Property InDegreeBucket As Integer()

        ''' <summary>出度的桶编号</summary>
        Public Property OutDegreeBucket As Integer()

        ''' <summary>度桶数量；桶编号取值范围为 <c>0 .. DegreeBucketCount-1</c></summary>
        Public Property DegreeBucketCount As Integer

        ''' <summary>
        ''' 计算先验调控网络的静态图编码。
        ''' </summary>
        ''' <param name="graphData">由 <see cref="GEARSGraph.GeneRegulatoryGraph"/> 构建的调控图</param>
        ''' <param name="config">配置对象（SPD 截断距离、LapPE 维度与度桶上限）</param>
        ''' <returns>图编码对象（一次性预计算，可直接缓存）</returns>
        Public Shared Function Compute(graphData As GEARSGraph.GeneRegulatoryGraph,
                                       config As GPSConfig) As GraphEncoding

            If graphData Is Nothing Then Throw New ArgumentNullException(NameOf(graphData))
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))

            Dim n As Integer = graphData.NumGenes
            Dim spdMax As Integer = System.Math.Max(1, config.SpdMaxDist)
            Dim bucketCount As Integer = spdMax + 3

            Dim encoding As New GraphEncoding With {
                .NGene = n,
                .SpdBucketCount = bucketCount,
                .DegreeBucketCount = DefaultDegreeBucketCount
            }

            ' ---- 邻接（无向，自环忽略）与入/出度 ----
            Dim adjacency As List(Of Integer)() = New List(Of Integer)(n - 1) {}
            Dim inDegree As Integer() = New Integer(n - 1) {}
            Dim outDegree As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                adjacency(i) = New List(Of Integer)()
            Next

            For Each e In graphData.Graph.Edges
                If e.Source = e.Target Then Continue For
                If e.Source < 0 OrElse e.Source >= n OrElse e.Target < 0 OrElse e.Target >= n Then Continue For

                outDegree(e.Source) += 1
                inDegree(e.Target) += 1

                Call adjacency(e.Source).Add(e.Target)
                Call adjacency(e.Target).Add(e.Source)
            Next

            ' 去重，避免同一对基因间的多条边让 BFS 重复入队
            For i As Integer = 0 To n - 1
                Dim unique As Integer() = adjacency(i).Distinct().ToArray()
                adjacency(i) = New List(Of Integer)(unique)
            Next

            encoding.InDegree = inDegree
            encoding.OutDegree = outDegree
            encoding.InDegreeBucket = Bucketize(inDegree, DefaultDegreeBucketCount)
            encoding.OutDegreeBucket = Bucketize(outDegree, DefaultDegreeBucketCount)

            ' ---- 最短路径桶（逐节点 BFS） ----
            encoding.SpdBuckets = ComputeSpdBuckets(adjacency, n, spdMax, bucketCount)

            ' ---- 拉普拉斯位置编码 ----
            Dim lapPe As (Matrix As Double(,), PeDim As Integer) = ComputeLapPe(adjacency, n, config.PeDim)
            encoding.LapPe = lapPe.Matrix
            encoding.LapPeDim = lapPe.PeDim

            Return encoding
        End Function

        ''' <summary>
        ''' 度桶的默认数量：入度/出度分别独立分桶，编号 <c>0 .. DegreeBucketCount-1</c>。
        ''' </summary>
        ''' <remarks>
        ''' 调控网络的度分布长尾明显，绝大多数基因度数很小；
        ''' 取 32 个桶（0 度 + 1..30 度 + 30 以上合并）足以区分中心 TF 与普通基因。
        ''' </remarks>
        Public Const DefaultDegreeBucketCount As Integer = 32

        ''' <summary>
        ''' 逐节点广度优先搜索得到最短路径桶矩阵。
        ''' </summary>
        ''' <param name="adjacency">无向邻接表</param>
        ''' <param name="n">节点数</param>
        ''' <param name="spdMax">最大编码距离</param>
        ''' <param name="bucketCount">桶总数</param>
        ''' <returns>桶索引矩阵 [N, N]</returns>
        Private Shared Function ComputeSpdBuckets(adjacency As List(Of Integer)(),
                                                  n As Integer,
                                                  spdMax As Integer,
                                                  bucketCount As Integer) As Integer(,)

            Dim farBucket As Integer = spdMax + 1
            Dim disconnectedBucket As Integer = spdMax + 2
            Dim buckets As Integer(,) = New Integer(n - 1, n - 1) {}
            Dim distance As Integer() = New Integer(n - 1) {}
            Dim queue As New Queue(Of Integer)()

            For source As Integer = 0 To n - 1
                ' 初始化：-1 表示未访问
                For i As Integer = 0 To n - 1
                    distance(i) = -1
                Next

                distance(source) = 0
                queue.Clear()
                queue.Enqueue(source)

                ' 完整 BFS：必须区分"超出编码距离但仍连通"与"真正不连通"，
                ' 因此不能在同一距离处提前截断搜索（否则远距离节点会被误判为不连通）
                While queue.Count > 0
                    Dim u As Integer = queue.Dequeue()
                    Dim d As Integer = distance(u)

                    For Each v As Integer In adjacency(u)
                        If distance(v) >= 0 Then Continue For

                        distance(v) = d + 1
                        queue.Enqueue(v)
                    Next
                End While

                For target As Integer = 0 To n - 1
                    Dim d As Integer = distance(target)

                    If d < 0 Then
                        buckets(source, target) = disconnectedBucket
                    ElseIf d > spdMax Then
                        buckets(source, target) = farBucket
                    Else
                        buckets(source, target) = d
                    End If
                Next
            Next

            Return buckets
        End Function

        ''' <summary>
        ''' 计算对称归一化拉普拉斯的前 K 个非平凡特征向量。
        ''' </summary>
        ''' <param name="adjacency">无向邻接表</param>
        ''' <param name="n">节点数</param>
        ''' <param name="peDim">期望维度</param>
        ''' <returns>位置编码矩阵与有效维度</returns>
        Private Shared Function ComputeLapPe(adjacency As List(Of Integer)(),
                                             n As Integer,
                                             peDim As Integer) As (Matrix As Double(,), PeDim As Integer)

            If peDim <= 0 OrElse n <= 2 Then
                Return (New Double(n - 1, -1) {}, 0)
            End If

            If n > LapPeMaxNodes Then
                Console.WriteLine($"[GPS] 基因数 {n} 超过 LapPE 稠密分解上限 {LapPeMaxNodes}，位置编码置零")
                Return (New Double(n - 1, -1) {}, 0)
            End If

            ' ---- 对称归一化拉普拉斯 L = I - D^(-1/2)·A·D^(-1/2) ----
            Dim degree As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                degree(i) = adjacency(i).Count
            Next

            Dim laplacian As Double()() = New Double(n - 1)() {}

            For i As Integer = 0 To n - 1
                laplacian(i) = New Double(n - 1) {}
                laplacian(i)(i) = 1.0
            Next

            For i As Integer = 0 To n - 1
                If degree(i) <= 0.0 Then Continue For

                For Each j As Integer In adjacency(i)
                    If degree(j) <= 0.0 Then Continue For

                    laplacian(i)(j) = -1.0 / System.Math.Sqrt(degree(i) * degree(j))
                Next
            Next

            ' ---- 特征分解 ----
            Dim eig As EigenvalueDecomposition

            Try
                eig = New EigenvalueDecomposition(New NumericMatrix(laplacian))
            Catch ex As Exception
                Console.WriteLine($"[GPS] 拉普拉斯特征分解失败，位置编码置零：{ex.Message}")
                Return (New Double(n - 1, -1) {}, 0)
            End Try

            Dim values As Double() = eig.RealEigenvalues
            Dim vectors As NumericMatrix = eig.V

            ' 按特征值升序排序，跳过最小的平凡特征向量（对应常数向量）
            Dim order As Integer() = Enumerable.Range(0, n).
                OrderBy(Function(k) values(k)).
                ToArray()

            Dim peDimUsed As Integer = System.Math.Min(peDim, n - 1)
            Dim pe As Double(,) = New Double(n - 1, peDimUsed - 1) {}

            For c As Integer = 0 To peDimUsed - 1
                Dim col As Integer = order(c + 1)

                ' 符号不确定性：以绝对值最大的分量为正，保证同图多次计算结果一致
                Dim absMax As Double = 0.0
                Dim sign As Double = 1.0

                For i As Integer = 0 To n - 1
                    Dim v As Double = vectors(i, col)

                    If System.Math.Abs(v) > absMax Then
                        absMax = System.Math.Abs(v)
                        sign = If(v < 0.0, -1.0, 1.0)
                    End If
                Next

                For i As Integer = 0 To n - 1
                    pe(i, c) = sign * vectors(i, col)
                Next
            Next

            Return (pe, peDimUsed)
        End Function

        ''' <summary>
        ''' 把非负整数数组映射为桶编号，超过上限的值统一归入最后一桶。
        ''' </summary>
        ''' <param name="values">原始度数组</param>
        ''' <param name="bucketCount">桶总数</param>
        ''' <returns>桶编号数组</returns>
        Private Shared Function Bucketize(values As Integer(), bucketCount As Integer) As Integer()
            Dim result As Integer() = New Integer(values.Length - 1) {}
            Dim cap As Integer = bucketCount - 1

            For i As Integer = 0 To values.Length - 1
                result(i) = System.Math.Min(System.Math.Max(values(i), 0), cap)
            Next

            Return result
        End Function

    End Class

End Namespace
