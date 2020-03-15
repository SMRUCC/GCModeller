Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Module VCellNetwork

    <Extension>
    Public Function CreateGraph(vcell As Vessel) As NetworkGraph
        Dim g As New NetworkGraph

        For Each mass As Factor In vcell.MassEnvironment
            g.AddNode(New Node With {.label = mass.ID})
        Next

        For Each process As Channel In vcell.Channels
            Dim processNode As Node = g.AddNode(New Node With {.label = process.ID})

            For Each mass In process.GetReactants
                g.AddEdge(g.GetElementByID(mass.Mass.ID), processNode, mass.Coefficient)
            Next
            For Each mass In process.GetProducts
                g.AddEdge(processNode, g.GetElementByID(mass.Mass.ID), mass.Coefficient)
            Next
            For Each factor In process.forward.activation
                g.AddEdge(g.GetElementByID(factor.Mass.ID), processNode, factor.Coefficient)
            Next
            For Each factor In process.forward.inhibition
                g.AddEdge(g.GetElementByID(factor.Mass.ID), processNode, factor.Coefficient)
            Next
            For Each factor In process.reverse.activation
                g.AddEdge(g.GetElementByID(factor.Mass.ID), processNode, factor.Coefficient)
            Next
            For Each factor In process.reverse.inhibition
                g.AddEdge(g.GetElementByID(factor.Mass.ID), processNode, factor.Coefficient)
            Next
        Next

        Return g
    End Function
End Module
