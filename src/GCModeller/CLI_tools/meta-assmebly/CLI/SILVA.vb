Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ''' <summary>
    ''' 如果不需要序列，而只是需要根据编号来获取物种的分类信息的话，可以先使用这个命令建立SILVA物种数据库，直接从这个建立好的库之中获取物种分类信息即可
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/SILVA.headers")>
    <Usage("/SILVA.headers /in <silva.fasta> /out <headers.tsv>")>
    <Group(CLIGroups.SILVA_cli)>
    Public Function SILVA_headers(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.headers.tsv"

        Using writer As StreamWriter = out.OpenWriter
            For Each SSU As FastaToken In StreamIterator.SeqSource(handle:=[in], debug:=True)
                Dim headers = SSU.Attributes.JoinBy("|").GetTagValue(" ", trim:=True)
                Call writer.WriteLine(headers.Name & vbTab & headers.Value)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/OTU.cluster")>
    <Usage("/OTU.cluster /left <left.fq> /right <right.fq> [/out <out.directory> /@set mothur=path]")>
    Public Function ClusterOTU(args As CommandLine) As Integer
        Dim left$ = args <= "/left"
        Dim right$ = args <= "/right"
        Dim out$ = args("/out") Or "./"

        Call MothurContigsOTU.ClusterOTUByMothur(left, right, workspace:=out, processor:=App.CPUCoreNumbers)

        Return 0
    End Function
End Module