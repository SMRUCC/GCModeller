' ============================================================================
' PriorGraphBuilder.vb — readme 一.1 build_prior_graph 的实现
'
' 三步构造先验调控图：
'
'   第一步 · TF-Target 有向边（基础骨架）
'       A[i][j] = 1
'       W_init[i][j] = sign(effector) · MAX(edge.Confidence, wgcna_adj[i][j])
'       mask[i][j] = 1
'       confidence[i][j] = edge.Confidence
'       （sign：抑制型 TF 取负权重，见 SpikingLoopConfig.UseRegulationSign）
'
'   第二步 · WGCNA 补充边（弱连接 / 双向）
'       仅当 A[i][j] = 0 且 A[j][i] = 0（两个方向都还没有边）、
'       wgcna_adj[i][j] > 阈值、且该关联位于 i 或 j 的 Top-K 之内时：
'         A[i][j] = A[j][i] = 1
'         W_init[i][j] = W_init[j][i] = 0.5 · wgcna_adj[i][j]
'         mask / confidence 同步开启
'
'   第三步 · 稀疏化与归一化
'       W_init = W_init ⊙ mask
'       按配置归一化（默认按突触前/行归一化，稳定下游发放率）
'
' WGCNA 的职责边界（与上游数据提供的分工）：
'   本类<b>只接收</b>已经算好的对称共表达关联矩阵 <c>wgcnaAdj</c>（取值 [0,1]，N×N），
'   不负责它的文件读取与计算——那属于上游 WGCNA 分析的职责。
'   传入 Nothing 时退化为"仅 TF-Target"的纯有向先验图。
' ============================================================================

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports std = System.Math

