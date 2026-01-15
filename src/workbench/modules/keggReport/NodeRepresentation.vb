Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Orthogonal
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Class NodeRepresentation

    Public Property images As Dictionary(Of String, Image)

    Public Const Representation As String = "representation"

    Dim g As NetworkGraph

    Public Function MakeSubNetwork(pathway As KGMLRender) As NetworkGraph
        g = New NetworkGraph

        For Each id As String In images.Keys
            Dim entry As entry = pathway(id)

            If entry Is Nothing Then
                Continue For
            End If

            Dim v = pathway.graph.GetElementByID("entry_" & entry.id)
            Dim data As NodeData = v.data.Clone

            data(Representation) = id
            g.CreateNode(v.label, data)
        Next

        For Each edge As Edge In pathway.graph.graphEdges
            Dim u = g.GetElementByID(edge.U.label)
            Dim v = g.GetElementByID(edge.V.label)

            If u IsNot Nothing AndAlso v IsNot Nothing Then
                Call g.CreateEdge(u, v, edge.weight, edge.data.Clone)
            End If
        Next

        Call g.ComputeNodeDegrees
        Call g.RemovesIsolatedNodes()
        Call g.ToString.debug
        Call g.AstarRouter

        Return g
    End Function

    Public Function DrawNodeShape(id As String, g As IGraphics, brush As Brush, radius As Single(), center As PointF) As RectangleF
        Dim node As Node = Me.g.GetElementByID(id)
        Dim imageKey As String = node(Representation)
        Dim represent As Image = images(imageKey)
        Dim w As Single = 500
        Dim h As Single = 300
        Dim rect As New RectangleF(center, New SizeF(w, h))

        rect = New RectangleF(center, New SizeF(10, 10))

        ' Call g.DrawImage(represent, rect)
        Call g.FillRectangle(brush, rect)

        Return rect
    End Function

End Class
