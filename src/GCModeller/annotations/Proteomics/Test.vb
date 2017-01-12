Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Assembly.Uniprot.Web

Module Test

    Sub Main()

        'Call Retrieve_IDmapping.GetMappingList("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml-mappingTable.tsv").SaveTo("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.uniprot_list.txt")
        'Call Retrieve_IDmapping.GetMappingList("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml-mappingTable.tsv").SaveTo("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.uniprot_list.txt")
        'Pause()

        'Call MergeMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK", "qlfTable.csv").SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK.logFC.csv",, "geneID")
        Call DEGsStatMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_GL", "qlfTable.csv").SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_GL.logFC-overviews.csv",, "geneID")
        Call DEGsStatMatrix("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK", "qlfTable.csv").SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\3. DEGs\proteinGroups_SK.logFC-overviews.csv",, "geneID")

        Pause()

        Call "C:\Users\xieguigang\OneDrive\1.5\samples\1. samples\proteinGroups_GL.csv".LoadSample.Select(Function(x) x.Identifier) _
            .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml-mappingTable.tsv",
                                 "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.xml", prefix:="GL").ToArray _
                                 .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_GL.csv",, "geneID")

        Call "C:\Users\xieguigang\OneDrive\1.5\samples\1. samples\proteinGroups_SK.csv".LoadSample.Select(Function(x) x.Identifier) _
            .GenerateAnnotations("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml-mappingTable.tsv",
                                 "C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.xml", prefix:="SK").ToArray _
                                 .SaveDataSet("C:\Users\xieguigang\OneDrive\1.5\samples\2. annotations\proteinGroups_SK.csv",, "geneID")


        Pause()

        Dim designer As Designer() = {
            New Designer With {.Experiment = "24.C4", .Control = "24.A4", .GroupLabel = "1.log2(GL24.C/GL24.A)"},
            New Designer With {.Experiment = "24.C5", .Control = "24.A5", .GroupLabel = "1.log2(GL24.C/GL24.A)"},
            New Designer With {.Experiment = "24.C6", .Control = "24.A6", .GroupLabel = "1.log2(GL24.C/GL24.A)"},
            New Designer With {.Experiment = "72.C4", .Control = "72.A4", .GroupLabel = "2.log2(GL72.C/GL72.A)"},
            New Designer With {.Experiment = "72.C5", .Control = "72.A5", .GroupLabel = "2.log2(GL72.C/GL72.A)"},
            New Designer With {.Experiment = "72.C6", .Control = "72.A6", .GroupLabel = "2.log2(GL72.C/GL72.A)"},
            New Designer With {.Experiment = "72.A4", .Control = "24.A4", .GroupLabel = "3.log2(GL72.A/GL24.A)"},
            New Designer With {.Experiment = "72.A5", .Control = "24.A5", .GroupLabel = "3.log2(GL72.A/GL24.A)"},
            New Designer With {.Experiment = "72.A6", .Control = "24.A6", .GroupLabel = "3.log2(GL72.A/GL24.A)"},
            New Designer With {.Experiment = "72.C4", .Control = "24.C4", .GroupLabel = "4.log2(GL72.C/GL24.C)"},
            New Designer With {.Experiment = "72.C5", .Control = "24.C5", .GroupLabel = "4.log2(GL72.C/GL24.C)"},
            New Designer With {.Experiment = "72.C6", .Control = "24.C6", .GroupLabel = "4.log2(GL72.C/GL24.C)"}
        }

        Call SMRUCC.genomics.Analysis.Microarray.DEGDesigner.log2("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_GL.csv", designer, "LFQ.intensity.GL").Save("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_GL-DEG-log2.csv")
        Call DEGDesigner.EdgeR_rawDesigner("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_GL.csv", designer, "LFQ.intensity.GL", "C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_GL/")

        designer = {
            New Designer With {.Experiment = "24.C4", .Control = "24.A4", .GroupLabel = "5.log2(SK24.C/SK24.A)"},
            New Designer With {.Experiment = "24.C5", .Control = "24.A5", .GroupLabel = "5.log2(SK24.C/SK24.A)"},
            New Designer With {.Experiment = "24.C6", .Control = "24.A6", .GroupLabel = "5.log2(SK24.C/SK24.A)"},
            New Designer With {.Experiment = "72.C4", .Control = "72.A4", .GroupLabel = "6.log2(SK72.C/SK72.A)"},
            New Designer With {.Experiment = "72.C5", .Control = "72.A5", .GroupLabel = "6.log2(SK72.C/SK72.A)"},
            New Designer With {.Experiment = "72.C6", .Control = "72.A6", .GroupLabel = "6.log2(SK72.C/SK72.A)"},
            New Designer With {.Experiment = "72.A4", .Control = "24.A4", .GroupLabel = "7.log2(SK72.A/SK24.A)"},
            New Designer With {.Experiment = "72.A5", .Control = "24.A5", .GroupLabel = "7.log2(SK72.A/SK24.A)"},
            New Designer With {.Experiment = "72.A6", .Control = "24.A6", .GroupLabel = "7.log2(SK72.A/SK24.A)"},
            New Designer With {.Experiment = "72.C4", .Control = "24.C4", .GroupLabel = "8.log2(SK72.C/SK24.C)"},
            New Designer With {.Experiment = "72.C5", .Control = "24.C5", .GroupLabel = "8.log2(SK72.C/SK24.C)"},
            New Designer With {.Experiment = "72.C6", .Control = "24.C6", .GroupLabel = "8.log2(SK72.C/SK24.C)"}
        }

        Call SMRUCC.genomics.Analysis.Microarray.DEGDesigner.log2("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK.csv", designer, "LFQ.intensity.SK").Save("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK-DEG-log2.csv")
        Call DEGDesigner.EdgeR_rawDesigner("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK.csv", designer, "LFQ.intensity.SK", "C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK/")

        Pause()
    End Sub
End Module
