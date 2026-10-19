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

                ' ---- 输入投影 ----
                Dim dQInput As Tensor = Nothing
                Dim dQm As Tensor = Nothing

                Call TensorOps.BatchedMatMulBackward(dQf, cache.Input, Qm(h), dQInput, dQm)
                Call TensorOps.Accumulate(qEntry(h).Gradient, dQm)

                Dim dKInput As Tensor = Nothing
                Dim dKm As Tensor = Nothing

                Call TensorOps.BatchedMatMulBackward(dKf, cache.Input, Km(h), dKInput, dKm)
                Call TensorOps.Accumulate(kEntry(h).Gradient, dKm)

                Dim dVInput As Tensor = Nothing
                Dim dVm As Tensor = Nothing

                Call TensorOps.BatchedMatMulBackward(dVf, cache.Input, Vm(h), dVInput, dVm)
                Call TensorOps.Accumulate(vEntry(h).Gradient, dVm)

                If dInput Is Nothing Then dInput = TensorOps.ZerosLike(dQInput)

                Call TensorOps.Accumulate(dInput, dQInput)
                Call TensorOps.Accumulate(dInput, dKInput)
                Call TensorOps.Accumulate(dInput, dVInput)
            Next

            Return dInput
        End Function

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
