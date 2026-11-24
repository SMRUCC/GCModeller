Imports System.Globalization
Imports System.Text

Namespace Model

    ''' <summary>
    ''' A candidate network topology of the three node enzyme network of the
    ''' Ma et al. 2009 adaptation circuit search.
    ''' (三节点酶网络的一种候选拓扑，即论文搜索空间中的一个元素)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The network owns three nodes: node 0 is the input node ``A``, node 1 is
    ''' the buffering / regulating node ``B`` and node 2 is the output node ``C``.
    ''' Nine directed links are possible, and the link slots are stored in the
    ''' fixed order used by the paper and by the README of this project:
    ''' </para>
    ''' <code>
    ''' slot 0: A→B    slot 1: A→C    slot 2: B→A
    ''' slot 3: B→C    slot 4: C→A    slot 5: C→B
    ''' slot 6: A→A    slot 7: B→B    slot 8: C→C
    ''' </code>
    ''' <para>
    ''' Every slot owns a sign inside of ``{-1, 0, +1}``: ``+1`` means activation,
    ''' ``-1`` means inactivation and ``0`` means no regulation, so the complete
    ''' search space of the three node network is ``3^9 = 19683`` topologies.
    ''' </para>
    ''' <para>
    ''' The topology identifier is the base 3 encoding of the sign vector:
    ''' ``id = Σ_i (sign_i + 1) * 3^i``, which maps every topology onto a unique
    ''' integer inside of ``0 .. 19682``. All statistics of the search pipeline
    ''' are keyed by this identifier.
    ''' </para>
    ''' </remarks>
    Public Class NetworkTopology

        ''' <summary>The number of network nodes: A, B and C.</summary>
        Public Const NodeCount As Integer = 3
        ''' <summary>The number of directed link slots, N^2 for N nodes.</summary>
        Public Const LinkSlots As Integer = NodeCount * NodeCount
        ''' <summary>The node index of the input node A.</summary>
        Public Const InputNode As Integer = 0
        ''' <summary>The node index of the buffering node B.</summary>
        Public Const BufferNode As Integer = 1
        ''' <summary>The node index of the output node C.</summary>
        Public Const OutputNode As Integer = 2
        ''' <summary>The display names of the three nodes.</summary>
        Public Shared ReadOnly NodeNames As String() = {"A", "B", "C"}

        ''' <summary>The sign of each link slot, values inside of {-1, 0, +1}.</summary>
        Public ReadOnly Property Signs As SByte()
            Get
                Return m_signs
            End Get
        End Property

        Private ReadOnly m_signs As SByte()

        ''' <summary>
        ''' Create a topology from its sign vector, the vector is cloned so that
        ''' the caller can not mutate the topology afterwards.
        ''' </summary>
        ''' <param name="signs">a 9 length sign vector with values in {-1, 0, +1}</param>
        ''' <exception cref="ArgumentException">the sign vector has a wrong length or an invalid sign</exception>
        Public Sub New(signs As SByte())
            If signs Is Nothing OrElse signs.Length <> LinkSlots Then
                Throw New ArgumentException($"the sign vector must own {LinkSlots} elements!", NameOf(signs))
            End If

            m_signs = New SByte(LinkSlots - 1) {}

            For i As Integer = 0 To LinkSlots - 1
                Dim s As SByte = signs(i)

                If s < -1 OrElse s > 1 Then
                    Throw New ArgumentException($"the sign of the link slot {i} must be inside of {{-1, 0, +1}}, but got {s}!", NameOf(signs))
                End If

                m_signs(i) = s
            Next
        End Sub

        ''' <summary>
        ''' Create a topology from its integer identifier.
        ''' (从拓扑编号解码还原拓扑对象)
        ''' </summary>
        ''' <param name="id">the base 3 encoding of the sign vector, 0 .. 19682</param>
        Public Shared Function FromId(id As Integer) As NetworkTopology
            If id < 0 OrElse id >= TotalCount Then
                Throw New ArgumentOutOfRangeException(NameOf(id), id, $"the topology id must be inside of 0 .. {TotalCount - 1}.")
            End If

            Dim signs As SByte() = New SByte(LinkSlots - 1) {}
            Dim rest As Integer = id

            For i As Integer = 0 To LinkSlots - 1
                ' the digit of the base 3 encoding is sign + 1, so that
                ' -1 → 0, 0 → 1 and +1 → 2
                signs(i) = CSByte(rest Mod 3 - 1)
                rest \= 3
            Next

            Return New NetworkTopology(signs)
        End Function

        ''' <summary>
        ''' The base 3 encoding of the sign vector of this topology.
        ''' (拓扑的唯一整数编号)
        ''' </summary>
        Public ReadOnly Property Id As Integer
            Get
                Return EncodeId(m_signs)
            End Get
        End Property

        ''' <summary>
        ''' Encode a sign vector into its unique topology identifier.
        ''' </summary>
        ''' <param name="signs">a 9 length sign vector</param>
        ''' <returns>the base 3 encoding, ``Σ_i (sign_i + 1) * 3^i``</returns>
        Public Shared Function EncodeId(signs As SByte()) As Integer
            If signs Is Nothing OrElse signs.Length <> LinkSlots Then
                Throw New ArgumentException($"the sign vector must own {LinkSlots} elements!", NameOf(signs))
            End If

            Dim id As Integer = 0
            Dim weight As Integer = 1

            For i As Integer = 0 To LinkSlots - 1
                id += (signs(i) + 1) * weight
                weight *= 3
            Next

            Return id
        End Function

        ''' <summary>
        ''' The sign of the link from the source node onto the target node,
        ''' ``0`` when there is no such link.
        ''' </summary>
        Public Function SignOf(source As Integer, target As Integer) As SByte
            Return m_signs(LinkIndex(source, target))
        End Function

        ''' <summary>
        ''' The link slot index of a directed link, using the fixed slot order.
        ''' </summary>
        ''' <param name="source">the source node index, 0 .. 2</param>
        ''' <param name="target">the target node index, 0 .. 2</param>
        Public Shared Function LinkIndex(source As Integer, target As Integer) As Integer
            If source < 0 OrElse source >= NodeCount OrElse target < 0 OrElse target >= NodeCount Then
                Throw New ArgumentOutOfRangeException(If(source < 0 OrElse source >= NodeCount, NameOf(source), NameOf(target)))
            End If

            ' the fixed slot order puts the six off diagonal links first
            ' (row major over the source and target pairs), then the three self loops
            If source = target Then
                Return 6 + source
            Else
                Return (source * (NodeCount - 1)) + target - If(target > source, 1, 0)
            End If
        End Function

        ''' <summary>
        ''' The (source, target) node pair of a link slot index.
        ''' </summary>
        ''' <param name="slot">the link slot index, 0 .. 8</param>
        Public Shared Function LinkAt(slot As Integer) As (source As Integer, target As Integer)
            If slot < 0 OrElse slot >= LinkSlots Then
                Throw New ArgumentOutOfRangeException(NameOf(slot), slot, $"the link slot must be inside of 0 .. {LinkSlots - 1}.")
            End If

            If slot >= 6 Then
                Return (slot - 6, slot - 6)
            End If

            Dim source As Integer = slot \ (NodeCount - 1)
            Dim offset As Integer = slot Mod (NodeCount - 1)
            Dim target As Integer = If(offset >= source, offset + 1, offset)

            Return (source, target)
        End Function

        ''' <summary>
        ''' The number of active links, i.e. the number of slots with a non zero sign.
        ''' (网络中实际存在的链接数量，论文最小网络搜索的关键指标)
        ''' </summary>
        Public ReadOnly Property LinkCount As Integer
            Get
                Dim n As Integer = 0

                For i As Integer = 0 To LinkSlots - 1
                    If m_signs(i) <> 0 Then
                        n += 1
                    End If
                Next

                Return n
            End Get
        End Property

        ''' <summary>
        ''' The total number of topologies of the three node network, 3^9.
        ''' (三节点网络拓扑空间的大小)
        ''' </summary>
        Public Shared ReadOnly Property TotalCount As Integer
            Get
                Return 19683
            End Get
        End Property

        ''' <summary>
        ''' The target nodes of all active out links of a node.
        ''' (某个节点的全部活跃出链接目标节点)
        ''' </summary>
        ''' <param name="node">the node index, 0 .. 2</param>
        Public Function OutLinks(node As Integer) As Integer()
            Dim list As New List(Of Integer)()

            For target As Integer = 0 To NodeCount - 1
                If SignOf(node, target) <> 0 Then
                    list.Add(target)
                End If
            Next

            Return list.ToArray
        End Function

        ''' <summary>
        ''' The source nodes of all active in links of a node together with
        ''' the sign of each link.
        ''' (某个节点的全部活跃入链接来源节点及其符号)
        ''' </summary>
        ''' <param name="node">the node index, 0 .. 2</param>
        Public Function InLinks(node As Integer) As (source As Integer, sign As SByte)()
            Dim list As New List(Of (Integer, SByte))()

            For source As Integer = 0 To NodeCount - 1
                Dim s As SByte = SignOf(source, node)

                If s <> 0 Then
                    list.Add((source, s))
                End If
            Next

            Return list.ToArray
        End Function

        ''' <summary>
        ''' Test whether the target node is reachable from the source node in
        ''' the directed graph of the active links, using a breadth first search.
        ''' (BFS 判定有向可达性，论文用于剪除输入到输出不连通的拓扑)
        ''' </summary>
        ''' <param name="source">the source node index</param>
        ''' <param name="target">the target node index</param>
        Public Function IsReachable(source As Integer, target As Integer) As Boolean
            If source = target Then
                Return True
            End If

            Dim visited As Boolean() = New Boolean(NodeCount - 1) {}
            Dim queue As New List(Of Integer)()
            Dim cursor As Integer = 0

            visited(source) = True
            queue.Add(source)

            While cursor < queue.Count
                Dim u As Integer = queue(cursor)
                cursor += 1

                For Each v As Integer In OutLinks(u)
                    If v = target Then
                        Return True
                    End If

                    If Not visited(v) Then
                        visited(v) = True
                        queue.Add(v)
                    End If
                Next
            End While

            Return False
        End Function

        ''' <summary>
        ''' Whether the input node A can reach the output node C through the
        ''' active links, the structural pre filter of the paper that removes
        ''' 3645 of the 19683 topologies.
        ''' (输入 A 到输出 C 是否存在因果连通路径)
        ''' </summary>
        Public ReadOnly Property HasInputOutputPath As Boolean
            Get
                Return IsReachable(InputNode, OutputNode)
            End Get
        End Property

        ''' <summary>
        ''' The sign vector as a compact symbol string, for example ``+-0+00-00``.
        ''' (符号向量的紧凑字符串形式)
        ''' </summary>
        Public Function ToSymbolString() As String
            Dim sb As New StringBuilder(LinkSlots)

            For i As Integer = 0 To LinkSlots - 1
                sb.Append(If(m_signs(i) > 0, "+"c, If(m_signs(i) < 0, "-"c, "0"c)))
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Parse a symbol string such as ``+-0+00-00`` back into a topology.
        ''' </summary>
        ''' <param name="symbols">a 9 character string of ``+``, ``-`` and ``0``</param>
        Public Shared Function FromSymbolString(symbols As String) As NetworkTopology
            If symbols Is Nothing OrElse symbols.Length <> LinkSlots Then
                Throw New ArgumentException($"the symbol string must own {LinkSlots} characters!", NameOf(symbols))
            End If

            Dim signs As SByte() = New SByte(LinkSlots - 1) {}

            For i As Integer = 0 To LinkSlots - 1
                Select Case symbols(i)
                    Case "+"c
                        signs(i) = 1
                    Case "-"c
                        signs(i) = -1
                    Case "0"c
                        signs(i) = 0
                    Case Else
                        Throw New ArgumentException($"the character '{symbols(i)}' at the slot {i} is not a valid link sign!", NameOf(symbols))
                End Select
            Next

            Return New NetworkTopology(signs)
        End Function

        ''' <summary>
        ''' The sign vector as an integer vector, which is the input format of
        ''' the Hamming distance helper of the data mining library.
        ''' (供 Hamming 距离计算使用的整数向量形式)
        ''' </summary>
        Public Function ToIntegerVector() As Integer()
            Dim v As Integer() = New Integer(LinkSlots - 1) {}

            For i As Integer = 0 To LinkSlots - 1
                v(i) = m_signs(i)
            Next

            Return v
        End Function

        ''' <summary>
        ''' A deep copy of this topology.
        ''' </summary>
        Public Function Clone() As NetworkTopology
            Return New NetworkTopology(m_signs)
        End Function

        ''' <summary>
        ''' The topology identifier and its symbol string.
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"[{Id}] {ToSymbolString()}"
        End Function

        ''' <summary>
        ''' Two topologies are equal when all of their nine signs are equal.
        ''' </summary>
        Public Overrides Function Equals(obj As Object) As Boolean
            Dim other As NetworkTopology = TryCast(obj, NetworkTopology)

            If other Is Nothing Then
                Return False
            End If

            For i As Integer = 0 To LinkSlots - 1
                If m_signs(i) <> other.m_signs(i) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' The hash code of the topology identifier.
        ''' </summary>
        Public Overrides Function GetHashCode() As Integer
            Return Id
        End Function

    End Class

End Namespace
