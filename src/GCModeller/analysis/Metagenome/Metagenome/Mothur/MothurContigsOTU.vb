Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.ClustalOrg
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module MothurContigsOTU

    ''' <summary>
    ''' 通过配置文件读取mothur程序的路径
    ''' </summary>
    Sub New()
        Call Settings.Initialize()
    End Sub

    ''' <summary>
    ''' 使用paired-end测序数据通过mothur程序构建出OTU序列
    ''' </summary>
    ''' <param name="left$"></param>
    ''' <param name="right$"></param>
    ''' <param name="workspace$"></param>
    ''' <remarks>
    ''' http://www.opiniomics.org/a-mothur-tutorial-what-can-we-find-out-about-the-horse-gut-metagenome/
    ''' </remarks>
    Public Sub ClusterOTUByMothur(left$, right$, silva$, Optional workspace$ = Nothing, Optional processor% = 2)
        Dim mothur As New Mothur(App:=Settings.Mothur)
        Dim contigs$ = $"{workspace}/contigs.files"
        Dim groups$
        Dim align$ = ""

        Call {
            "contigs", left, right
        } _
            .JoinBy(ASCII.TAB) _
            .SaveTo(contigs)
        Call App.CurrentDirectory.SetValue(workspace)

        ' make contigs from fq reads
        Call mothur.Make_contigs(contigs, processors:=processor).SaveTo("[1]make.contigs.log")
        ' contigs.Trim.contigs.fasta
        ' contigs.Trim.contigs.qual
        ' contigs.contigs.report
        ' contigs.scrap.contigs.fasta
        ' contigs.scrap.contigs.qual
        ' contigs.contigs.groups
        Call mothur.RunAutoScreen("contigs.Trim.contigs.fasta", "contigs.contigs.groups", processor).SaveTo("[2]summary.seqs.log")
        ' contigs.Trim.contigs.good.fasta
        ' contigs.Trim.contigs.bad.accnos
        ' contigs.contigs.good.groups

        contigs = "contigs.fasta"
        groups = "contigs.groups"

        Call "contigs.Trim.contigs.good.fasta".FileCopy(contigs)
        Call "contigs.contigs.good.groups".FileCopy(groups)

        ' align contigs or
        ' [ERROR]:your sequences are not the same length, aborting.
        Call mothur.Unique_seqs(contigs).SaveTo("[3]unique.seqs.log")
        ' contigs.names
        ' contigs.unique.fasta
        Call mothur.Count_seqs("contigs.names", groups).SaveTo("[4]count.seqs.log")
        ' contigs.count_table
        Call mothur.Summary_seqs("contigs.unique.fasta", "contigs.count_table").SaveTo("[5]summary.seqs.log")

        ' https://mothur.org/w/images/9/98/Silva.bacteria.zip
        Call mothur.align_seqs("contigs.unique.fasta", silva, "T", processor).SaveTo("[6]align.seqs.log")
        ' contigs.unique.align
        ' contigs.unique.align.report
        ' contigs.unique.flip.accnos

        ' Call mothur.RunAutoScreen("contigs.unique.align", "contigs.count_table", processor).SaveTo("[7]summary.seqs.log")
        ' Removing group: C_19-4 because all sequences have been removed. ????
        ' contigs.unique.good.summary
        ' contigs.unique.good.align
        ' contigs.unique.bad.accnos
        ' contigs.good.count_table
        ' Call align.SetValue("contigs.unique.good.align")

        Call align.SetValue("contigs.unique.align")
        Call mothur.filter_seqs(align).SaveTo("[7]filter.seqs.log")
        ' contigs.filter
        ' contigs.unique.filter.fasta

        Call "contigs.unique.filter.fasta".CopyTo(contigs)
        ' contigs.fasta
        Call mothur.Unique_seqs(contigs)
        ' contigs.names
        ' contigs.unique.fasta

        Call align.SetValue("contigs.unique.fasta")
        Call mothur.Dist_seqs(align, processors:=processor).SaveTo("[8]dist.seqs.log")
        ' contigs.unique.phylip.dist
        Call mothur.Cluster(phylip:="contigs.unique.phylip.dist").SaveTo("[9]cluster.log")

        Call mothur.Bin_seqs("contigs.fasta", "contigs.names").SaveTo("[10]bin.seqs.log")
        Call mothur.GetOTUrep(phylip:="contigs.unique.phylip.dist", fasta:="contigs.unique.fasta", list:="contigs.unique.phylip.fn.list", label:=0.03).SaveTo("[11]get.oturep.log")

        App.CurrentDirectory = App.PreviousDirectory
    End Sub

    Public Function RunAutoScreen2(mothur As Mothur, fasta$, count$, Optional processors% = 2) As String
        Dim summary = mothur.Summary_seqs(fasta, count, processors)
        ' contigs.unique.summary
        Dim table = SummaryTable(summary).ToDictionary
        Dim start% = table("2.5%-tile")!Start
        Dim end% = table("97.5%-tile")!End

        Call mothur.Screen_seqs(fasta, count, "contigs.unique.summary", start, [end], maxhomop:=8, processors:=processors)

        Return summary
    End Function

    ''' <summary>
    ''' 1. summary.seqs
    ''' 2. screen.seqs
    ''' </summary>
    ''' <param name="fasta"></param>
    <Extension>
    Public Function RunAutoScreen(mothur As Mothur, fasta$, group$, Optional processors% = 2) As String
        Dim summary = mothur.Summary_seqs(fasta, processors)
        Dim table = SummaryTable(summary).ToDictionary
        Dim min% = table("2.5%-tile")!End - table("2.5%-tile")!Start + 1
        Dim max% = table("97.5%-tile")!End - table("97.5%-tile")!Start + 1

        Call mothur.Screen_seqs(fasta, group, 0, min, max)

        Return summary
    End Function

    ''' <summary>
    ''' 使用序列长度最普遍的某个为序列模板
    ''' </summary>
    ''' <param name="contig"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetContigAlignmentTemplate(contig As String) As FastaToken
        Dim length As New Dictionary(Of Integer, Counter)

        For Each seq As FastaToken In StreamIterator.SeqSource(handle:=contig)
            If Not length.ContainsKey(seq.Length) Then
                Call length.Add(seq.Length, New Counter)
            End If

            Call length(seq.Length).Hit()
        Next

        Dim n As Integer = length _
            .OrderByDescending(Function(c)
                                   Return c.Value.Value
                               End Function) _
            .First _
            .Key

        For Each seq As FastaToken In StreamIterator.SeqSource(handle:=contig)
            If seq.Length = n Then
                Return seq
            End If
        Next

        Return Nothing
    End Function
End Module
