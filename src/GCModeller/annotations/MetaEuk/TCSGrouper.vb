
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
