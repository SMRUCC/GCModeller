
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.PubMed

<Package("pubmed")>
Module pubmedTools

    <ExportAPI("query")>
    Public Function QueryKeyword(keyword As String, Optional page As Integer = 1, Optional size As Integer = 2000) As String
        Return PubMed.QueryPubmedRaw(term:=keyword, page:=page, size:=size)
    End Function

    <ExportAPI("article")>
    Public Function Parse(text As String) As PubmedArticle

    End Function
End Module
