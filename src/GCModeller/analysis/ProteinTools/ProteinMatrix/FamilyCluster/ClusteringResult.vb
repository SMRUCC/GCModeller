#Region "Microsoft.VisualBasic::521e1fa6e04e0f954c0c894116d6cfc6, analysis\ProteinTools\ProteinMatrix\FamilyCluster\ClusteringResult.vb"

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

    '   Total Lines: 189
    '    Code Lines: 95 (50.26%)
    ' Comment Lines: 69 (36.51%)
    '    - Xml Docs: 98.55%
    ' 
    '   Blank Lines: 25 (13.23%)
    '     File Size: 8.04 KB


    '     Class ClusteringResult
    ' 
    '         Properties: families, familyAssignments, familyCount, k, knnEdges
    '                     referenceSequences, sequenceNames, svdDims, svdVectors, tfidfMatrix
    '                     topN, vocabulary, workDir
    ' 
    '         Function: FromDirectory, StreamKnnEdges, StreamKnnEdgesCount, StreamSvd, StreamTfidf
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace FamilyCluster

    ''' <summary>
    ''' full result of the unsupervised protein family clustering pipeline.
    ''' besides the final family assignment and reference sequences, every intermediate
    ''' artifact (TF-IDF matrix, SVD embedding, KNN edge list) is preserved so that the
    ''' run can be inspected and debugged.
    '''
    ''' for the streaming pipeline the large intermediate products (TF-IDF / SVD / KNN) live on
    ''' disk under <see cref="workDir"/>; the in-memory fields hold the essentials (names,
    ''' assignments, families, references, vocabulary) while the big matrices can be streamed back
    ''' on demand through the reader helpers, so the result object itself never forces the whole
    ''' database into memory.
    ''' </summary>
    Public Class ClusteringResult

        ''' <summary>
        ''' working directory of the streaming run; holds the on-disk intermediate products
        ''' (tfidf_vectors.coo, svd_vectors.tsv, knn_edges.tsv, family_assignment.tsv, ...).
        ''' </summary>
        Public Property workDir As String

        ''' <summary>
        ''' titles of the protein sequences, aligned across every step (TF-IDF rows, SVD rows, KNN indices, family assignments)
        ''' </summary>
        Public Property sequenceNames As String()

        ''' <summary>
        ''' family id assigned to each sequence (aligned with <see cref="sequenceNames"/>)
        ''' </summary>
        Public Property familyAssignments As Integer()

        ''' <summary>
        ''' reference sequence per family id
        ''' </summary>
        Public Property referenceSequences As Dictionary(Of Integer, FastaSeq)

        ''' <summary>
        ''' the discovered protein families
        ''' </summary>
        Public Property families As ProteinFamily()

        ''' <summary>
        ''' the TF-IDF matrix (row = sequence, column = kmer vocabulary word)
        ''' </summary>
        Public Property tfidfMatrix As DataFrame

        ''' <summary>
        ''' the selected kmer vocabulary (column names of the TF-IDF matrix)
        ''' </summary>
        Public Property vocabulary As String()

        ''' <summary>
        ''' the TruncatedSVD embedding: one row per sequence, <see cref="svdDims"/> columns
        ''' </summary>
        Public Property svdVectors As Double()()

        ''' <summary>
        ''' the number of SVD dimensions
        ''' </summary>
        Public Property svdDims As Integer

        ''' <summary>
        ''' undirected KNN edges: (u, v, weight) with u &lt; v and indices aligned to <see cref="sequenceNames"/>
        ''' </summary>
        Public Property knnEdges As (u As Integer, v As Integer, weight As Double)()

        ''' <summary>
        ''' the kmer length used for feature extraction
        ''' </summary>
        Public Property k As Integer

        ''' <summary>
        ''' number of kmer features kept
        ''' </summary>
        Public Property topN As Integer

        ''' <summary>
        ''' count of distinct protein families
        ''' </summary>
        Public ReadOnly Property familyCount As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If familyAssignments Is Nothing Then
                    Return 0
                Else
                    Return familyAssignments.Distinct.Count
                End If
            End Get
        End Property

        ''' <summary>
        ''' re-assemble a <see cref="ClusteringResult"/> from a completed streaming run. the essentials
        ''' (names, assignments, families, references, vocabulary) are loaded into memory; the very large
        ''' TF-IDF / SVD / KNN arrays stay on disk and can be streamed back via the reader helpers.
        ''' </summary>
        Public Shared Function FromDirectory(workDir As String,
                                             fastaHandle As String,
                                             vocab As KmerVocabulary,
                                             familyInfo As Dictionary(Of Integer, ProteinFamily)) As ClusteringResult
            Dim vectorDir = System.IO.Path.Combine(workDir, "tfidf")
            Dim titles = SparseVectorWriter.LoadTitleIndex(vectorDir)
            Dim assignment = BlockLouvain.ReadAssignment(workDir).ToArray
            Dim familyAssignments(assignment.Length - 1) As Integer

            For i As Integer = 0 To assignment.Length - 1
                familyAssignments(i) = assignment(i).family
            Next

            Dim families = familyInfo.Values _
                .OrderBy(Function(f) f.familyId) _
                .ToArray
            Dim references = families _
                .Where(Function(f) f.reference IsNot Nothing) _
                .ToDictionary(Function(f) f.familyId, Function(f) f.reference)

            Dim meta = SvdBlockReducer.LoadMeta(workDir)

            Return New ClusteringResult With {
                .workDir = workDir,
                .sequenceNames = titles,
                .familyAssignments = familyAssignments,
                .referenceSequences = references,
                .families = families,
                .vocabulary = vocab.words,
                .svdDims = meta.dims,
                .k = 0,
                .topN = vocab.size
            }
        End Function

        ''' <summary>
        ''' stream the TF-IDF sparse vectors (rowIndex, title, columnIndices, values) back from disk
        ''' </summary>
        Public Iterator Function StreamTfidf() As IEnumerable(Of (rowIndex As Integer, title As String, cols As Integer(), vals As Double()))
            Dim vectorDir = System.IO.Path.Combine(workDir, "tfidf")
            For Each row In New SparseVectorWriter(vectorDir).ReadRows
                Yield row
            Next
        End Function

        ''' <summary>
        ''' stream the dense SVD embeddings (rowIndex, embedding) back from disk
        ''' </summary>
        Public Iterator Function StreamSvd() As IEnumerable(Of (rowIndex As Integer, vector As Double()))
            For Each e In SvdBlockReducer.ReadEmbeddings(workDir)
                Yield e
            Next
        End Function

        ''' <summary>
        ''' stream the KNN edges (u, v, weight) back from disk
        ''' </summary>
        Public Iterator Function StreamKnnEdges() As IEnumerable(Of (u As Integer, v As Integer, weight As Double))
            For Each e In ApproxKnnBuilder.ReadEdges(workDir)
                Yield e
            Next
        End Function

        ''' <summary>
        ''' serialize the result to a plain JSON object (the TF-IDF / SVD matrices are summarized, not dumped in full)
        ''' </summary>
        Public Overrides Function ToString() As String
            Return New Dictionary(Of String, Object) From {
                {"sequenceCount", If(sequenceNames Is Nothing, 0, sequenceNames.Length)},
                {"familyCount", familyCount},
                {"k", k},
                {"topN", topN},
                {"svdDims", svdDims},
                {"knnEdgeCount", If(knnEdges Is Nothing, 0, StreamKnnEdgesCount())},
                {"vocabularySize", If(vocabulary Is Nothing, 0, vocabulary.Length)},
                {"workDir", workDir}
            }.GetJson
        End Function

        Private Function StreamKnnEdgesCount() As Integer
            If String.IsNullOrEmpty(workDir) OrElse Not File.Exists(System.IO.Path.Combine(workDir, ApproxKnnBuilder.KNN_META_FILE)) Then
                Return 0
            End If
            Return ApproxKnnBuilder.LoadMeta(workDir).edges
        End Function
    End Class
End Namespace

