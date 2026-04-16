#Region "Microsoft.VisualBasic::b23dd93d9b071606ccfa7dc90f19a080, analysis\Motifs\PrimerDesigner\Restriction_enzyme\Slicer.vb"

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

    '   Total Lines: 119
    '    Code Lines: 94 (78.99%)
    ' Comment Lines: 4 (3.36%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 21 (17.65%)
    '     File Size: 4.83 KB


    '     Class Slicer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DoSlice, GetSegments, tracer
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Restriction_enzyme

    ''' <summary>
    ''' enzyme cut sequence simulation
    ''' </summary>
    Public Class Slicer

        ReadOnly nt As FastaSeq
        ReadOnly enzymeList As (enzyme As Enzyme, motif As MotifPattern, Cut As Cut)()
        ReadOnly strand As Strands = Strands.Forward
        ReadOnly margin As Integer = 5
        ReadOnly depth As Integer = 3

        Sub New(nt As IAbstractFastaToken, enzymes As IEnumerable(Of Enzyme),
                Optional direction As Strands = Strands.Forward,
                Optional margin As Integer = 5,
                Optional depth As Integer = 5)

            Me.nt = New FastaSeq(nt)
            Me.depth = depth
            Me.margin = margin

            enzymeList = enzymes _
                .Select(Function(a)
                            Return (a, a.TranslateRegular(strand), a.GetCutSite(strand))
                        End Function) _
                .ToArray
            strand = direction
        End Sub

        Public Function GetSegments() As IEnumerable(Of FastaSeq)
            Dim pool As New List(Of FastaSeq) From {nt}
            Dim nt_pool As New List(Of Scanner) From {
                New Scanner(nt, reverse_search:=False)
            }
            Dim temp As New List(Of FastaSeq)

            For i As Integer = 1 To depth
                Dim q = From enzyme
                        In enzymeList.AsParallel
                        Select DoSlice(enzyme.enzyme, enzyme.motif, enzyme.Cut, nt_pool).ToArray

                Call temp.Clear()

                For Each block As FastaSeq() In q
                    Call temp.AddRange(block)
                Next

                If temp.Count = 0 Then
                    Exit For
                Else
                    nt_pool = temp _
                        .Select(Function(a) New Scanner(a, reverse_search:=False)) _
                        .AsList
                    pool.AddRange(temp)
                End If
            Next

            Return pool.ToArray
        End Function

        Private Iterator Function DoSlice(enzyme As Enzyme, motif As MotifPattern, cut As Cut, nt_pool As IEnumerable(Of Scanner)) As IEnumerable(Of FastaSeq)
            For Each seq As Scanner In nt_pool
                Dim sites As SimpleSegment() = motif.Scan(seq)
                ' break current sequence by cut site
                Dim str_nt As String = seq.GetBaseSequence
                Dim len As Integer = str_nt.Length
                Dim margin1 As Integer = len - margin

                If cut.IsSingle Then
                    For Each segment As SimpleSegment In sites
                        Dim sub_seq1 As String = str_nt.Substring(0, segment.Ends - 1)
                        Dim sub_seq2 As String = str_nt.Substring(segment.Ends - 1)

                        If segment.Ends < margin1 AndAlso segment.Start > margin Then
                            Yield New FastaSeq(sub_seq1, tracer(enzyme, seq, segment, cut, True))

                            If Not sub_seq2.StringEmpty Then
                                Yield New FastaSeq(sub_seq2, tracer(enzyme, seq, segment, cut, False))
                            End If
                        End If
                    Next
                Else
                    For Each segment As SimpleSegment In sites
                        Dim sub_seq1 As String = str_nt.Substring(0, segment.Ends - cut.CutSite2.Length - 1)
                        Dim sub_seq2 As String = str_nt.Substring(segment.Ends - cut.CutSite2.Length - 1)

                        If segment.Ends < margin1 AndAlso segment.Start > margin Then
                            Yield New FastaSeq(sub_seq1, tracer(enzyme, seq, segment, cut, True))
                            Yield New FastaSeq(sub_seq2, tracer(enzyme, seq, segment, cut, False))
                        End If
                    Next
                End If
            Next
        End Function

        Private Shared Function tracer(enzyme As Enzyme, seq As Scanner, site As SimpleSegment, cut As Cut, left As Boolean) As String
            Dim cut_seq As String

            If left Then
                cut_seq = cut.CutSite1
            Else
                If cut.IsSingle Then
                    cut_seq = cut.CutSite1
                Else
                    cut_seq = cut.CutSite2
                End If
            End If

            Return $"{seq.name}..{enzyme.Enzyme}..{site.Start}..{cut_seq}"
        End Function

    End Class
End Namespace
