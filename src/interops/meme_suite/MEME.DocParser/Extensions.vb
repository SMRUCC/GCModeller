Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.XmlOutput.MAST
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Linq

Public Module Extensions

    ReadOnly __doc As String() = {"meme.txt", "meme.xml", "meme.html", "mast.txt", "mast.xml", "mast.html"}

    <Extension> Public Function TraceName(path As String) As String
        Dim name As String = path.BaseName.ToLower
        If Array.IndexOf(__doc, name) > -1 Then
            Return path.ParentDirName
        Else
            Return name
        End If
    End Function

    <Extension>
    Public Function MASTSites(mast As XmlOutput.MAST.MAST) As SimpleSegment()
        Dim name As String = mast.Motifs.BriefName
        Dim result = mast.Sequences.SequenceList.ToArray(Function(x) x.__toSites(name))
        Dim sites As SimpleSegment() = result.MatrixAsIterator.MatrixToVector
        Return sites
    End Function

    <Extension>
    Private Function __toSites(seq As SequenceDescript, familyName As String) As SimpleSegment()()
        Return seq.Segments.ToArray(Function(loci) __createObject(loci, familyName))
    End Function

    Private Function __createObject(site As Segment, trace As String) As SimpleSegment()
        Dim sequence As String = TrimVBCrLf(site.SegmentData).Replace(vbTab, "").Trim
        Dim sites As SimpleSegment() = site.Hits.ToArray(
            Function(hit) __createObject(site.start, hit, sequence, OffSet:=5, trace:=trace))
        Return sites
    End Function

    ''' <summary>
    ''' 不做任何筛选，直接导出数据
    ''' </summary>
    ''' <param name="start"></param>
    ''' <param name="hit"></param>
    ''' <param name="sequence"></param>
    ''' <param name="OffSet"></param>
    ''' <param name="trace"></param>
    ''' <returns></returns>
    Private Function __createObject(start As Integer,
                                    hit As HitResult,
                                    sequence As String,
                                    OffSet As Integer,
                                    trace As String) As SimpleSegment

        Dim id As String = hit.motif.Split("_"c).Last
        Dim length As Integer = Len(hit.match) + 2 * OffSet  '为了保证在进行分子生物学实验的时候能够得到完整的片段，在这里将位点的范围扩大了10个bp
        start = hit.pos - start
        start -= OffSet
        If start <= 0 Then
            start = 1
            'Call $"{hit.pos} - {start} is not enough".__DEBUG_ECHO
        End If

        Dim site As String = Mid(sequence, start, length)
        Dim strand As String = hit.strand.GetBriefStrandCode
        Dim strands As Strands = strand.GetStrand
        Dim right As Integer

        If strands = Strands.Forward Then
            right = hit.pos + length
        Else
            right = hit.pos - length
        End If

        '需不需要将反向的序列互补为正向的？？？

        Dim mastSite As New SimpleSegment With {
            .ID = $"{trace}::{id}",
            .SequenceData = site,
            .Start = hit.pos,
            .Strand = strand,
            .Ends = right,
            .Complement = New String(NucleicAcid.Complement(site).Reverse.ToArray)
        }
        Return mastSite
    End Function
End Module
