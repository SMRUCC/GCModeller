Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.PubMed

Module pubmedParserTest

    Sub Main()
        Dim articles = {"M:\project\20250728-wheat\20251012\pubmed\pubmed-WheatStrip-set.txt",
"M:\project\20250728-wheat\20251012\pubmed\pubmed-plantNLRim-set.txt",
"M:\project\20250728-wheat\20251012\pubmed\pubmed-plantdisea-set.txt",
"M:\project\20250728-wheat\20251012\pubmed\pubmed-StripeRust-set.txt",
"M:\project\20250728-wheat\20251012\pubmed\pubmed-PlantImmun-set.txt"}.Select(Function(file) PlainTextParser.LoadArticles(file)).IteratesALL.GroupBy(Function(a) a.PMID).Select(Function(a) a.First).ToArray

        Dim table As New DataFrame With {
            .features = New Dictionary(Of String, FeatureVector),
            .rownames = (From article As PubmedArticle In articles Select article.PMID).ToArray
        }
        Dim genes As Dictionary(Of String, String())
        Dim ollama As New Ollama.Ollama("qwen3:30b", preserveMemory:=False)

        genes = "./tmp.json".ReadAllText(throwEx:=False).LoadJSON(Of Dictionary(Of String, String()))(throwEx:=False)

        If genes Is Nothing Then
            genes = New Dictionary(Of String, String())
        End If

        Call "LLM AI is thinking...".info

        For Each article As PubmedArticle In TqdmWrapper.Wrap(articles)
            If Not genes.ContainsKey(article.PMID) Then
                Dim pmid = article.PMID
                Dim abstract = article.GetAbstractText
                Dim keywords = article.GetOtherTerms.ToArray

                If keywords.IsNullOrEmpty Then
                    Continue For
                End If

                Dim payload As String = keywords.GetJson
                Dim prompt = "下面有一个我从研究文献中提取出来的关键词列表，现在我需要你从下面我所提取出来的关键词列表中提取出一个包含有基因名称或者基因家族名称的列表。
返回给我的文本应该是仅包含有结果的json数组的json字符串格式返回结果给我，例如[""geneName""]这样子的格式；假若没有结果，则返回一个空的json数组[]给我，以方便我进行自动化解析。
以下是需要做提取处理的关键词列表的json数组信息：" & payload
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
        Call table.add("genes", From article As PubmedArticle In articles Let mesh_genes = genes.TryGetValue(article.PMID) Select mesh_genes.JoinBy("; "))
        Call table.add("keywords", From article As PubmedArticle In articles Select article.GetOtherTerms.JoinBy("; "))

        Call table.WriteCsv("M:\project\20250728-wheat\20251012\pubmed\articles.csv")

        '  Pause()
    End Sub
End Module
