
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.PubMed
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("pubmed")>
Module pubmedTools

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(PubmedArticle()), AddressOf toTabular)
    End Sub

    Private Function toTabular(articles As PubmedArticle(), args As list, env As Environment) As dataframe

    End Function

    <ExportAPI("query")>
    Public Function QueryKeyword(keyword As String, Optional page As Integer = 1, Optional size As Integer = 2000) As String
        Return PubMed.QueryPubmedRaw(term:=keyword, page:=page, size:=size)
    End Function

    <ExportAPI("article")>
    Public Function Parse(text As String) As PubmedArticle()
        Return PubMed.ParseArticles(text).ToArray
    End Function
End Module
