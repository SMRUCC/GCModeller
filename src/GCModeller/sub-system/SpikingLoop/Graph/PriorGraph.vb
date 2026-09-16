' ============================================================================
' PriorGraph.vb — 先验调控图（TF-Target 骨架 + WGCNA 补充边）
'
' 这是 readme 一.1 <c>build_prior_graph</c> 的输出契约，是数据准备阶段与建模阶段
' 之间的"接口对象"：
'
'   Adjacency   A[i, j] ∈ {0,1}   突触存在性（i → j 表示基因 i 的脉冲传向基因 j）
'   WInit       W₀[i, j]          先验初始权重（已按配置归一化，符号体现激活/抑制）
'   Mask        M[i, j] ∈ {0,1}   允许梯度更新的突触位置（0 = 冻结）
'   Confidence  C[i, j]           逐边置信度，供结构正则加权
'
' 行列约定与 SNN 基础库的 SparseMatrix / SparseLIFLayer / RecurrentLIFLayer 完全一致：
'   W[pre, post]，行 = 突触前（调控者），列 = 突触后（靶基因）。
'
' 方向性说明（readme 一.1 关键设计要点）：
'   TF→Target 是唯一的"有向"依据；WGCNA 只提供"潜在连接存在性"，
'   因此它的补充边被写成双向弱连接，方向不参与语义解释。
' ============================================================================

Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Graph

    ''' <summary>
    ''' 先验调控图：邻接矩阵 + 初始权重 + 可训练掩码 + 逐边置信度。
    ''' </summary>
    Public Class PriorGraph

#Region "构建结果"

        ''' <summary>基因名称列表，顺序与表达矩阵的行顺序严格一致（决定神经元索引）</summary>
        Public ReadOnly Property GeneNames As String()

        ''' <summary>邻接矩阵 [N, N]：1 = 突触存在（含 WGCNA 补充的弱连接）</summary>
        Public ReadOnly Property Adjacency As Tensor

        ''' <summary>先验初始权重矩阵 [N, N]（已按配置归一化）</summary>
        Public ReadOnly Property WInit As Tensor

        ''' <summary>结构掩码 [N, N]：1 = 允许梯度更新，0 = 冻结</summary>
        Public ReadOnly Property Mask As Tensor

        ''' <summary>逐边置信度 [N, N]：TF-Target 用调控证据置信度，WGCNA 边用共表达关联强度</summary>
        Public ReadOnly Property Confidence As Tensor

        ''' <summary>
        ''' <b>仅</b> TF-Target 骨架边的掩码 [N, N]（1 = 已知调控关系，不含 WGCNA 补充的弱连接）。
        ''' 供 readme 五.2 的调控关系合理性评估使用："学习到的权重与已知 TF-Target 集合对比"，
        ''' 正例必须限定为"已知调控关系"，否则把共表达边也算成正例会让 AUROC 失去意义。
        ''' </summary>
        Public ReadOnly Property Skeleton As Tensor

#End Region

#Region "统计"

        ''' <summary>TF→Target 骨架边数量（有向）</summary>
        Public ReadOnly Property NumPriorEdges As Integer

        ''' <summary>WGCNA 补充的弱连接数量（按有向边计，即一对双向边算 2）</summary>
        Public ReadOnly Property NumWgcnaEdges As Integer

        ''' <summary>被跳过的调控边数量（基因名未命中表达矩阵 / 自环 / 相同边重复）</summary>
        Public ReadOnly Property NumSkippedEdges As Integer

        ''' <summary>基因数量 N</summary>
        Public ReadOnly Property Units As Integer

        Public Sub New(geneNames As String(), adjacency As Tensor, wInit As Tensor, mask As Tensor,
                       confidence As Tensor, numPriorEdges As Integer, numWgcnaEdges As Integer,
                       Optional numSkippedEdges As Integer = 0,
                       Optional skeleton As Tensor = Nothing)

            Me.GeneNames = geneNames
            Me.Adjacency = adjacency
            Me.WInit = wInit
            Me.Mask = mask
            Me.Confidence = confidence
            Me.NumPriorEdges = numPriorEdges
            Me.NumWgcnaEdges = numWgcnaEdges
            Me.NumSkippedEdges = numSkippedEdges
            Me.Units = If(geneNames Is Nothing, 0, geneNames.Length)
            ' 未显式提供骨架掩码时退化为"全部已知边"，保证评估仍可运行
            Me.Skeleton = If(skeleton, mask)
        End Sub

        ''' <summary>邻接矩阵中的突触总数（有向边数）</summary>
        Public ReadOnly Property NumSynapses As Integer
            Get
                If Adjacency Is Nothing Then Return 0
                Dim d = Adjacency.Data
                Dim n = 0
                For i = 0 To d.Length - 1
                    If d(i) <> 0.0 Then n += 1
                Next
                Return n
            End Get
        End Property

        ''' <summary>连接密度 = 有向边数 / N²</summary>
        Public ReadOnly Property Density As Double
            Get
                If Units = 0 Then Return 0.0
                Return NumSynapses / CDbl(Units * Units)
            End Get
        End Property

        ''' <summary>被冻结的突触位置数量（掩码为 0 的位置，含全部非零元素）</summary>
        Public ReadOnly Property NumFrozenSynapses As Integer
            Get
                If Mask Is Nothing Then Return 0
                Dim d = Mask.Data
                Dim n = 0
                For i = 0 To d.Length - 1
                    If d(i) = 0.0 Then n += 1
                Next
                Return n
            End Get
        End Property

        ''' <summary>先验权重的 L1 和（用于训练前后的漂移对比）</summary>
        Public ReadOnly Property WeightL1 As Double
            Get
                If WInit Is Nothing Then Return 0.0
                Dim d = WInit.Data
                Dim s = 0.0
                For i = 0 To d.Length - 1
                    s += std.Abs(d(i))
                Next
                Return s
            End Get
        End Property

#End Region

#Region "导出"

        ''' <summary>
        ''' 把先验权重矩阵导出为稀疏 CSR（行 = 突触前，列 = 突触后），
        ''' 可直接交给 <see cref="SparseLIFLayer"/> 做大图前向仿真。
        ''' </summary>
        ''' <param name="minAbsWeight">绝对值低于该值的突触被丢弃（0 = 保留全部非零突触）</param>
        Public Function ToSparseMatrix(Optional minAbsWeight As Double = 0.0) As SparseMatrix
            Dim pre As New List(Of Integer)()
            Dim post As New List(Of Integer)()
            Dim weight As New List(Of Double)()

            Dim w = WInit.Data
            For i = 0 To Units - 1
                Dim off = i * Units
                For j = 0 To Units - 1
                    Dim v = w(off + j)
                    If std.Abs(v) > minAbsWeight Then
                        pre.Add(i)
                        post.Add(j)
                        weight.Add(v)
                    End If
                Next
            Next

            Return SparseMatrix.FromTriplets(pre.ToArray(), post.ToArray(), weight.ToArray(), Units, Units)
        End Function

        ''' <summary>导出所有非零突触（pre, post, weight, confidence）用于结果表输出</summary>
        Public Function EdgeList() As List(Of (pre As Integer, post As Integer, weight As Double, confidence As Double))
            Dim list As New List(Of (pre As Integer, post As Integer, weight As Double, confidence As Double))()
            Dim w = WInit.Data
            Dim c = Confidence.Data

            For i = 0 To Units - 1
                Dim off = i * Units
                For j = 0 To Units - 1
                    If w(off + j) <> 0.0 Then
                        list.Add((i, j, w(off + j), c(off + j)))
                    End If
                Next
            Next

            Return list
        End Function

#End Region

        Public Overrides Function ToString() As String
            Return $"PriorGraph(N={Units}, TF→Target={NumPriorEdges}, WGCNA={NumWgcnaEdges}, " &
                   $"synapses={NumSynapses}, density={Density:P3}, skipped={NumSkippedEdges})"
        End Function

    End Class

End Namespace
