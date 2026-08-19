Imports System.Runtime.CompilerServices

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace ProteinStructure

    ''' <summary>
    ''' Extracts kmer features from a protein sequence database and selects a top-N
    ''' vocabulary used as the fixed dictionary for the TF-IDF vectorization step.
    ''' </summary>
    ''' <remarks>
    ''' For each distinct kmer two statistics are collected during a single pass over
    ''' the sequence database:
    ''' <list type="bullet">
    ''' <item><description>the per-document occurrence count (how many times the kmer appears inside a single protein sequence);</description></item>
    ''' <item><description>the global occurrence count (the total number of times the kmer appears across the whole database).</description></item>
    ''' </list>
    ''' The kmers are then ranked (by default in ascending order of the chosen metric)
    ''' and the top-N kmers are kept as the fixed vocabulary.
    ''' </remarks>
    Public Class KmerVocabulary

        ''' <summary>
        ''' the fixed kmer dictionary ordered as it will be used as the TF-IDF feature columns
        ''' </summary>
        Public ReadOnly Property words As String()

        ''' <summary>
        ''' kmer -> column index inside the <see cref="words"/> dictionary
        ''' </summary>
        Public ReadOnly Property index As Dictionary(Of String, Integer)

        ''' <summary>
        ''' global total occurrence count for each selected kmer (aligned with <see cref="words"/>)
        ''' </summary>
        Public ReadOnly Property globalCounts As Long()

        ''' <summary>
        ''' per-document occurrence counts: sequence id -> (kmer -> count) for the selected vocabulary only
        ''' </summary>
        Public ReadOnly Property docCounts As Dictionary(Of String, Dictionary(Of String, Integer))

        ''' <summary>
        ''' selection ranking mode</summary>
        Public ReadOnly Property rankingMode As SortMode

        ''' <summary>
        ''' how kmers are ranked before the top-N slice is taken
        ''' </summary>
        Public Enum SortMode As Integer
            ''' <summary>
            ''' ascending by global count then by per-document count (the literal ordering the pipeline
            ''' describes; keeps the rarest features)
            ''' </summary>
            Ascending
            ''' <summary>
            ''' descending by global count then by per-document count (keeps the most frequent features)
            ''' </summary>
            Descending
        End Enum

        Private Sub New(words As String(), globalCounts As Long(), docCounts As Dictionary(Of String, Dictionary(Of String, Integer)), sortMode As SortMode)
            Me.words = words
            Me.globalCounts = globalCounts
            Me.docCounts = docCounts
            Me.rankingMode = sortMode
            Me.index = words _
                .Select(Function(w, i) (w, i)) _
                .ToDictionary(Function(t) t.w, Function(t) t.i)
        End Sub

        ''' <summary>
        ''' build the kmer vocabulary from a stream of protein sequences
        ''' </summary>
        ''' <param name="sequences">the protein sequence database (title, sequence data)</param>
        ''' <param name="k">kmer length in residues (default 5)</param>
        ''' <param name="topN">number of kmer features to keep (default 10000)</param>
        ''' <param name="mode">ranking mode used before slicing the top-N</param>
        Public Shared Function Build(sequences As IEnumerable(Of (title As String, sequence As String)),
                                     Optional k As Integer = 5,
                                     Optional topN As Integer = 10000,
                                     Optional mode As SortMode = SortMode.Ascending) As KmerVocabulary

            Dim globalCount As New Dictionary(Of String, Long)
            Dim docCounts As New Dictionary(Of String, Dictionary(Of String, Integer))
            Dim inDocMax As New Dictionary(Of String, Integer)

            For Each seq In sequences.SafeQuery
                Dim counts As New Dictionary(Of String, Integer)

                For Each km In SMRUCC.genomics.SequenceModel.Slicer.KSeq.KmerSpans(seq.sequence, k)
                    ' per-document count
                    If counts.ContainsKey(km) Then
                        counts(km) += 1
                    Else
                        counts(km) = 1
                    End If

                    ' global count (total occurrences across the whole database)
                    If globalCount.ContainsKey(km) Then
                        globalCount(km) += 1
                    Else
                        globalCount(km) = 1
                    End If
                Next

                docCounts(seq.title) = counts

                For Each kv In counts
                    ' track the maximum per-document count for the secondary ranking key
                    If Not inDocMax.ContainsKey(kv.Key) OrElse kv.Value > inDocMax(kv.Key) Then
                        inDocMax(kv.Key) = kv.Value
                    End If
                Next
            Next

            Dim ordered = globalCount.Keys _
                .OrderBy(Function(w) globalCount(w)) _
                .ThenBy(Function(w) inDocMax(w)) _
                .ToArray

            If mode = SortMode.Descending Then
                Array.Reverse(ordered)
            End If

            Dim selected = ordered.Take(topN).ToArray

            Dim selGlobal As Long() = selected _
                .Select(Function(w) globalCount(w)) _
                .ToArray

            ' restrict the per-document counts to the selected vocabulary to keep memory bounded
            Dim selDocCounts As New Dictionary(Of String, Dictionary(Of String, Integer))

            For Each kv In docCounts
                Dim filtered = kv.Value _
                    .Where(Function(c) globalCount.ContainsKey(c.Key) AndAlso Array.IndexOf(selected, c.Key) >= 0) _
                    .ToDictionary(Function(c) c.Key, Function(c) c.Value)
                selDocCounts(kv.Key) = filtered
            Next

            Call VBDebugger.EchoLine($" [kmer_vocab] selected {selected.Length} kmers (k={k}, topN={topN}, mode={mode}) from {globalCount.Count} distinct kmers")

            Return New KmerVocabulary(selected, selGlobal, selDocCounts, mode)
        End Function

        ''' <summary>
        ''' number of features in the vocabulary
        ''' </summary>
        Public ReadOnly Property size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return words.Length
            End Get
        End Property
    End Class
End Namespace
