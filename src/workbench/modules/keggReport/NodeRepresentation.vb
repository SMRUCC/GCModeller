#Region "Microsoft.VisualBasic::0145d4b5f0ec4aa986c0155e10d8cb5c, modules\keggReport\NodeRepresentation.vb"

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

    '   Total Lines: 94
    '    Code Lines: 73 (77.66%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (22.34%)
    '     File Size: 3.32 KB


    ' Class NodeRepresentation
    ' 
    '     Properties: imagefiles, images
    ' 
    '     Function: DrawNodeShape, LoadFromFolder, MakeSubNetwork, SetGraph
    ' 
    ' /********************************************************************************/

#End Region

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
        Dim h As Single = 600
        Dim x = center.X - w / 2
        Dim y = center.Y - h / 2
        Dim rect As New RectangleF(New PointF(x, y), New SizeF(w, h))

        Call g.DrawImage(represent, rect)

        Return rect
    End Function

End Class

