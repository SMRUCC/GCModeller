Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.LPA
Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree.ApproximateNearNeighbor
Imports Microsoft.VisualBasic.Data.GraphTheory.KNearNeighbors
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.NLP
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace FamilyCluster

    ''' <summary>
    ''' unsupervised protein family clustering pipeline.
    ''' </summary>
    ''' <remarks>
    ''' pipeline (all parameters are configurable, defaults follow the described algorithm):
    ''' <list type="number">
    ''' <item><description>extract kmer features (default length 5) from the protein database and select a top-N vocabulary;</description></item>
    ''' <item><description>vectorize each protein as a TF-IDF document over the vocabulary;</description></item>
    ''' <item><description>compress the TF-IDF matrix with TruncatedSVD (default 9 dims);</description></item>
    ''' <item><description>build a KNN similarity network from the embedding;</description></item>
    ''' <item><description>split the network into communities with the Louvain algorithm (the protein families);</description></item>
    ''' <item><description>for every family run an MSA and pick the sequence with the fewest edits as the reference.</description></item>
    ''' </list>
    ''' </remarks>
    Public Class ProteinFamilyClustering

        ''' <summary>
        ''' kmer length in residues
        ''' </summary>
        Public Property k As Integer = 5
        ''' <summary>
        ''' number of kmer features to keep as the dictionary
        ''' </summary>
        Public Property topN As Integer = 10000
        ''' <summary>
        ''' target dimension after TruncatedSVD compression
        ''' </summary>
        Public Property svdDims As Integer = 9
        ''' <summary>
        ''' number of nearest neighbours used when building the similarity graph
        ''' </summary>
        Public Property knnK As Integer = 30
        ''' <summary>
        ''' similarity cutoff: KNN edges whose weight is below this value are dropped
        ''' </summary>
        Public Property similarityCutoff As Double = 0.0
        ''' <summary>
        ''' ranking mode used when selecting the top-N kmer vocabulary
        ''' </summary>
        Public Property kmerSortMode As KmerVocabulary.SortMode = KmerVocabulary.SortMode.Ascending

        ''' <summary>
        ''' run the full clustering pipeline over a FASTA protein database (file or directory)
        ''' </summary>
        ''' <param name="fastaHandle">path to a FASTA file or a directory of FASTA files</param>
        Public Function Run(fastaHandle As String) As ClusteringResult
            Dim originals = LoadSequences(fastaHandle).ToArray
            Dim sequences = originals _
                .Select(Function(fa) (fa.Title, fa.SequenceData)) _
                .ToArray

            Call VBDebugger.EchoLine($" [cluster] loaded {sequences.Length} protein sequences from '{fastaHandle}'")

            ' 1. kmer vocabulary (single pass counting + top-N selection)
            Dim vocab = KmerVocabulary.Build(sequences, k, topN, kmerSortMode)
            Dim words = vocab.words
            Dim nWords = words.Length
            Dim nSeq = sequences.Length

            If nSeq <= 1 Then
                Throw New InvalidOperationException("at least two protein sequences are required for clustering")
            End If

            ' 2. TF-IDF matrix over the fixed vocabulary
            Dim tfidf = BuildTfidf(vocab, sequences, words)
            Dim rownames As String() = tfidf.rownames _
                .Select(Function(r) CStr(r)) _
                .ToArray

            ' 3. TruncatedSVD compression (sparse input)
            Dim effDims As Integer = Math.Min(svdDims, Math.Min(nSeq, nWords))
            Dim sparse = ToSparseMatrix(tfidf, nSeq, nWords)
            Dim svd As Double()() = TruncatedSVD.Reduce(sparse, effDims)

            Call VBDebugger.EchoLine($" [cluster] TruncatedSVD -> {effDims} dims (requested {svdDims}, limited by matrix shape)")

            ' 4. KNN similarity network
            Dim knn = New KNN(New Cosine, similarityCutoff)
            Dim neighbors = knn.FindNeighbors(New NumericMatrix(svd), knnK).ToArray
            Dim edges = BuildEdgeList(neighbors, nSeq, similarityCutoff)

            ' 5. Louvain community detection on the KNN graph
            Dim assignments = LouvainPartition(edges, nSeq)

            Call VBDebugger.EchoLine($" [cluster] Louvain produced {assignments.Distinct.Count} families")

            ' 6. MSA per family + reference selection
            Dim families = BuildFamilies(assignments, rownames, originals)

            Return New ClusteringResult With {
                .sequenceNames = rownames,
                .familyAssignments = assignments,
                .referenceSequences = families _
                    .Where(Function(f) f.reference IsNot Nothing) _
                    .ToDictionary(Function(f) f.familyId, Function(f) f.reference),
                .families = families,
                .tfidfMatrix = tfidf,
                .vocabulary = words,
                .svdVectors = svd,
                .svdDims = effDims,
                .knnEdges = edges,
                .k = k,
                .topN = topN
            }
        End Function

        Private Iterator Function LoadSequences(fastaHandle As String) As IEnumerable(Of FastaSeq)
            For Each fa In StreamIterator.SeqSource(fastaHandle, {"*.fa"}, debug:=False)
                If Not fa Is Nothing AndAlso Not fa.SequenceData.StringEmpty Then
                    Yield fa
                End If
            Next
        End Function

        ''' <summary>
        ''' run the clustering pipeline in streaming mode, designed for databases that do not fit in
        ''' memory (e.g. a 20GB FASTA with tens of millions of sequences). the FASTA file is scanned
        ''' in two passes and every intermediate product is written to <paramref name="workDir"/> so
        ''' that memory stays bounded by the configured block size rather than the database size.
        ''' a failed run can be resumed (skipping already-produced artifacts) when
        ''' <see cref="StreamingClustering.resumeIfExists"/> is left at its default of true.
        ''' </summary>
        ''' <param name="fastaHandle">path to a FASTA file or a directory of FASTA files</param>
        ''' <param name="workDir">directory that will hold the on-disk intermediate products</param>
        Public Function RunStreaming(fastaHandle As String, workDir As String) As ClusteringResult
            Dim engine As New StreamingClustering(workDir) With {
                .k = k,
                .topN = topN,
                .svdDims = svdDims,
                .knnK = knnK,
                .kmerSortMode = kmerSortMode
            }
            Return engine.RunStreaming(fastaHandle)
        End Function

        ''' <summary>
        ''' build the TF-IDF dataframe limited to the selected vocabulary, with L2 normalization
        ''' </summary>
        Private Function BuildTfidf(vocab As KmerVocabulary, sequences As IEnumerable(Of FastaSeq), words As String()) As DataFrame
            Dim model As New TFIDF

            For Each seq As FastaSeq In sequences.SafeQuery
                If vocab.docCounts.ContainsKey(seq.Title) Then
                    model.Add(seq.Title, vocab.docCounts(seq.Title))
                Else
                    ' sequence with no selected kmer -> empty counter
                    model.Add(seq.Title, New Dictionary(Of String, Integer))
                End If
            Next

            model.SetWords(words)
            Return model.TfidfVectorizer(L2normalized:=True)
        End Function

        ''' <summary>
        ''' convert the TF-IDF dataframe into a SparseMatrix (COO format) for the randomized SVD
        ''' </summary>
        Private Function ToSparseMatrix(tfidf As DataFrame, nSeq As Integer, nWords As Integer) As SparseMatrix
            Dim rows As New List(Of Integer)
            Dim cols As New List(Of Integer)
            Dim vals As New List(Of Double)
            ' features: column name (kmer word) -> FeatureVector whose .vector holds the per-sequence values
            Dim colIndex = tfidf.features _
                .Keys _
                .Select(Function(name, i) (CStr(name), i)) _
                .ToDictionary(Function(t) t.Item1, Function(t) t.Item2)
            Dim columns = tfidf.features _
                .Select(Function(kv) (word:=CStr(kv.Key), vec:=DirectCast(kv.Value.vector, Double()))) _
                .ToArray

            For Each col In columns
                Dim j = colIndex(col.word)

                For i As Integer = 0 To nSeq - 1
                    Dim v = col.vec(i)

                    If v <> 0.0 Then
                        rows.Add(i)
                        cols.Add(j)
                        vals.Add(v)
                    End If
                Next
            Next

            Return New SparseMatrix(rows.ToArray, cols.ToArray, vals.ToArray, nSeq, nWords)
        End Function

        ''' <summary>
        ''' turn the KNN neighbour lists into a symmetric, deduplicated, undirected edge list
        ''' </summary>
        Private Function BuildEdgeList(neighbors As KNeighbors(), nSeq As Integer, cutoff As Double) As (u As Integer, v As Integer, weight As Double)()
            Dim edgeSet As New Dictionary(Of (Integer, Integer), Double)

            For i As Integer = 0 To neighbors.Length - 1
                Dim nb = neighbors(i)

                If nb.size = 0 OrElse nb.indices Is Nothing Then
                    Continue For
                End If

                For t As Integer = 0 To nb.size - 1
                    Dim j = nb.indices(t)
                    Dim w = nb.weights(t)

                    If j < 0 OrElse j >= nSeq OrElse j = i Then
                        Continue For
                    End If

                    If w < cutoff Then
                        Continue For
                    End If

                    Dim a = Math.Min(i, j)
                    Dim b = Math.Max(i, j)
                    Dim key = (a, b)

                    If Not edgeSet.ContainsKey(key) OrElse edgeSet(key) < w Then
                        edgeSet(key) = w
                    End If
                Next
            Next

            Return edgeSet _
                .Select(Function(kv) (kv.Key.Item1, kv.Key.Item2, kv.Value)) _
                .ToArray
        End Function

        ''' <summary>
        ''' load the edge list into a NetworkGraph and run Louvain community detection
        ''' </summary>
        Private Function LouvainPartition(edges As (u As Integer, v As Integer, weight As Double)(), nSeq As Integer) As Integer()
            ' nodes are created in sequence order so their 1-based ID matches the sequence index + 1
            Dim nodes As Node() = (From i As Integer In Enumerable.Range(0, nSeq)
                                   Select New Node With {.label = "v" & i.ToString()}).ToArray
            Dim netEdges As Edge(Of Node)() = edges _
                .Select(Function(e) New Edge(Of Node) With {
                    .U = nodes(e.u),
                    .V = nodes(e.v),
                    .weight = e.weight
                }) _
                .ToArray
            Dim graph As New NetworkGraph(Of Node, Edge(Of Node))(nodes, netEdges)
            Dim louvain = Builder.Load(graph)
            Dim community = louvain.SolveClusters().GetCommunity()

            ' GetCommunity() is aligned to the node insertion order (1-based id -> index)
            Dim assignments As Integer() = New Integer(nSeq - 1) {}

            For i As Integer = 0 To nSeq - 1
                assignments(i) = If(community.Length > i, CInt(Val(community(i))), 0)
            Next

            Return assignments
        End Function

        ''' <summary>
        ''' group sequences by family, run MSA per family and pick the lowest-edit reference sequence
        ''' </summary>
        Private Function BuildFamilies(assignments As Integer(), rownames As String(), originals As FastaSeq()) As ProteinFamily()
            Dim byTitle = originals _
                .GroupBy(Function(fa) fa.Title) _
                .ToDictionary(Function(g) g.Key, Function(g) g.First)
            Dim groups = assignments _
                .Select(Function(a, i) (a, rownames(i))) _
                .GroupBy(Function(t) t.a) _
                .ToArray

            Return groups _
                .Select(Function(g)
                            Dim members = g.Select(Function(t) t.Item2).ToArray
                            Dim seqs = members _
                                .Where(Function(m) byTitle.ContainsKey(m)) _
                                .Select(Function(m) byTitle(m)) _
                                .ToArray
                            Dim family = New ProteinFamily With {
                                .familyId = g.Key,
                                .members = members,
                                .memberSequences = seqs
                            }

                            If seqs.Length = 1 Then
                                family.reference = seqs(0)
                            ElseIf seqs.Length > 1 Then
                                Dim msa = New CenterStar(seqs).Compute(ScoreMatrix.DefaultMatrix)
                                family.msa = msa

                                ' pick the sequence with the fewest edits
                                Dim best As Integer = 0
                                For i As Integer = 1 To msa.edits.Length - 1
                                    If msa.edits(i) < msa.edits(best) Then
                                        best = i
                                    End If
                                Next

                                Dim refTitle = msa.names(best)
                                family.reference = If(byTitle.ContainsKey(refTitle), byTitle(refTitle), seqs(best))
                            End If

                            Return family
                        End Function) _
                .ToArray
        End Function
    End Class
End Namespace
