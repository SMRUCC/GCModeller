Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public NotInheritable Class TreeGraph(Of K, V)

    Public Shared Function CreateGraph(tree As BinaryTree(Of K, V), uniqueId As Func(Of V, String)) As NetworkGraph
        Dim g As New NetworkGraph
        Call appendGraph(g, tree, uniqueId)
        Return g
    End Function

    Private Shared Sub appendGraph(g As NetworkGraph, tree As BinaryTree(Of K, V), uniqueId As Func(Of V, String))
        Dim clusterId = tree.Key.ToString
        Dim center As Node
        Dim v As Node

        If g.GetElementByID(clusterId) Is Nothing Then
            center = g.CreateNode(clusterId, New NodeData With {.Properties = New Dictionary(Of String, String) From {{NamesOf.REFLECTION_ID_MAPPING_NODETYPE, clusterId}}})
        Else
            center = g.GetElementByID(clusterId)
        End If

        Dim guid As String

        For Each member In tree.Members
            guid = uniqueId(member)

            If g.GetElementByID(guid) Is Nothing Then
                v = g.CreateNode(uniqueId(member), New NodeData With {.Properties = New Dictionary(Of String, String) From {{NamesOf.REFLECTION_ID_MAPPING_NODETYPE, clusterId}}})
            Else
                v = g.GetElementByID(guid)
            End If

            g.CreateEdge(center, v)
        Next

        If Not tree.Left Is Nothing Then
            v = g.CreateNode(tree.Left.Key.ToString, New NodeData With {.Properties = New Dictionary(Of String, String) From {{NamesOf.REFLECTION_ID_MAPPING_NODETYPE, clusterId}}})
            g.CreateEdge(v, center)
            appendGraph(g, tree.Left, uniqueId)
        End If

        If Not tree.Right Is Nothing Then
            v = g.CreateNode(tree.Left.Key.ToString, New NodeData With {.Properties = New Dictionary(Of String, String) From {{NamesOf.REFLECTION_ID_MAPPING_NODETYPE, clusterId}}})
            g.CreateEdge(v, center)
            appendGraph(g, tree.Left, uniqueId)
        End If
    End Sub
End Class
