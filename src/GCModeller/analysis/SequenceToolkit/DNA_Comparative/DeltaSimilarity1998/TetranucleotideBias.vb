Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' TETRANUCLEOTIDE RELATIVE ABUNDANCE ``tau*`` (Karlin &amp; Cardon 1994, 
    ''' applied in Karlin, Campbell &amp; Mrazek 1998 Table 4).
    ''' 
    ''' The generalized odds ratio of a tetranucleotide XYZW:
    ''' 
    ''' ```
    '''    tau*(XYZW) = f(XYZW) * f(YZ) / ( f(XYZ) * f(YZW) )
    ''' ```
    ''' 
    ''' The numerator contains the target tetranucleotide and the central dinucleotide, 
    ''' the denominator the two overlapping trinucleotides. This subtracts the biases 
    ''' that are already explained by the lower order (di-/tri-nucleotide) composition, 
    ''' and therefore tests whether the tetranucleotide still shows an additional 
    ''' over-/under-representation.
    ''' 
    ''' All frequencies are double-strand symmetrized (evaluated on the sequence 
    ''' concatenated with its reverse complement, the same convention as the 
    ''' ``rho*`` genome signature).
    ''' 
    ''' Application: restriction site avoidance analysis (e.g. CTAG in 
    ''' ``M. jannaschii``: tau* = 0.06 with only 90 occurrences).
    ''' </summary>
    <Package("Tetranucleotide.Bias",
             Description:="tau* tetranucleotide relative abundance generalized odds ratio (Karlin & Cardon 1994)")>
    Public Module TetranucleotideBias

        ''' <summary>
        ''' Compute the symmetrized ``tau*`` value of the given tetranucleotide word.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="word">the four-base word, e.g. ``"CTAG"``</param>
        ''' <returns>the tau* value; 0 when the word (or its statistics) is absent</returns>
        <ExportAPI("TauStar")>
        <Extension>
        Public Function TauStar(nt As NucleicAcid, word As String) As Double
            Dim counts As TetranucleotideCounts = TetranucleotideCounts.Compute(nt)

            If counts Is Nothing Then
                Return 0
            Else
                Return counts.TauStar(word)
            End If
        End Function

        ''' <summary>
        ''' Compute the symmetrized ``tau*`` profile of all 256 tetranucleotides.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <returns>
        ''' ``tau*`` values keyed by the four-base word (only the words that actually 
        ''' occur in the sequence are included)
        ''' </returns>
        <ExportAPI("TauStar.Profile")>
        <Extension>
        Public Function TauStarProfile(nt As NucleicAcid) As Dictionary(Of String, Double)
            Dim counts As TetranucleotideCounts = TetranucleotideCounts.Compute(nt)
            Dim profile As New Dictionary(Of String, Double)

            For idx As Integer = 0 To 255
                Dim tau As Double = counts.TauStarByIndex(idx)

                If tau > 0 Then
                    profile(UnpackWord(idx)) = tau
                End If
            Next

            Return profile
        End Function

        ''' <summary>
        ''' List the rare and the frequent tetranucleotides against the given 
        ''' ``tau*`` thresholds (restriction avoidance candidates).
        ''' </summary>
        ''' <param name="profile">result of <see cref="TetranucleotideBias.TauStarProfile"/></param>
        ''' <param name="rareBelow">words with ``tau*`` below this value are rare (under-represented)</param>
        ''' <param name="frequentAbove">words with ``tau*`` above this value are frequent (over-represented)</param>
        ''' <returns></returns>
        <Extension>
        Public Function RareFrequentWords(profile As Dictionary(Of String, Double),
                                          Optional rareBelow As Double = 0.78,
                                          Optional frequentAbove As Double = 1.23) As (Rare As String(), Frequent As String())
            Dim rare As New List(Of String)
            Dim frequent As New List(Of String)

            For Each word In profile
                If word.Value < rareBelow Then
                    rare.Add(word.Key)
                ElseIf word.Value > frequentAbove Then
                    frequent.Add(word.Key)
                End If
            Next

            Return (rare.OrderBy(Function(s) profile(s)).ToArray,
                    frequent.OrderByDescending(Function(s) profile(s)).ToArray)
        End Function

        ''' <summary>
        ''' Count all occurrences of the given word in the genome sequence 
        ''' (returns the absolute count, useful e.g. for the CTAG rarity report).
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="word"></param>
        ''' <returns></returns>
        <ExportAPI("Word.Count")>
        <Extension>
        Public Function WordCount(nt As NucleicAcid, word As String) As Integer
            Dim idx As Integer = PackWord(word)

            If idx < 0 Then
                Return 0
            End If

            Dim counts As TetranucleotideCounts = TetranucleotideCounts.Compute(nt)
            Return counts.tetramerSym(idx)
        End Function

        Friend Function PackWord(word As String) As Integer
            If word Is Nothing OrElse word.Length <> 4 Then
                Return -1
            End If

            Dim idx As Integer = 0

            For Each c As Char In word.ToUpper
                Dim b As Integer = PackBase(c)

                If b < 0 Then
                    Return -1
                Else
                    idx = idx * 4 + b
                End If
            Next

            Return idx
        End Function

        Private Function PackBase(c As Char) As Integer
            Select Case c
                Case "A"c : Return 0
                Case "G"c : Return 1
                Case "C"c : Return 2
                Case "T"c : Return 3
                Case Else : Return -1
            End Select
        End Function

        Friend Function UnpackWord(idx As Integer) As String
            Dim chars(3) As Char

            For i As Integer = 3 To 0 Step -1
                Select Case idx Mod 4
                    Case 0 : chars(i) = "A"c
                    Case 1 : chars(i) = "G"c
                    Case 2 : chars(i) = "C"c
                    Case 3 : chars(i) = "T"c
                End Select

                idx \= 4
            Next

            Return New String(chars)
        End Function
    End Module

    ''' <summary>
    ''' The single-pass tetranucleotide/dinucleotide/trinucleotide count matrix of 
    ''' one sequence, symmetrized with its reverse complement.
    ''' </summary>
    Friend Class TetranucleotideCounts

        ''' <summary>symmetrized dinucleotide counts, packed index 0..15</summary>
        Friend ReadOnly dimerSym As Integer() = New Integer(15) {}
        ''' <summary>
        ''' symmetrized trinucleotide counts over all sequence positions, packed index 0..63
        ''' </summary>
        Friend ReadOnly trimerSym As Integer() = New Integer(63) {}
        ''' <summary>symmetrized tetranucleotide counts, packed index 0..255</summary>
        Friend ReadOnly tetramerSym As Integer() = New Integer(255) {}

        Private Shared ReadOnly complementMap As Integer() = {3, 2, 1, 0}
        '  (A=0, G=1, C=2, T=3) -> (T=3, C=2, G=1, A=0)

        ''' <summary>
        ''' Single pass counting over the sequence and its reverse complement.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <returns></returns>
        Public Shared Function Compute(nt As NucleicAcid) As TetranucleotideCounts
            Dim seq As DNA() = nt.nt
            Dim n As Integer = seq.Length

            If n < 4 Then
                Return Nothing
            End If

            Dim counts As New TetranucleotideCounts

            ' forward strand pass + reverse complement pass share the same counter 
            ' arrays (the symmetrization is additive)
            Call CountPass(counts, seq)

            Dim rc As DNA() = NucleicAcid.ReverseComplement(seq)
            Call CountPass(counts, rc)

            Return counts
        End Function

        Private Shared Sub CountPass(counts As TetranucleotideCounts, seq As DNA())
            Dim n As Integer = seq.Length

            ' rolling window over the packed 4-mer indices
            ' after processing position j: w3 = base(j), w2 = base(j-1), 
            ' w1 = base(j-2), w0 = base(j-3)
            Dim w0 As Integer = -1, w1 As Integer = -1, w2 As Integer = -1, w3 As Integer = -1

            For j As Integer = 0 To n - 1
                Dim b As Integer = BaseIndexOrMinus1(seq(j))

                If b < 0 Then
                    ' ambiguous base resets all rolling windows
                    w0 = -1 : w1 = -1 : w2 = -1 : w3 = -1
                    Continue For
                End If

                w0 = w1 : w1 = w2 : w2 = w3 : w3 = b

                ' dinucleotide count at every position
                If w2 >= 0 Then
                    counts.dimerSym(w2 * 4 + w3) += 1
                End If

                ' trinucleotide count at every position (positions j-2..j)
                If w1 >= 0 Then
                    counts.trimerSym((w1 * 4 + w2) * 4 + w3) += 1
                End If

                ' tetranucleotide count: full 4-mer w0w1w2w3 (positions j-3..j)
                If w0 >= 0 Then
                    counts.tetramerSym(((w0 * 4 + w1) * 4 + w2) * 4 + w3) += 1
                End If
            Next
        End Sub

        Private Shared Function BaseIndexOrMinus1(base As DNA) As Integer
            Select Case base
                Case DNA.dAMP : Return 0
                Case DNA.dGMP : Return 1
                Case DNA.dCMP : Return 2
                Case DNA.dTMP : Return 3
                Case Else : Return -1
            End Select
        End Function

        ''' <summary>
        ''' Symmetrized ``tau*`` value of the tetranucleotide at the packed index:
        ''' ``tau* = f(XYZW) * f(YZ) / (f(XYZ) * f(YZW))``
        ''' </summary>
        ''' <param name="idx">packed 4-mer index 0..255</param>
        ''' <returns></returns>
        Friend Function TauStarByIndex(idx As Integer) As Double
            Dim b0 As Integer = (idx \ 64) Mod 4
            Dim b1 As Integer = (idx \ 16) Mod 4
            Dim b2 As Integer = (idx \ 4) Mod 4
            Dim b3 As Integer = idx Mod 4

            ' the symmetrized word count of XYZW: count(XYZW) + count(comp word)
            Dim cIdx As Integer = ((complementMap(b3) * 4 + complementMap(b2)) * 4 + complementMap(b1)) * 4 + complementMap(b0)
            Dim fXYZW As Double = tetramerSym(idx) + tetramerSym(cIdx)

            If fXYZW <= 0 Then
                Return 0
            End If

            Dim yz As Integer = b1 * 4 + b2
            Dim yzComp As Integer = complementMap(b2) * 4 + complementMap(b1)
            Dim fYZ As Double = dimerSym(yz) + dimerSym(yzComp)

            Dim xyz As Integer = (b0 * 4 + b1) * 4 + b2
            Dim xyzComp As Integer = (complementMap(b2) * 4 + complementMap(b1)) * 4 + complementMap(b0)
            Dim fXYZ As Double = trimerSym(xyz) + trimerSym(xyzComp)

            ' YZW trinucleotide frequency over all sequence positions
            ' reverse complement of YZW = comp(W)comp(Z)comp(Y)
            Dim yzw As Integer = (b1 * 4 + b2) * 4 + b3
            Dim yzwComp As Integer = (complementMap(b3) * 4 + complementMap(b2)) * 4 + complementMap(b1)
            Dim fYZW As Double = trimerSym(yzw) + trimerSym(yzwComp)

            If fXYZ <= 0 OrElse fYZW <= 0 OrElse fYZ <= 0 Then
                Return 0
            End If

            Return fXYZW * fYZ / (fXYZ * fYZW)
        End Function

        ''' <summary>
        ''' Symmetrized ``tau*`` value of the given four-base word.
        ''' </summary>
        ''' <param name="word"></param>
        ''' <returns></returns>
        Friend Function TauStar(word As String) As Double
            Dim idx As Integer = TetranucleotideBias.PackWord(word)

            If idx < 0 Then
                Return 0
            Else
                Return TauStarByIndex(idx)
            End If
        End Function
    End Class
End Namespace
