Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Orthogonal
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Class NodeRepresentation

    Public Property images As Dictionary(Of String, Image)
    Public Property imagefiles As Dictionary(Of String, String)

    Public Const Representation As String = "representation"

    Dim g As NetworkGraph
    Dim representationKey As String = Representation

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

    Public Function SetGraph(g As NetworkGraph, key As String) As NodeRepresentation
        Me.representationKey = key
        Me.g = g

        Return Me
    End Function

    Public Shared Function LoadFromFolder(dir As String) As NodeRepresentation
        Dim files = dir.ListFiles("*.jpg", "*.png", "*.bmp").ToArray
        Dim imagefiles = files _
            .GroupBy(Function(file) file.BaseName) _
            .ToDictionary(Function(file) file.Key,
                          Function(file)
                              Return file.First.GetFullPath
                          End Function)
        Dim imgs = imagefiles.ToDictionary(Function(a) a.Key,
                                           Function(a)
                                               Return DriverLoad.LoadFromStream(a.Value.OpenReadonly)
                                           End Function)

        Return New NodeRepresentation With {
            .images = imgs,
            .imagefiles = imagefiles
        }
    End Function

    Public Function DrawNodeShape(id As String, g As IGraphics, brush As Brush, radius As Single(), center As PointF) As RectangleF
        Dim node As Node = Me.g.GetElementByID(id)
        Dim imageKey As String = node(representationKey)
        Dim represent As Image = images(imageKey)
        Dim w As Single = 900
        Dim h As Single = 700
        Dim x = center.X - w / 2
        Dim y = center.Y - h / 2
        Dim rect As New RectangleF(New PointF(x, y), New SizeF(w, h))

        Call g.DrawImage(represent, rect)

        Return rect
    End Function

End Class
