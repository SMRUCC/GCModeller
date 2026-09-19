' ---------------------------------------------------------------------------
' MultiHeadAttention —— 多头缩放点积注意力（迁移到 TensorFlow\Tensor.vb）
'
' 迁移前依赖 AD 张量的 N 维 MatMul/Concat/Softmax/Mask 与自动微分；
' 迁移后所有张量运算都走 TensorOps 中的手写算子与手写反向传播。
' 前向阶段缓存 Qf/Kf/Vf、softmax 概率与拼接结果，反向阶段逆序回传。
' ---------------------------------------------------------------------------

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Transformer

    Public Class MultiHeadAttention

        Private mask As Boolean
        Private embeddingSize As Integer
        Private nr_heads As Integer       ' Number of attention heads
        Private dk As Integer             ' Key dimension
        Private dv As Integer             ' Value dimension

        ' Learned linear input layers
        Private Qm, Km, Vm As Tensor()

        ' Learned output layer
        Private Wo As Tensor

        Private QmOptimizer, KmOptimizer, VmOptimizer As Optimizer()
        Private WoOptimizer As Optimizer

        ''' <summary>前向传播的中间量缓存，供反向传播使用。</summary>
        Public Class Cache
            Public Qf As Tensor()
            Public Kf As Tensor()
            Public Vf As Tensor()
            ''' <summary>每个 head 的注意力概率（softmax 输出）</summary>
            Public Probs As Tensor()
            ''' <summary>各 head 沿最后一维拼接后的结果</summary>
            Public Concat As Tensor
            ''' <summary>Query 侧的输入（self-attention 时与 K/V 输入相同）</summary>
            Public QueriesInput As Tensor
            ''' <summary>Key 侧的输入</summary>
            Public KInput As Tensor
            ''' <summary>Value 侧的输入</summary>
            Public VInput As Tensor
            ''' <summary>是否为交叉注意力（Q 与 K/V 来自不同输入）</summary>
            Public CrossAttention As Boolean
        End Class

        Private _lastCache As Cache

        ''' <summary>最近一次 <see cref="Update"/> 的中间量缓存。</summary>
        Public ReadOnly Property LastCache As Cache
            Get
                Return _lastCache
            End Get
        End Property

        Public Sub New(dk As Integer, dv As Integer, nr_heads As Integer, embeddingSize As Integer, mask As Boolean)
            Me.dk = dk
            Me.dv = dv
            Me.nr_heads = nr_heads
            Me.embeddingSize = embeddingSize
            Me.mask = mask

            Call InitializeLinearFilters()
            Call InitalizeOptimizers()
        End Sub

        ''' <summary>自注意力：Q/K/V 均来自 <paramref name="inputData"/>。</summary>
        Public Function Update(inputData As Tensor) As Tensor
            Dim Qf As Tensor() = Nothing, Kf As Tensor() = Nothing, Vf As Tensor() = Nothing
            Call ApplyLinearInputFilters(inputData, Qf, Kf, Vf)
            Return CalculateScaledMultiHeadedAttention(Qf, Kf, Vf, inputData, inputData, inputData, False)
        End Function

        ''' <summary>交叉注意力：Q 来自 <paramref name="queries"/>，K/V 来自 <paramref name="encoderOutput"/>。</summary>
        Public Function Update(encoderOutput As Tensor, queries As Tensor) As Tensor
            Dim Qf As Tensor() = Nothing, Kf As Tensor() = Nothing, Vf As Tensor() = Nothing
            Call ApplyLinearInputFilters(encoderOutput, queries, Qf, Kf, Vf)
            Return CalculateScaledMultiHeadedAttention(Qf, Kf, Vf, queries, encoderOutput, encoderOutput, True)
        End Function

        Private Sub ApplyLinearInputFilters(inputData As Tensor, <Out> ByRef Qf As Tensor(), <Out> ByRef Kf As Tensor(), <Out> ByRef Vf As Tensor())
            Qf = New Tensor(nr_heads - 1) {}
            Kf = New Tensor(nr_heads - 1) {}
            Vf = New Tensor(nr_heads - 1) {}

            For h = 0 To nr_heads - 1
                Qf(h) = TensorOps.BatchedMatMul(inputData, Qm(h))
                Kf(h) = TensorOps.BatchedMatMul(inputData, Km(h))
                Vf(h) = TensorOps.BatchedMatMul(inputData, Vm(h))
            Next
        End Sub

        Private Sub ApplyLinearInputFilters(encoderOutput As Tensor, queries As Tensor, <Out> ByRef Qf As Tensor(), <Out> ByRef Kf As Tensor(), <Out> ByRef Vf As Tensor())
            Qf = New Tensor(nr_heads - 1) {}
            Kf = New Tensor(nr_heads - 1) {}
            Vf = New Tensor(nr_heads - 1) {}

            For h = 0 To nr_heads - 1
                Qf(h) = TensorOps.BatchedMatMul(queries, Qm(h))
                Kf(h) = TensorOps.BatchedMatMul(encoderOutput, Km(h))
                Vf(h) = TensorOps.BatchedMatMul(encoderOutput, Vm(h))
            Next
        End Sub

        Private Function CalculateScaledMultiHeadedAttention(Qf As Tensor(), Kf As Tensor(), Vf As Tensor(),
                                                             queriesInput As Tensor, kInput As Tensor, vInput As Tensor,
                                                             crossAttention As Boolean) As Tensor
            Dim AttentionHeads = New Tensor(nr_heads - 1) {}
            Dim probabilities = New Tensor(nr_heads - 1) {}
            Dim scale = 1.0 / std.Sqrt(dk)

            For h = 0 To nr_heads - 1
                Dim AttentionFilter As Tensor = TensorOps.BatchedMatMul(Qf(h), TensorOps.TransposeLastTwo(Kf(h)))
                Dim scaledAttentionFilter = TensorOps.Scale(AttentionFilter, scale)

                If mask Then Call TensorOps.MaskUpperTriangular(scaledAttentionFilter)

                probabilities(h) = TensorOps.SoftmaxLastDim(scaledAttentionFilter)
                AttentionHeads(h) = TensorOps.BatchedMatMul(probabilities(h), Vf(h))
            Next

            ' Apply linear output layer to get the correct output size
            Dim C = TensorOps.ConcatLastDim(AttentionHeads)

            _lastCache = New Cache With {
                .Qf = Qf,
                .Kf = Kf,
                .Vf = Vf,
                .Probs = probabilities,
                .Concat = C,
                .QueriesInput = queriesInput,
                .KInput = kInput,
                .VInput = vInput,
                .CrossAttention = crossAttention
            }

            Return TensorOps.BatchedMatMul(C, Wo)
        End Function

        ''' <summary>
        ''' 反向传播：返回对 Query 侧输入的梯度，并累加全部线性层的参数梯度。
        ''' </summary>
        ''' <param name="dOut">对注意力输出（<c>Concat · Wo</c>）的梯度</param>
        ''' <param name="dEncoderOutput">交叉注意力场景下对 encoder 输出的梯度（自注意力时为 Nothing）</param>
        Public Function Backward(dOut As Tensor, ByRef dEncoderOutput As Tensor) As Tensor
            Dim cache = _lastCache

            If cache Is Nothing Then Throw New InvalidOperationException("必须先执行前向传播才能反向传播")

            ' 输出投影层
            Dim dConcat As Tensor = Nothing, dWo As Tensor = Nothing
            Call TensorOps.BatchedMatMulBackward(dOut, cache.Concat, Wo, dConcat, dWo)
            Call TensorOps.Accumulate(WoOptimizer.Gradient, dWo)

            Dim dAttentionHeads = TensorOps.SplitLastDim(dConcat, nr_heads)
            Dim scale = 1.0 / std.Sqrt(dk)
            Dim dQueries As Tensor = Nothing
            Dim dEncoder As Tensor = Nothing

            For h = 0 To nr_heads - 1
                ' 注意力输出：head = probs · Vf
                Dim dProbs As Tensor = Nothing, dVf As Tensor = Nothing
                Call TensorOps.BatchedMatMulBackward(dAttentionHeads(h), cache.Probs(h), cache.Vf(h), dProbs, dVf)

                ' V 投影
                Dim dVInput As Tensor = Nothing, dVm As Tensor = Nothing
                Call TensorOps.BatchedMatMulBackward(dVf, cache.VInput, Vm(h), dVInput, dVm)
                Call TensorOps.Accumulate(VmOptimizer(h).Gradient, dVm)

                ' softmax 反向 + 缩放（掩码位置 softmax 输出为 0，其梯度自然为 0）
                Dim dScore = TensorOps.SoftmaxBackward(cache.Probs(h), dProbs)
                Call TensorOps.ScaleInPlace(dScore, scale)

                ' 注意力分数：score = Qf · Kfᵀ
                Dim dQf As Tensor = Nothing, dKfT As Tensor = Nothing
                Call TensorOps.BatchedMatMulBackward(dScore, cache.Qf(h), TensorOps.TransposeLastTwo(cache.Kf(h)), dQf, dKfT)
                Dim dKf = TensorOps.TransposeLastTwo(dKfT)

                ' Q 投影
                Dim dQInput As Tensor = Nothing, dQm As Tensor = Nothing
                Call TensorOps.BatchedMatMulBackward(dQf, cache.QueriesInput, Qm(h), dQInput, dQm)
                Call TensorOps.Accumulate(QmOptimizer(h).Gradient, dQm)

                ' K 投影
                Dim dKInput As Tensor = Nothing, dKm As Tensor = Nothing
                Call TensorOps.BatchedMatMulBackward(dKf, cache.KInput, Km(h), dKInput, dKm)
                Call TensorOps.Accumulate(KmOptimizer(h).Gradient, dKm)

                If dQueries Is Nothing Then dQueries = TensorOps.ZerosLike(dQInput)
                Call TensorOps.Accumulate(dQueries, dQInput)

                If cache.CrossAttention Then
                    If dEncoder Is Nothing Then dEncoder = TensorOps.ZerosLike(dKInput)

                    Call TensorOps.Accumulate(dEncoder, dKInput)
                    Call TensorOps.Accumulate(dEncoder, dVInput)
                Else
                    Call TensorOps.Accumulate(dQueries, dKInput)
                    Call TensorOps.Accumulate(dQueries, dVInput)
                End If
            Next

            dEncoderOutput = dEncoder

            Return dQueries
        End Function

        ''' <summary>清零本层所有参数的梯度累加器。</summary>
        Public Sub ZeroGradients()
            For h = 0 To nr_heads - 1
                QmOptimizer(h).ZeroGrad()
                KmOptimizer(h).ZeroGrad()
                VmOptimizer(h).ZeroGrad()
            Next

            WoOptimizer.ZeroGrad()
        End Sub

        Public Sub MakeTrainingStep(learningRate As Double, [step] As Integer)
            For h = 0 To nr_heads - 1
                KmOptimizer(h).MakeTrainingStep(learningRate, [step], Km(h))
                QmOptimizer(h).MakeTrainingStep(learningRate, [step], Qm(h))
                VmOptimizer(h).MakeTrainingStep(learningRate, [step], Vm(h))
            Next
            WoOptimizer.MakeTrainingStep(learningRate, [step], Wo)
        End Sub

        Private Sub InitializeLinearFilters()
            Qm = New Tensor(nr_heads - 1) {}
            Km = New Tensor(nr_heads - 1) {}
            Vm = New Tensor(nr_heads - 1) {}

            For h = 0 To nr_heads - 1
                Qm(h) = TensorOps.HeNormalInit(New Integer() {embeddingSize, dk})
                Km(h) = TensorOps.HeNormalInit(New Integer() {embeddingSize, dk})
                Vm(h) = TensorOps.HeNormalInit(New Integer() {embeddingSize, dv})
            Next

            Wo = TensorOps.HeNormalInit(New Integer() {dv * nr_heads, embeddingSize})
        End Sub

        Private Sub InitalizeOptimizers()
            QmOptimizer = New Optimizer(nr_heads - 1) {}
            KmOptimizer = New Optimizer(nr_heads - 1) {}
            VmOptimizer = New Optimizer(nr_heads - 1) {}

            For h = 0 To nr_heads - 1
                QmOptimizer(h) = New Optimizer(Qm(h))
                KmOptimizer(h) = New Optimizer(Km(h))
                VmOptimizer(h) = New Optimizer(Vm(h))
            Next

            WoOptimizer = New Optimizer(Wo)
        End Sub

    End Class
End Namespace
