
' ========================================================================
' MODULE 5: HOMOLOGY SEARCH ENGINE
' ========================================================================

Imports SMRUCC.genomics.SequenceModel.FASTA

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
