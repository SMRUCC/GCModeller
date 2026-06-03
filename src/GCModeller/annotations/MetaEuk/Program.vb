' ============================================================================
' MetaEukVB - Eukaryotic Gene Prediction Tool (VB.NET Implementation)
' Based on MetaEuk algorithm: homology-based exon chain optimization
'
' Algorithm Pipeline:
'   1. Six-frame translation of contigs -> candidate coding fragments
'   2. Homology search (Smith-Waterman) against reference protein database
'   3. Group by (Target, Contig, Strand) -> TCS groups
'   4. Dynamic programming: optimal exon set per TCS
'   5. Redundancy removal: cluster overlapping predictions, pick representative
'   6. Same-strand conflict resolution: keep best E-value
'   7. Output: Protein FASTA, GFF3, TSV summary
' ============================================================================

Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Module MetaEukVB

    ' ========================================================================
    ' GLOBAL CONFIGURATION
    ' ========================================================================

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

    ''' <summary>Strand orientation</summary>
    Public Enum StrandOrientation
        Plus = 1
        Minus = -1
    End Enum

    ''' <summary>A candidate coding fragment from six-frame translation</summary>
    Public Class CandidateFragment
        Public ContigID As String
        Public Strand As StrandOrientation
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
        Public Strand As StrandOrientation
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
        Public Strand As StrandOrientation
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
        Public Strand As StrandOrientation
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

    ' ========================================================================
    ' MODULE 2: CODON TABLE & SIX-FRAME TRANSLATION
    ' ========================================================================

    Public Class CodonTable
        ' Standard genetic code (NCBI translation table 1)
        Private Shared ReadOnly CodonMap As New Dictionary(Of String, Char) From {
            {"TTT", "F"c}, {"TTC", "F"c}, {"TTA", "L"c}, {"TTG", "L"c},
            {"CTT", "L"c}, {"CTC", "L"c}, {"CTA", "L"c}, {"CTG", "L"c},
            {"ATT", "I"c}, {"ATC", "I"c}, {"ATA", "I"c}, {"ATG", "M"c},
            {"GTT", "V"c}, {"GTC", "V"c}, {"GTA", "V"c}, {"GTG", "V"c},
            {"TCT", "S"c}, {"TCC", "S"c}, {"TCA", "S"c}, {"TCG", "S"c},
            {"CCT", "P"c}, {"CCC", "P"c}, {"CCA", "P"c}, {"CCG", "P"c},
            {"ACT", "T"c}, {"ACC", "T"c}, {"ACA", "T"c}, {"ACG", "T"c},
            {"GCT", "A"c}, {"GCC", "A"c}, {"GCA", "A"c}, {"GCG", "A"c},
            {"TAT", "Y"c}, {"TAC", "Y"c}, {"TAA", "*"c}, {"TAG", "*"c},
            {"CAT", "H"c}, {"CAC", "H"c}, {"CAA", "Q"c}, {"CAG", "Q"c},
            {"AAT", "N"c}, {"AAC", "N"c}, {"AAA", "K"c}, {"AAG", "K"c},
            {"GAT", "D"c}, {"GAC", "D"c}, {"GAA", "E"c}, {"GAG", "E"c},
            {"TGT", "C"c}, {"TGC", "C"c}, {"TGA", "*"c}, {"TGG", "W"c},
            {"CGT", "R"c}, {"CGC", "R"c}, {"CGA", "R"c}, {"CGG", "R"c},
            {"AGT", "S"c}, {"AGC", "S"c}, {"AGA", "R"c}, {"AGG", "R"c},
            {"GGT", "G"c}, {"GGC", "G"c}, {"GGA", "G"c}, {"GGG", "G"c}
        }

        ''' <summary>Translate a single codon to amino acid; 'X' for unknown</summary>
        Public Shared Function TranslateCodon(codon As String) As Char
            If codon.Length <> 3 Then Return "X"c
            Dim upper = codon.ToUpper()
            If CodonMap.ContainsKey(upper) Then Return CodonMap(upper)
            ' Handle ambiguous bases: if any N, return X
            Return "X"c
        End Function

        ''' <summary>Translate a DNA sequence in one reading frame</summary>
        Public Shared Function Translate(dna As String, frameOffset As Integer) As String
            Dim sb As New StringBuilder()
            Dim i As Integer = frameOffset
            While i + 2 < dna.Length
                Dim codon = dna.Substring(i, 3)
                Dim aa = TranslateCodon(codon)
                sb.Append(aa)
                i += 3
            End While
            Return sb.ToString()
        End Function

        ''' <summary>Get reverse complement of a DNA sequence</summary>
        Public Shared Function ReverseComplement(dna As String) As String
            Dim complement As New Dictionary(Of Char, Char) From {
                {"A"c, "T"c}, {"T"c, "A"c}, {"G"c, "C"c}, {"C"c, "G"c},
                {"N"c, "N"c}, {"R"c, "Y"c}, {"Y"c, "R"c}, {"M"c, "K"c},
                {"K"c, "M"c}, {"S"c, "S"c}, {"W"c, "W"c}, {"H"c, "D"c},
                {"D"c, "H"c}, {"B"c, "V"c}, {"V"c, "B"c}
            }
            Dim sb As New StringBuilder(dna.Length)
            For i As Integer = dna.Length - 1 To 0 Step -1
                Dim ch = Char.ToUpper(dna(i))
                If complement.ContainsKey(ch) Then
                    sb.Append(complement(ch))
                Else
                    sb.Append("N"c)
                End If
            Next
            Return sb.ToString()
        End Function

    End Class

    ' ========================================================================
    ' MODULE 3: SIX-FRAME TRANSLATION & CANDIDATE FRAGMENT GENERATION
    ' ========================================================================

    Public Class SixFrameTranslator

        ''' <summary>
        ''' Perform six-frame translation on a contig and extract candidate coding fragments.
        ''' Fragments are delimited by stop codons (*). Only fragments meeting minimum
        ''' length criteria are retained.
        ''' </summary>
        Public Shared Function GenerateFragments(contig As FastaSeq, config As MetaEukConfig) As List(Of CandidateFragment)
            Dim fragments As New List(Of CandidateFragment)
            Dim dna = contig.SequenceData.ToUpper()
            Dim contigID = contig.locus_tag
            Dim fragIdx As Integer = 0

            ' --- Forward strand (frames 0, 1, 2) ---
            For frame = 0 To 2
                Dim peptide = CodonTable.Translate(dna, frame)
                Dim newFrags = ExtractFragmentsFromPeptide(
                    peptide, contigID, StrandOrientation.Plus, frame, dna.Length, config)
                For Each f In newFrags
                    f.FragmentIndex = fragIdx
                    fragIdx += 1
                Next
                fragments.AddRange(newFrags)
            Next

            ' --- Reverse strand (frames 0, 1, 2 on reverse complement) ---
            Dim rcDna = CodonTable.ReverseComplement(dna)
            For frame = 0 To 2
                Dim peptide = CodonTable.Translate(rcDna, frame)
                Dim newFrags = ExtractFragmentsFromPeptide(
                    peptide, contigID, StrandOrientation.Minus, frame, dna.Length, config)
                For Each f In newFrags
                    f.FragmentIndex = fragIdx
                    fragIdx += 1
                Next
                fragments.AddRange(newFrags)
            Next

            Return fragments
        End Function

        ''' <summary>
        ''' Extract candidate fragments from a translated peptide sequence.
        ''' Split at stop codons (*), filter by length, compute DNA coordinates.
        ''' </summary>
        Private Shared Function ExtractFragmentsFromPeptide(
            peptide As String,
            contigID As String,
            strand As StrandOrientation,
            frame As Integer,
            dnaLength As Integer,
            config As MetaEukConfig) As List(Of CandidateFragment)

            Dim fragments As New List(Of CandidateFragment)

            ' Split peptide at stop codons to get ORF segments
            Dim segments = peptide.Split({"*"c}, StringSplitOptions.RemoveEmptyEntries)

            ' Track position in peptide to compute DNA coordinates
            Dim pepPos As Integer = 0

            For Each seg In segments
                Dim segStart As Integer = peptide.IndexOf(seg, pepPos)
                Dim segEnd As Integer = segStart + seg.Length - 1
                pepPos = segEnd + 1

                If seg.Length < config.MinFragmentLength Then Continue For
                If seg.Length > config.MaxFragmentLength Then
                    ' For very long segments, split into overlapping windows
                    Dim stepSize As Integer = CInt(config.MaxFragmentLength * 0.75)
                    Dim winStart As Integer = 0
                    While winStart < seg.Length
                        Dim winEnd As Integer = Math.Min(winStart + config.MaxFragmentLength - 1, seg.Length - 1)
                        Dim winSeq = seg.Substring(winStart, winEnd - winStart + 1)
                        If winSeq.Length >= config.MinFragmentLength Then
                            Dim frag = CreateFragment(contigID, strand, frame, dnaLength,
                                                      segStart + winStart, segStart + winEnd, winSeq)
                            fragments.Add(frag)
                        End If
                        winStart += stepSize
                    End While
                Else
                    Dim frag = CreateFragment(contigID, strand, frame, dnaLength,
                                              segStart, segEnd, seg)
                    fragments.Add(frag)
                End If
            Next

            Return fragments
        End Function

        ''' <summary>
        ''' Create a CandidateFragment with proper DNA coordinate mapping.
        ''' Converts peptide coordinates back to DNA coordinates on the original contig.
        ''' </summary>
        Private Shared Function CreateFragment(
            contigID As String,
            strand As StrandOrientation,
            frame As Integer,
            dnaLength As Integer,
            pepStart As Integer,
            pepEnd As Integer,
            peptide As String) As CandidateFragment

            Dim frag As New CandidateFragment()
            frag.ContigID = contigID
            frag.Strand = strand
            frag.Frame = frame
            frag.Peptide = peptide

            ' Convert peptide position to DNA position
            ' pepStart is 0-based index in the translated peptide
            ' DNA position of codon for pepStart: frame + pepStart * 3
            ' DNA position of codon for pepEnd: frame + pepEnd * 3 + 2

            If strand = StrandOrientation.Plus Then
                ' Forward strand: DNA coordinates are straightforward
                frag.DnaStart = frame + pepStart * 3 + 1       ' 1-based
                frag.DnaEnd = frame + pepEnd * 3 + 3           ' 1-based, inclusive
            Else
                ' Reverse strand: need to map back to forward strand coordinates
                ' On reverse complement, position i maps to (dnaLength - 1 - i) on forward strand
                Dim rcStart = frame + pepStart * 3              ' 0-based on RC
                Dim rcEnd = frame + pepEnd * 3 + 2             ' 0-based on RC
                ' Map to forward strand (1-based)
                frag.DnaStart = dnaLength - rcEnd               ' 1-based
                frag.DnaEnd = dnaLength - rcStart               ' 1-based
            End If

            Return frag
        End Function

    End Class

    ' ========================================================================
    ' MODULE 4: SMITH-WATERMAN LOCAL ALIGNMENT (Homology Search)
    ' ========================================================================

    Public Class SmithWatermanAligner

        ' BLOSUM62 substitution matrix (standard for protein alignment)
        Private Shared ReadOnly BLOSUM62 As Dictionary(Of String, Integer) = CreateBlosum62()

        Private Shared Function CreateBlosum62() As Dictionary(Of String, Integer)
            Dim m As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            ' A
            m("AA") = 4 : m("AR") = -1 : m("AN") = -2 : m("AD") = -2 : m("AC") = 0 : m("AQ") = -1 : m("AE") = -1 : m("AG") = 0 : m("AH") = -2 : m("AI") = -1 : m("AL") = -1 : m("AK") = -1 : m("AM") = -1 : m("AF") = -2 : m("AP") = -1 : m("AS") = 1 : m("AT") = 0 : m("AW") = -3 : m("AY") = -2 : m("AV") = 0
            ' R
            m("RA") = -1 : m("RR") = 5 : m("RN") = 0 : m("RD") = -2 : m("RC") = -3 : m("RQ") = 1 : m("RE") = 0 : m("RG") = -2 : m("RH") = 0 : m("RI") = -3 : m("RL") = -2 : m("RK") = 2 : m("RM") = -1 : m("RF") = -3 : m("RP") = -2 : m("RS") = -1 : m("RT") = -1 : m("RW") = -3 : m("RY") = -2 : m("RV") = -3
            ' N
            m("NA") = -2 : m("NR") = 0 : m("NN") = 6 : m("ND") = 1 : m("NC") = -3 : m("NQ") = 0 : m("NE") = 0 : m("NG") = 0 : m("NH") = 1 : m("NI") = -3 : m("NL") = -3 : m("NK") = 0 : m("NM") = -2 : m("NF") = -3 : m("NP") = -2 : m("NS") = 1 : m("NT") = 0 : m("NW") = -4 : m("NY") = -2 : m("NV") = -3
            ' D
            m("DA") = -2 : m("DR") = -2 : m("DN") = 1 : m("DD") = 6 : m("DC") = -3 : m("DQ") = 0 : m("DE") = 2 : m("DG") = -1 : m("DH") = -1 : m("DI") = -3 : m("DL") = -4 : m("DK") = -1 : m("DM") = -3 : m("DF") = -3 : m("DP") = -1 : m("DS") = 0 : m("DT") = -1 : m("DW") = -4 : m("DY") = -3 : m("DV") = -3
            ' C
            m("CA") = 0 : m("CR") = -3 : m("CN") = -3 : m("CD") = -3 : m("CC") = 9 : m("CQ") = -3 : m("CE") = -4 : m("CG") = -3 : m("CH") = -3 : m("CI") = -1 : m("CL") = -1 : m("CK") = -3 : m("CM") = -1 : m("CF") = -2 : m("CP") = -3 : m("CS") = -1 : m("CT") = -1 : m("CW") = -2 : m("CY") = -2 : m("CV") = -1
            ' Q
            m("QA") = -1 : m("QR") = 1 : m("QN") = 0 : m("QD") = 0 : m("QC") = -3 : m("QQ") = 5 : m("QE") = 2 : m("QG") = -2 : m("QH") = 0 : m("QI") = -3 : m("QL") = -2 : m("QK") = 1 : m("QM") = 0 : m("QF") = -3 : m("QP") = -1 : m("QS") = 0 : m("QT") = -1 : m("QW") = -2 : m("QY") = -1 : m("QV") = -2
            ' E
            m("EA") = -1 : m("ER") = 0 : m("EN") = 0 : m("ED") = 2 : m("EC") = -4 : m("EQ") = 2 : m("EE") = 5 : m("EG") = -2 : m("EH") = 0 : m("EI") = -3 : m("EL") = -3 : m("EK") = 1 : m("EM") = -2 : m("EF") = -3 : m("EP") = -1 : m("ES") = 0 : m("ET") = -1 : m("EW") = -3 : m("EY") = -2 : m("EV") = -2
            ' G
            m("GA") = 0 : m("GR") = -2 : m("GN") = 0 : m("GD") = -1 : m("GC") = -3 : m("GQ") = -2 : m("GE") = -2 : m("GG") = 6 : m("GH") = -2 : m("GI") = -4 : m("GL") = -4 : m("GK") = -2 : m("GM") = -3 : m("GF") = -3 : m("GP") = -2 : m("GS") = 0 : m("GT") = -2 : m("GW") = -2 : m("GY") = -3 : m("GV") = -3
            ' H
            m("HA") = -2 : m("HR") = 0 : m("HN") = 1 : m("HD") = -1 : m("HC") = -3 : m("HQ") = 0 : m("HE") = 0 : m("HG") = -2 : m("HH") = 8 : m("HI") = -3 : m("HL") = -3 : m("HK") = -1 : m("HM") = -2 : m("HF") = -1 : m("HP") = -2 : m("HS") = -1 : m("HT") = -2 : m("HW") = -2 : m("HY") = 2 : m("HV") = -3
            ' I
            m("IA") = -1 : m("IR") = -3 : m("IN") = -3 : m("ID") = -3 : m("IC") = -1 : m("IQ") = -3 : m("IE") = -3 : m("IG") = -4 : m("IH") = -3 : m("II") = 4 : m("IL") = 2 : m("IK") = -3 : m("IM") = 1 : m("IF") = 0 : m("IP") = -3 : m("IS") = -2 : m("IT") = -1 : m("IW") = -3 : m("IY") = -1 : m("IV") = 3
            ' L
            m("LA") = -1 : m("LR") = -2 : m("LN") = -3 : m("LD") = -4 : m("LC") = -1 : m("LQ") = -2 : m("LE") = -3 : m("LG") = -4 : m("LH") = -3 : m("LI") = 2 : m("LL") = 4 : m("LK") = -2 : m("LM") = 2 : m("LF") = 0 : m("LP") = -3 : m("LS") = -2 : m("LT") = -1 : m("LW") = -2 : m("LY") = -1 : m("LV") = 1
            ' K
            m("KA") = -1 : m("KR") = 2 : m("KN") = 0 : m("KD") = -1 : m("KC") = -3 : m("KQ") = 1 : m("KE") = 1 : m("KG") = -2 : m("KH") = -1 : m("KI") = -3 : m("KL") = -2 : m("KK") = 5 : m("KM") = -1 : m("KF") = -3 : m("KP") = -1 : m("KS") = 0 : m("KT") = -1 : m("KW") = -3 : m("KY") = -2 : m("KV") = -2
            ' M
            m("MA") = -1 : m("MR") = -1 : m("MN") = -2 : m("MD") = -3 : m("MC") = -1 : m("MQ") = 0 : m("ME") = -2 : m("MG") = -3 : m("MH") = -2 : m("MI") = 1 : m("ML") = 2 : m("MK") = -1 : m("MM") = 5 : m("MF") = 0 : m("MP") = -2 : m("MS") = -1 : m("MT") = -1 : m("MW") = -1 : m("MY") = -1 : m("MV") = 1
            ' F
            m("FA") = -2 : m("FR") = -3 : m("FN") = -3 : m("FD") = -3 : m("FC") = -2 : m("FQ") = -3 : m("FE") = -3 : m("FG") = -3 : m("FH") = -1 : m("FI") = 0 : m("FL") = 0 : m("FK") = -3 : m("FM") = 0 : m("FF") = 6 : m("FP") = -4 : m("FS") = -2 : m("FT") = -2 : m("FW") = 1 : m("FY") = 3 : m("FV") = -1
            ' P
            m("PA") = -1 : m("PR") = -2 : m("PN") = -2 : m("PD") = -1 : m("PC") = -3 : m("PQ") = -1 : m("PE") = -1 : m("PG") = -2 : m("PH") = -2 : m("PI") = -3 : m("PL") = -3 : m("PK") = -1 : m("PM") = -2 : m("PF") = -4 : m("PP") = 7 : m("PS") = -1 : m("PT") = -1 : m("PW") = -4 : m("PY") = -3 : m("PV") = -2
            ' S
            m("SA") = 1 : m("SR") = -1 : m("SN") = 1 : m("SD") = 0 : m("SC") = -1 : m("SQ") = 0 : m("SE") = 0 : m("SG") = 0 : m("SH") = -1 : m("SI") = -2 : m("SL") = -2 : m("SK") = 0 : m("SM") = -1 : m("SF") = -2 : m("SP") = -1 : m("SS") = 4 : m("ST") = 1 : m("SW") = -3 : m("SY") = -2 : m("SV") = -2
            ' T
            m("TA") = 0 : m("TR") = -1 : m("TN") = 0 : m("TD") = -1 : m("TC") = -1 : m("TQ") = -1 : m("TE") = -1 : m("TG") = -2 : m("TH") = -2 : m("TI") = -1 : m("TL") = -1 : m("TK") = -1 : m("TM") = -1 : m("TF") = -2 : m("TP") = -1 : m("TS") = 1 : m("TT") = 5 : m("TW") = -2 : m("TY") = -2 : m("TV") = 0
            ' W
            m("WA") = -3 : m("WR") = -3 : m("WN") = -4 : m("WD") = -4 : m("WC") = -2 : m("WQ") = -2 : m("WE") = -3 : m("WG") = -2 : m("WH") = -2 : m("WI") = -3 : m("WL") = -2 : m("WK") = -3 : m("WM") = -1 : m("WF") = 1 : m("WP") = -4 : m("WS") = -3 : m("WT") = -2 : m("WW") = 11 : m("WY") = 2 : m("WV") = -3
            ' Y
            m("YA") = -2 : m("YR") = -2 : m("YN") = -2 : m("YD") = -3 : m("YC") = -2 : m("YQ") = -1 : m("YE") = -2 : m("YG") = -3 : m("YH") = 2 : m("YI") = -1 : m("YL") = -1 : m("YK") = -2 : m("YM") = -1 : m("YF") = 3 : m("YP") = -3 : m("YS") = -2 : m("YT") = -2 : m("YW") = 2 : m("YY") = 7 : m("YV") = -1
            ' V
            m("VA") = 0 : m("VR") = -3 : m("VN") = -3 : m("VD") = -3 : m("VC") = -1 : m("VQ") = -2 : m("VE") = -2 : m("VG") = -3 : m("VH") = -3 : m("VI") = 3 : m("VL") = 1 : m("VK") = -2 : m("VM") = 1 : m("VF") = -1 : m("VP") = -2 : m("VS") = -2 : m("VT") = 0 : m("VW") = -3 : m("VY") = -1 : m("VV") = 4
            Return m
        End Function

        ''' <summary>Get substitution score from BLOSUM62</summary>
        Public Shared Function GetSubScore(a As Char, b As Char) As Integer
            Dim key = $"{a}{b}"
            If BLOSUM62.ContainsKey(key) Then Return BLOSUM62(key)
            Return -1  ' default penalty for unknown residues
        End Function

        ''' <summary>
        ''' Smith-Waterman local alignment with affine gap penalties.
        ''' Returns the best alignment hit or Nothing if no significant alignment found.
        ''' </summary>
        Public Shared Function Align(
            query As String,
            target As String,
            gapOpen As Integer,
            gapExtend As Integer,
            config As MetaEukConfig) As HomologyHit

            Dim m As Integer = query.Length
            Dim n As Integer = target.Length

            If m = 0 OrElse n = 0 Then Return Nothing

            ' Limit alignment size for performance
            Const MAX_ALIGN_SIZE As Integer = 10000
            If m > MAX_ALIGN_SIZE Then m = MAX_ALIGN_SIZE
            If n > MAX_ALIGN_SIZE Then n = MAX_ALIGN_SIZE

            ' Score matrices
            Dim H(m, n) As Double    ' main score matrix
            Dim E(m, n) As Double    ' gap in query (insertion in target)
            Dim F(m, n) As Double    ' gap in target (insertion in query)

            ' Traceback: 0=stop, 1=diagonal, 2=from E, 3=from F
            Dim traceH(m, n) As Byte
            Dim traceE(m, n) As Byte  ' 1=open from H, 2=extend
            Dim traceF(m, n) As Byte  ' 1=open from H, 2=extend

            ' Initialize
            For i As Integer = 0 To m
                For j As Integer = 0 To n
                    H(i, j) = 0
                    E(i, j) = Double.NegativeInfinity
                    F(i, j) = Double.NegativeInfinity
                Next
            Next

            Dim maxScore As Double = 0
            Dim maxI As Integer = 0
            Dim maxJ As Integer = 0

            ' Fill matrices
            For i As Integer = 1 To m
                For j As Integer = 1 To n
                    ' Substitution score
                    Dim subScore As Integer = GetSubScore(query(i - 1), target(j - 1))

                    ' E: gap in query (horizontal move)
                    Dim eOpen = H(i, j - 1) - gapOpen - gapExtend
                    Dim eExtend = E(i, j - 1) - gapExtend
                    If eOpen >= eExtend Then
                        E(i, j) = eOpen
                        traceE(i, j) = 1
                    Else
                        E(i, j) = eExtend
                        traceE(i, j) = 2
                    End If

                    ' F: gap in target (vertical move)
                    Dim fOpen = H(i - 1, j) - gapOpen - gapExtend
                    Dim fExtend = F(i - 1, j) - gapExtend
                    If fOpen >= fExtend Then
                        F(i, j) = fOpen
                        traceF(i, j) = 1
                    Else
                        F(i, j) = fExtend
                        traceF(i, j) = 2
                    End If

                    ' H: max of diagonal, E, F, or 0 (local alignment)
                    Dim diag = H(i - 1, j - 1) + subScore
                    Dim best = diag
                    traceH(i, j) = 1

                    If E(i, j) > best Then
                        best = E(i, j)
                        traceH(i, j) = 2
                    End If

                    If F(i, j) > best Then
                        best = F(i, j)
                        traceH(i, j) = 3
                    End If

                    If best < 0 Then
                        best = 0
                        traceH(i, j) = 0
                    End If

                    H(i, j) = best

                    If best > maxScore Then
                        maxScore = best
                        maxI = i
                        maxJ = j
                    End If
                Next
            Next

            ' No significant alignment
            If maxScore < config.MinExonScore Then Return Nothing

            ' Traceback
            Dim alignedQuery As New StringBuilder()
            Dim alignedTarget As New StringBuilder()
            Dim ti As Integer = maxI
            Dim tj As Integer = maxJ
            Dim currentMatrix = "H"  ' start from H matrix

            While ti > 0 AndAlso tj > 0
                If currentMatrix = "H" Then
                    If traceH(ti, tj) = 0 Then Exit While
                    If traceH(ti, tj) = 1 Then
                        ' Diagonal
                        alignedQuery.Insert(0, query(ti - 1))
                        alignedTarget.Insert(0, target(tj - 1))
                        ti -= 1
                        tj -= 1
                        currentMatrix = "H"
                    ElseIf traceH(ti, tj) = 2 Then
                        currentMatrix = "E"
                    ElseIf traceH(ti, tj) = 3 Then
                        currentMatrix = "F"
                    End If
                ElseIf currentMatrix = "E" Then
                    ' Gap in query
                    alignedQuery.Insert(0, "-"c)
                    alignedTarget.Insert(0, target(tj - 1))
                    If traceE(ti, tj) = 1 Then
                        currentMatrix = "H"
                    End If
                    tj -= 1
                ElseIf currentMatrix = "F" Then
                    ' Gap in target
                    alignedQuery.Insert(0, query(ti - 1))
                    alignedTarget.Insert(0, "-"c)
                    If traceF(ti, tj) = 1 Then
                        currentMatrix = "H"
                    End If
                    ti -= 1
                End If
            End While

            ' Compute alignment statistics
            Dim alignLen = alignedQuery.Length
            Dim matches As Integer = 0
            Dim positives As Integer = 0
            For k As Integer = 0 To alignLen - 1
                If alignedQuery(k) = alignedTarget(k) Then
                    matches += 1
                    positives += 1
                ElseIf GetSubScore(alignedQuery(k), alignedTarget(k)) > 0 Then
                    positives += 1
                End If
            Next

            Dim identity = If(alignLen > 0, matches / alignLen, 0.0)
            Dim bitscore = ComputeBitScore(maxScore, m, n)
            Dim evalue = ComputeEValue(bitscore, m, n)

            ' Filter by thresholds
            If evalue > config.EvalueThreshold Then Return Nothing
            If identity < config.MinIdentity Then Return Nothing

            Dim hit As New HomologyHit()
            hit.Score = bitscore
            hit.Evalue = evalue
            hit.Identity = identity
            hit.AlignStartQuery = ti      ' 0-based start in query
            hit.AlignEndQuery = maxI - 1   ' 0-based end in query
            hit.AlignStartTarget = tj      ' 0-based start in target
            hit.AlignEndTarget = maxJ - 1   ' 0-based end in target
            hit.AlignedQuery = alignedQuery.ToString()
            hit.AlignedTarget = alignedTarget.ToString()
            Return hit
        End Function

        ''' <summary>Compute bit score from raw alignment score</summary>
        Public Shared Function ComputeBitScore(rawScore As Double, queryLen As Integer, targetLen As Integer) As Double
            ' Simplified bit score calculation
            ' In practice, uses Karlin-Altschul parameters lambda and K
            ' Here we use an approximation: bitscore = rawScore * 0.267 / ln(2)
            ' (typical values for BLOSUM62: lambda ~ 0.267, K ~ 0.041)
            Dim lambda As Double = 0.267
            Return If(rawScore > 0, (lambda * rawScore - Math.Log(0.041)) / Math.Log(2), 0)
        End Function

        ''' <summary>Compute E-value from bit score</summary>
        Public Shared Function ComputeEValue(bitScore As Double, queryLen As Integer, targetLen As Integer) As Double
            If bitScore <= 0 Then Return Double.MaxValue
            ' E = m * n * 2^(-bitscore) where m,n are effective search space sizes
            Dim searchSpace = CDbl(queryLen) * CDbl(targetLen)
            Return searchSpace * Math.Pow(2, -bitScore)
        End Function

    End Class

    ' ========================================================================
    ' MODULE 5: HOMOLOGY SEARCH ENGINE
    ' ========================================================================

    Public Class HomologySearchEngine

        ''' <summary>
        ''' Search all candidate fragments against reference protein database.
        ''' Returns list of significant homology hits.
        ''' </summary>
        Public Shared Function SearchAll(
            fragments As List(Of CandidateFragment),
            references As List(Of FastaSeq),
            config As MetaEukConfig) As List(Of HomologyHit)

            Dim hits As New List(Of HomologyHit)
            Dim gapOpen As Integer = 11
            Dim gapExtend As Integer = 1

            Console.WriteLine($"[INFO] Searching {fragments.Count} fragments against {references.Count} reference proteins...")

            Dim totalComparisons As Integer = fragments.Count * references.Count
            Dim doneComparisons As Integer = 0
            Dim lastPct As Integer = -1

            For Each frag In fragments
                For Each refSeq In references
                    doneComparisons += 1
                    Dim pct As Integer = CInt(doneComparisons / CDbl(totalComparisons) * 100)
                    If pct Mod 10 = 0 AndAlso pct <> lastPct Then
                        lastPct = pct
                        Console.WriteLine($"[INFO] Search progress: {pct}% ({doneComparisons}/{totalComparisons})")
                    End If

                    Dim hit = SmithWatermanAligner.Align(
                        frag.Peptide, refSeq.SequenceData, gapOpen, gapExtend, config)

                    If hit IsNot Nothing Then
                        hit.Fragment = frag
                        hit.TargetID = refSeq.locus_tag
                        hits.Add(hit)
                    End If
                Next
            Next

            Console.WriteLine($"[INFO] Found {hits.Count} significant homology hits")
            Return hits
        End Function

    End Class

    ' ========================================================================
    ' MODULE 6: TCS GROUPING
    ' ========================================================================

    Public Class TCSGrouper

        ''' <summary>
        ''' Group homology hits by (Target, Contig, Strand) and convert to candidate exons.
        ''' Within each TCS group, sort exons by DNA start position.
        ''' </summary>
        Public Shared Function GroupHits(hits As List(Of HomologyHit), config As MetaEukConfig) As List(Of TCSGroup)
            ' Convert hits to candidate exons and group by TCS key
            Dim groupDict As New Dictionary(Of String, TCSGroup)()

            For Each hit In hits
                If hit.Score < config.MinExonScore Then Continue For

                Dim exon As New CandidateExon()
                exon.Hit = hit
                exon.ContigID = hit.Fragment.ContigID
                exon.Strand = hit.Fragment.Strand
                exon.TargetID = hit.TargetID
                exon.Score = hit.Score
                exon.Evalue = hit.Evalue

                ' Compute exon DNA coordinates from alignment
                ' The aligned portion of the fragment maps to a sub-region
                Dim frag = hit.Fragment
                Dim alignLenInPep = hit.AlignEndQuery - hit.AlignStartQuery + 1
                Dim pepOffsetStart = hit.AlignStartQuery
                Dim pepOffsetEnd = hit.AlignEndQuery

                If frag.Strand = StrandOrientation.Plus Then
                    exon.DnaStart = frag.DnaStart + pepOffsetStart * 3
                    exon.DnaEnd = frag.DnaStart + pepOffsetEnd * 3 + 2
                Else
                    exon.DnaEnd = frag.DnaEnd - pepOffsetStart * 3
                    exon.DnaStart = frag.DnaEnd - pepOffsetEnd * 3 - 2
                End If

                ' Ensure start < end
                If exon.DnaStart > exon.DnaEnd Then
                    Dim tmp = exon.DnaStart
                    exon.DnaStart = exon.DnaEnd
                    exon.DnaEnd = tmp
                End If

                Dim key = $"{hit.TargetID}|{frag.ContigID}|{CStr(frag.Strand)}"
                If Not groupDict.ContainsKey(key) Then
                    groupDict(key) = New TCSGroup() With {
                        .TargetID = hit.TargetID,
                        .ContigID = frag.ContigID,
                        .Strand = frag.Strand
                    }
                End If

                exon.ExonIndex = groupDict(key).Exons.Count
                groupDict(key).Exons.Add(exon)
            Next

            ' Sort exons within each group by DNA start position
            Dim groups = groupDict.Values.ToList()
            For Each g In groups
                g.Exons.Sort(Function(a, b) a.DnaStart.CompareTo(b.DnaStart))
                ' Re-index after sorting
                For i = 0 To g.Exons.Count - 1
                    g.Exons(i).ExonIndex = i
                Next
            Next

            Console.WriteLine($"[INFO] Created {groups.Count} TCS groups")
            Return groups
        End Function

    End Class

    ' ========================================================================
    ' MODULE 7: DYNAMIC PROGRAMMING - OPTIMAL EXON SET SELECTION
    ' ========================================================================

    Public Class ExonChainOptimizer

        ''' <summary>
        ''' For each TCS group, use dynamic programming to find the optimal
        ''' non-overlapping exon chain that maximizes total score minus gap penalty.
        '''
        ''' DP formulation:
        '''   dp[i] = max(dp[i-1], score_i + max_{j&lt;i, compatible}(dp[j] - gap_cost(j,i)))
        '''   gap_cost(j,i) = lambda * (s_i - t_j - 1) / 3  (intronic bp / 3 = unmapped AAs)
        '''
        ''' Compatibility: exons must not overlap, and intron length &lt;= MaxIntronLength
        ''' </summary>
        Public Shared Sub OptimizeAll(groups As List(Of TCSGroup), config As MetaEukConfig)
            Console.WriteLine($"[INFO] Running DP exon chain optimization on {groups.Count} TCS groups...")

            For Each g In groups
                OptimizeGroup(g, config)
            Next

            Dim withChain = groups.Where(Function(g) g.OptimalChain.Count > 0).Count
            Console.WriteLine($"[INFO] {withChain} TCS groups have non-empty optimal exon chains")
        End Sub

        ''' <summary>Optimize a single TCS group using weighted interval scheduling DP</summary>
        Private Shared Sub OptimizeGroup(g As TCSGroup, config As MetaEukConfig)
            Dim n = g.Exons.Count
            If n = 0 Then Return

            ' dp(i) = best total score considering exons 0..i
            Dim dp(n - 1) As Double
            ' prev(i) = index of previous exon in optimal chain ending at i, or -1
            Dim prev(n - 1) As Integer
            ' selected(i) = whether exon i is included in optimal solution up to i
            Dim selected(n - 1) As Boolean

            ' Base case: first exon
            dp(0) = g.Exons(0).Score
            prev(0) = -1
            selected(0) = True

            For i = 1 To n - 1
                ' Option 1: don't include exon i
                Dim bestScore As Double = dp(i - 1)
                Dim bestPrev As Integer = -1
                Dim bestSelected As Boolean = False

                ' Option 2: include exon i, find best compatible predecessor
                Dim includeScore As Double = g.Exons(i).Score
                Dim bestCompatScore As Double = 0.0
                Dim bestCompatPrev As Integer = -1

                For j = i - 1 To 0 Step -1
                    ' Check compatibility: no overlap and reasonable intron length
                    Dim gapBp = g.Exons(i).DnaStart - g.Exons(j).DnaEnd - 1

                    If gapBp < 0 Then
                        ' Overlapping exons - skip
                        Continue For
                    End If

                    If gapBp > config.MaxIntronLength Then
                        ' Intron too long - skip
                        Continue For
                    End If

                    ' Gap cost: penalize unmapped amino acids in the intron
                    Dim gapCost = config.GapPenaltyLambda * (gapBp / 3.0)
                    Dim chainScore = dp(j) - gapCost

                    If chainScore > bestCompatScore Then
                        bestCompatScore = chainScore
                        bestCompatPrev = j
                    End If
                Next

                includeScore += bestCompatScore

                If includeScore > bestScore Then
                    dp(i) = includeScore
                    prev(i) = bestCompatPrev
                    selected(i) = True
                Else
                    dp(i) = bestScore
                    prev(i) = -1
                    selected(i) = False
                End If
            Next

            ' Traceback: find the optimal chain
            ' Find the position with maximum dp value
            Dim maxDpIdx As Integer = 0
            For i = 1 To n - 1
                If dp(i) > dp(maxDpIdx) Then maxDpIdx = i
            Next

            ' Reconstruct chain by backtracking
            Dim chain As New List(Of CandidateExon)
            Dim idx As Integer = maxDpIdx

            ' We need to trace which exons are actually selected
            ' Re-trace from the maximum dp position
            Dim includedSet As New HashSet(Of Integer)()
            TraceChain(n - 1, dp, prev, selected, includedSet)

            ' Build chain from included exons
            For i = 0 To n - 1
                If includedSet.Contains(i) Then
                    chain.Add(g.Exons(i))
                End If
            Next

            g.OptimalChain = chain
            g.ChainScore = If(chain.Count > 0, dp(maxDpIdx), 0)
            g.ChainEvalue = If(chain.Count > 0,
                chain.Min(Function(e) e.Evalue), Double.MaxValue)
        End Sub

        ''' <summary>Recursively trace the optimal chain through DP table</summary>
        Private Shared Sub TraceChain(
            i As Integer,
            dp() As Double,
            prev() As Integer,
            selected() As Boolean,
            included As HashSet(Of Integer))

            If i < 0 Then Return

            If selected(i) Then
                included.Add(i)
                If prev(i) >= 0 Then
                    TraceChain(prev(i), dp, prev, selected, included)
                End If
            Else
                ' This exon was not selected; the best chain up to i is same as up to i-1
                TraceChain(i - 1, dp, prev, selected, included)
            End If
        End Sub

    End Class

    ' ========================================================================
    ' MODULE 8: REDUNDANCY REMOVAL
    ' ========================================================================

    Public Class RedundancyReducer

        ''' <summary>
        ''' Remove redundant gene predictions by clustering TCS groups that
        ''' share exons on the same contig/strand, and selecting a representative.
        '''
        ''' Clustering criterion: same contig + same strand + at least one exon overlap
        ''' Representative selection: highest total score (or best E-value)
        ''' </summary>
        Public Shared Function Reduce(
            groups As List(Of TCSGroup),
            config As MetaEukConfig) As List(Of GenePrediction)

            Console.WriteLine("[INFO] Running redundancy removal...")

            ' Convert TCS groups to gene predictions
            Dim predictions As New List(Of GenePrediction)()
            Dim geneCounter As Integer = 0

            For Each g In groups
                If g.OptimalChain.Count = 0 Then Continue For

                geneCounter += 1
                Dim pred As New GenePrediction()
                pred.GeneID = $"gene_{geneCounter:D6}"
                pred.ContigID = g.ContigID
                pred.Strand = g.Strand
                pred.TargetID = g.TargetID
                pred.Exons = New List(Of CandidateExon)(g.OptimalChain)
                pred.TotalScore = g.ChainScore
                pred.BestEvalue = g.ChainEvalue
                predictions.Add(pred)
            Next

            Console.WriteLine($"[INFO] Initial predictions: {predictions.Count}")

            ' Cluster predictions on same contig+strand that share exons
            Dim clusters As New List(Of List(Of GenePrediction))()
            Dim assigned As New HashSet(Of Integer)()

            For i = 0 To predictions.Count - 1
                If assigned.Contains(i) Then Continue For

                Dim cluster As New List(Of GenePrediction)()
                cluster.Add(predictions(i))
                assigned.Add(i)

                ' Find all predictions that overlap with this cluster
                Dim expanded As Boolean = True
                While expanded
                    expanded = False
                    For j = 0 To predictions.Count - 1
                        If assigned.Contains(j) Then Continue For

                        ' Check if this prediction shares exons with any in the cluster
                        For Each cp In cluster
                            If SharesExons(predictions(j), cp, config) Then
                                cluster.Add(predictions(j))
                                assigned.Add(j)
                                expanded = True
                                Exit For
                            End If
                        Next
                    Next
                End While

                clusters.Add(cluster)
            Next

            ' Select representative from each cluster
            Dim representatives As New List(Of GenePrediction)()
            Dim clusterID As Integer = 0

            For Each cluster In clusters
                clusterID += 1

                ' Sort by total score descending, then by E-value ascending
                Dim sorted = cluster.OrderBy(Function(p) -p.TotalScore).ThenBy(Function(p) p.BestEvalue).ToList()

                ' Mark the best as representative
                sorted(0).IsRepresentative = True
                sorted(0).ClusterID = clusterID

                ' Mark others as redundant
                For k = 1 To sorted.Count - 1
                    sorted(k).IsRepresentative = False
                    sorted(k).ClusterID = clusterID
                Next

                representatives.Add(sorted(0))
            Next

            Console.WriteLine($"[INFO] After redundancy removal: {representatives.Count} representative predictions (from {predictions.Count} total)")

            ' Also store non-representative for reference
            For Each pred In predictions
                If pred.IsRepresentative AndAlso Not representatives.Contains(pred) Then
                    ' Already added
                ElseIf Not pred.IsRepresentative Then
                    ' Could optionally keep these
                End If
            Next

            Return representatives
        End Function

        ''' <summary>Check if two gene predictions share at least one overlapping exon</summary>
        Private Shared Function SharesExons(a As GenePrediction, b As GenePrediction, config As MetaEukConfig) As Boolean
            If a.ContigID <> b.ContigID Then Return False
            If a.Strand <> b.Strand Then Return False

            For Each ea In a.Exons
                For Each eb In b.Exons
                    Dim overlapLen = Math.Min(ea.DnaEnd, eb.DnaEnd) - Math.Max(ea.DnaStart, eb.DnaStart) + 1
                    If overlapLen > 0 Then
                        Dim minLen = Math.Min(ea.Length, eb.Length)
                        Dim overlapFrac = overlapLen / CDbl(minLen)
                        If overlapFrac >= config.MinExonOverlapFraction Then
                            Return True
                        End If
                    End If
                Next
            Next

            Return False
        End Function

    End Class

    ' ========================================================================
    ' MODULE 9: SAME-STRAND CONFLICT RESOLUTION
    ' ========================================================================

    Public Class ConflictResolver

        ''' <summary>
        ''' Resolve conflicts where predicted genes on the same strand overlap.
        ''' Keep the prediction with the better (lower) E-value; discard the other.
        ''' Repeat until no conflicts remain.
        ''' </summary>
        Public Shared Function Resolve(predictions As List(Of GenePrediction), config As MetaEukConfig) As List(Of GenePrediction)
            Console.WriteLine("[INFO] Running same-strand conflict resolution...")

            ' Group predictions by contig+strand
            Dim groups = predictions.GroupBy(Function(p) $"{p.ContigID}|{CStr(p.Strand)}").ToList()

            Dim resolved As New List(Of GenePrediction)()

            For Each grp In groups
                Dim strandPreds = grp.ToList()
                Dim kept = ResolveStrand(strandPreds, config)
                resolved.AddRange(kept)
            Next

            Console.WriteLine($"[INFO] After conflict resolution: {resolved.Count} predictions")
            Return resolved
        End Function

        ''' <summary>Resolve conflicts on a single contig+strand</summary>
        Private Shared Function ResolveStrand(preds As List(Of GenePrediction), config As MetaEukConfig) As List(Of GenePrediction)
            ' Sort by E-value (best first)
            Dim sorted = preds.OrderBy(Function(p) p.BestEvalue).ToList()
            Dim kept As New List(Of GenePrediction)()

            For Each pred In sorted
                Dim conflicts As Boolean = False
                For Each existing In kept
                    If Overlaps(pred, existing, config) Then
                        conflicts = True
                        Exit For
                    End If
                Next

                If Not conflicts Then
                    kept.Add(pred)
                End If
            Next

            Return kept
        End Function

        ''' <summary>Check if two gene predictions overlap on the genome</summary>
        Private Shared Function Overlaps(a As GenePrediction, b As GenePrediction, config As MetaEukConfig) As Boolean
            If a.ContigID <> b.ContigID Then Return False
            If a.Strand <> b.Strand Then Return False

            Dim overlapLen = Math.Min(a.DnaEnd, b.DnaEnd) - Math.Max(a.DnaStart, b.DnaStart) + 1
            Return overlapLen > config.OverlapBpThreshold
        End Function

    End Class

    ' ========================================================================
    ' MODULE 10: PROTEIN SEQUENCE RECONSTRUCTION
    ' ========================================================================

    Public Class ProteinReconstructor

        ''' <summary>
        ''' Reconstruct the predicted protein sequence from exon coordinates.
        ''' Translates the DNA subsequence for each exon and concatenates them.
        ''' </summary>
        Public Shared Sub ReconstructAll(predictions As List(Of GenePrediction), contigs As IEnumerable(Of FastaSeq))
            ' Build contig lookup
            Dim contigDict = contigs.ToDictionary(Function(c) c.locus_tag)

            For Each pred In predictions
                Dim protSB As New StringBuilder()

                If contigDict.ContainsKey(pred.ContigID) Then
                    Dim dna = contigDict(pred.ContigID).SequenceData.ToUpper()

                    ' Sort exons by position
                    pred.Exons.Sort(Function(a, b) a.DnaStart.CompareTo(b.DnaStart))

                    For Each exon In pred.Exons
                        Dim start0 = Math.Max(0, exon.DnaStart - 1)  ' convert to 0-based
                        Dim end0 = Math.Min(dna.Length - 1, exon.DnaEnd - 1)

                        If start0 > end0 OrElse start0 >= dna.Length Then Continue For

                        Dim exonDna = dna.Substring(start0, end0 - start0 + 1)

                        ' For minus strand, take reverse complement
                        If exon.Strand = StrandOrientation.Minus Then
                            exonDna = CodonTable.ReverseComplement(exonDna)
                        End If

                        ' Translate
                        Dim pep = CodonTable.Translate(exonDna, 0)
                        ' Remove trailing stop codon if present
                        If pep.Length > 0 AndAlso pep.EndsWith("*"c) Then
                            pep = pep.Substring(0, pep.Length - 1)
                        End If
                        protSB.Append(pep)
                    Next
                End If

                pred.ProteinSequence = protSB.ToString()
            Next
        End Sub

    End Class

    ' ========================================================================
    ' MODULE 11: OUTPUT WRITERS
    ' ========================================================================

    Public Class OutputWriter

        ''' <summary>Write protein sequences in FASTA format</summary>
        Public Shared Sub WriteProteinFasta(predictions As List(Of GenePrediction), filePath As String)
            Using writer As New System.IO.StreamWriter(filePath)
                For Each pred In predictions
                    If String.IsNullOrEmpty(pred.ProteinSequence) Then Continue For
                    writer.WriteLine($">{pred.GeneID} target={pred.TargetID} contig={pred.ContigID} strand={CStr(pred.Strand)} score={pred.TotalScore:F2} evalue={pred.BestEvalue:E2e} exons={pred.ExonCount}")
                    ' Write sequence in 80-char lines
                    Dim seq = pred.ProteinSequence
                    Dim pos As Integer = 0
                    While pos < seq.Length
                        Dim len = Math.Min(80, seq.Length - pos)
                        writer.WriteLine(seq.Substring(pos, len))
                        pos += len
                    End While
                Next
            End Using
            Console.WriteLine($"[INFO] Wrote protein FASTA: {filePath}")
        End Sub

        ''' <summary>Write gene models in GFF3 format</summary>
        Public Shared Sub WriteGFF3(predictions As List(Of GenePrediction), filePath As String)
            Using writer As New System.IO.StreamWriter(filePath)
                ' GFF3 header
                writer.WriteLine("##gff-version 3")
                writer.WriteLine("##feature-ontology https://github.com/The-Sequence-Ontology/SO-Ontologies/blob/v3.1/Features.md")
                writer.WriteLine("##source metaeuk-vb")

                For Each pred In predictions
                    Dim strandChar = If(pred.Strand = StrandOrientation.Plus, "+"c, "-"c)

                    ' Gene feature
                    writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}gene{vbTab}{pred.DnaStart}{vbTab}{pred.DnaEnd}{vbTab}{pred.TotalScore:F2}{vbTab}{strandChar}{vbTab}.{vbTab}ID={pred.GeneID};target={pred.TargetID};evalue={pred.BestEvalue:E2e}")

                    ' mRNA feature
                    Dim mrnaID = $"{pred.GeneID}.mRNA"
                    writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}mRNA{vbTab}{pred.DnaStart}{vbTab}{pred.DnaEnd}{vbTab}{pred.TotalScore:F2}{vbTab}{strandChar}{vbTab}.{vbTab}ID={mrnaID};Parent={pred.GeneID}")

                    ' Exon features
                    For exonIdx = 0 To pred.Exons.Count - 1
                        Dim exon = pred.Exons(exonIdx)
                        Dim exonID = $"{pred.GeneID}.exon{exonIdx + 1}"
                        writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}exon{vbTab}{exon.DnaStart}{vbTab}{exon.DnaEnd}{vbTab}{exon.Score:F2}{vbTab}{strandChar}{vbTab}.{vbTab}ID={exonID};Parent={mrnaID};target_align={exon.Hit.AlignStartTarget}-{exon.Hit.AlignEndTarget}")
                    Next

                    ' CDS features
                    For exonIdx = 0 To pred.Exons.Count - 1
                        Dim exon = pred.Exons(exonIdx)
                        Dim cdsID = $"{pred.GeneID}.cds{exonIdx + 1}"
                        writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}CDS{vbTab}{exon.DnaStart}{vbTab}{exon.DnaEnd}{vbTab}{exon.Score:F2}{vbTab}{strandChar}{vbTab}0{vbTab}ID={cdsID};Parent={mrnaID}")
                    Next
                Next
            End Using
            Console.WriteLine($"[INFO] Wrote GFF3: {filePath}")
        End Sub

        ''' <summary>Write summary in TSV format</summary>
        Public Shared Sub WriteTSV(predictions As List(Of GenePrediction), filePath As String)
            Using writer As New System.IO.StreamWriter(filePath)
                ' Header
                writer.WriteLine("gene_id" & vbTab & "contig" & vbTab & "strand" & vbTab &
                                 "start" & vbTab & "end" & vbTab & "length_bp" & vbTab &
                                 "exon_count" & vbTab & "target_id" & vbTab &
                                 "total_score" & vbTab & "best_evalue" & vbTab &
                                 "protein_length")

                For Each pred In predictions
                    Dim protLen = If(pred.ProteinSequence IsNot Nothing, pred.ProteinSequence.Length, 0)
                    writer.WriteLine($"{pred.GeneID}{vbTab}{pred.ContigID}{vbTab}{CStr(pred.Strand)}{vbTab}" &
                                     $"{pred.DnaStart}{vbTab}{pred.DnaEnd}{vbTab}{pred.DnaEnd - pred.DnaStart + 1}{vbTab}" &
                                     $"{pred.ExonCount}{vbTab}{pred.TargetID}{vbTab}" &
                                     $"{pred.TotalScore:F2}{vbTab}{pred.BestEvalue:E2e}{vbTab}{protLen}")
                Next
            End Using
            Console.WriteLine($"[INFO] Wrote TSV summary: {filePath}")
        End Sub

    End Class

    ' ========================================================================
    ' MODULE 12: COMMAND-LINE PARSER
    ' ========================================================================

    Public Class CommandLineParser

        Public Shared Function Parse(args As String()) As MetaEukConfig
            Dim config As New MetaEukConfig()

            Dim i As Integer = 0
            While i < args.Length
                Select Case args(i).ToLower()
                    Case "--contigs", "-c"
                        i += 1
                        If i < args.Length Then config.ContigsFile = args(i)

                    Case "--reference", "-r"
                        i += 1
                        If i < args.Length Then config.ReferenceFile = args(i)

                    Case "--output", "-o"
                        i += 1
                        If i < args.Length Then config.OutputPrefix = args(i)

                    Case "--evalue", "-e"
                        i += 1
                        If i < args.Length Then Double.TryParse(args(i), config.EvalueThreshold)

                    Case "--min-identity"
                        i += 1
                        If i < args.Length Then Double.TryParse(args(i), config.MinIdentity)

                    Case "--min-fragment-length"
                        i += 1
                        If i < args.Length Then Integer.TryParse(args(i), config.MinFragmentLength)

                    Case "--gap-penalty"
                        i += 1
                        If i < args.Length Then Double.TryParse(args(i), config.GapPenaltyLambda)

                    Case "--max-intron"
                        i += 1
                        If i < args.Length Then Integer.TryParse(args(i), config.MaxIntronLength)

                    Case "--min-exon-score"
                        i += 1
                        If i < args.Length Then Double.TryParse(args(i), config.MinExonScore)

                    Case "--overlap-threshold"
                        i += 1
                        If i < args.Length Then Integer.TryParse(args(i), config.OverlapBpThreshold)

                    Case "--exon-overlap-fraction"
                        i += 1
                        If i < args.Length Then Double.TryParse(args(i), config.MinExonOverlapFraction)

                    Case "--verbose", "-v"
                        config.Verbose = True

                    Case "--help", "-h"
                        PrintUsage()
                        End

                    Case Else
                        Console.Error.WriteLine($"[WARN] Unknown option: {args(i)}")
                End Select
                i += 1
            End While

            Return config
        End Function

        Public Shared Sub PrintUsage()
            Console.WriteLine("MetaEukVB - Eukaryotic Gene Prediction Tool (VB.NET)")
            Console.WriteLine("Based on MetaEuk algorithm: homology-based exon chain optimization")
            Console.WriteLine()
            Console.WriteLine("USAGE:")
            Console.WriteLine("  metaeuk-vb --contigs <contigs.fasta> --reference <proteins.fasta> [options]")
            Console.WriteLine()
            Console.WriteLine("REQUIRED ARGUMENTS:")
            Console.WriteLine("  -c, --contigs <file>       Input contigs in FASTA format")
            Console.WriteLine("  -r, --reference <file>     Reference protein database in FASTA format")
            Console.WriteLine()
            Console.WriteLine("OUTPUT OPTIONS:")
            Console.WriteLine("  -o, --output <prefix>      Output file prefix (default: metaeuk_out)")
            Console.WriteLine("                             Generates: <prefix>.faa, <prefix>.gff3, <prefix>.tsv")
            Console.WriteLine()
            Console.WriteLine("ALGORITHM PARAMETERS:")
            Console.WriteLine("  -e, --evalue <float>       E-value threshold (default: 1e-3)")
            Console.WriteLine("  --min-identity <float>     Minimum sequence identity fraction (default: 0.2)")
            Console.WriteLine("  --min-fragment-length <int> Minimum candidate fragment length in AA (default: 15)")
            Console.WriteLine("  --gap-penalty <float>      Gap penalty coefficient lambda (default: 0.5)")
            Console.WriteLine("  --max-intron <int>         Maximum intron length in bp (default: 50000)")
            Console.WriteLine("  --min-exon-score <float>   Minimum exon bitscore (default: 20.0)")
            Console.WriteLine("  --overlap-threshold <int>  Overlap bp for conflict detection (default: 10)")
            Console.WriteLine("  --exon-overlap-fraction <float> Exon overlap fraction for redundancy (default: 0.3)")
            Console.WriteLine()
            Console.WriteLine("OTHER OPTIONS:")
            Console.WriteLine("  -v, --verbose              Enable verbose output")
            Console.WriteLine("  -h, --help                 Show this help message")
            Console.WriteLine()
            Console.WriteLine("ALGORITHM PIPELINE:")
            Console.WriteLine("  1. Six-frame translation of contigs -> candidate coding fragments")
            Console.WriteLine("  2. Smith-Waterman local alignment against reference proteins")
            Console.WriteLine("  3. Group hits by (Target, Contig, Strand) -> TCS groups")
            Console.WriteLine("  4. Dynamic programming: optimal exon chain per TCS group")
            Console.WriteLine("  5. Redundancy removal: cluster overlapping predictions, pick representative")
            Console.WriteLine("  6. Same-strand conflict resolution: keep best E-value")
            Console.WriteLine("  7. Output: Protein FASTA (.faa), GFF3 (.gff3), TSV summary (.tsv)")
        End Sub

    End Class

    ' ========================================================================
    ' MAIN ENTRY POINT
    ' ========================================================================

    Sub Main(args As String())
        Console.WriteLine("============================================================")
        Console.WriteLine("  MetaEukVB - Eukaryotic Gene Prediction Tool (VB.NET)")
        Console.WriteLine("  Based on MetaEuk algorithm: homology-based exon chain DP")
        Console.WriteLine("============================================================")
        Console.WriteLine()

        ' Parse command line
        Dim config = CommandLineParser.Parse(args)

        ' Validate required arguments
        If String.IsNullOrEmpty(config.ContigsFile) OrElse String.IsNullOrEmpty(config.ReferenceFile) Then
            Console.Error.WriteLine("[ERROR] Both --contigs and --reference are required.")
            Console.Error.WriteLine("        Use --help for usage information.")
            End
        End If

        Dim startTime = DateTime.Now

        ' ----------------------------------------------------------
        ' STEP 1: Read input FASTA files
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 1] Reading input files...")
        Dim contigs = FastaFile.Read(config.ContigsFile)
        Dim references = FastaFile.Read(config.ReferenceFile)
        Console.WriteLine($"  Contigs: {contigs.Count} sequences, total {contigs.Sum(Function(c) c.SequenceData.Length):N0} bp")
        Console.WriteLine($"  References: {references.Count} protein sequences")

        If contigs.Count = 0 Then
            Console.Error.WriteLine("[ERROR] No contigs found. Check input file.")
            End
        End If
        If references.Count = 0 Then
            Console.Error.WriteLine("[ERROR] No reference proteins found. Check reference file.")
            End
        End If

        ' ----------------------------------------------------------
        ' STEP 2: Six-frame translation & candidate fragment generation
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 2] Six-frame translation and candidate fragment generation...")
        Dim allFragments As New List(Of CandidateFragment)
        For Each contig In contigs
            Dim frags = SixFrameTranslator.GenerateFragments(contig, config)
            allFragments.AddRange(frags)
            If config.Verbose Then
                Console.WriteLine($"  {contig.locus_tag}: {frags.Count} candidate fragments")
            End If
        Next
        Console.WriteLine($"  Total candidate fragments: {allFragments.Count}")

        ' ----------------------------------------------------------
        ' STEP 3: Homology search (Smith-Waterman alignment)
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 3] Homology search against reference proteins...")
        Dim hits = HomologySearchEngine.SearchAll(allFragments, references.ToList, config)
        Console.WriteLine($"  Significant hits: {hits.Count}")

        If hits.Count = 0 Then
            Console.WriteLine("[WARN] No significant homology hits found. Try relaxing E-value threshold.")
            Console.WriteLine("       Writing empty output files...")

            ' Write empty output files
            OutputWriter.WriteProteinFasta(New List(Of GenePrediction), $"{config.OutputPrefix}.faa")
            OutputWriter.WriteGFF3(New List(Of GenePrediction), $"{config.OutputPrefix}.gff3")
            OutputWriter.WriteTSV(New List(Of GenePrediction), $"{config.OutputPrefix}.tsv")
            End
        End If

        ' ----------------------------------------------------------
        ' STEP 4: TCS grouping (Target-Contig-Strand)
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 4] Grouping hits by Target-Contig-Strand...")
        Dim tcsGroups = TCSGrouper.GroupHits(hits, config)
        Console.WriteLine($"  TCS groups: {tcsGroups.Count}")

        ' ----------------------------------------------------------
        ' STEP 5: Dynamic programming - optimal exon chain
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 5] Dynamic programming: optimal exon chain selection...")
        ExonChainOptimizer.OptimizeAll(tcsGroups, config)

        ' ----------------------------------------------------------
        ' STEP 6: Redundancy removal
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 6] Redundancy removal...")
        Dim predictions = RedundancyReducer.Reduce(tcsGroups, config)

        ' ----------------------------------------------------------
        ' STEP 7: Same-strand conflict resolution
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 7] Same-strand conflict resolution...")
        predictions = ConflictResolver.Resolve(predictions, config)

        ' ----------------------------------------------------------
        ' STEP 8: Reconstruct protein sequences
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 8] Reconstructing protein sequences...")
        ProteinReconstructor.ReconstructAll(predictions, contigs)

        ' ----------------------------------------------------------
        ' STEP 9: Write output files
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 9] Writing output files...")
        OutputWriter.WriteProteinFasta(predictions, $"{config.OutputPrefix}.faa")
        OutputWriter.WriteGFF3(predictions, $"{config.OutputPrefix}.gff3")
        OutputWriter.WriteTSV(predictions, $"{config.OutputPrefix}.tsv")

        ' ----------------------------------------------------------
        ' Summary
        ' ----------------------------------------------------------
        Dim elapsed = DateTime.Now - startTime
        Console.WriteLine()
        Console.WriteLine("============================================================")
        Console.WriteLine("  PREDICTION SUMMARY")
        Console.WriteLine("============================================================")
        Console.WriteLine($"  Total gene predictions:  {predictions.Count}")
        Console.WriteLine($"  Total exons:             {predictions.Sum(Function(p) p.ExonCount)}")
        Console.WriteLine($"  Avg exons per gene:      {If(predictions.Count > 0, predictions.Average(Function(p) p.ExonCount).ToString("F1"), "N/A")}")
        Console.WriteLine($"  Avg protein length:      {If(predictions.Count > 0, predictions.Average(Function(p) If(p.ProteinSequence?.Length, 0)).ToString("F0"), "N/A")} AA")
        Console.WriteLine($"  Elapsed time:            {elapsed.TotalSeconds:F1} seconds")
        Console.WriteLine()
        Console.WriteLine($"  Output files:")
        Console.WriteLine($"    Protein FASTA:  {config.OutputPrefix}.faa")
        Console.WriteLine($"    Gene models:    {config.OutputPrefix}.gff3")
        Console.WriteLine($"    Summary table:  {config.OutputPrefix}.tsv")
        Console.WriteLine("============================================================")
    End Sub

End Module
