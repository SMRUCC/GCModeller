Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace SequenceModel.Slicer

    ''' <summary>
    ''' A wrapper of the biological sequence region cutter, which can be used to cut a specific sequence region 
    ''' from a FASTA sequence or a chunked FASTA sequence
    ''' </summary>
    Public MustInherit Class ISlicer

        Public Overridable ReadOnly Property title As String

        Protected Sub New(title As String)
            _title = title
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="start">1-based start index</param>
        ''' <param name="seqLength">string length of the target locus region</param>
        ''' <returns></returns>
        Public MustOverride Function SliceRegionSite(start As Integer, seqLength As Integer) As String

        Public Overrides Function ToString() As String
            Return title
        End Function

    End Class

    Public Class FastaSlicer : Inherits ISlicer

        ReadOnly sequenceData As String

        Sub New(fa As FastaSeq)
            Call MyBase.New(fa.Title)
            sequenceData = fa.SequenceData
        End Sub

        Public Overrides Function SliceRegionSite(start As Integer, seqLength As Integer) As String
            Return sequenceData.Substring(start - 1, seqLength)
        End Function
    End Class

    Public Class ChunkSlicer : Inherits ISlicer

        ReadOnly chromosome As ChunkedNtFasta

        Sub New(chromosome As ChunkedNtFasta)
            Call MyBase.New(chromosome.title)
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

        ReadOnly chromosome As String

        Sub New(chromosome As GBFF.File)
            Call MyBase.New(chromosome.Accession.AccessionId)
            Me.chromosome = chromosome.Origin.SequenceData
        End Sub

        Public Overrides Function SliceRegionSite(start As Integer, seqLength As Integer) As String
            Return chromosome.Substring(start - 1, seqLength)
        End Function
    End Class
End Namespace