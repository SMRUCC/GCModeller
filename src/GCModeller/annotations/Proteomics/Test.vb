Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner

Module Test

    Sub Main()
        ' Call GetProteinDefs("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK.csv")


        Dim designer As Designer() = {
            New Designer With {.Experiment = "24.C4", .Control = "24.A4"},
            New Designer With {.Experiment = "24.C5", .Control = "24.A5"},
            New Designer With {.Experiment = "24.C6", .Control = "24.A6"},
            New Designer With {.Experiment = "72.C4", .Control = "72.A4"},
            New Designer With {.Experiment = "72.C5", .Control = "72.A5"},
            New Designer With {.Experiment = "72.C6", .Control = "72.A6"},
            New Designer With {.Experiment = "72.A4", .Control = "24.A4"},
            New Designer With {.Experiment = "72.A5", .Control = "24.A5"},
            New Designer With {.Experiment = "72.A6", .Control = "24.A6"},
            New Designer With {.Experiment = "72.C4", .Control = "24.C4"},
            New Designer With {.Experiment = "72.C5", .Control = "24.C5"},
            New Designer With {.Experiment = "72.C6", .Control = "24.C6"}
        }

        Call SMRUCC.genomics.Analysis.Microarray.DEGDesigner.log2("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_GL.csv", designer, "LFQ.intensity.GL").Save("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_GL-DEG-log2.csv")

        Call SMRUCC.genomics.Analysis.Microarray.DEGDesigner.log2("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK.csv", designer, "LFQ.intensity.SK").Save("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK-DEG-log2.csv")


        Pause()
    End Sub
End Module
