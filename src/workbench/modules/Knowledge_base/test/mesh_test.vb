Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree

Module mesh_test

    Sub Main()

        Dim tree As Term() = MeSH.Tree.ReadTerms("G:\GCModeller\src\workbench\pkg\data\mtrees2024.txt".OpenReadonly).ToArray
        Dim terms = MeSH.DescriptorRecordSet.ReadTerms("C:\Users\Administrator\Downloads\desc2024.xml").ToArray

        tree = tree.JoinData(MeSH.DescriptorRecordSet.TreeTermIndex(terms)).ToArray
        tree.GetJson.SaveTo("Z:\ncbi_mesh.json")

        Pause()
    End Sub
End Module
