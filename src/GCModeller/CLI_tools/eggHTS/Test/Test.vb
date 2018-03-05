#Region "Microsoft.VisualBasic::2b9145a70f078fefc3b8cf8e7c11e7c1, CLI_tools\eggHTS\Test\Test.vb"

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

    ' Module Test
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.DAVID

Module Test

    Sub Main()


        Call SMRUCC.genomics.Assembly.Uniprot.XML.UniprotXML.EnumerateEntries("G:\GCModeller-repo\uniprot-all.xml\uniprot-all.xml").ToArray

        Dim g As New SMRUCC.genomics.Data.GeneOntology.DAG.Graph("D:\smartnucl_integrative\DATA\go.obo")



        Pause()

        ' 1. 从uniprot下载数据

        ' 2. 生成注释信息，并赋值一个临时的基因号
        'Dim uniprot = UniprotXML.Load("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml").entries.ToDictionary(Function(x) x.accession)
        'Dim mappings = Retrieve_IDmapping.MappingReader("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml-mappingTable.tsv")

        'Dim fields = {"LFQ.intensity.GL.24.A4", "LFQ.intensity.GL.24.A5", "LFQ.intensity.GL.24.A6", "LFQ.intensity.GL.24.C4", "LFQ.intensity.GL.24.C5", "LFQ.intensity.GL.24.C6", "LFQ.intensity.GL.72.A4", "LFQ.intensity.GL.72.A5", "LFQ.intensity.GL.72.A6", "LFQ.intensity.GL.72.C4", "LFQ.intensity.GL.72.C5", "LFQ.intensity.GL.72.C6"}

        'Call "C:\Users\xieguigang\OneDrive\1.5\samples\1. samples\proteinGroups_GL.csv".LoadSample _
        '    .GenerateAnnotations(mappings, uniprot, fields:=fields, prefix:="GL", scientifcName:="Danio rerio").ToArray _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.csv",, "geneID")

        'fields = {"LFQ.intensity.SK.24.A4", "LFQ.intensity.SK.24.A5", "LFQ.intensity.SK.24.A6", "LFQ.intensity.SK.24.C4", "LFQ.intensity.SK.24.C6", "LFQ.intensity.SK.24.C5", "LFQ.intensity.SK.72.A5", "LFQ.intensity.SK.72.A4", "LFQ.intensity.SK.72.A6", "LFQ.intensity.SK.72.C5", "LFQ.intensity.SK.72.C4", "LFQ.intensity.SK.72.C6"}

        'uniprot = UniprotXML.Load("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml").entries.ToDictionary(Function(x) x.accession)
        'mappings = Retrieve_IDmapping.MappingReader("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml-mappingTable.tsv")

        'Call "C:\Users\xieguigang\OneDrive\1.5\samples\1. samples\proteinGroups_SK.csv".LoadSample _
        '    .GenerateAnnotations(mappings, uniprot, fields, prefix:="SK", scientifcName:="Danio rerio").ToArray _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.csv",, "geneID")


        ' 绘制GO图
        'Dim goTerms = GO_OBO.Open("K:\GO_DB\go.obo").ToDictionary(Function(x) x.id)
        'Dim sample = "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.csv".LoadSample

        'Dim data = sample.CountStat(Function(x As EntityObject) x("GO").Split(";"c).Select(AddressOf Trim).ToArray, goTerms)
        'Call CatalogPlots.Plot(data, orderTakes:=20).SaveAs("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\GO\GL.png")
        ''.SaveCountValue("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\GO\GL.csv")

        'sample = "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.csv".LoadSample

        'data = sample.CountStat(Function(x As EntityObject) x("GO").Split(";"c).Select(AddressOf Trim).ToArray, goTerms)
        'Call CatalogPlots.Plot(data, orderTakes:=20).SaveAs("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\GO\SK.png")
        ''.SaveCountValue("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\GO\SK.csv")

        'Pause()


        ' 3. 应用实验设计进行DEP分析

        'Dim designer As Designer() = {
        '    New Designer With {.Experiment = "24.C4", .Control = "24.A4", .GroupLabel = "1.log2(GL24.C/GL24.A)"},
        '    New Designer With {.Experiment = "24.C5", .Control = "24.A5", .GroupLabel = "1.log2(GL24.C/GL24.A)"},
        '    New Designer With {.Experiment = "24.C6", .Control = "24.A6", .GroupLabel = "1.log2(GL24.C/GL24.A)"},
        '    New Designer With {.Experiment = "72.C4", .Control = "72.A4", .GroupLabel = "2.log2(GL72.C/GL72.A)"},
        '    New Designer With {.Experiment = "72.C5", .Control = "72.A5", .GroupLabel = "2.log2(GL72.C/GL72.A)"},
        '    New Designer With {.Experiment = "72.C6", .Control = "72.A6", .GroupLabel = "2.log2(GL72.C/GL72.A)"},
        '    New Designer With {.Experiment = "72.A4", .Control = "24.A4", .GroupLabel = "3.log2(GL72.A/GL24.A)"},
        '    New Designer With {.Experiment = "72.A5", .Control = "24.A5", .GroupLabel = "3.log2(GL72.A/GL24.A)"},
        '    New Designer With {.Experiment = "72.A6", .Control = "24.A6", .GroupLabel = "3.log2(GL72.A/GL24.A)"},
        '    New Designer With {.Experiment = "72.C4", .Control = "24.C4", .GroupLabel = "4.log2(GL72.C/GL24.C)"},
        '    New Designer With {.Experiment = "72.C5", .Control = "24.C5", .GroupLabel = "4.log2(GL72.C/GL24.C)"},
        '    New Designer With {.Experiment = "72.C6", .Control = "24.C6", .GroupLabel = "4.log2(GL72.C/GL24.C)"}
        '}

        'Call DEGDesigner.EdgeR_rawDesigner(
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.csv",
        '    designer,
        '    "LFQ.intensity.GL",
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\GL")

        'designer = {
        '    New Designer With {.Experiment = "24.C4", .Control = "24.A4", .GroupLabel = "5.log2(SK24.C/SK24.A)"},
        '    New Designer With {.Experiment = "24.C5", .Control = "24.A5", .GroupLabel = "5.log2(SK24.C/SK24.A)"},
        '    New Designer With {.Experiment = "24.C6", .Control = "24.A6", .GroupLabel = "5.log2(SK24.C/SK24.A)"},
        '    New Designer With {.Experiment = "72.C4", .Control = "72.A4", .GroupLabel = "6.log2(SK72.C/SK72.A)"},
        '    New Designer With {.Experiment = "72.C5", .Control = "72.A5", .GroupLabel = "6.log2(SK72.C/SK72.A)"},
        '    New Designer With {.Experiment = "72.C6", .Control = "72.A6", .GroupLabel = "6.log2(SK72.C/SK72.A)"},
        '    New Designer With {.Experiment = "72.A4", .Control = "24.A4", .GroupLabel = "7.log2(SK72.A/SK24.A)"},
        '    New Designer With {.Experiment = "72.A5", .Control = "24.A5", .GroupLabel = "7.log2(SK72.A/SK24.A)"},
        '    New Designer With {.Experiment = "72.A6", .Control = "24.A6", .GroupLabel = "7.log2(SK72.A/SK24.A)"},
        '    New Designer With {.Experiment = "72.C4", .Control = "24.C4", .GroupLabel = "8.log2(SK72.C/SK24.C)"},
        '    New Designer With {.Experiment = "72.C5", .Control = "24.C5", .GroupLabel = "8.log2(SK72.C/SK24.C)"},
        '    New Designer With {.Experiment = "72.C6", .Control = "24.C6", .GroupLabel = "8.log2(SK72.C/SK24.C)"}
        '}

        'Call DEGDesigner.EdgeR_rawDesigner(
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.csv",
        '    designer,
        '    "LFQ.intensity.SK",
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\SK")

        'Pause()

        ' 4. 筛选出DEP进行后续的分析操作,以及注释
        'Dim proteins = "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.csv".LoadSample.ToDictionary

        ''"logFC",	"logCPM",	"F",	"PValue"
        ''"geneName",	"fullName",	"uniprot",	"GO",	"EC",	"KO"
        'Call (ls - l - r - "qlfTable.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\GL") _
        '    .ApplyAnnotations("genes", {"logFC", "logCPM", "F", "PValue"}, {"geneName", "fullName", "uniprot", "GO", "EC", "KO"}, proteins)

        'proteins = "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.csv".LoadSample.ToDictionary

        'Call (ls - l - r - "qlfTable.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\SK") _
        '    .ApplyAnnotations("genes", {"logFC", "logCPM", "F", "PValue"}, {"geneName", "fullName", "uniprot", "GO", "EC", "KO"}, proteins)
        'Pause()

        ' 5. DEP 概览heatmap
        'Call DEGsStatMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\GL", "qlfTable.csv", DEP:=True) _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_GL.logFC-overviews.csv",, "design")
        'Call DEGsStatMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\SK", "qlfTable.csv", DEP:=True) _
        '    .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK.logFC-overviews.csv",, "design")

        ' 绘制蛋白质的热图

        Call DEGDesigner.MergeDEPMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\GL", "*-qlfTable-DEPs-annotations.csv") _
            .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\DEPs_heatmap\GL.csv",, "geneID", 0)

        Call DEGDesigner.MergeDEPMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\SK", "*-qlfTable-DEPs-annotations.csv") _
            .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\DEPs_heatmap\SK.csv",, "geneID", 0)

        Pause()

        ' 样品之间的DEPs的文世图

        'Call (ls - l - r - "*qlfTable-DEPs-annotations.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\GL").VennData.Save("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\venn\GL.csv")
        'Call (ls - l - r - "*qlfTable-DEPs-annotations.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\SK").VennData.Save("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\venn\SK.csv")

        'Pause()


        '6. 导出KEGG颜色代码,并构建代谢网络图

        'WebServiceUtils.Proxy = "http://127.0.0.1:8087"

        'Call ExportColorDEGs("C:\Users\xieguigang\OneDrive\1.5\samples\4. analysis\KEGG pathways")

        'For Each file As String In ls - l - r - "*.txt" <= "C:\Users\xieguigang\OneDrive\1.5\samples\4. analysis\KEGG pathways"
        '    Call SMRUCC.genomics.Assembly.KEGG.WebServices.PathwayMapping.ColorPathway(file, work:=file.TrimSuffix)
        'Next

        'Pause()


        ' 7. 为每一个DEP结果导出相应的uniprot编号，进行富集分析 

        'For Each file In ls - l - r - "*.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\4. analysis\enrichment\KOBAS"
        '    Call file.LoadSample("uniprot").Select(Function(g) g.ID).Distinct.SaveTo(file.TrimSuffix & "-uniprot.txt")
        'Next


        ' 处理DAVID数据

        For Each file In ls - l - r - "*.txt" <= "C:\Users\xieguigang\OneDrive\1.5\samples\4. analysis\enrichment\"
            Dim table = SMRUCC.genomics.Analysis.Microarray.DAVID.Load(file)
            Dim name As String = file.BaseName

            Dim GO = table.SelectGoTerms()
            Dim KEGG = table.SelectKEGGPathway

            Call GO.SaveTo($"{file.ParentPath}/GO/{name}.csv")
            Call KEGG.SaveTo($"{file.ParentPath}/KEGG_PATH/{name}.csv")

            Call GO.EnrichmentPlot().Save($"{file.ParentPath}/GO/{name}-enrichment.png")
            '   Call KEGG.KEGGEnrichmentPlot(size:=New Size(1000, 750)).Save($"{file.ParentPath}/KEGG_PATH/{name}-enrichment.png")


        Next

        Pause()




        'Call Plots.GOEnrichmentPlot(
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\4. analysis\enrichment\GO\proteinGroups_GL-2.log2(GL72.C_GL72.A)-qlfTable-DEGs-annotations-uniprot-KOBAS-GO.tsv".LoadTsv(Of EnrichmentTerm),
        '    "K:\GO_DB\go.obo").SaveAs("x:\test.png")

        'Pause()

        'For Each file In ls - l - r - "*.txt" <= "C:\Users\xieguigang\OneDrive\1.5\samples\4. analysis\enrichment\KOBAS"
        '    Call KOBAS.SplitData(file)
        'Next

        'Pause()


        'WebServiceUtils.Proxy = "http://127.0.0.1:8087"

        'Call SMRUCC.genomics.Assembly.KEGG.WebServices.PathwayMapping.Reconstruct(
        '     "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL-KO.txt",, "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL-KO/")

        'Call SMRUCC.genomics.Assembly.KEGG.WebServices.PathwayMapping.Reconstruct(
        '     "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK-KO.txt",, "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK-KO/")

        'Pause()

        'Call "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.csv".LoadSample.GetKOlist.SaveTo("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL-KO.txt")
        'Call "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.csv".LoadSample.GetKOlist.SaveTo("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK-KO.txt")

        'Pause()




        'Call MergeMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK", "qlfTable.csv").SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK.logFC.csv",, "geneID")


        'Dim uniprotDEGs As String() = Nothing

        'Call (ls - l - r - "qlfTable.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_GL").ApplyDEPsAnnotations(
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml-mappingTable.tsv",
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml", "genes", "GL", geneList:=uniprotDEGs)

        'Call uniprotDEGs.SaveTo("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_GL-uniprot.txt")

        'Call (ls - l - r - "qlfTable.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK").ApplyDEPsAnnotations(
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml-mappingTable.tsv",
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml", "genes", "SK", geneList:=uniprotDEGs)

        'Call uniprotDEGs.SaveTo("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK-uniprot.txt")

        'Call (ls - l - r - "qlfTable.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_GL").ApplyAnnotations(
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml-mappingTable.tsv",
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml", "genes", "GL")

        'Call (ls - l - r - "qlfTable.csv" <= "C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK").ApplyAnnotations(
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml-mappingTable.tsv",
        '    "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml", "genes", "SK")

        'Pause()


    End Sub
End Module
