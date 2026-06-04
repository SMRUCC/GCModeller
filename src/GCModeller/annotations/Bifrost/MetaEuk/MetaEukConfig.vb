''' <summary>Algorithm parameters controllable via command-line</summary>
Public Class MetaEukConfig

    ' --- Input/Output ---
    Public Property ContigsFile As String = ""
    Public Property ReferenceFile As String = ""
    Public Property OutputPrefix As String = "metaeuk_out"

    ' --- Fragment Generation ---
    Public Property MinFragmentLength As Integer = 15        ' minimum amino acids per candidate fragment
    Public Property MaxFragmentLength As Integer = 5000      ' maximum amino acids per candidate fragment

    ' --- Homology Search ---
    Public Property EvalueThreshold As Double = 0.001         ' E-value cutoff for significant hits
    Public Property MinIdentity As Double = 0.2              ' minimum sequence identity fraction
    Public Property AlignmentBandWidth As Integer = 32       ' band width for Smith-Waterman

    ' --- Dynamic Programming ---
    Public Property GapPenaltyLambda As Double = 0.5         ' gap penalty coefficient per AA of intron
    Public Property MaxIntronLength As Integer = 50000       ' maximum intron length in bp
    Public Property MinExonScore As Double = 20.0            ' minimum bitscore for an exon to be considered

    ' --- Redundancy Removal ---
    Public Property MinExonOverlapFraction As Double = 0.3   ' fraction overlap to consider exons shared

    ' --- Conflict Resolution ---
    Public Property OverlapBpThreshold As Integer = 10       ' bp overlap to trigger conflict resolution

    ' --- Performance ---
    Public Property Verbose As Boolean = False
    Public Property NumThreads As Integer = 4

End Class