Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Microarray.DEGDesigner

Module Test

    Sub Main()

        Dim A#() = {175, 168, 168, 190, 156, 181, 182, 175, 174, 179}
        Dim b#() = {185, 169, 173, 173, 188, 186, 175, 174, 179, 180}

        MsgBox(Microsoft.VisualBasic.Mathematical.Statistics.Ttest.Ttest(A, b).GetJson)

        MsgBox(Microsoft.VisualBasic.Mathematical.Statistics.Ttest.Ttest({0R, 1.0R, 1.0R, 1.0R}, 1).GetJson)
        MsgBox(Microsoft.VisualBasic.Mathematical.Statistics.Ttest.Ttest({0, 1, 1, 1}, {1, 2, 2, 2}, -1).Valid)

        ' Call GetProteinDefs("C:\Users\xieguigang\OneDrive\1.5\samples\proteinGroups_SK.csv")
        Dim dd = Microsoft.VisualBasic.Mathematical.Statistics.Ttest.Tcdf(1, 1)
        Call Microsoft.VisualBasic.Mathematical.Statistics.Ttest.Tcdf({-4.0R, -2.0R, 0R, 2.0R, 4.0R}, 5).GetJson.__DEBUG_ECHO


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
