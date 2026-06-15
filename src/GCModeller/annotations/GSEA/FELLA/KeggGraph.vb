' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' KeggGraph.vb - KEGG multi-layer heterogeneous graph data structure
' 
' Represents the KEGG knowledge model as a hierarchical graph with 5 layers:
'   Pathway -> Module -> Enzyme -> Reaction -> Compound
' 
' Nodes are connected by edges representing biochemical relationships.
' This structure is the foundation for FELLA's network diffusion analysis.
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Types of nodes in the KEGG hierarchical graph.
    ''' From top (pathway) to bottom (compound).
    ''' </summary>
    Public Enum KeggNodeType
        Pathway = 0
        Module = 1
        Enzyme = 2
        Reaction = 3
        Compound = 4
    End Enum

    ''' <summary>
    ''' Represents a single node in the KEGG graph.
    ''' Each node has a KEGG ID, a human-readable name, and a type.
    ''' </summary>
    Public Class KeggNode
        ''' <summary>KEGG identifier (e.g., "C00002", "R00710", "1.1.1.1", "M00001", "hsa00010")</summary>
        Public Property Id As String

        ''' <summary>Human-readable name</summary>
        Public Property Name As String

        ''' <summary>Node type in the KEGG hierarchy</summary>
        Public Property NodeType As KeggNodeType

        ''' <summary>Internal index for matrix operations</summary>
        Public Property Index As Integer

        Public Overrides Function ToString() As String
            Return $"{NodeType}:{Id} ({Name})"
        End Function
    End Class

    ''' <summary>
    ''' Represents a directed edge in the KEGG graph.
    ''' In FELLA, edges are undirected for diffusion but directed for
    ''' the hierarchical structure (pathway->module->enzyme->reaction->compound).
    ''' </summary>
    Public Class KeggEdge
        ''' <summary>Source node index</summary>
        Public Property SourceIndex As Integer

        ''' <summary>Target node index</summary>
        Public Property TargetIndex As Integer

        ''' <summary>Edge type description (e.g., "pathway-module", "reaction-compound")</summary>
        Public Property EdgeType As String

        Public Overrides Function ToString() As String
            Return $"{SourceIndex} -> {TargetIndex} ({EdgeType})"
        End Function
    End Class

    ''' <summary>
    ''' The KEGG multi-layer heterogeneous graph.
    ''' Contains all nodes (pathways, modules, enzymes, reactions, compounds)
    ''' and their connecting edges. Provides methods to build adjacency
    ''' matrices and Laplacians for diffusion/PageRank computations.
    ''' </summary>
    Public Class KeggGraph
        Private _nodes As New List(Of KeggNode)
        Private _edges As New List(Of KeggEdge)
        Private _idToIndex As New Dictionary(Of String, Integer)
        Private _adjacencyDirty As Boolean = True
        Private _adjacencyMatrix As Math.Matrix

        ''' <summary>All nodes in the graph</summary>
        Public ReadOnly Property Nodes As IReadOnlyList(Of KeggNode)
            Get
                Return _nodes
            End Get
        End Property

        ''' <summary>All edges in the graph</summary>
        Public ReadOnly Property Edges As IReadOnlyList(Of KeggEdge)
            Get
                Return _edges
            End Get
        End Property

        ''' <summary>Total number of nodes</summary>
        Public ReadOnly Property NodeCount As Integer
            Get
                Return _nodes.Count
            End Get
        End Property

        ''' <summary>Total number of edges</summary>
        Public ReadOnly Property EdgeCount As Integer
            Get
                Return _edges.Count
            End Get
        End Property

        ''' <summary>Get node by internal index</summary>
        Public Function GetNode(index As Integer) As KeggNode
            Return _nodes(index)
        End Function

        ''' <summary>Get node index by KEGG ID</summary>
        Public Function GetIndex(keggId As String) As Integer
            Dim idx As Integer
            If _idToIndex.TryGetValue(keggId, idx) Then
                Return idx
            End If
            Return -1
        End Function

        ''' <summary>Check if a KEGG ID exists in the graph</summary>
        Public Function ContainsId(keggId As String) As Boolean
            Return _idToIndex.ContainsKey(keggId)
        End Function

        ''' <summary>Get all nodes of a specific type</summary>
        Public Function GetNodesByType(type As KeggNodeType) As List(Of KeggNode)
            Dim result As New List(Of KeggNode)
            For Each node In _nodes
                If node.NodeType = type Then result.Add(node)
            Next
            Return result
        End Function

        ''' <summary>Get indices of all nodes of a specific type</summary>
        Public Function GetIndicesByType(type As KeggNodeType) As List(Of Integer)
            Dim result As New List(Of Integer)
            For Each node In _nodes
                If node.NodeType = type Then result.Add(node.Index)
            Next
            Return result
        End Function

        ''' <summary>
        ''' Add a node to the graph. Returns the internal index.
        ''' If the node already exists, returns the existing index.
        ''' </summary>
        Public Function AddNode(keggId As String, name As String, type As KeggNodeType) As Integer
            Dim existingIdx As Integer
            If _idToIndex.TryGetValue(keggId, existingIdx) Then
                Return existingIdx
            End If

            Dim node As New KeggNode With {
                .Id = keggId,
                .Name = name,
                .NodeType = type,
                .Index = _nodes.Count
            }
            _nodes.Add(node)
            _idToIndex(keggId) = node.Index
            _adjacencyDirty = True
            Return node.Index
        End Function

        ''' <summary>
        ''' Add an undirected edge between two nodes.
        ''' Both directions are added to the adjacency matrix.
        ''' </summary>
        Public Sub AddEdge(sourceId As String, targetId As String, edgeType As String)
            Dim srcIdx As Integer = GetIndex(sourceId)
            Dim tgtIdx As Integer = GetIndex(targetId)
            If srcIdx < 0 OrElse tgtIdx < 0 Then
                Throw New ArgumentException($"Node not found: {sourceId} or {targetId}")
            End If
            AddEdgeByIndex(srcIdx, tgtIdx, edgeType)
        End Sub

        ''' <summary>
        ''' Add an undirected edge by node indices.
        ''' </summary>
        Public Sub AddEdgeByIndex(sourceIdx As Integer, targetIdx As Integer, edgeType As String)
            ' Avoid duplicate edges
            For Each e In _edges
                If (e.SourceIndex = sourceIdx AndAlso e.TargetIndex = targetIdx) OrElse
                   (e.SourceIndex = targetIdx AndAlso e.TargetIndex = sourceIdx) Then
                    Return
                End If
            Next

            Dim edge As New KeggEdge With {
                .SourceIndex = sourceIdx,
                .TargetIndex = targetIdx,
                .EdgeType = edgeType
            }
            _edges.Add(edge)
            _adjacencyDirty = True
        End Sub

        ''' <summary>
        ''' Build the adjacency matrix of the graph.
        ''' For undirected graph: A[i,j] = 1 if edge exists between i and j.
        ''' </summary>
        Public Function BuildAdjacencyMatrix() As Math.Matrix
            If Not _adjacencyDirty AndAlso _adjacencyMatrix IsNot Nothing Then
                Return _adjacencyMatrix
            End If

            Dim n As Integer = _nodes.Count
            Dim A As New Math.Matrix(n, n)

            For Each edge In _edges
                A(edge.SourceIndex, edge.TargetIndex) = 1.0
                A(edge.TargetIndex, edge.SourceIndex) = 1.0 ' Undirected
            Next

            _adjacencyMatrix = A
            _adjacencyDirty = False
            Return A
        End Function

        ''' <summary>
        ''' Compute the degree vector: d[i] = number of neighbors of node i.
        ''' </summary>
        Public Function ComputeDegrees() As Double()
            Dim A As Math.Matrix = BuildAdjacencyMatrix()
            Return A.RowSums()
        End Function

        ''' <summary>
        ''' Compute the unnormalized Laplacian: L = D - A
        ''' where D is the diagonal degree matrix and A is the adjacency matrix.
        ''' </summary>
        Public Function ComputeLaplacian() As Math.Matrix
            Dim A As Math.Matrix = BuildAdjacencyMatrix()
            Dim degrees As Double() = ComputeDegrees()
            Dim D As Math.Matrix = Math.Matrix.Diagonal(degrees)
            Return D - A
        End Function

        ''' <summary>
        ''' Compute the regularized Laplacian for FELLA diffusion:
        ''' L_gamma = L + gamma * B
        ''' where L is the unnormalized Laplacian, gamma is the regularization
        ''' parameter (typically 1), and B is a diagonal matrix with B[i,i] = 1
        ''' for pathway nodes (top-level nodes that "leak" diffusion) and 0 otherwise.
        ''' 
        ''' In FELLA, only pathway nodes have the leak term, allowing diffusion
        ''' signal to escape at the top of the hierarchy, which models the
        ''' biological intuition that perturbations propagate upward but
        ''' pathways act as "sinks" for the diffusion process.
        ''' </summary>
        Public Function ComputeRegularizedLaplacian(Optional gamma As Double = 1.0) As Math.Matrix
            Dim L As Math.Matrix = ComputeLaplacian()
            Dim n As Integer = _nodes.Count

            ' B: diagonal matrix with 1 for pathway nodes, 0 otherwise
            Dim bDiag(n - 1) As Double
            For Each node In _nodes
                If node.NodeType = KeggNodeType.Pathway Then
                    bDiag(node.Index) = 1.0
                End If
            Next
            Dim B As Math.Matrix = Math.Matrix.Diagonal(bDiag)

            Return L + gamma * B
        End Function

        ''' <summary>
        ''' Compute the transition matrix for PageRank.
        ''' M[i,j] = 1/degree(j) if edge (j,i) exists, else 0.
        ''' This is the column-stochastic transition matrix.
        ''' </summary>
        Public Function ComputeTransitionMatrix() As Math.Matrix
            Dim A As Math.Matrix = BuildAdjacencyMatrix()
            Dim n As Integer = _nodes.Count
            Dim degrees As Double() = ComputeDegrees()
            Dim M As New Math.Matrix(n, n)

            For j = 0 To n - 1
                If degrees(j) > 0 Then
                    For i = 0 To n - 1
                        M(i, j) = A(i, j) / degrees(j)
                    Next
                End If
            Next

            Return M
        End Function

        ''' <summary>
        ''' Get the neighbors of a node.
        ''' </summary>
        Public Function GetNeighbors(nodeIndex As Integer) As List(Of Integer)
            Dim neighbors As New List(Of Integer)
            For Each edge In _edges
                If edge.SourceIndex = nodeIndex Then
                    neighbors.Add(edge.TargetIndex)
                ElseIf edge.TargetIndex = nodeIndex Then
                    neighbors.Add(edge.SourceIndex)
                End If
            Next
            Return neighbors
        End Function

        ''' <summary>
        ''' Get a subgraph containing only the specified node indices.
        ''' Re-indexes nodes from 0.
        ''' </summary>
        Public Function GetSubGraph(nodeIndices As HashSet(Of Integer)) As KeggGraph
            Dim subGraph As New KeggGraph()
            Dim indexMap As New Dictionary(Of Integer, Integer)

            ' Add nodes
            For Each idx In nodeIndices
                Dim node = _nodes(idx)
                Dim newIdx = subGraph.AddNode(node.Id, node.Name, node.NodeType)
                indexMap(idx) = newIdx
            Next

            ' Add edges that connect nodes within the subset
            For Each edge In _edges
                If nodeIndices.Contains(edge.SourceIndex) AndAlso nodeIndices.Contains(edge.TargetIndex) Then
                    subGraph.AddEdgeByIndex(indexMap(edge.SourceIndex), indexMap(edge.TargetIndex), edge.EdgeType)
                End If
            Next

            Return subGraph
        End Function

        ''' <summary>
        ''' Print graph statistics.
        ''' </summary>
        Public Function GetStatistics() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine($"Graph Statistics:")
            sb.AppendLine($"  Total nodes: {_nodes.Count}")
            sb.AppendLine($"  Total edges: {_edges.Count}")

            For Each type As KeggNodeType In [Enum].GetValues(GetType(KeggNodeType))
                Dim count = _nodes.Count(Function(n) n.NodeType = type)
                sb.AppendLine($"  {type}: {count} nodes")
            Next

            Return sb.ToString()
        End Function

    End Class

End Namespace
