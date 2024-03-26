Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

Module name_test

    Sub Main()

        Dim groups = GeneName.GroupBy(EntityObject.LoadDataSet("E:\GCModeller\src\workbench\modules\Knowledge_base\interested_genes.csv"), "description").ToArray

        Pause()
    End Sub
End Module
