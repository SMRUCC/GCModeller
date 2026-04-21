Imports SMRUCC.genomics.Assembly.NCBI.GenBank
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

    ''' <summary>
    ''' slicer for the ncbi genbank origin sequence, which is stored in the GBFF file, 
    ''' this slicer can be used to cut a specific sequence region from a genbank sequence file
    ''' </summary>
    Public Class GenBankSlicer : Inherits ISlicer

        Public ReadOnly Property title As String

        ReadOnly chromosome As String

        Sub New(chromosome As GBFF.File)
            Me.chromosome = chromosome.Origin.SequenceData
            Me.title = chromosome.Accession.AccessionId
        End Sub

        Public Overrides Function SliceRegionSite(start As Integer, seqLength As Integer) As String
            Return chromosome.Substring(start - 1, seqLength)
        End Function
    End Class
End Namespace