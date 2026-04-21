Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace SequenceModel.Slicer

    ''' <summary>
    ''' A wrapper of the biological sequence region cutter, which can be used to cut a specific sequence region 
    ''' from a FASTA sequence or a chunked FASTA sequence
    ''' </summary>
    Public MustInherit Class ISlicer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="start">1-based start index</param>
        ''' <param name="seqLength">string length of the target locus region</param>
        ''' <returns></returns>
        Public MustOverride Function SliceRegionSite(start As Integer, seqLength As Integer) As String

    End Class

    Public Class FastaSlicer : Inherits ISlicer

        Public ReadOnly Property title As String

        ReadOnly sequenceData As String

        Sub New(fa As FastaSeq)
            sequenceData = fa.SequenceData
            title = fa.Title
        End Sub

        Public Overrides Function SliceRegionSite(start As Integer, seqLength As Integer) As String
            Return sequenceData.Substring(start - 1, seqLength)
        End Function
    End Class

    Public Class ChunkSlicer : Inherits ISlicer

        ReadOnly chromosome As ChunkedNtFasta

        Public ReadOnly Property title As String
            Get
                Return chromosome.title
            End Get
        End Property

        Sub New(chromosome As ChunkedNtFasta)
            Me.chromosome = chromosome
        End Sub

        Public Overrides Function SliceRegionSite(start As Integer, seqLength As Integer) As String
            Return chromosome.GetRegion(start, right:=start + seqLength)
        End Function
    End Class
End Namespace