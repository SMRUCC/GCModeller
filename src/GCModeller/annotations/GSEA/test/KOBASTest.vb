Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Module KOBASTest

    Sub Main()

        Dim genelist = {"A", "B", "C", "D", "E", "F", "G"}
        Dim background As Index(Of String) = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N"}

        Dim name = {"test"}

        Dim m = KOBAS_GSEA.get_hit_matrix(genelist, genelist.Length, name, {}, {background}, 3, 1000)

        Pause()
    End Sub
End Module
