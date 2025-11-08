Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI

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

        Pause()
    End Sub
End Module
