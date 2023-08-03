Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.PubMed
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("pubmed")>
Module pubmed

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(PubmedArticle()), AddressOf createArticleTable)
    End Sub

    Public Function createArticleTable(list As PubmedArticle(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim articles = list.Select(Function(a) a.MedlineCitation).ToArray
        Call df.add("pubmed", articles.Select(Function(a) a.PMID?.ID))
        Call df.add("doi", articles.Select(Function(a) a.Article.ELocationID.SafeQuery.Where(Function(d) d.EIdType = "DOI").FirstOrDefault?.Value))
        Call df.add("title", articles.Select(Function(a) a.Article.ArticleTitle))
        Call df.add("abstract", articles.Select(Function(a) a.Article.Abstract?.AbstractText.SafeQuery.Select(Function(d) d.Text).JoinBy(vbCrLf)))
        Return df
    End Function

    <ExportAPI("parse")>
    Public Function ParsePubmed(<RRawVectorArgument> text As Object)
        Return CLRVector.asCharacter(text) _
            .Select(Function(si) PubMedServicesExtensions.ParseArticles(si)) _
            .IteratesALL _
            .ToArray
    End Function
End Module
