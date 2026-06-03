
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
