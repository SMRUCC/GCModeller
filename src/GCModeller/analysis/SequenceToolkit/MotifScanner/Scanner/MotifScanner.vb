' ============================================================================
'  MotifScanner.vb
'  ============================================================================
'  Purpose:
'     Algorithm module for scanning biological sequences with a MotifPWM
'     (Position Weight Matrix) to identify candidate TFBS
'     (Transcription Factor Binding Site) loci.
'
'  Scoring model:
'     Log-odds (base 2) of PWM probability vs. background probability.
'         score(window) = SUM_i  log2( (PWM[i][base_i] + pseudo) /
'                                       (bg[base_i]          + pseudo) )
'
'  P-value model:
'     Exact tail probability P(score >= observed) under the null model where
'     each position is drawn independently from the background distribution.
'     Computed by dynamic-programming convolution of the per-position score
'     distributions (discretized onto a fixed number of bins).
'
'  Strand handling:
'     Forward strand is always scanned. The reverse complement of the input
'     sequence is optionally scanned with the same motif; matches are mapped
'     back to forward-strand coordinates and tagged with strand = '-'.
'
'  Author: Qingyan Agent
'  ============================================================================

Imports System.IO
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif

' ========================================================================
'  Background model
' ========================================================================

''' <summary>
''' Background nucleotide frequency model used for log-odds scoring and
''' null-distribution construction.
''' </summary>
<Serializable>
Public Class BackgroundModel

    Public Property A As Double = 0.25
    Public Property C As Double = 0.25
    Public Property G As Double = 0.25
    Public Property T As Double = 0.25

    ''' <summary>Frequency of a base; 0 for unknown characters.</summary>
    Public Function Frequency(base As Char) As Double
        Select Case Char.ToUpper(base)
            Case "A"c : Return A
            Case "C"c : Return C
            Case "G"c : Return G
            Case "T"c, "U"c : Return T
            Case Else : Return 0.0
        End Select
    End Function

    ''' <summary>Build a uniform background (A=C=G=T=0.25).</summary>
    Public Shared Function Uniform() As BackgroundModel
        Return New BackgroundModel()
    End Function

    ''' <summary>Build a background from a sequence's nucleotide composition.</summary>
    Public Shared Function FromSequence(seq As String) As BackgroundModel
        Dim a As Long = 0, c As Long = 0, g As Long = 0, t As Long = 0
        For Each ch As Char In seq
            Select Case Char.ToUpper(ch)
                Case "A"c : a += 1
                Case "C"c : c += 1
                Case "G"c : g += 1
                Case "T"c, "U"c : t += 1
            End Select
        Next
        Dim total As Double = a + c + g + t
        If total = 0 Then Return Uniform()
        Return New BackgroundModel With {
            .A = a / total, .C = c / total,
            .G = g / total, .T = t / total
        }
    End Function

    Public Sub Validate()
        Dim s As Double = A + C + G + T
        If Math.Abs(s - 1.0) > 0.001 Then
            Throw New InvalidOperationException(
                String.Format("Background frequencies must sum to 1.0 (got {0}).", s))
        End If
        If A < 0 OrElse C < 0 OrElse G < 0 OrElse T < 0 Then
            Throw New InvalidOperationException(
                "Background frequencies must be non-negative.")
        End If
    End Sub

End Class

' ========================================================================
'  Null score distribution (for exact p-value via DP)
' ========================================================================

''' <summary>
''' Discretized probability distribution of the log-odds score under the
''' null (background) model. Built once per motif, then queried for each
''' observed score to obtain a p-value.
''' </summary>
Public Class ScoreDistribution

    ''' <summary>Inclusive lower bound of the binned score range.</summary>
    Public Property MinScore As Double

    ''' <summary>Inclusive upper bound of the binned score range.</summary>
    Public Property MaxScore As Double

    ''' <summary>Width of each bin.</summary>
    Public Property BinWidth As Double

    ''' <summary>
    ''' Probability mass in each bin; Bins[i] covers
    ''' [MinScore + i*BinWidth, MinScore + (i+1)*BinWidth).
    ''' </summary>
    Public Property Bins As Double()

    ''' <summary>Map a score to its bin index (clamped to valid range).</summary>
    Public Function BinIndex(score As Double) As Integer
        Dim idx As Integer = CInt(Math.Floor((score - MinScore) / BinWidth))
        If idx < 0 Then idx = 0
        If idx >= Bins.Length Then idx = Bins.Length - 1
        Return idx
    End Function

    ''' <summary>
    ''' Upper-tail p-value: P(score >= threshold) under the null model.
    ''' </summary>
    Public Function PValueGE(threshold As Double) As Double
        If threshold <= MinScore Then Return 1.0
        If threshold > MaxScore Then Return 0.0
        Dim startIdx As Integer = BinIndex(threshold)
        Dim p As Double = 0.0
        For i As Integer = startIdx To Bins.Length - 1
            p += Bins(i)
        Next
        If p > 1.0 Then p = 1.0
        If p < 0.0 Then p = 0.0
        Return p
    End Function

    ''' <summary>
    ''' Lower-tail p-value: P(score &lt;= threshold) under the null model.
    ''' Useful for detecting exceptionally poor matches.
    ''' </summary>
    Public Function PValueLE(threshold As Double) As Double
        If threshold < MinScore Then Return 0.0
        If threshold >= MaxScore Then Return 1.0
        Dim endIdx As Integer = BinIndex(threshold)
        Dim p As Double = 0.0
        For i As Integer = 0 To endIdx
            p += Bins(i)
        Next
        If p > 1.0 Then p = 1.0
        If p < 0.0 Then p = 0.0
        Return p
    End Function

    ''' <summary>
    ''' Upper-tail inverse CDF (quantile). Returns the score threshold t such
    ''' that P(score &gt;= t) = tailProbability under the null distribution.
    ''' Scores at or above t lie in the most extreme top tailProbability
    ''' fraction of the null model and can be used as a statistically
    ''' grounded minimum-score cutoff (e.g. Quantile(0.05) keeps only the
    ''' top 5% of background scores).
    ''' </summary>
    ''' <param name="tailProbability">Desired upper-tail mass in (0, 1).</param>
    Public Function Quantile(tailProbability As Double) As Double
        If tailProbability <= 0.0 Then Return MaxScore
        If tailProbability >= 1.0 Then Return MinScore

        ' Walk bins from the high-score end, accumulating mass until we
        ' reach the requested upper-tail probability. The left edge of the
        ' bin where the cumulative is first met is the threshold.
        Dim cum As Double = 0.0
        For i As Integer = Bins.Length - 1 To 0 Step -1
            cum += Bins(i)
            If cum >= tailProbability Then
                Return MinScore + i * BinWidth
            End If
        Next
        Return MinScore
    End Function

