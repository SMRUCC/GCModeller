Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Partial Module CLI

    <ExportAPI("/pubmed.kb")>
    <Usage("/pubmed.kb /term <term_string> [/out <out_directory>]")>
    Public Function BuildPubMedDatabase(args As CommandLine) As Integer
        Dim term$ = args <= "/term"
        Dim out$ = args("/out") Or $"./pubmed_{term.NormalizePathString}"
        Dim idlist = PubMed.QueryPubmed(term, pageSize:=50000).Distinct.ToArray

        Call idlist.GetJson.SaveTo($"{out}/id.json")

        ' 下载文献摘要数据
    End Function
End Module