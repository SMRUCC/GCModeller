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

        ''' <summary>SPD 桶索引矩阵 <c>[N, N]</c>；稀疏模式下为 <c>Nothing</c>（不会分配 N² 内存）</summary>
        Public Property SpdBuckets As Integer(,)

        ''' <summary>
        ''' 大图稀疏注意力的候选模式；稠密模式下为 <c>Nothing</c>。
        ''' </summary>
        ''' <remarks>
        ''' 当基因数超过 <see cref="GPSConfig.DenseAttentionMaxNodes"/> 时不再构造
        ''' <see cref="SpdBuckets"/>（N=6 万时是 14.5 GB 的 <c>Integer(N,N)</c>），
        ''' 改由本属性给出"每节点固定候选集"的压缩表示。
        ''' </remarks>
        Public Property Pattern As SparseAttentionPattern

        ''' <summary>当前是否使用稀疏注意力模式</summary>
        ''' <returns><see cref="Pattern"/> 非空时为 True</returns>
        Public ReadOnly Property IsSparse As Boolean
            Get
                Return Pattern IsNot Nothing
            End Get
        End Property

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

            ' ---- 注意力模式的规模分派 ----
            ' 稠密路径需要 [N,N] 的桶矩阵与分数矩阵，N 一旦达到万级就无法分配；
            ' 超过阈值时改用"每节点固定候选集"的稀疏模式（O(N·c) 内存与计算）
            Dim denseLimit As Integer = System.Math.Max(1, config.DenseAttentionMaxNodes)

            If n > denseLimit Then
                Dim hubs As Integer() = SelectHubGenes(inDegree, outDegree, config.AttentionHubGenes)

                encoding.Pattern = SparseAttentionPattern.Build(
                    adjacency:=adjacency,
                    n:=n,
                    spdMax:=spdMax,
                    bucketCount:=bucketCount,
                    neighbors:=config.AttentionNeighbors,
                    randomLinks:=config.AttentionRandomLinks,
                    hubs:=hubs,
                    seed:=config.Seed)

                Console.WriteLine(
                    $"[GPS] 基因数 {n} 超过稠密注意力上限 {denseLimit}，改用稀疏注意力：每基因 " &
                    $"{encoding.Pattern.CandidateCount} 个候选" &
                    $"（图邻域<={encoding.Pattern.GraphNeighborBudget}、" &
                    $"枢纽<={encoding.Pattern.HubGeneBudget}，" &
                    $"其余槽位用固定种子的随机长程链接补齐），" &
                    $"注意力权重缓存约 {AttentionCacheMegabytes(n, encoding.Pattern.CandidateCount, config):F1} MB")
            Else
                encoding.SpdBuckets = ComputeSpdBuckets(adjacency, n, spdMax, bucketCount)

                Console.WriteLine($"[GPS] 基因数 {n} ≤ 稠密注意力上限 {denseLimit}，使用稠密注意力（{n}×{n} 分数矩阵）")
            End If

            ' ---- 拉普拉斯位置编码（小图精确分解 / 大图随机游走近似） ----
            Dim lapPe As (Matrix As Double(,), PeDim As Integer) = ComputeLapPe(
                adjacency, n, config.PeDim, spdMax, config.Seed)

            encoding.LapPe = lapPe.Matrix
            encoding.LapPeDim = lapPe.PeDim

            Return encoding
        End Function

        ''' <summary>
        ''' 按（入度 + 出度）降序选取全局枢纽基因；无任何连边时返回空数组。
        ''' </summary>
        ''' <param name="inDegree">入度数组</param>
        ''' <param name="outDegree">出度数组</param>
        ''' <param name="count">期望数量</param>
        ''' <returns>枢纽基因下标（降序、同分按下标升序，保证确定性）</returns>
        Private Shared Function SelectHubGenes(inDegree As Integer(),
                                               outDegree As Integer(),
                                               count As Integer) As Integer()

            If count <= 0 Then Return New Integer() {}

            Dim eligible As New List(Of Integer)(inDegree.Length)

            For i As Integer = 0 To inDegree.Length - 1
                ' 无连边的基因不是枢纽：空图时全局混合由随机长程链接承担
                If inDegree(i) + outDegree(i) > 0 Then Call eligible.Add(i)
            Next

            If eligible.Count = 0 Then Return New Integer() {}

            Return eligible.
                OrderByDescending(Function(i) inDegree(i) + outDegree(i)).
                ThenBy(Function(i) i).
                Take(System.Math.Min(count, eligible.Count)).
                ToArray()
        End Function

        ''' <summary>
        ''' 估算稀疏注意力权重缓存的字节规模（每个头每层一份 <c>[N, c]</c> 双精度权重）。
        ''' </summary>
        ''' <param name="n">基因数</param>
        ''' <param name="candidates">每节点候选数</param>
        ''' <param name="config">配置（取头数与层数）</param>
        ''' <returns>估算的 MB 数</returns>
        Private Shared Function AttentionCacheMegabytes(n As Integer,
                                                        candidates As Integer,
                                                        config As GPSConfig) As Double

            Dim bytes As Double = CDbl(n) * candidates * 8.0 * config.NHeads * config.NLayers

            Return bytes / (1024.0 * 1024.0)
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
                                             peDim As Integer,
                                             spdMax As Integer,
                                             seed As Integer) As (Matrix As Double(,), PeDim As Integer)

            If peDim <= 0 OrElse n <= 2 Then
                Return (New Double(n - 1, -1) {}, 0)
            End If

            ' 小图保持精确的特征分解（结果与既有实现逐位一致）；
            ' 大图换用 O(K·T·|E|) 的随机游走近似，避免 O(N³) 与 N² 内存
            If n <= LapPeMaxNodes Then
                Return ComputeDenseLapPe(adjacency, n, peDim)
            End If

            Return ComputeRandomWalkPe(adjacency, n, peDim, spdMax, seed)
        End Function

        ''' <summary>
        ''' 精确的拉普拉斯位置编码：对称归一化拉普拉斯的前 K 个非平凡特征向量。
        ''' </summary>
        ''' <param name="adjacency">无向邻接表</param>
        ''' <param name="n">节点数（不超过 <see cref="LapPeMaxNodes"/>）</param>
        ''' <param name="peDim">期望维度</param>
        ''' <returns>位置编码矩阵与有效维度</returns>
        Private Shared Function ComputeDenseLapPe(adjacency As List(Of Integer)(),
                                                  n As Integer,
                                                  peDim As Integer) As (Matrix As Double(,), PeDim As Integer)

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
        ''' 大图的可扩展位置编码：随机游走（扩散）近似。
        ''' </summary>
        ''' <remarks>
        ''' <para>
        ''' 稠密拉普拉斯特征分解是 O(N³)，N=6 万时不可行。这里用"随机投影 + 懒惰随机游走"
        ''' 近似同一条低频子空间：
        ''' <code>
        ''' v ← (v + D⁻¹·A·v) / 2      （重复 T 步）
        ''' </code>
        ''' 初始向量取固定种子的 Rademacher 随机向量（±1），每步之后按平稳分布
        ''' π = deg / Σdeg 去掉平稳分量，最后做 Gram-Schmidt 正交化并逐列单位化。
        ''' 代价为 O(K·T·|E| + K²·N)，与 N² 无关。
        ''' </para>
        ''' <para>
        ''' 位置编码只由"图结构 + 种子"决定，因此与稠密 LapPE 一样是可复现的一次性预计算。
        ''' 先验图没有任何连边时扩散算子无定义，此时置零并给出说明。
        ''' </para>
        ''' </remarks>
        ''' <param name="adjacency">无向邻接表</param>
        ''' <param name="n">节点数（超过 <see cref="LapPeMaxNodes"/>）</param>
        ''' <param name="peDim">期望维度</param>
        ''' <param name="spdMax">最大编码距离（决定扩散步数）</param>
        ''' <param name="seed">随机投影种子</param>
        ''' <returns>位置编码矩阵与有效维度</returns>
        Private Shared Function ComputeRandomWalkPe(adjacency As List(Of Integer)(),
                                                    n As Integer,
                                                    peDim As Integer,
                                                    spdMax As Integer,
                                                    seed As Integer) As (Matrix As Double(,), PeDim As Integer)

            Dim degree As Double() = New Double(n - 1) {}
            Dim totalDegree As Double = 0.0

            For i As Integer = 0 To n - 1
                degree(i) = adjacency(i).Count
                totalDegree += degree(i)
            Next

            If totalDegree <= 0.0 Then
                Console.WriteLine(
                    $"[GPS] 基因数 {n} 超过 LapPE 稠密分解上限 {LapPeMaxNodes}，" &
                    $"且先验图没有任何连边，随机游走位置编码无定义，位置编码置零" &
                    $"（度编码与基因嵌入仍然生效）")

                Return (New Double(n - 1, -1) {}, 0)
            End If

            Dim used As Integer = System.Math.Min(System.Math.Max(1, peDim), System.Math.Max(1, n - 1))
            Dim steps As Integer = System.Math.Max(1, System.Math.Min(8, 2 * spdMax))
            Dim rand As New Random(seed Xor &H5EED)

            Dim invDegree As Double() = New Double(n - 1) {}
            Dim stationary As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                If degree(i) > 0.0 Then invDegree(i) = 1.0 / degree(i)

                stationary(i) = degree(i) / totalDegree
            Next

            ' ---- 随机初始向量（Rademacher 分布，固定种子） ----
            Dim vectors As Double()() = New Double(used - 1)() {}
            Dim work As Double() = New Double(n - 1) {}

            For c As Integer = 0 To used - 1
                Dim v As Double() = New Double(n - 1) {}

                For i As Integer = 0 To n - 1
                    v(i) = If(rand.Next(2) = 0, 1.0, -1.0)
                Next

                Call RemoveStationaryComponent(v, stationary)

                vectors(c) = v
            Next

            ' ---- 懒惰随机游走扩散 ----
            For iteration As Integer = 0 To steps - 1
                For c As Integer = 0 To used - 1
                    Dim v As Double() = vectors(c)

                    Call RandomWalkStep(v, adjacency, invDegree, work)
                    Call RemoveStationaryComponent(v, stationary)
                Next
            Next

            ' ---- Gram-Schmidt 正交化 + 单位化，丢弃退化分量 ----
            Dim basis As New List(Of Double())(used)

            For c As Integer = 0 To used - 1
                Dim v As Double() = vectors(c)

                For Each previous As Double() In basis
                    Dim dot As Double = 0.0

                    For i As Integer = 0 To n - 1
                        dot += v(i) * previous(i)
                    Next

                    For i As Integer = 0 To n - 1
                        v(i) -= dot * previous(i)
                    Next
                Next

                Dim sumSquare As Double = 0.0

                For i As Integer = 0 To n - 1
                    sumSquare += v(i) * v(i)
                Next

                Dim norm As Double = System.Math.Sqrt(sumSquare)

                If norm <= 1.0E-09 Then Continue For

                For i As Integer = 0 To n - 1
                    v(i) /= norm
                Next

                Call basis.Add(v)
            Next

            If basis.Count = 0 Then
                Console.WriteLine(
                    $"[GPS] 基因数 {n} 的随机游走位置编码在正交化后没有有效分量，位置编码置零")

                Return (New Double(n - 1, -1) {}, 0)
            End If

            Dim pe As Double(,) = New Double(n - 1, basis.Count - 1) {}

            For c As Integer = 0 To basis.Count - 1
                Dim v As Double() = basis(c)

                For i As Integer = 0 To n - 1
                    pe(i, c) = v(i)
                Next
            Next

            Console.WriteLine(
                $"[GPS] 基因数 {n} 超过 LapPE 稠密分解上限 {LapPeMaxNodes}，" &
                $"改用随机游走位置编码（K={basis.Count}，T={steps} 步）")

            Return (pe, basis.Count)
        End Function

        ''' <summary>
        ''' 去掉向量在平稳分布方向上的分量：<c>v ← v − (Σ π_i·v_i)·1</c>。
        ''' </summary>
        ''' <param name="v">待处理的向量（原地修改）</param>
        ''' <param name="stationary">平稳分布 π（各行和为 1）</param>
        Private Shared Sub RemoveStationaryComponent(v As Double(), stationary As Double())
            Dim mean As Double = 0.0

            For i As Integer = 0 To v.Length - 1
                mean += stationary(i) * v(i)
            Next

            For i As Integer = 0 To v.Length - 1
                v(i) -= mean
            Next
        End Sub

        ''' <summary>
        ''' 一步懒惰随机游走：<c>work ← (v + D⁻¹·A·v) / 2</c>，随后回写到 <paramref name="v"/>。
        ''' </summary>
        ''' <param name="v">当前向量（原地更新）</param>
        ''' <param name="adjacency">无向邻接表</param>
        ''' <param name="invDegree">度数倒数（孤立节点为 0）</param>
        ''' <param name="work">长度为 N 的临时缓冲区（复用，避免每步分配）</param>
        Private Shared Sub RandomWalkStep(v As Double(),
                                          adjacency As List(Of Integer)(),
                                          invDegree As Double(),
                                          work As Double())

            For i As Integer = 0 To v.Length - 1
                Dim sum As Double = 0.0

                For Each j As Integer In adjacency(i)
                    sum += v(j)
                Next

                work(i) = 0.5 * (v(i) + sum * invDegree(i))
            Next

            Call System.Array.Copy(work, v, v.Length)
        End Sub

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