End Class

' ========================================================================
'  Scanner
' ========================================================================

''' <summary>
''' Scans biological sequences with motif PWMs to identify candidate TFBS
''' loci. Each candidate carries a log-odds score and an exact null-model
''' p-value computed by dynamic programming.
''' </summary>
Public Class MotifScanner

    Private ReadOnly _background As BackgroundModel
    Private ReadOnly _pseudocount As Double
    Private ReadOnly _numBins As Integer

    ' Cache of (motif -> null distribution) so repeated scans with the same
    ' motif do not recompute the DP.
    Private ReadOnly _distCache As New Dictionary(Of String, ScoreDistribution)

    ' --------------------------------------------------------------------
    '  Construction
    ' --------------------------------------------------------------------

    ''' <summary>Construct a scanner with uniform background.</summary>
    Public Sub New()
        Me.New(BackgroundModel.Uniform(), 0.0001, 10000)
    End Sub

    ''' <summary>Construct a scanner with a custom background model.</summary>
    ''' <param name="background">Nucleotide background frequencies (must sum to 1).</param>
    ''' <param name="pseudocount">Pseudocount added to every PWM cell to avoid log(0).</param>
    ''' <param name="numBins">Number of bins for score-distribution discretization.</param>
    Public Sub New(background As BackgroundModel,
                   Optional pseudocount As Double = 0.0001,
                   Optional numBins As Integer = 10000)
        If background Is Nothing Then Throw New ArgumentNullException(NameOf(background))
        background.Validate()
        If pseudocount < 0 Then Throw New ArgumentOutOfRangeException(NameOf(pseudocount))
        If numBins < 100 Then Throw New ArgumentOutOfRangeException(NameOf(numBins))

        _background = background
        _pseudocount = pseudocount
        _numBins = numBins
    End Sub

    Public ReadOnly Property Background As BackgroundModel
        Get
            Return _background
        End Get
    End Property

    ' --------------------------------------------------------------------
    '  Public API
    ' --------------------------------------------------------------------

    ''' <summary>
    ''' Scan a sequence with a single motif and return all TFBS matches
    ''' satisfying both the score and p-value thresholds.
    ''' </summary>
    ''' <param name="motif">The motif PWM to scan with.</param>
    ''' <param name="sequence">The biological sequence (DNA) to scan.</param>
    ''' <param name="scoreThreshold">Minimum log-odds score to report (default: no filter).</param>
    ''' <param name="pValueThreshold">Maximum p-value to report (default: no filter).</param>
    ''' <param name="scanReverseStrand">If True, also scan the reverse complement strand.</param>
    ''' <returns>List of matches, sorted by p-value ascending.</returns>
    Public Function Scan(motif As MotifPWM,
                         sequence As String,
                         Optional scoreThreshold As Double = Double.NegativeInfinity,
                         Optional pValueThreshold As Double = Double.PositiveInfinity,
                         Optional scanReverseStrand As Boolean = True,
                         Optional topN As Integer = Integer.MaxValue) As List(Of MotifMatch)

        Dim results As New List(Of MotifMatch)()
        If motif Is Nothing OrElse motif.pwm Is Nothing OrElse motif.pwm.Length = 0 Then Return results
        If String.IsNullOrEmpty(sequence) Then Return results

        Dim motifLen As Integer = motif.pwm.Length
        If sequence.Length < motifLen Then Return results

        ' 1. Build log-odds matrix [position, alphabet-index]
        Dim logOdds As Double(,) = BuildLogOddsMatrix(motif)

        ' 2. Get (or compute) the null score distribution for this motif
        Dim dist As ScoreDistribution = GetOrComputeDistribution(motif, logOdds)

        ' 3. Forward strand
        ScanOneStrand(motif, sequence, logOdds, dist, "+"c,
                      scoreThreshold, pValueThreshold, results)

        ' 4. Reverse complement strand
        If scanReverseStrand Then
            Dim rcSeq As String = ReverseComplement(sequence)
            ScanOneStrand(motif, rcSeq, logOdds, dist, "-"c,
                          scoreThreshold, pValueThreshold, results)
        End If

        ' 5. Sort by p-value ascending (most significant first)
        ' score1 is negative value possible, but this is common situtation
        ' we should just make filter by pvalue
        Call results.Sort(Function(a, b) a.pvalue.CompareTo(b.pvalue))

        If topN <> Integer.MaxValue AndAlso topN <> Integer.MinValue Then
            results = FilterByScore(results, topN, dist)
        End If

        Return results
    End Function

    ''' <summary>
    ''' Scan a sequence with multiple motifs. Returns all matches from all
    ''' motifs, sorted by p-value ascending.
    ''' </summary>
    Public Function ScanMultiple(motifs As IEnumerable(Of MotifPWM),
                                 sequence As String,
                                 Optional scoreThreshold As Double = Double.NegativeInfinity,
                                 Optional pValueThreshold As Double = Double.PositiveInfinity,
                                 Optional scanReverseStrand As Boolean = True) As List(Of MotifMatch)
        Dim all As New List(Of MotifMatch)()
        If motifs Is Nothing Then Return all
        For Each m As MotifPWM In motifs
            all.AddRange(Scan(m, sequence, scoreThreshold, pValueThreshold, scanReverseStrand))
        Next
        all.Sort(Function(a, b) a.pvalue.CompareTo(b.pvalue))
        Return all
    End Function

    ' --------------------------------------------------------------------
    '  Result filtering by score
    ' --------------------------------------------------------------------

    ''' <summary>
    ''' Filter a list of TFBS matches by a computed quality score, keeping
    ''' the top-N matches with the highest score (higher = better).
    ''' The list is sorted by score descending before truncation.
    ''' </summary>
    ''' <param name="matches">Candidate matches (e.g. from Scan / ScanMultiple).</param>
    ''' <param name="topN">Maximum number of matches to keep.</param>
    ''' <param name="scoreFunc">
    '''   Optional function mapping a match to its quality score. If omitted,
    '''   a default composite score is used:
    '''       score1 + score2 - log10(pvalue)
    '''   where score1 is the log-odds score, score2 is the information
    '''   content (bits), and a smaller p-value yields a larger (more
    '''   significant) contribution. Higher scores are considered better.
    ''' </param>
    ''' <returns>A new list containing the top-N matches, sorted by score descending.</returns>
    Public Shared Function FilterByScore(matches As IEnumerable(Of MotifMatch),
                                         topN As Integer,
                                         Optional scoreFunc As Func(Of MotifMatch, Double) = Nothing) As List(Of MotifMatch)
        Return FilterByScore(matches, topN, Nothing, 0.05, scoreFunc)
    End Function

    ''' <summary>
    ''' Filter a list of TFBS matches using a null-distribution-derived score
    ''' cutoff, then keep the top-N by a computed quality score.
    '''
    ''' A minimum-score cutoff is derived from <paramref name="distribution"/>
    ''' via its upper-tail quantile at <paramref name="significanceLevel"/>
    ''' (e.g. 0.05 yields the score below which only 5% of the null model
    ''' falls). Matches whose score1 is below this cutoff are discarded as
    ''' not significant; the survivors are then sorted by the quality score
    ''' (higher = better) and the top-N are returned.
    ''' </summary>
    ''' <param name="matches">Candidate matches (e.g. from Scan / ScanMultiple).</param>
    ''' <param name="topN">Maximum number of matches to keep.</param>
    ''' <param name="distribution">
    '''   The motif's null score distribution (from GetNullDistribution). If
    '''   Nothing, no distribution-based cutoff is applied (pure ranking).
    ''' </param>
    ''' <param name="significanceLevel">Upper-tail mass for the cutoff, in (0,1).</param>
    ''' <param name="scoreFunc">Optional quality-score function (see other overload).</param>
    ''' <returns>Top-N significant matches sorted by quality score descending.</returns>
    Public Shared Function FilterByScore(matches As IEnumerable(Of MotifMatch),
                                         topN As Integer,
                                         distribution As ScoreDistribution,
                                         Optional significanceLevel As Double = 0.05,
                                         Optional scoreFunc As Func(Of MotifMatch, Double) = Nothing) As List(Of MotifMatch)
        If matches Is Nothing Then Return New List(Of MotifMatch)()
        If topN <= 0 Then Return New List(Of MotifMatch)()

        ' Derive a statistically grounded minimum-score cutoff from the
        ' null distribution, if one was supplied.
        Dim minScore As Double = Double.NegativeInfinity
        If distribution IsNot Nothing AndAlso
           significanceLevel > 0.0 AndAlso significanceLevel < 1.0 Then
            minScore = distribution.Quantile(significanceLevel)
        End If

        Dim scorer As Func(Of MotifMatch, Double)
        If scoreFunc Is Nothing Then
            ' Default composite quality score (higher = better).
            scorer = Function(m As MotifMatch)
                         Dim pval As Double = If(m.pvalue > 0, m.pvalue, Double.Epsilon)
                         Dim significance As Double = -Math.Log10(pval)
                         Return m.score1 + m.score2 + significance
                     End Function
        Else
            scorer = scoreFunc
        End If

        Dim query = matches.Where(Function(m) m IsNot Nothing)
        If minScore > Double.NegativeInfinity Then
            query = query.Where(Function(m) m.score1 >= minScore)
        End If

        Return query _
            .OrderByDescending(Function(m) scorer(m)) _
            .Take(topN) _
            .ToList()
    End Function

    ''' <summary>
    ''' Compute (or retrieve from cache) the null score distribution for a
    ''' motif. The returned distribution can be used to derive statistically
    ''' grounded score cutoffs, e.g. via ScoreDistribution.Quantile.
    ''' </summary>
    ''' <param name="motif">The motif PWM.</param>
    ''' <returns>The null score distribution, or Nothing if the motif is invalid.</returns>
    Public Function GetNullDistribution(motif As MotifPWM) As ScoreDistribution
        If motif Is Nothing OrElse motif.pwm Is Nothing OrElse motif.pwm.Length = 0 Then
            Return Nothing
        End If
        Dim logOdds As Double(,) = BuildLogOddsMatrix(motif)
        Return GetOrComputeDistribution(motif, logOdds)
    End Function

    ''' <summary>
    ''' Score a specific window of a sequence against the motif.
    ''' Returns the log-odds score, or Double.NaN if the window contains
    ''' characters outside the motif alphabet.
    ''' </summary>
    Public Function ScoreWindow(motif As MotifPWM, window As String) As Double
        If motif Is Nothing OrElse motif.pwm Is Nothing Then Return Double.NaN
        If window Is Nothing OrElse window.Length <> motif.pwm.Length Then Return Double.NaN

        Dim logOdds As Double(,) = BuildLogOddsMatrix(motif)
        Dim alphaIndex As Dictionary(Of Char, Integer) = BuildAlphabetIndex(motif)

        Dim score As Double = 0.0
        For k As Integer = 0 To window.Length - 1
            Dim j As Integer
            If Not alphaIndex.TryGetValue(Char.ToUpper(window(k)), j) Then
                Return Double.NaN
            End If
            score += logOdds(k, j)
        Next
        Return score
    End Function

    ''' <summary>
    ''' Compute the p-value for a given score against the motif's null
    ''' distribution. Useful for evaluating scores obtained externally.
    ''' </summary>
    Public Function PValueForScore(motif As MotifPWM, score As Double) As Double
        If motif Is Nothing OrElse motif.pwm Is Nothing OrElse motif.pwm.Length = 0 Then
            Return Double.NaN
        End If
        Dim logOdds As Double(,) = BuildLogOddsMatrix(motif)
        Dim dist As ScoreDistribution = GetOrComputeDistribution(motif, logOdds)
        Return dist.PValueGE(score)
    End Function

    ' --------------------------------------------------------------------
    '  Matrix construction
    ' --------------------------------------------------------------------

    ''' <summary>
    ''' Build a log-odds matrix [position, alphabet-index] from the motif PWM.
    ''' Each cell is:
    '''     log2( (PWM[i][j] + pseudo) / (bg[alphabet[j]] + pseudo) )
    ''' The pseudocount avoids log(0) when a PWM cell or background is zero.
    ''' </summary>
    Private Function BuildLogOddsMatrix(motif As MotifPWM) As Double(,)
        Dim motifLen As Integer = motif.pwm.Length
        Dim alphaLen As Integer = motif.alphabets.Length
        Dim m As Double(,) = New Double(motifLen - 1, alphaLen - 1) {}

        For i As Integer = 0 To motifLen - 1
            Dim pwmRow As Double() = motif.pwm(i).PWM
            If pwmRow Is Nothing OrElse pwmRow.Length <> alphaLen Then
                Throw New InvalidDataException(
                    String.Format("ResidueSite {0}: PWM row length ({1}) does not match alphabets length ({2}).",
                                  i, If(pwmRow Is Nothing, 0, pwmRow.Length), alphaLen))
            End If
            For j As Integer = 0 To alphaLen - 1
                Dim p As Double = pwmRow(j) + _pseudocount
                Dim bg As Double = _background.Frequency(motif.alphabets(j)) + _pseudocount
                m(i, j) = Math.Log(p / bg, 2.0)
            Next
        Next
        Return m
    End Function

    ''' <summary>
    ''' Build a case-insensitive char -> alphabet-index lookup for the motif.
    ''' </summary>
    Private Function BuildAlphabetIndex(motif As MotifPWM) As Dictionary(Of Char, Integer)
        Dim alphaLen As Integer = motif.alphabets.Length
        Dim d As New Dictionary(Of Char, Integer)(alphaLen)
        For j As Integer = 0 To alphaLen - 1
            d(Char.ToUpper(motif.alphabets(j))) = j
        Next
        Return d
    End Function

    ' --------------------------------------------------------------------
    '  Null score distribution (DP convolution)
    ' --------------------------------------------------------------------

    ''' <summary>
    ''' Return the cached null distribution for a motif, or compute and
    ''' cache it. The cache key is the motif name (falls back to a hash of
    ''' the PWM if the name is empty).
    ''' </summary>
    Private Function GetOrComputeDistribution(motif As MotifPWM,
                                              logOdds As Double(,)) As ScoreDistribution
        Dim key As String = If(String.IsNullOrEmpty(motif.name),
                               "motif_" & motif.GetHashCode().ToString(),
                               motif.name)
        Dim dist As ScoreDistribution = Nothing
        If _distCache.TryGetValue(key, dist) Then Return dist

        dist = ComputeNullDistribution(motif, logOdds)
        _distCache(key) = dist
        Return dist
    End Function

    ''' <summary>
    ''' Compute the exact null distribution of the log-odds score by
    ''' dynamic programming.
    '''
    ''' At each motif position i, the per-position score contribution is a
    ''' random variable taking value logOdds[i][j] with probability
    ''' bg[alphabet[j]]. The total score is the sum of these independent
    ''' per-position contributions; its distribution is the convolution of
    ''' the per-position distributions, computed iteratively on a
    ''' discretized score grid.
    ''' </summary>
    Private Function ComputeNullDistribution(motif As MotifPWM,
                                             logOdds As Double(,)) As ScoreDistribution
        Dim motifLen As Integer = motif.pwm.Length
        Dim alphaLen As Integer = motif.alphabets.Length

        ' 1. Determine the global min/max possible score
        Dim globalMin As Double = 0.0
        Dim globalMax As Double = 0.0
        For i As Integer = 0 To motifLen - 1
            Dim rowMin As Double = Double.PositiveInfinity
            Dim rowMax As Double = Double.NegativeInfinity
            For j As Integer = 0 To alphaLen - 1
                If logOdds(i, j) < rowMin Then rowMin = logOdds(i, j)
                If logOdds(i, j) > rowMax Then rowMax = logOdds(i, j)
            Next
            globalMin += rowMin
            globalMax += rowMax
        Next

        ' Guard against degenerate (zero-width) ranges
        If Math.Abs(globalMax - globalMin) < 0.000000000001 Then
            globalMax = globalMin + 1.0
        End If

        Dim binWidth As Double = (globalMax - globalMin) / _numBins
        Dim nBins As Integer = _numBins + 1

        Dim dist As New ScoreDistribution With {
            .MinScore = globalMin,
            .MaxScore = globalMax,
            .BinWidth = binWidth,
            .Bins = New Double(nBins - 1) {}
        }

        ' 2. DP: current distribution over binned scores.
        '    Start: score 0 with probability 1.
        Dim cur As Double() = New Double(nBins - 1) {}
        cur(dist.BinIndex(0.0)) = 1.0

        For i As Integer = 0 To motifLen - 1
            Dim nxt As Double() = New Double(nBins - 1) {}
            ' Precompute per-position (score, prob) pairs to avoid repeated
            ' background lookups in the inner loop.
            Dim contribs As New List(Of Tuple(Of Double, Double))(alphaLen)
            For j As Integer = 0 To alphaLen - 1
                Dim bgFreq As Double = _background.Frequency(motif.alphabets(j))
                If bgFreq <= 0 Then Continue For
                contribs.Add(Tuple.Create(logOdds(i, j), bgFreq))
            Next

            For b As Integer = 0 To nBins - 1
                Dim probHere As Double = cur(b)
                If probHere = 0.0 Then Continue For
                Dim s As Double = globalMin + b * binWidth
                For Each c As Tuple(Of Double, Double) In contribs
                    Dim newScore As Double = s + c.Item1
                    Dim newB As Integer = dist.BinIndex(newScore)
                    nxt(newB) += probHere * c.Item2
                Next
            Next
            cur = nxt
        Next

        dist.Bins = cur
        Return dist
    End Function

    ' --------------------------------------------------------------------
    '  Strand scanning
    ' --------------------------------------------------------------------

    Private Sub ScanOneStrand(motif As MotifPWM,
                              sequence As String,
                              logOdds As Double(,),
                              dist As ScoreDistribution,
                              strand As Char,
                              scoreThreshold As Double,
                              pValueThreshold As Double,
                              results As List(Of MotifMatch))
        Dim motifLen As Integer = motif.pwm.Length
        Dim seqLen As Integer = sequence.Length
        Dim alphaIndex As Dictionary(Of Char, Integer) = BuildAlphabetIndex(motif)

        For start As Integer = 0 To seqLen - motifLen
            Dim score As Double = 0.0
            Dim ok As Boolean = True
            For k As Integer = 0 To motifLen - 1
                Dim j As Integer
                If Not alphaIndex.TryGetValue(Char.ToUpper(sequence(start + k)), j) Then
                    ok = False
                    Exit For
                End If
                score += logOdds(k, j)
            Next

            If Not ok Then Continue For
            If score < scoreThreshold Then Continue For

            Dim pval As Double = dist.PValueGE(score)
            If pval > pValueThreshold Then Continue For

            Dim frag As String = sequence.Substring(start, motifLen)

            ' Map reverse-strand match back to forward-strand coordinates.
            Dim reportStart As Integer = start
            If strand = "-"c Then
                reportStart = seqLen - (start + motifLen)
            End If

            ' Total information content of the matched columns (if available)
            Dim bits As Double = 0.0
            For k As Integer = 0 To motifLen - 1
                If motif.pwm(k).bits > 0 Then bits += motif.pwm(k).bits
            Next

            results.Add(New MotifMatch With {
                        .seeds = {motif.name},
                        .start = reportStart,
                        .ends = motifLen + .start,
                        .segment = frag,
                        .score1 = score,
                        .pvalue = pval,
                        .strand = strand,
                        .score2 = bits,
                        .motif = motif.site_pattern,
                        .identities = .score1
                        })
        Next
    End Sub

    ' --------------------------------------------------------------------
    '  Sequence utilities
    ' --------------------------------------------------------------------

    ''' <summary>
    ''' Compute the reverse complement of a DNA sequence. Supports IUPAC
    ''' ambiguity codes; unknown characters are passed through unchanged.
    ''' </summary>
    Public Shared Function ReverseComplement(seq As String) As String
        If String.IsNullOrEmpty(seq) Then Return seq
        Dim rc As Char() = New Char(seq.Length - 1) {}
        For i As Integer = 0 To seq.Length - 1
            rc(seq.Length - 1 - i) = ComplementBase(seq(i))
        Next
        Return New String(rc)
    End Function

    ''' <summary>Complement of a single base (IUPAC-aware).</summary>
    Public Shared Function ComplementBase(c As Char) As Char
        Select Case Char.ToUpper(c)
            Case "A"c : Return "T"c
            Case "T"c, "U"c : Return "A"c
            Case "G"c : Return "C"c
            Case "C"c : Return "G"c
            Case "R"c : Return "Y"c
            Case "Y"c : Return "R"c
            Case "S"c : Return "S"c
            Case "W"c : Return "W"c
            Case "K"c : Return "M"c
            Case "M"c : Return "K"c
            Case "B"c : Return "V"c
            Case "V"c : Return "B"c
            Case "D"c : Return "H"c
            Case "H"c : Return "D"c
            Case "N"c : Return "N"c
            Case Else : Return c
        End Select
    End Function

End Class


