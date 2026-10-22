Imports System.Collections.Generic
Imports System.Linq

Namespace Analysis

    ''' <summary>
    ''' The motif analyzer of the topology search: it decides which of the two
    ''' adaptation motifs of the paper a topology contains and computes the
    ''' Venn statistics of the functional topology set.
    ''' (拓扑搜索的基序分析器：判定拓扑含有论文两大适应基序中的哪一种，并计算 Venn 交集统计)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The two motifs are operationalized on the sign vector of a topology:
    ''' </para>
    ''' <list type="bullet">
    ''' <item>
    ''' <b>NFBLB</b> (negative feedback loop with a buffering node): there is a
    ''' directed cycle with a negative sign product that passes through the
    ''' buffering node B, i.e. the output C feeds back to itself through B, and
    ''' the output does not regulate the input node A directly (no C→A link),
    ''' because the feedback has to pass through the buffer. This is the
    ''' integral control motif of the paper.
    ''' </item>
    ''' <item>
    ''' <b>IFFLP</b> (incoherent feedforward loop with a proportioner node): the
    ''' input A reaches the output C along the direct link and along the path
    ''' through B, and the two paths own opposite signs, with B directly
    ''' regulating C. This is the proportioner motif of the paper.
    ''' </item>
    ''' </list>
    ''' <para>
    ''' The necessity claim of the paper is that every over represented
    ''' functional topology owns at least one of the two motifs, which is what
    ''' <see cref="VennCounts"/> measures.
    ''' </para>
    ''' </remarks>
    Public Module MotifAnalyzer

        ''' <summary>
        ''' Enumerate every simple directed cycle of a topology. Each cycle is
        ''' reported exactly once as its node sequence, where the smallest node
        ''' index of the cycle comes first (the canonical rotation).
        ''' (枚举拓扑的全部简单有向环，每个环以最小节点在前的规范旋转报告一次)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Function EnumerateCycles(topology As Model.NetworkTopology) As List(Of Integer())
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            Dim cycles As New List(Of Integer())()

            For start As Integer = 0 To Model.NetworkTopology.NodeCount - 1
                Dim path As New List(Of Integer)()
                Dim visited As Boolean() = New Boolean(Model.NetworkTopology.NodeCount - 1) {}

                path.Add(start)
                visited(start) = True

                Call SearchCycles(topology, start, start, path, visited, cycles)
            Next

            Return cycles
        End Function

        ''' <summary>
        ''' The depth first search of <see cref="EnumerateCycles"/>: it extends the
        ''' current path with the out links of the last node and records a cycle
        ''' whenever the path closes back onto its start node. Only nodes with an
        ''' index above the start node are entered, which makes every cycle be
        ''' discovered from its smallest node exactly once.
        ''' </summary>
        Private Sub SearchCycles(topology As Model.NetworkTopology,
                                 start As Integer,
                                 current As Integer,
                                 path As List(Of Integer),
                                 visited As Boolean(),
                                 cycles As List(Of Integer()))
            For Each [next] As Integer In topology.OutLinks(current)
                If [next] = start Then
                    cycles.Add(path.ToArray())
                ElseIf [next] > start AndAlso Not visited([next]) Then
                    visited([next]) = True
                    path.Add([next])

                    Call SearchCycles(topology, start, [next], path, visited, cycles)

                    path.RemoveAt(path.Count - 1)
                    visited([next]) = False
                End If
            Next
        End Sub

        ''' <summary>
        ''' The sign product of a directed cycle: the product of the signs of the
        ''' links along the cycle. A negative product means a negative feedback
        ''' loop, a positive product means a positive feedback loop.
        ''' (有向环的符号乘积：环上各链接符号之积，负者为负反馈环，正者为正反馈环)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        ''' <param name="cycle">the node sequence of the cycle</param>
        Public Function CycleSignProduct(topology As Model.NetworkTopology, cycle As Integer()) As Integer
            Dim product As Integer = 1

            For i As Integer = 0 To cycle.Length - 1
                Dim from As Integer = cycle(i)
                Dim [to] As Integer = cycle((i + 1) Mod cycle.Length)

                product *= topology.SignOf(from, [to])
            Next

            Return product
        End Function

        ''' <summary>
        ''' Whether a topology owns at least one negative feedback loop, i.e. a
        ''' directed cycle with a negative sign product.
        ''' (拓扑是否含有至少一个负反馈环)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Function HasNegativeFeedbackLoop(topology As Model.NetworkTopology) As Boolean
            For Each cycle As Integer() In EnumerateCycles(topology)
                If CycleSignProduct(topology, cycle) < 0 Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Whether a topology owns the NFBLB motif: a negative feedback loop
        ''' that passes through the buffering node B and the output node C, so
        ''' that the output feeds back to itself through the buffer, while the
        ''' output does not regulate the input node A directly.
        ''' (拓扑是否含有 NFBLB 基序：经过缓冲节点 B 与输出节点 C 的负反馈环，且输出不直接回连输入)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Function HasNFBLB(topology As Model.NetworkTopology) As Boolean
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            ' the feedback has to pass through the buffer, a direct C→A link
            ' is the defect that the paper lists for the nonadaptive networks
            If topology.SignOf(Model.NetworkTopology.OutputNode, Model.NetworkTopology.InputNode) <> 0 Then
                Return False
            End If

            For Each cycle As Integer() In EnumerateCycles(topology)
                If CycleSignProduct(topology, cycle) >= 0 Then
                    Continue For
                End If

                Dim hasBuffer As Boolean = cycle.Contains(Model.NetworkTopology.BufferNode)
                Dim hasOutput As Boolean = cycle.Contains(Model.NetworkTopology.OutputNode)

                If hasBuffer AndAlso hasOutput Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Whether a topology owns the IFFLP motif: the input A reaches the
        ''' output C along the direct link and along the path through B, the two
        ''' paths own opposite signs and B directly regulates C.
        ''' (拓扑是否含有 IFFLP 基序：输入到输出的直接路径与经过 B 的路径符号相反，且 B 直接调控 C)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Function HasIFFLP(topology As Model.NetworkTopology) As Boolean
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            Dim a As Integer = Model.NetworkTopology.InputNode
            Dim b As Integer = Model.NetworkTopology.BufferNode
            Dim c As Integer = Model.NetworkTopology.OutputNode

            Dim direct As SByte = topology.SignOf(a, c)
            Dim viaB1 As SByte = topology.SignOf(a, b)
            Dim viaB2 As SByte = topology.SignOf(b, c)

            If direct = 0 OrElse viaB1 = 0 OrElse viaB2 = 0 Then
                Return False
            End If

            ' the two paths from A to C own opposite signs: incoherent
            Return direct * viaB1 * viaB2 < 0
        End Function

        ''' <summary>
        ''' Enumerate every simple directed path from a node onto another node,
        ''' together with the sign product of the path.
        ''' (枚举两节点间的全部简单有向路径及其符号乘积)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        ''' <param name="from">the source node index</param>
        ''' <param name="to">the target node index</param>
        Public Function EnumeratePaths(topology As Model.NetworkTopology,
                                       from As Integer,
                                       [to] As Integer) As List(Of (nodes As Integer(), signProduct As Integer))
            If topology Is Nothing Then
                Throw New ArgumentNullException(NameOf(topology))
            End If

            Dim paths As New List(Of (Integer(), Integer))()
            Dim path As New List(Of Integer)()
            Dim visited As Boolean() = New Boolean(Model.NetworkTopology.NodeCount - 1) {}

            path.Add(from)
            visited(from) = True

            Call SearchPaths(topology, from, [to], 1, path, visited, paths)

            Return paths
        End Function

        ''' <summary>
        ''' The depth first search of <see cref="EnumeratePaths"/>.
        ''' </summary>
        Private Sub SearchPaths(topology As Model.NetworkTopology,
                                current As Integer,
                                [to] As Integer,
                                signProduct As Integer,
                                path As List(Of Integer),
                                visited As Boolean(),
                                paths As List(Of (Integer(), Integer)))
            For Each [next] As Integer In topology.OutLinks(current)
                Dim stepSign As Integer = topology.SignOf(current, [next])
                Dim product As Integer = signProduct * stepSign

                If [next] = [to] Then
                    paths.Add((path.ToArray(), product))
                ElseIf Not visited([next]) Then
                    visited([next]) = True
                    path.Add([next])

                    Call SearchPaths(topology, [next], [to], product, path, visited, paths)

                    path.RemoveAt(path.Count - 1)
                    visited([next]) = False
                End If
            Next
        End Sub

        ''' <summary>
        ''' The number of pairs of directed paths from the input node A onto the
        ''' output node C that own opposite sign products, i.e. the broad
        ''' incoherent feedforward count of a topology. The canonical IFFLP motif
        ''' of <see cref="HasIFFLP"/> is the two path special case of this count.
        ''' (输入 A 到输出 C 的符号相反路径对数量，即广义非相干前馈计数)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Function IncoherentPathPairs(topology As Model.NetworkTopology) As Integer
            Dim paths = EnumeratePaths(topology,
                                       Model.NetworkTopology.InputNode,
                                       Model.NetworkTopology.OutputNode)
            Dim positive As Integer = paths.Where(Function(p) p.signProduct > 0).Count()
            Dim negative As Integer = paths.Where(Function(p) p.signProduct < 0).Count()

            Return positive * negative
        End Function

        ''' <summary>
        ''' The motif flags of a topology: whether it owns the NFBLB motif and
        ''' whether it owns the IFFLP motif.
        ''' (拓扑的基序标志：是否含有 NFBLB 与 IFFLP)
        ''' </summary>
        ''' <param name="topology">the network topology</param>
        Public Function MotifFlags(topology As Model.NetworkTopology) As (nfblb As Boolean, ifflp As Boolean)
            Return (HasNFBLB(topology), HasIFFLP(topology))
        End Function

        ''' <summary>
        ''' Assign the motif flags of every topology result in place.
        ''' (为全部拓扑结果就地赋值基序标志)
        ''' </summary>
        ''' <param name="results">the topology results of the search</param>
        Public Sub AssignMotifFlags(results As IEnumerable(Of Assessment.TopologyResult))
            If results Is Nothing Then
                Throw New ArgumentNullException(NameOf(results))
            End If

            For Each result As Assessment.TopologyResult In results
                Dim flags = MotifFlags(Model.NetworkTopology.FromId(result.TopologyId))

                result.HasNFBLB = flags.nfblb
                result.HasIFFLP = flags.ifflp
            Next
        End Sub

        ''' <summary>
        ''' The Venn statistics of a topology set: how many of the topologies own
        ''' the NFBLB motif only, the IFFLP motif only, both or neither. The
        ''' necessity claim of the paper is that the neither count of the over
        ''' represented functional topologies is zero.
        ''' (拓扑集合的 Venn 统计：只含 NFBLB、只含 IFFLP、两者兼有、两者皆无的拓扑数量)
        ''' </summary>
        ''' <param name="results">the topology results whose flags are counted</param>
        Public Function VennCounts(results As IEnumerable(Of Assessment.TopologyResult)) As (nfblbOnly As Integer, ifflpOnly As Integer, both As Integer, neither As Integer, total As Integer)
            If results Is Nothing Then
                Throw New ArgumentNullException(NameOf(results))
            End If

            Dim nfblbOnly As Integer = 0
            Dim ifflpOnly As Integer = 0
            Dim both As Integer = 0
            Dim neither As Integer = 0
            Dim total As Integer = 0

            For Each result As Assessment.TopologyResult In results
                total += 1

                If result.HasNFBLB AndAlso result.HasIFFLP Then
                    both += 1
                ElseIf result.HasNFBLB Then
                    nfblbOnly += 1
                ElseIf result.HasIFFLP Then
                    ifflpOnly += 1
                Else
                    neither += 1
                End If
            Next

            Return (nfblbOnly, ifflpOnly, both, neither, total)
        End Function

    End Module

End Namespace
