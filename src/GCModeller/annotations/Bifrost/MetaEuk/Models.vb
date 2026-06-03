' ========================================================================
' GLOBAL CONFIGURATION
' ========================================================================

Imports SMRUCC.genomics.ComponentModel.Loci

''' <summary>Algorithm parameters controllable via command-line</summary>
Public Class MetaEukConfig
    ' --- Input/Output ---
    Public ContigsFile As String = ""
    Public ReferenceFile As String = ""
    Public OutputPrefix As String = "metaeuk_out"

    ' --- Fragment Generation ---
    Public MinFragmentLength As Integer = 15        ' minimum amino acids per candidate fragment
    Public MaxFragmentLength As Integer = 5000      ' maximum amino acids per candidate fragment

    ' --- Homology Search ---
    Public EvalueThreshold As Double = 0.001         ' E-value cutoff for significant hits
    Public MinIdentity As Double = 0.2              ' minimum sequence identity fraction
    Public AlignmentBandWidth As Integer = 32       ' band width for Smith-Waterman

    ' --- Dynamic Programming ---
    Public GapPenaltyLambda As Double = 0.5         ' gap penalty coefficient per AA of intron
    Public MaxIntronLength As Integer = 50000       ' maximum intron length in bp
    Public MinExonScore As Double = 20.0            ' minimum bitscore for an exon to be considered

    ' --- Redundancy Removal ---
    Public MinExonOverlapFraction As Double = 0.3   ' fraction overlap to consider exons shared

    ' --- Conflict Resolution ---
    Public OverlapBpThreshold As Integer = 10       ' bp overlap to trigger conflict resolution

    ' --- Performance ---
    Public Verbose As Boolean = False
    Public NumThreads As Integer = 1
End Class

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
