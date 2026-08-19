Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace ProteinStructure

    ''' <summary>
    ''' full result of the unsupervised protein family clustering pipeline.
    ''' besides the final family assignment and reference sequences, every intermediate
    ''' artifact (TF-IDF matrix, SVD embedding, KNN edge list) is preserved so that the
    ''' run can be inspected and debugged.
    ''' </summary>
    Public Class ClusteringResult

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
        ''' serialize the result to a plain JSON object (the TF-IDF / SVD matrices are summarized, not dumped in full)
        ''' </summary>
        Public Overrides Function ToString() As String
            Return New Dictionary(Of String, Object) From {
                {"sequenceCount", sequenceNames.Length},
                {"familyCount", familyCount},
                {"k", k},
                {"topN", topN},
                {"svdDims", svdDims},
                {"knnEdgeCount", If(knnEdges Is Nothing, 0, knnEdges.Length)},
                {"vocabularySize", If(vocabulary Is Nothing, 0, vocabulary.Length)}
            }.GetJson
        End Function
    End Class
End Namespace
