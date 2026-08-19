Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace ProteinStructure

    ''' <summary>
    ''' streaming orchestrator for the unsupervised protein-family clustering pipeline.
    '''
    ''' the algorithm keeps the same semantics as <see cref="ProteinFamilyClustering"/> but never
    ''' holds the whole database in memory. it runs in two passes over the FASTA file plus a set of
    ''' on-disk intermediate products:
    '''
    ''' pass 1a : stream the FASTA once to count kmers and select the top-N vocabulary (memory ~ vocabulary size).
    ''' pass 1b : stream the FASTA a second time, compute each sequence's TF-IDF sparse vector over
    '''           the fixed vocabulary and append it to a COO file on disk.
    ''' pass 2a : stream the COO rows in blocks, reduce with randomized SVD, write m x dims embeddings to disk.
    ''' pass 2b : stream the embeddings in blocks, build an approximate KNN graph, write undirected edges to disk.
    ''' pass 2c : read the edges back, run Louvain, write the per-row family assignment to disk.
    ''' pass 2d : stream the original FASTA once, bucket each member sequence into a per-family FASTA file on disk,
    '''           then run CenterStar MSA per family (only one family resident at a time) and pick the least-edited reference.
    '''
    ''' every intermediate file lives under <see cref="workDir"/> and is reused when present so a failed run can be
    ''' resumed instead of restarted.
    ''' </summary>
    Public Class StreamingClustering

        Public Property workDir As String
        Public Property resumeIfExists As Boolean = True

        ' tuning knobs (kept configurable like the in-memory pipeline)
        Public Property k As Integer = 5
        Public Property topN As Integer = 10000
        Public Property svdDims As Integer = 9
        Public Property knnK As Integer = 30
        Public Property kmerSortMode As KmerVocabulary.SortMode = KmerVocabulary.SortMode.Ascending

        ' number of rows processed per SVD / KNN block; bounds the resident working set
        Public Property blockSize As Integer = 50000

        Private ReadOnly vocabularyDir As String
        Private ReadOnly vectorDir As String

        Public Sub New(workDir As String)
            Me.workDir = workDir
            Me.vocabularyDir = System.IO.Path.Combine(workDir, "vocabulary")
            Me.vectorDir = System.IO.Path.Combine(workDir, "tfidf")
        End Sub

        Public Function RunStreaming(fastaHandle As String) As ClusteringResult
            Call Directory.CreateDirectory(workDir)
            Call Directory.CreateDirectory(vocabularyDir)
            Call Directory.CreateDirectory(vectorDir)

            ' ---------- pass 1 : vocabulary + sparse vectors ----------
            Dim vocabFile = System.IO.Path.Combine(vocabularyDir, "vocab.txt")
            Dim vocab As KmerVocabulary

            If resumeIfExists AndAlso File.Exists(vocabFile) Then
                Call VBDebugger.EchoLine(" [stream] reuse existing vocabulary")
                vocab = KmerVocabulary.Load(vocabFile)
            Else
                vocab = Pass1BuildVocabulary(fastaHandle)
                Call vocab.Save(vocabFile)
            End If

            Dim cooMeta = System.IO.Path.Combine(vectorDir, SparseVectorWriter.META_FILE)

            If resumeIfExists AndAlso File.Exists(cooMeta) Then
                Call VBDebugger.EchoLine(" [stream] reuse existing TF-IDF vectors")
            Else
                Call Pass1WriteVectors(fastaHandle, vocab)
            End If

            ' ---------- pass 2 : reduce / cluster / msa ----------
            Dim meta = SparseVectorWriter.LoadMeta(vectorDir)
            Dim nRows = meta.rows
            Dim nCols = meta.cols

            ' 2a : SVD
            Dim svdMetaFile = System.IO.Path.Combine(workDir, SvdBlockReducer.SVD_META_FILE)
            If Not (resumeIfExists AndAlso File.Exists(svdMetaFile)) Then
                Call New SvdBlockReducer(workDir).Reduce(New SparseVectorWriter(vectorDir).ReadRows, svdDims, blockSize)
            End If

            ' 2b : KNN
            Dim knnMetaFile = System.IO.Path.Combine(workDir, ApproxKnnBuilder.KNN_META_FILE)
            If Not (resumeIfExists AndAlso File.Exists(knnMetaFile)) Then
                Call New ApproxKnnBuilder(workDir).Build(SvdBlockReducer.ReadEmbeddings(workDir), knnK, blockSize)
            End If

            ' 2c : Louvain
            Dim famMetaFile = System.IO.Path.Combine(workDir, BlockLouvain.ASSIGN_META_FILE)
            If Not (resumeIfExists AndAlso File.Exists(famMetaFile)) Then
                Call New BlockLouvain(workDir).Detect(ApproxKnnBuilder.ReadEdges(workDir), nRows)
            End If

            ' 2d : MSA per family + references
            Dim familiesDir = System.IO.Path.Combine(workDir, "families")
            Dim assignment = BlockLouvain.ReadAssignment(workDir).ToArray
            Call BucketSequencesByFamily(fastaHandle, assignment, familiesDir)
            Dim familyInfo = RunFamilyMsa(familiesDir, assignment)

            ' assemble the (file-backed) result
            Return ClusteringResult.FromDirectory(workDir, fastaHandle, vocab, familyInfo)
        End Function

        ' ---- pass 1a ----
        Private Function Pass1BuildVocabulary(fastaHandle As String, Optional tqdm_wrap As Boolean = True) As KmerVocabulary
            Call VBDebugger.EchoLine(" [stream] pass 1a : building kmer vocabulary (streaming)")

            Return KmerVocabulary.BuildVocabularyOnly(StreamIterator.SeqSource(
                handle:=fastaHandle,
                ext:=New String() {"*.fa", "*.fasta", "*.faa"},
                debug:=False,
                tqdm_wrap:=tqdm_wrap), k, topN, kmerSortMode
            )
        End Function

        ' ---- pass 1b ----
        Private Sub Pass1WriteVectors(fastaHandle As String, vocab As KmerVocabulary)
            Call VBDebugger.EchoLine(" [stream] pass 1b : writing TF-IDF sparse vectors (streaming)")

            Dim writer = New SparseVectorWriter(vectorDir, vocab.Size)
            Call writer.OpenForWrite()

            Dim bad As Integer = 0

            For Each fa In StreamIterator.SeqSource(fastaHandle, {"*.fa", "*.fasta", "*.faa"}, debug:=False)
                Try
                    Dim vec = vocab.Vectorize(fa.SequenceData)
                    Dim cols = vec.Select(Function(p) p.col).ToArray
                    Dim vals = vec.Select(Function(p) p.value).ToArray
                    Call writer.WriteRow(fa.Title, cols, vals)
                Catch ex As Exception
                    ' skip a single corrupt sequence instead of aborting the whole pass
                    bad += 1
                    Call VBDebugger.EchoLine($" [stream] skip bad sequence '{fa?.Title}' : {ex.Message}")
                End Try
            Next

            Call writer.CloseForWrite()

            If bad > 0 Then
                Call VBDebugger.EchoLine($" [stream] skipped {bad} sequences during vectorization")
            End If
        End Sub

        ' ---- pass 2d : bucket member sequences into per-family FASTA files ----
        Private Sub BucketSequencesByFamily(fastaHandle As String, assignment As (rowIndex As Integer, family As Integer)(), familiesDir As String)
            Call Directory.CreateDirectory(familiesDir)

            Dim titleToFamily = New Dictionary(Of String, Integer)
            Dim titles = SparseVectorWriter.LoadTitleIndex(vectorDir)
            For Each a In assignment
                titleToFamily(titles(a.rowIndex)) = a.family
            Next

            ' one append stream per family, opened lazily
            Dim famStreams As New Dictionary(Of Integer, System.IO.StreamWriter)

            For Each fa In StreamIterator.SeqSource(fastaHandle, {"*.fa", "*.fasta", "*.faa"}, debug:=False)
                If Not titleToFamily.ContainsKey(fa.Title) Then
                    Continue For
                End If

                Dim fam = titleToFamily(fa.Title)

                If Not famStreams.ContainsKey(fam) Then
                    Dim fpath = System.IO.Path.Combine(familiesDir, $"family_{fam}.fasta")
                    famStreams(fam) = New System.IO.StreamWriter(New BufferedStream(New FileStream(fpath, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20)), Encoding.ASCII)
                End If

                Call famStreams(fam).WriteLine(fa.ToString)
            Next

            For Each s In famStreams.Values
                s.Flush()
                s.Dispose()
            Next

            Call VBDebugger.EchoLine($" [stream] bucketed sequences into {famStreams.Count} family files")
        End Sub

        ' ---- pass 2d : run CenterStar MSA per family file, pick least-edited reference ----
        Private Function RunFamilyMsa(familiesDir As String, assignment As (rowIndex As Integer, family As Integer)()) As Dictionary(Of Integer, ProteinFamily)
            Call VBDebugger.EchoLine(" [stream] pass 2d : MSA per family (streaming)")

            Dim result As New Dictionary(Of Integer, ProteinFamily)
            Dim titles = SparseVectorWriter.LoadTitleIndex(vectorDir)
            Dim rowToTitle = titles.[Select](Function(t, i) (t, i)).ToDictionary(Function(x) x.i, Function(x) x.t)

            For Each ffile In Directory.EnumerateFiles(familiesDir, "family_*.fasta")
                Dim famId = CInt(Val(Path.GetFileNameWithoutExtension(ffile).Split("_"c)(1)))

                ' load only this family's sequences into memory (one family resident at a time)
                Dim famSeqs = LoadFamilySequences(ffile).ToArray

                If famSeqs.Length = 0 Then
                    Continue For
                End If

                Dim members = famSeqs.Select(Function(s) s.Title).ToArray
                Dim fam As New ProteinFamily With {
                    .familyId = famId,
                    .members = members,
                    .memberSequences = famSeqs
                }

                If famSeqs.Length = 1 Then
                    fam.reference = famSeqs(0)
                Else
                    Dim msa = New CenterStar(famSeqs).Compute(ScoreMatrix.DefaultMatrix)
                    fam.msa = msa

                    ' pick the sequence with the fewest edits
                    Dim best As Integer = 0
                    For i As Integer = 1 To msa.edits.Length - 1
                        If msa.edits(i) < msa.edits(best) Then
                            best = i
                        End If
                    Next

                    Dim refTitle = msa.names(best)
                    fam.reference = If(famSeqs.Any(Function(s) s.Title = refTitle), famSeqs.First(Function(s) s.Title = refTitle), famSeqs(best))
                End If

                result(famId) = fam

                Call VBDebugger.EchoLine($" [msa] family {famId} : {famSeqs.Length} seqs, reference = {fam.reference.Title}")
            Next

            Return result
        End Function

        Private Iterator Function LoadFamilySequences(ffile As String) As IEnumerable(Of FastaSeq)
            For Each fa In StreamIterator.SeqSource(ffile, {"*.fasta", "*.fa", "*.faa"}, debug:=False)
                Yield fa
            Next
        End Function
    End Class
End Namespace
