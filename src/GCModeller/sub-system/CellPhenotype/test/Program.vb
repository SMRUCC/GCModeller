Imports SMRUCC.genomics.Analysis.CellPhenotype

Module Program

    Sub Main()
        Dim net As New MetabolicNetwork With {.NodeCount = 5}
        net.Adjacency = New List(Of List(Of (Integer, Double))) From {
            New List(Of (Integer, Double)) From {(1, 1.0)},
            New List(Of (Integer, Double)) From {(2, 1.0), (3, 1.0)},
            New List(Of (Integer, Double)) From {(4, 1.0)},
            New List(Of (Integer, Double)) From {(4, 1.0)},
            New List(Of (Integer, Double)) From {}
        }

        Dim P = BuildRowStochasticMatrix(net)
        Dim steadyState = ComputePPR(P, seedNode:=0)

        For i = 0 To steadyState.Length - 1
            Console.WriteLine($"Metabolite {i}: {steadyState(i):F6}")
        Next
    End Sub

End Module
