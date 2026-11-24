Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace Bayesian.Tree

    ''' <summary>
    ''' 依赖树(dependency tree)：以联合比对的全部位置为节点的一棵生成树。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 论文把依赖树 <c>T</c> 当作 nuisance parameter 处理：给定树之后联合似然分解为
    ''' </para>
    ''' <code>
    ''' P(D | a, T) = [ Π_i P(D_i) ] · Π_(i,j)∈T R_ij
    ''' </code>
    ''' <para>
    ''' 因此「最优树」<c>T*</c> 就是在完全图之上、以 <c>ln R_ij</c> 为边权的<b>最大生成树</b>。
    ''' 这也是论文在行列式不可行时采用的近似 <c>P(D|a) ≈ P(D|a, T*)</c>。
    ''' </para>
    ''' <para>
    ''' 本项目需要三棵树：
    ''' <c>T*</c>(全部位置)、<c>T_k</c>(只含家族 A 的位置)、<c>T_r</c>(只含家族 B 的位置)，
    ''' 分别用于打分公式的分子与两个分母。
    ''' </para>
    ''' </remarks>
    Public Class DependencyTree

        ''' <summary>
        ''' 依赖树上的一条边
        ''' </summary>
        Public Class Edge

            ''' <summary>
            ''' 边的第一个端点(列下标)
            ''' </summary>
            Public Property I As Integer
            ''' <summary>
            ''' 边的第二个端点(列下标)
            ''' </summary>
            Public Property J As Integer
            ''' <summary>
            ''' 边权 <c>ln R_ij</c>
            ''' </summary>
            Public Property LogWeight As Double

            Public Overrides Function ToString() As String
                Return $"{I} -- {J} (logR={LogWeight:0.###})"
            End Function
        End Class

        Private ReadOnly _edges As Edge()
        Private ReadOnly _nodes As Integer()

        Private Sub New(nodes As Integer(), edges As Edge())
            _nodes = nodes
            _edges = edges
        End Sub

        ''' <summary>
        ''' 参与建树的节点(列下标，升序)
        ''' </summary>
        Public ReadOnly Property Nodes As Integer()
            Get
                Return _nodes
            End Get
        End Property

        ''' <summary>
        ''' 树上的边，共有 <c>节点数 - 1</c> 条
        ''' </summary>
        Public ReadOnly Property Edges As Edge()
            Get
                Return _edges
            End Get
        End Property

        ''' <summary>
        ''' 树的总权重 <c>Σ_(i,j)∈T ln R_ij</c>
        ''' </summary>
        Public ReadOnly Property TotalLogWeight As Double
            Get
                Dim total As Double = 0.0

                For Each e As Edge In _edges
                    total += e.LogWeight
                Next

                Return total
            End Get
        End Property

        ''' <summary>
        ''' 节点个数
        ''' </summary>
        Public ReadOnly Property NodeCount As Integer
            Get
                Return _nodes.Length
            End Get
        End Property

        ''' <summary>
        ''' 在给定的列集合之上求最大生成树(Prim 算法，稠密实现)。
        ''' </summary>
        ''' <param name="logR">似然比矩阵</param>
        ''' <param name="variables">参与建树的列下标，为 <c>Nothing</c> 的时候使用全部列</param>
        ''' <remarks>
        ''' 使用稠密的 <c>O(L²)</c> Prim 而不是基于堆的实现，是因为这里图的规模通常在数百个节点，
        ''' 稠密数组的常数因子远小于堆操作；同时避免了为 10⁵ 条边分配对象。
        ''' </remarks>
        Public Shared Function Prim(logR As LikelihoodRatioMatrix, Optional variables As Integer() = Nothing) As DependencyTree
            If logR Is Nothing Then
                Throw New ArgumentNullException(NameOf(logR))
            End If

            Dim nodes As Integer() = If(variables, Enumerable.Range(0, logR.Size).ToArray())

            If nodes.Length <= 1 Then
                Return New DependencyTree(nodes, New Edge() {})
            End If

            Dim count As Integer = nodes.Length
            Dim inTree As Boolean() = New Boolean(count - 1) {}
            Dim bestWeight As Double() = New Double(count - 1) {}
            Dim bestParent As Integer() = New Integer(count - 1) {}
            Dim edges As New List(Of Edge)

            For i As Integer = 0 To count - 1
                bestWeight(i) = Double.NegativeInfinity
                bestParent(i) = -1
            Next

            inTree(0) = True

            For i As Integer = 1 To count - 1
                bestWeight(i) = logR.LogR(nodes(0), nodes(i))
                bestParent(i) = 0
            Next

            For iteration As Integer = 1 To count - 1
                Dim pick As Integer = -1
                Dim weight As Double = Double.NegativeInfinity

                For i As Integer = 0 To count - 1
                    If Not inTree(i) AndAlso bestWeight(i) > weight Then
                        weight = bestWeight(i)
                        pick = i
                    End If
                Next

                If pick < 0 Then
                    Exit For
                End If

                inTree(pick) = True
                Call edges.Add(New Edge With {
                    .I = nodes(bestParent(pick)),
                    .J = nodes(pick),
                    .LogWeight = weight
                })

                For i As Integer = 0 To count - 1
                    If inTree(i) Then
                        Continue For
                    End If

                    Dim w As Double = logR.LogR(nodes(pick), nodes(i))

                    If w > bestWeight(i) Then
                        bestWeight(i) = w
                        bestParent(i) = pick
                    End If
                Next
            Next

            Return New DependencyTree(nodes, edges.ToArray())
        End Function

        ''' <summary>
        ''' 在预先计算好的 <c>ln R</c> 矩阵之上求最大生成树
        ''' </summary>
        Public Shared Function Prim(logWeights As Double(,), Optional variables As Integer() = Nothing) As DependencyTree
            If logWeights Is Nothing Then
                Throw New ArgumentNullException(NameOf(logWeights))
            End If

            Dim nodes As Integer() = If(variables, Enumerable.Range(0, logWeights.GetLength(0)).ToArray())

            If nodes.Length <= 1 Then
                Return New DependencyTree(nodes, New Edge() {})
            End If

            Dim count As Integer = nodes.Length
            Dim inTree As Boolean() = New Boolean(count - 1) {}
            Dim bestWeight As Double() = New Double(count - 1) {}
            Dim bestParent As Integer() = New Integer(count - 1) {}
            Dim edges As New List(Of Edge)

            For i As Integer = 0 To count - 1
                bestWeight(i) = Double.NegativeInfinity
                bestParent(i) = -1
            Next

            inTree(0) = True

            For i As Integer = 1 To count - 1
                bestWeight(i) = logWeights(nodes(0), nodes(i))
                bestParent(i) = 0
            Next

            For iteration As Integer = 1 To count - 1
                Dim pick As Integer = -1
                Dim weight As Double = Double.NegativeInfinity

                For i As Integer = 0 To count - 1
                    If Not inTree(i) AndAlso bestWeight(i) > weight Then
                        weight = bestWeight(i)
                        pick = i
                    End If
                Next

                If pick < 0 Then
                    Exit For
                End If

                inTree(pick) = True
                Call edges.Add(New Edge With {
                    .I = nodes(bestParent(pick)),
                    .J = nodes(pick),
                    .LogWeight = weight
                })

                For i As Integer = 0 To count - 1
                    If inTree(i) Then
                        Continue For
                    End If

                    Dim w As Double = logWeights(nodes(pick), nodes(i))

                    If w > bestWeight(i) Then
                        bestWeight(i) = w
                        bestParent(i) = pick
                    End If
                Next
            Next

            Return New DependencyTree(nodes, edges.ToArray())
        End Function

        ''' <summary>
        ''' 暴力枚举全部生成树并取最大权重者，仅用于测试工程之中与 <see cref="Prim"/> 对照。
        ''' </summary>
        ''' <remarks>
        ''' 复杂度为 <c>O(n^(n-2))</c>，只能用于 <c>n ≤ 6</c> 的小规模校验。
        ''' </remarks>
        Public Shared Function BruteForce(logWeights As Double(,), Optional variables As Integer() = Nothing) As DependencyTree
            Dim nodes As Integer() = If(variables, Enumerable.Range(0, logWeights.GetLength(0)).ToArray())
            Dim count As Integer = nodes.Length

            If count <= 1 Then
                Return New DependencyTree(nodes, New Edge() {})
            End If
            If count > 7 Then
                Throw New ArgumentException("brute force enumeration is limited to at most 7 nodes", NameOf(variables))
            End If

            Dim allPairs As New List(Of (i As Integer, j As Integer))

            For i As Integer = 0 To count - 2
                For j As Integer = i + 1 To count - 1
                    Call allPairs.Add((i, j))
                Next
            Next

            Dim bestWeight As Double = Double.NegativeInfinity
            Dim bestEdges As New List(Of Edge)

            ' 枚举边集的全部组合，保留其中「恰好是树」的那些
            Dim combinations As Long = 1L << allPairs.Count

            For mask As Long = 0 To combinations - 1
                Dim selected As New List(Of Integer)

                For k As Integer = 0 To allPairs.Count - 1
                    If (mask >> k And 1L) = 1L Then
                        Call selected.Add(k)
                    End If
                Next

                If selected.Count <> count - 1 Then
                    Continue For
                End If

                Dim parent As Integer() = New Integer(count - 1) {}

                For i As Integer = 0 To count - 1
                    parent(i) = i
                Next

                Dim weight As Double = 0.0
                Dim valid As Boolean = True

                For Each k As Integer In selected
                    Dim a As Integer = Find(parent, allPairs(k).i)
                    Dim b As Integer = Find(parent, allPairs(k).j)

                    If a = b Then
                        valid = False
                        Exit For
                    End If

                    parent(a) = b
                    weight += logWeights(nodes(allPairs(k).i), nodes(allPairs(k).j))
                Next

                If valid AndAlso weight > bestWeight Then
                    bestWeight = weight
                    Call bestEdges.Clear()

                    For Each k As Integer In selected
                        Call bestEdges.Add(New Edge With {
                            .I = nodes(allPairs(k).i),
                            .J = nodes(allPairs(k).j),
                            .LogWeight = logWeights(nodes(allPairs(k).i), nodes(allPairs(k).j))
                        })
                    Next
                End If
            Next

            Return New DependencyTree(nodes, bestEdges.ToArray())
        End Function

        Private Shared Function Find(parent As Integer(), x As Integer) As Integer
            While parent(x) <> x
                x = parent(x)
            End While

            Return x
        End Function

        Public Overrides Function ToString() As String
            Return $"DependencyTree[{NodeCount} nodes, {_edges.Length} edges, total logR={TotalLogWeight:0.###}]"
        End Function
    End Class
End Namespace
