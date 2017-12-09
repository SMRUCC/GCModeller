Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.Uniprot.XML
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

    <ExportAPI("/Metagenome.UniProt.Ref")>
    <Usage("/Metagenome.UniProt.Ref /in <uniprot.ultralarge.xml> [/out <out.directory>]")>
    Public Function BuildUniProtReference(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or [in].TrimSuffix & "/"

        For Each protein As entry In UniProtXML.EnumerateEntries(path:=[in])
            Dim taxonomy = protein.organism
            Dim lineage$ = taxonomy.lineage.taxonlist.Select(AddressOf NormalizePathString).JoinBy("/").MD5
            Dim path$

            With taxonomy.lineage.taxonlist
                path = $"{out}/{ .First}/{ .ref(1)}/{lineage}/[{taxonomy.dbReference.id}] {taxonomy.scientificName.NormalizePathString}.XML"
            End With

            Dim KO$() = protein.Xrefs _
                .TryGetValue("KO") _
                .SafeQuery _
                .Select(Function(KEGG) KEGG.id) _
                .ToArray

            If path.FileExists Then
                ' 追加
                ' 因为append并不会换行，所以在这里需要额外的追加一个LF换行
                Call (KO _
                    .JoinBy(ASCII.LF) & vbLf) _
                    .SaveTo(path.TrimSuffix & ".txt", append:=True, throwEx:=False)
            Else
                ' 写入新的数据
                Call taxonomy.GetXml.SaveTo(path)
                Call KO.FlushAllLines(path.TrimSuffix & ".txt")
            End If
        Next

        Return 0
    End Function
End Module