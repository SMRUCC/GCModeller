Imports SMRUCC.genomics.Data.Reactome

Module Program
    Sub Main(args As String())
        Call pathwayTree()

        Console.WriteLine("Hello World!")
    End Sub

    Sub pathwayTree()
        Dim hsa_tree = Hierarchy.LoadInternal(tax:="Homo sapiens")

        Pause()
    End Sub
End Module
