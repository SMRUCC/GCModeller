Imports Microsoft.VisualBasic.Data.Framework.IO
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Module name_test

    Sub Main()

        Dim groups = GeneName _
            .GroupBy(EntityObject.LoadDataSet("E:\GCModeller\src\workbench\modules\Knowledge_base\interested_genes.csv"), "description") _
            .OrderByDescending(Function(c) c.Length) _
            .ToArray

        Pause()
    End Sub
End Module
