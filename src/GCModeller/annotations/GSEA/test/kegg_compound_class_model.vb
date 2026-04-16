Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Public Module kegg_compound_class_model

    Sub Main()
        Dim bg = GSEA.KEGG.CompoundBriteBackground()
        Dim idlist = "D:\GCModeller\src\workbench\pkg\data\kegg_lipids.txt".ReadAllLines
        Dim enrichTest = bg.Enrichment(idlist).ToArray

        Call bg.GetXml.SaveTo("./bg.xml")
        Call enrichTest.SaveTo("./lipid_test.csv")

        Pause()
    End Sub
End Module
