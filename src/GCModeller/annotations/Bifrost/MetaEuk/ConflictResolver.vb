#Region "Microsoft.VisualBasic::ee66ca52d0c2a9407b3755c1da992989, annotations\Bifrost\MetaEuk\ConflictResolver.vb"

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

    '   Total Lines: 63
    '    Code Lines: 37 (58.73%)
    ' Comment Lines: 12 (19.05%)
    '    - Xml Docs: 58.33%
    ' 
    '   Blank Lines: 14 (22.22%)
    '     File Size: 2.49 KB


    ' Class ConflictResolver
    ' 
    '     Function: Overlaps, Resolve, ResolveStrand
    ' 
    ' /********************************************************************************/

#End Region


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

