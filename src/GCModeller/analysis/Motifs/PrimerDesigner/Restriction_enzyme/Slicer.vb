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

        Sub New(nt As IAbstractFastaToken, enzymes As IEnumerable(Of Enzyme), Optional direction As Strands = Strands.Forward)
            Me.nt = New FastaSeq(nt)

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

            Do While True
                Dim q = From enzyme
                        In enzymeList.AsParallel
                        Select DoSlice(enzyme.enzyme, enzyme.motif, enzyme.Cut, nt_pool).ToArray

                Call temp.Clear()

                For Each block As FastaSeq() In q
                    Call temp.AddRange(block)
                Next

                If temp.Count = 0 Then
                    Exit Do
                Else
                    nt_pool = temp _
                        .Select(Function(a) New Scanner(a, reverse_search:=False)) _
                        .AsList
                    pool.AddRange(temp)
                End If
            Loop

            Return pool.ToArray
        End Function

        Private Shared Iterator Function DoSlice(enzyme As Enzyme, motif As MotifPattern, cut As Cut, nt_pool As IEnumerable(Of Scanner)) As IEnumerable(Of FastaSeq)
            For Each seq As Scanner In nt_pool
                Dim sites As SimpleSegment() = motif.Scan(seq)
                ' break current sequence by cut site
                Dim str_nt As String = seq.GetBaseSequence

                If cut.IsSingle Then
                    For Each segment As SimpleSegment In sites
                        Dim sub_seq1 As String = str_nt.Substring(0, segment.Ends - 1)
                        Dim sub_seq2 As String = str_nt.Substring(segment.Ends - 1)

                        Yield New FastaSeq(sub_seq1, tracer(enzyme, seq, segment, cut, True))

                        If Not sub_seq2.StringEmpty Then
                            Yield New FastaSeq(sub_seq2, tracer(enzyme, seq, segment, cut, False))
                        End If
                    Next
                Else
                    For Each segment As SimpleSegment In sites
                        Dim sub_seq1 As String = str_nt.Substring(0, segment.Ends - cut.CutSite2.Length - 1)
                        Dim sub_seq2 As String = str_nt.Substring(segment.Ends - cut.CutSite2.Length - 1)

                        Yield New FastaSeq(sub_seq1, tracer(enzyme, seq, segment, cut, True))
                        Yield New FastaSeq(sub_seq2, tracer(enzyme, seq, segment, cut, False))
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