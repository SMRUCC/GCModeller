Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.Analysis.GPS.Graph
Imports std = System.Math

Namespace Model

    ''' <summary>
    ''' 带 Graphormer 空间偏置的多头自注意力（method.md §3.1 范式 A ②）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 与 <c>Transformer.MultiHeadAttention</c> 的唯一结构差异是注意力分数多了一个
    ''' 可学习的加性偏置：
    ''' <code>
    ''' score(i,j) = Q_i·K_jᵀ / sqrt(dk) + biasTable[SPD(i,j), head]
    ''' </code>
    ''' 其中 <c>SPD</c> 是由 <see cref="Graph.GraphEncoding"/> 预计算的桶索引矩阵。
    ''' 偏置按"桶 × 头"共享，因此参数量只有 <c>(SPD桶数 × 头数)</c>，
    ''' 且同一层内所有基因对通过距离桶共享统计强度。
    ''' </para>
    ''' <para>
    ''' 之所以新写而不修改 <c>MultiHeadAttention</c>：旧类被既有 Transformer 代码依赖，
    ''' 且其 <c>Backward</c> 没有偏置梯度通路。新类完全照抄旧类的张量布局与反向次序，
    ''' 只增加一张偏置表与其 scatter-add 反向，回归风险为零。
    ''' </para>
    ''' <para>
    ''' 加权与投影全部走 <see cref="TensorOps"/>：GEMM 内部已经接入
    ''' <c>MixedPrecisionOps</c> 外观，规模达标时自动降单精度计算。
    ''' 偏置表、softmax 与各层缓存保持双精度。
    ''' </para>
    ''' </remarks>
    Public Class BiasedMultiHeadAttention

        Private ReadOnly dk As Integer
        Private ReadOnly dv As Integer
        Private ReadOnly nrHeads As Integer
        Private ReadOnly embeddingSize As Integer
        Private ReadOnly spdBucketCount As Integer
        Private ReadOnly paramName As String

        ' 每头独立的输入投影
        Private ReadOnly Qm As Tensor()
        Private ReadOnly Km As Tensor()
        Private ReadOnly Vm As Tensor()

        ' 输出投影 [dv * heads, d]
        Private ReadOnly Wo As Tensor

        ' 空间偏置查找表 [SPD桶数, 头数]
        Private ReadOnly spdBiasTable As Tensor

        Private ReadOnly qEntry As ParameterEntry()
        Private ReadOnly kEntry As ParameterEntry()
        Private ReadOnly vEntry As ParameterEntry()
        Private ReadOnly woEntry As ParameterEntry
        Private ReadOnly biasEntry As ParameterEntry

        ''' <summary>
        ''' 一次前向传播的中间量，反向传播时需要全部回读。
        ''' </summary>
        Public Class Cache
            ''' <summary>输入张量 [N, d]</summary>
            Public Input As Tensor
            ''' <summary>各头的查询投影 [N, dk]</summary>
            Public Qf As Tensor()
            ''' <summary>各头的键投影 [N, dk]</summary>
            Public Kf As Tensor()
            ''' <summary>各头的值投影 [N, dv]</summary>
            Public Vf As Tensor()
            ''' <summary>各头键投影的转置 [dk, N]（反向复用）</summary>
            Public KfT As Tensor()
            ''' <summary>
            ''' 各头的 softmax 注意力权重：稠密模式为 <c>[N, N]</c>，
            ''' 稀疏模式为扁平的 <c>[N, c]</c>（按行存放候选权重）。
            ''' </summary>
            Public Probs As Tensor()
            ''' <summary>各头输出按最后一维拼接后的结果 [N, dv*heads]</summary>
            Public Concat As Tensor
            ''' <summary>本次前向使用的 SPD 桶索引矩阵 [N, N]（稀疏模式下为 Nothing）</summary>
            Public SpdBuckets As Integer(,)
            ''' <summary>本次前向使用的稀疏候选模式（稠密模式下为 Nothing）</summary>
            Public Pattern As SparseAttentionPattern
        End Class

        Private _lastCache As Cache

        ''' <summary>最近一次前向传播的缓存（调试与反向传播使用）</summary>
        Public ReadOnly Property LastCache As Cache
            Get
                Return _lastCache
            End Get
        End Property

        ''' <summary>空间偏置查找表 [SPD桶数, 头数]</summary>
        Public ReadOnly Property BiasTable As Tensor
            Get
                Return spdBiasTable
            End Get
        End Property

        ''' <summary>输出投影权重 [dv*heads, d]</summary>
        Public ReadOnly Property OutputWeight As Tensor
            Get
                Return Wo
            End Get
        End Property

        ''' <summary>头数</summary>
        Public ReadOnly Property NumHeads As Integer
            Get
                Return nrHeads
            End Get
        End Property

        ''' <summary>
        ''' 创建带空间偏置的多头自注意力层。
        ''' </summary>
        ''' <param name="dk">每头查询/键维度</param>
        ''' <param name="dv">每头值维度</param>
        ''' <param name="nrHeads">头数</param>
        ''' <param name="embeddingSize">模型隐藏维度</param>
        ''' <param name="spdBucketCount">SPD 桶总数（含自身、远距离与不连通桶）</param>
        ''' <param name="store">参数登记表</param>
        ''' <param name="name">参数名前缀</param>
        Public Sub New(dk As Integer,
                       dv As Integer,
                       nrHeads As Integer,
                       embeddingSize As Integer,
                       spdBucketCount As Integer,
                       store As ParameterStore,
                       Optional name As String = "attention")

            If store Is Nothing Then Throw New ArgumentNullException(NameOf(store))
            If nrHeads <= 0 Then Throw New ArgumentException("头数必须为正", NameOf(nrHeads))
            If spdBucketCount < 1 Then Throw New ArgumentException("SPD 桶数必须为正", NameOf(spdBucketCount))

            Me.dk = dk
            Me.dv = dv
            Me.nrHeads = nrHeads
            Me.embeddingSize = embeddingSize
            Me.spdBucketCount = spdBucketCount
            Me.paramName = name

            Qm = New Tensor(nrHeads - 1) {}
            Km = New Tensor(nrHeads - 1) {}
            Vm = New Tensor(nrHeads - 1) {}
            qEntry = New ParameterEntry(nrHeads - 1) {}
            kEntry = New ParameterEntry(nrHeads - 1) {}
            vEntry = New ParameterEntry(nrHeads - 1) {}

            For h As Integer = 0 To nrHeads - 1
                Qm(h) = TensorOps.HeNormalInit(New Integer() {embeddingSize, dk})
                Km(h) = TensorOps.HeNormalInit(New Integer() {embeddingSize, dk})
                Vm(h) = TensorOps.HeNormalInit(New Integer() {embeddingSize, dv})

                qEntry(h) = store.Register($"{name}.q{h}", Qm(h))
                kEntry(h) = store.Register($"{name}.k{h}", Km(h))
                vEntry(h) = store.Register($"{name}.v{h}", Vm(h))
            Next

            Wo = TensorOps.HeNormalInit(New Integer() {dv * nrHeads, embeddingSize})
            woEntry = store.Register($"{name}.wo", Wo)

            ' 偏置表从零初始化（Graphormer 的常规做法）：初始时注意力退化为纯内容注意力，
            ' 由训练数据决定"近距离基因对是否应获得更高权重"
            spdBiasTable = New Tensor(spdBucketCount, nrHeads)
            biasEntry = store.Register($"{name}.spdBias", spdBiasTable)
        End Sub

        ''' <summary>
        ''' 前向传播：自注意力 + 空间偏置。
        ''' </summary>
        ''' <param name="input">节点嵌入 [N, d]</param>
        ''' <param name="spdBuckets">SPD 桶索引矩阵 [N, N]</param>
        ''' <returns>注意力输出 [N, d]</returns>
        Public Function Forward(input As Tensor, spdBuckets As Integer(,)) As Tensor
            Return ForwardCore(input, spdBuckets, Nothing)
        End Function

        ''' <summary>
        ''' 前向传播（稀疏候选模式）：每个基因只在固定候选集上做 softmax 注意力。
        ''' </summary>
        ''' <param name="input">节点嵌入 [N, d]</param>
        ''' <param name="pattern">稀疏候选模式（N 超过稠密上限时使用）</param>
        ''' <returns>注意力输出 [N, d]</returns>
        ''' <remarks>
        ''' 与稠密重载共享同一组投影参数与空间偏置表，只是把注意力范围从"N 个节点"
        ''' 换成"每行 c 个候选"，因此权重缓存是 <c>[N, c]</c> 而不是 <c>[N, N]</c>。
        ''' </remarks>
        Public Function Forward(input As Tensor, pattern As SparseAttentionPattern) As Tensor
            Return ForwardCore(input, Nothing, pattern)
        End Function

        ''' <summary>前向传播的公共实现：按传入的桶矩阵 / 候选模式选择稠密或稀疏路径。</summary>
        ''' <param name="input">节点嵌入 [N, d]</param>
        ''' <param name="spdBuckets">稠密 SPD 桶矩阵；稀疏模式下传 Nothing</param>
        ''' <param name="pattern">稀疏候选模式；稠密模式下传 Nothing</param>
        ''' <returns>注意力输出 [N, d]</returns>
        Private Function ForwardCore(input As Tensor,
                                     spdBuckets As Integer(,),
                                     pattern As SparseAttentionPattern) As Tensor

            If input Is Nothing Then Throw New ArgumentNullException(NameOf(input))

            If spdBuckets Is Nothing AndAlso pattern Is Nothing Then
                Throw New ArgumentException("必须提供稠密 SPD 桶矩阵或稀疏候选模式")
            End If

            Dim n As Integer = input.Shape(0)

            If pattern IsNot Nothing Then
                If pattern.NGene <> n Then
                    Throw New ArgumentException(
                        $"稀疏候选模式的节点数 {pattern.NGene} 与输入 {n} 不一致")
                End If
            Else
                If spdBuckets.GetLength(0) <> n OrElse spdBuckets.GetLength(1) <> n Then
                    Throw New ArgumentException(
                        $"SPD 桶矩阵尺寸应为 [{n},{n}]，实际 [{spdBuckets.GetLength(0)},{spdBuckets.GetLength(1)}]")
                End If
            End If

            Dim Qf As Tensor() = New Tensor(nrHeads - 1) {}
            Dim Kf As Tensor() = New Tensor(nrHeads - 1) {}
            Dim Vf As Tensor() = New Tensor(nrHeads - 1) {}
            Dim KfT As Tensor() = New Tensor(nrHeads - 1) {}
            Dim probs As Tensor() = New Tensor(nrHeads - 1) {}
            Dim heads As Tensor() = New Tensor(nrHeads - 1) {}
            Dim scale As Double = 1.0 / std.Sqrt(dk)

            For h As Integer = 0 To nrHeads - 1
                Qf(h) = TensorOps.BatchedMatMul(input, Qm(h))
                Kf(h) = TensorOps.BatchedMatMul(input, Km(h))
                Vf(h) = TensorOps.BatchedMatMul(input, Vm(h))

                If pattern Is Nothing Then
                    KfT(h) = TensorOps.TransposeLastTwo(Kf(h))

                    Dim scores As Tensor = TensorOps.Scale(TensorOps.BatchedMatMul(Qf(h), KfT(h)), scale)

                    Call AddBias(scores, spdBuckets, h)

                    probs(h) = TensorOps.SoftmaxLastDim(scores)
                    heads(h) = TensorOps.BatchedMatMul(probs(h), Vf(h))
                Else
                    Dim weights As Tensor = New Tensor(n, pattern.CandidateCount)
                    Dim headOutput As Tensor = New Tensor(n, dv)

                    Call SparseAttentionForward(Qf(h), Kf(h), Vf(h), pattern, h, weights, headOutput)

                    probs(h) = weights
                    heads(h) = headOutput
                End If
            Next

            Dim concat As Tensor = TensorOps.ConcatLastDim(heads)

            _lastCache = New Cache With {
                .Input = input,
                .Qf = Qf,
                .Kf = Kf,
                .Vf = Vf,
                .KfT = KfT,
                .Probs = probs,
                .Concat = concat,
                .SpdBuckets = spdBuckets,
                .Pattern = pattern
            }

            Return TensorOps.BatchedMatMul(concat, Wo)
        End Function

        ''' <summary>
        ''' 反向传播：累积各投影与偏置表的梯度，返回输入梯度。
        ''' </summary>
        ''' <param name="dOut">对注意力输出的梯度 [N, d]</param>
        ''' <returns>对输入的梯度 [N, d]</returns>
        Public Function Backward(dOut As Tensor) As Tensor
            Dim cache As Cache = _lastCache

            If cache Is Nothing Then
                Throw New InvalidOperationException("必须先执行前向传播才能反向传播")
            End If

            ' ---- 输出投影 ----
            Dim dConcat As Tensor = Nothing
            Dim dWo As Tensor = Nothing

            Call TensorOps.BatchedMatMulBackward(dOut, cache.Concat, Wo, dConcat, dWo)
            Call TensorOps.Accumulate(woEntry.Gradient, dWo)

            Dim dHeads As Tensor() = TensorOps.SplitLastDim(dConcat, nrHeads)
            Dim scale As Double = 1.0 / std.Sqrt(dk)
            Dim dInput As Tensor = Nothing

            If cache.Pattern Is Nothing Then
                ' ==================== 稠密路径（N×N 注意力权重） ====================
                For h As Integer = 0 To nrHeads - 1
                    ' ---- 头输出：head = probs · Vf ----
                    Dim dProbs As Tensor = Nothing
                    Dim dVf As Tensor = Nothing

                    Call TensorOps.BatchedMatMulBackward(dHeads(h), cache.Probs(h), cache.Vf(h), dProbs, dVf)

                    ' ---- softmax 反向 ----
                    ' 注意：偏置是在缩放之后加到分数上的，因此偏置表的梯度必须使用
                    ' 缩放前的 dScore；若先缩放再累积，偏置梯度会被缩小 1/sqrt(dk) 倍
                    Dim dScore As Tensor = TensorOps.SoftmaxBackward(cache.Probs(h), dProbs)

                    ' ---- 偏置表梯度：按 SPD 桶聚合（缩放前） ----
                    Call AccumulateBiasGradient(dScore, cache.SpdBuckets, h)

                    ' ---- 缩放，得到对 Qf·Kfᵀ 的梯度 ----
                    Call TensorOps.ScaleInPlace(dScore, scale)

                    ' ---- 分数 = Qf · Kfᵀ ----
                    Dim dQf As Tensor = Nothing
                    Dim dKfT As Tensor = Nothing

                    Call TensorOps.BatchedMatMulBackward(dScore, cache.Qf(h), cache.KfT(h), dQf, dKfT)
                    Dim dKf As Tensor = TensorOps.TransposeLastTwo(dKfT)

                    Call AccumulateProjectionGradients(h, dQf, dKf, dVf, dInput)
                Next
            Else
                ' ==================== 稀疏路径（N×c 注意力权重） ====================
                Dim n As Integer = cache.Pattern.NGene

                For h As Integer = 0 To nrHeads - 1
                    Dim dQf As Tensor = New Tensor(n, dk)
                    Dim dKf As Tensor = New Tensor(n, dk)
                    Dim dVf As Tensor = New Tensor(n, dv)

                    Call SparseAttentionBackward(cache, h, dHeads(h), dQf, dKf, dVf)
                    Call AccumulateProjectionGradients(h, dQf, dKf, dVf, dInput)
                Next
            End If

            Return dInput
        End Function

        ''' <summary>
        ''' 输入投影的反向：把 Qf/Kf/Vf 的梯度回传到输入并累积三组投影权重梯度。
        ''' </summary>
        ''' <param name="head">头编号</param>
        ''' <param name="dQf">查询投影梯度 [N, dk]</param>
        ''' <param name="dKf">键投影梯度 [N, dk]</param>
        ''' <param name="dVf">值投影梯度 [N, dv]</param>
        ''' <param name="dInput">输入梯度累加目标（首次调用时为 Nothing，内部初始化）</param>
        Private Sub AccumulateProjectionGradients(head As Integer,
                                                  dQf As Tensor,
                                                  dKf As Tensor,
                                                  dVf As Tensor,
                                                  ByRef dInput As Tensor)

            Dim cache As Cache = _lastCache

            Dim dQInput As Tensor = Nothing
            Dim dQm As Tensor = Nothing

            Call TensorOps.BatchedMatMulBackward(dQf, cache.Input, Qm(head), dQInput, dQm)
            Call TensorOps.Accumulate(qEntry(head).Gradient, dQm)

            Dim dKInput As Tensor = Nothing
            Dim dKm As Tensor = Nothing

            Call TensorOps.BatchedMatMulBackward(dKf, cache.Input, Km(head), dKInput, dKm)
            Call TensorOps.Accumulate(kEntry(head).Gradient, dKm)

            Dim dVInput As Tensor = Nothing
            Dim dVm As Tensor = Nothing

            Call TensorOps.BatchedMatMulBackward(dVf, cache.Input, Vm(head), dVInput, dVm)
            Call TensorOps.Accumulate(vEntry(head).Gradient, dVm)

            If dInput Is Nothing Then dInput = TensorOps.ZerosLike(dQInput)

            Call TensorOps.Accumulate(dInput, dQInput)
            Call TensorOps.Accumulate(dInput, dKInput)
            Call TensorOps.Accumulate(dInput, dVInput)
        End Sub

        ''' <summary>
        ''' 稀疏注意力前向：每行只在 <c>c</c> 个候选上做 softmax，输出按加权和写回。
        ''' </summary>
        ''' <param name="qf">查询投影 [N, dk]</param>
        ''' <param name="kf">键投影 [N, dk]</param>
        ''' <param name="vf">值投影 [N, dv]</param>
        ''' <param name="pattern">稀疏候选模式</param>
        ''' <param name="head">头编号</param>
        ''' <param name="weights">输出：注意力权重 [N, c]</param>
        ''' <param name="output">输出：头输出 [N, dv]</param>
        ''' <remarks>
        ''' 直接按候选下标读取 <c>K/V</c>，不构造 <c>[N, c, dk]</c> 的中间张量，
        ''' 因此额外内存只有 <c>O(N·c)</c> 的权重缓存与每线程 <c>O(c + dv)</c> 的 scratch。
        ''' </remarks>
        Private Sub SparseAttentionForward(qf As Tensor,
                                           kf As Tensor,
                                           vf As Tensor,
                                           pattern As SparseAttentionPattern,
                                           head As Integer,
                                           weights As Tensor,
                                           output As Tensor)

            Dim n As Integer = pattern.NGene
            Dim c As Integer = pattern.CandidateCount
            Dim dk As Integer = Me.dk
            Dim dv As Integer = Me.dv
            Dim scale As Double = 1.0 / std.Sqrt(dk)

            Dim targets As Integer(,) = pattern.Targets
            Dim buckets As Integer(,) = pattern.Buckets
            Dim q As Double() = qf.Data
            Dim k As Double() = kf.Data
            Dim v As Double() = vf.Data
            Dim bias As Double() = spdBiasTable.Data
            Dim w As Double() = weights.Data
            Dim o As Double() = output.Data
            Dim heads As Integer = nrHeads

            Call ParallelChunks(n,
                Sub(first As Integer, last As Integer)
                    Dim scores As Double() = New Double(c - 1) {}
                    Dim acc As Double() = New Double(dv - 1) {}

                    For i As Integer = first To last
                        Dim qBase As Integer = i * dk
                        Dim maxScore As Double = Double.NegativeInfinity

                        For j As Integer = 0 To c - 1
                            Dim kBase As Integer = targets(i, j) * dk
                            Dim score As Double = 0.0

                            For x As Integer = 0 To dk - 1
                                score += q(qBase + x) * k(kBase + x)
                            Next

                            score = score * scale + bias(buckets(i, j) * heads + head)
                            scores(j) = score

                            If score > maxScore Then maxScore = score
                        Next

                        Dim sum As Double = 0.0

                        For j As Integer = 0 To c - 1
                            Dim e As Double = std.Exp(scores(j) - maxScore)

                            scores(j) = e
                            sum += e
                        Next

                        Dim inv As Double = If(sum > 0.0, 1.0 / sum, 0.0)
                        Dim row As Integer = i * c

                        For x As Integer = 0 To dv - 1
                            acc(x) = 0.0
                        Next

                        For j As Integer = 0 To c - 1
                            Dim p As Double = scores(j) * inv

                            w(row + j) = p

                            Dim vBase As Integer = targets(i, j) * dv

                            For x As Integer = 0 To dv - 1
                                acc(x) += p * v(vBase + x)
                            Next
                        Next

                        Dim oBase As Integer = i * dv

                        For x As Integer = 0 To dv - 1
                            o(oBase + x) = acc(x)
                        Next
                    Next
                End Sub)

            Call weights.MarkHostModified()
            Call output.MarkHostModified()
        End Sub

        ''' <summary>
        ''' 稀疏注意力反向：softmax、偏置表与 Q/K/V 的梯度。
        ''' </summary>
        ''' <param name="cache">前向缓存（必须来自稀疏模式）</param>
        ''' <param name="head">头编号</param>
        ''' <param name="dOut">头输出梯度 [N, dv]</param>
        ''' <param name="dQf">输出：查询投影梯度 [N, dk]</param>
        ''' <param name="dKf">输出：键投影梯度 [N, dk]</param>
        ''' <param name="dVf">输出：值投影梯度 [N, dv]</param>
        ''' <remarks>
        ''' 写竞争处理：
        ''' <list type="bullet">
        ''' <item><description><c>dQ</c> 按行并行，每行只写自己的 <c>dk</c> 段；</description></item>
        ''' <item><description><c>dK/dV</c> 按<b>目标节点</b>并行，通过反向索引把"谁关注了我"聚合起来，
        ''' 每个目标行只被一个线程写；</description></item>
        ''' <item><description>偏置表梯度按桶聚合，同一桶会被多行写入，因此串行执行（O(N·c)，
        ''' 相对注意力本身可忽略）。</description></item>
        ''' </list>
        ''' </remarks>
        Private Sub SparseAttentionBackward(cache As Cache,
                                            head As Integer,
                                            dOut As Tensor,
                                            dQf As Tensor,
                                            dKf As Tensor,
                                            dVf As Tensor)

            Dim pattern As SparseAttentionPattern = cache.Pattern
            Dim n As Integer = pattern.NGene
            Dim c As Integer = pattern.CandidateCount
            Dim dk As Integer = Me.dk
            Dim dv As Integer = Me.dv
            Dim scale As Double = 1.0 / std.Sqrt(dk)

            Dim targets As Integer(,) = pattern.Targets
            Dim buckets As Integer(,) = pattern.Buckets
            Dim reverseOffsets As Integer() = pattern.ReverseOffsets
            Dim reverseSources As Integer() = pattern.ReverseSources

            Dim q As Double() = cache.Qf(head).Data
            Dim k As Double() = cache.Kf(head).Data
            Dim v As Double() = cache.Vf(head).Data
            Dim w As Double() = cache.Probs(head).Data
            Dim go As Double() = dOut.Data
            Dim heads As Integer = nrHeads

            Dim dScore As Double() = New Double(n * c - 1) {}

            ' ---- 1. softmax 反向：dScore_ij = p_ij·(dp_ij − Σ_k p_ik·dp_ik) ----
            Call ParallelChunks(n,
                Sub(first As Integer, last As Integer)
                    Dim dProbs As Double() = New Double(c - 1) {}

                    For i As Integer = first To last
                        Dim oBase As Integer = i * dv
                        Dim dot As Double = 0.0

                        For j As Integer = 0 To c - 1
                            Dim vBase As Integer = targets(i, j) * dv
                            Dim dp As Double = 0.0

                            For x As Integer = 0 To dv - 1
                                dp += go(oBase + x) * v(vBase + x)
                            Next

                            dProbs(j) = dp
                            dot += w(i * c + j) * dp
                        Next

                        For j As Integer = 0 To c - 1
                            Dim p As Double = w(i * c + j)

                            dScore(i * c + j) = p * (dProbs(j) - dot)
                        Next
                    Next
                End Sub)

            ' ---- 2. 偏置表梯度（缩放前聚合，与稠密路径口径一致） ----
            Dim biasGrad As Double() = biasEntry.Gradient.Data

            For i As Integer = 0 To n - 1
                Dim row As Integer = i * c

                For j As Integer = 0 To c - 1
                    biasGrad(buckets(i, j) * heads + head) += dScore(row + j)
                Next
            Next

            Call biasEntry.Gradient.MarkHostModified()

            ' ---- 3. dQ：每行独立 ----
            Dim dq As Double() = dQf.Data

            Call ParallelChunks(n,
                Sub(first As Integer, last As Integer)
                    For i As Integer = first To last
                        Dim qBase As Integer = i * dk
                        Dim row As Integer = i * c

                        For j As Integer = 0 To c - 1
                            Dim weight As Double = dScore(row + j) * scale

                            If weight = 0.0 Then Continue For

                            Dim kBase As Integer = targets(i, j) * dk

                            For x As Integer = 0 To dk - 1
                                dq(qBase + x) += weight * k(kBase + x)
                            Next
                        Next
                    Next
                End Sub)

            Call dQf.MarkHostModified()

            ' ---- 4. dK/dV：按目标节点并行（反向索引，单写者） ----
            Dim dkData As Double() = dKf.Data
            Dim dvData As Double() = dVf.Data

            Call ParallelChunks(n,
                Sub(first As Integer, last As Integer)
                    For target As Integer = first To last
                        Dim dkBase As Integer = target * dk
                        Dim dvBase As Integer = target * dv

                        For s As Integer = reverseOffsets(target) To reverseOffsets(target + 1) - 1
                            Dim encoded As Integer = reverseSources(s)
                            Dim i As Integer = encoded \ c
                            Dim slot As Integer = encoded - i * c
                            Dim weight As Double = dScore(i * c + slot) * scale
                            Dim qBase As Integer = i * dk

                            For x As Integer = 0 To dk - 1
                                dkData(dkBase + x) += weight * q(qBase + x)
                            Next

                            Dim p As Double = w(i * c + slot)
                            Dim oBase As Integer = i * dv

                            For x As Integer = 0 To dv - 1
                                dvData(dvBase + x) += p * go(oBase + x)
                            Next
                        Next
                    Next
                End Sub)

            Call dKf.MarkHostModified()
            Call dVf.MarkHostModified()
        End Sub

        ''' <summary>
        ''' 把 <c>[0, n)</c> 切成若干块并并行执行；每块调用一次委托，
        ''' 便于委托内部创建"每线程一份"的 scratch 数组。
        ''' </summary>
        ''' <param name="n">元素总数</param>
        ''' <param name="body">块执行体，参数为 <c>(first, last)</c>（闭区间）</param>
        Private Shared Sub ParallelChunks(n As Integer, body As System.Action(Of Integer, Integer))
            If n <= 0 Then Return

            Dim chunk As Integer = System.Math.Max(1, n \ System.Math.Max(1, Environment.ProcessorCount * 4))
            Dim chunks As Integer = (n + chunk - 1) \ chunk

            Call System.Threading.Tasks.Parallel.For(0, chunks,
                Sub(ci As Integer)
                    Dim first As Integer = ci * chunk
                    Dim last As Integer = System.Math.Min(n, first + chunk) - 1

                    Call body(first, last)
                End Sub)
        End Sub

        ''' <summary>
        ''' 把第 <paramref name="head"/> 个头的空间偏置加到注意力分数上（原地）。
        ''' </summary>
        ''' <param name="scores">缩放后的注意力分数 [N, N]</param>
        ''' <param name="spdBuckets">SPD 桶索引矩阵 [N, N]</param>
        ''' <param name="head">头编号</param>
        Private Sub AddBias(scores As Tensor, spdBuckets As Integer(,), head As Integer)
            Dim n As Integer = scores.Shape(0)
            Dim sd As Double() = scores.Data
            Dim bd As Double() = spdBiasTable.Data
            Dim heads As Integer = nrHeads

            For i As Integer = 0 To n - 1
                Dim row As Integer = i * n

                For j As Integer = 0 To n - 1
                    sd(row + j) += bd(spdBuckets(i, j) * heads + head)
                Next
            Next

            Call scores.MarkHostModified()
        End Sub

        ''' <summary>
        ''' 把分数梯度按 SPD 桶聚合回偏置表梯度。
        ''' </summary>
        ''' <param name="dScore">softmax 反向并缩放后的分数梯度 [N, N]</param>
        ''' <param name="spdBuckets">SPD 桶索引矩阵 [N, N]</param>
        ''' <param name="head">头编号</param>
        Private Sub AccumulateBiasGradient(dScore As Tensor, spdBuckets As Integer(,), head As Integer)
            Dim n As Integer = dScore.Shape(0)
            Dim gd As Double() = dScore.Data
            Dim bg As Double() = biasEntry.Gradient.Data
            Dim heads As Integer = nrHeads

            For i As Integer = 0 To n - 1
                Dim row As Integer = i * n

                For j As Integer = 0 To n - 1
                    bg(spdBuckets(i, j) * heads + head) += gd(row + j)
                Next
            Next

            Call biasEntry.Gradient.MarkHostModified()
        End Sub

    End Class

End Namespace
