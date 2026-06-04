' ========================================================================
' GLOBAL CONFIGURATION
' ========================================================================

Imports SMRUCC.genomics.ComponentModel.Loci

''' <summary>A candidate coding fragment from six-frame translation</summary>
Public Class CandidateFragment
    Public ContigID As String
    Public Strand As Strands
    Public Frame As Integer             ' 0, 1, or 2 (reading frame offset)
    Public DnaStart As Integer          ' 1-based start on contig (forward strand coordinates)
    Public DnaEnd As Integer            ' 1-based end on contig (forward strand coordinates)
    Public Peptide As String            ' translated amino acid sequence
    Public FragmentIndex As Integer     ' unique index for this fragment

    Public ReadOnly Property Length() As Integer
        Get
            Return Peptide.Length
        End Get
    End Property
End Class

''' <summary>A homology hit: fragment aligned to a reference protein</summary>
Public Class HomologyHit
    Public Fragment As CandidateFragment
    Public TargetID As String           ' reference protein ID
    Public Score As Double              ' alignment bitscore
    Public Evalue As Double             ' E-value
    Public Identity As Double           ' fraction identity
    Public AlignStartQuery As Integer   ' alignment start on query (fragment peptide)
    Public AlignEndQuery As Integer     ' alignment end on query
    Public AlignStartTarget As Integer  ' alignment start on target (reference protein)
    Public AlignEndTarget As Integer    ' alignment end on target
    Public AlignedQuery As String       ' aligned query sequence (with gaps)
    Public AlignedTarget As String      ' aligned target sequence (with gaps)
End Class

''' <summary>A candidate exon derived from a homology hit</summary>
Public Class CandidateExon
    Public Hit As HomologyHit
    Public ContigID As String
    Public Strand As Strands
    Public DnaStart As Integer          ' 1-based
    Public DnaEnd As Integer            ' 1-based
    Public Score As Double
    Public Evalue As Double
    Public TargetID As String
    Public ExonIndex As Integer         ' index within TCS group

    Public ReadOnly Property Length() As Integer
        Get
            Return DnaEnd - DnaStart + 1
        End Get
    End Property
End Class

''' <summary>TCS group: Target-Contig-Strand combination</summary>
Public Class TCSGroup
    Public TargetID As String
    Public ContigID As String
    Public Strand As Strands
    Public Exons As New List(Of CandidateExon)
    Public OptimalChain As New List(Of CandidateExon)
    Public ChainScore As Double = Double.NegativeInfinity
    Public ChainEvalue As Double = Double.MaxValue

    Public ReadOnly Property Key() As String
        Get
            Return $"{TargetID}|{ContigID}|{CStr(Strand)}"
        End Get
    End Property
End Class

''' <summary>A predicted gene model (result of optimal exon chain)</summary>
Public Class GenePrediction
    Public GeneID As String
    Public ContigID As String
    Public Strand As Strands
    Public Exons As New List(Of CandidateExon)   ' sorted by DnaStart
    Public TargetID As String
    Public TotalScore As Double
    Public BestEvalue As Double
    Public ProteinSequence As String
    Public IsRepresentative As Boolean = True
    Public ClusterID As Integer = -1

    Public ReadOnly Property DnaStart() As Integer
        Get
            If Exons.Count = 0 Then Return 0
            Return Exons.Min(Function(e) e.DnaStart)
        End Get
    End Property

    Public ReadOnly Property DnaEnd() As Integer
        Get
            If Exons.Count = 0 Then Return 0
            Return Exons.Max(Function(e) e.DnaEnd)
        End Get
    End Property

    Public ReadOnly Property ExonCount() As Integer
        Get
            Return Exons.Count
        End Get
    End Property
End Class
