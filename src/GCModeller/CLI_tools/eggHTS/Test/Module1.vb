#Region "Microsoft.VisualBasic::9afb9f3fd0784b39bbbfe99a85aa5ea1, CLI_tools\eggHTS\Test\Module1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module Module1
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.GoStat

Module Module1

    Sub Main()

        ' 1. 总蛋白注释
        'Call "C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.txt" _
        '    .ReadAllLines _
        '    .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.17\2. annotations\uniprot.xml") _
        '    .Select(Function(x) x.Item1) _
        '    .ToArray _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\2. annotations\sample-annotation.csv")

        ' 绘制GO图
        'Dim goTerms = GO_OBO.Open("K:\GO_DB\go.obo").ToDictionary(Function(x) x.id)
        'Dim sample = "C:\Users\xieguigang\OneDrive\1.17\2. annotations\sample-annotation.csv".LoadSample

        'Dim data = sample.CountStat(Function(x As EntityObject) x("GO").Split(";"c).Select(AddressOf Trim).ToArray, goTerms)
        'Call CatalogPlots.Plot(data, orderTakes:=20).SaveAs("C:\Users\xieguigang\OneDrive\1.17\2. annotations\GO\plot.png")
        'Call data.SaveCountValue("C:\Users\xieguigang\OneDrive\1.17\2. annotations\GO\plot.csv")

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
        '    .MergeMatrix("C:\Users\xieguigang\OneDrive\1.17\3. DEP\heatmap", "*.csv", 1.25, 0.05, "FC.avg", 1 / 1.25, "p.value") _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\3. DEP\DEP.heatmap.csv", blank:=1)

        ' 文氏图
        Call DEGDesigner _
            .MergeMatrix("C:\Users\xieguigang\OneDrive\1.17\3. DEP\heatmap", "*.csv", 1.25, 0.05, "FC.avg", 1 / 1.25, "p.value") _
            .SaveDataSet("C:\Users\xieguigang\OneDrive\1.17\3. DEP\DEP.venn.csv")

        Pause()

        ' 4。 导出KOBAS结果
        'Call KOBAS.SplitData("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KOBAS\C-T.txt")
        'Call KOBAS.SplitData("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KOBAS\WT-KO.txt")

        ' 5. go enrichment 绘图
        '   Dim terms = GO_OBO.Open("K:\GO_DB\go.obo").ToDictionary(Function(x) x.id)
        '   Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\GO\C-T-Gene Ontology.csv" _
        '       .LoadCsv(Of EnrichmentTerm) _
        '       .EnrichmentPlot(terms, 0.05, New Size(1600, 800)) _
        '       .SaveAs("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\GO\C-T-Gene Ontology.png")
        '   Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\GO\WT-KO-Gene Ontology.csv" _
        '.LoadCsv(Of EnrichmentTerm) _
        '.EnrichmentPlot(terms, 0.01, New Size(2000, 1800)) _
        '.SaveAs("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\GO\WT-KO-Gene Ontology.png")

        '   Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KEGG\C-T-KEGG PATHWAY.csv" _
        '       .LoadCsv(Of EnrichmentTerm) _
        '       .KEGGEnrichmentPlot(New Size(1000, 600)) _
        '       .SaveAs("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KEGG\C-T-KEGG PATHWAY.png")
        '   Call "C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KEGG\WT-KO-KEGG PATHWAY.csv" _
        '.LoadCsv(Of EnrichmentTerm) _
        '.KEGGEnrichmentPlot(New Size(1000, 600)) _
        '.SaveAs("C:\Users\xieguigang\OneDrive\1.17\4. analysis\enrichment\KEGG\WT-KO-KEGG PATHWAY.png")

        '   Pause()
    End Sub
End Module
