Imports Microsoft.VisualBasic.Data.GraphTheory
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree

Module mesh_test

    Sub Main()

        Dim tree As Tree(Of Term) = MeSH.Tree.ParseTree("G:\GCModeller\src\workbench\pkg\data\mtrees2024.txt")
        Dim terms = MeSH.DescriptorRecordSet.ReadTerms("C:\Users\Administrator\Downloads\desc2024.xml").ToArray

        Pause()
    End Sub
End Module
