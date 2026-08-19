Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.Louvain
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ProteinStructure

    ''' <summary>
    ''' community detection stage of the streaming pipeline.
    '''
    ''' edges are read back from disk (they were already persisted by <see cref="ApproxKnnBuilder"/>
    ''' so the FASTA / embedding matrices are never re-materialised) and fed into a
    ''' <see cref="NetworkGraph(Of Node, Edge)"/>. classic Louvain is then run via
    ''' <see cref="Builder.Load"/> / <see cref="Builder.SolveClusters"/>. the resulting per-row
    ''' family assignment is streamed to disk keyed by row index, keeping only the assignment array
    ''' (one integer per node) resident instead of any intermediate matrix.
    '''
    ''' NOTE: the in-memory graph is bounded by the number of nodes (= number of sequences) and the
    ''' number of edges. for truly massive node counts where the graph itself does not fit in RAM the
    ''' caller can pre-shard the edge file and run this routine per shard, then merge the communities
    ''' with the supplied <see cref="MergeCommunities"/> helper. the default path reuses the proven
    ''' single-graph Louvain which is already a large improvement over the original all-in-memory
    ''' pipeline that also held every protein sequence string.
    ''' </summary>
    Public Class BlockLouvain

        Public Const ASSIGN_FILE As String = "family_assignment.tsv"
        Public Const ASSIGN_META_FILE As String = "family_meta.json"

        Private ReadOnly workDir As String

        Public Sub New(workDir As String)
            Me.workDir = workDir
        End Sub

        Public Sub Detect(edges As IEnumerable(Of (u As Integer, v As Integer, weight As Double)), nNodes As Integer)
            Dim nodes As Node() = (From i As Integer In Enumerable.Range(0, nNodes)
                                   Select New Node With {.label = "v" & i.ToString}).ToArray
            Dim netEdges As New List(Of Edge(Of Node))

            For Each e In edges.SafeQuery
                netEdges.Add(New Edge(Of Node) With {
                    .U = nodes(e.u),
                    .V = nodes(e.v),
                    .weight = e.weight
                })
            Next

            Dim graph As New NetworkGraph(Of Node, Edge(Of Node))(nodes, netEdges)

            Call VBDebugger.EchoLine($" [louvain] graph built with {nNodes} nodes, running community detection...")

            Dim community = Builder _
                .Load(graph) _
                .SolveClusters() _
                .GetCommunity()

            Dim nFamilies As Integer = 0
            Dim maxFam As Integer = 0

            Using writer = New StreamWriter(New BufferedStream(New FileStream(System.IO.Path.Combine(workDir, ASSIGN_FILE), FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20)), Encoding.ASCII)
                For i As Integer = 0 To nNodes - 1
                    Dim fam = If(community.Length > i, CInt(Val(community(i))), 0)
                    If fam > maxFam Then maxFam = fam
                    writer.WriteLine($"{i}" & vbTab & fam)
                Next
            End Using

            nFamilies = maxFam + 1

            Call File.WriteAllText(System.IO.Path.Combine(workDir, ASSIGN_META_FILE), New Dictionary(Of String, String) From {
                {"nodes", nNodes.ToString},
                {"families", nFamilies.ToString}
            }.GetJson)

            Call VBDebugger.EchoLine($" [louvain] detected {nFamilies} families")
        End Sub

        Public Shared Iterator Function ReadAssignment(workDir As String) As IEnumerable(Of (rowIndex As Integer, family As Integer))
            Dim path = System.IO.Path.Combine(workDir, ASSIGN_FILE)

            Using reader = New StreamReader(New BufferedStream(New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1 << 20)))
                Dim line As String = Nothing
                Do
                    line = reader.ReadLine()
                    If line Is Nothing Then Exit Do

                    Dim parts = line.Split(vbTab)
                    Yield (CInt(Val(parts(0))), CInt(Val(parts(1))))
                Loop
            End Using
        End Function

        ''' <summary>
        ''' merge per-shard community ids into globally consistent family ids using the cross-shard
        ''' edges. used when the graph was sharded to stay under the memory budget.
        ''' </summary>
        Public Shared Function MergeCommunities(shardAssign As IEnumerable(Of (rowIndex As Integer, family As Integer)),
                                                 crossEdges As IEnumerable(Of (u As Integer, v As Integer, weight As Double))) As Dictionary(Of Integer, Integer)
            Dim parent As New Dictionary(Of Integer, Integer)

            For Each a In shardAssign.SafeQuery
                Dim key = a.rowIndex
                If Not parent.ContainsKey(key) Then parent(key) = key
            Next

            For Each e In crossEdges.SafeQuery
                Call Union(parent, e.u, e.v)
            Next

            ' relabel roots to compact family ids
            Dim rootToFam As New Dictionary(Of Integer, Integer)
            Dim nextFam As Integer = 0
            Dim result As New Dictionary(Of Integer, Integer)

            For Each a In shardAssign.SafeQuery
                Dim root = Find(parent, a.rowIndex)
                If Not rootToFam.ContainsKey(root) Then
                    rootToFam(root) = nextFam
                    nextFam += 1
                End If
                result(a.rowIndex) = rootToFam(root)
            Next

            Return result
        End Function

        Private Shared Function Find(parent As Dictionary(Of Integer, Integer), x As Integer) As Integer
            While parent(x) <> x
                parent(x) = parent(parent(x))
                x = parent(x)
            End While
            Return x
        End Function

        Private Shared Sub Union(parent As Dictionary(Of Integer, Integer), a As Integer, b As Integer)
            If Not parent.ContainsKey(a) Then parent(a) = a
            If Not parent.ContainsKey(b) Then parent(b) = b
            Dim ra = Find(parent, a)
            Dim rb = Find(parent, b)
            If ra <> rb Then parent(ra) = rb
        End Sub
    End Class
End Namespace