Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.PubMed

Module pubmedParserTest

    Sub Main()
        Dim articles = {"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-Secalespik-set.txt",
"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-Triticumsp-set.txt",
"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-wheatspike-set.txt",
"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-Agropyrons-set.txt",
"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-barleyspik-set.txt",
"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-Hordeumspi-set.txt",
"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-Oryzaspike-set.txt",
"M:\project\20251010-wheat\释放数据20250927\pubmed\pubmed-ricespiked-set.txt"}.Select(Function(file) PlainTextParser.LoadArticles(file)).IteratesALL.GroupBy(Function(a) a.PMID).Select(Function(a) a.First).ToArray

        Dim table As New DataFrame With {
            .features = New Dictionary(Of String, FeatureVector),
            .rownames = (From article As PubmedArticle In articles Select article.PMID).ToArray
        }

        Call table.add("title", From article As PubmedArticle In articles Select article.GetTitle)
        Call table.add("doi", From article As PubmedArticle In articles Select article.GetArticleDoi)
        Call table.add("authors", From article As PubmedArticle In articles Select article.MedlineCitation.Article.AuthorList.AsEnumerable.Select(Function(a) a.Initials).JoinBy("; "))
        Call table.add("journal", From article As PubmedArticle In articles Select article.GetJournal)
        Call table.add("year", From article As PubmedArticle In articles Select article.GetPublishYear)
        Call table.add("genes", From article As PubmedArticle In articles Let mesh_genes = article.GetOtherTerms.Where(Function(t) t.IsPattern("[A-Za-z][A-Za-z0-9-]+")).ToArray Select mesh_genes.JoinBy("; "))
        Call table.add("keywords", From article As PubmedArticle In articles Select article.GetOtherTerms.JoinBy("; "))

        Call table.WriteCsv("M:\project\20251010-wheat\20251105\LargePanicleDevelopment\articles.csv")

        '  Pause()
    End Sub
End Module
