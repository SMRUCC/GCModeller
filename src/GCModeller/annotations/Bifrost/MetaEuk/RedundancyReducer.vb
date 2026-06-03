
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
