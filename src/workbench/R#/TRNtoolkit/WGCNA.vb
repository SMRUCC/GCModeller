Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports any = Microsoft.VisualBasic.Scripting

<Package("WGCNA")>
Module WGCNA

    <ExportAPI("shapeTRN")>
    Public Function FilterRegulation(g As NetworkGraph, WGCNA As WGCNAWeight, Optional threshold As Double = 0.3) As Object
        For Each edge As Edge In g.graphEdges.ToArray
            If WGCNA.GetValue(edge.U.label, edge.V.label) < threshold Then
                Call g.RemoveEdge(edge)
            End If
        Next

        Return g
    End Function

    <ExportAPI("interations")>
    Public Function CorrelationNetwork(g As NetworkGraph, WGCNA As WGCNAWeight, modules As list, Optional threshold As Double = 0.3) As Object
        For Each conn As Weight In WGCNA.AsEnumerable.Where(Function(cn) cn.Weight >= threshold)
            Dim u As Node = g.GetElementByID(conn.FromNode)
            Dim v As Node = g.GetElementByID(conn.ToNode)

            If u Is Nothing OrElse v Is Nothing Then
                Continue For
            End If

            Dim edges As Edge() = g.GetEdges(u, v).SafeQuery.ToArray
            Dim data As New EdgeData
            Dim color1 As String = any.ToString(modules.slots(u.label))
            Dim color2 As String = any.ToString(modules.slots(v.label))

            data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = If(color1 = color2, color1, $"{color1}+{color2}")

            If edges.Length = 0 Then
                g.CreateEdge(u, v, conn.Weight, data)
            Else
                For Each link In edges
                    link.weight += conn.Weight
                Next
            End If
        Next

        Return g
    End Function
End Module
