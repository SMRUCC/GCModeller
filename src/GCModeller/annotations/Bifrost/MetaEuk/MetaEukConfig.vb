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