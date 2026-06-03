
' ========================================================================
' MODULE 4: SMITH-WATERMAN LOCAL ALIGNMENT (Homology Search)
' ========================================================================

Imports System.Text

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
