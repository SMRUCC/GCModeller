Imports SMRUCC.genomics.Analysis.CellPhenotype

Module Program

    Sub Main()
        Dim net As New MetabolicNetwork
        net.MetaIDs = {"A", "B", "C", "D", "E", "F"}
        net.Adjacency = New Dictionary(Of String, AdjacencyWeight()) From {
            {"A", {New AdjacencyWeight("B", 1), New AdjacencyWeight("D", 3)}},
            {"B", {New AdjacencyWeight("C", 2)}},
            {"C", {New AdjacencyWeight("D", 5), New AdjacencyWeight("F", 3), New AdjacencyWeight("E", 1)}},
            {"D", {New AdjacencyWeight("F", 1)}},
            {"E", {New AdjacencyWeight("F", 1)}},
            {"F", {New AdjacencyWeight("B")}}
        }

        Dim steadyState = ComputePPR(net, seedNode:=0)
        Dim P2 = PPRSolver.ComputeSteadyStateClosed(net, 999)
        Dim P3 = PPRSolver.SolveWithDrain(net, seedNode:=0, {0.1, 0.1, 0.1, 0.1, 0.1, 0.1})

        For i = 0 To steadyState.Length - 1
            Console.WriteLine($"[PPR] Metabolite {i}: {steadyState(i):F6}")
        Next

        Call Console.WriteLine()
        Call Console.WriteLine()

        For i = 0 To P2.Length - 1
            Console.WriteLine($"[Steady] Metabolite {i}: {P2(i):F6}")
        Next

        Call Console.WriteLine()
        Call Console.WriteLine()


        For i = 0 To P3.Length - 1
            Console.WriteLine($"[Drain] Metabolite {i}: {P3(i):F6}")
        Next
    End Sub

End Module
