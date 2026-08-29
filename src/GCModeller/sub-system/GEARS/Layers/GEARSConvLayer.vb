Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.GEARS.Graph
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN

Namespace Layers

    ''' <summary>
    ''' GEARS 边类型感知的图卷积层
    ''' </summary>
    ''' <remarks>
    ''' 实现 readme 第五节的「带边类型感知的消息传递」：
    ''' <code>
    ''' h_i^(l+1) = σ( W_self · h_i^(l) + Σ_{j ∈ N_in(i)} α_ji · sign(r_ji) · (W_{r_ji} · h_j^(l)) )
    ''' </code>
    '''
    ''' 要点：
    ''' <list type="bullet">
    ''' <item><description>调控图中 TF → Target 是有向边，扰动信号沿调控方向向下游传播，
    ''' 因此聚合的是<strong>入边邻居</strong>（即调控该基因的上游基因）；</description></item>
    ''' <item><description>抑制边（<see cref="EdgeRelationType.Repression"/>）的消息符号为 -1，
    ''' 使模型能够表达「上游下调导致下游上调」这样的反向效应；</description></item>
    ''' <item><description>默认使用稀疏入边聚合（复杂度 O(|E|·d)），因为先验调控网络非常稀疏
    ''' （350+ 基因仅约 350 条边），稠密邻接矩阵乘法会带来两个数量级的无用开销；</description></item>
    ''' <item><description>堆叠 L 层即可捕捉 L-hop 的间接调控效应；readme 建议 2~4 层以避免过平滑。</description></item>
    ''' </list>
    '''
    ''' 之所以不使用 GNN 模块自带的 <see cref="GNN.GATLayer"/>，是因为它的 <c>Backward</c>
    ''' 直接抛出 <see cref="InvalidOperationException"/>，无法参与训练。
    ''' </remarks>
    Public Class GEARSConvLayer
        Inherits GNN.Layer

        ''' <summary>自身信息变换权重 [inFeatures, outFeatures]</summary>
        ReadOnly wSelf As Tensor

        ''' <summary>偏置 [1, outFeatures]</summary>
        ReadOnly bias As Tensor

        ''' <summary>每种边关系类型专属的邻居变换权重</summary>
        ReadOnly relW As Tensor()

        ''' <summary>自身变换权重梯度</summary>
        ReadOnly wSelfGrad As Tensor

        ''' <summary>偏置梯度</summary>
        ReadOnly biasGrad As Tensor

        ''' <summary>每种边关系类型的邻居变换权重梯度</summary>
        ReadOnly relWGrad As Tensor()

        ''' <summary>上一次前向传播的输入特征</summary>
        Dim lastInput As Tensor

        ''' <summary>上一次前向传播的激活前输出（用于计算激活函数导数）</summary>
        Dim preActivation As Tensor

        ''' <summary>稠密模式下的归一化邻接矩阵缓存</summary>
        Dim denseAdj As Tensor

        ''' <summary>反向传播用的按边类型分组的梯度缓冲区</summary>
        Dim dTransBuffers As Tensor()

        ''' <summary>输入特征维度</summary>
        ''' <returns>输入维度大小</returns>
        Public ReadOnly Property InFeatures As Integer

        ''' <summary>输出特征维度</summary>
        ''' <returns>输出维度大小</returns>
        Public ReadOnly Property OutFeatures As Integer

        ''' <summary>激活函数类型</summary>
        ''' <returns><see cref="GNN.ActivationType"/> 枚举值</returns>
        Public ReadOnly Property Activation As GNN.ActivationType

        ''' <summary>是否为每种边关系类型分配独立的变换矩阵</summary>
        ''' <returns>独立变换则返回 True；所有类型共享同一个变换矩阵则返回 False</returns>
        Public ReadOnly Property UsePerRelationTransform As Boolean

        ''' <summary>是否使用稠密邻接矩阵聚合（仅用于小图对照，默认关闭）</summary>
        ''' <returns>稠密模式则返回 True</returns>
        Public ReadOnly Property UseDense As Boolean

        ''' <summary>
        ''' 创建 GEARS 图卷积层
        ''' </summary>
        ''' <param name="inFeatures">输入特征维度</param>
        ''' <param name="outFeatures">输出特征维度</param>
        ''' <param name="activation">激活函数类型，默认 <see cref="GNN.ActivationType.Tanh"/></param>
        ''' <param name="usePerRelationTransform">
        ''' 为 True 时为每种边关系类型分配独立的变换矩阵（严格对应 readme §5.4）；
        ''' 为 False 时所有类型共享同一个变换矩阵，仅用符号区分激活/抑制，计算量更小
        ''' </param>
        ''' <param name="useDense">为 True 时使用稠密归一化邻接矩阵聚合</param>
        ''' <param name="seed">权重初始化随机种子；给定后初始化结果可复现</param>
        ''' <param name="name">层名称</param>
        Public Sub New(inFeatures As Integer,
                       outFeatures As Integer,
                       Optional activation As GNN.ActivationType = GNN.ActivationType.Tanh,
                       Optional usePerRelationTransform As Boolean = False,
                       Optional useDense As Boolean = False,
                       Optional seed As Integer? = Nothing,
                       Optional name As String = Nothing)

            Me.InFeatures = inFeatures
            Me.OutFeatures = outFeatures
            Me.Activation = activation
            Me.UsePerRelationTransform = usePerRelationTransform
            Me.UseDense = useDense
            MyBase.Name = If(name, $"GEARSConv_{inFeatures}_{outFeatures}")

            wSelf = Tensor.XavierInit(inFeatures, outFeatures, seed)
            bias = New Tensor(1, outFeatures)
            wSelfGrad = New Tensor(inFeatures, outFeatures)
            biasGrad = New Tensor(1, outFeatures)

            ReDim relW(EdgeRelationTypes.NumRelationTypes - 1)
            ReDim relWGrad(EdgeRelationTypes.NumRelationTypes - 1)

            If usePerRelationTransform Then
                For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                    relW(r) = Tensor.XavierInit(inFeatures, outFeatures, seed)
                    relWGrad(r) = New Tensor(inFeatures, outFeatures)
                Next
            Else
                ' 所有关系类型共享同一个变换矩阵实例，梯度在反向传播时自动累加
                Dim sharedW As Tensor = Tensor.XavierInit(inFeatures, outFeatures, seed)
                Dim sharedGrad As Tensor = New Tensor(inFeatures, outFeatures)

                For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                    relW(r) = sharedW
                    relWGrad(r) = sharedGrad
                Next
            End If
        End Sub

        ''' <summary>
        ''' 前向传播：沿调控图的入边做边类型感知的稀疏（或稠密）聚合
        ''' </summary>
        ''' <param name="input">节点特征 [numGenes, inFeatures]</param>
        ''' <param name="graphData">基因调控图（提供稀疏入边缓存）</param>
        ''' <returns>更新后的节点特征 [numGenes, outFeatures]</returns>
        Public Overloads Function Forward(input As Tensor, graphData As GeneRegulatoryGraph) As Tensor
            lastInput = input

            If UseDense Then
                Return ForwardDense(input, graphData)
            End If

            Dim n As Integer = input.Shape(0)
            Dim dOut As Integer = OutFeatures
            Dim signs As Double() = EdgeRelationTypes.SignTable()
            Dim selfT As Tensor = MatOps.Mul(input, wSelf)
            Dim result As Tensor = New Tensor(n, dOut)
            Dim rd As Double() = result.Data
            Dim sd As Double() = selfT.Data
            Dim bd As Double() = bias.Data
            Dim transformed As Tensor() = BuildTransformed(input, graphData)

            For i As Integer = 0 To n - 1
                Dim iOff As Integer = i * dOut
                Dim sw As Double = graphData.SelfWeight(i)

                For k As Integer = 0 To dOut - 1
                    rd(iOff + k) = sw * sd(iOff + k) + bd(k)
                Next

                Dim sources As Integer() = graphData.InEdgeSources(i)
                Dim types As Integer() = graphData.InEdgeTypes(i)
                Dim weights As Double() = graphData.InEdgeWeights(i)

                For e As Integer = 0 To sources.Length - 1
                    Dim coeff As Double = weights(e) * signs(types(e))

                    If coeff = 0.0 Then
                        Continue For
                    End If

                    Dim td As Double() = transformed(types(e)).Data
                    Dim jOff As Integer = sources(e) * dOut

                    For k As Integer = 0 To dOut - 1
                        rd(iOff + k) += coeff * td(jOff + k)
                    Next
                Next
            Next

            preActivation = result

            Return GNN.ActivationFunctions.Apply(result, Activation)
        End Function

        ''' <summary>
        ''' 按边关系类型对输入做线性变换；共享模式下所有类型复用同一个结果
        ''' </summary>
        ''' <param name="input">节点特征 [numGenes, inFeatures]</param>
        ''' <param name="graphData">基因调控图，用于判断哪些关系类型实际存在</param>
        ''' <returns>长度为关系类型数量的变换结果数组</returns>
        Private Function BuildTransformed(input As Tensor, graphData As GeneRegulatoryGraph) As Tensor()
            Dim transformed As Tensor() = New Tensor(EdgeRelationTypes.NumRelationTypes - 1) {}
            Dim sharedT As Tensor = Nothing

            For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                If graphData.RelationTypeCounts(r) = 0 Then
                    Continue For
                End If

                If Not UsePerRelationTransform Then
                    If sharedT Is Nothing Then
                        sharedT = MatOps.Mul(input, relW(0))
                    End If

                    transformed(r) = sharedT
                Else
                    transformed(r) = MatOps.Mul(input, relW(r))
                End If
            Next

            Return transformed
        End Function

        ''' <summary>
        ''' 稠密模式下的前向传播（使用归一化邻接矩阵，含自环）
        ''' </summary>
        ''' <param name="input">节点特征 [numGenes, inFeatures]</param>
        ''' <param name="graphData">基因调控图</param>
        ''' <returns>更新后的节点特征 [numGenes, outFeatures]</returns>
        Private Function ForwardDense(input As Tensor, graphData As GeneRegulatoryGraph) As Tensor
            If denseAdj Is Nothing Then
                denseAdj = graphData.Graph.GetNormalizedAdjacencyMatrix()
            End If

            Dim aggregated As Tensor = MatOps.Mul(denseAdj, input)
            Dim result As Tensor = MatOps.Mul(aggregated, wSelf)
            Dim rd As Double() = result.Data
            Dim bd As Double() = bias.Data
            Dim n As Integer = result.Shape(0)
            Dim d As Integer = result.Shape(1)

            For i As Integer = 0 To n - 1
                Dim off As Integer = i * d

                For k As Integer = 0 To d - 1
                    rd(off + k) += bd(k)
                Next
            Next

            preActivation = result

            Return GNN.ActivationFunctions.Apply(result, Activation)
        End Function

        ''' <summary>
        ''' 反向传播：累积本层权重梯度并返回输入梯度
        ''' </summary>
        ''' <param name="gradient">上游梯度 [numGenes, outFeatures]</param>
        ''' <param name="graphData">基因调控图（提供稀疏入边缓存）</param>
        ''' <returns>输入梯度 [numGenes, inFeatures]</returns>
        Public Overloads Function Backward(gradient As Tensor, graphData As GeneRegulatoryGraph) As Tensor
            Dim gAct As Tensor = gradient.ElementwiseMultiply(
                GNN.ActivationFunctions.Derivative(preActivation, Activation))

            If UseDense Then
                Return BackwardDense(gAct, graphData)
            End If

            Dim n As Integer = gAct.Shape(0)
            Dim dOut As Integer = OutFeatures
            Dim dIn As Integer = InFeatures
            Dim gd As Double() = gAct.Data
            Dim signs As Double() = EdgeRelationTypes.SignTable()

            ' ---- 偏置梯度 ----
            Dim dBias As Tensor = New Tensor(1, dOut)

            Call MatOps.ColSumInto(gAct, dBias)
            Call MatOps.Accumulate(dBias, biasGrad)

            ' ---- 自身分支：dSelf[i,k] = selfWeight(i) * gAct[i,k] ----
            Dim dSelf As Tensor = New Tensor(n, dOut)
            Dim dsd As Double() = dSelf.Data

            For i As Integer = 0 To n - 1
                Dim sw As Double = graphData.SelfWeight(i)
                Dim off As Integer = i * dOut

                For k As Integer = 0 To dOut - 1
                    dsd(off + k) = sw * gd(off + k)
                Next
            Next

            Call MatOps.Accumulate(MatOps.MulAT(lastInput, dSelf), wSelfGrad)

            ' ---- 邻居分支：把梯度散射回每条入边的源节点，按边类型分组 ----
            dTransBuffers = EnsureBuffers(graphData, n, dOut)

            For i As Integer = 0 To n - 1
                Dim sources As Integer() = graphData.InEdgeSources(i)
                Dim types As Integer() = graphData.InEdgeTypes(i)
                Dim weights As Double() = graphData.InEdgeWeights(i)
                Dim iOff As Integer = i * dOut

                For e As Integer = 0 To sources.Length - 1
                    Dim coeff As Double = weights(e) * signs(types(e))

                    If coeff = 0.0 Then
                        Continue For
                    End If

                    Dim td As Double() = dTransBuffers(types(e)).Data
                    Dim jOff As Integer = sources(e) * dOut

                    For k As Integer = 0 To dOut - 1
                        td(jOff + k) += coeff * gd(iOff + k)
                    Next
                Next
            Next

            Dim dX As Tensor = MatOps.MulBT(dSelf, wSelf)

            For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                If dTransBuffers(r) Is Nothing Then
                    Continue For
                End If

                Call MatOps.Accumulate(MatOps.MulAT(lastInput, dTransBuffers(r)), relWGrad(r))
                Call MatOps.Accumulate(MatOps.MulBT(dTransBuffers(r), relW(r)), dX)
            Next

            Return dX
        End Function

        ''' <summary>
        ''' 稠密模式下的反向传播
        ''' </summary>
        ''' <param name="gAct">经过激活函数导数修正的梯度 [numGenes, outFeatures]</param>
        ''' <param name="graphData">基因调控图</param>
        ''' <returns>输入梯度 [numGenes, inFeatures]</returns>
        Private Function BackwardDense(gAct As Tensor, graphData As GeneRegulatoryGraph) As Tensor
            Dim aggregated As Tensor = MatOps.Mul(denseAdj, lastInput)
            Dim dBias As Tensor = New Tensor(1, OutFeatures)

            Call MatOps.ColSumInto(gAct, dBias)
            Call MatOps.Accumulate(dBias, biasGrad)
            Call MatOps.Accumulate(MatOps.MulAT(aggregated, gAct), wSelfGrad)

            Dim dAgg As Tensor = MatOps.MulBT(gAct, wSelf)

            ' 归一化邻接矩阵是对称矩阵，梯度回传同样使用 A_norm
            Return MatOps.Mul(denseAdj, dAgg)
        End Function

        ''' <summary>
        ''' 获取（并在必要时重新分配与清零）按边类型分组的反向传播缓冲区
        ''' </summary>
        ''' <param name="graphData">基因调控图，用于判断哪些关系类型实际存在</param>
        ''' <param name="rows">行数（基因数量）</param>
        ''' <param name="cols">列数（输出维度）</param>
        ''' <returns>缓冲区数组；图上不存在的关系类型对应位置为 Nothing</returns>
        Private Function EnsureBuffers(graphData As GeneRegulatoryGraph, rows As Integer, cols As Integer) As Tensor()
            Dim needRealloc As Boolean = dTransBuffers Is Nothing

            If Not needRealloc Then
                For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                    Dim buf As Tensor = dTransBuffers(r)

                    If buf Is Nothing Then
                        Continue For
                    End If

                    If buf.Shape(0) <> rows OrElse buf.Shape(1) <> cols Then
                        needRealloc = True
                        Exit For
                    End If
                Next
            End If

            If needRealloc Then
                dTransBuffers = New Tensor(EdgeRelationTypes.NumRelationTypes - 1) {}

                For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                    If graphData.RelationTypeCounts(r) > 0 Then
                        dTransBuffers(r) = New Tensor(rows, cols)
                    End If
                Next
            Else
                For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                    If dTransBuffers(r) IsNot Nothing Then
                        Call MatOps.Zero(dTransBuffers(r))
                    End If
                Next
            End If

            Return dTransBuffers
        End Function

        ''' <summary>
        ''' 不支持的调用方式：本层需要图结构参数
        ''' </summary>
        ''' <param name="input">节点特征</param>
        ''' <returns>永不返回</returns>
        Public Overrides Function Forward(input As Tensor) As Tensor
            Throw New InvalidOperationException("GEARSConvLayer 需要图结构，请使用 Forward(input, graphData) 重载")
        End Function

        ''' <summary>
        ''' 不支持的调用方式：本层需要图结构参数
        ''' </summary>
        ''' <param name="gradient">上游梯度</param>
        ''' <returns>永不返回</returns>
        Public Overrides Function Backward(gradient As Tensor) As Tensor
            Throw New InvalidOperationException("GEARSConvLayer 需要图结构，请使用 Backward(gradient, graphData) 重载")
        End Function

        ''' <summary>
        ''' 获取本层可训练参数
        ''' </summary>
        ''' <returns>参数张量列表</returns>
        Public Overrides Function GetParameters() As List(Of Tensor)
            Dim params As New List(Of Tensor) From {wSelf, bias}

            If UsePerRelationTransform Then
                For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                    params.Add(relW(r))
                Next
            Else
                params.Add(relW(0))
            End If

            Return params
        End Function

        ''' <summary>
        ''' 获取本层参数梯度（顺序与 <see cref="GetParameters"/> 严格一致）
        ''' </summary>
        ''' <returns>梯度张量列表</returns>
        Public Overrides Function GetGradients() As List(Of Tensor)
            Dim grads As New List(Of Tensor) From {wSelfGrad, biasGrad}

            If UsePerRelationTransform Then
                For r As Integer = 0 To EdgeRelationTypes.NumRelationTypes - 1
                    grads.Add(relWGrad(r))
                Next
            Else
                grads.Add(relWGrad(0))
            End If

            Return grads
        End Function
    End Class
End Namespace
