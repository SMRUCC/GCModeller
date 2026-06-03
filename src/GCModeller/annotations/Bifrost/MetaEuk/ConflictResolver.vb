
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
