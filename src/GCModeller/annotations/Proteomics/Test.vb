Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Module Test

    Sub Main()

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


        'Pause()




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
