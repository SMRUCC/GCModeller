Imports Microsoft.VisualBasic.MIME.application.json
Imports SMRUCC.genomics.Data.Reactome

Module Program
    Sub Main(args As String())
        Call pathwayTree()

        Console.WriteLine("Hello World!")
    End Sub

    Sub pathwayTree()
        Dim hsa_tree = Hierarchy.LoadInternal(tax:="Homo sapiens")
        Dim json As String = Hierarchy.TreeJSON(hsa_tree)

        Call Console.WriteLine(json)
        Call json.SaveTo("./HSA.json")

        Dim json2 As String = HierarchyLink.LoadInternal("Homo sapiens").Values.ToArray.GetJson

        Call json2.SaveTo("./HSA.json")

        Pause()
    End Sub
End Module
