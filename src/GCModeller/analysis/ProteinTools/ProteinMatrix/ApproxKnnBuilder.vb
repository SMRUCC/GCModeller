Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree.ApproximateNearNeighbor
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ProteinStructure

    ''' <summary>
    ''' block-wise approximate nearest-neighbour builder for the streaming pipeline.
    '''
    ''' the dense SVD embeddings are read in blocks, wrapped as <see cref="TagVector"/> and passed
    ''' to <see cref="ApproximateNearNeighbor.FindNeighbors"/>. because the approximate index is
    ''' built per-block the working set stays at one block's worth of points instead of all m rows.
    ''' produced directed neighbour lists are symmetrised (u-v saved once) and streamed to disk as
    ''' undirected edge triples so the Louvain stage can consume them without re-reading the
    ''' embedding matrix.
    ''' </summary>
    Public Class ApproxKnnBuilder

        Public Const KNN_FILE As String = "knn_edges.tsv"
        Public Const KNN_META_FILE As String = "knn_meta.json"

        Private ReadOnly workDir As String
        Private edgeWriter As StreamWriter
        Private seenEdges As New HashSet(Of String)
        Private edgeCount As Integer

        Public Sub New(workDir As String)
            Me.workDir = workDir
        End Sub

        ''' <summary>
        ''' build the KNN graph from the streaming SVD embeddings and write undirected edges to disk.
        ''' </summary>
        Public Sub Build(embeddings As IEnumerable(Of (rowIndex As Integer, vector As Double())), k As Integer, blockSize As Integer)
            Dim path = System.IO.Path.Combine(workDir, KNN_FILE)
            edgeWriter = New StreamWriter(New BufferedStream(New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20)), Encoding.ASCII)

            Dim block As New List(Of (rowIndex As Integer, vector As Double()))

            For Each e In embeddings.SafeQuery
                block.Add(e)

                If block.Count >= blockSize Then
                    Call FlushBlock(block, k)
                    block.Clear()
                End If
            Next

            If block.Count > 0 Then
                Call FlushBlock(block, k)
            End If

            edgeWriter.Flush()
            edgeWriter.Dispose()

            Call File.WriteAllText(System.IO.Path.Combine(workDir, KNN_META_FILE), New Dictionary(Of String, String) From {
                {"edges", edgeCount.ToString},
                {"k", k.ToString}
            }.GetJson)

            Call VBDebugger.EchoLine($" [knn] wrote {edgeCount} undirected edges to {KNN_FILE}")
        End Sub

        Private Sub FlushBlock(block As List(Of (rowIndex As Integer, vector As Double())), k As Integer)
            Dim tags As New List(Of TagVector)

            For i As Integer = 0 To block.Count - 1
                tags.Add(New TagVector(i, block(i).rowIndex.ToString, block(i).vector))
            Next

            ' approximate nearest neighbours within the current block (extensions method, internally
            ' builds the index for this block only)
            Dim neighbors = ApproximateNearNeighbor.FindNeighbors(tags, k)

            For i As Integer = 0 To block.Count - 1
                Dim u = block(i).rowIndex
                Dim nb = neighbors(i)

                If nb.size = 0 OrElse nb.indices Is Nothing Then
                    Continue For
                End If

                For j As Integer = 0 To nb.size - 1
                    Dim v = block(nb.indices(j)).rowIndex
                    Call EmitEdge(u, v, nb.weights(j))
                Next
            Next
        End Sub

        Private Sub EmitEdge(u As Integer, v As Integer, w As Double)
            ' keep only one direction of the undirected edge
            Dim key As String
            Dim a, b As Integer

            If u < v Then
                a = u : b = v
            Else
                a = v : b = u
            End If

            key = a & ":" & b

            If seenEdges.Contains(key) Then
                Return
            End If

            seenEdges.Add(key)
            edgeWriter.WriteLine($"{a}" & vbTab & b & vbTab & w.ToString("G17"))
            edgeCount += 1
        End Sub

        Public Shared Function LoadMeta(workDir As String) As (edges As Integer, k As Integer)
            Dim json = File.ReadAllText(System.IO.Path.Combine(workDir, KNN_META_FILE)).LoadObject(Of Dictionary(Of String, String))
            Return (CInt(Val(json("edges"))), CInt(Val(json("k"))))
        End Function

        ''' <summary>
        ''' stream the undirected edges (u, v, weight) back for the Louvain stage
        ''' </summary>
        Public Shared Iterator Function ReadEdges(workDir As String) As IEnumerable(Of (u As Integer, v As Integer, weight As Double))
            Dim path = System.IO.Path.Combine(workDir, KNN_FILE)

            Using reader = New StreamReader(New BufferedStream(New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1 << 20)))
                Dim line As String = Nothing
                Do
                    line = reader.ReadLine()
                    If line Is Nothing Then Exit Do

                    Dim parts = line.Split(vbTab)
                    Yield (CInt(Val(parts(0))), CInt(Val(parts(1))), Val(parts(2)))
                Loop
            End Using
        End Function
    End Class
End Namespace
