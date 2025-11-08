Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
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

        Dim ignores As Index(Of String) = {"wheat", "transcriptome", "spike", "transcriptome", "yield", "metabolome", "barley", "rice", "bioinformatics", "gwas", "haplotype", "h2o2", "lncrna", "proteomics", "rye",
            "genome-wide", "genome", "transgenosis", "anther", "starch", "heredity", "mutant", "evolution", "spikelet", "expression", "rna-seq", "differentiation", "tissue"}

        Dim genes As Dictionary(Of String, String())
        Dim ollama As New Ollama.Ollama("qwen3:30b")

        genes = "./tmp.json".LoadJsonFile(Of Dictionary(Of String, String()))

        If genes Is Nothing Then
            genes = New Dictionary(Of String, String())
        End If

        For Each article As PubmedArticle In articles
            If genes.TryGetValue(article.PMID).IsNullOrEmpty Then
                Dim pmid = article.PMID
                Dim abstract = article.GetAbstractText
                Dim keywords = article.GetOtherTerms.ToArray
                Dim payload As String = keywords.GetJson
                Dim prompt = "下面有一个我从研究文献中提取出来的关键词列表，现在我需要你从下面我所提取出来的关键词列表中提取出一个包含有基因名称或者基因家族或者转录调控位点名称的列表，使用仅包含有结果的json数组的json字符串格式返回结果给我,假若没有结果，则返回一个空的json数组[]给我，以方便我进行自动化解析。以下是需要做提取处理的关键词列表的json数组信息：" & payload
                Dim result = ollama.Chat(prompt)

                Call genes.Add(pmid, result.output.LoadJSON(Of String()))
                Call genes.GetJson.SaveTo("./tmp.json")
            End If
        Next

        Call table.add("title", From article As PubmedArticle In articles Select article.GetTitle)
        Call table.add("doi", From article As PubmedArticle In articles Select article.GetArticleDoi)
        Call table.add("authors", From article As PubmedArticle In articles Select article.MedlineCitation.Article.AuthorList.AsEnumerable.Select(Function(a) a.Initials).JoinBy("; "))
        Call table.add("journal", From article As PubmedArticle In articles Select article.GetJournal)
        Call table.add("year", From article As PubmedArticle In articles Select article.GetPublishYear)
        Call table.add("genes", From article As PubmedArticle In articles Let mesh_genes = article.GetOtherTerms.Where(Function(t) t.IsPattern("[A-Za-z][A-Za-z0-9-]+") AndAlso Not t.ToLower Like ignores).ToArray Select mesh_genes.JoinBy("; "))
        Call table.add("keywords", From article As PubmedArticle In articles Select article.GetOtherTerms.JoinBy("; "))

        Call table.WriteCsv("M:\project\20251010-wheat\20251105\LargePanicleDevelopment\articles.csv")

        '  Pause()
    End Sub
End Module
