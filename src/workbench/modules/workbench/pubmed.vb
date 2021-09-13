
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.PubMed

<Package("pubmed")>
Module pubmedTools

    <ExportAPI("query")>
    Public Function QueryKeyword(keyword As String, Optional pageSize As Integer = 2000) As String()
        Return PubMed.QueryPubmed(term:=keyword, pageSize:=pageSize).ToArray
    End Function

    <ExportAPI("article")>
    Public Function PubmedArticle(id As String) As PubmedArticle
        Return PubMed.GetArticleInfo(term:=id)
    End Function

End Module
