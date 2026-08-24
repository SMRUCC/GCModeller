#Region "Microsoft.VisualBasic::13c63152c2f4c0fdfeaa885075d36021, sub-system\CellPhenotype\test\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 41
    '    Code Lines: 31 (75.61%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (24.39%)
    '     File Size: 1.42 KB


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

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

