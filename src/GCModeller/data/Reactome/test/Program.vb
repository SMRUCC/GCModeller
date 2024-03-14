Imports SMRUCC.genomics.Data.Reactome

Module Program
    Sub Main(args As String())
        Call pathwayTree()

        Console.WriteLine("Hello World!")
    End Sub

    Sub pathwayTree()
        Dim tree = Hierarchy.LoadInternal

        Pause()
    End Sub
End Module
