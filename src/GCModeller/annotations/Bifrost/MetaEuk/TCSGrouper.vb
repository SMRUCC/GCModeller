#Region "Microsoft.VisualBasic::bbc15119b9a7211a514915e29624ce4d, annotations\Bifrost\MetaEuk\TCSGrouper.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 78
    '    Code Lines: 51 (65.38%)
    ' Comment Lines: 13 (16.67%)
    '    - Xml Docs: 30.77%
    ' 
    '   Blank Lines: 14 (17.95%)
    '     File Size: 3.00 KB


    ' Class TCSGrouper
    ' 
    '     Function: GroupHits
    ' 
    ' /********************************************************************************/

#End Region


' ========================================================================
' MODULE 6: TCS GROUPING
' ========================================================================

Imports SMRUCC.genomics.ComponentModel.Loci

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

            If frag.Strand = Strands.Forward Then
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

