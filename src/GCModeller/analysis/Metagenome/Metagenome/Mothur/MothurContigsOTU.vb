Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
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
    Public Sub ClusterOTUByMothur(left$, right$, Optional workspace$ = Nothing, Optional processor% = 2)
        Dim mothur As New Mothur(App:=Settings.Mothur)
        Dim contig$ = left.BaseName & ".trim.contigs.fasta"
        Dim template$ = "template.fasta"

        App.CurrentDirectory = workspace

        ' make contigs from fq reads
        Call mothur.Make_contigs(left, right, processors:=2)
        Call contig.FileCopy("contig.fasta")
        Call contig.SetValue("contig.fasta") _
                   .GetContigAlignmentTemplate() _
                   .SaveAsOneLine(template)

        ' align contigs or
        ' [ERROR]:your sequences are not the same length, aborting.
        Call mothur.Unique_seqs(contig)
        Call mothur.align_seqs(candidate:="contig.unique.fasta", template:=template, processors:=processor)
        Call mothur.filter_seqs("contig.unique.align")
        Call "contig.unique.filter.fasta".FileCopy(contig)

        ' run OTU cluster
        Call mothur.Unique_seqs(contig)
        Call mothur.Dist_seqs(contig)
        Call mothur.Cluster("contig.unique.phylip.dist")
        Call mothur.Bin_seqs(fasta:=contig, name:="contig.names")
        Call mothur.GetOTUrep(
            phylip:="contig.unique.phylip.dist",
            fasta:="contig.unique.fasta",
            list:="contig.unique.phylip.fn.list"
        )

        App.CurrentDirectory = App.PreviousDirectory
    End Sub

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