Namespace Graph

    ''' <summary>
    ''' 由先验调控网络（TF-Target）与 WGCNA 共表达关联矩阵构造
    ''' 脉冲神经网络所需的邻接矩阵、初始权重与结构掩码。
    ''' </summary>
    Public Class PriorGraphBuilder

        Private ReadOnly _config As SpikingLoopConfig

        Public Sub New(config As SpikingLoopConfig)
            If config Is Nothing Then
                Throw New ArgumentNullException(NameOf(config))
            End If
            _config = config
        End Sub

        ''' <summary>
        ''' 构建先验调控图。
        ''' </summary>
        ''' <param name="geneNames">基因列表（顺序决定神经元索引，必须与表达矩阵行顺序一致）</param>
        ''' <param name="prior">先验调控网络（BNLearn 的 PriorNetwork）</param>
        ''' <param name="wgcnaAdj">
        ''' WGCNA 共表达关联矩阵 [N, N]，对称、取值 [0,1]。
        ''' 为 Nothing 或配置中 <c>TopKWgcna = 0</c> 时不补充共表达边。
        ''' </param>
        Public Function Build(geneNames As String(), prior As PriorNetwork,
                              Optional wgcnaAdj As Double(,) = Nothing) As PriorGraph

            If geneNames Is Nothing OrElse geneNames.Length = 0 Then
                Throw New ArgumentException("基因列表不能为空", NameOf(geneNames))
            End If

            Dim n = geneNames.Length
            Dim index = BuildGeneIndex(geneNames)

            Dim A(n - 1, n - 1) As Double
            Dim W(n - 1, n - 1) As Double
            Dim M(n - 1, n - 1) As Double
            Dim C(n - 1, n - 1) As Double

            ' ---------- 第一步：TF-Target 有向边（基础骨架） ----------
            Dim numPriorEdges = 0
            Dim skipped = 0

            For Each edge As RegulatoryEdge In prior.Edges.SafeQuery()
                Dim fromIdx = -1, toIdx = -1
                index.TryGetValue(edge.TF, fromIdx)
                index.TryGetValue(edge.TargetGene, toIdx)

                If fromIdx < 0 OrElse toIdx < 0 Then
                    skipped += 1
                    Continue For
                End If
                If fromIdx = toIdx AndAlso Not _config.AllowSelfLoop Then
                    skipped += 1
                    Continue For
                End If

                Dim conf = If(Double.IsNaN(edge.Confidence) OrElse edge.Confidence <= 0.0,
                              1.0, edge.Confidence)
                Dim sign = RegulationSign(edge.RegulationType)
                Dim wgcnaWeight = If(wgcnaAdj Is Nothing, 0.0, std.Abs(wgcnaAdj(fromIdx, toIdx)))

                If A(fromIdx, toIdx) <> 0.0 Then
                    ' 同一对 (TF, Target) 出现多条证据：取更强的置信度，不重复计数为多条突触
                    If conf > C(fromIdx, toIdx) Then
                        C(fromIdx, toIdx) = conf
                        W(fromIdx, toIdx) = sign * std.Max(conf, wgcnaWeight)
                    End If
                    skipped += 1
                    Continue For
                End If

                A(fromIdx, toIdx) = 1.0
                M(fromIdx, toIdx) = 1.0
                C(fromIdx, toIdx) = conf
                W(fromIdx, toIdx) = sign * std.Max(conf, wgcnaWeight)
                numPriorEdges += 1
            Next

            ' ---------- 第二步：WGCNA 补充边（双向弱连接） ----------
            Dim numWgcnaEdges = 0

            If wgcnaAdj IsNot Nothing AndAlso _config.TopKWgcna > 0 Then
                AssertWgcnaShape(wgcnaAdj, n)

                Dim keep = TopWgcnaPairs(wgcnaAdj, _config.TopKWgcna, _config.WgcnaThreshold)

                For Each pair In keep
                    Dim i = pair.Item1
                    Dim j = pair.Item2
                    Dim strength = wgcnaAdj(i, j)

                    ' 两个方向都还没有边时才补充（TF-Target 的方向语义优先）
                    If A(i, j) <> 0.0 OrElse A(j, i) <> 0.0 Then
                        Continue For
                    End If

                    Dim half = 0.5 * strength
                    A(i, j) = 1.0 : A(j, i) = 1.0
                    W(i, j) = half : W(j, i) = half
                    M(i, j) = 1.0 : M(j, i) = 1.0
                    C(i, j) = strength : C(j, i) = strength
                    numWgcnaEdges += 2
                Next
            End If

            ' ---------- 第三步：稀疏化与归一化 ----------
            Dim maskTensor = Tensor.Wrap(OneDim(M), n, n)
            For i = 0 To n - 1
                For j = 0 To n - 1
                    If M(i, j) = 0.0 Then W(i, j) = 0.0
                Next
            Next

            NormalizeByRow(W, n, _config.PriorNormalization)

            Return New PriorGraph(
                geneNames,
                Tensor.Wrap(OneDim(A), n, n),
                Tensor.Wrap(OneDim(W), n, n),
                maskTensor,
                Tensor.Wrap(OneDim(C), n, n),
                numPriorEdges,
                numWgcnaEdges,
                skipped)
        End Function

#Region "辅助"

        ''' <summary>抑制型 TF 取负权重，激活/未知型取正权重</summary>
        Private Function RegulationSign(effector As Effector) As Double
            If Not _config.UseRegulationSign Then Return 1.0
            Return If(effector = Effector.Inhibitor, -1.0, 1.0)
        End Function

        Private Shared Function BuildGeneIndex(geneNames As String()) As Dictionary(Of String, Integer)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To geneNames.Length - 1
                ' 重名基因保留第一个索引（表达矩阵中重名本身即为异常数据）
                If Not map.ContainsKey(geneNames(i)) Then
                    map(geneNames(i)) = i
                End If
            Next
            Return map
        End Function

        Private Shared Sub AssertWgcnaShape(wgcnaAdj As Double(,), n As Integer)
            If wgcnaAdj.GetLength(0) <> n OrElse wgcnaAdj.GetLength(1) <> n Then
                Throw New ArgumentException(
                    $"WGCNA 关联矩阵形状应为 [{n}, {n}]，实际 [{wgcnaAdj.GetLength(0)}, {wgcnaAdj.GetLength(1)}]")
            End If
        End Sub

        ''' <summary>
        ''' 选取 WGCNA 补充边候选：对每个基因取其关联强度最高的 Top-K 个伙伴，
        ''' 只要该关联位于<b>任一端</b>的 Top-K 之内即入选（等价于"Top-K 并集"）。
        ''' 返回按 i &lt; j 去重后的无向对。
        ''' </summary>
        Private Shared Function TopWgcnaPairs(wgcnaAdj As Double(,), topK As Integer,
                                              threshold As Double) As List(Of (Integer, Integer))
            Dim n = wgcnaAdj.GetLength(0)
            Dim seen As New HashSet(Of Long)()
            Dim pairs As New List(Of (Integer, Integer))()

            For i = 0 To n - 1
                Dim candidates As New List(Of (Integer, Double))

                For j = 0 To n - 1
                    If i = j Then Continue For
                    Dim v = wgcnaAdj(i, j)
                    If Double.IsNaN(v) OrElse v <= threshold Then Continue For
                    candidates.Add((j, std.Abs(v)))
                Next

                candidates.Sort(Function(a, b) b.Item2.CompareTo(a.Item2))

                Dim take = std.Min(topK, candidates.Count)
                For k = 0 To take - 1
                    Dim j = candidates(k).Item1
                    Dim lo = std.Min(i, j)
                    Dim hi = std.Max(i, j)
                    Dim key = CLng(lo) * 1000000L + hi

                    If seen.Add(key) Then
                        pairs.Add((lo, hi))
                    End If
                Next
            Next

            Return pairs
        End Function

        ''' <summary>按行（突触前）归一化：每行权重和缩放为 1（按绝对值求和，避免正负相消）</summary>
        Private Shared Sub NormalizeByRow(w As Double(,), n As Integer, mode As PriorNormalization)
            Select Case mode
                Case PriorNormalization.None
                    Return

                Case PriorNormalization.GlobalMax
                    Dim mx = 0.0
                    For i = 0 To n - 1
                        For j = 0 To n - 1
                            mx = std.Max(mx, std.Abs(w(i, j)))
                        Next
                    Next
                    If mx > 0.0 Then
                        For i = 0 To n - 1
                            For j = 0 To n - 1
                                w(i, j) /= mx
                            Next
                        Next
                    End If

                Case PriorNormalization.FanIn
                    Dim colSum(n - 1) As Double
                    For i = 0 To n - 1
                        For j = 0 To n - 1
                            colSum(j) += std.Abs(w(i, j))
                        Next
                    Next
                    For i = 0 To n - 1
                        For j = 0 To n - 1
                            If colSum(j) > 0.0 Then w(i, j) /= colSum(j)
                        Next
                    Next

                Case Else ' FanOut
                    For i = 0 To n - 1
                        Dim s = 0.0
                        For j = 0 To n - 1
                            s += std.Abs(w(i, j))
                        Next
                        If s > 0.0 Then
                            For j = 0 To n - 1
                                w(i, j) /= s
                            Next
                        End If
                    Next
            End Select
        End Sub

        ''' <summary>把二维数组按行优先展平（Tensor 底层存储约定）</summary>
        Private Shared Function OneDim(a As Double(,)) As Double()
            Dim rows = a.GetLength(0)
            Dim cols = a.GetLength(1)
            Dim d(rows * cols - 1) As Double
            For i = 0 To rows - 1
                Dim off = i * cols
                For j = 0 To cols - 1
                    d(off + j) = a(i, j)
                Next
            Next
            Return d
        End Function

#End Region

    End Class

End Namespace
