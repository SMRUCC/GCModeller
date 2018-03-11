Imports SMRUCC.genomics.GCModeller.Workbench.SeqFeature

Module Module1

    Sub Main()
        Call viewerTest()
    End Sub

    Sub viewerTest()
        Dim sites As New List(Of Site)

        For Each line In "E:\GCModeller\src\workbench\SeqFeature\test.csv".ReadAllLines.Skip(1)
            Dim t = line.Split(","c)
            Dim name = t(0)
            Dim lefts = t(1).Split(" "c)

            For Each x In lefts
                sites.Add(New Site With {.Name = name, .Left = x})
            Next
        Next

        Dim seq = "SERVELAT"

        Call sites.DisplayOn(seq)

        Pause()
    End Sub
End Module
