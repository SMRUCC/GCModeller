

' ========================================================================
' MODULE 3: SIX-FRAME TRANSLATION & CANDIDATE FRAGMENT GENERATION
' ========================================================================

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class SixFrameTranslator

    ''' <summary>
    ''' Perform six-frame translation on a contig and extract candidate coding fragments.
    ''' Fragments are delimited by stop codons (*). Only fragments meeting minimum
    ''' length criteria are retained.
    ''' </summary>
    Public Shared Function GenerateFragments(contig As FastaSeq, config As MetaEukConfig) As List(Of CandidateFragment)
        Dim fragments As New List(Of CandidateFragment)
        Dim dna = contig.SequenceData.ToUpper()
        Dim contigID = contig.locus_tag
        Dim fragIdx As Integer = 0

        ' --- Forward strand (frames 0, 1, 2) ---
        For frame = 0 To 2
            Dim peptide = CodonTable.Translate(dna, frame)
            Dim newFrags = ExtractFragmentsFromPeptide(
                peptide, contigID, Strands.Forward, frame, dna.Length, config)
            For Each f In newFrags
                f.FragmentIndex = fragIdx
                fragIdx += 1
            Next
            fragments.AddRange(newFrags)
        Next

        ' --- Reverse strand (frames 0, 1, 2 on reverse complement) ---
        Dim rcDna = CodonTable.ReverseComplement(dna)
        For frame = 0 To 2
            Dim peptide = CodonTable.Translate(rcDna, frame)
            Dim newFrags = ExtractFragmentsFromPeptide(
                peptide, contigID, Strands.Reverse, frame, dna.Length, config)
            For Each f In newFrags
                f.FragmentIndex = fragIdx
                fragIdx += 1
            Next
            fragments.AddRange(newFrags)
        Next

        Return fragments
    End Function

    ''' <summary>
    ''' Extract candidate fragments from a translated peptide sequence.
    ''' Split at stop codons (*), filter by length, compute DNA coordinates.
    ''' </summary>
    Private Shared Function ExtractFragmentsFromPeptide(
        peptide As String,
        contigID As String,
        strand As Strands,
        frame As Integer,
        dnaLength As Integer,
        config As MetaEukConfig) As List(Of CandidateFragment)

        Dim fragments As New List(Of CandidateFragment)

        ' Split peptide at stop codons to get ORF segments
        Dim segments = peptide.Split({"*"c}, StringSplitOptions.RemoveEmptyEntries)

        ' Track position in peptide to compute DNA coordinates
        Dim pepPos As Integer = 0

        For Each seg In segments
            Dim segStart As Integer = peptide.IndexOf(seg, pepPos)
            Dim segEnd As Integer = segStart + seg.Length - 1
            pepPos = segEnd + 1

            If seg.Length < config.MinFragmentLength Then Continue For
            If seg.Length > config.MaxFragmentLength Then
                ' For very long segments, split into overlapping windows
                Dim stepSize As Integer = CInt(config.MaxFragmentLength * 0.75)
                Dim winStart As Integer = 0
                While winStart < seg.Length
                    Dim winEnd As Integer = Math.Min(winStart + config.MaxFragmentLength - 1, seg.Length - 1)
                    Dim winSeq = seg.Substring(winStart, winEnd - winStart + 1)
                    If winSeq.Length >= config.MinFragmentLength Then
                        Dim frag = CreateFragment(contigID, strand, frame, dnaLength,
                                                  segStart + winStart, segStart + winEnd, winSeq)
                        fragments.Add(frag)
                    End If
                    winStart += stepSize
                End While
            Else
                Dim frag = CreateFragment(contigID, strand, frame, dnaLength,
                                          segStart, segEnd, seg)
                fragments.Add(frag)
            End If
        Next

        Return fragments
    End Function

    ''' <summary>
    ''' Create a CandidateFragment with proper DNA coordinate mapping.
    ''' Converts peptide coordinates back to DNA coordinates on the original contig.
    ''' </summary>
    Private Shared Function CreateFragment(
        contigID As String,
        strand As Strands,
        frame As Integer,
        dnaLength As Integer,
        pepStart As Integer,
        pepEnd As Integer,
        peptide As String) As CandidateFragment

        Dim frag As New CandidateFragment()
        frag.ContigID = contigID
        frag.Strand = strand
        frag.Frame = frame
        frag.Peptide = peptide

        ' Convert peptide position to DNA position
        ' pepStart is 0-based index in the translated peptide
        ' DNA position of codon for pepStart: frame + pepStart * 3
        ' DNA position of codon for pepEnd: frame + pepEnd * 3 + 2

        If strand = Strands.Forward Then
            ' Forward strand: DNA coordinates are straightforward
            frag.DnaStart = frame + pepStart * 3 + 1       ' 1-based
            frag.DnaEnd = frame + pepEnd * 3 + 3           ' 1-based, inclusive
        Else
            ' Reverse strand: need to map back to forward strand coordinates
            ' On reverse complement, position i maps to (dnaLength - 1 - i) on forward strand
            Dim rcStart = frame + pepStart * 3              ' 0-based on RC
            Dim rcEnd = frame + pepEnd * 3 + 2             ' 0-based on RC
            ' Map to forward strand (1-based)
            frag.DnaStart = dnaLength - rcEnd               ' 1-based
            frag.DnaEnd = dnaLength - rcStart               ' 1-based
        End If

        Return frag
    End Function

End Class
