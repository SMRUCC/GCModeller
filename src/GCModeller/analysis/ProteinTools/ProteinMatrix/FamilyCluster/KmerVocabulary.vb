#Region "Microsoft.VisualBasic::6aede5aa2ea5382dbaa70e97b000d0ab, analysis\ProteinTools\ProteinMatrix\FamilyCluster\KmerVocabulary.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 299
    '    Code Lines: 172 (57.53%)
    ' Comment Lines: 82 (27.42%)
    '    - Xml Docs: 91.46%
    ' 
    '   Blank Lines: 45 (15.05%)
    '     File Size: 13.55 KB


    '     Class KmerVocabulary
    ' 
    '         Properties: docCounts, globalCounts, index, k, rankingMode
    '                     words
    '         Enum SortMode
    ' 
    '             Ascending, Descending
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: size
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: Build, BuildVocabularyOnly, Load, SelectVocabulary, Vectorize
    ' 
    '     Sub: Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace FamilyCluster

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
        ''' kmer length in residues (used when vectorizing a single sequence)
        ''' </summary>
        Public ReadOnly Property k As Integer

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

        ''' <summary>
        ''' number of features in the vocabulary
        ''' </summary>
        Public ReadOnly Property size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return words.Length
            End Get
        End Property

        Private Sub New(words As String(), globalCounts As Long(), docCounts As Dictionary(Of String, Dictionary(Of String, Integer)), sortMode As SortMode, kvalue As Integer)
            Me.words = words
            Me.globalCounts = globalCounts
            Me.docCounts = docCounts
            Me.rankingMode = sortMode
            Me.k = kvalue
            Me.index = words _
                .Select(Function(w, i) (w, i)) _
                .ToDictionary(Function(t) t.w,
                              Function(t)
                                  Return t.i
                              End Function)
        End Sub

        ''' <summary>
        ''' count the kmer occurrences over the whole database in a single streaming pass,
        ''' keeping only the global total count and the per-document maximum (the two ranking
        ''' keys). per-document counts are NOT retained so the memory footprint stays bounded by
        ''' the vocabulary size rather than the number of sequences - this is what makes the
        ''' streaming pipeline able to handle databases that do not fit in memory.
        ''' </summary>
        Public Shared Function BuildVocabularyOnly(sequences As IEnumerable(Of FastaSeq),
                                                   Optional k As Integer = 5,
                                                   Optional topN As Integer = 10000,
                                                   Optional mode As SortMode = SortMode.Ascending) As KmerVocabulary

            Dim globalCount As New Dictionary(Of String, Long)
            Dim inDocMax As New Dictionary(Of String, Integer)

            For Each seq As FastaSeq In sequences.SafeQuery
                ' de-duplicated per-document counts (we only need the max inside one doc)
                Dim seenInDoc As New HashSet(Of String)

                For Each km As String In KSeq.KmerSpans(seq.SequenceData, k)
                    If globalCount.ContainsKey(km) Then
                        globalCount(km) += 1
                    Else
                        globalCount(km) = 1
                    End If

                    If Not seenInDoc.Contains(km) Then
                        seenInDoc.Add(km)

                        If Not inDocMax.ContainsKey(km) OrElse 1 > inDocMax(km) Then
                            inDocMax(km) = 1
                        End If
                    End If
                Next
            Next

            Return SelectVocabulary(globalCount, inDocMax, Nothing, topN, mode, k)
        End Function

        ''' <summary>
        ''' shared top-N vocabulary selection used by both the in-memory and the streaming build paths
        ''' </summary>
        Private Shared Function SelectVocabulary(globalCount As Dictionary(Of String, Long),
                                                 inDocMax As Dictionary(Of String, Integer),
                                                 docCounts As Dictionary(Of String, Dictionary(Of String, Integer)),
                                                 topN As Integer,
                                                 mode As SortMode,
                                                 kvalue As Integer) As KmerVocabulary
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

            Dim selDocCounts As Dictionary(Of String, Dictionary(Of String, Integer)) = Nothing

            If docCounts IsNot Nothing Then
                ' restrict the per-document counts to the selected vocabulary to keep memory bounded
                Dim selSet = New HashSet(Of String)(selected)

                selDocCounts = New Dictionary(Of String, Dictionary(Of String, Integer))

                For Each kv In docCounts
                    Dim filtered = kv.Value _
                        .Where(Function(c) selSet.Contains(c.Key)) _
                        .ToDictionary(Function(c) c.Key, Function(c) c.Value)
                    selDocCounts(kv.Key) = filtered
                Next
            End If

            Call VBDebugger.EchoLine($" [kmer_vocab] selected {selected.Length} kmers from {globalCount.Count} distinct kmers (topN={topN}, mode={mode})")

            Return New KmerVocabulary(selected, selGlobal, selDocCounts, mode, kvalue)
        End Function

        ''' <summary>
        ''' build the kmer vocabulary from a stream of protein sequences
        ''' </summary>
        ''' <param name="sequences">the protein sequence database (title, sequence data)</param>
        ''' <param name="k">kmer length in residues (default 5)</param>
        ''' <param name="topN">number of kmer features to keep (default 10000)</param>
        ''' <param name="mode">ranking mode used before slicing the top-N</param>
        Public Shared Function Build(sequences As IEnumerable(Of FastaSeq),
                                     Optional k As Integer = 5,
                                     Optional topN As Integer = 10000,
                                     Optional mode As SortMode = SortMode.Ascending) As KmerVocabulary

            Dim globalCount As New Dictionary(Of String, Long)
            Dim docCounts As New Dictionary(Of String, Dictionary(Of String, Integer))
            Dim inDocMax As New Dictionary(Of String, Integer)

            For Each seq In sequences.SafeQuery
                Dim counts As New Dictionary(Of String, Integer)

                For Each km As String In KSeq.KmerSpans(seq.SequenceData, k)
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

                docCounts(seq.Title) = counts

                For Each kv In counts
                    ' track the maximum per-document count for the secondary ranking key
                    If Not inDocMax.ContainsKey(kv.Key) OrElse kv.Value > inDocMax(kv.Key) Then
                        inDocMax(kv.Key) = kv.Value
                    End If
                Next
            Next

            Return SelectVocabulary(globalCount, inDocMax, docCounts, topN, mode, k)
        End Function

        ''' <summary>
        ''' compute the L2-normalized TF-IDF sparse vector of a single protein sequence over the
        ''' fixed vocabulary, returning the non-zero (columnIndex, value) pairs. this is the
        ''' streaming building block that lets the second pass emit one COO row per sequence
        ''' without holding the whole database in memory.
        ''' </summary>
        Public Function Vectorize(sequence As String) As (col As Integer, value As Double)()
            Dim tf As New Dictionary(Of String, Integer)

            For Each km As String In KSeq.KmerSpans(sequence, k)
                If index.ContainsKey(km) Then
                    If tf.ContainsKey(km) Then
                        tf(km) += 1
                    Else
                        tf(km) = 1
                    End If
                End If
            Next

            If tf.Count = 0 Then
                Return New(col As Integer, value As Double)() {}
            End If

            ' idf uses the global document frequency captured during vocabulary building
            Dim nDocs As Double = globalCounts.Sum(Function(c) CDbl(c))
            Dim vec As New List(Of (col As Integer, value As Double))

            For Each kv In tf
                Dim col = index(kv.Key)
                Dim idf = Math.Log((1.0 + nDocs) / (1.0 + globalCounts(col)))
                Dim tfidf = kv.Value * idf
                vec.Add((col, tfidf))
            Next

            ' L2 normalization
            Dim norm As Double = Math.Sqrt(vec.Sum(Function(p) p.value * p.value))

            If norm > 0.0 Then
                vec = vec _
                    .Select(Function(p) (p.col, p.value / norm)) _
                    .ToList
            End If

            Return vec.ToArray
        End Function

        ''' <summary>
        ''' persist the vocabulary (words + global counts + ranking mode) so a streaming run can be
        ''' resumed without re-counting kmers. per-document counts are not stored because the streaming
        ''' path never keeps them in memory.
        ''' </summary>
        Public Sub Save(path As String)
            Dim blob = New Dictionary(Of String, String) From {
                {"words", words.JoinBy(",")},
                {"globalCounts", globalCounts.JoinBy(",")},
                {"rankingMode", rankingMode.ToString},
                {"k", k.ToString}
            }
            Call File.WriteAllText(path, blob.GetJson)
        End Sub

        ''' <summary>
        ''' load a vocabulary previously written by <see cref="Save"/>
        ''' </summary>
        Public Shared Function Load(path As String) As KmerVocabulary
            Dim blob = CType(File.ReadAllText(path).LoadObject(GetType(Dictionary(Of String, String))), Dictionary(Of String, String))
            Dim words = blob("words").Split(","c)
            Dim globalCounts = blob("globalCounts").Split(",").AsLong
            Dim mode = CType([Enum].Parse(GetType(SortMode), blob("rankingMode")), SortMode)
            Dim kvalue = CInt(Val(blob("k")))
            Return New KmerVocabulary(words, globalCounts, Nothing, mode, kvalue)
        End Function
    End Class
End Namespace

