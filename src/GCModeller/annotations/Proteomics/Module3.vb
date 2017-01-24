Imports System.Drawing
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.GoStat

Module Module3

    Sub Main()
        '' 1. 总蛋白注释
        'Call "C:\Users\xieguigang\OneDrive\1.23\2. annotations\uniprot.txt" _
        '    .ReadAllLines _
        '    .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.23\2. annotations\uniprot-yourlist-M20170123A7434721E10EE6586998A056CCD0537E6843CEM.xml") _
        '    .Select(Function(x) x.Item1) _
        '    .ToArray _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.23\2. annotations\proteins-uniprot-annotations.csv")

        '' Pause()

        '' 绘制GO图
        'Dim goTerms = GO_OBO.Open("K:\GO_DB\go.obo").ToDictionary(Function(x) x.id)
        'Dim sample = "C:\Users\xieguigang\OneDrive\1.23\2. annotations\proteins-uniprot-annotations.csv".LoadSample

        'Dim data = sample.CountStat(Function(x As EntityObject) x("GO").Split(";"c).Select(AddressOf Trim).ToArray, goTerms)
        'Call CatalogPlots.Plot(data, orderTakes:=20).SaveAs("C:\Users\xieguigang\OneDrive\1.23\2. annotations\GO\plot.png")
        'Call data.SaveCountValue("C:\Users\xieguigang\OneDrive\1.23\2. annotations\GO\plot.csv")

        'Pause()

        ' 2. DEP注释
        'Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\C-T.txt" _
        '    .ReadAllLines _
        '    .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
        '    .Select(Function(x) x.Item1) _
        '    .ToArray _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\4. analysis\C-T.annotations.csv")

        'Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\WT-KO.txt" _
        ' .ReadAllLines _
        ' .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
        ' .Select(Function(x) x.Item1) _
        ' .ToArray _
        ' .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\4. analysis\WT-KO.annotations.csv")

        'Pause()
        ' 3. heatmap绘图

        'Call DEGDesigner _
        '    .MergeMatrix("C:\Users\xieguigang\OneDrive\1.23\3. DEP\heatmap", "*.csv", 1.5, 0.05, "FC.avg", 1 / 1.5, "p.value") _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.23\3. DEP\DEP.heatmap.csv", blank:=1)

        '' 文氏图
        Call DEGDesigner _
            .MergeMatrix("C:\Users\xieguigang\OneDrive\1.23\3. DEP\heatmap", "*.csv", 1.5, 0.05, "FC.avg", 1 / 1.5, "p.value") _
            .SaveDataSet("C:\Users\xieguigang\OneDrive\1.23\3. DEP\DEP.venn.csv")

        Pause()

        ' 4。 导出KOBAS结果
        ' Call KOBAS.SplitData("C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\KOBAS\CD.txt")
        ' Call KOBAS.SplitData("C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\KOBAS\CO.txt")

        ' 5. go enrichment 绘图
        '   Dim terms = GO_OBO.Open("K:\GO_DB\go.obo").ToDictionary(Function(x) x.id)
        '   Call "C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\GO\CD-Gene Ontology.csv" _
        '       .LoadCsv(Of EnrichmentTerm) _
        '       .EnrichmentPlot(terms, 0.01, New Size(1600, 800)) _
        '       .SaveAs("C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\GO\CD-Gene Ontology.png")
        '   Call "C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\GO\CO-Gene Ontology.csv" _
        '.LoadCsv(Of EnrichmentTerm) _
        '.EnrichmentPlot(terms, 0.5, New Size(2000, 1800)) _
        '.SaveAs("C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\GO\CO-Gene Ontology.png")

        '   Call "C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\KEGG\CD-KEGG PATHWAY.csv" _
        '       .LoadCsv(Of EnrichmentTerm) _
        '       .KEGGEnrichmentPlot(New Size(1000, 700), 0.1) _
        '       .SaveAs("C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\KEGG\CD-KEGG PATHWAY.png")
        '   Call "C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\KEGG\CO-KEGG PATHWAY.csv" _
        '.LoadCsv(Of EnrichmentTerm) _
        '.KEGGEnrichmentPlot(New Size(1000, 500), 0.5, 0.25) _
        '.SaveAs("C:\Users\xieguigang\OneDrive\1.23\4. analysis\enrichment\KEGG\CO-KEGG PATHWAY.png")

        '   Pause()
    End Sub
End Module
